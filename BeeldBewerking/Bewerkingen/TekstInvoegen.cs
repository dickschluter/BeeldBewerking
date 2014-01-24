using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class TekstInvoegen : BewerkingMetHulpfiguren
    {
        TextBox textBoxTekst, textBoxKleur;
        NumericUpDown numericGrootte, numericHoek;
        CheckBox checkBoxWitKader;

        decimal schaal;
        Font letterType;
        SolidBrush kleurBrush;

        bool muisBevatTekst; // true na MouseEnter, false na invoegen tekst

        public TekstInvoegen(Form1 form1)
            : base(form1)
        {
            Naam = "Tekst invoegen";
            labelBewerking.Text = Naam;

            textBoxTekst = new TextBox();
            textBoxTekst.Multiline = true;
            textBoxTekst.WordWrap = false;
            textBoxTekst.ScrollBars = ScrollBars.Both;
            textBoxTekst.Size = new Size(160, 130);
            textBoxTekst.Location = new Point(20, 120);
            lijstControls.Add(textBoxTekst);

            string[] labelTekst = { "Grootte", "Kleur", "Hoek" };
            for (int i = 0; i < 3; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Location = new Point(47, 303 + i * 30);
                label.Text = labelTekst[i];
                lijstControls.Add(label);
            }

            numericGrootte = new NumericUpDown();
            numericGrootte.Size = new Size(44, 21);
            numericGrootte.Location = new Point(106, 300);
            numericGrootte.Increment = 2;
            numericGrootte.Minimum = 8;
            numericGrootte.Maximum = 1000;
            numericGrootte.Value = 8;
            numericGrootte.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericGrootte);

            textBoxKleur = new TextBox();
            textBoxKleur.Size = new Size(44, 20);
            textBoxKleur.Location = new Point(106, 330);
            textBoxKleur.BackColor = Color.Black;
            textBoxKleur.ReadOnly = true;
            textBoxKleur.Click += new EventHandler(textBoxKleur_Click);
            lijstControls.Add(textBoxKleur);

            numericHoek = new NumericUpDown();
            numericHoek.Size = new Size(44, 20);
            numericHoek.Location = new Point(106, 360);
            numericHoek.Maximum = 180;
            numericHoek.Minimum = -180;
            numericHoek.MouseUp += (sender, e) => buttonFocus.Focus();
            lijstControls.Add(numericHoek);

            checkBoxWitKader = new CheckBox();
            checkBoxWitKader.AutoSize = true;
            checkBoxWitKader.Location = new Point(50, 390);
            checkBoxWitKader.Text = "Wit kader";
            lijstControls.Add(checkBoxWitKader);

            form1.InitialiseerBewerking(lijstControls, true);
            form1.BitmapViewer.Cursor = Cursors.Cross;
        }

        void textBoxKleur_Click(object sender, EventArgs e)
        {
            buttonFocus.Focus();
            if (DialoogVensters.KleurDialoog.ShowDialog() == DialogResult.OK)
                textBoxKleur.BackColor = DialoogVensters.KleurDialoog.Color;
            else
                buttonFocus.Focus();
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e) // tijdelijk tekenen
        {
            e.Graphics.TranslateTransform(startpunt.X, startpunt.Y);
            e.Graphics.ScaleTransform((float)schaal, (float)schaal);
            e.Graphics.RotateTransform((float)numericHoek.Value);
            e.Graphics.DrawString(textBoxTekst.Text, letterType, kleurBrush, Point.Empty);
        }

        protected override void viewer_MouseEnter(object sender, EventArgs e)
        {
            if (textBoxTekst.Text != "")
            {
                schaal = form1.BitmapViewer.Schaal;
                letterType = new Font("Microsoft Sans Serif", (float)numericGrootte.Value);
                kleurBrush = new SolidBrush(textBoxKleur.BackColor);

                muisBevatTekst = true;
                form1.BitmapViewer.Paint += viewer_Paint;
            }
        }

        protected override void viewer_MouseLeave(object sender, EventArgs e)
        {
            muisBevatTekst = false;
            form1.BitmapViewer.Paint -= viewer_Paint;
            form1.BitmapViewer.Refresh();
        }

        protected override void viewer_MouseMove(object sender, MouseEventArgs e) // tijdelijk tekenen
        {
            if (muisBevatTekst)
            {
                startpunt = e.Location;
                form1.BitmapViewer.Refresh();
            }
        }

        protected override void viewer_MouseDown(object sender, MouseEventArgs e) // definitief invoegen
        {
            if (muisBevatTekst)
            {
                using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                {
                    g.TranslateTransform(
                        (int)(e.X / schaal), (int)(e.Y / schaal));
                    g.RotateTransform((float)numericHoek.Value);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    if (checkBoxWitKader.Checked)
                    {
                        SizeF sizeF = g.MeasureString(textBoxTekst.Text, letterType);
                        g.FillRectangle(new SolidBrush(Color.White), 0, 0, sizeF.Width, sizeF.Height);
                    }
                    g.DrawString(textBoxTekst.Text, letterType, kleurBrush, Point.Empty);
                }

                Geschiedenis.HuidigeBitmapToevoegen();
                form1.ToonBitmap();
            }

            muisBevatTekst = false;
            form1.BitmapViewer.Paint -= viewer_Paint;
            form1.BitmapViewer.Refresh();
        }
    }
}
