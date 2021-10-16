using System;
using System.Timers;

namespace Minesweeper_3D_Library
{
    public class Game : IDisposable
    {
        #region Variables and Properties
        #region Public

        public MineSpace MineSpace { get; }

        public GameStatus Status { get; } = GameStatus.NotStarted;
        public TimeSpan ElapsedTime { get; protected set; } = new TimeSpan(0);
        public int PlacedFlags { get; private set; }
        public int MinesPresent { get => MineSpace.ActualMineCount; }

        #endregion

        #region Not public

        private readonly DateTime gameStartTime;
        protected readonly Timer timer_elapsedTimeRefresh = new Timer(100)
        {
            AutoReset = true,
            Enabled = false
        };

        #endregion
        #endregion

        #region Constructors

        public Game(int width, int height, int depth, int mineCount)
        {
            MineSpace = new MineSpace(width, height, depth, mineCount);

            timer_elapsedTimeRefresh.Elapsed += Timer_elapsedTimeRefresh_Elapsed;
        }

        #endregion

        #region Methods
        #region Public

        public void StartTimer()
        {
            timer_elapsedTimeRefresh.Start();
        }

        public void StopTimer()
        {
            timer_elapsedTimeRefresh.Stop();

            RecalculateElapsedTime();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            timer_elapsedTimeRefresh.Dispose();
        }

        #endregion

        #region Not public

        private void RecalculateElapsedTime()
        {
            ElapsedTime = new TimeSpan(DateTime.Now.Ticks - gameStartTime.Ticks);
        }

        #endregion
        #endregion

        #region Event handlers

        private void Timer_elapsedTimeRefresh_Elapsed(object sender, ElapsedEventArgs e)
        {
            RecalculateElapsedTime();
        }

        #endregion
    }
}
