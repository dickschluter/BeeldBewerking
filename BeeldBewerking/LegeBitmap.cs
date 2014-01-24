using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class LegeBitmap : BewerkingBasis
    {
        NumericUpDown numericBreedte, numericHoogte;
        TextBox textBoxKleur;
        Button buttonMaken;

        public LegeBitmap(Form1 form1)
            : base(form1)
        {
            Naam = "Lege bitmap";
            labelBewerking.Text = Naam;

            string[] tekst = { "Breedte", "Hoogte", "Kleur" };
            for (int i = 0; i < 3; i++)
            {
                Label label = new Label();
                label.Size = new Size(44, 13);
                label.Location = new Point(47, 123 + i * 30);
                label.Text = tekst[i];
                lijstControls.Add(label);
            }

            numericBreedte = new NumericUpDown();
            numericBreedte.Size = new Size(44, 20);
            numericBreedte.Location = new Point(106, 120);
            numericBreedte.Minimum = minimumAfmeting;
            numericBreedte.Maximum = 1920;
            numericBreedte.Value = 500;
            lijstControls.Add(numericBreedte);

            numericHoogte = new NumericUpDown();
            numericHoogte.Size = new Size(44, 20);
            numericHoogte.Location = new Point(106, 150);
            numericHoogte.Minimum = minimumAfmeting;
            numericHoogte.Maximum = 1080;
            numericHoogte.Value = 500;
            lijstControls.Add(numericHoogte);

            textBoxKleur = new TextBox();
            textBoxKleur.Size = new Size(44, 20);
            textBoxKleur.Location = new Point(106, 180);
            textBoxKleur.BackColor = Color.White;
            textBoxKleur.ReadOnly = true;
            textBoxKleur.Click += new EventHandler(textBoxKleur_Click);
            lijstControls.Add(textBoxKleur);

            buttonMaken = new Button();
            buttonMaken.Size = new Size(100, 23);
            buttonMaken.Location = new Point(50, 240);
            buttonMaken.Text = "Maken";
            buttonMaken.Click += new EventHandler(buttonMaken_Click);
            lijstControls.Add(buttonMaken);

            form1.InitialiseerBewerking(lijstControls, true);
        }

        void textBoxKleur_Click(object sender, EventArgs e)
        {
            buttonMaken.Focus();
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.FullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                textBoxKleur.BackColor = colorDialog.Color;
        }

        void buttonMaken_Click(object sender, EventArgs e)
        {
            Huidige.Naam = "Naamloos";
            Huidige.Bitmap = new Bitmap((int)numericBreedte.Value, (int)numericHoogte.Value);
            using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                g.Clear(textBoxKleur.BackColor);

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();
        }
    }
}
