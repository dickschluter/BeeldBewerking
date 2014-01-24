using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    partial class FormColorMatrix : Form
    {
        public static FormColorMatrix GeefInstantie(KleurenVeranderen bewerking)
        {
            if (formColorMatrix == null)
                formColorMatrix = new FormColorMatrix(bewerking);
            return formColorMatrix;
        }

        static FormColorMatrix formColorMatrix; // singleton

        KleurenVeranderen bewerking;
        TextBox[,] textBoxCoefficient = new TextBox[5, 3];

        private FormColorMatrix(KleurenVeranderen bewerking)
        {
            InitializeComponent();

            this.bewerking = bewerking;
            for (int rij = 0; rij < 5; rij++)
            {
                if (rij == 3) // coefficienten voor alfa worden niet gebruikt
                    continue;
                for (int kolom = 0; kolom < 3; kolom++)
                {
                    textBoxCoefficient[rij, kolom] = new TextBox();
                    textBoxCoefficient[rij, kolom].Size = new Size(40, 20);
                    textBoxCoefficient[rij, kolom].Location = new Point(80 + 60 * kolom, 50 + 30 * rij);
                    textBoxCoefficient[rij, kolom].Text = rij == kolom ? "1" : "0";
                    this.Controls.Add(textBoxCoefficient[rij, kolom]);
                }
            }

            this.DesktopLocation = new Point(Screen.PrimaryScreen.Bounds.Width - 200 - this.Width, 0);
        }

        private void buttonToepassen_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormColorMatrix_FormClosing(object sender, FormClosingEventArgs e)
        {
            ColorMatrix colorMatrix = new ColorMatrix();
            for (int rij = 0; rij < 5; rij++)
            {
                if (rij == 3)
                    continue;
                for (int kolom = 0; kolom < 3; kolom++)
                {
                    float coefficient;
                    if (float.TryParse(textBoxCoefficient[rij, kolom].Text, out coefficient))
                        colorMatrix[rij, kolom] = coefficient;
                    textBoxCoefficient[rij, kolom].Text = rij == kolom ? "1" : "0"; // resetten voor volgend gebruik
                }
            }
            bewerking.KleurenMatrix = colorMatrix;
        }
    }
}
