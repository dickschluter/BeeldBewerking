using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Kleuren : BewerkingMetMasker
    {
        TrackBar[] trackBarCorrectie = new TrackBar[6];
        Button buttonToepassen;
        
        public Kleuren(Form1 form1)
            : base(form1)
        {
            Naam = "Kleuren";
            labelBewerking.Text = Naam;

            string[] correctie = { "Rood", "Groen", "Blauw", "Helderheid", "Kontrast", "Verzadiging" };
            for (int i = 0; i < 6; i++)
            {
                trackBarCorrectie[i] = new TrackBar();
                trackBarCorrectie[i].Width = 112;
                trackBarCorrectie[i].Location = new Point(44, 330 + 50 * i);
                trackBarCorrectie[i].Minimum = -10;
                trackBarCorrectie[i].Maximum = 10;
                trackBarCorrectie[i].TickFrequency = 5;
                trackBarCorrectie[i].MouseUp += new MouseEventHandler(trackBar_MouseUp);
                trackBarCorrectie[i].ValueChanged += new EventHandler(trackBar_ValueChanged);
                lijstControls.Add(trackBarCorrectie[i]);

                Label labelMin = new Label();
                labelMin.AutoSize = true;
                labelMin.Location = new Point(8, 28);
                labelMin.Font = new Font("Microsoft Sans Serif", 10f);
                labelMin.Text = "-";
                labelMin.Parent = trackBarCorrectie[i];
                lijstControls.Add(labelMin);

                Label labelCorrectie = new Label();
                labelCorrectie.Size = new Size(62, 13);
                labelCorrectie.Location = new Point(26, 30);
                labelCorrectie.Font = new Font("Microsoft Sans Serif", 7f);
                labelCorrectie.Text = correctie[i];
                labelCorrectie.TextAlign = ContentAlignment.TopCenter;
                labelCorrectie.Parent = trackBarCorrectie[i];
                lijstControls.Add(labelCorrectie);

                Label labelPlus = new Label();
                labelPlus.AutoSize = true;
                labelPlus.Location = new Point(92, 30);
                labelPlus.Text = "+";
                labelPlus.Parent = trackBarCorrectie[i];
                lijstControls.Add(labelPlus);
            }

            buttonToepassen = new Button();
            buttonToepassen.Location = new Point(50, 660);
            buttonToepassen.Size = new Size(100, 23);
            buttonToepassen.Text = "Toepassen";
            buttonToepassen.Click += new EventHandler(buttonToepassen_Click);
            lijstControls.Add(buttonToepassen);

            form1.BitmapViewer.Paint += viewer_Paint;
            form1.InitialiseerBewerking(lijstControls, true);
        }

        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < 6; i++)
                trackBarCorrectie[i].Value = 0;
        }
        
        protected override void viewer_Paint(object sender, PaintEventArgs e) // tijdelijk tekenen
        {
            if (gebruikKader)
            {
                if (kaderInBeeld)
                {
                    if (kaderVast)
                        using (Bitmap bitmapKader = maakBitmapKaderBewerkt())
                            e.Graphics.DrawImage(bitmapKader, doelRechthoek,
                                0, 0, kaderBreedte, kaderHoogte, GraphicsUnit.Pixel, attributes);

                    if (rechthoekigKader)
                        e.Graphics.DrawRectangle(Pens.Yellow, startpunt.X - kaderBreedte / 2 - 1,
                            startpunt.Y - kaderHoogte / 2 - 1, kaderBreedte + 1, kaderHoogte + 1);
                    else
                        e.Graphics.DrawEllipse(Pens.Yellow, startpunt.X - kaderBreedte / 2 - 1,
                        startpunt.Y - kaderHoogte / 2 - 1, kaderBreedte + 1, kaderHoogte + 1);
                }
            }
            else
                hulpAfbeelding(e.Graphics, Huidige.Bitmap, Point.Empty,
                    0, form1.BitmapViewer.Schaal, attributes);
        }

        protected override void viewer_MouseEnter(object sender, EventArgs e)
        {
            if (gebruikKader)
                kaderInBeeld = true;
        }

        protected override void viewer_MouseLeave(object sender, EventArgs e)
        {
            if (kaderVast == false)
            {
                kaderInBeeld = false;
                form1.BitmapViewer.Refresh();
            }
        }

        protected override void viewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (kaderInBeeld && kaderVast == false)
            {
                startpunt = e.Location;
                form1.BitmapViewer.Refresh();
            }
        }

        protected override void viewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (kaderInBeeld)
            {
                sluitHulpVenster();
                kaderVast = !kaderVast;
            }
        }

        void trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (kaderVast == false)
                trackBarVeranderd();
        }

        void trackBar_ValueChanged(object sender, EventArgs e)
        {
            if (kaderVast)
                trackBarVeranderd();
        }

        void trackBarVeranderd()
        {
            ColorMatrix colorMatrix = new ColorMatrix(); // R = cm[0,0]*Rbron + cm[1,0]*Gbron + cm[2,0]*Bbron + cm[4,0] etc
            for (int rij = 0; rij < 3; rij++)
                for (int kolom = 0; kolom < 3; kolom++)
                {
                    if (rij == kolom)
                        colorMatrix[rij, kolom] += trackBarCorrectie[4].Value * 0.02f; // kontrast
                    colorMatrix[rij, kolom] +=
                        (rij == kolom) ? trackBarCorrectie[5].Value * 0.02f : trackBarCorrectie[5].Value * -0.01f; // verzadiging
                }
            for (int kolom = 0; kolom < 3; kolom++)
            {
                colorMatrix[4, kolom] += (trackBarCorrectie[kolom].Value + trackBarCorrectie[3].Value) * 0.01f; // RGB en helderheid
                colorMatrix[4, kolom] += trackBarCorrectie[4].Value * -0.01f; // kontrast
            }

            attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            form1.BitmapViewer.Refresh();
        }

        void buttonToepassen_Click(object sender, EventArgs e)
        {
            bool allesOpNul = true;
            foreach (var trackBar in trackBarCorrectie)
                if (trackBar.Value != 0)
                    allesOpNul = false;
            if (allesOpNul) // niet uitvoeren als alle regelaars op nul staan
                return;

            if (gebruikKader)
            {
                if (kaderVast)
                {
                    using (Bitmap bitmapKaderBewerkt = maakBitmapKaderBewerkt())
                    using (Graphics graphics = Graphics.FromImage(Huidige.Bitmap))
                        graphics.DrawImage(bitmapKaderBewerkt, doelRechthoek,
                            0, 0, kaderBreedte, kaderHoogte, GraphicsUnit.Pixel, attributes);
                }
                else
                    if (MessageBox.Show("Geen doelgebied geselecteerd") == DialogResult.OK)
                        return;
            }
            else
                using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                    g.DrawImage(Huidige.Bitmap, new Rectangle(Point.Empty, Huidige.Bitmap.Size),
                        0, 0, Huidige.Bitmap.Width, Huidige.Bitmap.Height, GraphicsUnit.Pixel, attributes);

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();

            Reset();
        }
        
        Bitmap maakBitmapKaderBewerkt()
        {
            bool metMasker = (hulpVenster != null && hulpVenster.IsDisposed == false);

            doelRechthoek =
                new Rectangle(startpunt.X - kaderBreedte / 2, startpunt.Y - kaderHoogte / 2,
                    kaderBreedte, kaderHoogte);

            GraphicsPath pad = new GraphicsPath();
            if (rechthoekigKader)
                pad.AddRectangle(new Rectangle(0, 0, kaderBreedte, kaderHoogte));
            else
                pad.AddEllipse(0, 0, kaderBreedte - 1, kaderHoogte - 1);
            Region regionKader = new Region(pad);

            Bitmap bitmapKader = new Bitmap(kaderBreedte, kaderHoogte);
            using (Graphics g = Graphics.FromImage(bitmapKader))
                g.DrawImage(Huidige.Bitmap, 0, 0, doelRechthoek, GraphicsUnit.Pixel);
            for (int x = 0; x < bitmapKader.Width; x++)
                for (int y = 0; y < bitmapKader.Height; y++)
                    if (regionKader.IsVisible(x, y) == false || (metMasker && HuidigMasker.Gemaskeerd[x, y]))
                        bitmapKader.SetPixel(x, y, Color.Black);

            bitmapKader.MakeTransparent(Color.Black);
            return bitmapKader;
        }
    }
}
