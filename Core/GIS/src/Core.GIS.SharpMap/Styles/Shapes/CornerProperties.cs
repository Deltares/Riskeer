using System;
using System.ComponentModel;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace Core.GIS.SharpMap.Styles.Shapes
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
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
            _All = -1;
            _UpperLeft = 0;
            _UpperRight = 0;
            _LowerLeft = 0;
            _LowerRight = 0;
            LowerLeft = 0;
            LowerRight = 0;
            UpperLeft = 0;
            UpperRight = 0;
        }

        public CornersProperty(short LowerLeft, short LowerRight, short UpperLeft, short UpperRight)
        {
            _All = -1;
            _UpperLeft = 0;
            _UpperRight = 0;
            _LowerLeft = 0;
            _LowerRight = 0;
            this.LowerLeft = LowerLeft;
            this.LowerRight = LowerRight;
            this.UpperLeft = UpperLeft;
            this.UpperRight = UpperRight;
        }

        // Properties
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Set the Radius of the All four Corners the same")]
        [NotifyParentProperty(true)]
        public short All
        {
            get
            {
                return _All;
            }
            set
            {
                _All = value;
                if (value > -1)
                {
                    LowerLeft = value;
                    LowerRight = value;
                    UpperLeft = value;
                    UpperRight = value;
                }
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [Description("Set the Radius of the Lower Left Corner")]
        public short LowerLeft
        {
            get
            {
                return _LowerLeft;
            }
            set
            {
                _LowerLeft = value;
                CheckForAll(value);
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Set the Radius of the Lower Right Corner")]
        [NotifyParentProperty(true)]
        public short LowerRight
        {
            get
            {
                return _LowerRight;
            }
            set
            {
                _LowerRight = value;
                CheckForAll(value);
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [Description("Set the Radius of the Upper Left Corner")]
        public short UpperLeft
        {
            get
            {
                return _UpperLeft;
            }
            set
            {
                _UpperLeft = value;
                CheckForAll(value);
            }
        }

        [NotifyParentProperty(true)]
        [Description("Set the Radius of the Upper Right Corner")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public short UpperRight
        {
            get
            {
                return _UpperRight;
            }
            set
            {
                _UpperRight = value;
                CheckForAll(value);
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", new object[]
            {
                LowerLeft,
                LowerRight,
                UpperLeft,
                UpperRight
            });
        }

        private void CheckForAll(short val)
        {
            if (((val == LowerLeft) && (val == LowerRight)) && ((val == UpperLeft) && (val == UpperRight)))
            {
                if (All != val)
                {
                    All = val;
                }
            }
            else if (All != -1)
            {
                All = -1;
            }
        }
    }
}