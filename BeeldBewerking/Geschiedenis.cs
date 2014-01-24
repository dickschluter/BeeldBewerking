using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeeldBewerking
{
    static class Geschiedenis
        // opslag van voorgaande bitmaps
    {
        public static bool BevatVorige { get; private set; } // false als huidige == bodem
        public static bool BevatVolgende { get; private set; } // false als huidige == top

        public static event EventHandler StatusGewijzigd; // geeft aan dat BevatVorige of BevatVolgende gewijzigd kan zijn

        static Form1 form1;
        static BitmapMetNaam[] stack; // circulair array als stack. Indien vol wordt onderste overschreven
        static readonly int maxAantal = 8;
        static int huidige, bodem, top;
        static bool isLeeg = true; // alleen true voor eerste keer toevoegen

        public static void Initialiseren(Form1 form1)
        {
            Geschiedenis.form1 = form1;
            stack = new BitmapMetNaam[maxAantal];
        }

        public static void HuidigeBitmapToevoegen()
        {
            Toevoegen(Huidige.Naam, Huidige.Bitmap);
        }

        public static void Toevoegen(string naam, Bitmap bitmap)
        {
            if (bitmap == null)
                return;
            
            if (isLeeg)
                isLeeg = false;
            else
            {
                huidige = (huidige + 1) % maxAantal;
                if (huidige == bodem)
                    bodem = (bodem + 1) % maxAantal;
                top = huidige;
                BevatVorige = true;
            }

            BevatVolgende = false;
            bijStatusWijziging();

            stack[huidige] = new BitmapMetNaam(naam, new Bitmap(bitmap));
        }

        public static bool Vorige(out BitmapMetNaam bitmapMetNaam)
        {
            bitmapMetNaam = null;
            bool retourneertBitmap = BevatVorige;
            
            if (BevatVorige)
            {
                huidige = (huidige + maxAantal - 1) % maxAantal;
                BevatVorige = (huidige != bodem);
                BevatVolgende = true;
                bijStatusWijziging();

                bitmapMetNaam = stack[huidige].Clone();
            }

            return retourneertBitmap;
        }

        public static bool LaatstToegevoegd(out BitmapMetNaam bitmapMetNaam)
        {
            if (isLeeg)
            {
                bitmapMetNaam = null;
                return false;
            }

            bijStatusWijziging();
            bitmapMetNaam = stack[huidige].Clone();
            return true;
        }

        public static bool Volgende(out BitmapMetNaam bitmapMetNaam)
        {
            bitmapMetNaam = null;
            bool retourneertBitmap = BevatVolgende;

            if (BevatVolgende)
            {
                huidige = (huidige + 1) % maxAantal;
                BevatVorige = true;
                BevatVolgende = (huidige != top);
                bijStatusWijziging();

                bitmapMetNaam = stack[huidige].Clone();
            }

            return retourneertBitmap;
        }

        static void bijStatusWijziging()
        {
            if (StatusGewijzigd != null)
                StatusGewijzigd(null, null);
        }
    }
}
