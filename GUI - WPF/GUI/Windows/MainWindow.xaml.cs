﻿using System.Windows;
using Minesweeper3D.Library;

namespace Minesweeper3D.WPF.GUI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables and Properties

        private Game game;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            Data.GameImages.LoadImages();

            infoStripe.InitValues(0, 0, 0, 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new <see cref="Game"/> with the given parameters and sets it as <see cref="game"/>.
        /// </summary>
        /// <param name="width">The desired width of the <see cref="Game.MineSpace"/> in the newly created <see cref="Game"/>.</param>
        /// <param name="height">The desired height of the <see cref="Game.MineSpace"/> in the newly created <see cref="Game"/>.</param>
        /// <param name="depth">The desired depth of the <see cref="Game.MineSpace"/> in the newly created <see cref="Game"/>.</param>
        /// <param name="mineCount">The desired number of mines in the <see cref="Game.MineSpace"/> of the newly created <see cref="Game"/>.</param>
        public void StartNewGame(int width, int height, int depth, int mineCount) => game = new Game(width, height, depth, mineCount);

        /// <summary>
        /// Performs necessary steps for beginning a new game.
        /// </summary>
        private void InitializeGame()
        {
            gameModule.StopGame();

            gameWonItem.Visibility = Visibility.Hidden;
            gameLostItem.Visibility = Visibility.Hidden;

            gameModule.MainWindow = this;
            gameModule.CurrentGame = game;
            gameModule.AssignedInfoStripe = infoStripe;
            gameModule.DepthSetter = Nud_depth;

            gameModule.StartGame();

            Nud_depth.MinValue = 1;
            Nud_depth.MaxValue = game.MineSpace.Depth;
            Nud_depth.Value = 1;
        }

        /// <summary>
        /// Displays the indications for a won game.
        /// </summary>
        public void DisplayWin()
        {
            gameWonItem.Visibility = Visibility.Visible;
            gameLostItem.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the indications for a lost game.
        /// </summary>
        public void DisplayLose()
        {
            gameLostItem.Visibility = Visibility.Visible;
            gameWonItem.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Event handlers

        private void B_new_Click(object sender, RoutedEventArgs e)
        {
            NewGame window = new(this);

            if ((bool)window.ShowDialog())
            {
                InitializeGame();
            }
        }

        #endregion
    }
}
