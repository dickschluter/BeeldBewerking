using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Roteren : BewerkingMetHulpfiguren
    {
        Button button90, button180, button270, buttonToepassen;
        GroupBox groupBox;
        TrackBar trackBarGrof, trackBarFijn;
        Label[] labelTrackBar = new Label[4];
        CheckBox checkBoxRaster;

        float hoek;

        public Roteren(Form1 form1)
            : base(form1)
        {
            Naam = "Roteren";
            labelBewerking.Text = Naam;

            button90 = new Button();
            button90.Location = new Point(50, 120);
            button90.Size = new Size(100, 23);
            button90.Text = "90";
            button90.Click += new EventHandler(buttonHoek_Click);
            lijstControls.Add(button90);

            button180 = new Button();
            button180.Location = new Point(50, 150);
            button180.Size = new Size(100, 23);
            button180.Text = "180";
            button180.Click += new EventHandler(buttonHoek_Click);
            lijstControls.Add(button180);

            button270 = new Button();
            button270.Location = new Point(50, 180);
            button270.Size = new Size(100, 23);
            button270.Text = "270";
            button270.Click += new EventHandler(buttonHoek_Click);
            lijstControls.Add(button270);

            groupBox = new GroupBox();
            groupBox.Size = new Size(140, 240);
            groupBox.Location = new Point(30, 240);
            groupBox.Text = "Andere hoek";
            lijstControls.Add(groupBox);

            trackBarGrof = new TrackBar();
            trackBarGrof.Width = 112;
            trackBarGrof.Location = new Point(14, 40);
            trackBarGrof.Minimum = -18;
            trackBarGrof.Maximum = 18;
            trackBarGrof.TickFrequency = 6;
            trackBarGrof.ValueChanged += new EventHandler(trackBar_ValueChanged);
            trackBarGrof.MouseUp += new MouseEventHandler(trackBar_MouseUp);
            trackBarGrof.Parent = groupBox;
            lijstControls.Add(trackBarGrof);

            trackBarFijn = new TrackBar();
            trackBarFijn.Width = 112;
            trackBarFijn.Location = new Point(14, 90);
            trackBarFijn.Minimum = -24;
            trackBarFijn.Maximum = 24;
            trackBarFijn.TickFrequency = 8;
            trackBarFijn.ValueChanged += new EventHandler(trackBar_ValueChanged);
            trackBarFijn.MouseUp += new MouseEventHandler(trackBar_MouseUp);
            trackBarFijn.Parent = groupBox;
            lijstControls.Add(trackBarFijn);

            string[] hoeken = { "-90", "90", "-5", "5" };
            for (int i = 0; i < 4; i++)
            {
                labelTrackBar[i] = new Label();
                labelTrackBar[i].AutoSize = true;
                labelTrackBar[i].Location = new Point((i % 2 == 0 ? 6 : 95), 30);
                labelTrackBar[i].Font = new Font("Microsoft Sans Serif", 6f);
                labelTrackBar[i].Text = hoeken[i];
                labelTrackBar[i].Parent = (i < 2 ? trackBarGrof : trackBarFijn);
                lijstControls.Add(labelTrackBar[i]);
            }

            checkBoxRaster = new CheckBox();
            checkBoxRaster.Location = new Point(20, 150);
            checkBoxRaster.Text = "Toon raster";
            checkBoxRaster.CheckedChanged += (sender, e) => form1.BitmapViewer.Refresh();
            checkBoxRaster.Parent = groupBox;
            lijstControls.Add(checkBoxRaster);

            buttonToepassen = new Button();
            buttonToepassen.Location = new Point(20, 200);
            buttonToepassen.Size = new Size(100, 23);
            buttonToepassen.Text = "Toepassen";
            buttonToepassen.Click += new EventHandler(buttonToepassen_Click);
            buttonToepassen.Parent = groupBox;
            lijstControls.Add(buttonToepassen);

            form1.InitialiseerBewerking(lijstControls, true);
            form1.BitmapViewer.Paint += viewer_Paint;
        }

        public override void Reset()
        {
            trackBarGrof.Value = 0;
            trackBarFijn.Value = 0;
            checkBoxRaster.Checked = false;
        }

        void buttonHoek_Click(object sender, EventArgs e)
        {
            RotateFlipType flipType = RotateFlipType.Rotate90FlipNone;
            if (sender == button180)
                flipType = RotateFlipType.Rotate180FlipNone;
            else if (sender == button270)
                flipType = RotateFlipType.Rotate270FlipNone;

            Huidige.Bitmap.RotateFlip(flipType);

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();

            Reset();
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e) // tijdelijk tekenen
        {
            Point middelpunt = new Point(form1.BitmapViewer.Width / 2, form1.BitmapViewer.Height / 2);
            e.Graphics.Clear(Color.White);
            e.Graphics.TranslateTransform(middelpunt.X, middelpunt.Y);
            e.Graphics.ScaleTransform((float)form1.BitmapViewer.Schaal, (float)form1.BitmapViewer.Schaal);
            e.Graphics.RotateTransform(hoek);
            e.Graphics.DrawImage(Huidige.Bitmap, -Huidige.Bitmap.Width / 2, -Huidige.Bitmap.Height / 2);

            if (checkBoxRaster.Checked)
            {
                e.Graphics.RotateTransform(-hoek);
                e.Graphics.ScaleTransform(1.0f / (float)form1.BitmapViewer.Schaal, 1.0f / (float)form1.BitmapViewer.Schaal);
                int x = 0, y = 0;
                while (x < middelpunt.X)
                {
                    e.Graphics.DrawLine(Pens.Yellow, -x, -middelpunt.Y, -x, middelpunt.Y);
                    e.Graphics.DrawLine(Pens.Yellow, x, -middelpunt.Y, x, middelpunt.Y);
                    x += 40;
                }
                while (y < middelpunt.Y)
                {
                    e.Graphics.DrawLine(Pens.Yellow, -middelpunt.X, -y, middelpunt.X, -y);
                    e.Graphics.DrawLine(Pens.Yellow, -middelpunt.X, y, middelpunt.X, y);
                    y += 40;
                }
            }
        }

        void trackBar_ValueChanged(object sender, EventArgs e)
        {
            hoek = (float)trackBarGrof.Value * 5 + (float)trackBarFijn.Value * 0.2f;
            groupBox.Text = string.Format("Andere hoek = {0}", hoek.ToString("0.0"));
            form1.BitmapViewer.Refresh();
        }

        void trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                (sender as TrackBar).Value = 0;
        }

        void buttonToepassen_Click(object sender, EventArgs e)
        {
            if (hoek != 0)
            {
                double radialen = hoek * Math.PI / 180.0;
                int nieuweBreedte = (int)(Math.Abs(Math.Cos(radialen)) * Huidige.Bitmap.Width
                    + Math.Abs(Math.Sin(radialen)) * Huidige.Bitmap.Height);
                int nieuweHoogte = (int)(Math.Abs(Math.Sin(radialen)) * Huidige.Bitmap.Width
                    + Math.Abs(Math.Cos(radialen)) * Huidige.Bitmap.Height);

                Bitmap vorigeBitmap = Huidige.Bitmap;
                Huidige.Bitmap = new Bitmap(nieuweBreedte, nieuweHoogte);
                using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                {
                    g.Clear(Color.White);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.TranslateTransform((float)nieuweBreedte / 2, (float)nieuweHoogte / 2);
                    g.RotateTransform(hoek);
                    g.DrawImage(vorigeBitmap, -vorigeBitmap.Width / 2, -vorigeBitmap.Height / 2);
                }

                Geschiedenis.HuidigeBitmapToevoegen();
                form1.ToonBitmap();

                Reset();
            }
        }
    }
}
