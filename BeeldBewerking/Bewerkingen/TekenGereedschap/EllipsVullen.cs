using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking.Bewerkingen.TekenGereedschap
{
    class EllipsVullen : EllipsTekenen
    {
        public override void Tekenen(Graphics g, Pen pen, Point startpunt, Point eindpunt)
        {
            g.FillEllipse(new SolidBrush(pen.Color), geefRechthoek(startpunt, eindpunt));
        }
    }
}
