using System.Drawing;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    public class cFocalPoints
    {
        // Fields
        private PointF _CenterPoint;
        private PointF _FocusScales;

        // Methods
        public cFocalPoints()
        {
            _CenterPoint = new PointF(0.5f, 0.5f);
            _FocusScales = new PointF(0f, 0f);
        }

        public cFocalPoints(PointF ptC, PointF ptF)
        {
            _CenterPoint = new PointF(0.5f, 0.5f);
            _FocusScales = new PointF(0f, 0f);
            CenterPoint = ptC;
            FocusScales = ptF;
        }

        public cFocalPoints(double Cx, double Cy, double Fx, double Fy)
        {
            _CenterPoint = new PointF(0.5f, 0.5f);
            _FocusScales = new PointF(0f, 0f);
            PointF S0 = new PointF((float) Cx, (float) Cy);
            CenterPoint = S0;
            S0 = new PointF((float) Fx, (float) Fy);
            FocusScales = S0;
        }

        // Properties
        public PointF CenterPoint
        {
            get
            {
                return _CenterPoint;
            }
            set
            {
                if (value.X < 0f)
                {
                    value.X = 0f;
                }
                if (value.X > 1f)
                {
                    value.X = 1f;
                }
                if (value.Y < 0f)
                {
                    value.Y = 0f;
                }
                if (value.Y > 1f)
                {
                    value.Y = 1f;
                }
                _CenterPoint = value;
            }
        }

        public PointF FocusScales
        {
            get
            {
                return _FocusScales;
            }
            set
            {
                if (value.X < 0f)
                {
                    value.X = 0f;
                }
                if (value.X > 1f)
                {
                    value.X = 1f;
                }
                if (value.Y < 0f)
                {
                    value.Y = 0f;
                }
                if (value.Y > 1f)
                {
                    value.Y = 1f;
                }
                _FocusScales = value;
            }
        }

        public override string ToString()
        {
            return ("CP=" + _CenterPoint.ToString() + ", FP=" + _FocusScales.ToString());
        }
    }
}