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
    class FormMasker : FormVergroting
    {
        public static FormVergroting GeefInstantie(Form1 form1, Bitmap bitmap, BewerkingMetMasker bewerking)
        {
            if (formVergroting == null)
                formVergroting = new FormMasker(form1, bitmap, bewerking);
            return formVergroting;
        }

        BewerkingMetMasker bewerking;
        Bitmap bitmapMasker;

        protected FormMasker(Form1 form1, Bitmap bitmap, BewerkingMetMasker bewerking)
            : base(form1, bitmap)
        {
            this.bewerking = bewerking;
            this.Text = "L=Masker    R=Herstellen";
            bitmapMasker = new Bitmap(bitmapVergroting);
            pictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseClick);
            pictureBox.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
        }

        void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > 0 && e.X < pictureBox.Width && e.Y > 0 && e.Y < pictureBox.Height
                && bitmapVergroting.GetPixel(e.X, e.Y).A > 0)
            {
                int x = e.X / 8, y = e.Y / 8;
                if (e.Button == MouseButtons.Left) // maskeren
                {
                    bewerking.HuidigMasker.ZetPixel(x, y, true);
                    for (int a = 0; a < 8; a++)
                        for (int b = 0; b < 8; b++)
                            bitmapMasker.SetPixel(x * 8 + a, y * 8 + b, Color.Yellow);
                }
                else // maskeren ongedaan maken
                {
                    bewerking.HuidigMasker.ZetPixel(x, y, false);
                    Color kleur = bitmapVergroting.GetPixel(e.X, e.Y);
                    for (int a = 0; a < 8; a++)
                        for (int b = 0; b < 8; b++)
                            bitmapMasker.SetPixel(x * 8 + a, y * 8 + b, kleur);
                }
                pictureBox.Image = bitmapMasker;
            }
        }

        void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                pictureBox_MouseClick(sender, e);
        }
    }
}
