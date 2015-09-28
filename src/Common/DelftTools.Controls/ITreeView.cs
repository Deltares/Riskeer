using System;
using System.Collections;
using System.Collections.Generic;

namespace DelftTools.Controls
{
    /// <summary>
    /// Interface for visual TreeView to display hierarchical data.
    /// </summary>
    public interface ITreeView: IView 
    {
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
        /// Event to notify observers of changes in the selection
        /// </summary>
        event EventHandler SelectedNodeChanged;

        /// <summary>
        /// Node presenters, sitting between specific data objects and the tree view
        /// </summary>
        ICollection<ITreeNodePresenter> NodePresenters { get; }

        /// <summary>
        /// Returns a specific node presenter for the given data object.
        /// </summary>
        /// <param name="nodeData"></param>
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

        IComparer TreeViewNodeSorter { get; set; }

        IEnumerable<ITreeNode> AllLoadedNodes { get; }

        void UpdateNode(ITreeNode treeNode);

        /// <summary>
        /// Tree view may perform some updates asynchroneously. 
        /// This method allows to wait until all pending asynchroneous events are processed.
        /// </summary>
        void WaitUntilAllEventsAreProcessed();

        event Action BeforeWaitUntilAllEventsAreProcessed;

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
        /// Suspend tree view refreshes (long running actions), sets <see cref="IsUpdateSuspended"/> to true.
        /// </summary>
        void BeginUpdate();

        /// <summary>
        /// Ends updates, sets <see cref="IsUpdateSuspended"/> to false.
        /// </summary>
        void EndUpdate();

        /// <summary>
        /// Indicates if updates are suspended (rendering, layout).
        /// </summary>
        bool IsUpdateSuspended { get; }

        /// <summary>
        /// Checks if label of current node can be edited and starts edit mode if this is the case.
        /// </summary>
        void StartLabelEdit();

        void DeleteNodeData();
    }
}
