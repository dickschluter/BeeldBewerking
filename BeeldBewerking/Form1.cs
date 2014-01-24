using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    public partial class Form1 : Form
    {
        public BitmapViewer BitmapViewer { get; private set; }

        public bool SchaalWeergave { get; private set; }
        public bool SchaalWeergaveToestaan { get; private set; }
        public bool WeergaveOpBijscherm { get; private set; }
        public bool VolledigScherm { get; private set; }

        public Form1()
        {
            InitializeComponent();

            toolStripStatusLabelNaam.Text = null;
            toolStripStatusLabelFormaat.Text = null;
            toolStripStatusLabelSchaal.Text = null;
            toolStripStatusLabelPositie.Text = null;
            toolStripStatusLabelRGB.Text = null;
            toolStripStatusLabelA.Text = null;
            toolStripProgressBar1.Visible = false;

            SchaalWeergave = true;
            SchaalWeergaveToestaan = true;

            BitmapViewer = new BitmapViewer();
            BitmapViewer.MouseClick += viewer_MouseClick;
            panelViewer.Controls.Add(BitmapViewer);

            Geschiedenis.Initialiseren(this);
            BewaardeBitmaps.Initialiseren(this);
            Bewerking.Kies(this, null);
        }

        #region Publieke methoden

        public void ToonBitmap()
        {
            if (Huidige.Bitmap != null)
            {
                BitmapViewer.ToonBitmap(Huidige.Bitmap, SchaalWeergave);
                toolStripStatusLabelNaam.Text = Huidige.Naam;
                toolStripStatusLabelFormaat.Text =
                    string.Format("Formaat = {0} x {1}", Huidige.Bitmap.Width, Huidige.Bitmap.Height);
                toolStripStatusLabelSchaal.Text =
                    string.Format("Schaal = {0}", BitmapViewer.Schaal.ToString("0.00"));
                toolStripStatusLabelPositie.Text = null;
                toolStripStatusLabelRGB.Text = null;
                toolStripStatusLabelA.Text = null;
            }
        }
        
        public void InitialiseerBewerking(List<Control> lijstControls, bool schaalWeergaveToestaan)
        {
            if (lijstControls != null)
                foreach (Control control in lijstControls)
                {
                    if (control.Parent == null)
                        panelBewerking.Controls.Add(control);
                    else
                        control.Parent.Controls.Add(control);
                }
            
            SchaalWeergave = schaalWeergaveToestaan;
            SchaalWeergaveToestaan = schaalWeergaveToestaan;
            ToonBitmap();
        }

        public void StartProgressBar()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = true;
            statusStrip1.Update();
        }

        public void UpdateProgressBar(int percentage)
        {
            toolStripProgressBar1.Value = percentage;
        }

        public void StopProgressBar()
        {
            toolStripProgressBar1.Visible = false;
        }

        #endregion

        #region Private methoden

        private void viewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (VolledigScherm)
                volledigSchermSluiten(this, null);
            else if (BitmapViewer.Image != null)
            {
                int x = (int)(e.X / BitmapViewer.Schaal);
                int y = (int)(e.Y / BitmapViewer.Schaal);
                Color kleur = (BitmapViewer.Image as Bitmap).GetPixel(e.X, e.Y);
                toolStripStatusLabelPositie.Text = string.Format("Positie = {0}, {1}", x, y);
                toolStripStatusLabelRGB.Text = string.Format("RGB = {0}, {1}, {2}", kleur.R, kleur.G, kleur.B);
                toolStripStatusLabelA.Text = kleur.A == 255 ? null : string.Format("A = {0}", kleur.A);
            }
        }

        private void maakStandaardIndeling(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                panelBewerking.Size = new Size(200, ClientSize.Height - statusStrip1.Height);
                panelBewerking.Location = new Point(ClientSize.Width - 200, 0);
                panelBewerking.Visible = true;

                statusStrip1.Visible = true;

                panelViewer.Size = new Size(ClientSize.Width - 200, ClientSize.Height - statusStrip1.Height);
                ToonBitmap();
            }
        }

        private void kiesBewerking(object sender, EventArgs e)
        {
            Bewerking.Kies(this, (sender as ToolStripItem).Text);
        }

        #endregion

        #region Bestand

        private void openenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialoogVensters.OpenenDialoog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Bitmap tijdelijkeBitmap = new Bitmap(DialoogVensters.OpenenDialoog.FileName))
                    {
                        Huidige.Naam = DialoogVensters.OpenenDialoog.FileName;
                        Huidige.Bitmap = new Bitmap(tijdelijkeBitmap);
                    }

                    Geschiedenis.HuidigeBitmapToevoegen();
                    ToonBitmap();
                }
                catch
                {
                    MessageBox.Show("Bestand bevat geen afbeelding");
                }
            }
        }

        private void opslaanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opslaan(false);
        }

        private void opslaanAlsBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opslaan(true);
        }

        void opslaan(bool alsBitmap)
        {
            if (Huidige.Bitmap == null)
                return;

            if (Huidige.Naam != "")
                DialoogVensters.OpslaanDialoog.FileName = System.IO.Path.GetFileNameWithoutExtension(Huidige.Naam);
            DialoogVensters.OpslaanDialoog.Filter = alsBitmap ? "Bitmap-bestand|*.bmp" : "Jpeg-bestand|*.jpg";

            if (DialoogVensters.OpslaanDialoog.ShowDialog() == DialogResult.OK)
            {
                Huidige.Bitmap.Save(DialoogVensters.OpslaanDialoog.FileName,
                    (alsBitmap ? ImageFormat.Bmp : ImageFormat.Jpeg));
                Huidige.Naam = DialoogVensters.OpslaanDialoog.FileName;
                ToonBitmap();
            }
        }

        private void huidigeBitmapBewarenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BewaardeBitmaps.Toevoegen(Huidige.Naam, Huidige.Bitmap, bewaardeBitmapsToolStripMenuItem);
        }

        private void legeBitmapMakenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bewerking.Kies(this, "Lege bitmap maken");
        }

        private void schermFotoMakenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            System.Threading.Thread.Sleep(4000);

            Huidige.Naam = "Naamloos";
            Rectangle schermGrootte = Screen.PrimaryScreen.Bounds;
            Huidige.Bitmap = new Bitmap(schermGrootte.Width, schermGrootte.Height);
            using (Graphics g = Graphics.FromImage(Huidige.Bitmap))
                g.CopyFromScreen(0, 0, 0, 0, new Size(schermGrootte.Width, schermGrootte.Height));

            Geschiedenis.HuidigeBitmapToevoegen();
            ToonBitmap();
            this.Show();
        }

        private void minimaliserenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void afsluitenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Weergave

        private void weergaveToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            geschaaldToolStripMenuItem.Enabled = SchaalWeergaveToestaan;
            geschaaldToolStripMenuItem.Image = SchaalWeergave ? Properties.Resources.Vinkje : null;
            nietGeschaaldToolStripMenuItem.Image = SchaalWeergave ? null : Properties.Resources.Vinkje;

            zwarteAchtergrondToolStripMenuItem.Image = panelViewer.BackColor == Color.Black ? Properties.Resources.Vinkje : null;
            witteAchtergrondToolStripMenuItem.Image = panelViewer.BackColor == Color.White ? Properties.Resources.Vinkje : null;

            opHoofdschermToolStripMenuItem.Image = WeergaveOpBijscherm ? null :Properties.Resources.Vinkje;
            opBijschermToolStripMenuItem.Image = WeergaveOpBijscherm ? Properties.Resources.Vinkje : null;
            opBijschermToolStripMenuItem.Enabled = (Screen.AllScreens.Length > 1);
        }

        private void geschaaldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SchaalWeergave = true;
            ToonBitmap();
        }

        private void nietGeschaaldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SchaalWeergave = false;
            ToonBitmap();
        }

        private void zwarteAchtergrondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelViewer.BackColor = Color.Black;
        }

        private void witteAchtergrondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelViewer.BackColor = Color.White;
        }

        private void opHoofdschermToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WeergaveOpBijscherm = false;
            this.Location = Point.Empty;
            this.WindowState = FormWindowState.Maximized;
        }

        private void opBijschermToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WeergaveOpBijscherm = true;
            this.WindowState = FormWindowState.Normal;
            this.Location = Screen.AllScreens[1].Bounds.Location;
            this.ClientSize = Screen.AllScreens[1].Bounds.Size;
        }

        private void volledigSchermToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VolledigScherm = true;
            panelBewerking.Visible = false;
            statusStrip1.Visible = false;
            panelViewer.Size = this.Size;
            ToonBitmap();
        }

        private void volledigSchermSluiten(object sender, EventArgs e)
        {
            VolledigScherm = false;
            maakStandaardIndeling(this, null);
        }

        private void panelViewer_Click(object sender, EventArgs e)
        {
            if (VolledigScherm)
                volledigSchermSluiten(this, null);
        }

        #endregion
    }
}
