using System.ComponentModel;
using System.Drawing;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    public class cBlendItems
    {
        // Fields
        private System.Drawing.Color[] _iColor;
        private float[] _iPoint;

        // Methods
        public cBlendItems()
        {
        }

        public cBlendItems(Color[] color, float[] Pt)
        {
            this.iColor = color;
            this.iPoint = Pt;
        }

        public override string ToString()
        {
            return "BlendItems";
        }

        // Properties
        [Category("Appearance"), Description("The Color for the Point")]
        public System.Drawing.Color[] iColor
        {
            get
            {
                return this._iColor;
            }
            set
            {
                this._iColor = value;
            }
        }

        [Description("The Color for the Point"), Category("Appearance")]
        public float[] iPoint
        {
            get
            {
                return this._iPoint;
            }
            set
            {
                this._iPoint = value;
            }
        }
    }
}