using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    abstract class BewerkingMetHulpfiguren : BewerkingBasis
    {
        protected bool muisIngedrukt, kaderInBeeld, kaderVast;
        protected Point startpunt, eindpunt; // muiscoordinaten
        protected Rectangle doelRechthoek;
        protected ImageAttributes attributes;
        
        public BewerkingMetHulpfiguren(Form1 form1)
            : base(form1)
        {
            form1.BitmapViewer.MouseEnter += viewer_MouseEnter;
            form1.BitmapViewer.MouseLeave += viewer_MouseLeave;
            form1.BitmapViewer.MouseMove += viewer_MouseMove;
            form1.BitmapViewer.MouseDown += viewer_MouseDown;
            form1.BitmapViewer.MouseUp += viewer_MouseUp;
        }

        protected virtual void viewer_Paint(object sender, PaintEventArgs e) { }
        protected virtual void viewer_MouseEnter(object sender, EventArgs e) { }
        protected virtual void viewer_MouseLeave(object sender, EventArgs e) { }
        protected virtual void viewer_MouseMove(object sender, MouseEventArgs e) { }
        protected virtual void viewer_MouseDown(object sender, MouseEventArgs e) { }
        protected virtual void viewer_MouseUp(object sender, MouseEventArgs e) { }

        protected void hulpAfbeelding(Graphics g, Bitmap bitmap, Point p1,
            decimal hoek, decimal schaal, ImageAttributes attributes)
        {
            g.TranslateTransform(p1.X, p1.Y);
            g.ScaleTransform((float)schaal, (float)schaal);
            g.RotateTransform((float)hoek);
            g.DrawImage(bitmap, new Rectangle(Point.Empty, bitmap.Size),
                0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        }

        public override void Dispose()
        {
            base.Dispose();

            form1.BitmapViewer.Cursor = Cursors.Default;
            form1.BitmapViewer.Paint -= viewer_Paint;
            form1.BitmapViewer.MouseEnter -= viewer_MouseEnter;
            form1.BitmapViewer.MouseLeave -= viewer_MouseLeave;
            form1.BitmapViewer.MouseMove -= viewer_MouseMove;
            form1.BitmapViewer.MouseDown -= viewer_MouseDown;
            form1.BitmapViewer.MouseUp -= viewer_MouseUp;
        }
    }
}
