// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView.Properties;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// This class takes a <see cref="TreeNode"/> and maps the current collapsed/expanded
    /// state of that node and its children. Then the state instance can be used to fully
    /// restore those states at a later time.
    /// </summary>
    public class TreeNodeExpandCollapseState
    {
        readonly bool wasExpanded;
        readonly object tag;
        readonly IDictionary<object, TreeNodeExpandCollapseState> childStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeExpandCollapseState"/> class, 
        /// recording the expanded / collapsed state of the given node and its children.
        /// </summary>
        /// <param name="nodeToBeRecorded">The node to be recorded.</param>
        /// <exception cref="System.ArgumentNullException">When <paramref name="nodeToBeRecorded"/> is null.</exception>
        /// <exception cref="System.ArgumentException">When <paramref name="nodeToBeRecorded"/> 
        /// does not have data on its <see cref="TreeNode.Tag"/>.</exception>
        public TreeNodeExpandCollapseState(TreeNode nodeToBeRecorded)
        {
            if (nodeToBeRecorded == null)
            {
                throw new ArgumentNullException("nodeToBeRecorded", Resources.TreeNodeExpandCollapseState_Node_cannot_be_null_for_record_to_work);
            }
            tag = nodeToBeRecorded.Tag;
            if (tag == null)
            {
                throw new ArgumentException(Resources.TreeNodeExpandCollapseState_Node_tag_cannot_be_null_for_record_to_work);
            }
            wasExpanded = nodeToBeRecorded.IsExpanded;
            childStates = nodeToBeRecorded.Nodes.OfType<TreeNode>().Where(n => n.Nodes.Count > 0).ToDictionary(n => n.Tag, n => new TreeNodeExpandCollapseState(n));
        }

        /// <summary>
        /// Restores the specified target node and its children to the recorded state.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <exception cref="System.ArgumentException">When <paramref name="targetNode"/> data
        /// is not the same as was recorded.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="targetNode"/> has
        /// different node-tree than recorded.</exception>
        public void Restore(TreeNode targetNode)
        {
            if (!targetNode.Tag.Equals(tag))
            {
                throw new ArgumentException(Resources.TreeNodeExpandCollapseState_Node_not_matching_tag_for_restore, "targetNode");
            }

            if (targetNode.IsExpanded)
            {
                if (!wasExpanded)
                {
                    targetNode.Collapse();
                }
            }
            else
            {
                if (wasExpanded)
                {
                    targetNode.Expand();
                }
            }

            foreach (var treeNode in targetNode.Nodes.OfType<TreeNode>().Where(n => n.Nodes.Count > 0 && childStates.ContainsKey(n.Tag)))
            {
                childStates[treeNode.Tag].Restore(treeNode);
            }
        }
    }
}