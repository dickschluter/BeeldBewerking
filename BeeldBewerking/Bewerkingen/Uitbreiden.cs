using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Uitbreiden : BewerkingBasis
    {
        NumericUpDown[] numericHor = new NumericUpDown[2];
        NumericUpDown[] numericVert = new NumericUpDown[2];
        Button buttonToepassen;

        public Uitbreiden(Form1 form1)
            : base(form1)
        {
            Naam = "Uitbreiden";
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
                numericHor[i].Maximum = 1000; 
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
                numericVert[i].Maximum = 1000;
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

        private void buttonToepassen_Click(object sender, EventArgs e)
        {
            if (numericHor[0].Value == 0 && numericHor[1].Value == 0 && numericVert[0].Value == 0 && numericVert[1].Value == 0)
                return;

            Bitmap vorigeBitmap = Huidige.Bitmap;
            Huidige.Bitmap = new Bitmap(
                    vorigeBitmap.Width + (int)numericHor[0].Value + (int)numericHor[1].Value,
                    vorigeBitmap.Height + (int)numericVert[0].Value + (int)numericVert[1].Value);
            using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
            {
                g.Clear(Color.White);
                g.DrawImage(vorigeBitmap, (int)numericHor[0].Value, (int)numericVert[0].Value);
            }

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();
        }
    }
}
