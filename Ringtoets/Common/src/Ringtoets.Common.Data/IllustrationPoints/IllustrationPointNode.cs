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
using System.Linq;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.IllustrationPoints
{
    /// <summary>
    /// A node with attached illustration point data that is part of a tree. Multiple
    /// nodes form the tree structure describing how the illustration points are related.
    /// </summary>
    public class IllustrationPointNode : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointNode"/>.
        /// </summary>
        /// <param name="data">The illustration point data that is attached
        /// to this node.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/>
        /// is <c>null</c>.</exception>
        public IllustrationPointNode(IllustrationPointBase data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
            Children = Enumerable.Empty<IllustrationPointNode>();
        }

        /// <summary>
        /// Gets the attached illustration point data to this node.
        /// </summary>
        public IllustrationPointBase Data { get; private set; }

        /// <summary>
        /// Gets the attached child nodes of this node.
        /// </summary>
        public IEnumerable<IllustrationPointNode> Children { get; private set; }

        /// <summary>
        /// Sets the children to this node.
        /// </summary>
        /// <param name="children">The children that are attached to this node.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="children"/>
        /// does not contain 0 or 2 elements.</exception>
        public void SetChildren(IllustrationPointNode[] children)
        {
            if (children == null)
            {
                throw new ArgumentNullException(nameof(children));
            }

            int nrOfChildren = children.Length;
            if (nrOfChildren != 0 && nrOfChildren != 2)
            {
                throw new ArgumentException(Resources.IllustrationPointNode_SetChildren_Node_must_have_zero_or_two_child_nodes,
                                            nameof(children));
            }
            Children = children;
        }

        public object Clone()
        {
            var clone = (IllustrationPointNode) MemberwiseClone();

            clone.Data = (IllustrationPointBase) Data.Clone();
            clone.Children = Children.Select(c => (IllustrationPointNode) c.Clone());

            return clone;
        }
    }
}