// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.MapFunctions
{
    /// <summary>
    /// <see cref="MapFunction"/> that can zoom into the map using left mouse clicks or rectangle dragging..
    /// </summary>
    /// <remarks>This is a copy of <see cref="MapFunctionClickZoom"/> with the following changes:
    /// <list type="bullet">
    /// <item>It does not zoom out on right mouse clicks.</item>
    /// <item>It does not zoom when the location of the cursor on <see cref="OnMouseUp"/> is equal to the location set at 
    /// <see cref="OnMouseDown"/>.</item>
    /// </list>
    /// </remarks>
    public class MapFunctionSelectionZoom : MapFunctionZoom
    {
        private readonly Pen selectionPen;
        private Point currentPoint;
        private Coordinate geoStartPoint;
        private bool isDragging;
        private Point startPoint = Point.Empty;
        private bool mouseButtonMiddleDown;

        /// <summary>
        /// Creates a new instance of <see cref="MapFunctionSelectionZoom"/>.
        /// </summary>
        /// <param name="map">Any valid <see cref="IMap"/> interface.</param>
        public MapFunctionSelectionZoom(IMap map) : base(map)
        {
            selectionPen = new Pen(Color.Black)
            {
                DashStyle = DashStyle.Dash
            };
            YieldStyle = YieldStyles.LeftButton | YieldStyles.RightButton | YieldStyles.Scroll;
        }

        protected override void OnDraw(MapDrawArgs e)
        {
            if (isDragging)
            {
                Rectangle rectangle = Opp.RectangleFromPoints(startPoint, currentPoint);
                rectangle.Width -= 1;
                rectangle.Height -= 1;
                e.Graphics.DrawRectangle(Pens.White, rectangle);
                e.Graphics.DrawRectangle(selectionPen, rectangle);
            }

            base.OnDraw(e);
        }

        protected override void OnMouseDown(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                mouseButtonMiddleDown = true;
            }

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

            if (mouseButtonMiddleDown)
            {
                return;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                mouseButtonMiddleDown = false;
            }

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

                if (!handled)
                {
                    e.Map.MapFrame.ResetExtents();
                }
            }

            base.OnMouseUp(e);
            Map.IsBusy = false;
        }
    }
}