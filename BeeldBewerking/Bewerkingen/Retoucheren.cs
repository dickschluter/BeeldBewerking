using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class Retoucheren : BewerkingMetHulpfiguren
    {
        // kaderInBeeld: false als geen kader zichtbaar is
        // kaderVast: true als een doelgebied geselecteerd is
        // startpunt: middelpunt van doelgebied
        // eindpunt: middelpunt van brongebied

        CheckBox checkBoxRechthoekig;
        NumericUpDown[] numericGrootte = new NumericUpDown[2];
        NumericUpDown numericHoek, numericSchaal, numericGradient, numericDekking;
        
        readonly int maxAfmeting = 499;
        readonly int minAfmeting = 5;

        bool rechthoekigKader;
        int kaderBreedte = 45; // afmeting binnenkant kader (werkzame gebied)
        int kaderHoogte = 45; // afmeting binnenkant kader (werkzame gebied)
        float kaderHoek = 0;
        float schaal = 100;
        int gradient = 0;
        
        public Retoucheren(Form1 form1)
            : base(form1)
        {
            Naam = "Retoucheren";
            labelBewerking.Text = Naam;

            checkBoxRechthoekig = new CheckBox();
            checkBoxRechthoekig.Location = new Point(50, 120);
            checkBoxRechthoekig.Text = "Rechthoekig";
            checkBoxRechthoekig.CheckedChanged += new EventHandler(control_ValueChanged);
            lijstControls.Add(checkBoxRechthoekig);
            
            string[] labelTekst = { "Breedte", "Hoogte", "Hoek", "Schaal", "Gradient", "Dekking" };
            for (int i = 0; i < 6; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Location = new Point(47, 183 + 30 * i);
                label.Text = labelTekst[i];
                lijstControls.Add(label);
            }

            for (int i = 0; i < 2; i++)
            {
                numericGrootte[i] = new NumericUpDown();
                numericGrootte[i].Size = new Size(44, 20);
                numericGrootte[i].Location = new Point(106, 180 + 30 * i);
                numericGrootte[i].Increment = 2;
                numericGrootte[i].Minimum = minAfmeting;
                numericGrootte[i].Maximum = maxAfmeting;
                numericGrootte[i].Value = kaderBreedte;
                numericGrootte[i].MouseUp += new MouseEventHandler(control_ValueChanged);
                lijstControls.Add(numericGrootte[i]);
            }

            numericHoek = new NumericUpDown();
            numericHoek.Size = new Size(44, 20);
            numericHoek.Location = new Point(106, 240);
            numericHoek.Minimum = -45;
            numericHoek.Maximum = 45;
            numericHoek.MouseUp += new MouseEventHandler(control_ValueChanged);
            lijstControls.Add(numericHoek);

            numericSchaal = new NumericUpDown();
            numericSchaal.Size = new Size(44, 20);
            numericSchaal.Location = new Point(106, 270);
            numericSchaal.Minimum = 50;
            numericSchaal.Maximum = 200;
            numericSchaal.Value = 100;
            numericSchaal.MouseUp += new MouseEventHandler(control_ValueChanged);
            lijstControls.Add(numericSchaal);

            numericGradient = new NumericUpDown();
            numericGradient.Size = new Size(44, 20);
            numericGradient.Location = new Point(106, 300);
            numericGradient.Maximum = 20;
            numericGradient.MouseUp += new MouseEventHandler(control_ValueChanged);
            lijstControls.Add(numericGradient);

            numericDekking = new NumericUpDown();
            numericDekking.Size = new Size(44, 20);
            numericDekking.Location = new Point(106, 330);
            numericDekking.Increment = 10;
            numericDekking.Minimum = 10;
            numericDekking.Value = 100;
            numericDekking.MouseUp += new MouseEventHandler(control_ValueChanged);
            lijstControls.Add(numericDekking);

            form1.BitmapViewer.Cursor = Cursors.Cross;
            form1.BitmapViewer.Paint += viewer_Paint;
            form1.InitialiseerBewerking(lijstControls, false);
        }

        protected override void viewer_Paint(object sender, PaintEventArgs e) // tijdelijk tekenen
        {
            if (kaderInBeeld)
            {
                e.Graphics.TranslateTransform((float)startpunt.X, (float)startpunt.Y);
                e.Graphics.RotateTransform(kaderHoek);

                if (kaderVast)
                    using (Bitmap bitmapBron = maakBitmapBron())
                        e.Graphics.DrawImage(bitmapBron, doelRechthoek,
                            0, 0, kaderBreedte, kaderHoogte, GraphicsUnit.Pixel, attributes);

                if (rechthoekigKader)
                    e.Graphics.DrawRectangle(Pens.Yellow, -kaderBreedte / 2 - 1,
                        -kaderHoogte / 2 - 1, kaderBreedte + 1, kaderHoogte + 1);
                else
                    e.Graphics.DrawEllipse(Pens.Yellow, -kaderBreedte / 2 - 1,
                        -kaderHoogte / 2 - 1, kaderBreedte + 1, kaderHoogte + 1);
            }
        }

        protected override void viewer_MouseEnter(object sender, EventArgs e)
        {
            kaderInBeeld = true;
            leesControls();
            doelRechthoek =
                 new Rectangle(-kaderBreedte / 2, -kaderHoogte / 2, kaderBreedte, kaderHoogte);
        }

        protected override void viewer_MouseLeave(object sender, EventArgs e)
        {
            if (kaderVast)
                eindpunt = startpunt;
            else
                kaderInBeeld = false;
            form1.BitmapViewer.Refresh();
        }

        protected override void viewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (kaderInBeeld)
            {
                if (kaderVast)
                        eindpunt = e.Location;
                else
                    startpunt = e.Location;
                form1.BitmapViewer.Refresh();
            }
        }

        protected override void viewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (kaderInBeeld)
            {
                if (e.Button == MouseButtons.Left)
                    if (kaderVast)
                        bronGebiedInvoegen();
                    else
                        kaderVast = true;
                else // met rechter muisknop kader losmaken
                    kaderVast = false;
            }
        }

        void bronGebiedInvoegen()
        {
            kaderInBeeld = kaderVast = false;

            using (Bitmap bitmapBron = maakBitmapBron())
            using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
            {
                g.TranslateTransform((float)startpunt.X, (float)startpunt.Y);
                g.RotateTransform(kaderHoek);
                g.DrawImage(bitmapBron, doelRechthoek,
                    0, 0, kaderBreedte, kaderHoogte, GraphicsUnit.Pixel, attributes);
            }

            Geschiedenis.HuidigeBitmapToevoegen();
            form1.ToonBitmap();
        }

        void control_ValueChanged(object sender, EventArgs e)
        {
            leesControls();
            if (kaderVast)
                form1.BitmapViewer.Refresh();
            buttonFocus.Focus();
        }

        void leesControls()
        {
            rechthoekigKader = checkBoxRechthoekig.Checked;
            kaderBreedte = (int)numericGrootte[0].Value;
            kaderHoogte = (int)numericGrootte[1].Value;
            kaderHoek = (float)numericHoek.Value;
            schaal = (float)numericSchaal.Value / 100;
            gradient = (int)numericGradient.Value;

            attributes = new ImageAttributes();
            if (numericDekking.Value < 100)
            {
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix33 = (float)numericDekking.Value / 100;
                attributes.SetColorMatrix(colorMatrix);
            }
        }

        Bitmap maakBitmapBron()
        {
            int schetsGebied = 2 * (kaderBreedte + kaderHoogte); // schetsgebied moet groter zijn dan bitmapBron i.v.m hoek en schaal
            Rectangle bronRechthoek =
                new Rectangle(eindpunt.X - schetsGebied / 2, eindpunt.Y - schetsGebied / 2, schetsGebied, schetsGebied);

            Region regionTotaal = null;
            Region[] regionGradient = new Region[gradient + 1];
            GraphicsPath pad = new GraphicsPath();
            for (int i = 0; i < gradient + 1; i++)
            {
                pad.Reset();
                if (rechthoekigKader)
                    pad.AddRectangle(
                        new Rectangle(i, i, kaderBreedte - 2 * i, kaderHoogte - 2 * i));
                else
                    pad.AddEllipse(i, i, kaderBreedte - 1 - 2 * i, kaderHoogte - 1 - 2 * i);
                regionGradient[i] = new Region(pad);
                if (i == 0)
                    regionTotaal = new Region(pad);
            }
            for (int i = 0; i < gradient; i++)
                regionGradient[i].Exclude(regionGradient[i + 1]);

            Bitmap bitmapBron = new Bitmap(kaderBreedte, kaderHoogte);
            using (Graphics g = Graphics.FromImage(bitmapBron))
            {
                g.TranslateTransform((float)(kaderBreedte / 2), (float)(kaderHoogte / 2));
                g.RotateTransform(-kaderHoek);
                g.ScaleTransform(schaal, schaal);
                g.DrawImage(Huidige.Bitmap, -schetsGebied / 2, -schetsGebied / 2, bronRechthoek, GraphicsUnit.Pixel);
            }
            for (int x = 0; x < bitmapBron.Width; x++)
                for (int y = 0; y < bitmapBron.Height; y++)
                {
                    for (int i = 0; i < gradient; i++)
                        if (regionGradient[i].IsVisible(x, y))
                        {
                            int alfa = 255 * (i + 1) / (gradient + 1);
                            bitmapBron.SetPixel(x, y, Color.FromArgb(alfa, bitmapBron.GetPixel(x, y))); // gradient-ringen maken
                        }

                    if (regionTotaal.IsVisible(x, y) == false)
                        bitmapBron.SetPixel(x, y, Color.FromArgb(0, Color.Black)); // pixels buiten kader uitsluiten
                }

            return bitmapBron;
        }
    }
}
