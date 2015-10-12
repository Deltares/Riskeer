﻿namespace DelftTools.Controls.Swf.Charting
{
    public struct ChartRectangle
    {
        public double Left;
        public double Right;
        public double Bottom;
        public double Top;

        public ChartRectangle(double left, double right, double bottom, double top)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }
    }
}