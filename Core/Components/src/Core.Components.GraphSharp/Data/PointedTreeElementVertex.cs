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
using System.Windows.Media;
using GraphSharp.Controls;

namespace Core.Components.GraphSharp.Data
{
    /// <summary>
    /// Class describing the vertex to show in the <see cref="GraphLayout"/>.
    /// </summary>
    public class PointedTreeElementVertex
    {
        /// <summary>
        /// Creates a new instance of <see cref="PointedTreeElementVertex"/>.
        /// </summary>
        /// <param name="content">The content of the vertex.</param>
        /// <param name="fillColor">The fill color of the vertex.</param>
        /// <param name="lineColor">The line color of the vertex.</param>
        /// <param name="lineWidth">The line width of the vertex.</param>
        /// <param name="type">The type of the vertex.</param>
        /// <param name="isSelectable">Indicator whether the vertex is selectable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/>
        /// <paramref name="fillColor"/> or <paramref name="lineColor"/> is <c>null</c>.</exception>
        public PointedTreeElementVertex(string content, Brush fillColor, Brush lineColor, int lineWidth, PointedTreeVertexType type, bool isSelectable)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (fillColor == null)
            {
                throw new ArgumentNullException(nameof(fillColor));
            }
            if (lineColor == null)
            {
                throw new ArgumentNullException(nameof(lineColor));
            }
            Content = content;
            FillColor = fillColor;
            LineColor = lineColor;
            LineWidth = lineWidth;
            Type = type;
            IsSelectable = isSelectable;
        }

        /// <summary>
        /// Gets the content of the vertex.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the fill color of the vertex.
        /// </summary>
        public Brush FillColor { get; }

        /// <summary>
        /// Gets the line color of the vertex.
        /// </summary>
        public Brush LineColor { get; }

        /// <summary>
        /// Gets the line widht of the vertex.
        /// </summary>
        public int LineWidth { get;}

        /// <summary>
        /// Gets the type of the vertex.
        /// </summary>
        public PointedTreeVertexType Type { get; }

        /// <summary>
        /// Gets whether the vertex is selectable.
        /// </summary>
        public bool IsSelectable { get; }

        /// <summary>
        /// Gets whether the vertex is selected.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}