using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Schalen : BewerkingBasis
    {
        NumericUpDown numericBreedte, numericHoogte;
        CheckBox checkBoxGelijk;
        Button buttonToepassen;

        public Schalen(Form1 form1)
            : base(form1)
        {
            Naam = "Schalen";
            labelBewerking.Text = Naam;

            for (int i = 0; i < 2; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Location = new Point(47, 123 + i * 30);
                label.Text = (i == 0 ? "Breedte" : "Hoogte");
                lijstControls.Add(label);
            }

            numericBreedte = new NumericUpDown();
            numericBreedte.Size = new Size(44, 20);
            numericBreedte.Location = new Point(106, 120);
            numericBreedte.Minimum = 1;
            numericBreedte.Maximum = 200;
            numericBreedte.Value = 100;
            numericBreedte.MouseUp += new MouseEventHandler(numericBreedte_MouseUp);
            lijstControls.Add(numericBreedte);

            numericHoogte = new NumericUpDown();
            numericHoogte.Size = new Size(44, 20);
            numericHoogte.Location = new Point(106, 150);
            numericHoogte.Minimum = 1;
            numericHoogte.Maximum = 200;
            numericHoogte.Value = 100;
            numericHoogte.MouseUp += new MouseEventHandler(numericHoogte_MouseUp);
            lijstControls.Add(numericHoogte);

            checkBoxGelijk = new CheckBox();
            checkBoxGelijk.AutoSize = true;
            checkBoxGelijk.Location = new Point(50, 180);
            checkBoxGelijk.Text = "Gelijke waarden";
            checkBoxGelijk.Checked = true;
            checkBoxGelijk.CheckedChanged += new EventHandler(checkBoxGelijk_CheckedChanged);
            lijstControls.Add(checkBoxGelijk);

            buttonToepassen = new Button();
            buttonToepassen.Size = new Size(100, 23);
            buttonToepassen.Location = new Point(50, 240);
            buttonToepassen.Text = "Toepassen";
            buttonToepassen.Click += new EventHandler(buttonToepassen_Click);
            lijstControls.Add(buttonToepassen);

            form1.InitialiseerBewerking(lijstControls, true);
        }

        void numericBreedte_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkBoxGelijk.Checked)
                numericHoogte.Value = numericBreedte.Value;
            buttonFocus.Focus();
        }

        void numericHoogte_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkBoxGelijk.Checked)
                numericBreedte.Value = numericHoogte.Value;
            buttonFocus.Focus();
        }

        void checkBoxGelijk_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxGelijk.Checked)
                numericHoogte.Value = numericBreedte.Value;
        }

        void buttonToepassen_Click(object sender, EventArgs e)
        {
            int nieuweBreedte = Huidige.Bitmap.Width * (int)numericBreedte.Value / 100;
            int nieuweHoogte = Huidige.Bitmap.Height * (int)numericHoogte.Value / 100;

            if (nieuweBreedte >= minimumAfmeting && nieuweHoogte >= minimumAfmeting)
            {
                Bitmap vorigeBitmap = Huidige.Bitmap;
                Huidige.Bitmap = new Bitmap(nieuweBreedte, nieuweHoogte);
                using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(vorigeBitmap, 0, 0, Huidige.Bitmap.Width, Huidige.Bitmap.Height);
                }

                Geschiedenis.HuidigeBitmapToevoegen();
                form1.ToonBitmap();

                numericBreedte.Value = numericHoogte.Value = 100;
            }
            else
                MessageBox.Show("Bitmap wordt te klein");
        }
    }
}
