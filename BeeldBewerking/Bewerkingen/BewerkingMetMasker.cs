using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class BewerkingMetMasker : BewerkingMetHulpfiguren
    {
        public class Masker
        {
            public int XDoelgebied { get; private set; } // middelpunt van kader in bitmap
            public int YDoelgebied { get; private set; } // middelpunt van kader in bitmap
            public int Breedte { get; private set; }
            public int Hoogte { get; private set; }

            public bool[,] Gemaskeerd { get; private set; }

            public Masker(int x, int y, int breedte, int hoogte)
            {
                XDoelgebied = x;
                YDoelgebied = y;
                Breedte = breedte;
                Hoogte = hoogte;
                Gemaskeerd = new bool[breedte, hoogte];
            }

            public void ZetPixel(int xMasker, int yMasker, bool gemaskeerd)
            {
                if (xMasker >= 0 & xMasker < Breedte & yMasker >= 0 & yMasker < Hoogte)
                    Gemaskeerd[xMasker, yMasker] = gemaskeerd;
            }
        }

        public Masker HuidigMasker { get; protected set; }

        protected GroupBox groupBoxKader;
        protected CheckBox checkBoxGebruikKader, checkBoxRechthoekig;
        protected NumericUpDown[] numericGrootte = new NumericUpDown[2];
        protected Button buttonMasker;

        protected bool gebruikKader;
        protected bool rechthoekigKader;
        protected int kaderBreedte = 45; // afmeting binnenkant kader (werkzame gebied)
        protected int kaderHoogte = 45; // afmeting binnenkant kader (werkzame gebied)
        
        public BewerkingMetMasker(Form1 form1)
            : base(form1)
        {
            groupBoxKader = new GroupBox();
            groupBoxKader.Size = new Size(140, 193);
            groupBoxKader.Location = new Point(30, 120);
            groupBoxKader.Text = "Kader";
            lijstControls.Add(groupBoxKader);

            checkBoxGebruikKader = new CheckBox();
            checkBoxGebruikKader.Location = new Point(20, 30);
            checkBoxGebruikKader.Text = "Gebruik kader";
            checkBoxGebruikKader.CheckedChanged += new EventHandler(checkBoxKaderActief_CheckedChanged);
            checkBoxGebruikKader.Parent = groupBoxKader;
            lijstControls.Add(checkBoxGebruikKader);

            checkBoxRechthoekig = new CheckBox();
            checkBoxRechthoekig.Location = new Point(20, 60);
            checkBoxRechthoekig.Text = "Rechthoekig";
            checkBoxRechthoekig.CheckedChanged += new EventHandler(controlKader_ValueChanged);
            checkBoxRechthoekig.Parent = groupBoxKader;
            lijstControls.Add(checkBoxRechthoekig);

            for (int i = 0; i < 2; i++)
            {
                Label labelTekst = new Label();
                labelTekst.AutoSize = true;
                labelTekst.Location = new Point(17, 93 + 30 * i);
                labelTekst.Text = (i == 0) ? "Breedte" : "Hoogte";
                labelTekst.Parent = groupBoxKader;
                lijstControls.Add(labelTekst);

                numericGrootte[i] = new NumericUpDown();
                numericGrootte[i].Size = new Size(44, 20);
                numericGrootte[i].Location = new Point(76, 90 + 30 * i);
                numericGrootte[i].Increment = 2;
                numericGrootte[i].Minimum = 15;
                numericGrootte[i].Maximum = 125;
                numericGrootte[i].Value = kaderBreedte;
                numericGrootte[i].ValueChanged += new EventHandler(controlKader_ValueChanged);
                numericGrootte[i].MouseUp += (sender, e) => buttonFocus.Focus();
                numericGrootte[i].Parent = groupBoxKader;
                lijstControls.Add(numericGrootte[i]);
            }

            buttonMasker = new Button();
            buttonMasker.Size = new Size(100, 23);
            buttonMasker.Location = new Point(20, 150);
            buttonMasker.Text = "Masker";
            buttonMasker.Click += new EventHandler(buttonMasker_Click);
            buttonMasker.Parent = groupBoxKader;
            lijstControls.Add(buttonMasker);
        }

        public override void Reset()
        {
            sluitHulpVenster();
            kaderInBeeld = kaderVast = false;
            attributes = new ImageAttributes();
            form1.BitmapViewer.Refresh();
        }

        void checkBoxKaderActief_CheckedChanged(object sender, EventArgs e)
        {
            gebruikKader = checkBoxGebruikKader.Checked;
            form1.BitmapViewer.Cursor = gebruikKader ? Cursors.Cross : Cursors.Default;
            form1.InitialiseerBewerking(null, !gebruikKader);
            Reset();
        }

        protected void controlKader_ValueChanged(object sender, EventArgs e)
        {
            kaderBreedte = (int)numericGrootte[0].Value;
            kaderHoogte = (int)numericGrootte[1].Value;
            rechthoekigKader = checkBoxRechthoekig.Checked;
            sluitHulpVenster();
            form1.BitmapViewer.Refresh();
        }

        protected void buttonMasker_Click(object sender, EventArgs e)
        {
            if (hulpVenster != null && !hulpVenster.IsDisposed) // formMasker wordt al getoond
                return;

            if (kaderVast)
            {
                HuidigMasker = new Masker(startpunt.X, startpunt.Y, kaderBreedte, kaderHoogte);

                using (Bitmap bitmapKader = maakBitmapKader())
                    hulpVenster = FormMasker.GeefInstantie(form1, bitmapKader, this);
                hulpVenster.Show(form1);
            }
        }

        protected Bitmap maakBitmapKader()
        {
            bool metMasker = (hulpVenster != null && hulpVenster.IsDisposed == false);

            doelRechthoek =
                new Rectangle(startpunt.X - kaderBreedte / 2, startpunt.Y - kaderHoogte / 2,
                    kaderBreedte, kaderHoogte);

            GraphicsPath pad = new GraphicsPath();
            if (rechthoekigKader)
                pad.AddRectangle(new Rectangle(0, 0, kaderBreedte, kaderHoogte));
            else
                pad.AddEllipse(0, 0, kaderBreedte - 1, kaderHoogte - 1);
            Region regionKader = new Region(pad);

            Bitmap bitmapKader = new Bitmap(kaderBreedte, kaderHoogte);
            using (Graphics g = Graphics.FromImage(bitmapKader))
                g.DrawImage(Huidige.Bitmap, 0, 0, doelRechthoek, GraphicsUnit.Pixel);
            for (int x = 0; x < bitmapKader.Width; x++)
                for (int y = 0; y < bitmapKader.Height; y++)
                    if (regionKader.IsVisible(x, y) == false || (metMasker && HuidigMasker.Gemaskeerd[x, y]))
                        bitmapKader.SetPixel(x, y, Color.Black);

            bitmapKader.MakeTransparent(Color.Black);

            return bitmapKader;
        }
    }
}
