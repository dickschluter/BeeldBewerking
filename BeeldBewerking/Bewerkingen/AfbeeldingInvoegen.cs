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
    class AfbeeldingInvoegen : BewerkingMetHulpfiguren
    {
        GroupBox groupBoxTransparantie, groupBoxOvervloeien;
        ComboBox comboBoxAfbeelding;
        NumericUpDown numericHoek, numericSchaal, numericDekking, numericBereik, numericPixels;
        RadioButton[] radioKleur = new RadioButton[4];
        CheckBox[] checkBoxHor = new CheckBox[2];
        CheckBox[] checkBoxVert = new CheckBox[2];
        
        Bitmap bitmapInvoegen;

        bool bewerkbareAfbeelding; // Cursor en Hand zijn niet bewerkbaar
        bool muisBevatAfbeelding; // true na MouseEnter, false na invoegen afbeelding

        public AfbeeldingInvoegen(Form1 form1)
            : base(form1)
        {
            Naam = "Afbeelding invoegen";
            labelBewerking.Text = Naam;

            comboBoxAfbeelding = new ComboBox();
            comboBoxAfbeelding.Size = new Size(100, 21);
            comboBoxAfbeelding.Location = new Point(50, 120);
            comboBoxAfbeelding.Items.AddRange(new object[] { "Cursor", "Hand", "Uit bestand" });
            foreach (string naam in BewaardeBitmaps.VerkorteNamen)
                comboBoxAfbeelding.Items.Add(naam);
            comboBoxAfbeelding.SelectedIndex = 0;
            comboBoxAfbeelding.SelectedIndexChanged += comboBoxAfbeelding_SelectedIndexChanged;
            lijstControls.Add(comboBoxAfbeelding);

            Label label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new Point(47, 153);
            label1.Text = "Hoek";
            lijstControls.Add(label1);

            numericHoek = new NumericUpDown();
            numericHoek.Size = new Size(44, 20);
            numericHoek.Location = new Point(106, 150);
            numericHoek.Maximum = 180;
            numericHoek.Minimum = -180;
            numericHoek.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericHoek);

            Label label2 = new Label();
            label2.AutoSize = true;
            label2.Location = new Point(47, 183);
            label2.Text = "Schaal";
            lijstControls.Add(label2);

            numericSchaal = new NumericUpDown();
            numericSchaal.Size = new Size(44, 20);
            numericSchaal.Location = new Point(106, 180);
            numericSchaal.Minimum = 1;
            numericSchaal.Value = 100;
            numericSchaal.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericSchaal);

            Label label3 = new Label();
            label3.AutoSize = true;
            label3.Location = new Point(47, 213);
            label3.Text = "Dekking";
            lijstControls.Add(label3);

            numericDekking = new NumericUpDown();
            numericDekking.Size = new Size(44, 20);
            numericDekking.Location = new Point(106, 210);
            numericDekking.Increment = 10;
            numericDekking.Minimum = 10;
            numericDekking.Value = 100;
            numericDekking.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericDekking);

            groupBoxTransparantie = new GroupBox();
            groupBoxTransparantie.Size = new Size(140, 150);
            groupBoxTransparantie.Location = new Point(30, 260);
            groupBoxTransparantie.Text = "Transparante kleur";
            lijstControls.Add(groupBoxTransparantie);

            Label label4 = new Label();
            label4.AutoSize = true;
            label4.Location = new Point(17, 33);
            label4.Text = "Bereik";
            label4.Parent = groupBoxTransparantie;
            lijstControls.Add(label4);

            numericBereik = new NumericUpDown();
            numericBereik.Size = new Size(44, 20);
            numericBereik.Location = new Point(76, 30);
            numericBereik.Maximum = 255;
            numericBereik.MouseUp += (sender, e) => buttonFocus.Focus();
            numericBereik.Parent = groupBoxTransparantie;
            lijstControls.Add(numericBereik);

            string[] kleuren = { "Geen", "Blauw", "Wit", "Zwart" };
            for (int i = 0; i < 4; i++)
            {
                radioKleur[i] = new RadioButton();
                radioKleur[i].AutoSize = true;
                radioKleur[i].Location = new Point(20, 60 + i * 20);
                radioKleur[i].Checked = (i == 0);
                radioKleur[i].Text = kleuren[i];
                radioKleur[i].Parent = groupBoxTransparantie;
                lijstControls.Add(radioKleur[i]);
            }

            groupBoxOvervloeien = new GroupBox();
            groupBoxOvervloeien.Size = new Size(140, 150);
            groupBoxOvervloeien.Location = new Point(30, 440);
            groupBoxOvervloeien.Text = "Overvloeien";
            lijstControls.Add(groupBoxOvervloeien);

            Label label5 = new Label();
            label5.AutoSize = true;
            label5.Location = new Point(17, 33);
            label5.Text = "Pixels";
            label5.Parent = groupBoxOvervloeien;
            lijstControls.Add(label5);

            numericPixels = new NumericUpDown();
            numericPixels.Size = new Size(44, 20);
            numericPixels.Location = new Point(76, 30);
            numericPixels.Maximum = 500;
            numericPixels.MouseUp += (sender, e) => buttonFocus.Focus();
            numericPixels.Parent = groupBoxOvervloeien;
            lijstControls.Add(numericPixels);

            for (int i = 0; i < 2; i++)
            {
                checkBoxHor[i] = new CheckBox();
                checkBoxHor[i].AutoSize = true;
                checkBoxHor[i].Location = new Point(20, 60 + i * 20);
                checkBoxHor[i].Text = (i == 0 ? "Links" : "Rechts");
                checkBoxHor[i].Parent = groupBoxOvervloeien;
                lijstControls.Add(checkBoxHor[i]);

                checkBoxVert[i] = new CheckBox();
                checkBoxVert[i].AutoSize = true;
                checkBoxVert[i].Location = new Point(20, 100 + i * 20);
                checkBoxVert[i].Text = (i == 0 ? "Boven" : "Onder");
                checkBoxVert[i].Parent = groupBoxOvervloeien;
                lijstControls.Add(checkBoxVert[i]);
            }

            bitmapInvoegen = Properties.Resources.Cursor;
            bitmapInvoegen.MakeTransparent(Color.Blue);

            form1.InitialiseerBewerking(lijstControls, true);
        }

        void comboBoxAfbeelding_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAfbeelding.SelectedIndex)
            {
                case 0:
                    bitmapInvoegen = Properties.Resources.Cursor;
                    bitmapInvoegen.MakeTransparent(Color.Blue);
                    form1.BitmapViewer.Cursor = Cursors.Default;
                    bewerkbareAfbeelding = false;
                    break;
                case 1:
                    bitmapInvoegen = Properties.Resources.Hand;
                    bitmapInvoegen.MakeTransparent(Color.Blue);
                    form1.BitmapViewer.Cursor = Cursors.Hand;
                    bewerkbareAfbeelding = false;
                    break;
                case 2:
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory =
                        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    openFileDialog.Filter =
                        "Alle afbeeldingen|*.jpg;*.bmp;*.gif;*.png;*.ico";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (Bitmap tijdelijkeBitmap = new Bitmap(openFileDialog.FileName))
                                bitmapInvoegen = new Bitmap(tijdelijkeBitmap);
                            form1.BitmapViewer.Cursor = Cursors.Cross;
                            bewerkbareAfbeelding = true;
                        }
                        catch
                        {
                            MessageBox.Show("Bestand bevat geen afbeelding");
                        }
                    }
                    break;
                default:
                    int index = comboBoxAfbeelding.SelectedIndex - 3;
                    bitmapInvoegen = BewaardeBitmaps.GeefBitmap(index);
                    form1.BitmapViewer.Cursor = Cursors.Cross;
                    bewerkbareAfbeelding = true;
                    break;
            }

            if (bewerkbareAfbeelding) // ingevoegde bitmap is minimaal 10 pixels
            {
                int kleinsteAfmeting = Math.Min(bitmapInvoegen.Width, bitmapInvoegen.Height);
                if (kleinsteAfmeting >= 1000)
                    numericSchaal.Minimum = 1;
                else
                    numericSchaal.Minimum =
                        1000 / kleinsteAfmeting;
            }

            buttonFocus.Focus();
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e) // tijdelijk tekenen
        {
            decimal tekenSchaal = form1.BitmapViewer.Schaal * numericSchaal.Value / 100;
            hulpAfbeelding(e.Graphics, bitmapInvoegen, startpunt,
                numericHoek.Value, tekenSchaal, attributes);
        }

        protected override void viewer_MouseEnter(object sender, EventArgs e)
        {
            attributes = new ImageAttributes();

            if (bewerkbareAfbeelding)
            {
                muisBevatAfbeelding = true;

                if (numericDekking.Value < 100)
                {
                    ColorMatrix colorMatrix = new ColorMatrix();
                    colorMatrix.Matrix33 = (float)numericDekking.Value / 100;
                    attributes.SetColorMatrix(colorMatrix);
                }
                if (radioKleur[0].Checked == false)
                {
                    int b = (int)numericBereik.Value;
                    if (radioKleur[1].Checked) // blauw
                        attributes.SetColorKey(
                            Color.FromArgb(0, 0, 255 - b), Color.FromArgb(b, b, 255));
                    else if (radioKleur[2].Checked) // wit
                        attributes.SetColorKey(
                            Color.FromArgb(255 - b, 255 - b, 255 - b), Color.FromArgb(255, 255, 255));
                    else if (radioKleur[3].Checked) // zwart
                        attributes.SetColorKey(
                            Color.FromArgb(0, 0, 0), Color.FromArgb(b, b, b));
                }

                form1.BitmapViewer.Paint += viewer_Paint;
            }
        }

        protected override void viewer_MouseLeave(object sender, EventArgs e)
        {
            muisBevatAfbeelding = false;
            form1.BitmapViewer.Paint -= viewer_Paint;
            form1.BitmapViewer.Refresh();
        }

        protected override void viewer_MouseMove(object sender, MouseEventArgs e) // tijdelijk tekenen
        {
            if (muisBevatAfbeelding)
            {
                startpunt = e.Location;
                form1.BitmapViewer.Refresh();
            }
        }

        protected override void viewer_MouseDown(object sender, MouseEventArgs e) // definitief invoegen
        {
            if (bewerkbareAfbeelding == false || muisBevatAfbeelding)
            {
                using (Bitmap bitmapBewerkt = new Bitmap(bitmapInvoegen))
                {
                    if (bewerkbareAfbeelding && numericPixels.Value > 0 &&
                    (checkBoxHor[0].Checked || checkBoxHor[1].Checked ||
                    checkBoxVert[0].Checked || checkBoxVert[1].Checked))
                        maakOvervloeiGradient(bitmapBewerkt);

                    using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                    {
                        g.TranslateTransform(
                            (float)(e.X / form1.BitmapViewer.Schaal), (float)(e.Y / form1.BitmapViewer.Schaal));
                        if (comboBoxAfbeelding.SelectedIndex == 1) // Hand
                            g.TranslateTransform(-5, 0);
                        if (bewerkbareAfbeelding)
                        {
                            g.ScaleTransform((float)numericSchaal.Value / 100,
                                (float)numericSchaal.Value / 100);
                            g.RotateTransform((float)numericHoek.Value);
                        }
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(bitmapBewerkt, new Rectangle(Point.Empty, bitmapBewerkt.Size),
                            0, 0, bitmapBewerkt.Width, bitmapBewerkt.Height, GraphicsUnit.Pixel, attributes);
                    }
                }

                Geschiedenis.HuidigeBitmapToevoegen();
                form1.ToonBitmap();
            }

            muisBevatAfbeelding = false;
            form1.BitmapViewer.Paint -= viewer_Paint;
            form1.BitmapViewer.Refresh();
        }

        void maakOvervloeiGradient(Bitmap bitmap)
        {
            if ((bitmap.Width * bitmap.Height / 1000) * numericPixels.Value > 1000)
                form1.StartProgressBar();

            BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int aantalLijnen = (int)(numericPixels.Value * 100 / numericSchaal.Value);
            int intLinks = checkBoxHor[0].Checked ? 1 : 0;
            int intRechts = checkBoxHor[1].Checked ? 1 : 0;
            int intBoven = checkBoxVert[0].Checked ? 1 : 0;
            int intOnder = checkBoxVert[1].Checked ? 1 : 0;

            for (int d = 0; d < aantalLijnen; d++) // d is aantal pixels tot de rand van de bitmap
            {
                form1.UpdateProgressBar(100 * d / aantalLijnen);

                byte gradient =
                    (byte)(255 * Math.Sin((double)(d + 1) / ((double)aantalLijnen + 1) * 1.57));

                if (checkBoxHor[0].Checked && d < bitmap.Width / (1 + intRechts))
                    for (int y = d * intBoven; y < bitmap.Height - d * intOnder; y++)
                    {
                        IntPtr pixelPtr = new IntPtr(data.Scan0.ToInt64() + y * data.Stride + d * 4);
                        byte alfa = (Marshal.ReadByte(pixelPtr, 3) > 0 ? gradient : (byte)0);
                        Marshal.WriteByte(pixelPtr, 3, alfa);
                    }
                if (checkBoxHor[1].Checked && d < bitmap.Width / (1 + intLinks))
                    for (int y = d * intBoven; y < bitmap.Height - d * intOnder; y++)
                    {
                        IntPtr pixelPtr =
                            new IntPtr(data.Scan0.ToInt64() + y * data.Stride + (bitmap.Width - 1 - d) * 4);
                        byte alfa = (Marshal.ReadByte(pixelPtr, 3) > 0 ? gradient : (byte)0);
                        Marshal.WriteByte(pixelPtr, 3, alfa);
                    }
                if (checkBoxVert[0].Checked && d < bitmap.Height / (1 + intOnder))
                    for (int x = d * intLinks; x < bitmap.Width - d * intRechts; x++)
                    {
                        IntPtr pixelPtr = new IntPtr(data.Scan0.ToInt64() + d * data.Stride + x * 4);
                        byte alfa = (Marshal.ReadByte(pixelPtr, 3) > 0 ? gradient : (byte)0);
                        Marshal.WriteByte(pixelPtr, 3, alfa);
                    }
                if (checkBoxVert[1].Checked && d < bitmap.Height / (1 + intBoven))
                    for (int x = d * intLinks; x < bitmap.Width - d * intRechts; x++)
                    {
                        IntPtr pixelPtr =
                            new IntPtr(data.Scan0.ToInt64() + (bitmap.Height - 1 - d) * data.Stride + x * 4);
                        byte alfa = (Marshal.ReadByte(pixelPtr, 3) > 0 ? gradient : (byte)0);
                        Marshal.WriteByte(pixelPtr, 3, alfa);
                    }
            }

            bitmap.UnlockBits(data);
            form1.StopProgressBar();
        }
    }
}
