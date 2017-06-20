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
using System.Collections.Generic;

namespace Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints
{
    /// <summary>
    /// A node that is part of a tree, that has illustration point data attached to it.
    /// Multiple nodes form the tree structure describing how the illustration points are 
    /// related.
    /// </summary>
    public class IllustrationPointTreeNode
    {
        /// <summary>
        /// Creates a new node with associated data.
        /// </summary>
        /// <param name="data">The data that is attached to this node.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/>
        /// is <c>null</c>.</exception>
        public IllustrationPointTreeNode(IIllustrationPoint data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            Data = data;
            Children = new List<IllustrationPointTreeNode>(2);
        }

        /// <summary>
        /// Gets the data attached to this node.
        /// </summary>
        public IIllustrationPoint Data { get; }

        /// <summary>
        /// Gets the child nodes of this node.
        /// </summary>
        public IList<IllustrationPointTreeNode> Children { get; }
    }
}