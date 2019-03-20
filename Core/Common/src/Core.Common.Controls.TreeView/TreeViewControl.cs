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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Core.Common.Base;
using Core.Common.Controls.Forms;
using Core.Common.Controls.TreeView.Properties;
using Core.Common.Util.Events;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// A <see cref="UserControl"/> that encapsulates a <see cref="DoubleBufferedTreeView"/>
    /// in such a way that <see cref="TreeNodeInfo"/> objects can be registered for configuring
    /// the way tree nodes are drawn and can be interacted with. A general implementation is
    /// provided when it comes to:
    /// <list type="bullet">
    /// <item>
    /// <description>drawing tree nodes (including images, checkboxes and forecolors);</description>
    /// </item>
    /// <item>
    /// <description>selecting tree nodes;</description>
    /// </item>
    /// <item>
    /// <description>expanding/collapsing tree nodes;</description>
    /// </item>
    /// <item>
    /// <description>renaming tree nodes (including feedback popups);</description>
    /// </item>
    /// <item>
    /// <description>removing tree nodes (including feedback popups);</description>
    /// </item>
    /// <item>
    /// <description>dragging and dropping tree nodes (including visual feedback);</description>
    /// </item>
    /// <item>
    /// <description>opening tree node context menus;</description>
    /// </item>
    /// <item>
    /// <description>short keys for tree node interaction via the keyboard.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// The current <see cref="TreeViewControl"/> implementation assumes that the data hierarchy,
    /// defined by the combination of registered <see cref="TreeNodeInfo"/> objects and the provided
    /// <see cref="Data"/>, only contains uniquely identifiable data objects. Additionally, only one
    /// <see cref="TreeNodeInfo"/> object can be registered per <see cref="TreeNodeInfo.TagType"/>.
    /// </remarks>
    public partial class TreeViewControl : UserControl
    {
        private const int maximumTextLength = 259;
        private const string stateImageLocationString = "StateImage";
        private const int uncheckedCheckBoxStateImageIndex = 0;
        private const int checkedCheckBoxStateImageIndex = 1;
        private const int mixedCheckBoxStateImageIndex = 2;
        private const int updateTimerInterval = 10;

        private readonly DragDropHandler dragDropHandler = new DragDropHandler();
        private readonly Dictionary<Type, TreeNodeInfo> tagTypeTreeNodeInfoLookup = new Dictionary<Type, TreeNodeInfo>();
        private readonly Dictionary<TreeNode, TreeNodeObserver> treeNodeObserverLookup = new Dictionary<TreeNode, TreeNodeObserver>();

        private object data;
        private Timer updateTimer;

        public event EventHandler DataDoubleClick;
        public event EventHandler SelectedDataChanged;
        public event EventHandler<EventArgs<object>> DataDeleted; // TODO; Way to explicit!

        /// <summary>
        /// Creates a new instance of <see cref="TreeViewControl"/>.
        /// </summary>
        public TreeViewControl()
        {
            InitializeComponent();

            treeView.ImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };

            treeView.StateImageList = new ImageList();
            treeView.StateImageList.Images.Add(CreateCheckBoxGlyph(CheckBoxState.UncheckedNormal));
            treeView.StateImageList.Images.Add(CreateCheckBoxGlyph(CheckBoxState.CheckedNormal));
            treeView.StateImageList.Images.Add(CreateCheckBoxGlyph(CheckBoxState.MixedNormal));

            treeView.DrawMode = TreeViewDrawMode.Normal;
            treeView.AllowDrop = true;
            treeView.LabelEdit = true;
            treeView.HideSelection = false;

            treeView.BeforeLabelEdit += TreeViewBeforeLabelEdit;
            treeView.AfterLabelEdit += TreeViewAfterLabelEdit;
            treeView.AfterCheck += TreeViewAfterCheck;
            treeView.KeyDown += TreeViewKeyDown;
            treeView.MouseClick += TreeViewMouseClick;
            treeView.DoubleClick += TreeViewDoubleClick;
            treeView.DragDrop += TreeViewDragDrop;
            treeView.DragOver += TreeViewDragOver;
            treeView.ItemDrag += TreeViewItemDrag;
            treeView.DragLeave += TreeViewDragLeave;
            treeView.AfterSelect += TreeViewAfterSelect;

            InitializeUpdateTimer();
        }

        /// <summary>
        /// Gets or sets the data to show in the <see cref="TreeViewControl"/>.
        /// </summary>
        /// <remarks>
        /// Ensure all necessary <see cref="TreeNodeInfo"/> objects are registered in order to prevent
        /// an <see cref="InvalidOperationException"/>. Take notice of the fact that these kind of
        /// exceptions might not directly occur after setting the data.
        /// </remarks>
        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                RemoveAllNodes();

                data = value;

                if (data == null)
                {
                    // Propagate a selection change here manually as setting the treeview selection to null (by clearing all nodes)
                    // does not fire the AfterSelect event
                    OnSelectedDataChanged();

                    return;
                }

                AddRootNode();

                treeView.SelectedNode = treeView.Nodes.Count > 0 ? treeView.Nodes[0] : null;
            }
        }

        /// <summary>
        /// Gets the data of the selected tree node in the <see cref="TreeViewControl"/>.
        /// </summary>
        /// <remarks><c>null</c> is returned when no tree node is selected.</remarks>
        public object SelectedData
        {
            get
            {
                return treeView.SelectedNode?.Tag;
            }
        }

        /// <summary>
        /// This method registers a <see cref="TreeNodeInfo"/> object.
        /// </summary>
        /// <param name="treeNodeInfo">The <see cref="TreeNodeInfo"/> to register.</param>
        /// <remarks>Only one <see cref="TreeNodeInfo"/> object can be registered per <see cref="TreeNodeInfo.TagType"/>.</remarks>
        public void RegisterTreeNodeInfo(TreeNodeInfo treeNodeInfo)
        {
            tagTypeTreeNodeInfoLookup[treeNodeInfo.TagType] = treeNodeInfo;
        }

        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// can be renamed.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns>Whether or not the tree node can be renamed or <c>false</c> when no corresponding tree node is found.</returns>
        public bool CanRenameNodeForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            return treeNode != null && CanRename(treeNode);
        }

        /// <summary>
        /// This method tries to start a rename action for the tree node corresponding to the
        /// <paramref name="dataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <remarks>
        /// When a tree node is found that cannot be renamed, a popup is shown for notifying the end user.
        /// The renaming logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        public void TryRenameNodeForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            if (treeNode != null)
            {
                Rename(treeNode);
            }
        }

        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// can be removed.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns>Whether or not the tree node can be removed or <c>false</c> when no corresponding tree node is found.</returns>
        public bool CanRemoveNodeForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            return treeNode != null && CanRemove(treeNode);
        }

        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// has children which can be removed.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns><c>true</c> if the tree node has a child node which can be removed or <c>false</c> otherwise.</returns>
        public bool CanRemoveChildNodesOfData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            return treeNode != null && CanRemoveChildNodes(treeNode);
        }

        /// <summary>
        /// This method tries to remove the tree node corresponding to the <paramref name="dataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <remarks>
        /// When a tree node is found that can be removed, a popup is shown for confirmation by the end user.
        /// When a tree node is found that cannot be removed, a popup is shown for notifying the end user.
        /// The removing logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        public void TryRemoveNodeForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            if (treeNode != null)
            {
                TryRemoveNode(treeNode);
            }
        }

        public void TryRemoveChildNodesOfData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            if (treeNode != null)
            {
                TryRemoveChildNodes(treeNode);
            }
        }

        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// can be collapsed/expanded.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns>Whether or not the tree node can be collapsed/expanded or <c>false</c> when no corresponding tree node is found.</returns>
        public bool CanExpandOrCollapseForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            return treeNode != null && treeNode.Nodes.Count > 0;
        }

        /// <summary>
        /// This method tries to collapse all nodes of the tree node corresponding to the <paramref name="dataObject"/>
        /// (child nodes are taken into account recursively).
        /// </summary>
        /// <remarks>
        /// The collapsing logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        public void TryCollapseAllNodesForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            if (treeNode != null)
            {
                CollapseAll(treeNode);
            }
        }

        /// <summary>
        /// This method tries to expand all nodes of the tree node corresponding to the <paramref name="dataObject"/>
        /// (child nodes are taken into account recursively).
        /// </summary>
        /// <remarks>
        /// The expanding logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        public void TryExpandAllNodesForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            if (treeNode != null)
            {
                ExpandAll(treeNode);
            }
        }

        /// <summary>
        /// This method tries to select the tree node corresponding to the <paramref name="dataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <remarks>
        /// The tree node selection is set to <c>null</c> when no corresponding tree node is found.
        /// </remarks>
        public void TrySelectNodeForData(object dataObject)
        {
            treeView.SelectedNode = GetNodeByTag(dataObject);
        }

        /// <summary>
        /// This method tries to return the path of the tree node corresponding to the <paramref name="dataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns>The path of the tree node or <c>null</c> when no corresponding tree node is found.</returns>
        public string TryGetPathForData(object dataObject)
        {
            TreeNode treeNode = GetNodeByTag(dataObject);

            return treeNode?.FullPath;
        }

        protected override void Dispose(bool disposing)
        {
            updateTimer.Stop();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool CanRename(TreeNode treeNode)
        {
            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(treeNode.Tag);
            object parentTag = GetParentTag(treeNode);

            return treeNodeInfo.CanRename != null && treeNodeInfo.CanRename(treeNode.Tag, parentTag);
        }

        private void Rename(TreeNode treeNode)
        {
            if (!CanRename(treeNode))
            {
                MessageBox.Show(Resources.TreeViewControl_The_selected_item_cannot_be_renamed, BaseResources.Confirm, MessageBoxButtons.OK);
                return;
            }

            treeNode.BeginEdit();
        }

        private bool CanRemove(TreeNode treeNode)
        {
            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(treeNode.Tag);
            object parentTag = GetParentTag(treeNode);

            return treeNodeInfo.CanRemove != null && treeNodeInfo.CanRemove(treeNode.Tag, parentTag);
        }

        private bool CanRemoveChildNodes(TreeNode treeNode)
        {
            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(treeNode.Tag);
            if (treeNodeInfo.ChildNodeObjects != null)
            {
                return treeNodeInfo.ChildNodeObjects(treeNode.Tag).Any(childData => CanRemove(GetNodeByTag(childData)));
            }

            return false;
        }

        private void TryRemoveNode(TreeNode treeNode)
        {
            if (!CanRemove(treeNode))
            {
                MessageBox.Show(Resources.TreeViewControl_The_selected_item_cannot_be_removed, BaseResources.Confirm, MessageBoxButtons.OK);
                return;
            }

            string message = Resources.TreeViewControl_Are_you_sure_you_want_to_remove_the_selected_item;
            if (MessageBox.Show(message, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            RemoveNode(treeNode);
        }

        private void TryRemoveChildNodes(TreeNode treeNode)
        {
            string message = Resources.TreeViewControl_Are_you_sure_you_want_to_remove_children_of_the_selected_item;
            if (MessageBox.Show(message, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            RemoveChildNodes(treeNode);
        }

        private void RemoveChildNodes(TreeNode treeNode)
        {
            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(treeNode.Tag);

            if (treeNodeInfo.ChildNodeObjects != null)
            {
                foreach (object childNodeObject in treeNodeInfo.ChildNodeObjects(treeNode.Tag))
                {
                    TreeNode childNode = GetNodeByTag(childNodeObject);

                    if (CanRemove(childNode))
                    {
                        RemoveNode(childNode);
                    }
                }
            }
        }

        private void RemoveNode(TreeNode treeNode)
        {
            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(treeNode.Tag);

            if (treeNodeInfo.OnNodeRemoved != null)
            {
                object parentTag = GetParentTag(treeNode);

                treeNodeInfo.OnNodeRemoved(treeNode.Tag, parentTag);
            }

            OnNodeDataDeleted(treeNode);
        }

        private void CollapseAll(TreeNode treeNode)
        {
            treeView.BeginUpdate();

            CollapseNodeAndChildNodes(treeNode);

            treeView.EndUpdate();
        }

        private static void CollapseNodeAndChildNodes(TreeNode treeNode)
        {
            treeNode.Collapse();

            foreach (TreeNode childNode in treeNode.Nodes)
            {
                CollapseNodeAndChildNodes(childNode);
            }
        }

        private void ExpandAll(TreeNode treeNode)
        {
            treeView.BeginUpdate();

            ExpandNodeAndChildNodes(treeNode);

            treeView.EndUpdate();
        }

        private static void ExpandNodeAndChildNodes(TreeNode treeNode)
        {
            treeNode.Expand();

            foreach (TreeNode childNode in treeNode.Nodes)
            {
                ExpandNodeAndChildNodes(childNode);
            }
        }

        /// <summary>
        /// This method searches all nodes in the tree view for a node with a matching tag.
        /// </summary>
        /// <param name="nodeData">The node data to search the corresponding <see cref="TreeNode"/> for.</param>
        /// <returns>The <see cref="TreeNode"/> corresponding the provided node data or <c>null</c> if not found.</returns>
        private TreeNode GetNodeByTag(object nodeData)
        {
            return treeView.Nodes.Count > 0 ? GetNodeByTag(treeView.Nodes[0], nodeData) : null;
        }

        private static TreeNode GetNodeByTag(TreeNode rootNode, object tag)
        {
            if (Equals(rootNode.Tag, tag))
            {
                return rootNode;
            }

            return rootNode.Nodes
                           .Cast<TreeNode>()
                           .Select(n => GetNodeByTag(n, tag))
                           .FirstOrDefault(node => node != null);
        }

        private void AddRootNode()
        {
            var rootNode = new TreeNode
            {
                Tag = data
            };

            UpdateNode(rootNode);

            if (rootNode.Nodes.Count > 0)
            {
                rootNode.Expand();
            }

            treeView.Nodes.Add(rootNode);

            treeNodeObserverLookup.Add(rootNode, new TreeNodeObserver(rootNode, this));
        }

        private TreeNode CreateTreeNode(TreeNode parentNode, object nodeData)
        {
            var newNode = new TreeNode
            {
                Tag = nodeData
            };

            if (treeView.CheckBoxes)
            {
                newNode.Checked = parentNode.Checked;
            }

            UpdateNode(newNode);

            treeNodeObserverLookup.Add(newNode, new TreeNodeObserver(newNode, this));

            return newNode;
        }

        /// <summary>
        /// This method updates the provided <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="TreeNode"/> to update.</param>
        /// <exception cref="InvalidOperationException">Thrown when no corresponding <see cref="TreeNodeInfo"/> can be found for the provided <see cref="TreeNode"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when the <see cref="TreeNodeCheckedState"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="TreeNodeCheckedState"/>
        /// is a valid value, but unsupported.</exception>
        private void UpdateNode(TreeNode treeNode)
        {
            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(treeNode.Tag);
            if (treeNodeInfo == null)
            {
                throw new InvalidOperationException("No tree node info registered");
            }

            StartUpdateTimer();

            // First of all refresh the child nodes as the other logic below might depend on the presence of child nodes
            RefreshChildNodes(treeNode, treeNodeInfo);

            if (treeNodeInfo.Text != null)
            {
                string text = treeNodeInfo.Text(treeNode.Tag);

                // Having very big strings causes rendering problems in the tree view
                treeNode.Text = text.Length > maximumTextLength
                                    ? text.Substring(0, maximumTextLength)
                                    : text;
            }
            else
            {
                treeNode.Text = "";
            }

            treeNode.ForeColor = treeNodeInfo.ForeColor?.Invoke(treeNode.Tag) ?? Color.FromKnownColor(KnownColor.ControlText);
            SetTreeNodeImageKey(treeNode, treeNodeInfo);

            if (treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(treeNode.Tag) &&
                treeNodeInfo.CheckedState != null)
            {
                TreeNodeCheckedState treeNodeCheckedState = treeNodeInfo.CheckedState(treeNode.Tag);
                if (!IsCheckedStateUpToDate(treeNode, treeNodeCheckedState))
                {
                    treeView.AfterCheck -= TreeViewAfterCheck;
                    treeNode.Checked = !treeNode.Checked;
                    treeView.AfterCheck += TreeViewAfterCheck;
                }

                SetTreeNodeStateImageIndex(treeNode, treeNodeCheckedState);
            }

            if (treeNodeInfo.ExpandOnCreate != null && treeNodeInfo.ExpandOnCreate(treeNode.Tag))
            {
                treeNode.Expand();
            }
        }

        /// <summary>
        /// Sets the state image index of the tree node.
        /// </summary>
        /// <param name="treeNode">The tree node to set the state image index for.</param>
        /// <param name="treeNodeCheckedState">The checked state to determine the state index from.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when the <paramref name="treeNodeCheckedState"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="treeNodeCheckedState"/>
        /// is a valid value, but unsupported.</exception>
        private static void SetTreeNodeStateImageIndex(TreeNode treeNode, TreeNodeCheckedState treeNodeCheckedState)
        {
            if (!Enum.IsDefined(typeof(TreeNodeCheckedState), treeNodeCheckedState))
            {
                throw new InvalidEnumArgumentException(nameof(treeNodeCheckedState),
                                                       (int) treeNodeCheckedState,
                                                       typeof(TreeNodeCheckedState));
            }

            switch (treeNodeCheckedState)
            {
                case TreeNodeCheckedState.Checked:
                    treeNode.StateImageIndex = checkedCheckBoxStateImageIndex;
                    break;
                case TreeNodeCheckedState.Unchecked:
                    treeNode.StateImageIndex = uncheckedCheckBoxStateImageIndex;
                    break;
                case TreeNodeCheckedState.Mixed:
                    treeNode.StateImageIndex = mixedCheckBoxStateImageIndex;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static bool IsCheckedStateUpToDate(TreeNode treeNode, TreeNodeCheckedState treeNodeCheckedState)
        {
            return treeNode.Checked && (treeNodeCheckedState == TreeNodeCheckedState.Checked || treeNodeCheckedState == TreeNodeCheckedState.Mixed)
                   || !treeNode.Checked && treeNodeCheckedState == TreeNodeCheckedState.Unchecked;
        }

        private void RefreshChildNodes(TreeNode treeNode, TreeNodeInfo treeNodeInfo)
        {
            var newlyAddedChildNodes = new List<TreeNode>();
            object[] expectedChildNodeObjects = treeNodeInfo.ChildNodeObjects != null
                                                    ? treeNodeInfo.ChildNodeObjects(treeNode.Tag)
                                                    : new object[0];
            int expectedChildNodeCount = expectedChildNodeObjects.Length;

            for (var childNodeIndex = 0; childNodeIndex < expectedChildNodeCount; childNodeIndex++)
            {
                RemovePossiblyOutdatedChildNodeAtIndex(treeNode, childNodeIndex, expectedChildNodeObjects);

                object expectedChildNodeObject = expectedChildNodeObjects[childNodeIndex];

                if (IsExpectedChildNodePresentAtIndex(treeNode, childNodeIndex, expectedChildNodeObject))
                {
                    continue;
                }

                int currentChildNodeIndex = Array.FindIndex(treeNode.Nodes.Cast<TreeNode>().ToArray(), c => c.Tag == expectedChildNodeObject);
                if (currentChildNodeIndex > -1)
                {
                    MoveExistingChildNodeToIndex(treeNode, currentChildNodeIndex, childNodeIndex);
                }
                else
                {
                    InsertNewChildNodeAtIndex(treeNode, childNodeIndex, expectedChildNodeObject, newlyAddedChildNodes);
                }
            }

            RemovePossiblyOutdatedChildNodesAtEnd(treeNode, expectedChildNodeCount);

            SelectLastNewChildNode(newlyAddedChildNodes);
        }

        private void RemovePossiblyOutdatedChildNodeAtIndex(TreeNode parentNode, int childNodeIndex, IEnumerable<object> expectedChildNodeObjects)
        {
            if (parentNode.Nodes.Count > childNodeIndex)
            {
                TreeNode possiblyOutdatedChildNode = parentNode.Nodes[childNodeIndex];

                if (!expectedChildNodeObjects.Contains(possiblyOutdatedChildNode.Tag))
                {
                    parentNode.Nodes.Remove(possiblyOutdatedChildNode);
                    RemoveTreeNodeFromLookupRecursively(possiblyOutdatedChildNode);
                }
            }
        }

        private static bool IsExpectedChildNodePresentAtIndex(TreeNode parentNode, int childNodeIndex, object expectedChildNodeData)
        {
            return parentNode.Nodes.Count > childNodeIndex && expectedChildNodeData.Equals(parentNode.Nodes[childNodeIndex].Tag);
        }

        private static void MoveExistingChildNodeToIndex(TreeNode parentNode, int currentChildNodeIndex, int newChildNodeIndex)
        {
            TreeNode existingTreeNode = parentNode.Nodes[currentChildNodeIndex];

            parentNode.Nodes.RemoveAt(currentChildNodeIndex);
            parentNode.Nodes.Insert(newChildNodeIndex, existingTreeNode);
        }

        private void InsertNewChildNodeAtIndex(TreeNode parentNode, int childNodeIndex, object expectedChildNodeData, List<TreeNode> newlyAddedTreeNodes)
        {
            TreeNode newTreeNode = CreateTreeNode(parentNode, expectedChildNodeData);

            parentNode.Nodes.Insert(childNodeIndex, newTreeNode);
            newlyAddedTreeNodes.Add(newTreeNode);
        }

        private static void RemovePossiblyOutdatedChildNodesAtEnd(TreeNode parentNode, int expectedChildNodeLength)
        {
            for (int i = parentNode.Nodes.Count - 1; i >= expectedChildNodeLength; i--)
            {
                parentNode.Nodes.RemoveAt(i);
            }
        }

        private void SelectLastNewChildNode(IEnumerable<TreeNode> newChildNodes)
        {
            TreeNode lastAddedChildNodeToSetSelectionTo = newChildNodes.LastOrDefault(node =>
            {
                object dataObject = node.Tag;
                TreeNodeInfo info = TryGetTreeNodeInfoForData(dataObject);

                return info.EnsureVisibleOnCreate != null && info.EnsureVisibleOnCreate(dataObject, node.Parent.Tag);
            });

            if (lastAddedChildNodeToSetSelectionTo != null)
            {
                lastAddedChildNodeToSetSelectionTo.EnsureVisible();
                treeView.SelectedNode = lastAddedChildNodeToSetSelectionTo;
            }
        }

        private void RemoveAllNodes()
        {
            foreach (TreeNode treeNode in treeNodeObserverLookup.Keys)
            {
                treeNodeObserverLookup[treeNode].Dispose();
            }

            treeNodeObserverLookup.Clear();
            treeView.Nodes.Clear();
        }

        private void RemoveTreeNodeFromLookupRecursively(TreeNode treeNode)
        {
            treeNodeObserverLookup[treeNode].Dispose();
            treeNodeObserverLookup.Remove(treeNode);

            foreach (TreeNode childNode in treeNode.Nodes)
            {
                RemoveTreeNodeFromLookupRecursively(childNode);
            }
        }

        /// <summary>
        /// This method tries to return a <see cref="TreeNodeInfo"/> object corresponding to the provided data.
        /// </summary>
        /// <param name="item">The data to find the corresponding <see cref="TreeNodeInfo"/> for.</param>
        /// <returns>The <see cref="TreeNodeInfo"/> for the provided data or <c>null</c> if no corresponding <see cref="TreeNodeInfo"/> was found.</returns>
        private TreeNodeInfo TryGetTreeNodeInfoForData(object item)
        {
            if (item == null)
            {
                return null;
            }

            TreeNodeInfo treeNodeInfo;

            // Try to find an exact match
            tagTypeTreeNodeInfoLookup.TryGetValue(item.GetType(), out treeNodeInfo);

            // Try to match based on class hierarchy
            return treeNodeInfo ?? tagTypeTreeNodeInfoLookup.FirstOrDefault(kvp => kvp.Key.IsInstanceOfType(item)).Value;
        }

        private static object GetParentTag(TreeNode treeNode)
        {
            return treeNode.Parent?.Tag;
        }

        private static Image CreateCheckBoxGlyph(CheckBoxState state)
        {
            var result = new Bitmap(16, 16);

            using (Graphics g = Graphics.FromImage(result))
            {
                Size glyphSize = CheckBoxRenderer.GetGlyphSize(g, state);

                CheckBoxRenderer.DrawCheckBox(g, new Point((result.Width - glyphSize.Width) / 2, (result.Height - glyphSize.Height) / 2), state);
            }

            return result;
        }

        private void SetTreeNodeImageKey(TreeNode treeNode, TreeNodeInfo treeNodeInfo)
        {
            if (treeNodeInfo.Image != null)
            {
                Image image = treeNodeInfo.Image(treeNode.Tag);
                ImageList.ImageCollection imageCollection = treeView.ImageList.Images;
                string imageKey = GetImageHash(image);
                if (imageCollection.ContainsKey(imageKey))
                {
                    treeNode.ImageKey = imageKey;
                    treeNode.SelectedImageKey = imageKey;
                }
                else
                {
                    treeNode.ImageKey = imageKey;
                    treeNode.SelectedImageKey = imageKey;
                    imageCollection.Add(imageKey, image);
                }
            }
        }

        private static string GetImageHash(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, image.RawFormat);
            var md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(stream.ToArray());
            return Encoding.UTF8.GetString(hash);
        }

        private void OnNodeDataDeleted(TreeNode node)
        {
            DataDeleted?.Invoke(this, new EventArgs<object>(node.Tag));
        }

        #region Nested types

        private class TreeNodeObserver : IDisposable, IObserver
        {
            private readonly TreeNode treeNode;
            private readonly TreeViewControl treeViewControl;

            public TreeNodeObserver(TreeNode treeNode, TreeViewControl treeViewControl)
            {
                this.treeNode = treeNode;
                this.treeViewControl = treeViewControl;

                var observable = treeNode.Tag as IObservable;
                observable?.Attach(this);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void UpdateObserver()
            {
                treeViewControl.UpdateNode(treeNode);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    var observable = treeNode.Tag as IObservable;
                    observable?.Detach(this);
                }
            }
        }

        #endregion

        #region TreeView event handling

        private void TreeViewBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!CanRename(e.Node))
            {
                e.CancelEdit = true;
            }
        }

        private void TreeViewAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Check label for null as this indicates the node edit was canceled
            if (e.Label == null)
            {
                return;
            }

            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(e.Node.Tag);
            treeNodeInfo.OnNodeRenamed?.Invoke(e.Node.Tag, e.Label);
        }

        private void TreeViewAfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(e.Node.Tag);
            if (treeNodeInfo.OnNodeChecked != null)
            {
                object parentTag = GetParentTag(e.Node);

                treeNodeInfo.OnNodeChecked(e.Node.Tag, parentTag);
            }
        }

        private void TreeViewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            if (selectedNode == null)
            {
                return;
            }

            switch (keyEventArgs.KeyData)
            {
                case Keys.F5: // Refresh the selected tree node in the tree view
                {
                    if (treeView.SelectedNode != null)
                    {
                        UpdateNode(treeView.SelectedNode);
                    }

                    break;
                }
                case Keys.F2: // Start editing the label of the selected tree node
                {
                    Rename(selectedNode);
                    break;
                }
                case Keys.Apps: // If implemented, show the context menu of the selected tree node
                {
                    TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(selectedNode.Tag);
                    object parentTag = GetParentTag(selectedNode);

                    // Update the context menu (relevant in case of keyboard navigation in the tree view)
                    UpdateContextMenuStrip(selectedNode, treeNodeInfo, parentTag);

                    if (treeView.ContextMenu != null && selectedNode.ContextMenuStrip != null)
                    {
                        Point location = selectedNode.Bounds.Location;
                        location.Offset(0, selectedNode.Bounds.Height);

                        selectedNode.ContextMenuStrip.Show(location);
                    }

                    break;
                }
                case Keys.Enter: // Perform the same action as on double click
                {
                    OnDataDoubleClick();

                    break;
                }
                case Keys.Delete: // Try to delete the selected tree node
                {
                    TryRemoveNode(selectedNode);

                    break;
                }
                case Keys.Space: // If applicable, change the checked state of the selected tree node
                {
                    TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(selectedNode.Tag);
                    if (treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(selectedNode.Tag))
                    {
                        selectedNode.Checked = !selectedNode.Checked;
                    }

                    break;
                }
            }

            if (keyEventArgs.KeyData == (Keys.Control | Keys.Shift | Keys.Right)) // Expand all child nodes of the selected tree node
            {
                ExpandAll(selectedNode);
                selectedNode.EnsureVisible();
            }

            if (keyEventArgs.KeyData == (Keys.Control | Keys.Shift | Keys.Left)) // Collapse all child nodes of the selected tree node
            {
                CollapseAll(selectedNode);
                selectedNode.EnsureVisible();
            }
        }

        private void TreeViewMouseClick(object sender, MouseEventArgs e)
        {
            var point = new Point(e.X, e.Y);
            TreeNode clickedNode = treeView.GetNodeAt(point);
            if (clickedNode == null)
            {
                return;
            }

            TreeNodeInfo treeNodeInfo = TryGetTreeNodeInfoForData(clickedNode.Tag);

            if (e.Button.HasFlag(MouseButtons.Right))
            {
                treeView.SelectedNode = clickedNode;

                object parentTag = GetParentTag(clickedNode);

                // Update the context menu
                UpdateContextMenuStrip(clickedNode, treeNodeInfo, parentTag);

                return;
            }

            bool isOnCheckBox = IsOnCheckBox(point);
            if (treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(clickedNode.Tag) && isOnCheckBox)
            {
                clickedNode.Checked = !clickedNode.Checked;
            }
        }

        private void UpdateContextMenuStrip(TreeNode node, TreeNodeInfo treeNodeInfo, object parentTag)
        {
            node.ContextMenuStrip?.Dispose();
            node.ContextMenuStrip = treeNodeInfo.ContextMenuStrip?.Invoke(node.Tag, parentTag, this);
        }

        private bool IsOnCheckBox(Point point)
        {
            return treeView.HitTest(point).Location.ToString() == stateImageLocationString;
        }

        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            OnDataDoubleClick();
        }

        private void OnDataDoubleClick()
        {
            DataDoubleClick?.Invoke(treeView.SelectedNode, EventArgs.Empty);
        }

        private void TreeViewDragDrop(object sender, DragEventArgs e)
        {
            dragDropHandler.HandleDragDrop(this, treeView, e, TryGetTreeNodeInfoForData);
        }

        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            dragDropHandler.HandleDragOver(treeView, e, TryGetTreeNodeInfoForData);
        }

        private void TreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            dragDropHandler.HandleItemDrag(treeView, e, TryGetTreeNodeInfoForData);
        }

        private void TreeViewDragLeave(object sender, EventArgs e)
        {
            dragDropHandler.HandleDragLeave(treeView);
        }

        private void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            OnSelectedDataChanged();
        }

        private void OnSelectedDataChanged()
        {
            SelectedDataChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Update timer

        private void InitializeUpdateTimer()
        {
            updateTimer = new Timer
            {
                Interval = updateTimerInterval
            };

            updateTimer.Tick += UpdateTimerOnTick;
        }

        private void StartUpdateTimer()
        {
            if (updateTimer.Enabled)
            {
                updateTimer.Stop();
            }
            else
            {
                treeView.BeginUpdate();
            }

            updateTimer.Start();
        }

        private void UpdateTimerOnTick(object sender, EventArgs eventArgs)
        {
            treeView.EndUpdate();
            updateTimer.Stop();
        }

        #endregion
    }
}