using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    static class BewaardeBitmaps
        // opslag van door gebruiker bewaarde bitmaps
    {
        static Form1 form1;
        static readonly int maxAantal = 8;
        static List<BitmapMetNaam> container = new List<BitmapMetNaam>();

        public static void Initialiseren(Form1 form1)
        {
            BewaardeBitmaps.form1 = form1;
        }

        public static IEnumerable<string> VerkorteNamen
        {
            get
            {
                return
                    from afbeelding in container
                    select Path.GetFileName(afbeelding.Naam);
            }
        }

        public static Bitmap GeefBitmap(int index)
        {
            if (index >= 0 && index < container.Count)
                return new Bitmap(container[index].Bitmap);
            return null;
        }

        public static void Toevoegen(string naam, Bitmap bitmap, ToolStripMenuItem parentMenuItem)
        {
            if (naam != null && bitmap != null)
            {
                if (container.Count < maxAantal)
                {
                    naam = naam.TrimEnd("[234567]".ToCharArray()); // verwijder eventuele index
                    string uniekeNaam = naam;
                    int nummer = 1;
                    while (container.Count((BitmapMetNaam afb) => afb.Naam == uniekeNaam) == 1) // naam bestaat al
                    {
                        nummer++;
                        uniekeNaam = string.Format("{0}[{1}]", naam, nummer); // voeg index toe
                    }

                    Bitmap bewaardeBitmap = new Bitmap(bitmap);
                    container.Add(new BitmapMetNaam(uniekeNaam, bewaardeBitmap));
                    parentMenuItem.DropDownItems.Add(
                        new ToolStripMenuItem(Path.GetFileName(uniekeNaam), bewaardeBitmap, bitmapKiezen));
                }
                else
                    MessageBox.Show("Maximum aantal bereikt");
            }              
        }

        private static void bitmapKiezen(object sender, EventArgs e)
        {
            string verkorteNaam = (sender as ToolStripMenuItem).Text;
            foreach (BitmapMetNaam afbeelding in container)
                if (Path.GetFileName(afbeelding.Naam) == verkorteNaam)
                {
                    Huidige.Naam = afbeelding.Naam;
                    Huidige.Bitmap = new Bitmap(afbeelding.Bitmap);

                    Geschiedenis.HuidigeBitmapToevoegen();
                    form1.ToonBitmap();
                    return;
                }
        }
    }
}
