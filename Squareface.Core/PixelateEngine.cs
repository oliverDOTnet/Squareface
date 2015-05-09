using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if __ANDROID__
using Android.Graphics;
using Android.Graphics.Drawables;
#endif

namespace Squareface.Core
{
    public class PixelateEngine
    {
        private void Pixelate(int x, int y, int pixelSize, int width, Byte[] pixels, Byte b, Byte g, Byte r, Byte a)
        {
            for (int xx = x; xx < x + pixelSize; xx++)
            {
                for (int yy = y; yy < y + pixelSize; yy++)
                {
                    //if(yy + width > )

                    int index = ((yy * width) + xx) * 4;

                    pixels[index + 0] = b;
                    pixels[index + 1] = g;
                    pixels[index + 2] = r;
                    pixels[index + 3] = a;
                }
            }
        }

#if __ANDROID__
        private void Pixelate(int x, int y, int pixelSize, Bitmap image, Color c)
        {
            for (int xx = x; xx < x + pixelSize; xx++)
            {
                for (int yy = y; yy < y + pixelSize; yy++)
                {
                    image.SetPixel(xx, yy, c);
                }
            }
        }
#endif

        public void StartPixelation(Byte[] pixels, int width, int height, int pixelSize)
        {
            for (int x = 0; x < width - pixelSize; x += pixelSize)
            {
                for (int y = 0; y < height - pixelSize; y += pixelSize)
                {
                    int indexStart = ((y * width) + x) * 4;
                    int indexEnd = (((y + pixelSize - 1) * width) + x + pixelSize - 1) * 4;
                    int indexOfCenter = (indexEnd - indexStart) / 2 + indexStart;

                    Byte b = pixels[indexOfCenter + 0];
                    Byte g = pixels[indexOfCenter + 1];
                    Byte r = pixels[indexOfCenter + 2];
                    Byte a = pixels[indexOfCenter + 3];

                    Pixelate(x, y, pixelSize, width, pixels, b, g, r, a);
                }
            }
        }

#if __ANDROID__
        public void StartPixelation(Bitmap bitmap, int width, int height, int pixelSize)
        {
            for (int x = 0; x < width - pixelSize; x += pixelSize)
            {
                for (int y = 0; y < height - pixelSize; y += pixelSize)
                {
                    int centerColor = bitmap.GetPixel(x, y);
                    int a = Color.GetAlphaComponent(centerColor);
                    int r = Color.GetRedComponent(centerColor);
                    int g = Color.GetGreenComponent(centerColor);
                    int b = Color.GetBlueComponent(centerColor);

                    Pixelate(x, y, pixelSize, bitmap, Color.Argb(a, r, g, b));
                }
            }
        }
#endif
    }
}
