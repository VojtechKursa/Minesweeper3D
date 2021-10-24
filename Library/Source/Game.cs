using Minesweeper3D.Library.Exceptions;
using System;
using System.Collections.Generic;

namespace Minesweeper3D.Library
{
    /// <summary>
    /// Represents an extension of the <see cref="Library.MineSpace"/> class.<br />
    /// Tracks game's status, elapsed time, provides <see cref="Uncover(int, int, int)"/> method that uncovers surrounding <see cref="Cube"/>s if the uncovered <see cref="Cube"/> has 0 <see cref="Cube.SurroundingMines"/> etc.
    /// </summary>
    public class Game
    {
        #region Variables and Properties
        #region Public

        /// <summary>
        /// Gets the <see cref="Minesweeper3D.Library.MineSpace"/> used by this <see cref="Game"/>.
        /// </summary>
        public MineSpace MineSpace { get; }

        /// <summary>
        /// Gets the total amount of cubes in the <see cref="MineSpace"/>.
        /// </summary>
        public int CubeCount { get; }

        /// <summary>
        /// Gets (or sets) the current status of the <see cref="Game"/>.
        /// </summary>
        public GameStatus Status { get; protected set; } = GameStatus.NotStarted;

        /// <summary>
        /// Gets the time that has elapsed since the start of the game.<br />
        /// If the game hasn't started yet, returns null.<br />
        /// If the game is already over, returns the time the game took up.
        /// </summary>
        public TimeSpan? ElapsedTime
        {
            get
            {
                if (gameStartTime == null)
                    return null;
                else if (gameEndTime == null)
                    return (TimeSpan?)new TimeSpan(DateTime.Now.Ticks - ((DateTime)gameStartTime).Ticks);
                else
                    return (TimeSpan?)new TimeSpan(((DateTime)gameEndTime).Ticks - ((DateTime)gameStartTime).Ticks);
            }
        }

        /// <summary>
        /// Gets the amount of <see cref="CubeState.Flagged"/> <see cref="Cube"/>s in the <see cref="MineSpace"/>.
        /// </summary>
        public int Flagged { get; private set; }

        /// <summary>
        /// Gets the amount of mined <see cref="Cube"/>s in the <see cref="MineSpace"/>.
        /// </summary>
        public int MineCount { get => MineSpace.ActualMineCount; }

        /// <summary>
        /// Gets the amount of successully <see cref="CubeState.Uncovered"/> <see cref="Cube"/>s in the <see cref="MineSpace"/>.
        /// </summary>
        public int Cleared { get; private set; }

        #endregion

        #region Not public

        private DateTime? gameStartTime;
        private DateTime? gameEndTime;

        #endregion
        #endregion

        #region Constructors

        /// <summary>
        /// Initiates a new <see cref="Game"/> instance.<br />
        /// Throws an <see cref="ArgumentException"/> when any of width, depth or height is less than 1 or when mineCount is less than 0.
        /// </summary>
        /// <param name="width">The width (the amount of X coordinates) of the internal <see cref="Minesweeper3D.Library.MineSpace"/>.</param>
        /// <param name="height">The height (the amount of Y coordinates) of the internal <see cref="Minesweeper3D.Library.MineSpace"/>.</param>
        /// <param name="depth">The depth (the amount of Z coordinates) of the internal <see cref="Minesweeper3D.Library.MineSpace"/>.</param>
        /// <param name="mineCount">The amount of mines this internal <see cref="Minesweeper3D.Library.MineSpace"/> should contain.</param>
        /// <exception cref="ArgumentException"/>
        public Game(int width, int height, int depth, int mineCount)
        {
            MineSpace = new MineSpace(width, height, depth, mineCount);

            CubeCount = MineSpace.Width * MineSpace.Height * MineSpace.Depth;
        }

        #endregion

        #region Methods
        #region Public

