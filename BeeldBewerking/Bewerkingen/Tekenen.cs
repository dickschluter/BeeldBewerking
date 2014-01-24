using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BeeldBewerking.Bewerkingen.TekenGereedschap;

namespace BeeldBewerking
{
    class Tekenen : BewerkingMetHulpfiguren
    {
        RadioButton[] radioGereedschap= new RadioButton[6];
        TextBox textBoxKleur;
        NumericUpDown numericLijnDikte;
        CheckBox checkBoxAntiAlias;
        Button buttonFixeren;

        Pen pen;
        IGereedschap gereedschap;
        bool huidigeBitmapGewijzigd;

        public Tekenen(Form1 form1)
            : base(form1)
        {
            Naam = "Tekenen";
            labelBewerking.Text = Naam;

            string[] gereedschappen =
                { "Pen", "Lijn", "Rechthoek", "Rechthoek vullen", "Ellips", "Ellips vullen" };
            for (int i = 0; i < 6; i++)
            {
                radioGereedschap[i] = new RadioButton();
                radioGereedschap[i].AutoSize = true;
                radioGereedschap[i].Location = new Point(50, 120 + 30 * i);
                radioGereedschap[i].Text = gereedschappen[i];
                radioGereedschap[i].CheckedChanged += new EventHandler(control_CheckedChanged);
                lijstControls.Add(radioGereedschap[i]);
            }

            for (int i = 0; i < 2; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Location = new Point(47, 333 + 30 * i);
                label.Text = (i == 0) ? "Kleur" : "Dikte";
                lijstControls.Add(label);
            }

            textBoxKleur = new TextBox();
            textBoxKleur.Size = new Size(44, 20);
            textBoxKleur.Location = new Point(106, 330);
            textBoxKleur.BackColor = Color.Black;
            textBoxKleur.ReadOnly = true;
            textBoxKleur.Click += new EventHandler(textBoxKleur_Click);
            lijstControls.Add(textBoxKleur);

            numericLijnDikte = new NumericUpDown();
            numericLijnDikte.Size = new Size(44, 20);
            numericLijnDikte.Location = new Point(106, 360);
            numericLijnDikte.Minimum = 1;
            numericLijnDikte.MouseUp += new MouseEventHandler(numericLijnDikte_MouseUp);
            lijstControls.Add(numericLijnDikte);

            checkBoxAntiAlias = new CheckBox();
            checkBoxAntiAlias.Location = new Point(50, 390);
            checkBoxAntiAlias.Checked = true;
            checkBoxAntiAlias.Text = "Anti-alias";
            lijstControls.Add(checkBoxAntiAlias);

            buttonFixeren = new Button();
            buttonFixeren.Enabled = false;
            buttonFixeren.Size = new Size(100, 23);
            buttonFixeren.Location = new Point(50, 450);
            buttonFixeren.Text = "Fixeren";
            buttonFixeren.Click += new EventHandler(buttonFixeren_Click);
            lijstControls.Add(buttonFixeren);

            radioGereedschap[0].Checked = true;
            maakPen();

            form1.InitialiseerBewerking(lijstControls, false);
        }

        protected override void pictureBoxVorige_Click(object sender, EventArgs e)
        {
            BitmapMetNaam vorigeBitmapMetNaam;
            if (huidigeBitmapGewijzigd)
            {
                if (Geschiedenis.LaatstToegevoegd(out vorigeBitmapMetNaam))
                    Huidige.Bitmap = vorigeBitmapMetNaam.Bitmap;
            }
            else if (Geschiedenis.Vorige(out vorigeBitmapMetNaam))
            {
                Huidige.Naam = vorigeBitmapMetNaam.Naam;
                Huidige.Bitmap = vorigeBitmapMetNaam.Bitmap;
            }
            form1.ToonBitmap();

            Reset();
        }

        public override void Reset()
        {
            huidigeBitmapGewijzigd = false;
            buttonFixeren.Enabled = false;
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e)
        {
            gereedschap.HulpFiguur(e.Graphics, Pens.Yellow, startpunt, eindpunt);
        }

        protected override void viewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (form1.BitmapViewer.Schaal == 1)
            {
                muisIngedrukt = true;
                startpunt = e.Location;
                form1.BitmapViewer.Paint += viewer_Paint;
            }
        }

        protected override void viewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (muisIngedrukt)
            {
                eindpunt = e.Location;
                form1.BitmapViewer.Refresh();

                if (gereedschap is PenTekenen)
                {
                    viewer_MouseUp(sender, e);
                    viewer_MouseDown(sender, e);
                }
            }
        }

        protected override void viewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (muisIngedrukt)
            {
                muisIngedrukt = false;
                eindpunt = e.Location;
                form1.BitmapViewer.Paint -= viewer_Paint;
                using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                {
                    if (checkBoxAntiAlias.Checked)
                        g.SmoothingMode = SmoothingMode.HighQuality;
                    gereedschap.Tekenen(g, pen, startpunt, eindpunt);
                }
                form1.ToonBitmap();

                huidigeBitmapGewijzigd = true;
                buttonFixeren.Enabled = true;
                pictureBoxVorige.Visible = true;
            }
        }

        void control_CheckedChanged(object sender, EventArgs e)
        {
            switch ((sender as Control).Text)
            {
                case "Lijn":
                    gereedschap = new LijnTekenen();
                    break;
                case "Rechthoek":
                    gereedschap = new RechthoekTekenen();
                    break;
                case "Rechthoek vullen":
                    gereedschap = new RechthoekVullen();
                    break;
                case "Ellips":
                    gereedschap = new EllipsTekenen();
                    break;
                case "Ellips vullen":
                    gereedschap = new EllipsVullen();
                    break;
                default:
                    gereedschap = new PenTekenen();
                    break;
            }
        }

        void textBoxKleur_Click(object sender, EventArgs e)
        {
            if (DialoogVensters.KleurDialoog.ShowDialog() == DialogResult.OK)
            {
                textBoxKleur.BackColor = DialoogVensters.KleurDialoog.Color;
                maakPen();
            }
            else
                buttonFocus.Focus();
        }

        void numericLijnDikte_MouseUp(object sender, MouseEventArgs e)
        {
            maakPen();
            buttonFocus.Focus();
        }

        void maakPen()
        {
            pen = new Pen(textBoxKleur.BackColor, (float)numericLijnDikte.Value);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
        }

        void buttonFixeren_Click(object sender, EventArgs e)
        {
            Geschiedenis.HuidigeBitmapToevoegen();
            Reset();
        }
    }
}
