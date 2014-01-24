using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking.Bewerkingen.TekenGereedschap
{
    class EllipsTekenen : RechthoekTekenen
    {
        public override void HulpFiguur(Graphics g, Pen pen, Point startpunt, Point eindpunt)
        {
            g.DrawEllipse(pen, geefRechthoek(startpunt, eindpunt));
        }

        public override void Tekenen(Graphics g, Pen pen, Point startpunt, Point eindpunt)
        {
            g.DrawEllipse(pen, geefRechthoek(startpunt, eindpunt));
        }
    }
}
