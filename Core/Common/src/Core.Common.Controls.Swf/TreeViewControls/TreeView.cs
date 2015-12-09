using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Properties;
using log4net;

namespace Core.Common.Controls.Swf.TreeViewControls
{
    /// <summary>
    /// Summary description for Tree.
    /// </summary>
    public class TreeView : System.Windows.Forms.TreeView, ITreeView
    {
        public event EventHandler SelectedNodeChanged;
        public event EventHandler OnUpdate;

        public event Action BeforeWaitUntilAllEventsAreProcessed;

        private const int TV_FIRST = 0x1100;
        private const int TVM_SETBKCOLOR = TV_FIRST + 29;
        private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;

        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly TreeNodeList nodes;

        private TreeViewController controller;
        private int dropAtLocation;
        private Point lastDragOverPoint;
        private PlaceholderLocation lastPlaceholderLocation;
        private ITreeNode nodeDropTarget;
        private ITreeNode lastPlaceholderNode;
        private Graphics placeHolderGraphics;
        private bool bufferedNodeExpanded;

        /// <summary>
        /// TreeView based on system windows forms component.
        /// </summary>
        public TreeView()
        {
            controller = new TreeViewController(this);
            ImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };
            nodes = new TreeNodeList(base.Nodes);
            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            LabelEdit = true;
            HideSelection = false;

            BeforeLabelEdit += TreeViewBeforeLabelEdit;
            AfterCheck += TreeViewAfterCheck;

            DragDrop += TreeViewDragDrop;
            DragOver += TreeViewDragOver;
            ItemDrag += TreeViewItemDrag;
            DragLeave += TreeViewDragLeave;

            MouseDown += TreeViewMouseDown;
            MouseClick += TreeViewClick;

            SelectNodeOnRightMouseClick = true; // default behaviour

