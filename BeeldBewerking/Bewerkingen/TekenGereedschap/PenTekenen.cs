using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking.Bewerkingen.TekenGereedschap
{
    class PenTekenen : IGereedschap
    {
        public virtual void HulpFiguur(Graphics g, Pen pen, Point startpunt, Point eindpunt) { }

        public virtual void Tekenen(Graphics g, Pen pen, Point startpunt, Point eindpunt)
        {
            g.DrawLine(pen, startpunt, eindpunt);
        }
    }
}
