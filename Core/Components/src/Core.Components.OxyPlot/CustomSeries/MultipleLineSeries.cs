// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    /// Represents multiple line series that are each defined by a collection of points.
    /// </summary>
    public class MultipleLineSeries : XYAxisSeries
    {
        private readonly OxyColor defaultColor = OxyColors.Fuchsia;
        private OxyColor color;

        /// <summary>
        /// Creates a new instance of <see cref="MultipleLineSeries"/>.
        /// </summary>
        public MultipleLineSeries()
        {
            Lines = new List<IEnumerable<DataPoint>>();
            Color = OxyColors.Automatic;
        }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        public List<IEnumerable<DataPoint>> Lines { get; }

        /// <summary>
        /// Gets or sets the stroke thickness of the lines.
        /// </summary>
        public int StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the color of the lines.
        /// </summary>
        public OxyColor Color
        {
            get
            {
                return color.GetActualColor(defaultColor);
            }
            set
            {
                color = value;
            }
        }

        /// <summary>
        /// Gets or sets the style of the lines.
        /// </summary>
        public LineStyle LineStyle { get; set; }

        /// <summary>
        /// Gets or sets the definition of a dashed line style.
        /// Overrides the <see cref="LineStyle"/>.
        /// </summary>
        public IEnumerable<double> Dashes { get; set; }

        public override void Render(IRenderContext renderContext)
        {
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext));
            }

            if (!Lines.Any() || Lines.All(a => !a.Any()))
            {
                return;
            }

            VerifyAxes();

            OxyRect clippingRect = GetClippingRect();
            renderContext.SetClip(clippingRect);

            // Transform all points to screen coordinates
            foreach (IEnumerable<DataPoint> line in Lines)
            {
                int n0 = line.Count();
                var pts0 = new ScreenPoint[n0];
                TransformToScreenCoordinates(n0, pts0, line);

                renderContext.DrawLine(pts0, Color, StrokeThickness, Dashes?.ToArray() ?? LineStyle.GetDashArray());
            }

            renderContext.ResetClip();
        }

        protected override void UpdateMaxMin()
        {
            base.UpdateMaxMin();

            DataPoint[] allPoints = Lines.SelectMany(l => l).ToArray();

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

        private void TransformToScreenCoordinates(int n0, ScreenPoint[] pts0, IEnumerable<DataPoint> actualPoints)
        {
            for (var i = 0; i < n0; i++)
            {
                pts0[i] = XAxis.Transform(actualPoints.ElementAt(i).X, actualPoints.ElementAt(i).Y, YAxis);
            }
        }
    }
}