        /// <summary>
        /// Starts the game.<br />
        /// <br />
        /// If the <see cref="Game"/>'s <see cref="Status"/> is <see cref="GameStatus.NotStarted"/>, starts the timer, sets the <see cref="Status"/> to <see cref="GameStatus.Ongoing"/> and returns true.<br />
        /// Otherwise returns false.
        /// </summary>
        /// <returns>True if the game was sucessfully started, otherwise false.</returns>
        public bool StartGame()
        {
            if (Status == GameStatus.NotStarted)
            {
                gameStartTime = DateTime.Now;
                Status = GameStatus.Ongoing;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// <inheritdoc cref="MineSpace.Uncover(int, int, int)"/><br />
        /// If the number of <see cref="Cube.SurroundingMines"/> of the uncovered <see cref="Cube"/> is 0, starts uncovering all of the safe <see cref="Cube"/>s around it.<br />
        /// Updates the <see cref="Cleared"/> property and evaluates the <see cref="Status"/>.<br />
        /// <br />
        /// If the uncovered <see cref="Cube"/> was mined, the <see cref="Game"/>'s <see cref="Status"/> is set to <see cref="GameStatus.Lost"/> and timer is stopped.<br />
        /// If the <see cref="MineSpace"/> has been completely cleared, the <see cref="Game"/>'s <see cref="Status"/> is set to <see cref="GameStatus.Won"/> and timer is stopped.<br />
        /// If the <see cref="Game"/>'s <see cref="Status"/> is <see cref="GameStatus.NotStarted"/>, the <see cref="StartGame"/> is called and then method continues as usual.<br />
        /// If the <see cref="Game"/>'s <see cref="Status"/> is either <see cref="GameStatus.Won"/> or <see cref="GameStatus.Lost"/>, a <see cref="GameOverException"/> is thrown.
        /// </summary>
        /// <exception cref="GameOverException"/>
        /// <inheritdoc cref="MineSpace.Uncover(int, int, int)"/>
        public UncoverResult Uncover(int x, int y, int z)
        {
            if (Status == GameStatus.NotStarted)
                StartGame();

            if (Status == GameStatus.Ongoing)
            {
                Cube cube = MineSpace.GetCube(x, y, z);
                UncoverResult uncoverResult = cube.Uncover();

                if (uncoverResult == UncoverResult.Mine)
                    Lose();
                else if (uncoverResult == UncoverResult.Clear)
                {
                    Cleared++;

                    if (cube.SurroundingMines == 0)
                    {
                        if (UncoverSafe(cube) > 0)
                            uncoverResult = UncoverResult.ClearMultiple;
                    }

                    if (Cleared == CubeCount - MineCount)
                        Win();
                }

                return uncoverResult;
            }
            else
                throw new GameOverException();
        }

        /// <summary>
        /// <inheritdoc cref="MineSpace.ChangeFlag(int, int, int)"/><br />
        /// Updates the <see cref="Flagged"/> property.<br />
        /// <br />
        /// If the <see cref="Game"/>'s <see cref="Status"/> is <see cref="GameStatus.NotStarted"/>, the <see cref="StartGame"/> is called and then method continues as usual.<br />
        /// If the <see cref="Game"/>'s <see cref="Status"/> is either <see cref="GameStatus.Won"/> or <see cref="GameStatus.Lost"/>, a <see cref="GameOverException"/> is thrown.
        /// </summary>
        /// <exception cref="GameOverException"/>
        /// <inheritdoc cref="MineSpace.ChangeFlag(int, int, int)"/>
        public FlagResult ChangeFlag(int x, int y, int z)
        {
            if (Status == GameStatus.NotStarted)
                StartGame();

            if (Status == GameStatus.Ongoing)
            {
                FlagResult flagResult = MineSpace.ChangeFlag(x, y, z);

                if (flagResult == FlagResult.Flagged)
                    Flagged++;
                else if (flagResult == FlagResult.Unflagged)
                    Flagged--;

                return flagResult;
            }
            else
                throw new GameOverException();
        }

        #endregion

        #region Not public

        protected void Win()
        {
            gameEndTime = DateTime.Now;

            Status = GameStatus.Won;
        }

        protected void Lose()
        {
            gameEndTime = DateTime.Now;

            Status = GameStatus.Lost;
        }

        private int UncoverSafe(Cube referenceCube)
        {
            int uncoveredCubes = 0;
            Queue<Cube> uncoverQueue = new Queue<Cube>(GetUncoverable(MineSpace.GetSurroundingCubes(referenceCube)));

            Cube currentCube;
            Cube[] cubesToAdd;

            while (uncoverQueue.Count > 0)
            {
                currentCube = uncoverQueue.Dequeue();

                currentCube.Uncover();
                uncoveredCubes++;

                if (currentCube.SurroundingMines == 0)
                {
                    cubesToAdd = GetUncoverable(MineSpace.GetSurroundingCubes(currentCube));

                    foreach (Cube x in cubesToAdd)
                    {
                        if (!uncoverQueue.Contains(x))
                            uncoverQueue.Enqueue(x);
                    }
                }
            }

            Cleared += uncoveredCubes;

            return uncoveredCubes;
        }

        private static Cube[] GetUncoverable(Cube[] cubes)
        {
            List<Cube> uncoverable = new List<Cube>();

            foreach (Cube cube in cubes)
            {
                if (cube.State == CubeState.Covered)
                    uncoverable.Add(cube);
            }

            return uncoverable.ToArray();
        }

        #endregion
        #endregion

        #region Event handlers



        #endregion
    }
}
