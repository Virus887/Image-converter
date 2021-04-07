using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GkProject3.Separators
{
    public class RgbToHsvSeparator : Separator
    {
        private double[,] H;
        private double[,] S;
        private double[,] V;

        public RgbToHsvSeparator(DirectBitmap mainBitmap) : base(mainBitmap)
        {
            H = new double[w, h];
            S = new double[w, h];
            V = new double[w, h];
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
                    bitmap1.SetPixel(i, j, ConvertHToColor(i, j));
                    bitmap2.SetPixel(i, j, ConvertSToColor(i, j));
                    bitmap3.SetPixel(i, j, ConvertVToColor(i, j));
                }
            }
        }


       
        public Color ConvertHToColor(int i, int j)
        {
            return Color.FromArgb((int)(H[i, j] * (255.0f / 359)), (int)(H[i, j] * (255.0f / 359)), (int)(H[i, j] * (255.0f / 359)));
        }

        public Color ConvertSToColor(int i, int j)
        {
            return Color.FromArgb((int)(S[i,j]*(255.0f/100)), (int)(S[i,j]*(255.0f/100)), (int)(S[i,j]*(255.0f/100)));
        }

        public Color ConvertVToColor(int i, int j)
        {
            return Color.FromArgb((int)(V[i, j] * (255.0f / 100)), (int)(V[i, j]*(255.0f / 100)), (int)(V[i, j]*(255.0f / 100)));
        }


        private void GetCoefs()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {

                    double R = mainBitmap.GetPixel(i, j).R /255.0f;
                    double G = mainBitmap.GetPixel(i, j).G /255.0f;
                    double B = mainBitmap.GetPixel(i, j).B / 255.0f;


                    double h = -1, s = -1, v;

                    double cmax = Math.Max(R, Math.Max(G, B)); 
                    double cmin = Math.Min(R, Math.Min(G, B)); 
                    double diff = cmax - cmin;

                    if (cmax == R) h = (60 * ((G - B) / diff) + 360) % 360;
                    if (cmax == G) h = (60 * ((B - R) / diff) + 120) % 360;
                    if (cmax == B) h = (60 * ((R - G) / diff) + 240) % 360;

                    if (cmax == cmin) h = 0;

                    if (cmax == 0) s = 0;
                    else s = (diff / cmax) * 100;


                    v = cmax * 100;

                    H[i, j] = h;
                    S[i, j] = s;
                    V[i, j] = v;
                }
            }
        }


    }
}
