using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GkProject3.Separators
{
    public class RgbToYCrCbSeparator : Separator
    {

        private float[,] Y;
        private float[,] Cb;
        private float[,] Cr;
        public RgbToYCrCbSeparator(DirectBitmap mainBitmap) : base(mainBitmap)
        {
            Y = new float[w, h];
            Cr = new float[w, h];
            Cb = new float[w, h];
            Separate();
        }

        public void Separate()
        {
            bitmap1 = new DirectBitmap(w, h);
            bitmap2 = new DirectBitmap(w, h);
            bitmap3 = new DirectBitmap(w, h);
            GetCoefs();
            SetBitmaps();
        }

        private void SetBitmaps()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    bitmap1.SetPixel(i, j, ConvertYToColor(i, j));
                    bitmap2.SetPixel(i, j, ConvertCrToColor(i, j));
                    bitmap3.SetPixel(i, j, ConvertCbToColor(i, j));
                }
            }
        }


        public Color ConvertYToColor (int i, int j)
        {
            return Color.FromArgb((int)Y[i,j], (int)Y[i, j], (int)Y[i, j]);
        }

        public Color ConvertCbToColor(int i, int j)
        {
            return Color.FromArgb(127 ,255 -(int)Cb[i, j], (int)Cb[i, j]);
        }

        public Color ConvertCrToColor (int i, int j)
        {
            return Color.FromArgb((int)Cr[i, j],255 - (int)Cr[i, j], 127);
        }   
        

        private void GetCoefs()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float R = mainBitmap.GetPixel(i, j).R;
                    float G = mainBitmap.GetPixel(i, j).G;
                    float B = mainBitmap.GetPixel(i, j).B;

                    Y[i, j] = (0.299f * R + 0.587f * G + 0.114f * B);
                    Cb[i, j] = (B - Y[i,j])/1.772f + 127.5f;
                    Cr[i, j] = (R - Y[i,j])/1.402f + 127.5f;
                }
            }
        }
    }
}
