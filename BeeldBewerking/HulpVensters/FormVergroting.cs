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
    abstract partial class FormVergroting : Form
        // basisklasse van FormDetail en FormMasker
    {
        protected static FormVergroting formVergroting; // singleton

        protected Form1 form1;
        protected Bitmap bitmapVergroting;

        protected FormVergroting(Form1 form1, Bitmap bitmap)
        {
            InitializeComponent();
            this.form1 = form1;

            bitmapVergroting = new Bitmap(8 * bitmap.Width, 8 * bitmap.Height);
            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color kleur = bitmap.GetPixel(x, y);
                    for (int a = 0; a < 8; a++)
                        for (int b = 0; b < 8; b++)
                            bitmapVergroting.SetPixel(x * 8 + a, y * 8 + b, kleur);
                }

            this.ClientSize = bitmapVergroting.Size;
            this.pictureBox.Image = bitmapVergroting;
            this.DesktopLocation = new Point(Screen.PrimaryScreen.Bounds.Width - 200 - this.Width, 0);
        }

        protected virtual void FormVergroting_FormClosed(object sender, FormClosedEventArgs e)
        {
            formVergroting = null;
            this.Dispose();
        }
    }
}