            // http://dev.nomad-net.info/articles/double-buffered-tree-and-list-views
            // Enable default double buffering processing
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            // Disable default CommCtrl painting on non-Vista systems
            if (!NativeInterop.IsWinVista)
            {
                SetStyle(ControlStyles.UserPaint, true);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<Keys, bool> OnProcessCmdKey { get; set; }

        public bool SelectNodeOnRightMouseClick { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ITreeNode SelectedNode
        {
            get
            {
                return ((ITreeView) this).SelectedNode;
            }
            set
            {
                ((ITreeView) this).SelectedNode = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Data
        {
            get
            {
                if (IsDisposed || controller == null)
                {
                    return null;
                }
                return controller.Data;
            }
            set
            {
                if (IsDisposed || controller == null)
                {
                    return;
                }
                controller.Data = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Image { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ITreeNode> AllLoadedNodes
        {
            get
            {
                return Enumerable.SelectMany<ITreeNode, ITreeNode>(Nodes, GetAllLoadedNodes);
            }
        }

        /// <summary>
        /// The nodepresenters handle building logic for dataobjects added to the tree.
        /// </summary>
        public ICollection<ITreeNodePresenter> NodePresenters
        {
            get
            {
                return controller.NodePresenters;
            }
        }

        /// <summary>
        /// List of all nodes that belong to the tree.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new IList<ITreeNode> Nodes
        {
            get
            {
                return nodes;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ITreeNode ITreeView.SelectedNode
        {
            get
            {
                return (TreeNode) base.SelectedNode;
            }
            set
            {
                base.SelectedNode = (System.Windows.Forms.TreeNode) value;

                if (SelectedNodeChanged != null)
                {
                    SelectedNodeChanged(this, new EventArgs());
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewInfo ViewInfo { get; set; }

        public bool CanDelete(ITreeNode node)
        {
            return controller.CanDeleteNode(node);
        }

        public bool SelectedNodeCanRename()
        {
            return controller.CanRenameNode(SelectedNode);
        }

        public new void CollapseAll()
        {
            foreach (var node in Nodes)
            {
                CollapseAll(node);
            }
        }

        public void CollapseAll(ITreeNode node)
        {
            node.Collapse();
            foreach (var childNode in node.Nodes)
            {
                CollapseAll(childNode);
            }
        }

        public new void ExpandAll()
        {
            foreach (var node in Nodes)
            {
                ExpandAll(node);
            }
        }

        public void ExpandAll(ITreeNode node)
        {
            node.Expand();
            foreach (var childNode in node.Nodes)
            {
                ExpandAll(childNode);
            }
        }

        /// <summary>
        /// Checks if label of current node can be edited and starts edit mode if this is the case.
        /// </summary>
        public void StartLabelEdit()
        {
            if (!SelectedNodeCanRename())
            {
                return;
            }

            SelectedNode.BeginEdit();
        }

        public void EnsureVisible(object item) {}

        public ITreeNodePresenter GetTreeViewNodePresenter(object nodeData, ITreeNode node)
        {
            if (nodeData == null)
            {
                throw new ArgumentNullException("nodeData", Resources.TreeView_Error_Unable_to_resolve_node_presenter_for_null_data);
            }
            return controller.ResolveNodePresenterForData(nodeData, node == null ? null : node.Parent);
        }

        /// <summary>
        /// Create a new Node for this tree with default properties
        /// </summary>
        /// <returns></returns>
        public ITreeNode NewNode()
        {
            return new TreeNode(this);
        }

        public ITreeNode AddNewNode(ITreeNode parentNode, object nodeData, int insertionIndex = -1)
        {
            return controller.AddNewNode(parentNode, nodeData, insertionIndex);
        }

        /// <summary>
        /// Search all the nodes in the treeView, for a node with a matching tag.
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="skipUnLoadedNodes">if a node is not loaded, don't do so</param>
        /// <returns></returns>
        public ITreeNode GetNodeByTag(object nodeData, bool skipUnLoadedNodes = true)
        {
            if (Nodes.Count > 0)
            {
                return GetNodeByTag(Nodes[0], nodeData, skipUnLoadedNodes);
            }

            return null;
        }

        public void UpdateNode(ITreeNode treeNode)
        {
            if (controller != null)
            {
                controller.UpdateNode(treeNode, treeNode.Tag);
                FireOnUpdateEvent(treeNode);
            }
        }

        /// <summary>
        /// Fires the OnUpdate event with the given <see cref="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode"/> to fire an update event for.</param>
        private void FireOnUpdateEvent(ITreeNode treeNode)
        {
            if (OnUpdate != null)
            {
                OnUpdate(treeNode, EventArgs.Empty);
            }
        }

        public void RefreshChildNodes(ITreeNode treeNode)
        {
            if (controller != null)
            {
                controller.RefreshChildNodes(treeNode);
            }
        }

        public void TryDeleteNodeData(ITreeNode treeNode)
        {
            if (!CanDelete(treeNode))
            {
                MessageBox.Show(Resources.TreeView_DeleteNodeData_The_selected_item_cannot_be_removed, Resources.TreeView_DeleteNodeData_Confirm, MessageBoxButtons.OK);
                return;
            }

            var message = string.Format(Resources.TreeView_DeleteNodeData_Are_you_sure_you_want_to_delete_the_following_item_0_, treeNode.Text);
            if (MessageBox.Show(message, Resources.TreeView_DeleteNodeData_Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            DeleteNodeData(treeNode);
        }

        public void TryDeleteSelectedNodeData()
        {
            TryDeleteNodeData(SelectedNode);
        }

        private void DeleteNodeData(ITreeNode node)
        {
            var presenter = GetTreeViewNodePresenter(node.Tag, node);
            presenter.RemoveNodeData(node.Parent.Tag, node.Tag);
            SelectedNode = SelectedNode ?? Nodes.FirstOrDefault();
        }

        public override void Refresh()
        {
            if (Nodes.Count == 0 || controller == null)
            {
                return;
            }

            BeginUpdate();
            try
            {
                var selectedNodePath = SelectedNode == null ? string.Empty : SelectedNode.FullPath;

                // sometimes collection results in full refresh, as a result root node does not get expanded
                bool rootNodeIsLoaded = Nodes[0].IsLoaded;

                //don't use allNodes since update of childnodes is done by parent.
                foreach (TreeNode node in Nodes)
                {
                    controller.UpdateNode(node);
                }

                if (!string.IsNullOrEmpty(selectedNodePath))
                {
                    var nodeToSelect =
                        Nodes.SelectMany(GetAllLoadedNodes).FirstOrDefault(n => n.FullPath == selectedNodePath);

                    if (nodeToSelect != null)
                    {
                        base.SelectedNode = (System.Windows.Forms.TreeNode) nodeToSelect;
                    }
                }

                // expand root node if children were added
                if (!rootNodeIsLoaded && Nodes.Count > 0 && Nodes[0].Nodes.Count > 0)
                {
                    Nodes[0].Expand();
                }

                Sort();
            }
            finally
            {
                EndUpdate();
            }
        }

        public IEnumerable<ITreeNode> GetAllLoadedNodes(ITreeNode currentNode)
        {
            var allChildNodes = new[]
            {
                currentNode
            };

            return (currentNode.IsLoaded)
                       ? allChildNodes.Concat(currentNode.Nodes.SelectMany(GetAllLoadedNodes))
                       : allChildNodes;
        }

        public void WaitUntilAllEventsAreProcessed()
        {
            if (BeforeWaitUntilAllEventsAreProcessed != null)
            {
                BeforeWaitUntilAllEventsAreProcessed();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                Message m = new Message();
                m.HWnd = Handle;
                m.Msg = NativeInterop.WM_PRINTCLIENT;
                m.WParam = e.Graphics.GetHdc();
                m.LParam = (IntPtr) NativeInterop.PRF_CLIENT;
                DefWndProc(ref m);
                e.Graphics.ReleaseHdc(m.WParam);
            }
            base.OnPaint(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateExtendedStyles();
            if (!NativeInterop.IsWinXP || !Application.RenderWithVisualStyles)
            {
                NativeInterop.SendMessage(Handle, TVM_SETBKCOLOR, IntPtr.Zero, (IntPtr) ColorTranslator.ToWin32(BackColor));
            }
            controller.OnTreeViewHandleCreated();
        }

        /// <summary>
        /// Custom drawing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = false;

            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;

            ((TreeNode) e.Node).DrawNode(e.Graphics, selected);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && controller != null)
            {
                controller = null;
            }

            if (IsHandleCreated)
            {
                base.Dispose(disposing);
            }
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            BeginUpdate();

            try
            {
                var treeNode = e.Node as TreeNode;

                if (treeNode != null)
                {
                    //check e.Label for null, this indicates node edit was cancelled.
                    if (treeNode.IsUpdating || treeNode.Tag == null || e.Label == null)
                    {
                        return;
                    }

                    var nodePresenter = GetTreeViewNodePresenter(treeNode.Tag, treeNode);
                    if (nodePresenter == null)
                    {
                        return;
                    }

                    if (nodePresenter.CanRenameNodeTo(treeNode, e.Label))
                    {
                        // First set text to prevent eventHandlers from working with a node that
                        // is not yet updated (with the previous text)
                        // Setting the node text is done after this method is completed (and e.cancel is false).
                        treeNode.Text = e.Label;
                        nodePresenter.OnNodeRenamed(treeNode.Tag, e.Label);
                    }
                    else
                    {
                        e.CancelEdit = true;
                        return;
                    }
                }

                base.OnAfterLabelEdit(e);
                BeginInvoke(new Action(Sort));
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Do not expand/collapse on doubleclick
        /// </summary>
        /// <param name="m"></param>
        protected override void DefWndProc(ref Message m)
        {
            const int WM_LBUTTONDBLCLK = 515;
            const int WM_ERASEBKGND = 0x0014;

            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                //dont handle doubleclick in base class
            }
            else if (m.Msg == WM_ERASEBKGND)
            {
                return; //don't clear background (only causes flicker)
            }
            else
            {
                //log.DebugFormat(m.ToString());
                base.DefWndProc(ref m);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (Nodes.Count == 0)
            {
                if (OnProcessCmdKey != null)
                {
                    return OnProcessCmdKey(keyData);
                }

                return base.ProcessCmdKey(ref msg, keyData);
            }

            if (SelectedNode == null)
            {
                SelectedNode = Nodes[0];
            }

            if (SelectedNode.IsEditing)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            switch (keyData)
            {
                case Keys.F2:
                    //start editing the label
                    StartLabelEdit();
                    return true;

                case Keys.Apps:
                    if (SelectedNode != null && ContextMenu != null)
                    {
                        Point location = SelectedNode.Bounds.Location;
                        location.Offset(0, SelectedNode.Bounds.Height);
                        SelectedNode.ShowContextMenu(location);
                    }
                    return true;

                // TODO: Customize completely within OnProcessCmdKey
                case Keys.Delete:
                    if (OnProcessCmdKey == null && SelectedNode != null && SelectedNode.Tag != null)
                    {
                        TryDeleteSelectedNodeData();
                        return true;
                    }
                    break;

                case Keys.F5:

                    //refresh treeview
                    Refresh();
                    return true;

                case Keys.Space:
                    ITreeNode node = (TreeNode) SelectedNode;
                    if (node != null && node.ShowCheckBox)
                    {
                        //Toggle checked state
                        node.Checked = !node.Checked;
                    }
                    return true;
                case Keys.Up:
                {
                    //hack: we manually handle this because ms doesnot fire selectednodechanged
                    // Select the previous node
                    var treeNode = SelectedNode.PreviousVisibleNode;
                    if (treeNode != null)
                    {
                        SelectedNode = treeNode;
                    }
                }
                    return true;
                case Keys.Down:
                {
                    //hack: we manually handle this because ms doesnot fire selectednodechanged
                    // Select the next node
                    var treeNode = SelectedNode.NextVisibleNode;
                    if (treeNode != null)
                    {
                        SelectedNode = treeNode;
                    }
                }
                    return true;
                case Keys.Right:
                {
                    //hack: we manually handle this because ms doesnot fire selectednodechanged

                    if (SelectedNode.Nodes.Count > 0)
                    {
                        if (!SelectedNode.IsExpanded)
                        {
                            SelectedNode.Expand();
                        }
                        else
                        {
                            SelectedNode = SelectedNode.Nodes[0];
                        }
                    }
                }
                    return true;
                case Keys.Left:
                {
                    //hack: we manually handle this because ms doesnot fire selectednodechanged

                    if (SelectedNode.IsExpanded)
                    {
                        SelectedNode.Collapse();
                    }
                    else
                    {
                        if (SelectedNode.Parent != null)
                        {
                            SelectedNode = SelectedNode.Parent;
                        }
                    }
                }
                    return true;
                case Keys.Home:
                {
                    //hack: we manually handle this because ms doesnot fire selectednodechanged

                    SelectedNode = Nodes.First();
                }
                    return true;
                case Keys.End:
                {
                    //hack: we manually handle this because ms doesnot fire selectednodechanged

                    SelectedNode = GetLastNode(Nodes.Last());
                }
                    return true;
            }

            if (keyData == (Keys.Control | Keys.Shift | Keys.Right))
            {
                ExpandAll(SelectedNode);
                SelectedNode.EnsureVisible();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Shift | Keys.Left))
            {
                CollapseAll(SelectedNode);
                SelectedNode.EnsureVisible();
                return true;
            }

            if (OnProcessCmdKey != null)
            {
                if (OnProcessCmdKey(keyData))
                {
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateExtendedStyles()
        {
            int Style = 0;

            if (DoubleBuffered)
            {
                Style |= TVS_EX_DOUBLEBUFFER;
            }

            if (Style != 0)
            {
                NativeInterop.SendMessage(Handle, TVM_SETEXTENDEDSTYLE, (IntPtr) TVS_EX_DOUBLEBUFFER, (IntPtr) Style);
            }
        }

        private new void Sort()
        {
            if (TreeViewNodeSorter == null)
            {
                return;
            }

            var oldSelectedNode = SelectedNode;

            SuspendLayout();
            base.Sort();
            ResumeLayout();

            SelectedNode = oldSelectedNode;
        }

        private ITreeNode GetLastNode(ITreeNode treeNode)
        {
            if (treeNode.IsLoaded && treeNode.Nodes.Count > 0)
            {
                return GetLastNode(treeNode.Nodes.Last());
            }

            return treeNode;
        }

        private void TreeViewMouseDown(object sender, MouseEventArgs e)
        {
            var treeView = sender as TreeView;
            if (treeView == null)
            {
                return;
            }

            var treeNode = (ITreeNode) GetNodeAt(e.X, e.Y);
            if (treeNode == null)
            {
                return;
            }

            // buffer expanded state for use in TreeViewClick
            bufferedNodeExpanded = treeNode.IsExpanded;
        }

        private void TreeViewClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !SelectNodeOnRightMouseClick)
            {
                return;
            }

            var point = PointToClient(MousePosition);
            var node = (TreeNode) GetNodeAt(point);
            if (node == null)
            {
                return;
            }

            SelectedNode = node;

            if (node.IsOnCheckBox(point))
            {
                node.Checked = !node.Checked;
             }

            if (node.IsOnExpandButton(point))
            {
                // Use buffered expanded state because it gets changed just
                // before Click event is handled
                if (bufferedNodeExpanded)
                {
                    node.Collapse(true);
                }
                else
                {
                    node.Expand();
                }
            }
        }

        private void TreeViewAfterCheck(object sender, TreeViewEventArgs e)
        {
            controller.OnNodeChecked((ITreeNode) e.Node);
        }

        /// <summary>
        /// Check if node label can be edited 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!controller.CanRenameNode(e.Node as ITreeNode))
            {
                e.CancelEdit = true;
            }
        }

        private static ITreeNode GetNodeByTag(ITreeNode rootNode, object tag, bool skipUnLoadedNodes)
        {
            if (Equals(rootNode.Tag, tag))
            {
                return rootNode;
            }

            return rootNode.IsLoaded || !skipUnLoadedNodes
                       ? rootNode.Nodes.Select(n => GetNodeByTag(n, tag, skipUnLoadedNodes)).FirstOrDefault(node => node != null)
                       : null;
        }

        /// <summary>
        /// Use Nodepresenter to see wether the current node can be dragged
        /// set the allowed effects based on the possible dragoperation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            // gather allowed effects for the current item.
            var sourceNode = (TreeNode) e.Item;
            ITreeNodePresenter presenter = GetTreeViewNodePresenter(sourceNode.Tag, sourceNode);

            if (presenter == null)
            {
                return;
            }

            DragOperations dragOperation = presenter.CanDrag(sourceNode.Tag);

            DragDropEffects effects = WindowsFormsHelper.ToDragDropEffects(dragOperation);

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

            DoDragDrop(dataObject, effects);
        }

        /// <summary>
        /// Update the UI of the treeview for the current action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            if (lastDragOverPoint.X == e.X && lastDragOverPoint.Y == e.Y)
            {
                return;
            }

            lastDragOverPoint = new Point(e.X, e.Y);
            var point = PointToClient(lastDragOverPoint);

            var nodeOver = GetNodeAt(point) as TreeNode;
            var nodeDragging = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (nodeOver == null || nodeDragging == null || nodeOver == nodeDragging || nodeOver.IsChildOf(nodeDragging))
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

            ITreeNodePresenter presenter = GetTreeViewNodePresenter(nodeDropTarget.Tag, nodeDropTarget);
            DragOperations allowedOperations = presenter.CanDrop(nodeDragging.Tag, nodeDragging, nodeDropTarget,
                                                                 WindowsFormsHelper.ToDragOperation(e.AllowedEffect));
            e.Effect = WindowsFormsHelper.ToDragDropEffects(allowedOperations);

            if (PlaceholderLocation.None == placeholderLocation)
            {
                return;
            }

            // determine if the node can be dropped based on the allowed operations.
            // A node can also be a valid drop traget if it is the root item! (nodeDragging.Parent == null)
            // This applies in any case to the MapLegendView
            if (DragOperations.None != presenter.CanDrop(nodeDragging.Tag, nodeDragging, nodeDropTarget, allowedOperations))
            {
                DrawPlaceholder(nodeOver, placeholderLocation);
            }
            else
            {
                ClearPlaceHolders();
                e.Effect = DragDropEffects.None;
            }
        }

        private PlaceholderLocation GetPlaceHoldersLocation(ITreeNode nodeDragging, ITreeNode nodeOver, DragEventArgs e)
        {
            PlaceholderLocation loc = PlaceholderLocation.None;
            int offsetY = PointToClient(Cursor.Position).Y - nodeOver.Bounds.Top;

            if (offsetY < (nodeOver.Bounds.Height/3) && nodeDragging.NextNode != nodeOver)
            {
                if (nodeOver.Parent != null)
                {
                    ITreeNodePresenter parentNodePresenter = GetTreeViewNodePresenter(nodeOver.Parent.Tag, nodeOver.Parent);
                    if (parentNodePresenter.CanInsert(nodeDragging.Tag, nodeDragging, nodeOver))
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
            else if ((nodeOver.Parent != null) && (offsetY > (nodeOver.Bounds.Height - (nodeOver.Bounds.Height/3))) &&
                     nodeDragging.PreviousNode != nodeOver)
            {
                ITreeNodePresenter nodePresenter = GetTreeViewNodePresenter(nodeOver.Parent.Tag, nodeOver.Parent);
                if (nodePresenter.CanInsert(nodeDragging.Tag, nodeDragging, nodeOver))
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
            else if (nodeDragging != nodeOver && offsetY < (nodeOver.Bounds.Height - (nodeOver.Bounds.Height/4))
                     && offsetY > (nodeOver.Bounds.Height/4))
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

        /// <summary>
        /// handle scrolling
        /// http://www.syncfusion.com/FAQ/windowsforms/faq_c91c.aspx
        /// </summary>
        /// <param name="point"></param>
        /// <param name="nodeOver"></param>
        /// <param name="sender"></param>
        private static void ScrollIntoView(Point point, ITreeNode nodeOver, object sender)
        {
            var treeView = sender as TreeView;
            if (treeView == null)
            {
                return;
            }
            int delta = treeView.Height - point.Y;
            if ((delta < treeView.Height/2) && (delta > 0))
            {
                if (nodeOver.NextVisibleNode != null)
                {
                    nodeOver.NextVisibleNode.ScrollTo();
                }
            }
            if ((delta > treeView.Height/2) && (delta < treeView.Height))
            {
                if (nodeOver.PreviousVisibleNode != null)
                {
                    nodeOver.PreviousVisibleNode.ScrollTo();
                }
            }
        }

        /// <summary>
        /// Event handler for drag drop. Inserts Node at droplocation when node is being moved
        /// When node is being copied from other location Dropped should be handled externally
        /// Triggers afterdrop event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewDragDrop(object sender, DragEventArgs e)
        {
            ClearPlaceHolders();
            Point point = PointToClient(new Point(e.X, e.Y));
            var nodeOver = GetNodeAt(point) as TreeNode;

            var nodeDragging = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (nodeOver == null || nodeDragging == null)
            {
                //this handler only deals with nodes.
                ClearPlaceHolders();

                if (nodeOver != null)
                {
                    e.Effect = DragDropEffects.All;
                }

                return;
            }

            if (e.Effect.Equals(DragDropEffects.Move) && nodeDragging.Parent == nodeDropTarget &&
                nodeOver.Index > nodeDragging.Index)
            {
                // src item higher up in the tree is removed. This means all indices shift by one.
                if (dropAtLocation > 0)
                {
                    dropAtLocation--;
                }
            }

            //dropAtLocation should never be < 0
            if (dropAtLocation < 0)
            {
                dropAtLocation = 0;
            }

            var parentNode = nodeDragging.Parent;

            //ensure droptarget is loaded.
            Trace.Assert(nodeDropTarget.Nodes != null);

            try
            {
                controller.OnDragDrop(nodeDragging, parentNode, nodeDropTarget, WindowsFormsHelper.ToDragOperation(e.Effect), dropAtLocation);
            }
            catch (Exception ex)
            {
                log.Error(string.Format(Resources.TreeView_TreeViewDragDrop_Error_during_drag_drop_0_, ex.Message));
            }
        }

        private void TreeViewDragLeave(object sender, EventArgs e)
        {
            ClearPlaceHolders();
        }

        /// <summary>
        /// Indicate the dragged node can be inserted either above or below
        /// another node
        /// </summary>
        private void DrawPlaceholder(ITreeNode node, PlaceholderLocation location)
        {
            if (lastPlaceholderNode == node && lastPlaceholderLocation == location)
            {
                return;
            }

            ClearPlaceHolders();

            lastPlaceholderNode = node;
            lastPlaceholderLocation = location;

            placeHolderGraphics = CreateGraphics();
            node.DrawPlaceHolder(location, CreateGraphics());
        }

        /// <summary>
        /// If placeholders were drawn before, refresh the tree
        /// </summary>
        private void ClearPlaceHolders()
        {
            if (placeHolderGraphics != null)
            {
                lastPlaceholderNode = null;

                base.Refresh();
                placeHolderGraphics.Dispose();
                placeHolderGraphics = null;
            }
        }

        internal class NativeInterop
        {
            public const int WM_PRINTCLIENT = 0x0318;
            public const int PRF_CLIENT = 0x00000004;

            public static bool IsWinXP
            {
                get
                {
                    OperatingSystem OS = Environment.OSVersion;
                    return (OS.Platform == PlatformID.Win32NT) &&
                           ((OS.Version.Major > 5) || ((OS.Version.Major == 5) && (OS.Version.Minor == 1)));
                }
            }

            public static bool IsWinVista
            {
                get
                {
                    OperatingSystem OS = Environment.OSVersion;
                    return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
                }
            }

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        }
    }
}