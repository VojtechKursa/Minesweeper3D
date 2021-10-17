using Minesweeper3D.Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Minesweeper3D.Library
{
    public class Game : IDisposable
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
        /// Gets (or sets) the time that has elapsed since the start of the game.
        /// </summary>
        public TimeSpan ElapsedTime { get; protected set; } = new TimeSpan(0);

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

        private DateTime gameStartTime;
        protected readonly Timer timer_elapsedTimeRefresh = new Timer(100)
        {
            AutoReset = true,
            Enabled = false
        };

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

            timer_elapsedTimeRefresh.Elapsed += Timer_elapsedTimeRefresh_Elapsed;
        }

        #endregion

        #region Methods
        #region Public

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            timer_elapsedTimeRefresh.Dispose();
        }

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
                StartTimer();
                Status = GameStatus.Ongoing;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Starts the game timer and sets the <see cref="gameStartTime"/> to <see cref="DateTime.Now"/>.
        /// </summary>
        public void StartTimer()
        {
            if (gameStartTime == null)
                gameStartTime = DateTime.Now;

            timer_elapsedTimeRefresh.Start();
        }

        /// <summary>
        /// Stops the game timer and recalculates the elapsed time.
        /// </summary>
        public void StopTimer()
        {
            timer_elapsedTimeRefresh.Stop();

            RecalculateElapsedTime();
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
            StopTimer();

            Status = GameStatus.Won;
        }

        protected void Lose()
        {
            StopTimer();

            Status = GameStatus.Lost;
        }

        private void RecalculateElapsedTime()
        {
            ElapsedTime = new TimeSpan(DateTime.Now.Ticks - gameStartTime.Ticks);
        }

        private int UncoverSafe(Cube cube)
        {
            int uncoveredCubes = 0;
            Queue<Cube> uncoverQueue = new Queue<Cube>(GetUncoverable(MineSpace.GetSurroundingCubes(cube)));

            Cube currentCube;
            Cube[] cubesToAdd;

            while (uncoverQueue.Count > 0)
            {
                currentCube = uncoverQueue.Dequeue();

                currentCube.Uncover();
                uncoveredCubes++;

                if (currentCube.SurroundingMines == 0)
                {
                    cubesToAdd = GetUncoverable(MineSpace.GetSurroundingCubes(cube));

                    foreach (Cube x in cubesToAdd)
                    {
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

        private void Timer_elapsedTimeRefresh_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RecalculateElapsedTime();
            }
            catch
            { }
        }

        #endregion
    }
}
