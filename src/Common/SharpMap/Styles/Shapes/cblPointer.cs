using System.Drawing;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    internal class cblPointer
    {
        // Fields
        private Color _pColor;
        private bool _pIsCurr;
        private float _pPos;

        // Methods
        public cblPointer(float pt, Color c, bool IsCurr)
        {
            this.pPos = pt;
            this.pColor = c;
            this.pIsCurr = IsCurr;
        }

        // Properties
        public Color pColor
        {
            get
            {
                return this._pColor;
            }
            set
            {
                this._pColor = value;
            }
        }

        public bool pIsCurr
        {
            get
            {
                return this._pIsCurr;
            }
            set
            {
                this._pIsCurr = value;
            }
        }

        public float pPos
        {
            get
            {
                return this._pPos;
            }
            set
            {
                this._pPos = value;
            }
        }
    }
}