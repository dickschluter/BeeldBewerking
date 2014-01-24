using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class DetailTekenen : BewerkingMetHulpfiguren
    {
        TextBox textBoxKleur;
        NumericUpDown numericDekking;
        Button buttonFixeren;

        readonly int kaderGrootte = 55;
        bool huidigeBitmapGewijzigd;

        public Color TekenKleur
        {
            get { return textBoxKleur.BackColor; }
            set { textBoxKleur.BackColor = value; }
        }

        public decimal Dekking { get { return numericDekking.Value / 100; } }

        public DetailTekenen(Form1 form1)
            : base(form1)
        {
            Naam = "Detail tekenen";
            labelBewerking.Text = Naam;

            for (int i = 0; i < 2; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Location = new Point(47, 123 + 30 * i);
                label.Text = (i == 0) ? "Kleur" : "Dekking";
                lijstControls.Add(label);
            }

            textBoxKleur = new TextBox();
            textBoxKleur.Size = new Size(44, 20);
            textBoxKleur.Location = new Point(106, 120);
            textBoxKleur.BackColor = Color.Black;
            textBoxKleur.ReadOnly = true;
            textBoxKleur.Click += new EventHandler(textBoxKleur_Click);
            lijstControls.Add(textBoxKleur);

            numericDekking = new NumericUpDown();
            numericDekking.Size = new Size(44, 20);
            numericDekking.Location = new Point(106, 150);
            numericDekking.Increment = 10;
            numericDekking.Minimum = 10;
            numericDekking.Value = 100;
            numericDekking.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericDekking);

            buttonFixeren = new Button();
            buttonFixeren.Enabled = false;
            buttonFixeren.Size = new Size(100, 23);
            buttonFixeren.Location = new Point(50, 210);
            buttonFixeren.Text = "Fixeren";
            buttonFixeren.Click += new EventHandler(buttonFixeren_Click);
            lijstControls.Add(buttonFixeren);

            form1.BitmapViewer.Cursor = Cursors.Cross;
            form1.BitmapViewer.Paint += viewer_Paint;
            form1.InitialiseerBewerking(lijstControls, true);
        }

        protected override void pictureBoxVorige_Click(object sender, EventArgs e)
        {
            BitmapMetNaam vorigeBitmapMetNaam;
            if (huidigeBitmapGewijzigd)
            {
                if (Geschiedenis.LaatstToegevoegd(out vorigeBitmapMetNaam))
                    Huidige.Bitmap = vorigeBitmapMetNaam.Bitmap;
            }
            else if (Geschiedenis.Vorige(out vorigeBitmapMetNaam))
            {
                Huidige.Naam = vorigeBitmapMetNaam.Naam;
                Huidige.Bitmap = vorigeBitmapMetNaam.Bitmap;
            }
            form1.ToonBitmap();

            Reset();
        }

        public override void Reset()
        {
            huidigeBitmapGewijzigd = false;
            buttonFixeren.Enabled = false;
            sluitHulpVenster();
        }

        public void BitmapIsGewijzigd()
        {
            huidigeBitmapGewijzigd = true;
            buttonFixeren.Enabled = true;
            pictureBoxVorige.Visible = true;
        }

        public void HulpVensterGesloten()
        {
            kaderInBeeld = kaderVast = false;
            form1.BitmapViewer.Refresh();
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e)
        {
            if (kaderInBeeld && form1.BitmapViewer.Schaal == 1)
                e.Graphics.DrawRectangle(Pens.Yellow, startpunt.X - kaderGrootte / 2,
                    startpunt.Y - kaderGrootte / 2, kaderGrootte - 1, kaderGrootte - 1);
        }

        protected override void viewer_MouseEnter(object sender, EventArgs e)
        {
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

        protected override void viewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (kaderVast == false)
            {
                using (Bitmap bitmapKader = new Bitmap(kaderGrootte, kaderGrootte))
                using (Graphics g = Graphics.FromImage(bitmapKader))
                {
                    doelRechthoek = new Rectangle(
                        (int)(e.X / form1.BitmapViewer.Schaal - kaderGrootte / 2),
                        (int)(e.Y / form1.BitmapViewer.Schaal - kaderGrootte / 2),
                        kaderGrootte, kaderGrootte);
                    g.DrawImage(Huidige.Bitmap, 0, 0, doelRechthoek, GraphicsUnit.Pixel);
                    hulpVenster = FormDetail.GeefInstantie(
                        form1, this, bitmapKader, doelRechthoek.X, doelRechthoek.Y);
                }
                hulpVenster.Show(form1);
            }
            else
                sluitHulpVenster();

            kaderVast = !kaderVast;
        }

        void textBoxKleur_Click(object sender, EventArgs e)
        {
            if (DialoogVensters.KleurDialoog.ShowDialog() == DialogResult.OK)
                textBoxKleur.BackColor = DialoogVensters.KleurDialoog.Color;
            else
                buttonFocus.Focus();
        }

        void buttonFixeren_Click(object sender, EventArgs e)
        {
            Geschiedenis.HuidigeBitmapToevoegen();
            Reset();
        }
    }
}
