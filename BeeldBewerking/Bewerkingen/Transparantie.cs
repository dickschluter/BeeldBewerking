using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Transparantie : BewerkingMetHulpfiguren
    {
        NumericUpDown numericDekking;
        Button buttonToepassen;

        public Transparantie(Form1 form1)
            : base(form1)
        {
            Naam = "Transparantie";
            labelBewerking.Text = Naam;

            buttonToepassen = new Button();
            buttonToepassen.Size = new Size(100, 23);
            buttonToepassen.Location = new Point(50, 240);
            buttonToepassen.Text = "Toepassen";
            buttonToepassen.Click += new EventHandler(buttonToepassen_Click);
            lijstControls.Add(buttonToepassen);

            numericDekking = new NumericUpDown();
            numericDekking.Size = new Size(44, 20);
            numericDekking.Location = new Point(106, 210);
            numericDekking.Increment = 10;
            numericDekking.Minimum = 10;
            numericDekking.Value = 100;
            numericDekking.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericDekking);

            form1.InitialiseerBewerking(lijstControls, true);
        }

        private void buttonToepassen_Click(object sender, EventArgs e)
        {
            attributes = new ImageAttributes();
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Matrix33 = 0;
            colorMatrix.Matrix43 = (float)numericDekking.Value / 100;
            attributes.SetColorMatrix(colorMatrix);

            Bitmap vorigeBitmap = Huidige.Bitmap;
            Huidige.Bitmap = new Bitmap(vorigeBitmap.Width, vorigeBitmap.Height);
            using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                g.DrawImage(vorigeBitmap, new Rectangle(Point.Empty, vorigeBitmap.Size),
                    0, 0, vorigeBitmap.Width, vorigeBitmap.Height, GraphicsUnit.Pixel, attributes);

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();
        }
    }
}
