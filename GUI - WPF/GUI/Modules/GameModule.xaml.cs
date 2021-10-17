using Minesweeper3D.Library;
using Minesweeper3D.WPF.Data;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper3D.WPF.GUI.Modules
{
    /// <summary>
    /// Interaction logic for GameModule.xaml
    /// </summary>
    public partial class GameModule : UserControl
    {
        public MineSpace MineSpace { get => CurrentGame?.MineSpace; }
        public Game CurrentGame { get; set; }
        public InfoStripe AssignedInfoStripe { get; set; }
        public NumericUpDown DepthSetter { get; set; }

        private Image[,] images;

        private readonly Timer timeRefreshTimer = new()
        {
            AutoReset = true,
            Interval = 100,
            Enabled = false
        };

        public GameModule()
        {
            InitializeComponent();

            timeRefreshTimer.Elapsed += TimeRefreshTimer_Elapsed;

            // Fix the loading of images later
            //GameImages.LoadImages();
        }

        private void TimeRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                AssignedInfoStripe.ElapsedTime = CurrentGame.ElapsedTime;
            }
            catch
            { }
        }

        public void StartGame()
        {
            AssignedInfoStripe.InitValues(MineSpace.Width, MineSpace.Height, MineSpace.Depth, CurrentGame.MineCount);

            ResetMineField();
            InitializeMineField();

            timeRefreshTimer.Start();
        }

        public void StopGame()
        {
            timeRefreshTimer.Stop();
        }

        private void InitializeMineField()
        {
            images = new Image[MineSpace.Width, MineSpace.Height];

            for (int i = 0; i < MineSpace.Width; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < MineSpace.Height; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int y = 0; y < MineSpace.Height; y++)
            {
                for (int x = 0; x < MineSpace.Width; x++)
                {
                    Image image = new();
                    image.SetValue(Grid.RowProperty, x);
                    image.SetValue(Grid.ColumnProperty, y);

                    image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                    image.MouseRightButtonDown += Image_MouseRightButtonDown;

                    images[x, y] = image;
                    grid.Children.Add(image);
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;

            int x = (int)image.GetValue(Grid.RowProperty);
            int y = (int)image.GetValue(Grid.ColumnProperty);
            int z = DepthSetter.Value;

            CurrentGame.Uncover(x, y, z);
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;

            int x = (int)image.GetValue(Grid.RowProperty);
            int y = (int)image.GetValue(Grid.ColumnProperty);
            int z = DepthSetter.Value;

            CurrentGame.ChangeFlag(x, y, z);
        }

        private void ResetMineField()
        {
            images = null;

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }

        private void ResizeViewport()
        {
            if (CurrentGame != null)
            {
                double mineFieldRatio = MineSpace.Width / (double)MineSpace.Height;
                double canvasRatio = canvas.ActualWidth / canvas.ActualHeight;

                double gridWidth;
                double gridHeight;

                if (mineFieldRatio > canvasRatio)    //Limited by width of Canvas
                {
                    gridWidth = canvas.ActualWidth;
                    gridHeight = (1 / mineFieldRatio) * gridWidth;
                }
                else    //Limited by height of Canvas
                {
                    gridHeight = canvas.ActualHeight;
                    gridWidth = mineFieldRatio * gridHeight;
                }

                double horizontalMargin = canvas.ActualWidth / 2 - gridWidth / 2;
                double verticalMargin = canvas.ActualHeight / 2 - gridHeight / 2;

                grid.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
                grid.Height = gridHeight;
                grid.Width = gridWidth;
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeViewport();
        }
    }
}
