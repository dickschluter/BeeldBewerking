using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace BeeldBewerking
{
    class GeenBewerking : BewerkingBasis
    {
        public GeenBewerking(Form1 form1)
            : base(form1)
        {
            labelBewerking.Visible = false;
            form1.InitialiseerBewerking(lijstControls, true);
        }
    }
}
