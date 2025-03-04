using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Minesweeper3D.Library;
using Minesweeper3D.WPF.Data;
using Minesweeper3D.WPF.GUI.Windows;

namespace Minesweeper3D.WPF.GUI.Modules
{
    /// <summary>
    /// Interaction logic for GameModule.xaml
    /// </summary>
    public partial class GameModule : UserControl
    {
        #region Variables and Properties
        #region Public

        /// <summary>
        /// Gets the <see cref="Minesweeper3D.Library.MineSpace"/> of the <see cref="Game"/> associated with this <see cref="GameModule"/>.
        /// </summary>
        public MineSpace MineSpace => CurrentGame?.MineSpace;

        /// <summary>
        /// Gets or sets the <see cref="Game"/> associated with this <see cref="GameModule"/>.
        /// </summary>
        public Game CurrentGame { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="InfoStripe"/> used for displaying information from this <see cref="GameModule"/>.
        /// </summary>
        public InfoStripe AssignedInfoStripe { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Windows.MainWindow"/> this <see cref="GameModule"/> is a part of.
        /// </summary>
        public MainWindow MainWindow { get; set; }

        /// <summary>
        /// Gets or sets the control used to control the current depth at which the user wants to be.
        /// </summary>
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

        #endregion

        #region Non-public

        private NumericUpDown depthSetter;
        private Image[,] images;
        private readonly Timer timeRefreshTimer = new()
        {
            AutoReset = true,
            Interval = 100,
            Enabled = false
        };

        #endregion
        #endregion

        #region Constructors

        public GameModule()
        {
            InitializeComponent();

            timeRefreshTimer.Elapsed += TimeRefreshTimer_Elapsed;
        }

        #endregion

        #region Methods
        #region Public

        /// <summary>
        /// Resets the <see cref="AssignedInfoStripe"/>, re-renders itself according to the current <see cref="MineSpace"/> and starts the refreshing of elapsed time.<br />
        /// This method should be called after the <see cref="GameModule"/> is created and after <see cref="CurrentGame"/> and <see cref="AssignedInfoStripe"/> are assigned.
        /// </summary>
        public void StartGame()
        {
            AssignedInfoStripe.InitValues(MineSpace.Width, MineSpace.Height, MineSpace.Depth, CurrentGame.MineCount);

            ResetMineField();
            InitializeMineField();
            ResizeViewport();

            timeRefreshTimer.Start();
        }

        /// <summary>
        /// Stops the refreshing of elapsed time and sets it to the last value.
        /// </summary>
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

        #endregion

        #region Non-public

        /// <summary>
        /// Triggers the actions required to be done after the game is won.
        /// </summary>
        private void Win()
        {
            StopGame();

            MainWindow.DisplayWin();
        }

        /// <summary>
        /// Triggers the actions required to be done after the game is lost.
        /// </summary>
        private void Lose()
        {
            StopGame();

            RedrawMinefield();

            MainWindow.DisplayLose();
        }

        /// <summary>
        /// Initializes the minefield by defining collumns and rows to the main <see cref="Grid"/> and by generating <see cref="Image"/>s and assigning them to the <see cref="Grid"/> and <see cref="images"/> appropriately.
        /// </summary>
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

        /// <summary>
        /// Re-renders the entire minefield based on the current depth obtained from the <see cref="DepthSetter"/>.
        /// </summary>
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

        /// <summary>
        /// Forgets all <see cref="Image"/>s and clears all <see cref="Grid.RowDefinitions"/> and <see cref="Grid.ColumnDefinitions"/> from the main <see cref="Grid"/>.
        /// </summary>
        private void ResetMineField()
        {
            images = null;

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }

        /// <summary>
        /// Recalculates and sets the new size of the <see cref="Grid"/> used to display the minefield based on the current size of the main <see cref="Canvas"/>.
        /// </summary>
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
                    gridHeight = 1 / mineFieldRatio * gridWidth;
                }
                else    //Limited by height of Canvas
                {
                    gridHeight = canvas.ActualHeight;
                    gridWidth = mineFieldRatio * gridHeight;
                }

                double horizontalMargin = (canvas.ActualWidth / 2) - (gridWidth / 2);
                double verticalMargin = (canvas.ActualHeight / 2) - (gridHeight / 2);

                grid.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
                grid.Height = gridHeight;
                grid.Width = gridWidth;
            }
        }

        #endregion
        #endregion

        #region Event handlers

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

                    image.Source = result == FlagResult.Flagged ? GameImages.Flag : GameImages.Covered;
                }
            }
        }

        private void Setter_ValueChanged(object sender, System.EventArgs e) => RedrawMinefield();

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

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e) => ResizeViewport();

        #endregion
    }
}
