using System.ComponentModel;
using System.Drawing;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    public class cBlendItems
    {
        // Fields

        // Methods
        public cBlendItems() {}

        public cBlendItems(Color[] color, float[] Pt)
        {
            iColor = color;
            iPoint = Pt;
        }

        // Properties
        [Category("Appearance")]
        [Description("The Color for the Point")]
        public Color[] iColor { get; set; }

        [Description("The Color for the Point")]
        [Category("Appearance")]
        public float[] iPoint { get; set; }

        public override string ToString()
        {
            return "BlendItems";
        }
    }
}