using System;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Controls.Swf.Properties;

namespace Core.Common.Controls.Swf.TreeViewControls
{
    /// <summary>
    /// This class takes a <see cref="ITreeNode"/> and maps the current collapsed/expanded
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
        /// does not have data on its <see cref="ITreeNode.Tag"/>.</exception>
        public TreeNodeExpandCollapseState(ITreeNode nodeToBeRecorded)
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
            childStates = nodeToBeRecorded.Nodes.Where(n => n.Nodes.Any()).ToDictionary(n => n.Tag, n => new TreeNodeExpandCollapseState(n));
        }

        /// <summary>
        /// Restores the specified target node and its children to the recorded state.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <exception cref="System.ArgumentException">When <paramref name="targetNode"/> data
        /// is not the same as was recorded.</exception>
        /// <exception cref="KeyNotFoundException">When <paramref name="targetNode"/> has
        /// different node-tree than recorded.</exception>
        public void Restore(ITreeNode targetNode)
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

            foreach (var treeNode in targetNode.Nodes.Where(n => n.Nodes.Any() && childStates.ContainsKey(n.Tag)))
            {
                childStates[treeNode.Tag].Restore(treeNode);
            }
        }
    }
}