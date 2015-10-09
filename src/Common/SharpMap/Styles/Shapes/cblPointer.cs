using System.Drawing;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    internal class cblPointer
    {
        // Fields

        // Methods
        public cblPointer(float pt, Color c, bool IsCurr)
        {
            pPos = pt;
            pColor = c;
            pIsCurr = IsCurr;
        }

        // Properties
        public Color pColor { get; set; }

        public bool pIsCurr { get; set; }

        public float pPos { get; set; }
    }
}