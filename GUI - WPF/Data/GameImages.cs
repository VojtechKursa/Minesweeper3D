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

        public static BitmapImage LoadImage(string name)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Minesweeper 3D GUI;component/Media/Images/" + name, UriKind.Absolute));
        }
    }
}
