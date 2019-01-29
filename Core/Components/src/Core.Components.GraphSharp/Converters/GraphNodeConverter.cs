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
using System.ComponentModel;
using System.Windows.Media;
using Core.Components.GraphSharp.Data;
using Core.Components.PointedTree.Data;

namespace Core.Components.GraphSharp.Converters
{
    /// <summary>
    /// Converter to change a <see cref="GraphNode"/> to <see cref="PointedTreeElementVertex"/>.
    /// </summary>
    public static class GraphNodeConverter
    {
        /// <summary>
        /// Converts a <see cref="GraphNode"/> to a <see cref="PointedTreeElementVertex"/>.
        /// </summary>
        /// <param name="graphNode">The graph node to convert.</param>
        /// <returns>The created <see cref="PointedTreeElementVertex"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graphNode"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="GraphNodeStyle.Shape"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="GraphNodeStyle.Shape"/>
        /// is a valid value, but unsupported.</exception>
        public static PointedTreeElementVertex Convert(GraphNode graphNode)
        {
            if (graphNode == null)
            {
                throw new ArgumentNullException(nameof(graphNode));
            }

            GraphNodeStyle style = graphNode.Style;

            return new PointedTreeElementVertex(
                graphNode.Content,
                ConvertColor(style.FillColor),
                ConvertColor(style.LineColor),
                style.LineWidth,
                ConvertType(style.Shape),
                graphNode.IsSelectable);
        }

        private static Color ConvertColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A,
                                  color.R,
                                  color.G,
                                  color.B);
        }

        /// <summary>
        /// Converts a <see cref="GraphNodeShape"/> to a <see cref="PointedTreeVertexType"/>.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The converted <see cref="PointedTreeVertexType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shape"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shape"/>
        /// is a valid value, but unsupported.</exception>
        private static PointedTreeVertexType ConvertType(GraphNodeShape shape)
        {
            if (!Enum.IsDefined(typeof(GraphNodeShape), shape))
            {
                throw new InvalidEnumArgumentException(nameof(shape),
                                                       (int) shape,
                                                       typeof(GraphNodeShape));
            }

            switch (shape)
            {
                case GraphNodeShape.Rectangle:
                    return PointedTreeVertexType.Rectangle;
                case GraphNodeShape.None:
                    return PointedTreeVertexType.None;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}