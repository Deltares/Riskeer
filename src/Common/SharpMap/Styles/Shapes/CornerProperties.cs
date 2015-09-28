using System;
using System.ComponentModel;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    [Serializable, TypeConverter(typeof(ExpandableObjectConverter))]
    public class CornersProperty
    {
        // Fields
        private short _All;
        private short _LowerLeft;
        private short _LowerRight;
        private short _UpperLeft;
        private short _UpperRight;

        // Methods
        public CornersProperty()
        {
            this._All = -1;
            this._UpperLeft = 0;
            this._UpperRight = 0;
            this._LowerLeft = 0;
            this._LowerRight = 0;
            this.LowerLeft = 0;
            this.LowerRight = 0;
            this.UpperLeft = 0;
            this.UpperRight = 0;
        }

        public CornersProperty(short LowerLeft, short LowerRight, short UpperLeft, short UpperRight)
        {
            this._All = -1;
            this._UpperLeft = 0;
            this._UpperRight = 0;
            this._LowerLeft = 0;
            this._LowerRight = 0;
            this.LowerLeft = LowerLeft;
            this.LowerRight = LowerRight;
            this.UpperLeft = UpperLeft;
            this.UpperRight = UpperRight;
        }

        private void CheckForAll(short val)
        {
            if (((val == this.LowerLeft) && (val == this.LowerRight)) && ((val == this.UpperLeft) && (val == this.UpperRight)))
            {
                if (this.All != val)
                {
                    this.All = val;
                }
            }
            else if (this.All != -1)
            {
                this.All = -1;
            }
        }

        // Properties
        [RefreshProperties(RefreshProperties.Repaint), Description("Set the Radius of the All four Corners the same"), NotifyParentProperty(true)]
        public short All
        {
            get
            {
                return this._All;
            }
            set
            {
                this._All = value;
                if (value > -1)
                {
                    this.LowerLeft = value;
                    this.LowerRight = value;
                    this.UpperLeft = value;
                    this.UpperRight = value;
                }
            }
        }

        [RefreshProperties(RefreshProperties.Repaint), NotifyParentProperty(true), Description("Set the Radius of the Lower Left Corner")]
        public short LowerLeft
        {
            get
            {
                return this._LowerLeft;
            }
            set
            {
                this._LowerLeft = value;
                this.CheckForAll(value);
            }
        }

        [RefreshProperties(RefreshProperties.Repaint), Description("Set the Radius of the Lower Right Corner"), NotifyParentProperty(true)]
        public short LowerRight
        {
            get
            {
                return this._LowerRight;
            }
            set
            {
                this._LowerRight = value;
                this.CheckForAll(value);
            }
        }

        [RefreshProperties(RefreshProperties.Repaint), NotifyParentProperty(true), Description("Set the Radius of the Upper Left Corner")]
        public short UpperLeft
        {
            get
            {
                return this._UpperLeft;
            }
            set
            {
                this._UpperLeft = value;
                this.CheckForAll(value);
            }
        }

        [NotifyParentProperty(true), Description("Set the Radius of the Upper Right Corner"), RefreshProperties(RefreshProperties.Repaint)]
        public short UpperRight
        {
            get
            {
                return this._UpperRight;
            }
            set
            {
                this._UpperRight = value;
                this.CheckForAll(value);
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", new object[] { LowerLeft, LowerRight, UpperLeft, UpperRight });
        }
    }
}