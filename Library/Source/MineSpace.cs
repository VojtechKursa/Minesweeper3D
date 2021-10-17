using Minesweeper3D.Library.Exceptions;
using System;
using System.Collections.Generic;

namespace Minesweeper3D.Library
{
    /// <summary>
    /// Represents the playing area of the Minesweeper game. (Equivalent to the minefield in the 2D version)<br />
    /// Contains basic methods and properties for getting information about and interacting with the playing area.
    /// </summary>
    public class MineSpace
    {
        #region Variables and Properties

        /// <summary>
        /// Gets an array containing all of the <see cref="Cubes"/> in the <see cref="MineSpace"/>.
        /// </summary>
        public Cube[,,] Cubes { get; }

        /// <summary>
        /// Gets the width (the amount of X coordinates) of the <see cref="MineSpace"/>.
        /// </summary>
        public int Width { get => Cubes.GetLength(0); }

        /// <summary>
        /// Gets the height (the amount of Y coordinates) of the <see cref="MineSpace"/>.
        /// </summary>
        public int Height { get => Cubes.GetLength(1); }

        /// <summary>
        /// Gets the depth (the amount of Z coordinates) of the <see cref="MineSpace"/>.
        /// </summary>
        public int Depth { get => Cubes.GetLength(2); }

        /// <summary>
        /// Gets the actual number of mines present in the <see cref="MineSpace"/>.
        /// </summary>
        public int ActualMineCount { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="MineSpace"/>.<br />
        /// Throws an <see cref="ArgumentException"/> when any of width, depth or height is less than 1 or when mineCount is less than 0.
        /// </summary>
        /// <param name="width">The width (the amount of X coordinates) of the new <see cref="MineSpace"/>.</param>
        /// <param name="height">The height (the amount of Y coordinates) of the new <see cref="MineSpace"/>.</param>
        /// <param name="depth">The depth (the amount of Z coordinates) of the new <see cref="MineSpace"/>.</param>
        /// <param name="mineCount">The amount of mines this new <see cref="MineSpace"/> should contain.</param>
        /// <exception cref="ArgumentException"/>
        public MineSpace(int width, int height, int depth, int mineCount)
        {
            ArgumentException exception = CheckValues(width, height, depth, mineCount);
            if (exception != null)
                throw exception;

            Cubes = new Cube[width, height, depth];

            ActualMineCount = FillField(mineCount);
            InitCubes();
        }

        #endregion

        #region Methods
        #region Public

        /// <summary>
        /// Returns a <see cref="Cube"/> that's at the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the desired cube.</param>
        /// <param name="y">The Y coordinate of the desired cube.</param>
        /// <param name="z">The Z coordinate of the desired cube.</param>
        /// <returns>The cube at the given position.</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        public Cube GetCube(int x, int y, int z)
        {
            try
            {
                return Cubes[x, y, z];
            }
            catch
            {
                throw new InvalidCoordinatesException();
            }
        }

        /// <summary>
        /// Returns an array of <see cref="Cube"/>s surrounding the given <see cref="Cube"/>.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> around which the scan will be performed.</param>
        /// <returns>An array of <see cref="Cube"/>s surrounding the given <see cref="Cube"/>.</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        public Cube[] GetSurroundingCubes(Cube cube)
        {
            return GetSurroundingCubes(cube.Position[0], cube.Position[1], cube.Position[2]);
        }

        /// <summary>
        /// Returns an array of <see cref="Cube"/>s surrounding the given position.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>An array of <see cref="Cube"/>s surrounding the given position.</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        public Cube[] GetSurroundingCubes(int x, int y, int z)
        {
            if (x >= Width || y >= Height || z >= Depth || x < 0 || y < 0 || z < 0)
                throw new InvalidCoordinatesException();

            int[] lowerLimits = new int[3];
            lowerLimits[0] = x - 1 < 0 ? 0 : x - 1;
            lowerLimits[1] = y - 1 < 0 ? 0 : y - 1;
            lowerLimits[2] = z - 1 < 0 ? 0 : z - 1;

            int[] upperLimits = new int[3];
            upperLimits[0] = x + 1 >= Width ? Width : x + 2;
            upperLimits[1] = y + 1 >= Height ? Height : y + 2;
            upperLimits[2] = z + 1 >= Depth ? Depth : z + 2;

            List<Cube> cubes = new List<Cube>();

            for (int zz = lowerLimits[2]; zz < upperLimits[2]; zz++)
            {
                for (int yy = lowerLimits[1]; yy < upperLimits[1]; yy++)
                {
                    for (int xx = lowerLimits[0]; xx < upperLimits[0]; xx++)
                    {
                        if (xx== x && yy==y && zz== z)
                            continue;
                        else
                            cubes.Add(GetCube(xx, yy, zz));
                    }
                }
            }

            return cubes.ToArray();
        }

        /// <summary>
        /// Attempts to uncover the given <see cref="Cube"/>.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> to uncover.</param>
        /// <returns>The result of the uncovering operation as an <see cref="UncoverResult"/>.</returns>
        public static UncoverResult Uncover(Cube cube)
        {
            return cube.Uncover();
        }

        /// <summary>
        /// Attempts to uncover a <see cref="Cube"/> at the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>The result of the uncovering operation as an <see cref="UncoverResult"/>.</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        public UncoverResult Uncover(int x, int y, int z)
        {
            return Uncover(GetCube(x, y, z));
        }

        /// <summary>
        /// Attempts to flag the given <see cref="Cube"/>.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> to flag.</param>
        /// <returns>True if the flagging was successful, otherwise false (if the <see cref="Cube"/> was already <see cref="CubeState.Flagged"/> or <see cref="CubeState.Uncovered"/>).</returns>
        public static bool Flag(Cube cube)
        {
            return cube.Flag();
        }

        /// <summary>
        /// Attempts to flag a <see cref="Cube"/> at the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>True if the flagging was successful, otherwise false (if the <see cref="Cube"/> was already <see cref="CubeState.Flagged"/> or <see cref="CubeState.Uncovered"/>).</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        public bool Flag(int x, int y, int z)
        {
            return Flag(GetCube(x, y, z));
        }

        /// <summary>
        /// Attempts to unflag the given <see cref="Cube"/>.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> to unflag.</param>
        /// <returns>True if the unflagging was successful, otherwise false (if the <see cref="Cube"/> was not <see cref="CubeState.Flagged"/>).</returns>
        public static bool Unflag(Cube cube)
        {
            return cube.Unflag();
        }

        /// <summary>
        /// Attempts to unflag a <see cref="Cube"/> at the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>True if the unflagging was successful, otherwise false (if the <see cref="Cube"/> was not <see cref="CubeState.Flagged"/>).</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        public bool Unflag(int x, int y, int z)
        {
            return Unflag(GetCube(x, y, z));
        }

        /// <summary>
        /// Attempts to change the flag state of the given <see cref="Cube"/>.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> whose flag state to change.</param>
        /// <returns>The result of the action as a <see cref="FlagResult"/>.</returns>
        public static FlagResult ChangeFlag(Cube cube)
        {
            return cube.ChangeFlag();
        }

        /// <summary>
        /// Attempts to change the flag state of a <see cref="Cube"/> at the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>The result of the action as a <see cref="FlagResult"/>.</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        public FlagResult ChangeFlag(int x, int y, int z)
        {
            return ChangeFlag(GetCube(x, y, z));
        }

        #endregion

        #region Non public

        /// <summary>
        /// Checks the initial values of the <see cref="MineSpace"/>.
        /// </summary>
        /// <param name="width">The width parameter.</param>
        /// <param name="height">The height parameter.</param>
        /// <param name="depth">The depth parameter.</param>
        /// <param name="mineCount">The mineCount parameter.</param>
        /// <returns>An <see cref="ArgumentException"/> with a message containing an error description if any of the values are invalid, otherwise null.</returns>
        internal static ArgumentException CheckValues(int width, int height, int depth, int mineCount)
        {
            if (width < 1)
                return new ArgumentException(nameof(width) + " cannot be lower than 1.");
            else if (height < 1)
                return new ArgumentException(nameof(height) + " cannot be lower than 1.");
            else if (depth < 1)
                return new ArgumentException(nameof(depth) + " cannot be lower than 1.");
            else if (mineCount < 0)
                return new ArgumentException(nameof(mineCount) + " cannot be lower than 0.");
            else
                return null;
        }

        /// <summary>
        /// Finds the position of the given <see cref="Cube"/> in the <see cref="Cubes"/> array.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> to find.</param>
        /// <returns>An array of <see cref="int"/> containing the [x,y,z] coordinates of the given <see cref="Cube"/>, or throws a <see cref="CubeNotFoundException"/> if the <see cref="Cube"/> is not found.</returns>
        /// <exception cref="CubeNotFoundException"/>
        internal int[] GetCubePosition(Cube cube)
        {
            for (int z = 0; z < Depth; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (GetCube(x, y, z) == cube)
                            return new int[] { x, y, z };
                    }
                }
            }

