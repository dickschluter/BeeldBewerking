using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Vervormen : BewerkingBasis
    {
        enum CurveVorm
        {
            KegelAflopend,
            KegelOplopend,
            SinusNaarBinnen,
            SinusNaarBuiten
        }

        PictureBox pictureBoxType;
        NumericUpDown numericPercentage;
        Button buttonToepassen;

        int type = -1;

        public Vervormen(Form1 form1)
            : base(form1)
        {
            Naam = "Vervormen";
            labelBewerking.Text = Naam;

            Bitmap bitmapType = Properties.Resources.VervormingsType;
            bitmapType.MakeTransparent(Color.White);
            pictureBoxType = new PictureBox();
            pictureBoxType.Size = new Size(100, 170);
            pictureBoxType.Location = new Point(50, 120);
            pictureBoxType.Image = bitmapType;
            pictureBoxType.MouseClick += new MouseEventHandler(pictureBoxType_MouseClick);
            pictureBoxType.Paint += new PaintEventHandler(pictureBoxType_Paint);
            lijstControls.Add(pictureBoxType);

            Label label = new Label();
            label.AutoSize = true;
            label.Location = new Point(47, 333);
            label.Text = "Procent";
            lijstControls.Add(label);

            numericPercentage = new NumericUpDown();
            numericPercentage.Size = new Size(44, 20);
            numericPercentage.Location = new Point(106, 330);
            numericPercentage.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericPercentage);

            buttonToepassen = new Button();
            buttonToepassen.Size = new Size(100, 24);
            buttonToepassen.Location = new Point(50, 390);
            buttonToepassen.Text = "Toepassen";
            buttonToepassen.Click += new EventHandler(buttonToepassen_Click);
            lijstControls.Add(buttonToepassen);

            form1.InitialiseerBewerking(lijstControls, true);
        }

        void pictureBoxType_MouseClick(object sender, MouseEventArgs e)
        {
            type = (e.Y + 5) / 30 * 2 + e.X / 50;
            pictureBoxType.Refresh();
        }

        void pictureBoxType_Paint(object sender, PaintEventArgs e)
        {
            if (type >= 0)
                e.Graphics.DrawRectangle(Pens.Red, 15 + (type % 2) * 50, 0 + (type / 2) * 30, 19, 19);
        }

        void buttonToepassen_Click(object sender, EventArgs e)
        {
            if (type >= 0 && numericPercentage.Value > 0)
            {
                form1.StartProgressBar();

                switch (type)
                {
                    case 0:
                        HellenHorizontaal((float)numericPercentage.Value / 100);
                        break;
                    case 1:
                        HellenHorizontaal(-(float)numericPercentage.Value / 100);
                        break;
                    case 2:
                        HellenVerticaal((float)numericPercentage.Value / 100);
                        break;
                    case 3:
                        HellenVerticaal(-(float)numericPercentage.Value / 100);
                        break;
                    case 4:
                        CurveHorizontaal(CurveVorm.KegelAflopend, (float)numericPercentage.Value / 100);
                        break;
                    case 5:
                        CurveHorizontaal(CurveVorm.KegelOplopend, (float)numericPercentage.Value / 100);
                        break;
                    case 6:
                        CurveVerticaal(CurveVorm.KegelAflopend, (float)numericPercentage.Value / 100);
                        break;
                    case 7:
                        CurveVerticaal(CurveVorm.KegelOplopend, (float)numericPercentage.Value / 100);
                        break;
                    case 8:
                        CurveHorizontaal(CurveVorm.SinusNaarBinnen, (float)numericPercentage.Value / 100);
                        break;
                    case 9:
                        CurveHorizontaal(CurveVorm.SinusNaarBuiten, (float)numericPercentage.Value / 100);
                        break;
                    case 10:
                        CurveVerticaal(CurveVorm.SinusNaarBinnen, (float)numericPercentage.Value / 100);
                        break;
                    case 11:
                        CurveVerticaal(CurveVorm.SinusNaarBuiten, (float)numericPercentage.Value / 100);
                        break;
                }

                form1.StopProgressBar();

                Geschiedenis.HuidigeBitmapToevoegen();
                form1.ToonBitmap();
            }
        }

        void HellenHorizontaal(float proportie)
        {
            Bitmap vorigeBitmap = Huidige.Bitmap;
            Huidige.Bitmap = new Bitmap(
                Huidige.Bitmap.Width + (int)(Huidige.Bitmap.Height * Math.Abs(proportie)), Huidige.Bitmap.Height);
            Matrix matrix = new Matrix();
            matrix.Shear(proportie, 0);
            using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Transform = matrix;
                g.DrawImage(
                    vorigeBitmap, (proportie > 0 ? 0 : Huidige.Bitmap.Height * Math.Abs(proportie)), 0);
            }
        }

        void HellenVerticaal(float proportie)
        {
            Bitmap vorigeBitmap = Huidige.Bitmap;
            Huidige.Bitmap = new Bitmap(
                Huidige.Bitmap.Width, Huidige.Bitmap.Height + (int)(Huidige.Bitmap.Width * Math.Abs(proportie)));
            Matrix matrix = new Matrix();
            matrix.Shear(0, proportie);
            using (Graphics graphics = Graphics.FromImage(Huidige.Bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.Transform = matrix;
                graphics.DrawImage(
                    vorigeBitmap, 0, (proportie > 0 ? 0 : Huidige.Bitmap.Width * Math.Abs(proportie)));
            }
        }

        void CurveHorizontaal(CurveVorm curveVorm, float proportie) 
        {
            BitmapData data = Huidige.Bitmap.LockBits(new Rectangle(Point.Empty, Huidige.Bitmap.Size),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr regelPtr = data.Scan0;

            float[] doelBuffer = new float[3 * Huidige.Bitmap.Width];
            float middelpunt = (float)Huidige.Bitmap.Width / 2;

            for (int y = 0; y < Huidige.Bitmap.Height; y++)
            {
                form1.UpdateProgressBar(y * 100 / Huidige.Bitmap.Height);

                float krimpFactor =
                    geefKrimpFactor(curveVorm, proportie, y, Huidige.Bitmap.Height);

                // reset doelBuffer
                for (int i = 0; i < doelBuffer.Length; i++)
                    doelBuffer[i] = 0;

                for (int x = 0; x < Huidige.Bitmap.Width; x++) // itereer over bronpixels
                {
                    // lees bronpixel
                    byte blauw = Marshal.ReadByte(regelPtr, 4 * x);
                    byte groen = Marshal.ReadByte(regelPtr, 4 * x + 1);
                    byte rood = Marshal.ReadByte(regelPtr, 4 * x + 2);

                    // map bronpixel naar doelpixels
                    float onderGrens = ((float)x - middelpunt) * krimpFactor + middelpunt;
                    float proportieEerstePixel =
                        Math.Min(((int)onderGrens + 1 - onderGrens), krimpFactor);

                    doelBuffer[3 * (int)onderGrens] += (proportieEerstePixel * blauw);
                    doelBuffer[3 * (int)onderGrens + 1] += (proportieEerstePixel * groen);
                    doelBuffer[3 * (int)onderGrens + 2] += (proportieEerstePixel * rood);

                    // als bronpixel over 2 doelpixels wordt verdeeld
                    if (proportieEerstePixel < krimpFactor)
                    {
                        doelBuffer[3 * (int)onderGrens + 3] += ((krimpFactor - proportieEerstePixel) * blauw);
                        doelBuffer[3 * (int)onderGrens + 4] += ((krimpFactor - proportieEerstePixel) * groen);
                        doelBuffer[3 * (int)onderGrens + 5] += ((krimpFactor - proportieEerstePixel) * rood);
                    }
                }

                // kopieer doelpixels naar bitmap
                for (int x = 0; x < Huidige.Bitmap.Width; x++)
                {
                    Marshal.WriteByte(regelPtr, 4 * x, (byte)doelBuffer[3 * x]);
                    Marshal.WriteByte(regelPtr, 4 * x + 1, (byte)doelBuffer[3 * x + 1]);
                    Marshal.WriteByte(regelPtr, 4 * x + 2, (byte)doelBuffer[3 * x + 2]);
                }
                regelPtr = new IntPtr(regelPtr.ToInt64() + data.Stride);
            }

            Huidige.Bitmap.UnlockBits(data);
        }

        void CurveVerticaal(CurveVorm curveVorm, float proportie) 
        {
            BitmapData data = Huidige.Bitmap.LockBits(new Rectangle(Point.Empty, Huidige.Bitmap.Size),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr kolomPtr = data.Scan0;

            float[] doelBuffer = new float[3 * Huidige.Bitmap.Height];
            float middelpunt = (float)Huidige.Bitmap.Height / 2;

            for (int x = 0; x < Huidige.Bitmap.Width; x++)
            {
                form1.UpdateProgressBar(x * 100 / Huidige.Bitmap.Width);

                float krimpFactor =
                    geefKrimpFactor(curveVorm, proportie, x, Huidige.Bitmap.Width);

                // reset doelBuffer
                for (int i = 0; i < doelBuffer.Length; i++)
                    doelBuffer[i] = 0;

                for (int y = 0; y < Huidige.Bitmap.Height; y++) // itereer over bronpixels
                {
                    // lees bronpixel
                    byte blauw = Marshal.ReadByte(kolomPtr, data.Stride * y);
                    byte groen = Marshal.ReadByte(kolomPtr, data.Stride * y + 1);
                    byte rood = Marshal.ReadByte(kolomPtr, data.Stride * y + 2);

                    // map bronpixel naar doelpixels
                    float onderGrens = ((float)y - middelpunt) * krimpFactor + middelpunt;
                    float proportieEerstePixel =
                        Math.Min(((int)onderGrens + 1 - onderGrens), krimpFactor);

                    doelBuffer[3 * (int)onderGrens] += (proportieEerstePixel * blauw);
                    doelBuffer[3 * (int)onderGrens + 1] += (proportieEerstePixel * groen);
                    doelBuffer[3 * (int)onderGrens + 2] += (proportieEerstePixel * rood);

                    // als bronpixel over 2 doelpixels wordt verdeeld
                    if (proportieEerstePixel < krimpFactor)
                    {
                        doelBuffer[3 * (int)onderGrens + 3] += ((krimpFactor - proportieEerstePixel) * blauw);
                        doelBuffer[3 * (int)onderGrens + 4] += ((krimpFactor - proportieEerstePixel) * groen);
                        doelBuffer[3 * (int)onderGrens + 5] += ((krimpFactor - proportieEerstePixel) * rood);
                    }
                }

                // kopieer doelpixels naar bitmap
                for (int y = 0; y < Huidige.Bitmap.Height; y++)
                {
                    Marshal.WriteByte(kolomPtr, data.Stride * y, (byte)doelBuffer[3 * y]);
                    Marshal.WriteByte(kolomPtr, data.Stride * y + 1, (byte)doelBuffer[3 * y + 1]);
                    Marshal.WriteByte(kolomPtr, data.Stride * y + 2, (byte)doelBuffer[3 * y + 2]);
                }
                kolomPtr = new IntPtr(kolomPtr.ToInt64() + 4);
            }

            Huidige.Bitmap.UnlockBits(data);
        }

        float geefKrimpFactor(CurveVorm curveVorm, float proportie, int iterator, int bereik)
        {
            switch (curveVorm)
            {
                case CurveVorm.KegelAflopend:
                    return 1.0f - proportie * (iterator + 0.5f) / bereik;
                case CurveVorm.KegelOplopend:
                    return 1.0f - proportie + proportie * (iterator + 0.5f) / bereik;
                case CurveVorm.SinusNaarBinnen:
                    return 1.0f - proportie * (float)Math.Sin(Math.PI * (iterator + 0.5) / bereik);
                case CurveVorm.SinusNaarBuiten:
                    return 1.0f - proportie + proportie * (float)Math.Sin(Math.PI * (iterator + 0.5) / bereik);
                default:
                    return 1.0f;
            }
        }
    }
}
