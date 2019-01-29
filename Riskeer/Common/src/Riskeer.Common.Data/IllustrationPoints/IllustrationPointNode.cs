// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.Properties;

namespace Riskeer.Common.Data.IllustrationPoints
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
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="children"/> does not contain 0 or 2 elements;</item>
        /// <item>the children have duplicate names;</item>
        /// <item>they have different stochasts than the node itself.</item>
        /// </list>
        /// </exception>
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

            var faultTreeData = Data as FaultTreeIllustrationPoint;
            if (faultTreeData != null)
            {
                ValidateChildNames(children);
                ValidateChildStochasts(faultTreeData, children);
            }

            Children = children;
        }

        public object Clone()
        {
            var clone = (IllustrationPointNode) MemberwiseClone();

            clone.Data = (IllustrationPointBase) Data.Clone();
            clone.Children = Children.Select(c => (IllustrationPointNode) c.Clone()).ToArray();

            return clone;
        }

        /// <summary>
        /// Validates a <see cref="FaultTreeIllustrationPoint"/> by checking for duplicate names in child nodes.
        /// </summary>
        /// <param name="children">The collection of <see cref="IllustrationPointNode"/> to be validated.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="children"/> contains child nodes with
        /// duplicate names.</exception>
        private static void ValidateChildNames(IEnumerable<IllustrationPointNode> children)
        {
            if (children.HasDuplicates(c => c.Data.Name))
            {
                throw new ArgumentException(string.Format(Resources.IllustrationPointNode_ValidateChildNames_Child_names_not_unique));
            }
        }

        /// <summary>
        /// Validates a <see cref="FaultTreeIllustrationPoint"/> by comparing the stochasts in child nodes with its
        /// own stochasts.
        /// </summary>
        /// <param name="data">The <see cref="FaultTreeIllustrationPoint"/> to be validated.</param>
        /// <param name="children">The collection of <see cref="IllustrationPointNode"/> to be validated.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="children"/> contains stochasts that 
        /// are not in <paramref name="data"/>'s stochasts.</exception>
        private static void ValidateChildStochasts(FaultTreeIllustrationPoint data, IEnumerable<IllustrationPointNode> children)
        {
            List<string> stochastNames = children.SelectMany(c => c.GetStochastNames()).ToList();

            if (data.GetStochastNames().Intersect(stochastNames).Count() != stochastNames.Distinct().Count())
            {
                throw new ArgumentException(string.Format(Resources.Child_stochasts_not_same_as_parent_stochasts));
            }
        }
    }
}