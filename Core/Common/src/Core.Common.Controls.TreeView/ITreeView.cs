using System;
using System.Collections;
using System.Collections.Generic;
using Core.Common.Controls.Views;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Interface for visual TreeView to display hierarchical data.
    /// </summary>
    public interface ITreeView : IView
    {
        /// <summary>
        /// Event to notify observers of changes in the selection.
        /// </summary>
        event EventHandler SelectedNodeChanged;

        /// <summary>
        /// Event to notify observers of changes in the tree structure.
        /// </summary>
        event EventHandler OnUpdate;

        event Action BeforeWaitUntilAllEventsAreProcessed;

        /// <summary>
        /// All nodes contained in the tree
        /// </summary>
        IList<ITreeNode> Nodes { get; }

        /// <summary>
        /// Currently selected node.
        /// </summary>
        ITreeNode SelectedNode { get; set; }

        /// <summary>
        /// Show / hide check boxes.
        /// </summary>
        bool CheckBoxes { get; set; }

        /// <summary>
        /// Node presenters, sitting between specific data objects and the tree view
        /// </summary>
        IEnumerable<ITreeNodePresenter> NodePresenters { get; }

        IComparer TreeViewNodeSorter { get; set; }

        IEnumerable<ITreeNode> AllLoadedNodes { get; }

        /// <summary>
        /// Registers a node presenter.
        /// </summary>
        /// <param name="presenter">The presenter to be added.</param>
        void RegisterNodePresenter(ITreeNodePresenter presenter);

        /// <summary>
        /// Returns a specific node presenter for the given data object.
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="node"></param>
        ITreeNodePresenter GetTreeViewNodePresenter(object nodeData, ITreeNode node);

        /// <summary>
        /// Creates a new node (not added to the tree yet)
        /// </summary>
        /// <returns></returns>
        ITreeNode NewNode();

        /// <summary>
        /// Creates a new node and adds it to the tree
        /// </summary>
        /// <returns></returns>
        ITreeNode AddNewNode(ITreeNode parentNode, object nodeData, int insertionIndex = -1);

        /// <summary>
        /// Returns the node which tag is set to the given data object
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="skipUnLoadedNodes"></param>
        /// <returns></returns>
        ITreeNode GetNodeByTag(object nodeData, bool skipUnLoadedNodes = true);

        /// <summary>
        /// Refreshes the tree view based on the underlying data.
        /// </summary>
        void Refresh();

        /// <summary>
        /// TODO: move to node
        /// </summary>
        /// <param name="treeNode"></param>
        [Obsolete("YAGNI")]
        void RefreshChildNodes(ITreeNode treeNode);

        void UpdateNode(ITreeNode treeNode);

        /// <summary>
        /// Tree view may perform some updates asynchroneously. 
        /// This method allows to wait until all pending asynchroneous events are processed.
        /// </summary>
        void WaitUntilAllEventsAreProcessed();

        /// <summary>
        /// Collapses all nodes and their child nodes (recursively).
        /// </summary>
        void CollapseAll();

        /// <summary>
        /// Collapses a given node and all its child nodes (recursively).
        /// </summary>
        /// <param name="node"></param>
        void CollapseAll(ITreeNode node);

        /// <summary>
        /// Expands all nodes and their child nodes (recursively).
        /// </summary>
        void ExpandAll();

        /// <summary>
        /// Expands a given node and all its child nodes (recursively).
        /// </summary>
        /// <param name="node"></param>
        void ExpandAll(ITreeNode node);

        /// <summary>
        /// Gets node and all its child nodes (if loaded).
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<ITreeNode> GetAllLoadedNodes(ITreeNode node);

        /// <summary>
        /// Suspend tree view refreshes (long running actions), sets IsUpdateSuspended to true.
        /// </summary>
        void BeginUpdate();

        /// <summary>
        /// Ends updates, sets IsUpdateSuspended to false.
        /// </summary>
        void EndUpdate();

        /// <summary>
        /// Checks if label of current node can be edited and starts edit mode if this is the case.
        /// </summary>
        void StartLabelEdit();

        /// <summary>
        /// Checks if label of the <paramref name="node"/> can be edited and starts edit mode if this is the case.
        /// </summary>
        /// <param name="node">The <see cref="ITreeNode"/> to start editing the label for.</param>
        void StartLabelEdit(ITreeNode node);

        /// <summary>
        /// Attempts to delete the currently selected <see cref="ITreeNode"/> data.
        /// </summary>
        void TryDeleteSelectedNodeData();
        
        /// <summary>
        /// Attempts to delete the given <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <see cref="ITreeNode"/> to try and delete.</param>
        void TryDeleteNodeData(ITreeNode node);
    }
}