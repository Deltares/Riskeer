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

namespace Core.Components.OxyPlot.Forms
{
    /// <summary>
    /// Class for helping determine new extents for the plot view.
    /// </summary>
    internal class Extent
    {
        /// <summary>
        /// Creates a new instance of <see cref="Extent"/>.
        /// </summary>
        /// <param name="xMin">The smallest x value.</param>
        /// <param name="xMax">The largest x value.</param>
        /// <param name="yMin">The smallest y value.</param>
        /// <param name="yMax">The largest y value.</param>
        public Extent(double xMin, double xMax, double yMin, double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Extent"/> without values for
        /// the properties.
        /// </summary>
        public Extent()
        {
            XMin = double.NaN;
            XMax = double.NaN;
            YMin = double.NaN;
            YMax = double.NaN;
        }

        /// <summary>
        /// Gets the smallest x value.
        /// </summary>
        public double XMin { get; private set; }

        /// <summary>
        /// Gets the largest x value.
        /// </summary>
        public double XMax { get; private set; }

        /// <summary>
        /// Gets the smallest y value.
        /// </summary>
        public double YMin { get; private set; }

        /// <summary>
        /// Gets the largest y value.
        /// </summary>
        public double YMax { get; private set; }

        /// <summary>
        /// Gets a value indicating whether values were
        /// given to the properties.
        /// </summary>
        public bool IsNaN
        {
            get
            {
                return double.IsNaN(XMin)
                       || double.IsNaN(XMax)
                       || double.IsNaN(YMin)
                       || double.IsNaN(YMax);
            }
        }

        /// <summary>
        /// Adds padding to the extent and returns the new
        /// extent.
        /// </summary>
        /// <param name="padding">The padding to add.</param>
        /// <returns>A new extent with padding added
        /// to the property values.</returns>
        public Extent AddPadding(double padding)
        {
            double xPadding = (XMax - XMin) * padding;
            double yPadding = (YMax - YMin) * padding;

            return new Extent
            {
                XMin = XMin - xPadding,
                XMax = XMax + xPadding,
                YMin = YMin - yPadding,
                YMax = YMax + yPadding
            };
        }

        /// <summary>
        /// Assigns new values to the properties in such a way
        /// that the new values include both the <see cref="Extent"/>
        /// and the <paramref name="otherExtent"/>.
        /// </summary>
        /// <param name="otherExtent">The other extent to include in
        /// the <see cref="Extent"/>.</param>
        public void ExpandToInclude(Extent otherExtent)
        {
            if (otherExtent.IsNaN)
            {
                return;
            }

            XMin = double.IsNaN(XMin) ? otherExtent.XMin : Math.Min(XMin, otherExtent.XMin);
            XMax = double.IsNaN(XMax) ? otherExtent.XMax : Math.Max(XMax, otherExtent.XMax);
            YMin = double.IsNaN(YMin) ? otherExtent.YMin : Math.Min(YMin, otherExtent.YMin);
            YMax = double.IsNaN(YMax) ? otherExtent.YMax : Math.Max(YMax, otherExtent.YMax);
        }
    }
}