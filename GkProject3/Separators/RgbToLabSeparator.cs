using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GkProject3.Separators
{
    public class RgbToLabSeparator : Separator
    {
        public RgbToLabSeparator(DirectBitmap mainBitmap, Matrix<double> rawData, double Gamma) : base(mainBitmap)
        {
            L = new double[w, h];
            a = new double[w, h];
            b = new double[w, h];
            this.RawData = rawData;
            this.Gamma = Gamma;
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

        private double[,] L;
        private double[,] a;
        private double[,] b;

        private double Gamma;
        private Matrix<double> RawData;

        private void GetCoefs()
        {
            double xw = RawData[0, 3];
            double yw = RawData[1, 3];
            double zw = RawData[2, 3];

            double Yw = 1.0;
            double Xw = xw / yw;
            double Zw = zw / yw;
            var XYZw = DenseVector.OfArray(new double[] { Xw, Yw, Zw });

            Matrix<double> sRGB = DenseMatrix.OfArray(new double[,] {
                    {RawData[0,0],RawData[0,1],RawData[0,2]},
                    {RawData[1,0],RawData[1,1],RawData[1,2]},
                    {RawData[2,0],RawData[2,1],RawData[2,2]} });

            Matrix<double> SRGB = sRGB.Inverse();

            var Srgb = SRGB * XYZw;

            Matrix<double> XYZ = DenseMatrix.OfArray(new double[,] {
                    {sRGB[0,0]*Srgb[0],sRGB[0,1]*Srgb[1],sRGB[0,2]*Srgb[2]},
                    {sRGB[1,0]*Srgb[0],sRGB[1,1]*Srgb[1],sRGB[1,2]*Srgb[2]},
                    {sRGB[2,0]*Srgb[0],sRGB[2,1]*Srgb[1],sRGB[2,2]*Srgb[2]} });


            Matrix<double> Lab = DenseMatrix.OfArray(new double[,] {
                    //L
                    {XYZ[1,0]/Yw > 0.008856 ? (Math.Pow(XYZ[1,0]/Yw, 1.0/3.0))*116.0 -16.0 : (XYZ[1,0]/Yw) * 903.3,
                     XYZ[1,1]/Yw > 0.008856 ? (Math.Pow(XYZ[1,1]/Yw, 1.0/3.0))*116.0 -16.0 : (XYZ[1,1]/Yw) * 903.3,
                     XYZ[1,2]/Yw > 0.008856 ? (Math.Pow(XYZ[1,2]/Yw, 1.0/3.0))*116.0 -16.0 : (XYZ[1,2]/Yw) * 903.3 },
                    //a
                    {500.0 * (Math.Pow(XYZ[0,0]/Xw, 1.0/3.0) - Math.Pow(XYZ[1,0]/Yw, 1.0/3.0)),
                     500.0 * (Math.Pow(XYZ[0,1]/Xw, 1.0/3.0) - Math.Pow(XYZ[1,1]/Yw, 1.0/3.0)),
                     500.0 * (Math.Pow(XYZ[0,2]/Xw, 1.0/3.0) - Math.Pow(XYZ[1,2]/Yw, 1.0/3.0))},
                    //b
                    {200.0 * (Math.Pow(XYZ[1,0]/Yw, 1.0/3.0) - Math.Pow(XYZ[2,0]/Zw, 1.0/3.0)),
                     200.0 * (Math.Pow(XYZ[1,1]/Yw, 1.0/3.0) - Math.Pow(XYZ[2,1]/Zw, 1.0/3.0)),
                     200.0 * (Math.Pow(XYZ[1,2]/Yw, 1.0/3.0) - Math.Pow(XYZ[2,2]/Zw, 1.0/3.0))} });


            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    bool inverse = false;
                    //1. Inverse Companding
                    double R = mainBitmap.GetPixel(i, j).R / 255.0;
                    if (inverse)
                    {
                        if (R > 0.04045) R = Math.Pow(((R + 0.055) / 1.055), 2.4);
                        else R /= 12.92;
                    }

                    double G = mainBitmap.GetPixel(i, j).G / 255.0;
                    if (inverse)
                    {
                        if (G > 0.04045) G = Math.Pow(((G + 0.055) / 1.055), 2.4);
                        else G /= 12.92;
                    }

                    double B = mainBitmap.GetPixel(i, j).B / 255.0;
                    if (inverse)
                    {
                        if (B > 0.04045) B = Math.Pow(((B + 0.055) / 1.055), 2.4);
                        else B /= 12.92;
                    }


                    R = Math.Pow(R, Gamma);
                    G = Math.Pow(G, Gamma);
                    B = Math.Pow(B, Gamma);


                    L[i, j] = Lab[0, 0] * R + Lab[0, 1] * G + Lab[0, 2] * B;
                    a[i, j] = Lab[1, 0] * R + Lab[1, 1] * G + Lab[1, 2] * B;
                    b[i, j] = Lab[2, 0] * R + Lab[2, 1] * G + Lab[2, 2] * B;
                }
            }
        }

        private void SetBitmaps()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    bitmap1.SetPixel(i, j, ConvertLToColor(i, j));
                    bitmap2.SetPixel(i, j, ConvertaToColor(i, j));
                    bitmap3.SetPixel(i, j, ConvertbToColor(i, j));
                }
            }
        }

        public Color ConvertLToColor(int i, int j)
        {
            double ret = L[i, j];
            if (ret > 100) ret = 100;
            if (ret < 0) ret = 0;
            int ret2 = (int)(ret * 255.0 / 100.0);
           
            return Color.FromArgb(ret2 , ret2,  ret2);
        }

        public Color ConvertaToColor(int i, int j)
        {            
            int ret = (int)a[i, j];
            if (ret < -127) ret = -127;
            if (ret > 128) ret = 128;
            ret += 127;
            return Color.FromArgb(ret ,255 - ret, 127);
        }

        public Color ConvertbToColor(int i, int j)
        {
            
            int ret = (int)b[i, j];
            if (ret < -127) ret = -127;
            if (ret > 128) ret = 128;
            ret += 127;
            return Color.FromArgb(ret, 127, 255-ret);
        }

    }
}
