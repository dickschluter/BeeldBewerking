using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking.Bewerkingen.TekenGereedschap
{
    class RechthoekTekenen : IGereedschap
    {
        public virtual void HulpFiguur(Graphics g, Pen pen, Point startpunt, Point eindpunt)
        {
            g.DrawRectangle(pen, geefRechthoek(startpunt, eindpunt));
        }

        public virtual void Tekenen(Graphics g, Pen pen, Point startpunt, Point eindpunt)
        {
            g.DrawRectangle(pen, geefRechthoek(startpunt, eindpunt));
        }

        protected Rectangle geefRechthoek(Point startpunt, Point eindpunt)
        {
            return new Rectangle(Math.Min(startpunt.X, eindpunt.X), Math.Min(startpunt.Y, eindpunt.Y),
                Math.Abs(startpunt.X - eindpunt.X), Math.Abs(startpunt.Y - eindpunt.Y));
        }
    }
}
