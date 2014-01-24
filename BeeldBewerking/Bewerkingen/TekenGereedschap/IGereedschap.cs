using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking.Bewerkingen.TekenGereedschap
{
    interface IGereedschap
    {
        void HulpFiguur(Graphics g, Pen pen, Point startpunt, Point eindpunt);
        void Tekenen(Graphics g, Pen pen, Point startpunt, Point eindpunt);
    }
}
