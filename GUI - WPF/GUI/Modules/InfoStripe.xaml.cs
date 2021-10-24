using System;
using System.Windows.Controls;

namespace Minesweeper3D.WPF.GUI.Modules
{
    /// <summary>
    /// Interaction logic for InfoStripe.xaml
    /// </summary>
    public partial class InfoStripe : UserControl
    {
        private TimeSpan elapsedTime;
        public TimeSpan ElapsedTime
        {
            get => elapsedTime;
            set
            {
                elapsedTime = value;
                Dispatcher.Invoke(UpdateTime);
            }
        }

        private int flagged;
        public int Flagged
        {
            get => flagged;
            set
            {
                flagged = value;
                L_flagCount.Content = flagged;
            }
        }

        private int cleared;
        public int Cleared
        {
            get => cleared;
            set
            {
                cleared = value;
                L_cleared.Content = string.Format("{0} / {1} ({2} x {3} x {4})", cleared, SpaceTotal, SpaceWidth, SpaceHeight, SpaceDepth);
            }
        }

        public int SpaceWidth { get; private set; }
        public int SpaceHeight { get; private set; }
        public int SpaceDepth { get; private set; }
        public int SpaceTotal { get => SpaceWidth * SpaceHeight * SpaceDepth; }

        public int MineCount { get; private set; }

        public InfoStripe()
        {
            InitializeComponent();
        }

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
            L_flagCount.Content = Flagged;
        }

        private void UpdateTime()
        {
            L_time.Content = string.Format("{0}:{1}:{2}", elapsedTime.Hours.ToString().PadLeft(2, '0'), elapsedTime.Minutes.ToString().PadLeft(2, '0'), elapsedTime.Seconds.ToString().PadLeft(2, '0'));
        }
    }
}
