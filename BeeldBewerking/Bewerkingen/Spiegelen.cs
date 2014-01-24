using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Spiegelen : BewerkingBasis
    {
        Button buttonHorizontaal, buttonVerticaal;

        public Spiegelen(Form1 form1)
            : base(form1)
        {
            Naam = "Spiegelen";
            labelBewerking.Text = Naam;

            buttonHorizontaal = new Button();
            buttonHorizontaal.Size = new Size(100, 23);
            buttonHorizontaal.Location = new Point(50, 120);
            buttonHorizontaal.Text = "Horizontaal";
            buttonHorizontaal.Click += new EventHandler(buttonHorizontaal_Click);
            lijstControls.Add(buttonHorizontaal);

            buttonVerticaal = new Button();
            buttonVerticaal.Size = new Size(100, 23);
            buttonVerticaal.Location = new Point(50, 150);
            buttonVerticaal.Text = "Verticaal";
            buttonVerticaal.Click += new EventHandler(buttonVerticaal_Click);
            lijstControls.Add(buttonVerticaal);

            form1.InitialiseerBewerking(lijstControls, true);
        }

        void buttonHorizontaal_Click(object sender, EventArgs e)
        {
            Huidige.Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();
        }

        void buttonVerticaal_Click(object sender, EventArgs e)
        {
            Huidige.Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();
        }
    }
}
