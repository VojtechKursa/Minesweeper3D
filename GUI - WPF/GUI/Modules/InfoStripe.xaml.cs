using System;
using System.Windows.Controls;

namespace Minesweeper3D.WPF.GUI.Modules
{
    /// <summary>
    /// Interaction logic for InfoStripe.xaml
    /// </summary>
    public partial class InfoStripe : UserControl
    {
        #region Variables and Properties
        #region Public

        /// <summary>
        /// Gets or sets the time that has elapsed since the start of the game.<br />
        /// When setting this value, the displayed value is automatically updated.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get => elapsedTime;
            set
            {
                elapsedTime = value;
                Dispatcher.Invoke(UpdateTime);
            }
        }

        /// <summary>
        /// Gets or sets the number of cubes that are currently flagged.<br />
        /// When setting this value, the displayed value is automatically updated.
        /// </summary>
        public int Flagged
        {
            get => flagged;
            set
            {
                flagged = value;
                L_flagCount.Content = flagged;
            }
        }

        /// <summary>
        /// Gets or sets the number of cubes that are cleared.<br />
        /// When setting this value, the displayed value is automatically updated.
        /// </summary>
        public int Cleared
        {
            get => cleared;
            set
            {
                cleared = value;
                L_cleared.Content = string.Format("{0} / {1}", cleared, SpaceTotal - MineCount);
            }
        }

        /// <summary>
        /// Gets (or sets) the width of the current <see cref="Library.MineSpace"/>.
        /// </summary>
        public int SpaceWidth { get; private set; }
        /// <summary>
        /// Gets (or sets) the height of the current <see cref="Library.MineSpace"/>.
        /// </summary>
        public int SpaceHeight { get; private set; }
        /// <summary>
        /// Gets (or sets) the depth of the current <see cref="Library.MineSpace"/>.
        /// </summary>
        public int SpaceDepth { get; private set; }
        /// <summary>
        /// Gets the total amount of cubes in the current <see cref="Library.MineSpace"/>.
        /// </summary>
        public int SpaceTotal { get => SpaceWidth * SpaceHeight * SpaceDepth; }

        /// <summary>
        /// Gets (or sets) the number of mines in the current <see cref="Library.MineSpace"/>.
        /// </summary>
        public int MineCount { get; private set; }

        #endregion
        #region Non-public

        private TimeSpan elapsedTime;
        private int flagged;
        private int cleared;

        #endregion
        #endregion

        #region Constructors

        public InfoStripe()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the initial values of the <see cref="InfoStripe"/>.
        /// </summary>
        /// <param name="width">The width of the current <see cref="Library.MineSpace"/>.</param>
        /// <param name="height">The height of the current <see cref="Library.MineSpace"/>.</param>
        /// <param name="depth">The depth of the current <see cref="Library.MineSpace"/>.</param>
        /// <param name="mineCount">The number of mines in the current <see cref="Library.MineSpace"/>.</param>
        public void InitValues(int width, int height, int depth, int mineCount)
        {
            SpaceWidth = width;
            SpaceHeight = height;
            SpaceDepth = depth;
            MineCount = mineCount;

            Cleared = 0;
            Flagged = 0;
            ElapsedTime = new TimeSpan(0);

            L_mineCount.Content = MineCount;
            L_total.Content = string.Format("{0} x {1} x {2}", SpaceWidth, SpaceHeight, SpaceDepth);
        }

        /// <summary>
        /// Formats the value in the <see cref="elapsedTime"/> and sets it as a content to the <see cref="L_time"/>.
        /// </summary>
        private void UpdateTime()
        {
            L_time.Content = string.Format("{0}:{1}:{2}", elapsedTime.Hours.ToString().PadLeft(2, '0'), elapsedTime.Minutes.ToString().PadLeft(2, '0'), elapsedTime.Seconds.ToString().PadLeft(2, '0'));
        }

        #endregion
    }
}
