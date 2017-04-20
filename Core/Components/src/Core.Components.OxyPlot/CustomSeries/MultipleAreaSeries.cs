// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.CustomSeries
{
    /// <summary>
    /// Represents multiple area series that fills the polygons defined by each collection of points.
    /// </summary>
    public class MultipleAreaSeries : XYAxisSeries
    {
        private readonly OxyColor defaultColor = OxyColors.Fuchsia;

        /// <summary>
        /// Creates a new instance of <see cref="MultipleAreaSeries"/>.
        /// </summary>
        public MultipleAreaSeries()
        {
            Areas = new List<DataPoint[]>();
            Fill = OxyColors.Automatic;
            Color = OxyColors.Automatic;
        }

        public List<DataPoint[]> Areas { get; private set; }

        /// <summary>
        /// Gets or sets the color of the curve.
        /// </summary>
        /// <value>The color.</value>
        public OxyColor Color { get; set; }

        /// <summary>
        /// Gets the actual color.
        /// </summary>
        /// <value>The actual color.</value>
        public OxyColor ActualColor
        {
            get
            {
                return Color.GetActualColor(defaultColor);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the curve.
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the area fill color.
        /// </summary>
        /// <value>The fill.</value>
        public OxyColor Fill { get; set; }

        /// <summary>
        /// Gets the actual fill color.
        /// </summary>
        /// <value>The actual fill.</value>
        public OxyColor ActualFill
        {
            get
            {
                return Fill.GetActualColor(defaultColor);
            }
        }

        public override void Render(IRenderContext renderContext)
        {
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext));
            }

            List<DataPoint[]> areas = Areas;
            int numberOfAreas = areas.Count;
            if (numberOfAreas == 0)
            {
                return;
            }
            if (areas.TrueForAll(a => !a.Any()))
            {
                return;
            }

            VerifyAxes();

            OxyRect clippingRect = GetClippingRect();
            renderContext.SetClip(clippingRect);

            // Transform all points to screen coordinates
            foreach (DataPoint[] area in areas)
            {
                int n0 = area.Length;
                IList<ScreenPoint> pts0 = new ScreenPoint[n0];
                TransformToScreenCoordinates(n0, pts0, area);

                renderContext.DrawClippedPolygon(clippingRect, pts0, 1, GetSelectableFillColor(ActualFill), ActualColor, StrokeThickness);
            }

            renderContext.ResetClip();
        }

        protected override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            DataPoint[] allPoints = Areas.SelectMany(a => a).ToArray();

            if (!allPoints.Any())
            {
                return;
            }

            MinX = allPoints.Min(p => p.X);
            MaxX = allPoints.Max(p => p.X);

            MinY = allPoints.Min(p => p.Y);
            MaxY = allPoints.Max(p => p.Y);

            XAxis.Include(MinX);
            XAxis.Include(MaxX);

            YAxis.Include(MinY);
            YAxis.Include(MaxY);
        }

        private void TransformToScreenCoordinates(int n0, IList<ScreenPoint> pts0, DataPoint[] actualPoints)
        {
            for (var i = 0; i < n0; i++)
            {
                pts0[i] = XAxis.Transform(actualPoints[i].X, actualPoints[i].Y, YAxis);
            }
        }
    }
}