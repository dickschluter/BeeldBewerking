using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class FormDetail : FormVergroting
    {
        DetailTekenen bewerking;
        int xDoel, yDoel;

        public static FormVergroting GeefInstantie(
            Form1 form1, DetailTekenen bewerking, Bitmap bitmap, int xDoel, int yDoel)
        {
            if (formVergroting == null)
                formVergroting = new FormDetail(form1, bewerking, bitmap, xDoel, yDoel);
            return formVergroting;
        }

        protected FormDetail(Form1 form1, DetailTekenen bewerking, Bitmap bitmap, int xDoel, int yDoel)
            : base(form1, bitmap)
        {
            this.bewerking = bewerking;
            this.xDoel = xDoel;
            this.yDoel = yDoel;
            this.Text = "L=Tekenen    R=Kleur kiezen";

            pictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseClick);
            pictureBox.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
        }

        void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > 0 && e.X < pictureBox.Width && e.Y > 0 && e.Y < pictureBox.Height
                && bitmapVergroting.GetPixel(e.X, e.Y).A > 0)
            {
                int x = e.X / 8, y = e.Y / 8;
                if (e.Button == MouseButtons.Left) // tekenen
                {
                    Color nieuweKleur = geefMengKleur(
                        bewerking.TekenKleur, Huidige.Bitmap.GetPixel(xDoel + x, yDoel + y), bewerking.Dekking);
                    for (int a = 0; a < 8; a++)
                        for (int b = 0; b < 8; b++)
                            bitmapVergroting.SetPixel(x * 8 + a, y * 8 + b, nieuweKleur);
                    pictureBox.Image = bitmapVergroting;
                    Huidige.Bitmap.SetPixel(xDoel + x, yDoel + y, nieuweKleur);
                    form1.ToonBitmap();

                    bewerking.BitmapIsGewijzigd();
                }
                else // kleur kiezen
                    bewerking.TekenKleur = bitmapVergroting.GetPixel(e.X, e.Y);
            }
        }

        void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && bewerking.Dekking == 1)
                pictureBox_MouseClick(sender, e);
        }

        protected override void FormVergroting_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.FormVergroting_FormClosed(sender, e);
            bewerking.HulpVensterGesloten();
        }

        Color geefMengKleur(Color kleur1, Color kleur2, decimal verhouding)
        {
            if (verhouding == 1)
                return kleur1;
            if (verhouding == 0)
                return kleur2;
            int r = (int)(verhouding * kleur1.R + (1 - verhouding) * kleur2.R);
            int g = (int)(verhouding * kleur1.G + (1 - verhouding) * kleur2.G);
            int b = (int)(verhouding * kleur1.B + (1 - verhouding) * kleur2.B);
            return Color.FromArgb(r, g, b);
        }
    }
}
