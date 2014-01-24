using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    static class DialoogVensters
    {
        public static OpenFileDialog OpenenDialoog { get; private set; }
        public static SaveFileDialog OpslaanDialoog { get; private set; }
        public static ColorDialog KleurDialoog { get; private set; }

        static DialoogVensters()
        {
            OpenenDialoog = new OpenFileDialog();
            OpenenDialoog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            OpenenDialoog.Filter = "Alle afbeeldingen|*.jpg;*.bmp;*.gif;*.png;*.ico";

            OpslaanDialoog = new SaveFileDialog();

            KleurDialoog = new ColorDialog();
            KleurDialoog.FullOpen = true;
        }
    }
}
