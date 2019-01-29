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

using System.Drawing;

namespace Core.Components.PointedTree.Data
{
    /// <summary>
    /// This class represents styling of a <see cref="GraphNode"/>.
    /// </summary>
    public class GraphNodeStyle
    {
        /// <summary>
        /// Creates a new instance of <see cref="GraphNodeStyle"/>.
        /// </summary>
        /// <param name="shape">The shape of the node.</param>
        /// <param name="fillColor">The fill color of the node.</param>
        /// <param name="lineColor">The line color of the node.</param>
        /// <param name="lineWidth">The line width of the node.</param>
        public GraphNodeStyle(GraphNodeShape shape, Color fillColor, Color lineColor, int lineWidth)
        {
            Shape = shape;
            FillColor = fillColor;
            LineColor = lineColor;
            LineWidth = lineWidth;
        }

        /// <summary>
        /// Gets the shape of the node.
        /// </summary>
        public GraphNodeShape Shape { get; }

        /// <summary>
        /// Gets the fill color of the node.
        /// </summary>
        public Color FillColor { get; }

        /// <summary>
        /// Gets the line color of the node.
        /// </summary>
        public Color LineColor { get; }

        /// <summary>
        /// Gets the line width of the node.
        /// </summary>
        public int LineWidth { get; }
    }
}