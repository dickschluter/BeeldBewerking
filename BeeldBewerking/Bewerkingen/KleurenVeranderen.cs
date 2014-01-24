using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class KleurenVeranderen : BewerkingMetMasker
    {
        public ColorMatrix KleurenMatrix { get; set; } // R = cm[0,0]*Rbron + cm[1,0]*Gbron + cm[2,0]*Bbron + cm[4,0] etc

        Button[] buttonEffect = new Button[4];
        Button buttonPupilCorrectie;
        
        public KleurenVeranderen(Form1 form1)
            : base(form1)
        {
            Naam = "Kleuren veranderen";
            labelBewerking.Text = Naam;

            string[] effecten = { "Kleurenmatrix", "Zwartwit", "Negatief", "Primaire kleuren" };
            for (int i = 0; i < buttonEffect.Length; i++)
            {
                buttonEffect[i] = new Button();
                buttonEffect[i].Location = new Point(50, 360 + 30 * i);
                buttonEffect[i].Size = new Size(100, 23);
                buttonEffect[i].Text = effecten[i];
                buttonEffect[i].Click += new EventHandler(buttonEffect_Click);
                lijstControls.Add(buttonEffect[i]);
            }

            buttonPupilCorrectie = new Button();
            buttonPupilCorrectie.Location = new Point(50, 480);
            buttonPupilCorrectie.Size = new Size(100, 23);
            buttonPupilCorrectie.Text = "Pupilcorrectie";
            buttonPupilCorrectie.Click += new EventHandler(buttonPupilCorrectie_Click);
            lijstControls.Add(buttonPupilCorrectie);

            form1.BitmapViewer.Paint += viewer_Paint;
            form1.InitialiseerBewerking(lijstControls, true);
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e) // tijdelijk tekenen
        {
            if (kaderInBeeld)
            {
                if(rechthoekigKader)
                    e.Graphics.DrawRectangle(Pens.Yellow, startpunt.X - kaderBreedte / 2 - 1,
                        startpunt.Y - kaderHoogte / 2 - 1, kaderBreedte + 1, kaderHoogte + 1);
                else
                    e.Graphics.DrawEllipse(Pens.Yellow, startpunt.X - kaderBreedte / 2 - 1,
                    startpunt.Y - kaderHoogte / 2 - 1, kaderBreedte + 1, kaderHoogte + 1);
            }
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

        void buttonEffect_Click(object sender, EventArgs e)
        {
            attributes = new ImageAttributes();

            switch ((sender as Control).Text)
            {
                case "Kleurenmatrix":
                    FormColorMatrix formColorMatrix = FormColorMatrix.GeefInstantie(this);
                    formColorMatrix.ShowDialog();
                    attributes.SetColorMatrix(KleurenMatrix);
                    break;
                case "Zwartwit":
                    KleurenMatrix = new ColorMatrix();
                    for (int kolom = 0; kolom < 3; kolom++)
                    {
                        KleurenMatrix[0, kolom] = 0.30f;
                        KleurenMatrix[1, kolom] = 0.59f;
                        KleurenMatrix[2, kolom] = 0.11f;
                    }
                    attributes.SetColorMatrix(KleurenMatrix);
                    break;
                case "Negatief":
                    KleurenMatrix = new ColorMatrix();
                    KleurenMatrix.Matrix00 = -1f;
                    KleurenMatrix.Matrix11 = -1f;
                    KleurenMatrix.Matrix22 = -1f;
                    KleurenMatrix.Matrix40 = 1f;
                    KleurenMatrix.Matrix41 = 1f;
                    KleurenMatrix.Matrix42 = 1f;
                    attributes.SetColorMatrix(KleurenMatrix);
                    break;
                case "Primaire kleuren":
                    attributes.SetThreshold(0.5f);
                    break;
            }

            kleurenVeranderen(false);
        }

        void buttonPupilCorrectie_Click(object sender, EventArgs e)
        {
            if (kaderVast == false && MessageBox.Show("Pupilcorrectie werkt alleen binnen een kader") == DialogResult.OK)
                    return;
            kleurenVeranderen(true);
        }

        void kleurenVeranderen(bool metPupilCorrectie)
        {
            if (gebruikKader)
            {
                if (kaderVast)
                {
                    using (Bitmap bitmapKaderBewerkt = maakBitmapKaderBewerkt(metPupilCorrectie))
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

        Bitmap maakBitmapKaderBewerkt(bool metPupilCorrectie)
        {
            bool metMasker = (hulpVenster != null && hulpVenster.IsDisposed == false);

            doelRechthoek =
                new Rectangle(startpunt.X - kaderBreedte / 2, startpunt.Y - kaderHoogte / 2,
                    kaderBreedte, kaderHoogte);

            GraphicsPath pad = new GraphicsPath();
            if (rechthoekigKader)
                pad.AddRectangle(new Rectangle(0, 0, kaderBreedte, kaderHoogte));
            else
                pad.AddEllipse(0, 0, kaderBreedte - 1, kaderHoogte -1);
            Region regionKader = new Region(pad);

            Bitmap bitmapKader = new Bitmap(kaderBreedte, kaderHoogte);
            using (Graphics g = Graphics.FromImage(bitmapKader))
                g.DrawImage(Huidige.Bitmap, 0, 0, doelRechthoek, GraphicsUnit.Pixel);
            for (int x = 0; x < bitmapKader.Width; x++)
                for (int y = 0; y < bitmapKader.Height; y++)
                    if (regionKader.IsVisible(x, y) == false || (metMasker && HuidigMasker.Gemaskeerd[x, y]))
                        bitmapKader.SetPixel(x, y, Color.Black);

            if (metPupilCorrectie)
                pupilCorrectie(bitmapKader);

            bitmapKader.MakeTransparent(Color.Black);

            return bitmapKader;
        }

        void pupilCorrectie(Bitmap bitmapKader)
        {
            for (int x = 0; x < bitmapKader.Width; x++)
                for (int y = 0; y < bitmapKader.Height; y++)
                {
                    int r = bitmapKader.GetPixel(x, y).R;
                    int g = bitmapKader.GetPixel(x, y).G;
                    int b = bitmapKader.GetPixel(x, y).B;
                    int roodNieuw = (r > Math.Max(g, b)) ? (9 * Math.Max(g, b) + r) / 10 : r;
                    bitmapKader.SetPixel(x, y, Color.FromArgb(roodNieuw, g, b));
                }
        }
    }
}
