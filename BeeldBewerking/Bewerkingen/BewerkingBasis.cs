using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    abstract class BewerkingBasis
    {
        public string Naam { get; protected set; }

        protected Form1 form1;
        protected FormVergroting hulpVenster;

        protected PictureBox pictureBoxVorige;
        protected PictureBox pictureBoxVolgende;
        protected Label labelBewerking;
        protected Button buttonFocus;
        
        protected List<Control> lijstControls = new List<Control>();
        protected readonly int minimumAfmeting = 10; // minimale afmeting van bitmap

        public BewerkingBasis(Form1 form1)
        {
            this.form1 = form1;

            Bitmap bitmapVorige = Properties.Resources.VorigeBitmap;
            bitmapVorige.MakeTransparent(Color.White);
            Bitmap bitmapVolgende = Properties.Resources.VolgendeBitmap;
            bitmapVolgende.MakeTransparent(Color.White);

            pictureBoxVorige = new PictureBox();
            pictureBoxVorige.Image = bitmapVorige;
            pictureBoxVorige.Size = new Size(20, 20);
            pictureBoxVorige.Location = new Point(10, 30);
            pictureBoxVorige.Visible = Geschiedenis.BevatVorige;
            pictureBoxVorige.Click += new EventHandler(pictureBoxVorige_Click);
            pictureBoxVorige.DoubleClick += new EventHandler(pictureBoxVorige_DoubleClick);
            lijstControls.Add(pictureBoxVorige);

            pictureBoxVolgende = new PictureBox();
            pictureBoxVolgende.Image = bitmapVolgende;
            pictureBoxVolgende.Size = new Size(20, 20);
            pictureBoxVolgende.Location = new Point(170, 30);
            pictureBoxVolgende.Visible = Geschiedenis.BevatVolgende;
            pictureBoxVolgende.Click += new EventHandler(pictureBoxVolgende_Click);
            pictureBoxVolgende.DoubleClick += new EventHandler(pictureBoxVolgende_DoubleClick);
            lijstControls.Add(pictureBoxVolgende);

            labelBewerking = new Label();
            labelBewerking.Font = new Font("Microsoft Sans Serif", 10F);
            labelBewerking.ForeColor = SystemColors.Desktop;
            labelBewerking.Location = new Point(20, 60);
            labelBewerking.Size = new Size(160, 17);
            labelBewerking.TextAlign = ContentAlignment.MiddleCenter;
            lijstControls.Add(labelBewerking);

            buttonFocus = new Button();
            buttonFocus.Location = new Point(199, 0);
            buttonFocus.Size = new Size(1, 1);
            lijstControls.Add(buttonFocus);

            Geschiedenis.StatusGewijzigd += new EventHandler(updateVorigeVolgende);
        }

        void updateVorigeVolgende(object sender, EventArgs e)
        {
            pictureBoxVorige.Visible = Geschiedenis.BevatVorige;
            pictureBoxVolgende.Visible = Geschiedenis.BevatVolgende;
        }

        protected virtual void pictureBoxVorige_Click(object sender, EventArgs e)
        {
            BitmapMetNaam vorigeBitmapMetNaam;
            if (Geschiedenis.Vorige(out vorigeBitmapMetNaam))
            {
                Huidige.Naam = vorigeBitmapMetNaam.Naam;
                Huidige.Bitmap = vorigeBitmapMetNaam.Bitmap;
                form1.ToonBitmap();

                Reset();
            }
        }

        protected virtual void pictureBoxVorige_DoubleClick(object sender, EventArgs e)
        {
            pictureBoxVorige_Click(sender, e);
        }

        protected virtual void pictureBoxVolgende_Click(object sender, EventArgs e)
        {
            BitmapMetNaam volgendeBitmapMetNaam;
            if (Geschiedenis.Volgende(out volgendeBitmapMetNaam))
            {
                Huidige.Naam = volgendeBitmapMetNaam.Naam;
                Huidige.Bitmap = volgendeBitmapMetNaam.Bitmap;
                form1.ToonBitmap();

                Reset();
            }
        }

        protected virtual void pictureBoxVolgende_DoubleClick(object sender, EventArgs e)
        {
            pictureBoxVolgende_Click(sender, e);
        }

        public virtual void Reset()
        {
            sluitHulpVenster();
        }

        protected void sluitHulpVenster()
        {
            if (hulpVenster != null)
                hulpVenster.Close();
        }

        public virtual void Dispose()
        {
            sluitHulpVenster();
            foreach (Control c in lijstControls)
                c.Dispose();
        }
    }
}
