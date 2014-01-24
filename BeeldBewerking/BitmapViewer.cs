using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    public class BitmapViewer : PictureBox
    {
        public decimal Schaal { get; private set; }
        private Bitmap bitmapGeschaald;

        public BitmapViewer()
        {
            this.DoubleBuffered = true;
        }

        public void ToonBitmap(Bitmap bitmap, bool schaalWeergave)
        {
            if (schaalWeergave && (bitmap.Width > this.Parent.Width || bitmap.Height > this.Parent.Height))
            {
                decimal ruweSchaal =
                    Math.Min((decimal)this.Parent.Width / bitmap.Width, (decimal)this.Parent.Height / bitmap.Height);
                Schaal = Math.Floor(ruweSchaal * 100) / 100;
                toonBitmapGeschaald(bitmap);
            }
            else
            {
                Schaal = 1.0M;
                this.Image = bitmap;
            }

            veranderAfmeting();
        }

        private void toonBitmapGeschaald(Bitmap bitmap)
        {
            if (bitmapGeschaald != null)
                bitmapGeschaald.Dispose();

            bitmapGeschaald = new Bitmap((int)(bitmap.Width * Schaal),
                    (int)(bitmap.Height * Schaal));
            using (Graphics graphics = Graphics.FromImage(bitmapGeschaald))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage
                    (bitmap, 0, 0, bitmapGeschaald.Width, bitmapGeschaald.Height);
            }
            this.Image = bitmapGeschaald;
        }

        private void veranderAfmeting()
        {
            if (this.Image != null)
            {
                int xPositie = (this.Parent.Width - this.Image.Width) / 2;
                if (xPositie < 0)
                    xPositie = 0;
                int yPositie = (this.Parent.Height - this.Image.Height) / 2;
                if (yPositie < 0)
                    yPositie = 0;
                if (yPositie > 200)
                    yPositie = 200;

                (Parent as Panel).AutoScrollPosition = new Point(0, 0);
                this.Size = this.Image.Size;
                this.Location = new Point(xPositie, yPositie);
            }
        }
    }
}
