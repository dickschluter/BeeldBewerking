using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Bijsnijden : BewerkingMetHulpfiguren
    {
        NumericUpDown[] numericHor = new NumericUpDown[2];
        NumericUpDown[] numericVert = new NumericUpDown[2];
        Button buttonToepassen;

        bool nuttigeParameters; // true als een van de parameters groter dan nul is

        public Bijsnijden(Form1 form1)
            : base(form1)
        {
            Naam = "Bijsnijden";
            labelBewerking.Text = Naam;

            for (int i = 0; i < 2; i++)
            {
                Label label = new Label();
                label.Size = new Size(14, 13);
                label.Location = new Point(36 + i * 114, 153);
                label.Text = (i == 0 ? "L" : "R");
                lijstControls.Add(label);

                numericHor[i] = new NumericUpDown();
                numericHor[i].Size = new Size(44, 20);
                numericHor[i].Location = new Point(50 + i * 56, 150);
                numericHor[i].MouseUp += numericUpDown_MouseUp;
                lijstControls.Add(numericHor[i]);
            }
            for (int i = 0; i < 2; i++)
            {
                Label label = new Label();
                label.Size = new Size(14, 13);
                label.Location = new Point(64, 123 + i * 60);
                label.Text = (i == 0 ? "B" : "O");
                lijstControls.Add(label);

                numericVert[i] = new NumericUpDown();
                numericVert[i].Size = new Size(46, 20);
                numericVert[i].Location = new Point(78, 120 + i * 60);
                numericVert[i].MouseUp += numericUpDown_MouseUp;
                lijstControls.Add(numericVert[i]);
            }

            buttonToepassen = new Button();
            buttonToepassen.Size = new Size(100, 23);
            buttonToepassen.Location = new Point(50, 240);
            buttonToepassen.Text = "Toepassen";
            buttonToepassen.Click += new EventHandler(buttonToepassen_Click);
            lijstControls.Add(buttonToepassen);

            form1.InitialiseerBewerking(lijstControls, true);
        }

        public override void Reset()
        {
            nuttigeParameters = false;
            for (int i = 0; i < 2; i++)
            {
                numericHor[i].Value = 0;
                numericVert[i].Value = 0;
            }
            bepaalMaximumWaarden();
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(Math.Min(startpunt.X, eindpunt.X), Math.Min(startpunt.Y, eindpunt.Y),
                Math.Abs(startpunt.X - eindpunt.X), Math.Abs(startpunt.Y - eindpunt.Y));
            e.Graphics.DrawRectangle(Pens.Yellow, rect);
        }

        protected override void viewer_MouseDown(object sender, MouseEventArgs e)
        {
            muisIngedrukt = true;
            startpunt = e.Location;
            form1.BitmapViewer.Paint += viewer_Paint;
        }

        protected override void viewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (muisIngedrukt)
            {
                eindpunt = e.Location;
                form1.BitmapViewer.Refresh();
            }
        }

        protected override void viewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (muisIngedrukt)
            {
                muisIngedrukt = false;
                eindpunt = e.Location;
                form1.BitmapViewer.Paint -= viewer_Paint;
                nuttigeParameters = true;

                int x0 = Math.Min(startpunt.X, eindpunt.X);
                int x1 = Math.Max(startpunt.X, eindpunt.X);
                int y0 = Math.Min(startpunt.Y, eindpunt.Y);
                int y1 = Math.Max(startpunt.Y, eindpunt.Y);

                resetMaximumWaarden();
                numericHor[0].Value = Math.Max((int)((decimal)(x0 + 1) / form1.BitmapViewer.Schaal), 0);
                numericVert[0].Value = Math.Max((int)((decimal)(y0 + 1) / form1.BitmapViewer.Schaal), 0);
                numericHor[1].Value =
                    Math.Max(Huidige.Bitmap.Width - (int)((decimal)x1 / form1.BitmapViewer.Schaal), 0);
                numericVert[1].Value =
                    Math.Max(Huidige.Bitmap.Height - (int)((decimal)y1 / form1.BitmapViewer.Schaal), 0);
                bepaalMaximumWaarden();
            }
        }

        private void numericUpDown_MouseUp(object sender, MouseEventArgs e)
        {
            nuttigeParameters = true;
            bepaalMaximumWaarden();

            startpunt = new Point((int)(numericHor[0].Value * form1.BitmapViewer.Schaal) - 1,
                (int)(numericVert[0].Value * form1.BitmapViewer.Schaal) - 1);
            eindpunt = new Point((int)((Huidige.Bitmap.Width - numericHor[1].Value) * form1.BitmapViewer.Schaal),
                (int)((Huidige.Bitmap.Height - numericVert[1].Value) * form1.BitmapViewer.Schaal));
            form1.BitmapViewer.Paint += viewer_Paint;
            form1.BitmapViewer.Refresh();
            form1.BitmapViewer.Paint -= viewer_Paint;
        }

        private void buttonToepassen_Click(object sender, EventArgs e)
        {
            if (nuttigeParameters)
            {
                Rectangle rect = new Rectangle(
                    (int)numericHor[0].Value, (int)numericVert[0].Value,
                    (int)(Huidige.Bitmap.Width - numericHor[0].Value - numericHor[1].Value),
                    (int)(Huidige.Bitmap.Height - numericVert[0].Value - numericVert[1].Value));
                Huidige.Bitmap = Huidige.Bitmap.Clone(rect, PixelFormat.DontCare);

                Geschiedenis.HuidigeBitmapToevoegen();
                form1.ToonBitmap();

                Reset();
            }
        }

        private void bepaalMaximumWaarden()
        {
            numericHor[0].Maximum = Huidige.Bitmap.Width - numericHor[1].Value - minimumAfmeting;
            numericHor[1].Maximum = Huidige.Bitmap.Width - numericHor[0].Value - minimumAfmeting;
            numericVert[0].Maximum = Huidige.Bitmap.Height - numericVert[1].Value - minimumAfmeting;
            numericVert[1].Maximum = Huidige.Bitmap.Height - numericVert[0].Value - minimumAfmeting;
        }

        private void resetMaximumWaarden()
        {
            numericHor[0].Maximum = Huidige.Bitmap.Width;
            numericHor[1].Maximum = Huidige.Bitmap.Width;
            numericVert[0].Maximum = Huidige.Bitmap.Height;
            numericVert[1].Maximum = Huidige.Bitmap.Height;
        }
    }
}
