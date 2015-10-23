﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using log4net;

namespace DelftTools.Controls.Swf.TreeViewControls
{
    public class TreeViewController : IDisposable
    {
        public static bool SkipEvents; // for debugging purposes, find a better way (see usage)
        private static readonly ILog Log = LogManager.GetLogger(typeof(TreeViewController));

        private readonly ITreeView treeView;
        private readonly IEventedList<ITreeNodePresenter> nodePresenters = new EventedList<ITreeNodePresenter>();

        // use timer in order to combine property and collection change
        // note that timer is active only when there is a full refresh taking place
        private readonly Timer fullRefreshTimer = new Timer
        {
            Interval = 250
        };

        private readonly Dictionary<Type, ITreeNodePresenter> nodeTagTypePresenters = new Dictionary<Type, ITreeNodePresenter>();
        private object data;

        private bool updatingExpandedState; // prevents recursive entries
        private ITreeNode expandedStateRootNode;
        private string[] loadedNodePaths;
        private string[] expandedNodePaths;

        public TreeViewController(ITreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentException("Tree view can't be null");
            }

            this.treeView = treeView;

            nodePresenters.CollectionChanged += NodePresentersCollectionChanged;

            // for a time being we use timer here to perform full refresh to avoid double refresh
            fullRefreshTimer.Tick += (sender, args) =>
            {
                fullRefreshTimer.Stop();
                treeView.Refresh();
            };
        }

        /// <summary>
        /// List of registered node presenters
        /// </summary>
        public IEventedList<ITreeNodePresenter> NodePresenters
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
                if (data != null)
                {
                    DisableDataEventListeners();
                }

                treeView.Nodes.Clear();
                data = value;

                if (data == null)
                {
                    return;
                }

                CreateRootNode();
                EnableDataEventListeners();

