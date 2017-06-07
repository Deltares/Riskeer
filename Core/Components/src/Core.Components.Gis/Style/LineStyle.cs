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
using System.Drawing.Drawing2D;

namespace Core.Components.Gis.Style
{
    /// <summary>
    /// This class represents styling of a line on a map.
    /// </summary>
    public class LineStyle
    {
        /// <summary>
        /// Gets or sets the line color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the line width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        public DashStyle DashStyle { get; set; }
    }
}