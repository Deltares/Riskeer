using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DelftTools.Controls
{
    /// <summary>
    /// Interface for tree nodes
    /// </summary>
    public interface ITreeNode : IDisposable
    {
        /// <summary>
        /// Returns the object bound to this node
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// Returns the label for this node
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// The icon of this node
        /// </summary>
        Image Image { get; set; }

        /// <summary>
        /// Gets the shortcut menu associated with this tree node.
        /// </summary>
        ContextMenuStrip ContextMenuStrip { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a check box is displayed next to the node
        /// </summary>
        bool ShowCheckBox { get; set; }

        ///<summary>
        /// Gets or sets a value indicating whether the tree node is in a checked state
        ///</summary>
        bool Checked { get; set; }

        /// <summary>
        /// Node can be hidden in the tree view
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Font bold setting
        /// </summary>
        bool Bold { get; set; }

        /// <summary>
        /// Background color of tree node
        /// </summary>
        Color BackgroundColor { set; get; }

        /// <summary>
        /// Font color of tree node.
        /// </summary>
        Color ForegroundColor { set; get; }

        ///<summary>
        /// The tree view that holds the node
        ///</summary>
        ITreeView TreeView { get; }

        /// <summary>
        /// Node presenter used to visualize this node.
        /// </summary>
        ITreeNodePresenter Presenter { get; set; }

        /// <summary>
        /// Returns a list of child nodes for this node
        /// </summary>
        IList<ITreeNode> Nodes { get; }

        /// <summary>
        /// Parent node of the current node
        /// </summary>
        ITreeNode Parent { get; }

        /// <summary>
        /// Indicates whether child nodes have been created
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Drawing bounds of node
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        /// Gets the path from the root node to this node
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// Previous sibling tree node
        /// </summary>
        ITreeNode PreviousNode { get; }

        /// <summary>
        /// Previous visible tree node
        /// </summary>
        ITreeNode PreviousVisibleNode { get; }

        /// <summary>
        /// Next sibling tree node
        /// </summary>
        ITreeNode NextNode { get; }

        /// <summary>
        /// Next visible tree node
        /// </summary>
        ITreeNode NextVisibleNode { get; }

        /// <summary>
        /// Gets the zero-based depth of the tree node in the TreeView control
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Gets a value indicating whether the tree node is in the expanded state
        /// </summary>
        bool IsExpanded { get; }

        bool IsUpdating { get; }

        bool IsEditing { get; }

        /// <summary>
        /// Expands the tree node
        /// </summary>
        void Expand();

        /// <summary>
        /// Collapses the tree node
        /// </summary>
        void Collapse(bool ignoreChildren = true);

        /// <summary>
        /// Updates node and all it's sub-nodes based on their tag properties
        /// 
        /// <remarks>
        /// Use it only in case if you tag objects do not implement INotifyPropertyChanged interface. 
        /// When object in tag of the root node implements this interface - tree node will call ITreeViewNodePresenter.OnNotifyPropertyChanged()
        /// </remarks>
        /// </summary>
        /// TODO: rename to Refresh(bool refreshChildren = true)
        void Update();

        /// <summary>
        /// Initiates the editing of the tree node label
        /// </summary>
        void BeginEdit();

        /// <summary>
        /// Returns child node of the current node by tag
        /// </summary>
        /// <param name="item"></param>
        ITreeNode GetNodeByTag(object item);

        /// <summary>
        /// Ensures that the tree node is visible, expanding tree nodes and scrolling the tree view control as necessary
        /// </summary>
        void ScrollTo();

        /// <summary>
        /// Gets the parent of the node on the supplied (nesting) level 
        /// </summary>
        /// <param name="level">Nesting level</param>
        ITreeNode GetParentOfLevel(int level);

        void ShowContextMenu(Point location);

        void EnsureVisible();
    }
}