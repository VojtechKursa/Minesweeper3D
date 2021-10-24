using Minesweeper3D.Library;
using System.Windows;

namespace Minesweeper3D.WPF.GUI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();

            Data.GameImages.LoadImages();

            infoStripe.InitValues(0, 0, 0, 0);
        }

        public void StartNewGame(int width, int height, int depth, int mineCount)
        {
            game = new Game(width, height, depth, mineCount);
        }

        private void B_new_Click(object sender, RoutedEventArgs e)
        {
            NewGame window = new(this);

            if ((bool)window.ShowDialog())
            {
                InitializeGame();
            }
        }

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

        public void DisplayWin()
        {
            gameWonItem.Visibility = Visibility.Visible;
            gameLostItem.Visibility = Visibility.Hidden;
        }

        public void DisplayLose()
        {
            gameLostItem.Visibility = Visibility.Visible;
            gameWonItem.Visibility = Visibility.Hidden;
        }
    }
}
