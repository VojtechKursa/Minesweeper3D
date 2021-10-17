using System;
using System.Windows.Media.Imaging;

namespace Minesweeper3D.WPF.Data
{
    public static class GameImages
    {
        public static BitmapImage Covered { get; set; }
        public static BitmapImage[] Uncovered { get; set; }
        public static BitmapImage Flag { get; set; }
        public static BitmapImage FlagWrong { get; set; }
        public static BitmapImage Mine { get; set; }
        public static BitmapImage MineActivated { get; set; }

        public static void LoadImages()
        {
            Covered = new BitmapImage(new Uri("Media/Images/Covered.png", UriKind.Relative));
            Flag = new BitmapImage(new Uri("Media/Images/Flag.png", UriKind.Relative));
            FlagWrong = new BitmapImage(new Uri("Media/Images/FlagWrong.png", UriKind.Relative));
            Mine = new BitmapImage(new Uri("Media/Images/Mine.png", UriKind.Relative));
            MineActivated = new BitmapImage(new Uri("Media/Images/MineActivated.png", UriKind.Relative));

            Uncovered[0] = new BitmapImage(new Uri("Media/Images/Uncovered.png", UriKind.Relative));
            for (int i = 1; i < 27; i++)
            {
                Uncovered[i] = new BitmapImage(new Uri("Media/Images/" + i.ToString() + ".png", UriKind.Relative));
            }
        }
    }
}
