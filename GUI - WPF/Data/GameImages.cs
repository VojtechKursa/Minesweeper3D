using System;
using System.Windows.Media.Imaging;

namespace Minesweeper3D.WPF.Data
{
    /// <summary>
    /// Class containing all images used by the GUI.
    /// </summary>
    public static class GameImages
    {
        #region Variables and Properties

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> used for a covered cube.
        /// </summary>
        public static BitmapImage Covered { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> array used for an uncovered cube.<br />
        /// In this array an image displaying the number of mines surrounding the cube is found under an equal index.
        /// </summary>
        public static BitmapImage[] Uncovered { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> used for a flagged cube.
        /// </summary>
        public static BitmapImage Flag { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> used for a wrongly flagged cube (used in the after-game view).
        /// </summary>
        public static BitmapImage FlagWrong { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> used for a mined cube (used in the after-game view).
        /// </summary>
        public static BitmapImage Mine { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> used for a mined cube that was activated by the player (used in the after-game view).
        /// </summary>
        public static BitmapImage MineActivated { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initiates loading of images from the application's resources.
        /// </summary>
        public static void LoadImages()
        {
            Covered = LoadImage("Covered.png");
            Flag = LoadImage("Flag.png");
            FlagWrong = LoadImage("FlagWrong.png");
            Mine = LoadImage("Mine.png");
            MineActivated = LoadImage("MineActivated.png");

            Uncovered = new BitmapImage[27];

            Uncovered[0] = LoadImage("Uncovered.png");
            for (int i = 1; i < 27; i++)
            {
                Uncovered[i] = LoadImage(i.ToString() + ".png");
            }
        }

        /// <summary>
        /// Loads an image with the specified name from program's resources.
        /// </summary>
        /// <param name="name">Name of the image to load.</param>
        /// <returns>The <see cref="BitmapImage"/> that was loaded.</returns>
        private static BitmapImage LoadImage(string name) => new(new Uri("pack://application:,,,/Minesweeper 3D GUI;component/Media/Images/" + name, UriKind.Absolute));

        #endregion
    }
}
