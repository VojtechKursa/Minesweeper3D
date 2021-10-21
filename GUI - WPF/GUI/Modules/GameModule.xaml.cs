using Minesweeper3D.Library;
using Minesweeper3D.WPF.Data;
using System;
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
        public NumericUpDown DepthSetter
        {
            get => depthSetter;
            set
            {
                depthSetter = value;

                try
                {
                    depthSetter.ValueChanged -= Setter_ValueChanged;
                }
                catch
                { }

                depthSetter.ValueChanged += Setter_ValueChanged;
            }
        }

        private NumericUpDown depthSetter;
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
        }

        private void Setter_ValueChanged(object sender, System.EventArgs e)
        {
            RedrawMinefield();
        }

        private void TimeRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (CurrentGame.ElapsedTime != null)
                    AssignedInfoStripe.ElapsedTime = (TimeSpan)CurrentGame.ElapsedTime;
            }
            catch
            { }
        }

        public void StartGame()
        {
            AssignedInfoStripe.InitValues(MineSpace.Width, MineSpace.Height, MineSpace.Depth, CurrentGame.MineCount);

            ResetMineField();
            InitializeMineField();
            ResizeViewport();

            timeRefreshTimer.Start();
        }

        public void StopGame()
        {
            timeRefreshTimer.Stop();

            if (CurrentGame != null && AssignedInfoStripe != null)
            {
                try
                {
                    if (CurrentGame.ElapsedTime != null)
                        AssignedInfoStripe.ElapsedTime = (TimeSpan)CurrentGame.ElapsedTime;
                }
                catch
                { }
            }
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
                    image.SetValue(Grid.ColumnProperty, x);
                    image.SetValue(Grid.RowProperty, y);

                    image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                    image.MouseRightButtonDown += Image_MouseRightButtonDown;

                    image.Source = GameImages.Covered;

                    images[x, y] = image;
                    grid.Children.Add(image);
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentGame.Status is GameStatus.NotStarted or GameStatus.Ongoing)
            {
                Image image = (Image)sender;

                int x = (int)image.GetValue(Grid.ColumnProperty);
                int y = (int)image.GetValue(Grid.RowProperty);
                int z = DepthSetter.Value - 1;

                UncoverResult result = CurrentGame.Uncover(x, y, z);

                if (result is not UncoverResult.Flag and not UncoverResult.Uncovered)
                {
                    if (result == UncoverResult.Mine)
                        Lose();
                    else
                    {
                        if (result == UncoverResult.Clear)
                            image.Source = GameImages.Uncovered[MineSpace.GetCube(x, y, z).SurroundingMines];
                        else
                            RedrawMinefield();

                        AssignedInfoStripe.Cleared = CurrentGame.Cleared;

                        if (CurrentGame.Status == GameStatus.Won)
                            Win();
                    }
                }
            }
        }

        private void RedrawMinefield()
        {
            Cube[,] cubes = MineSpace.GetLayer(DepthSetter.Value - 1);

            if (CurrentGame.Status is not GameStatus.Lost)
            {
                for (int y = 0; y < MineSpace.Height; y++)
                {
                    for (int x = 0; x < MineSpace.Width; x++)
                    {
                        switch (cubes[x, y].State)
                        {
                            case CubeState.Flagged:
                                images[x, y].Source = GameImages.Flag;
                                break;
                            case CubeState.Covered:
                                images[x, y].Source = GameImages.Covered;
                                break;
                            case CubeState.Uncovered:
                                images[x, y].Source = GameImages.Uncovered[cubes[x, y].SurroundingMines];
                                break;
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < MineSpace.Height; y++)
                {
                    for (int x = 0; x < MineSpace.Width; x++)
                    {
                        switch (cubes[x, y].State)
                        {
                            case CubeState.Flagged:
                                images[x, y].Source = cubes[x, y].HasMine ? GameImages.Flag : GameImages.FlagWrong;
                                break;
                            case CubeState.Covered:
                                images[x, y].Source = cubes[x, y].HasMine ? GameImages.Mine : GameImages.Covered;
                                break;
                            case CubeState.Uncovered:
                                images[x, y].Source = cubes[x, y].HasMine ? GameImages.MineActivated : GameImages.Uncovered[cubes[x, y].SurroundingMines];
                                break;
                        }
                    }
                }
            }
        }

        private void Win()
        {
            StopGame();
        }

        private void Lose()
        {
            StopGame();

            RedrawMinefield();
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentGame.Status is GameStatus.NotStarted or GameStatus.Ongoing)
            {
                Image image = (Image)sender;

                int x = (int)image.GetValue(Grid.ColumnProperty);
                int y = (int)image.GetValue(Grid.RowProperty);
                int z = DepthSetter.Value - 1;

                FlagResult result = CurrentGame.ChangeFlag(x, y, z);

                if (result is FlagResult.Flagged or FlagResult.Unflagged)
                {
                    AssignedInfoStripe.Flagged = CurrentGame.Flagged;

                    if (result == FlagResult.Flagged)
                        image.Source = GameImages.Flag;
                    else
                        image.Source = GameImages.Covered;
                }
            }
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
