using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView.Properties;
using log4net;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Common.Controls.TreeView
{
    public class TreeViewController : IDisposable
    {
        private readonly TreeView treeView;
        private readonly ICollection<TreeNodeInfo> treeNodeInfos = new HashSet<TreeNodeInfo>();
        private readonly Dictionary<Type, TreeNodeInfo> tagTypeTreeNodeInfoLookup = new Dictionary<Type, TreeNodeInfo>();
        private readonly int maximumTextLength = 259;
        private readonly Dictionary<TreeNode, TreeNodeObserver> treeNodeObserverLookup = new Dictionary<TreeNode, TreeNodeObserver>();
        private object data;
        private int dropAtLocation;
        private Point lastDragOverPoint;
        private PlaceholderLocation lastPlaceholderLocation;
        private TreeNode nodeDropTarget;
        private TreeNode lastPlaceholderNode;
        private Graphics placeHolderGraphics;

        private static readonly ILog Log = LogManager.GetLogger(typeof(TreeViewController));

        public event EventHandler TreeNodeDoubleClick;
        public event EventHandler NodeUpdated; // TODO; Way to explicit!
        public event EventHandler<TreeNodeDataDeletedEventArgs> NodeDataDeleted; // TODO; Way to explicit!

        public TreeViewController(TreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentException(Resources.TreeViewController_TreeViewController_Tree_view_can_t_be_null);
            }

            this.treeView = treeView;

            // Ensure tree nodes are correctly aligned
            treeView.ImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };

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
            treeView.DrawNode += TreeViewDrawNode;
        }

        /// <summary>
        /// Gets or sets the data to render in the tree view.
        /// </summary>
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
                    return;
                }

                AddRootNode();

                treeView.SelectedNode = treeView.Nodes.Count > 0 ? treeView.Nodes[0] : null;
            }
        }

        public IEnumerable<TreeNodeInfo> TreeNodeInfos
        {
            get
            {
                return treeNodeInfos;
            }
        }

        /// <summary>
        /// This method registers the provided <see cref="TreeNodeInfo"/>.
        /// </summary>
        /// <param name="treeNodeInfo">The <see cref="TreeNodeInfo"/> to register.</param>
        public void RegisterTreeNodeInfo(TreeNodeInfo treeNodeInfo)
        {
            treeNodeInfos.Add(treeNodeInfo);
            tagTypeTreeNodeInfoLookup[treeNodeInfo.TagType] = treeNodeInfo;
        }

        public void DeleteNode(TreeNode selectedNode, TreeNodeInfo treeNodeInfo)
        {
            var message = string.Format(Resources.TreeView_DeleteNodeData_Are_you_sure_you_want_to_delete_the_following_item_0_, selectedNode.Text);
            if (MessageBox.Show(message, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            if (treeNodeInfo.OnNodeRemoved != null)
            {
                treeNodeInfo.OnNodeRemoved(selectedNode.Tag, selectedNode.Parent != null ? selectedNode.Parent.Tag : null);
            }

            OnNodeDataDeleted(selectedNode);
        }

        public void CollapseAll(TreeNode node)
        {
            node.Collapse();

            foreach (var childNode in node.Nodes.OfType<TreeNode>())
            {
                CollapseAll(childNode);
            }
        }

        public void ExpandAll(TreeNode node)
        {
            node.Expand();

            foreach (var childNode in node.Nodes.OfType<TreeNode>())
            {
                ExpandAll(childNode);
            }
        }

        /// <summary>
        /// This method searches all nodes in the <see cref="TreeView"/> for a node with a matching tag.
        /// </summary>
        /// <param name="nodeData">The node data to search the corresponding <see cref="TreeNode"/> for.</param>
        /// <returns>The <see cref="TreeNode"/> corresponding the provided node data or <c>null</c> if not found.</returns>
        public TreeNode GetNodeByTag(object nodeData)
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
                           .OfType<TreeNode>()
                           .Select(n => GetNodeByTag(n, tag))
                           .FirstOrDefault(node => node != null);
        }

        /// <summary>
        /// This method updates the provided <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="TreeNode"/> to update.</param>
        /// <exception cref="InvalidOperationException">Thrown when no corresponding <seealso cref="TreeNodeInfo"/> can be found for the provided <see cref="TreeNode"/>.</exception>
        private void UpdateNode(TreeNode treeNode)
        {
            var treeNodeInfo = GetTreeNodeInfoForData(treeNode.Tag);
            if (treeNodeInfo == null)
            {
                throw new InvalidOperationException("No tree node info registered");
            }

            // First of all refresh the child nodes as the other logic below might depend on the presence of child nodes
            RefreshChildNodes(treeNode, treeNodeInfo);

            if (treeNodeInfo.Text != null)
            {
                var text = treeNodeInfo.Text(treeNode.Tag);

                // Having very big strings causes rendering problems in the tree view
                treeNode.Text = text.Length > maximumTextLength
                                    ? text.Substring(0, maximumTextLength)
                                    : text;
            }
            else
            {
                treeNode.Text = "";
            }

            treeNode.ForeColor = treeNodeInfo.ForeColor != null
                                     ? treeNodeInfo.ForeColor(treeNode.Tag)
                                     : Color.FromKnownColor(KnownColor.ControlText);

            if (treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(treeNode.Tag)
                && treeNodeInfo.IsChecked != null && treeNode.Checked != treeNodeInfo.IsChecked(treeNode.Tag))
            {
                treeView.AfterCheck -= TreeViewAfterCheck;
                treeNode.Checked = !treeNode.Checked;
                treeView.AfterCheck += TreeViewAfterCheck;
            }

            OnNodeUpdated(treeNode);
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

        private void AddNode(TreeNode parentNode, object nodeData, int insertionIndex = -1)
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

            if (insertionIndex != -1)
            {
                parentNode.Nodes.Insert(insertionIndex, newNode);
            }
            else
            {
                parentNode.Nodes.Add(newNode);
            }

            treeNodeObserverLookup.Add(newNode, new TreeNodeObserver(newNode, this));
        }

        private void RemoveAllNodes()
        {
            foreach (var treeNode in treeNodeObserverLookup.Keys)
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

            foreach (var childNode in treeNode.Nodes.OfType<TreeNode>())
            {
                RemoveTreeNodeFromLookupRecursively(childNode);
            }
        }

        /// <summary>
        /// This method tries to return a <see cref="TreeNodeInfo"/> object corresponding to the provided data.
        /// </summary>
        /// <param name="item">The data to find the corresponding <see cref="TreeNodeInfo"/> for.</param>
        /// <returns>The <see cref="TreeNodeInfo"/> for the provided data or <c>null</c> if no corresponding <see cref="TreeNodeInfo"/> was found.</returns>
        private TreeNodeInfo GetTreeNodeInfoForData(object item)
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

        private void RefreshChildNodes(TreeNode treeNode, TreeNodeInfo treeNodeInfo)
        {
            var currentTreeNodes = treeNode.Nodes.OfType<TreeNode>().ToList();
            var currentTreeNodesPerTag = currentTreeNodes.ToDictionary(ctn => ctn.Tag, ctn => ctn);
            var newChildNodeObjects = treeNodeInfo.ChildNodeObjects != null
                                          ? treeNodeInfo.ChildNodeObjects(treeNode.Tag)
                                          : new object[0];

            treeNode.Nodes.Clear();

            foreach (var newChildNodeObject in newChildNodeObjects)
            {
                // Try to recycle any exiting node
                if (currentTreeNodesPerTag.ContainsKey(newChildNodeObject))
                {
                    var existingNode = currentTreeNodesPerTag[newChildNodeObject];

                    treeNode.Nodes.Add(existingNode);
                    currentTreeNodes.Remove(existingNode);
                }
                else
                {
                    // Create a new one otherwise
                    AddNode(treeNode, newChildNodeObject);
                }
            }

            foreach (var removedNode in currentTreeNodes)
            {
                RemoveTreeNodeFromLookupRecursively(removedNode);
            }
        }

        # region TreeView event handling

        private void TreeViewBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var treeNodeInfo = GetTreeNodeInfoForData(e.Node.Tag);
            if (treeNodeInfo.CanRename == null || !treeNodeInfo.CanRename(e.Node))
            {
                e.CancelEdit = true;
            }
        }

        private void TreeViewAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Check Label for null as this indicates the node edit was cancelled
            if (e.Label == null)
            {
                return;
            }

            var treeNodeInfo = GetTreeNodeInfoForData(e.Node.Tag);
            if (treeNodeInfo.OnNodeRenamed != null)
            {
                treeNodeInfo.OnNodeRenamed(e.Node.Tag, e.Label);
            }
        }

        private void TreeViewAfterCheck(object sender, TreeViewEventArgs e)
        {
            var treeNodeInfo = GetTreeNodeInfoForData(e.Node.Tag);
            if (treeNodeInfo.OnNodeChecked != null)
            {
                treeNodeInfo.OnNodeChecked(e.Node);
            }
        }

        private void TreeViewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var selectedNode = treeView.SelectedNode;
            if (selectedNode == null)
            {
                return;
            }

            switch (keyEventArgs.KeyData)
            {
                case Keys.F5: // Refresh the selected node in the tree view
                {
                    if (treeView.SelectedNode != null)
                    {
                        UpdateNode(treeView.SelectedNode);
                    }
                    break;
                }
                case Keys.F2: // Start editing the label of the selected node
                {
                    selectedNode.BeginEdit();
                    break;
                }
                case Keys.Apps: // If implemented, show the context menu of the selected node
                {
                    var treeNodeInfo = GetTreeNodeInfoForData(selectedNode.Tag);

                    // Update the context menu (relevant in case of keyboard navigation in the tree view)
                    selectedNode.ContextMenuStrip = treeNodeInfo.ContextMenuStrip != null
                                        ? treeNodeInfo.ContextMenuStrip(selectedNode.Tag, selectedNode, treeNodeInfo)
                                        : null;

                    if (treeView.ContextMenu != null && selectedNode.ContextMenuStrip != null)
                    {
                        var location = selectedNode.Bounds.Location;
                        location.Offset(0, selectedNode.Bounds.Height);

                        selectedNode.ContextMenuStrip.Show(location);
                    }
                    break;
                }
                case Keys.Enter: // Perform the same action as on double click
                {
                    OnTreeNodeDoubleClick();

                    break;
                }
                case Keys.Delete: // If allowed, delete the selected node
                {
                    var treeNodeInfo = GetTreeNodeInfoForData(selectedNode.Tag);

                    if (treeNodeInfo.CanRemove == null || !treeNodeInfo.CanRemove(selectedNode.Tag, selectedNode.Parent != null ? selectedNode.Parent.Tag : null))
                    {
                        MessageBox.Show(Resources.TreeView_DeleteNodeData_The_selected_item_cannot_be_removed, BaseResources.Confirm, MessageBoxButtons.OK);
                        break;
                    }

                    DeleteNode(selectedNode, treeNodeInfo);

                    break;
                }
                case Keys.Space: // If applicable, change the checked state of the selected node
                {
                    var treeNodeInfo = GetTreeNodeInfoForData(selectedNode.Tag);
                    if (treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(selectedNode.Tag))
                    {
                        selectedNode.Checked = !selectedNode.Checked;
                    }

                    break;
                }
            }

            if (keyEventArgs.KeyData == (Keys.Control | Keys.Shift | Keys.Right)) // Expand all tree nodes
            {
                ExpandAll(selectedNode);
                selectedNode.EnsureVisible();
            }

            if (keyEventArgs.KeyData == (Keys.Control | Keys.Shift | Keys.Left)) // Collapse all tree nodes
            {
                CollapseAll(selectedNode);
                selectedNode.EnsureVisible();
            }
        }

        private void TreeViewMouseClick(object sender, MouseEventArgs e)
        {
            var point = treeView.PointToClient(Cursor.Position);
            var clickedNode = treeView.GetNodeAt(point);
            if (clickedNode == null)
            {
                return;
            }

            var treeNodeInfo = GetTreeNodeInfoForData(clickedNode.Tag);

            if ((e.Button & MouseButtons.Right) != 0)
            {
                treeView.SelectedNode = clickedNode;

                // Update the context menu
                clickedNode.ContextMenuStrip = treeNodeInfo.ContextMenuStrip != null
                                    ? treeNodeInfo.ContextMenuStrip(clickedNode.Tag, clickedNode, treeNodeInfo)
                                    : null;

                return;
            }

            if (treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(clickedNode.Tag) && clickedNode.IsOnCheckBox(point))
            {
                clickedNode.Checked = !clickedNode.Checked;
            }
        }

        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            OnTreeNodeDoubleClick();
        }

        private void OnTreeNodeDoubleClick()
        {
            if (TreeNodeDoubleClick != null)
            {
                TreeNodeDoubleClick(treeView.SelectedNode, EventArgs.Empty);
            }
        }

        private void TreeViewDragDrop(object sender, DragEventArgs e)
        {
            ClearPlaceHolders();

            Point point = treeView.PointToClient(new Point(e.X, e.Y));
            var nodeOver = treeView.GetNodeAt(point);

            var nodeDragging = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (nodeOver == null || nodeDragging == null)
            {
                if (nodeOver != null)
                {
                    e.Effect = DragDropEffects.All;
                }

                return;
            }

            // Handle dragged items which were originally higher up in the tree under the same parent (all indices shift by one)
            if (e.Effect.Equals(DragDropEffects.Move) && nodeDragging.Parent == nodeDropTarget && nodeOver.Index > nodeDragging.Index)
            {
                if (dropAtLocation > 0)
                {
                    dropAtLocation--;
                }
            }

            // Ensure the drop location is never < 0
            if (dropAtLocation < 0)
            {
                dropAtLocation = 0;
            }

            var treeNodeInfo = GetTreeNodeInfoForData(nodeDropTarget.Tag);

            try
            {
                if (treeNodeInfo.OnDrop != null)
                {
                    treeNodeInfo.OnDrop(nodeDragging, nodeDropTarget, ToDragOperation(e.Effect), dropAtLocation);
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format(Resources.TreeView_TreeViewDragDrop_Error_during_drag_drop_0_, ex.Message));
            }
        }

        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            if (lastDragOverPoint.X == e.X && lastDragOverPoint.Y == e.Y)
            {
                return;
            }

            lastDragOverPoint = new Point(e.X, e.Y);

            var point = treeView.PointToClient(lastDragOverPoint);
            var nodeOver = treeView.GetNodeAt(point);
            var nodeDragging = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (nodeOver == null || nodeDragging == null || nodeOver == nodeDragging || IsChildOf(nodeOver, nodeDragging))
            {
                ClearPlaceHolders();

                return;
            }

            ScrollIntoView(point, nodeOver, sender);

            PlaceholderLocation placeholderLocation = GetPlaceHoldersLocation(nodeDragging, nodeOver, e);

            if (null == nodeDropTarget)
            {
                return;
            }

            var treeNodeInfo = GetTreeNodeInfoForData(nodeDropTarget.Tag);

            DragOperations allowedOperations = treeNodeInfo.CanDrop != null
                                                   ? treeNodeInfo.CanDrop(nodeDragging, nodeDropTarget, ToDragOperation(e.AllowedEffect))
                                                   : DragOperations.None;

            e.Effect = ToDragDropEffects(allowedOperations);

            if (PlaceholderLocation.None == placeholderLocation)
            {
                return;
            }

            // Determine whether ot not the node can be dropped based on the allowed operations.
            // A node can also be a valid drop traget if it is the root item (nodeDragging.Parent == null).
            var dragOperations = treeNodeInfo.CanDrop != null
                ? treeNodeInfo.CanDrop(nodeDragging, nodeDropTarget, allowedOperations)
                : DragOperations.None;

            if (DragOperations.None != dragOperations)
            {
                DrawPlaceholder(nodeOver, placeholderLocation);
            }
            else
            {
                ClearPlaceHolders();

                e.Effect = DragDropEffects.None;
            }
        }

        private void TreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            // gather allowed effects for the current item.
            var sourceNode = (TreeNode)e.Item;
            var treeNodeInfo = GetTreeNodeInfoForData(sourceNode.Tag);

            DragOperations dragOperation = treeNodeInfo.CanDrag != null
                                               ? treeNodeInfo.CanDrag(sourceNode.Tag, sourceNode)
                                               : DragOperations.None;

            DragDropEffects effects = ToDragDropEffects(dragOperation);

            if (effects == DragDropEffects.None)
            {
                return;
            }

            // store both treenode and tag of treenode in dataobject
            // to be dragged.
            IDataObject dataObject = new DataObject();
            dataObject.SetData(typeof(TreeNode), sourceNode);

            if (sourceNode.Tag != null)
            {
                dataObject.SetData(sourceNode.Tag.GetType(), sourceNode.Tag);
            }

            treeView.DoDragDrop(dataObject, effects);
        }

        private void TreeViewDragLeave(object sender, EventArgs e)
        {
            ClearPlaceHolders();
        }

        private void TreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = false;

            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;

            e.Node.DrawNode(GetTreeNodeInfoForData(e.Node.Tag), e.Graphics, selected);
        }

        private void DrawPlaceholder(TreeNode node, PlaceholderLocation location)
        {
            if (lastPlaceholderNode == node && lastPlaceholderLocation == location)
            {
                return;
            }

            ClearPlaceHolders();

            lastPlaceholderNode = node;
            lastPlaceholderLocation = location;

            placeHolderGraphics = treeView.CreateGraphics();
            node.DrawPlaceHolder(location, treeView.CreateGraphics());
        }

        private void ClearPlaceHolders()
        {
            if (placeHolderGraphics != null)
            {
                lastPlaceholderNode = null;

                treeView.Refresh();

                placeHolderGraphics.Dispose();
                placeHolderGraphics = null;
            }
        }

        private DragOperations ToDragOperation(DragDropEffects dragDropEffects)
        {
            return (DragOperations) Enum.Parse(typeof(DragOperations), dragDropEffects.ToString());
        }

        private bool IsChildOf(TreeNode childNode, TreeNode node)
        {
            while (childNode != null && childNode.Parent != null)
            {
                if (childNode.Parent.Equals(node))
                {
                    return true;
                }

                childNode = childNode.Parent; // Walk up the tree
            }

            return false;
        }

        private static void ScrollIntoView(Point point, TreeNode nodeOver, object sender)
        {
            var treeView = sender as TreeView;
            if (treeView == null)
            {
                return;
            }
            int delta = treeView.Height - point.Y;
            if ((delta < treeView.Height / 2) && (delta > 0))
            {
                var nextVisibleNode = nodeOver.NextVisibleNode;
                if (nextVisibleNode != null)
                {
                    nextVisibleNode.EnsureVisible();
                }
            }
            if ((delta > treeView.Height / 2) && (delta < treeView.Height))
            {
                var previousVisibleNode = nodeOver.PrevVisibleNode;
                if (previousVisibleNode != null)
                {
                    previousVisibleNode.EnsureVisible();
                }
            }
        }

        private PlaceholderLocation GetPlaceHoldersLocation(TreeNode nodeDragging, TreeNode nodeOver, DragEventArgs e)
        {
            var loc = PlaceholderLocation.None;
            int offsetY = treeView.PointToClient(Cursor.Position).Y - nodeOver.Bounds.Top;

            if (offsetY < nodeOver.Bounds.Height / 3 && nodeDragging.NextNode != nodeOver)
            {
                if (nodeOver.Parent != null)
                {
                    var treeNodeInfo = GetTreeNodeInfoForData(nodeOver.Parent.Tag);
                    if (treeNodeInfo.CanInsert != null && treeNodeInfo.CanInsert(nodeDragging, nodeOver))
                    {
                        nodeDropTarget = nodeOver.Parent;
                        dropAtLocation = nodeOver.Parent == null ? 0 : nodeOver.Parent.Nodes.IndexOf(nodeOver);
                        loc = PlaceholderLocation.Top;
                    }
                    else
                    {
                        nodeDropTarget = nodeOver;
                        dropAtLocation = 0;
                        loc = PlaceholderLocation.Middle;
                    }
                }
                else
                {
                    nodeDropTarget = nodeOver;
                    dropAtLocation = 0;
                    loc = PlaceholderLocation.Middle;
                }
            }
            else if ((nodeOver.Parent != null) && (offsetY > nodeOver.Bounds.Height - nodeOver.Bounds.Height / 3) &&
                     nodeDragging.PrevNode != nodeOver)
            {
                var treeNodeInfo = GetTreeNodeInfoForData(nodeOver.Parent.Tag);
                if (treeNodeInfo.CanInsert != null && treeNodeInfo.CanInsert(nodeDragging, nodeOver))
                {
                    nodeDropTarget = nodeOver.Parent;
                    dropAtLocation = nodeOver.Parent == null
                                         ? 0
                                         : nodeOver.Parent.Nodes.IndexOf(nodeOver) + 1;
                    loc = PlaceholderLocation.Bottom;
                }
                else
                {
                    nodeDropTarget = nodeOver;
                    dropAtLocation = 0;
                    loc = PlaceholderLocation.Middle;
                }
            }
            else if (nodeDragging != nodeOver && offsetY < nodeOver.Bounds.Height - nodeOver.Bounds.Height / 4
                     && offsetY > nodeOver.Bounds.Height / 4)
            {
                nodeDropTarget = nodeOver;
                dropAtLocation = 0;
                loc = PlaceholderLocation.Middle;
            }

            if (loc == PlaceholderLocation.None ||
                (loc == PlaceholderLocation.Middle && nodeDropTarget == nodeDragging.Parent))
            {
                ClearPlaceHolders();
                e.Effect = DragDropEffects.None;
            }
            return loc;
        }

        private DragDropEffects ToDragDropEffects(DragOperations operation)
        {
            return (DragDropEffects) Enum.Parse(typeof(DragDropEffects), operation.ToString());
        }

        # endregion

        # region Event handling

        private void OnNodeUpdated(TreeNode treeNode)
        {
            if (NodeUpdated != null)
            {
                NodeUpdated(treeNode, EventArgs.Empty);
            }
        }

        private void OnNodeDataDeleted(TreeNode node)
        {
            if (NodeDataDeleted != null)
            {
                NodeDataDeleted(this, new TreeNodeDataDeletedEventArgs(node.Tag));
            }
        }

        # endregion

        # region Nested types

        private class TreeNodeObserver : IDisposable, IObserver
        {
            private readonly TreeNode treeNode;
            private readonly TreeViewController controller;

            public TreeNodeObserver(TreeNode treeNode, TreeViewController controller)
            {
                this.treeNode = treeNode;
                this.controller = controller;

                var observable = treeNode.Tag as IObservable;
                if (observable != null)
                {
                    observable.Attach(this);
                }
            }

            public void Dispose()
            {
                var observable = treeNode.Tag as IObservable;
                if (observable != null)
                {
                    observable.Detach(this);
                }
            }

            public void UpdateObserver()
            {
                controller.UpdateNode(treeNode);
            }
        }

        # endregion

        public void Dispose()
        {
            treeView.BeforeLabelEdit -= TreeViewBeforeLabelEdit;
            treeView.AfterLabelEdit -= TreeViewAfterLabelEdit;
            treeView.AfterCheck -= TreeViewAfterCheck;
            treeView.KeyDown -= TreeViewKeyDown;
            treeView.MouseClick -= TreeViewMouseClick;
            treeView.DoubleClick -= TreeNodeDoubleClick;
            treeView.DragDrop -= TreeViewDragDrop;
            treeView.DragOver -= TreeViewDragOver;
            treeView.ItemDrag -= TreeViewItemDrag;
            treeView.DragLeave -= TreeViewDragLeave;
            treeView.DrawNode -= TreeViewDrawNode;
        }
    }
}