using System;
using System.Windows;
using System.Windows.Controls;

namespace Minesweeper3D.WPF.GUI.Windows
{
    /// <summary>
    /// Interaction logic for NewGame.xaml
    /// </summary>
    public partial class NewGame : Window
    {
        #region Variables and Properties

        private readonly MainWindow mainWindow;
        private readonly bool initComplete;

        #endregion

        #region Constructors

        public NewGame(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            InitializeComponent();

            initComplete = true;

            RefreshInfo();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verifies whether all of the current parameters are valid.
        /// </summary>
        /// <returns>The name of the parameter that's invalid or null if all parameters are valid.</returns>
        private string ParametersValid()
        {
            try
            {
                int height = Convert.ToInt32(TB_height.Text);
                if (height < 1)
                    return "Height";
            }
            catch
            {
                return "Height";
            }

            try
            {
                int width = Convert.ToInt32(TB_width.Text);
                if (width < 1)
                    return "Width";
            }
            catch
            {
                return "Width";
            }

            try
            {
                int depth = Convert.ToInt32(TB_depth.Text);
                if (depth < 1)
                    return "Depth";
            }
            catch
            {
                return "Depth";
            }

            try
            {
                int mineCount = Convert.ToInt32(TB_mineCount.Text);
                if (mineCount < 0)
                    return "Mine count";
            }
            catch
            {
                return "Mine count";
            }

            return null;
        }

        /// <summary>
        /// Refreshes the info section of this window.
        /// </summary>
        private void RefreshInfo()
        {
            if (initComplete)
            {
                string invalidParameter = ParametersValid();

                if (invalidParameter == null)
                {
                    int total = Convert.ToInt32(TB_width.Text) * Convert.ToInt32(TB_height.Text) * Convert.ToInt32(TB_depth.Text);

                    L_totalCubes.Content = total;
                    L_mined.Content = Math.Round(Convert.ToInt32(TB_mineCount.Text) / (double)total * 100, 2);
                }
            }
        }

        #endregion

        #region Event handlers

        private void TB_TextChanged(object sender, TextChangedEventArgs e) => RefreshInfo();

        private void B_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void B_confirm_Click(object sender, RoutedEventArgs e)
        {
            string invalidParameter = ParametersValid();

            if (invalidParameter == null)
            {
                mainWindow.StartNewGame(Convert.ToInt32(TB_width.Text), Convert.ToInt32(TB_height.Text), Convert.ToInt32(TB_depth.Text), Convert.ToInt32(TB_mineCount.Text));

                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Invalid parameter: " + invalidParameter, "Invalid parameter", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        #endregion
    }
}
