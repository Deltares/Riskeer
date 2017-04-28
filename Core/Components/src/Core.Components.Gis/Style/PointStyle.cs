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

using System.Drawing;

namespace Core.Components.Gis.Style
{
    /// <summary>
    /// This class represents styling of a point on a map.
    /// </summary>
    public class PointStyle
    {
        /// <summary>
        /// Creates a new instance of <see cref="PointStyle"/>.
        /// </summary>
        /// <param name="color">The color of the point.</param>
        /// <param name="size">The size of the point.</param>
        /// <param name="symbol">The symbol of the point.</param>
        public PointStyle(Color color, int size, PointSymbol symbol)
        {
            Color = color;
            Size = size;
            Symbol = symbol;
        }

        /// <summary>
        /// Gets the point color.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Gets the point size.
        /// </summary>
        public double Size { get; private set; }

        /// <summary>
        /// Gets the point symbol.
        /// </summary>
        public PointSymbol Symbol { get; private set; }
    }
}