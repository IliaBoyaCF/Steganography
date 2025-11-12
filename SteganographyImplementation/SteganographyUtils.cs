
using SteganographyInterfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace SteganographyImplementation
{
    public class SteganographyUtils
    {
        public static long HideMessage(string input, string sourceImage, string destinationImage)
        {
            Bitmap sourceBitmap = new Bitmap(sourceImage);
            Bitmap destinationBitmap = CloneWithFormat(sourceBitmap, sourceBitmap.PixelFormat);

            long seed = HideMessage(input, new Bitmap(sourceImage), destinationBitmap);

            destinationBitmap.Save(destinationImage, sourceBitmap.RawFormat);

            return seed;
        }

        public static long HideMessage(string input, string sourceImage)
        {

            Bitmap bitmap = new Bitmap(sourceImage);
            ImageFormat imageFormat = bitmap.RawFormat;

            long seed = HideMessage(input, bitmap, bitmap);

            bitmap.Save(sourceImage, imageFormat);

            return seed;
        }

        private static long HideMessage(string input, Bitmap sourceImage, Bitmap destinationImage)
        {
            IHidder hidder = new Hidder(input, sourceImage, destinationImage);

            return hidder.Hide();
        }

        public static string FindMessage(long seed, string image)
        {
            IFinder finder = new Finder(new Bitmap(image));

            return finder.Find(seed);
        }

        public static Bitmap CloneWithFormat(Bitmap original, PixelFormat format)
        {
            Bitmap clone = new Bitmap(original.Width, original.Height, format);

            using (Graphics g = Graphics.FromImage(clone))
            {
                g.DrawImage(original, 0, 0, original.Width, original.Height);
            }

            return clone;
        }
    }
}
