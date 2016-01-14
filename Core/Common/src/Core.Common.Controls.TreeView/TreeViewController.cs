using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView.Properties;
using Core.Common.Utils.Extensions;

using log4net;

namespace Core.Common.Controls.TreeView
{
    public class TreeViewController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TreeViewController));

        private readonly TreeView treeView;
        private readonly ICollection<ITreeNodePresenter> nodePresenters = new HashSet<ITreeNodePresenter>();

        private readonly Dictionary<Type, ITreeNodePresenter> nodeTagTypePresenters = new Dictionary<Type, ITreeNodePresenter>();
        private object data;

        private bool updatingExpandedState; // prevents recursive entries
        private TreeNode expandedStateRootNode;
        private string[] loadedNodePaths;
        private string[] expandedNodePaths;

        public TreeViewController(TreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentException(Resources.TreeViewController_TreeViewController_Tree_view_can_t_be_null);
            }

            this.treeView = treeView;
        }

        /// <summary>
        /// List of registered node presenters
        /// </summary>
        public IEnumerable<ITreeNodePresenter> NodePresenters
        {
            get
            {
                return nodePresenters;
            }
        }

        /// <summary>
        /// Data to render in the tree view
        /// </summary>
        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                treeView.Nodes.Clear();
                data = value;

                if (data == null)
                {
                    return;
                }

                CreateRootNode();

                treeView.SelectedNode = treeView.Nodes.Count > 0 ? treeView.Nodes[0] : null;
            }
        }

        /// <summary>
        /// Registers the node presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        public void RegisterNodePresenter(ITreeNodePresenter presenter)
        {
            nodePresenters.Add(presenter);
            presenter.TreeView = treeView;
        }

        /// <summary>
        /// Gets the most specific node presenter for a piece of data. Does so by walking up the class hierarchy and trying to find an exact match
        /// It is in the helper to allow heavy testing since this is complex logic.
        /// </summary>
        /// <param name="item">Object to search a node presenter for</param>
        /// <param name="parentNode">Parent node of the node to resolve</param>
        public ITreeNodePresenter ResolveNodePresenterForData(object item, TreeNode parentNode = null)
        {
            if (parentNode == null)
            {
                var nodeByTag = treeView.GetNodeByTag(item);

                if (nodeByTag != null)
                {
                    parentNode = nodeByTag.Parent;
                }
            }

            //try to find a match on the exact type
            if (item == null)
            {
                return null;
            }

            ITreeNodePresenter presenter;

            // try to get presenter from the cache
            var type = item.GetType();
            nodeTagTypePresenters.TryGetValue(type, out presenter);

            // resolve presenter for type
            if (presenter == null)
            {
                presenter = GetNodePresenterForType(type, parentNode) ??
                            nodePresenters.FirstOrDefault(np => np.NodeTagType.IsInstanceOfType(item));

                nodeTagTypePresenters[type] = presenter;
            }

            return presenter;
        }

        /// <summary>
        /// Action to perform when the node is checked/unchecked
        /// </summary>
        /// <param name="node">Node that is checked/unchecked</param>
        public void OnNodeChecked(TreeNode node)
        {
            if (node.IsUpdating)
            {
                return;
            }

            if (node.Tag == null)
            {
                throw new InvalidOperationException(Resources.TreeView_Error_Unable_to_resolve_node_presenter_for_null_data);
            }

            node.Presenter.OnNodeChecked(node);
        }

        public void UpdateNode(TreeNode treeNode)
        {
            UpdateNode(treeNode, treeNode.Tag);
        }

        /// <summary>
        /// Updates the node and if loaded the sub nodes
        /// </summary>
        /// <param name="treeNode">Node to update</param>
        /// <param name="tag">the object bound to this node</param>
        public void UpdateNode(TreeNode treeNode, object tag)
        {
            var treeViewControl = treeView as Control;
            if (treeViewControl != null && treeViewControl.InvokeRequired)
            {
                UpdateNodeInvokeDelegate updateNode = UpdateNode;
                treeViewControl.Invoke(updateNode, treeNode, tag);
            }
            else
            {
                treeView.BeginUpdate();
                try
                {
                    var nodePresenter = treeNode.Presenter;

                    if (nodePresenter == null)
                    {
                        Log.Debug(string.Format(Resources.TreeViewController_UpdateNode_Can_t_find_INodePresenter_for_0_make_sure_you_added_it_to_Presenters_collection_of_a_TreeView, tag));
                        return;
                    }

                    if (!ReferenceEquals(treeNode.Tag, tag))
                    {
                        treeNode.Tag = tag;
                        treeNode.Presenter = ResolveNodePresenterForData(tag);
                        nodePresenter = treeNode.Presenter;
                    }

                    nodePresenter.UpdateNode(treeNode.Parent, treeNode, treeNode.Tag);

                    var childNodeObjects = GetChildNodeObjects(treeNode).ToArray();
                    var count = childNodeObjects.Length;

                    treeNode.HasChildren = count > 0;

                    if (treeNode.Nodes.Count != count)
                    {
                        RefreshChildNodes(treeNode);
                    }
                    else
                    {
                        //update existing nodes
                        for (var i = 0; i < treeNode.Nodes.Count; i++)
                        {
                            UpdateNode(treeNode.Nodes[i], childNodeObjects[i]);
                        }
                    }
                }
                finally
                {
                    treeView.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Refreshes the sub nodes of the treeNode
        /// </summary>
        /// <param name="treeNode">Node for which to refresh the children</param>
        /// <returns>If the tree node is loaded</returns>
        public void RefreshChildNodes(TreeNode treeNode)
        {
            RememberExpandedState(treeNode);

            try
            {
                treeNode.Nodes.Clear();

                var nodePresenter = treeNode.Presenter;
                if (nodePresenter == null)
                {
                    return;
                }

                var childNodeObjects = nodePresenter.GetChildNodeObjects(treeNode.Tag);
                if (childNodeObjects == null)
                {
                    return;
                }

                foreach (object o in childNodeObjects)
                {
                    AddNewNode(treeNode, o);
                }
            }
            finally
            {
                RestoreExpandedState(treeNode);
            }
        }

        private void AddNewNode(TreeNode parentNode, object nodeData, int insertionIndex = -1)
        {
            var newNode = treeView.NewNode();

            if (treeView.CheckBoxes)
            {
                newNode.Checked = parentNode.Checked;
            }

            UpdateNode(parentNode, newNode, nodeData);

            if (insertionIndex != -1)
            {
                parentNode.Nodes.Insert(insertionIndex, newNode);
            }
            else
            {
                parentNode.Nodes.Add(newNode);
            }

            newNode.HasChildren = HasChildren(newNode);
        }

        /// <summary>
        /// Checks if the label of the treeNode can be changed
        /// </summary>
        /// <param name="node">Node to check for</param>
        public bool CanRenameNode(TreeNode node)
        {
            return AskNodePresenter(node, np => np.CanRenameNode(node), false);
        }

        /// <summary>
        /// Checks if the provided node can be deleted
        /// </summary>
        /// <param name="node">Node to check</param>
        public bool CanDeleteNode(TreeNode node)
        {
            return AskNodePresenter(node, np =>
            {
                var parentNodeData = node.Parent != null ? node.Parent.Tag : null;
                return np.CanRemove(parentNodeData, node.Tag);
            }, false);
        }

        public void OnDragDrop(TreeNode nodeDragging, TreeNode parentNode, TreeNode nodeDropTarget, DragOperations dragOperation, int dropAtLocation)
        {
            var nodePresenter = nodeDropTarget.Presenter;
            if (nodePresenter == null)
            {
                return;
            }

            nodePresenter.OnDragDrop(nodeDragging.Tag, parentNode.Tag, nodeDropTarget.Tag, dragOperation, dropAtLocation);
        }

        public void OnTreeViewHandleCreated()
        {
            treeView.Refresh(); // Ensure the treeview is always up to date after creating handle (data is set and might be changed before enabling the delayed event handlers)
        }

        private IEnumerable<TreeNode> GetAllLoadedChildNodes(TreeNode node, string[] forceNodeLoad = null)
        {
            foreach (var childNode in node.Nodes)
            {
                yield return childNode;

                foreach (var childChildNode in GetAllLoadedChildNodes(childNode, forceNodeLoad))
                {
                    yield return childChildNode;
                }
            }
        }

        private void RememberExpandedState(TreeNode node)
        {
            if (updatingExpandedState)
            {
                return;
            }

            updatingExpandedState = true;

            // we need to remember all loaded nodes since after clear / build we have no idea anymore which nodes were loaded 
            var allLoadedNodes = GetAllLoadedChildNodes(node).ToArray();
            loadedNodePaths = allLoadedNodes.Select(n => n.FullPath).ToArray();
            expandedNodePaths = GetAllLoadedChildNodes(node).Where(n => n.IsExpanded).Select(n => n.FullPath).ToArray();
            expandedStateRootNode = node;
        }

        private void RestoreExpandedState(TreeNode node)
        {
            if (!updatingExpandedState || node != expandedStateRootNode)
            {
                return;
            }

            GetAllLoadedChildNodes(node, loadedNodePaths).Where(n => expandedNodePaths.Contains(n.FullPath)).ToArray().ForEachElementDo(n => n.Expand());
            expandedStateRootNode = null;
            updatingExpandedState = false;
        }

        private bool HasChildren(TreeNode treeNode)
        {
            return GetChildNodeObjects(treeNode).Any();
        }

        private IEnumerable<object> GetChildNodeObjects(TreeNode treeNode)
        {
            return (AskNodePresenter(treeNode, p => p.GetChildNodeObjects(treeNode.Tag), null) ??
                    new object[0]).OfType<object>();
        }

        private T AskNodePresenter<T>(TreeNode node, Func<ITreeNodePresenter, T> nodePresenterFunction, T defaultValue)
        {
            if (node == null || node.Tag == null)
            {
                return defaultValue;
            }

            var nodePresenter = node.Presenter;
            if (nodePresenter == null)
            {
                return defaultValue;
            }

            return nodePresenterFunction(nodePresenter);
        }

        private void CreateRootNode()
        {
            var rootNode = new TreeNode(treeView)
            {
                Tag = data
            };

            UpdateNode(null, rootNode, data);

            treeView.Nodes.Add(rootNode);

            if (HasChildren(rootNode))
            {
                rootNode.HasChildren = true;
                rootNode.Expand();
            }
        }

        private ITreeNodePresenter GetNodePresenterForType(Type type, TreeNode parentNode)
        {
            var nodePresentersForType = nodePresenters.Where(p => p.NodeTagType == type).ToList();

            if (!nodePresentersForType.Any() && type.BaseType != null)
            {
                // Walk up the class hierarchy... ignore interfaces
                return GetNodePresenterForType(type.BaseType, parentNode);
            }

            // filter base node presenter types
            var types = nodePresentersForType.Select(np => np.GetType()).ToList();
            var nodePresenterType = types.Except(types.Where(bt => types.Any(t => t.IsSubclassOf(bt)))).FirstOrDefault();

            return nodePresenterType != null
                       ? nodePresentersForType.FirstOrDefault(p => p.GetType() == nodePresenterType)
                       : null;
        }

        private void UpdateNode(TreeNode parentNode, TreeNode node, object nodeData)
        {
            var presenter = node.Presenter;

            if (presenter == null)
            {
                presenter = ResolveNodePresenterForData(nodeData, parentNode);
                node.Presenter = presenter;
            }

            if (presenter == null)
            {
                var message = String.Format(Resources.TreeViewController_UpdateNode_Can_t_find_INodePresenter_for_0_make_sure_you_added_it_to_Presenters_collection_of_a_TreeView, nodeData);

                throw new ArgumentNullException(message);
            }

            node.Tag = nodeData;
            presenter.UpdateNode(parentNode, node, nodeData);
            RefreshChildNodes(node);
        }

        /// <summary>
        /// Delegate required to perform asynchronous calls to <see cref="UpdateNode(TreeNode, object)"/>.
        /// </summary>
        delegate void UpdateNodeInvokeDelegate(TreeNode treeNode, object tag);
    }
}