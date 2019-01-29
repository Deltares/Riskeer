// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Style
{
    /// <summary>
    /// This class represents styling of a polygon on a map.
    /// </summary>
    public class PolygonStyle
    {
        private readonly Range<int> strokeThicknessValidityRange = new Range<int>(0, 48);
        private int strokeThickness;

        /// <summary>
        /// Gets or sets the polygon fill color.
        /// </summary>
        public Color FillColor { get; set; }

        /// <summary>
        /// Gets or sets the polygon stroke color.
        /// </summary>
        public Color StrokeColor { get; set; }

        /// <summary>
        /// Gets or sets the polygon border stroke thickness.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not in range [0, 48].</exception>
        public int StrokeThickness
        {
            get
            {
                return strokeThickness;
            }
            set
            {
                if (strokeThicknessValidityRange.InRange(value))
                {
                    strokeThickness = value;
                }
                else
                {
                    string message = string.Format(Resources.StrokeThickness_Value_should_be_in_Range_0_, strokeThicknessValidityRange);
                    throw new ArgumentOutOfRangeException(nameof(value), message);
                }
            }
        }
    }
}