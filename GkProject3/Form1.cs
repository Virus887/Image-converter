using GkProject3.Separators;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace GkProject3
{
    public partial class Form1 : Form
    {
        public DirectBitmap MainBitmap;
        public DirectBitmap Bitmap1;
        public DirectBitmap Bitmap2;
        public DirectBitmap Bitmap3;

        public Form1()
        {
            InitializeComponent();
            OptionComboBox.SelectedItem = "YCrCb";
            IluminantComboBox.SelectedItem = "D65";
            ColorProfileComboBox.SelectedItem = "sRGB";
        }

        private void RedrawApp()
        {
            RightPictureBox1.Image = new Bitmap(Bitmap1.Bitmap);
            RightPictureBox2.Image = new Bitmap(Bitmap2.Bitmap);
            RightPictureBox3.Image = new Bitmap(Bitmap3.Bitmap);
            RightPictureBox1.Refresh();
            RightPictureBox2.Refresh();
            RightPictureBox3.Refresh();
        }

        //BUTTONS
 
        private void SeparateButton_Click(object sender, EventArgs e)
        {
            if (MainBitmap == null)
            {
                MessageBox.Show("Please, load image first.");
                return;
            }

            if (OptionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please, choose conversion type.");
                return;
            }

            switch (OptionComboBox.SelectedItem.ToString())
            {
                case "YCrCb":
                    {
                        RgbToYCrCbSeparator x = new RgbToYCrCbSeparator(MainBitmap);
                        Bitmap1 = x.bitmap1;
                        Bitmap2 = x.bitmap2;
                        Bitmap3 = x.bitmap3;
                        RightLabel1.Text = "\nY:";
                        RightLabel2.Text = "\nCr:";
                        RightLabel3.Text = "\nCb:";
                        RedrawApp();
                        break;
                    }
                case "HSV":
                    {
                        RgbToHsvSeparator x = new RgbToHsvSeparator(MainBitmap);
                        Bitmap1 = x.bitmap1;
                        Bitmap2 = x.bitmap2;
                        Bitmap3 = x.bitmap3;
                        RightLabel1.Text = "\nH:";
                        RightLabel2.Text = "\nS:";
                        RightLabel3.Text = "\nV:";
                        RedrawApp();
                        break;
                    }
                case "Lab":
                    {
                        Matrix<double> DataMatrix = GenerateDataMatrix();
                        RgbToLabSeparator x = new RgbToLabSeparator(MainBitmap, DataMatrix, (double)GammaNumeric.Value);
                        Bitmap1 = x.bitmap1;
                        Bitmap2 = x.bitmap2;
                        Bitmap3 = x.bitmap3;
                        RightLabel1.Text = "\nL:";
                        RightLabel2.Text = "\na:";
                        RightLabel3.Text = "\nb:";
                        RedrawApp();
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (MainBitmap == null || OptionComboBox.SelectedItem == null || Bitmap1 == null || Bitmap2 == null || Bitmap3 == null)
            {
                MessageBox.Show("Separate channels first.");
                return;
            }

            Image im1 = (Image)Bitmap1.Bitmap;
            Image im2 = (Image)Bitmap2.Bitmap;
            Image im3 = (Image)Bitmap3.Bitmap;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = path.Substring(6);

            dlg.Filter = "Image files (*.jpg)|*.jpg|(*.png)|*.png|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                switch (OptionComboBox.SelectedItem.ToString())
                {
                    case "YCrCb":
                        {
                            im1.Save(dlg.FileName + "_Y.jpg", ImageFormat.Jpeg);
                            im2.Save(dlg.FileName + "_Cr.jpg", ImageFormat.Jpeg);
                            im3.Save(dlg.FileName + "_Cb.jpg", ImageFormat.Jpeg);
                            break;
                        }
                    case "HSV":
                        {
                            im1.Save(dlg.FileName + "_H.jpg", ImageFormat.Jpeg);
                            im2.Save(dlg.FileName + "_S.jpg", ImageFormat.Jpeg);
                            im3.Save(dlg.FileName + "_V.jpg", ImageFormat.Jpeg);
                            break;
                        }
                    case "Lab":
                        {
                            im1.Save(dlg.FileName + "_L.jpg", ImageFormat.Jpeg);
                            im2.Save(dlg.FileName + "_a.jpg", ImageFormat.Jpeg);
                            im3.Save(dlg.FileName + "_b.jpg", ImageFormat.Jpeg);
                            break;
                        }
                }
            }
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = path.Substring(6);
            dlg.Filter = "Image files (*.jpg)|*.jpg|(*.png)|*.png|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                MainPictureBox.Image = new Bitmap(dlg.FileName);
                MainBitmap = new DirectBitmap(MainPictureBox.Image);
            }
        }




        private Matrix<double> GenerateDataMatrix()
        {
            Matrix<double> ret = DenseMatrix.OfArray(new double[,] {
            {(double)XRedNumeric.Value,(double)XGreenNumeric.Value,(double)XBlueNumeric.Value,(double)XWhiteNumeric.Value},
            {(double)YRedNumeric.Value,(double)YGreenNumeric.Value,(double)YBlueNumeric.Value,(double)YWhiteNumeric.Value},
            {1 - (double)XRedNumeric.Value - (double)YRedNumeric.Value,1 - (double)XGreenNumeric.Value - (double)YGreenNumeric.Value,
            1- (double)XBlueNumeric.Value -(double)YBlueNumeric.Value,1 - (double)XWhiteNumeric.Value - (double)YWhiteNumeric.Value}
            });
            return ret;
        }

        //LAB MENU

        private void EnableLabOptions()
        {
            ColorProfileComboBox.Enabled = true;
            EnableChromacity();
            EnableWhite();
        }
        private void DisableLabOptions()
        {
            ColorProfileComboBox.Enabled = false;
            IluminantComboBox.Enabled = false;
            DisableChromacity();
            DisableWhite();
        }
        private void EnableChromacity()
        {
            XRedNumeric.Enabled = true;
            YRedNumeric.Enabled = true;
            XBlueNumeric.Enabled = true;
            YBlueNumeric.Enabled = true;
            XGreenNumeric.Enabled = true;
            YGreenNumeric.Enabled = true;
            XWhiteNumeric.Enabled = true;
            YWhiteNumeric.Enabled = true;
            GammaNumeric.Enabled = true;
        }
        private void DisableChromacity()
        {
            XRedNumeric.Enabled = false;
            YRedNumeric.Enabled = false;
            XBlueNumeric.Enabled = false;
            YBlueNumeric.Enabled = false;
            XGreenNumeric.Enabled = false;
            YGreenNumeric.Enabled = false;
            XWhiteNumeric.Enabled = false;
            YWhiteNumeric.Enabled = false;
            GammaNumeric.Enabled = false;
        }
        private void EnableWhite()
        {
            XWhiteNumeric.Enabled = true;
            YWhiteNumeric.Enabled = true;
        }
        private void DisableWhite()
        {
            XWhiteNumeric.Enabled = false;
            YWhiteNumeric.Enabled = false;
        }
        private void OptionComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (OptionComboBox.SelectedItem.ToString())
            {
                case "Lab":
                    {
                        ColorProfileComboBox.Enabled = true;
                        ColorProfileComboBox_SelectedValueChanged(null, new EventArgs());
                        break;
                    }
                default:
                    {
                        DisableLabOptions();
                        break;
                    }

            }
        }
        private void ColorProfileComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (ColorProfileComboBox.SelectedItem.ToString())
            {
                case "Custom":
                    {
                        EnableChromacity();
                        IluminantComboBox.Enabled = true;
                        IluminantComboBox_SelectedValueChanged(null, new EventArgs());
                        break;
                    }
                case "sRGB":
                    {
                        SetChromacity(0.64, 0.33, 0.3, 0.6, 0.15, 0.06, 2.2, "D65");
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
                case "Adobe RGB":
                    {
                        SetChromacity(0.64, 0.33, 0.21, 0.71, 0.15, 0.06, 2.2, "D65");
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
                case "Apple RGB":
                    {
                        SetChromacity(0.625, 0.34, 0.28, 0.595, 0.155, 0.07, 1.8, "D65");
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
                case "Best RGB":
                    {
                        SetChromacity(0.7347, 0.2653, 0.215, 0.775, 0.13, 0.035, 2.2, "D50");
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
                case "CIE RGB":
                    {
                        SetChromacity(0.7350, 0.2650, 0.274, 0.717, 0.167, 0.009, 2.2, "E");
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
                case "Wide Gamut RGB":
                    {
                        SetChromacity(0.735, 0.265, 0.115, 0.826, 0.157, 0.018, 2.2, "D50");
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
                case "PAL/SECAM RGB":
                    {
                        SetChromacity(0.64, 0.33, 0.29, 0.6, 0.15, 0.06, 2.2, "D65");
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
                default:
                    {
                        IluminantComboBox.Enabled = false;
                        DisableChromacity();
                        break;
                    }
            }
        }
        private void IluminantComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (IluminantComboBox.SelectedItem.ToString())
            {
                case "Custom":
                    {
                        EnableWhite();
                        break;
                    }
                case "A":
                    {
                        SetWhite(0.44757, 0.40744);
                        DisableWhite();
                        break;
                    }
                case "C":
                    {
                        SetWhite(0.31006, 0.31616);
                        DisableWhite();
                        break;
                    }
                case "D50":
                    {
                        SetWhite(0.34567, 0.35850);
                        DisableWhite();
                        break;
                    }
                case "D55":
                    {
                        SetWhite(0.33242, 0.34743);
                        DisableWhite();
                        break;
                    }
                case "D65":
                    {
                        SetWhite(0.31271, 0.32902);
                        DisableWhite();
                        break;
                    }
                case "E":
                    {
                        SetWhite(0.33333, 0.33333);
                        DisableWhite();
                        break;
                    }
                case "F7":
                    {
                        SetWhite(0.31292, 0.32933);
                        DisableWhite();
                        break;
                    }
                default:
                    {
                        DisableWhite();
                        break;
                    }
            }
        }
        private void SetWhite(double a, double b)
        {
            XWhiteNumeric.Value = (decimal)a;
            YWhiteNumeric.Value = (decimal)b;
        }
        private void SetChromacity(double a, double b, double c, double d, double e, double f, double gamma, string white)
        {
            XRedNumeric.Value = (decimal)a;
            YRedNumeric.Value = (decimal)b;
            XBlueNumeric.Value = (decimal)c;
            YBlueNumeric.Value = (decimal)d;
            XGreenNumeric.Value = (decimal)e;
            YGreenNumeric.Value = (decimal)f;
            GammaNumeric.Value = (decimal)gamma;
            IluminantComboBox.SelectedItem = white;
        }


        //LABORATORY PART

        private void CreateImage_Click(object sender, EventArgs e)
        {
            Image img = GenerateHSVImage();
            MainPictureBox.Image = new Bitmap(img);
            MainBitmap = new DirectBitmap(img);
        }

        private Bitmap GenerateHSVImage()
        {
            int W = 600, H = 600;
            double RADIUS = (double)RADIUSNumeric.Value;
            DirectBitmap directBitmap = new DirectBitmap(W, H);
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    directBitmap.SetPixel(j, i, Color.Black);
                    directBitmap.SetPixel(H - j - 1, i, Color.Black);
                }
            }
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    directBitmap.SetPixel(i, j, Color.Black);
                    directBitmap.SetPixel(i, W - j - 1, Color.Black);
                }
            }


            for (int i = (int)(W / 2.0 - RADIUS); i <= (int)(W / 2.0) + RADIUS; i++)
            {
                for (int j = (int)(W / 2.0 - RADIUS); j <= W / 2.0 + RADIUS; j++)
                {
                    double dist;
                    if ((dist = Math.Sqrt((i - W / 2) * (i - W / 2) + (j - W / 2) * (j - W / 2))) > RADIUS) continue;
                    double v = 1;
                    double s = dist / RADIUS;
                    double h = Math.Atan2(j - W / 2, i - W / 2) * (180 / Math.PI);
                    directBitmap.SetPixel(i, j, HsvToRgb(h, s, v));
                }
            }
            return new Bitmap(directBitmap.Bitmap);
        }

        private Color HsvToRgb(double H, double S, double V)
        {
            while (H < 0) H += 360;
            while (H >= 360) H -= 360;

            double r = 0, g = 0, b = 0;
            if (S == 0)
            {
                r = V;
                g = V;
                b = V;
            }
            else
            {
                int i;
                double f, p, q, t;

                if (H == 360)
                    H = 0;
                else
                    H = H / 60;

                i = (int)Math.Truncate(H);
                f = H - i;

                p = V * (1.0 - S);
                q = V * (1.0 - (S * f));
                t = V * (1.0 - (S * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = V;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = V;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = V;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = V;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = V;
                        break;

                    default:
                        r = V;
                        g = p;
                        b = q;
                        break;
                }

            }
            return Color.FromArgb((int)(r * 255.0), (int)(g * 255.0), (int)(b * 255.0));
        }
    }

}

