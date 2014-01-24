using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BeeldBewerking
{
    public class BitmapMetNaam
        // Dataklasse voor BitmapGeschiedenis en BitmapContainer
    {
        public string Naam { get; private set; }
        public Bitmap Bitmap { get; private set; }

        public BitmapMetNaam(string naam, Bitmap bitmap)
        {
            Naam = naam;
            Bitmap = bitmap;
        }

        public BitmapMetNaam Clone()
        {
            return new BitmapMetNaam(Naam, new Bitmap(Bitmap));
        }
    }
}