            throw new CubeNotFoundException();
        }

        /// <summary>
        /// Gets the amount of mines surrounding a given <see cref="Cube"/>.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> around which to search for mines.</param>
        /// <returns>The amount of mines surrounding the given <see cref="Cube"/>.</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        /// <exception cref="CubeNotFoundException"/>
        internal byte GetSurroundingMines(Cube cube)
        {
            int[] cubePosition;

            if (GetCube(cube.Position[0], cube.Position[1], cube.Position[2]) == cube)
                cubePosition = cube.Position;
            else
                cubePosition = GetCubePosition(cube);

            return GetSurroundingMines(cubePosition[0], cubePosition[1], cubePosition[2]);
        }

        /// <summary>
        /// Gets the amount of mines surrounding a given position.
        /// </summary>
        /// <param name="position">The position around which to search for mines.</param>
        /// <returns>The amount of mines surrounding the given position.</returns>
        /// <exception cref="InvalidCoordinatesException"/>
        internal byte GetSurroundingMines(int x, int y, int z)
        {
            Cube[] cubes = GetSurroundingCubes(x, y, z);

            byte surroundingMines = 0;

            foreach(Cube cube in cubes)
            {
                if (cube.HasMine)
                    surroundingMines++;
            }

            return surroundingMines;
        }

        private int FillField(int mineCount)
        {
            int minesCreated = FillMines(mineCount);

            for (int z = 0; z < Depth; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (Cubes[x, y, z] == null)
                            Cubes[x, y, z] = new Cube(this, false, new int[] { x, y, z });
                    }
                }
            }

            return minesCreated;
        }

        private int FillMines(int mineCount)
        {
            Random random = new Random();
            int limit = mineCount < Width * Height * Depth ? mineCount : Width * Height * Depth;

            int[] coords;
            for (int i = 0; i < limit; i++)
            {
                coords = new int[] { random.Next(0, Width), random.Next(0, Height), random.Next(0, Depth) };

                if (Cubes[coords[0], coords[1], coords[2]] == null)
                    Cubes[coords[0], coords[1], coords[2]] = new Cube(this, true, new int[] { coords[0], coords[1], coords[2] });
                else
                    i--;
            }

            return limit;
        }

        private void InitCubes()
        {
            for (int z = 0; z < Depth; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Cubes[x, y, z].Init();
                    }
                }
            }
        }

        #endregion
        #endregion
    }
}
