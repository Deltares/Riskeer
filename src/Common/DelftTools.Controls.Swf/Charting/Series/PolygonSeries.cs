using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Steema.TeeChart.Drawing;

namespace DelftTools.Controls.Swf.Charting.Series
{
    /// <summary>
    /// TeeChart wrapper class to fix PolygonSeries when drawn into a 2D axes (was meant for drawing on a Map).
    /// </summary>
    internal class PolygonSeries : Steema.TeeChart.Styles.PolygonSeries
    {
        private bool autoClose;

        /// <summary>
        /// Closes the polygon automatically when true.
        /// </summary>
        public bool AutoClose
        {
            get { return autoClose; }
            set
            {
                autoClose = value;
                Repaint();
            }
        }

        /// <summary>
        /// Override Draw method and use the TeeChart Graphics3D object to draw the polygon shape
        /// </summary>
        public override void Draw()
        {
            if (XValues.Count < 2)
            {
                return;
            }

            if (XValues.Count != YValues.Count)
            {
                // Just to be sure. I think TeeChart already accounts for this in Add methods.
                throw new Exception("Number of X values should be equal to the number of Y values");
            }

            var g = Chart.Graphics3D;
            PrepareGraphics3D(g);

            g.ClipRectangle(Chart.ChartBounds);

            // Draw polygon groups:
            foreach (var pointList in GetDrawingPoints())
            {
                g.Polygon(pointList);
            }

            g.UnClip();
        }

        /// <summary>
        /// Override DrawLegendShape because it includes a call to Polygon().ParentShape.... which is null if we do not use Polygon to create it.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="valueIndex"></param>
        /// <param name="rect"></param>
        protected override void DrawLegendShape(Graphics3D g, int valueIndex, Rectangle rect)
        {
            PrepareGraphics3D(g);

            var oldWidth = 1;
            if (g.Pen.Visible)
            {
                oldWidth = g.Pen.Width;
                g.Pen.Width = 1;
            }

            base.DrawLegendShape(g, valueIndex, rect);

            if (g.Pen.Visible)
            {
                g.Pen.Width = oldWidth;
            }
        }

        /// <summary>
        /// Override SetActive because it includes a call to Polygon().ParentShape.... which is null if we do not use Polygon to create it.
        /// </summary>
        /// <param name="value"></param>
        protected override void SetActive(bool value)
        {
            SetBooleanProperty(ref bActive, value);
        }

        /// <summary>
        /// Override PrepareLegendCanvas because it includes a call to Polygon().ParentShape.... which is null if we do not use Polygon to create it.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="valueIndex"></param>
        /// <param name="backColor"></param>
        /// <param name="aBrush"></param>
        protected override void PrepareLegendCanvas(Graphics3D g, int valueIndex, ref Color backColor, ref ChartBrush aBrush)
        {
        }

        private void PrepareGraphics3D(Graphics3D g)
        {
            g.Pen = Pen;
            g.Brush = bBrush;
            g.Pen.DrawingPen.LineJoin = LineJoin.Round;
        }

        private IEnumerable<Point[]> GetDrawingPoints()
        {
            var pointGroupsToRender = new List<Point[]>();
            var currentPointsGroup = new List<Point>();
            for (int i = 0; i < XValues.Count; i++)
            {
                var xValue = XValues[i];
                var yValue = YValues[i];
                if (double.IsNaN(xValue) || double.IsNaN(yValue))
                {
                    continue;
                }
                // TODO: How to distinguish null??
                // Can cause collision when a regular value has the same value!
                if (xValue == DefaultNullValue || yValue == DefaultNullValue)
                {
                    AddToPointsList(pointGroupsToRender, currentPointsGroup);
                    continue;
                }

                currentPointsGroup.Add(new Point(CalcXPosValue(xValue), CalcYPosValue(yValue)));
            }

            AddToPointsList(pointGroupsToRender, currentPointsGroup);
            return pointGroupsToRender;
        }

        private void AddToPointsList(ICollection<Point[]> pointGroupsToRender, List<Point> currentPointsGroup)
        {
            if (!currentPointsGroup.Any()) return; // Do nothing for empty list

            if (autoClose && (currentPointsGroup[0].X != currentPointsGroup.Last().X || currentPointsGroup[0].Y != currentPointsGroup.Last().Y))
            {
                currentPointsGroup.Add(currentPointsGroup[0]);
            }
            pointGroupsToRender.Add(currentPointsGroup.ToArray());
            currentPointsGroup.Clear();
        }
    }
}