using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial
{
    /// <summary>
    /// <see cref="MapFunction"/> that can zoom into the map using left mouse clicks or rectangle dragging. It zooms out on right mouse clicks.
    /// </summary>
    /// <remarks>This is a copy of <see cref="DotSpatial.Controls.MapFunctionClickZoom"/> with the following changes:
    /// <list type="bullet">
    /// <item>It does not zoom out on right mouse clicks.</item>
    /// <item>It does not zoom when the location of the cursus on <see cref="OnMouseUp"/> is equal to the location set at 
    /// <see cref="OnMouseDown"/>.</item></list></remarks>
    public class MapFunctionSelectionZoom : MapFunction
    {
        private readonly Pen selectionPen;
        private Point currentPoint;
        private Coordinate geoStartPoint;
        private bool isDragging;
        private Point startPoint = Point.Empty;

        /// <summary>
        /// Creates a new instance of <see cref="MapFunctionSelectionZoom"/>.
        /// </summary>
        public MapFunctionSelectionZoom(IMap inMap) : base(inMap)
        {
            selectionPen = new Pen(Color.Black)
            {
                DashStyle = DashStyle.Dash
            };
            YieldStyle = YieldStyles.LeftButton | YieldStyles.RightButton;
        }

        protected override void OnDraw(MapDrawArgs e)
        {
            if (isDragging)
            {
                var rectangle = Opp.RectangleFromPoints(startPoint, currentPoint);
                rectangle.Width -= 1;
                rectangle.Height -= 1;
                e.Graphics.DrawRectangle(Pens.White, rectangle);
                e.Graphics.DrawRectangle(selectionPen, rectangle);
            }
            base.OnDraw(e);
        }

        protected override void OnMouseDown(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                currentPoint = startPoint;
                geoStartPoint = e.GeographicLocation;
                isDragging = true;
                Map.IsBusy = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(GeoMouseArgs e)
        {
            if (isDragging)
            {
                int x = Math.Min(Math.Min(startPoint.X, currentPoint.X), e.X);
                int y = Math.Min(Math.Min(startPoint.Y, currentPoint.Y), e.Y);
                int mx = Math.Max(Math.Max(startPoint.X, currentPoint.X), e.X);
                int my = Math.Max(Math.Max(startPoint.Y, currentPoint.Y), e.Y);
                currentPoint = e.Location;
                Map.Invalidate(new Rectangle(x, y, mx - x, my - y));
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(GeoMouseArgs e)
        {
            if (!(e.Map.IsZoomedToMaxExtent && e.Button == MouseButtons.Right))
            {
                e.Map.IsZoomedToMaxExtent = false;
                var handled = false;
                currentPoint = e.Location;

                Map.Invalidate();
                if (isDragging)
                {
                    if (startPoint == currentPoint)
                    {
                        handled = true;
                    }
                    else if (geoStartPoint != null)
                    {
                        IEnvelope env = new Envelope(geoStartPoint.X, e.GeographicLocation.X,
                                                     geoStartPoint.Y, e.GeographicLocation.Y);
                        if (Math.Abs(e.X - startPoint.X) > 1 && Math.Abs(e.Y - startPoint.Y) > 1)
                        {
                            e.Map.ViewExtents = env.ToExtent();
                            handled = true;
                        }
                    }
                }
                isDragging = false;

                if (handled == false)
                {
                    Rectangle r = e.Map.MapFrame.View;
                    var w = r.Width;
                    var h = r.Height;
                    if (e.Button == MouseButtons.Left)
                    {
                        r.Inflate(-r.Width/4, -r.Height/4);
                        // The mouse cursor should anchor the geographic location during zoom.
                        r.X += (e.X/2) - w/4;
                        r.Y += (e.Y/2) - h/4;
                        e.Map.MapFrame.View = r;
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        startPoint = Point.Empty;
                    }

                    e.Map.MapFrame.ResetExtents();
                }
            }

            base.OnMouseUp(e);
            Map.IsBusy = false;
        }
    }
}