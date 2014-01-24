using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeeldBewerking
{
    static class Bewerking
    {
        private static BewerkingBasis huidigeBewerking;

        public static void Kies(Form1 form1, string keuze)
        {
            if (keuze != null && keuze == huidigeBewerking.Naam) // niet zelfde bewerking opnieuw laden
                return;

            if (Huidige.Bitmap != null || keuze == null || keuze == "Lege bitmap maken")
            {
                if (huidigeBewerking != null)
                    huidigeBewerking.Dispose();
                form1.BitmapViewer.Refresh();
                System.Threading.Thread.Sleep(100);

                switch (keuze)
                {
                    case "Afbeelding invoegen":
                        huidigeBewerking = new AfbeeldingInvoegen(form1);
                        break;
                    case "Bijsnijden":
                        huidigeBewerking = new Bijsnijden(form1);
                        break;
                    case "Detail tekenen":
                        huidigeBewerking = new DetailTekenen(form1);
                        break;
                    case "Kleuren":
                        huidigeBewerking = new Kleuren(form1);
                        break;
                    case "Kleuren veranderen":
                        huidigeBewerking = new KleurenVeranderen(form1);
                        break;
                    case "Lege bitmap maken":
                        huidigeBewerking = new LegeBitmap(form1);
                        break;
                    case "Retoucheren":
                        huidigeBewerking = new Retoucheren(form1);
                        break;
                    case "Roteren":
                        huidigeBewerking = new Roteren(form1);
                        break;
                    case "Schalen":
                        huidigeBewerking = new Schalen(form1);
                        break;
                    case "Spiegelen":
                        huidigeBewerking = new Spiegelen(form1);
                        break;
                    case "Tekenen":
                        huidigeBewerking = new Tekenen(form1);
                        break;
                    case "Tekst invoegen":
                        huidigeBewerking = new TekstInvoegen(form1);
                        break;
                    case "Transparantie":
                        huidigeBewerking = new Transparantie(form1);
                        break;
                    case "Uitbreiden":
                        huidigeBewerking = new Uitbreiden(form1);
                        break;
                    case "Vervormen":
                        huidigeBewerking = new Vervormen(form1);
                        break;
                    default:
                        huidigeBewerking = new GeenBewerking(form1);
                        break;
                }
            }
        }
    }
}
