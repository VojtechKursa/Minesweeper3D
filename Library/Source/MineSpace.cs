using System;

namespace Minesweeper_3D_Library
{
    /// <summary>
    /// Represents the playing area of the Minesweeper game. (Equivalent to the minefield in the 2D version)
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
        /// Creates a new instance of <see cref="Minespace"/>.
        /// </summary>
        /// <param name="width">The width (the amount of X coordinates) of the new <see cref="MineSpace"/>.</param>
        /// <param name="height">The height (the amount of Y coordinates) of the new <see cref="MineSpace"/>.</param>
        /// <param name="depth">The depth (the amount of Z coordinates) of the new <see cref="MineSpace"/>.</param>
        /// <param name="mineCount">The amount of mines this new minespace should contain.</param>
        public MineSpace(int width, int height, int depth, int mineCount)
        {
            Cubes = new Cube[width, height, depth];

            ActualMineCount = FillField(mineCount);
            InitCubes();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a <see cref="Cube"/> that's at the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the desired cube.</param>
        /// <param name="y">The Y coordinate of the desired cube.</param>
        /// <param name="z">The Z coordinate of the desired cube.</param>
        /// <returns>The cube at the given position.</returns>
        public Cube GetCube(int x, int y, int z)
        {
            return Cubes[x, y, z];
        }

        /// <summary>
        /// Finds the position of the given <see cref="Cube"/> in the <see cref="Cubes"/> array.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> to find.</param>
        /// <returns>An array of <see cref="int"/> containing the [x,y,z] coordinates of the given <see cref="Cube"/>, or <see cref="null"/> if the <see cref="Cube"/> is not found.</returns>
        internal int[] GetCubePosition(Cube cube)
        {
            for (int z = 0; z < Depth; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (Cubes[x, y, z] == cube)
                            return new int[] { x, y, z };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the amount of mines surrounding a given position.
        /// </summary>
        /// <param name="position">The position around which to search for mines.</param>
        /// <returns>The amount of mines surrounding the given position.</returns>
        internal byte GetSurroundingMines(int[] position)
        {
            byte surroundingMines = 0;

            int[] lowerLimits = new int[3];
            for (int i = 0; i < lowerLimits.Length; i++)
            {
                lowerLimits[i] = position[i] - 1 < 0 ? 0 : position[i] - 1;
            }

            int[] upperLimits = new int[3];
            upperLimits[0] = position[0] + 1 >= Width ? Width : position[0] + 2;
            upperLimits[1] = position[1] + 1 >= Height ? Height : position[1] + 2;
            upperLimits[2] = position[2] + 1 >= Depth ? Depth : position[2] + 2;


            for (int z = lowerLimits[2]; z < upperLimits[2]; z++)
            {
                for (int y = lowerLimits[1]; y < upperLimits[1]; y++)
                {
                    for (int x = lowerLimits[0]; x < upperLimits[0]; x++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            continue;

                        if (Cubes[position[0] + x, position[1] + y, position[2] + z].HasMine)
                            surroundingMines++;
                    }
                }
            }

            return surroundingMines;
        }

        /// <summary>
        /// Gets the amount of mines surrounding a given <see cref="Cube"/>.
        /// </summary>
        /// <param name="cube">The <see cref="Cube"/> around which to search for mines.</param>
        /// <returns>The amount of mines surrounding the given <see cref="Cube"/>.</returns>
        internal byte GetSurroundingMines(Cube cube)
        {
            int[] cubePosition;

            if (Cubes[cube.Position[0], cube.Position[1], cube.Position[2]] == cube)
                cubePosition = cube.Position;
            else
                cubePosition = GetCubePosition(cube);

            return GetSurroundingMines(cubePosition);
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
    }
}
