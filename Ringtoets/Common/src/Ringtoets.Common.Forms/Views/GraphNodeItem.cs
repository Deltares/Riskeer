// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Components.PointedTree.Data;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class represents a <see cref="GraphNode"/> <see cref="IllustrationPointNode"/> combination.
    /// </summary>
    public class GraphNodeItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="GraphNodeItem"/>.
        /// </summary>
        /// <param name="graphNode">The <see cref="GraphNode"/>.</param>
        /// <param name="illustrationPointNode">The <see cref="IllustrationPointNode"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GraphNodeItem(GraphNode graphNode, IllustrationPointNode illustrationPointNode)
        {
            if (graphNode == null)
            {
                throw new ArgumentNullException(nameof(graphNode));
            }
            if (illustrationPointNode == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointNode));
            }
            GraphNode = graphNode;
            IllustrationPointNode = illustrationPointNode;
        }

        /// <summary>
        /// Gets the graph node.
        /// </summary>
        public GraphNode GraphNode { get; }

        /// <summary>
        /// Gets the illustration node.
        /// </summary>
        public IllustrationPointNode IllustrationPointNode { get; }
    }
}