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
using System.Collections.Generic;
using System.Drawing;

namespace Core.Components.PointedTree.Data
{
    /// <summary>
    /// Class for data with the purpose of becoming visible in pointed tree components.
    /// </summary>
    public class GraphNode
    {
        private readonly GraphNode[] childNodes;

        /// <summary>
        /// Creates a new instance of <see cref="GraphNode"/> with default styling.
        /// </summary>
        /// <param name="title">The title of the node.</param>
        /// <param name="childNodes">The child nodes of the node.</param>
        /// <param name="isSelectable">Indicator whether the node is selectable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="title"/>
        /// or <paramref name="childNodes"/> is <c>null</c>.</exception>
        public GraphNode(string title, GraphNode[] childNodes, bool isSelectable)
            : this(title, childNodes, isSelectable, CreateDefaultGraphNodeStyle()) { }

        /// <summary>
        /// Creates a new instance of <see cref="GraphNode"/>.
        /// </summary>
        /// <param name="title">The title of the node.</param>
        /// <param name="childNodes">The child nodes of the node.</param>
        /// <param name="isSelectable">Indicator whether the node is selectable.</param>
        /// <param name="style">The style of the node.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="title"/>,
        /// <paramref name="childNodes"/> or <paramref name="style"/> is <c>null</c>.</exception>
        public GraphNode(string title, GraphNode[] childNodes, bool isSelectable, GraphNodeStyle style)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }
            if (childNodes == null)
            {
                throw new ArgumentNullException(nameof(childNodes));
            }
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Title = title;
            this.childNodes = childNodes;
            IsSelectable = isSelectable;
            Style = style;
        }

        /// <summary>
        /// Gets the title of the node.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the child nodes of the node.
        /// </summary>
        public IEnumerable<GraphNode> ChildNodes
        {
            get
            {
                return childNodes;
            }
        }

        /// <summary>
        /// Gets an indicator whether the node is selectable.
        /// </summary>
        public bool IsSelectable { get; }

        /// <summary>
        /// Gets the style of the node.
        /// </summary>
        public GraphNodeStyle Style { get; }

        private static GraphNodeStyle CreateDefaultGraphNodeStyle()
        {
            return new GraphNodeStyle(GraphNodeShape.Rectangle, Color.Gray, Color.Black, 2);
        }
    }
}