                treeView.SelectedNode = treeView.Nodes.Count > 0 ? treeView.Nodes[0] : null;
            }
        }

        public void WaitUntilAllEventsAreProcessed()
        {
            while (treeView.IsUpdateSuspended || (treeView.Visible && fullRefreshTimer.Enabled))
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Gets the most specific node presenter for a piece of data. Does so by walking up the class hierarchy and trying to find an exact match
        /// It is in the helper to allow heavy testing since this is complex logic.
        /// </summary>
        /// <param name="item">Object to search a node presenter for</param>
        /// <param name="parentNode">Parent node of the node to resolve</param>
        public ITreeNodePresenter ResolveNodePresenterForData(object item, ITreeNode parentNode = null)
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
                            NodePresenters.FirstOrDefault(np => np.NodeTagType.IsInstanceOfType(item));

                nodeTagTypePresenters[type] = presenter;
            }

            return presenter;
        }

        /// <summary>
        /// Action to perform when the node is checked/unchecked
        /// </summary>
        /// <param name="node">Node that is checked/unchecked</param>
        public void OnNodeChecked(ITreeNode node)
        {
            if (node.IsUpdating)
            {
                return;
            }

            if (node.Tag == null)
            {
                throw new InvalidOperationException("Unable to resolve node presenter for null data");
            }

            node.Presenter.OnNodeChecked(node);
        }

        public void UpdateNode(ITreeNode treeNode)
        {
            UpdateNode(treeNode, treeNode.Tag);
        }

        /// <summary>
        /// Updates the node and if loaded the sub nodes
        /// </summary>
        /// <param name="treeNode">Node to update</param>
        /// <param name="tag">the object bound to this node</param>
        public void UpdateNode(ITreeNode treeNode, object tag)
        {
            var treeViewControl = treeView as Control;
            if (treeViewControl != null && treeViewControl.InvokeRequired)
            {
                UpdateNodeInvokeDelegate updateNode = UpdateNode;
                treeViewControl.Invoke(updateNode, treeNode, tag);
            }
            else
            {
                var suspend = false; // suspend tree view locally

                if (!treeView.IsUpdateSuspended)
                {
                    treeView.BeginUpdate();
                    suspend = true;
                }
                try
                {
                    var nodePresenter = treeNode.Presenter;

                    if (nodePresenter == null)
                    {
                        Log.Debug("Can't find node presenter for tree view, object:" + tag);
                        return;
                    }

                    bool wasLoaded = treeNode.IsLoaded;
                    if (!ReferenceEquals(treeNode.Tag, tag))
                    {
                        treeNode.Tag = tag;
                        treeNode.Presenter = ResolveNodePresenterForData(tag);
                        nodePresenter = treeNode.Presenter;
                    }

                    nodePresenter.UpdateNode(treeNode.Parent, treeNode, treeNode.Tag);

                    var childNodeObjects = GetChildNodeObjects(treeNode).ToArray();
                    var count = childNodeObjects.Length;

                    ((TreeNode)treeNode).HasChildren = count > 0;

                    if (!treeNode.IsLoaded && !wasLoaded)
                    {
                        return;
                    }

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
                    if (suspend)
                    {
                        treeView.EndUpdate();
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the sub nodes of the treeNode
        /// </summary>
        /// <param name="treeNode">Node for which to refresh the children</param>
        /// <returns>If the tree node is loaded</returns>
        public void RefreshChildNodes(ITreeNode treeNode)
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

                var childNodeObjects = nodePresenter.GetChildNodeObjects(treeNode.Tag, treeNode);
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

        public ITreeNode AddNewNode(ITreeNode parentNode, object nodeData, int insertionIndex = -1)
        {
            var newNode = treeView.NewNode();

            newNode.Tag = nodeData;

            if (treeView.CheckBoxes)
            {
                newNode.Checked = parentNode.Checked;
            }

            UpdateNode(parentNode, newNode, nodeData);

            //if (newNode.IsVisible)
            {
                if (insertionIndex != -1)
                {
                    parentNode.Nodes.Insert(insertionIndex, newNode);
                }
                else
                {
                    parentNode.Nodes.Add(newNode);
                }
            }

            ((TreeNode) newNode).HasChildren = HasChildren(newNode);
            return newNode;
        }

        /// <summary>
        /// Checks if the label of the treeNode can be changed
        /// </summary>
        /// <param name="node">Node to check for</param>
        public bool CanRenameNode(ITreeNode node)
        {
            return AskNodePresenter(node, np => np.CanRenameNode(node), false);
        }

        /// <summary>
        /// Checks if the provided node can be deleted
        /// </summary>
        /// <param name="node">Node to check</param>
        public bool CanDeleteNode(ITreeNode node)
        {
            return AskNodePresenter(node, np =>
            {
                var parentNodeData = node.Parent != null ? node.Parent.Tag : null;
                return np.CanRemove(parentNodeData, node.Tag);
            }, false);
        }

        public void OnDragDrop(ITreeNode nodeDragging, ITreeNode parentNode, ITreeNode nodeDropTarget, DragOperations dragOperation, int dropAtLocation)
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

        public void OnTreeViewHandleDestroyed()
        {
            fullRefreshTimer.Stop();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            DisableDataEventListeners();
        }

        #endregion

        internal void EnableDataEventListeners()
        {
            var notifyPropertyChanged = data as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                (notifyPropertyChanged).PropertyChanged += DataPropertyChanged;
            }

            var notifyCollectionChange = data as INotifyCollectionChange;
            if (notifyCollectionChange != null)
            {
                (notifyCollectionChange).CollectionChanged += DataCollectionChanged;
            }
        }

        internal void DisableDataEventListeners()
        {
            var notifyPropertyChanged = data as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                (notifyPropertyChanged).PropertyChanged -= DataPropertyChanged;
            }

            var notifyCollectionChange = data as INotifyCollectionChange;
            if (notifyCollectionChange != null)
            {
                (notifyCollectionChange).CollectionChanged -= DataCollectionChanged;
            }
        }

        private void FullRefreshEventHandler(object sender, EventArgs eventArgs)
        {
            if (fullRefreshTimer.Enabled)
            {
                fullRefreshTimer.Stop(); // Reset timer when already started before
            }

            fullRefreshTimer.Start(); // schedule full refresh
        }

        private IEnumerable<ITreeNode> GetAllLoadedChildNodes(ITreeNode node, string[] forceNodeLoad = null)
        {
            if (!node.IsLoaded && (forceNodeLoad == null || !forceNodeLoad.Contains(node.FullPath)))
            {
                yield break;
            }

            foreach (var childNode in node.Nodes)
            {
                yield return childNode;

                foreach (var childChildNode in GetAllLoadedChildNodes(childNode, forceNodeLoad))
                {
                    yield return childChildNode;
                }
            }
        }

        private void RememberExpandedState(ITreeNode node)
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

        private void RestoreExpandedState(ITreeNode node)
        {
            if (!updatingExpandedState || node != expandedStateRootNode)
            {
                return;
            }

            GetAllLoadedChildNodes(node, loadedNodePaths).Where(n => expandedNodePaths.Contains(n.FullPath)).ToArray().ForEach(n => n.Expand());
            expandedStateRootNode = null;
            updatingExpandedState = false;
        }

        private bool HasChildren(ITreeNode treeNode)
        {
            return GetChildNodeObjects(treeNode).Any();
        }

        private IEnumerable<object> GetChildNodeObjects(ITreeNode treeNode)
        {
            return (AskNodePresenter(treeNode, p => p.GetChildNodeObjects(treeNode.Tag, treeNode), null) ??
                    new object[0]).OfType<object>();
        }

        private T AskNodePresenter<T>(ITreeNode node, Func<ITreeNodePresenter, T> nodePresenterFunction, T defaultValue)
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

        private void DataCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (SkipEvents)
            {
                return;
            }

            DataCollectionChangedSynchronized(sender, e);

            FullRefreshEventHandler(sender, e);
        }

        private void DataCollectionChangedSynchronized(object sender, NotifyCollectionChangingEventArgs e)
        {
            var nodePresenter = ResolveNodePresenterForData(e.Item);
            if (nodePresenter == null)
            {
                return;
            }

            nodePresenter.OnCollectionChanged(sender, e);
        }

        private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SkipEvents)
            {
                return;
            }

            var nodePresenter = ResolveNodePresenterForData(sender);

            if (nodePresenter == null)
            {
                return;
            }
            var node = treeView.GetNodeByTag(sender);

            // HACK: bug in WaterFlowModel1DBoundaryDataNodeDataNodePresenter, in some cases event somes with a sender which is child of actual node.Tag object - find out how to fix it
            var actualSender = sender;
            if (node != null && sender != node.Tag)
            {
                actualSender = node.Tag;
            }

            nodePresenter.OnPropertyChanged(actualSender, node, e);

            FullRefreshEventHandler(sender, e);
        }

        private void CreateRootNode()
        {
            var rootNode = new TreeNode(treeView)
            {
                Tag = data
            };

            treeView.Nodes.Add(rootNode);

            UpdateNode(null, rootNode, data);

            if (HasChildren(rootNode))
            {
                rootNode.HasChildren = true;
                rootNode.Expand();
            }
        }

        private ITreeNodePresenter GetNodePresenterForType(Type type, ITreeNode parentNode)
        {
            var nodePresentersForType = NodePresenters.Where(p => p.NodeTagType == type).ToList();

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

        private void NodePresentersCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (e.Action != NotifyCollectionChangeAction.Add)
            {
                return;
            }

            ((ITreeNodePresenter) e.Item).TreeView = treeView;
        }

        private void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            var presenter = node.Presenter;

            if (presenter == null)
            {
                presenter = ResolveNodePresenterForData(nodeData, parentNode);
                node.Presenter = presenter;
            }

            if (presenter == null)
            {
                var message = String.Format("Can't find INodePresenter for {0}, " +
                                            "make sure you added it to Presenters collection of a TreeView", nodeData);

                throw new ArgumentNullException(message);
            }

            node.Tag = nodeData;
            presenter.UpdateNode(parentNode, node, nodeData);
        }

        /// <summary>
        /// Delegate required to perform asynchronous calls to <see cref="UpdateNode(ITreeNode, object)"/>.
        /// </summary>
        delegate void UpdateNodeInvokeDelegate(ITreeNode treeNode, object tag);
    }
}