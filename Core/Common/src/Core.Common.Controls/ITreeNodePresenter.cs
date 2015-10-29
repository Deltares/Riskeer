using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.Utils.Collections;

namespace Core.Common.Controls
{
    /// <summary>
    /// Defines logic for building treeViewNodes based on a certain node datatype.
    /// </summary>
    public interface ITreeNodePresenter
    {
        /// <summary>
        /// TreeView containing the nodes.
        /// </summary>
        ITreeView TreeView { get; set; }

        ///<summary>
        /// Gets supported type of the object for this builder.
        ///</summary>
        Type NodeTagType { get; }

        /// <summary>
        /// Apply properties to newly created newNode.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="node"></param>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData);

        /// <summary>
        /// Returns array of child objects for <paramref name="parentNodeData"/> for which nodes should be added.
        /// </summary>
        /// <param name="parentNodeData">The data belonging to the parent node specified by <paramref name="node"/></param>
        /// <param name="node">The parent node whose child object should be returned</param>
        /// <returns>
        ///   The collection of child objects.
        ///   Post condition: Shall not return null.
        /// </returns>
        IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node);

        /// <summary>
        /// Indicates whether the node text property is editable.
        /// </summary>
        /// <param name="node">The node to be renamed.</param>
        /// <returns>True if <paramref name="node"/> can be renamed, false otherwise.</returns>
        /// <remarks>Any <see cref="ITreeNodePresenter"/> that can return true with this method, should use <see cref="OnNodeRenamed"/> for renaming logic.</remarks>
        /// <seealso cref="OnNodeRenamed"/>
        bool CanRenameNode(ITreeNode node);

        ///<summary>
        /// Indicates that the can be renamed and newName is a valid name.
        ///</summary>
        ///<param name="node">The node to be renamed.</param>
        ///<param name="newName">Suggested new name for <paramref name="node"/>.</param>
        ///<returns>True if <paramref name="node"/> can be renamed, false otherwise.</returns>
        /// <remarks>Any <see cref="ITreeNodePresenter"/> that can return true with this method, should use <see cref="OnNodeRenamed"/> for renaming logic.</remarks>
        /// <seealso cref="OnNodeRenamed"/>
        bool CanRenameNodeTo(ITreeNode node, string newName);

        /// <summary>
        /// Renames the node
        /// </summary>
        /// <param name="nodeData">Entity to be renamed.</param>
        /// <param name="newName">New name for <paramref name="nodeData"/>.</param>
        /// <seealso cref="CanRenameNode"/>
        /// <seealso cref="CanRenameNodeTo"/>
        void OnNodeRenamed(object nodeData, string newName);

        /// <summary>
        /// When the node has a checkbox, this method handles the data operation for checking/unchecking the node.
        /// </summary>
        /// <param name="node"></param>
        void OnNodeChecked(ITreeNode node);

        /// <summary>
        /// Indicates if a node can be dragged to another location.
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns>
        /// The return value indicates which operations are valid. 
        /// DragOperations can be xored
        /// examples:
        /// folder can be moved and copied
        /// output vars of models can be linked
        /// </returns>
        DragOperations CanDrag(object nodeData);

        ///<summary>
        /// Indicates if a node can be dropped to another location.
        ///</summary>
        ///<param name="item"></param>
        ///<param name="sourceNode"></param>
        ///<param name="targetNode"></param>
        ///<param name="validOperations"></param>
        ///<returns>
        /// The return value indicates what operation is valid when the sourceNode is dropped onto the targetNode.
        /// This can only one of the following values
        /// None, Copy, Link, Move
        /// ControlKeys pressed by the user should also be considered.
        /// examples:
        /// CanDrag: Folder can be moved or copied
        ///          shift key is move
        ///          ctrl key is copy
        ///          alt key is liknk but not supported -> default
        ///          default is move for folder in same project
        ///                  is copy for folder to other project
        /// </returns>
        /// TODO: change item, sourceParentNodeData, targetParentNodeData to ITreeNode!
        DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations);

        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<param name="sourceNode"></param>
        ///<param name="targetNode"></param>
        ///<returns></returns>
        bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode);

        /// <summary>
        /// when a node is dropped onto a target node, this method handles the necessary data operations.
        /// </summary>
        /// <param name="item">Item being dropped</param>
        /// <param name="sourceParentNodeData"></param>
        /// <param name="targetParentNodeData">Target node data where <paramref name="item"/> is being dropped.</param>
        /// <param name="operation"></param>
        /// <param name="position"></param>
        /// TODO: change item, sourceParentNodeData, targetParentNodeData to ITreeNode!
        void OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position);

        /// <summary>
        /// In case a node is selected by the user it might be necessary to notify observers.
        /// </summary>
        /// <param name="nodeData"></param>
        void OnNodeSelected(object nodeData);

        /// <summary>
        /// Returns context menu based on current data
        /// </summary>
        ContextMenuStrip GetContextMenu(ITreeNode sender, object nodeData);

        /// <summary>
        /// Updates node due to it's property change.
        /// </summary>
        void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e);

        /// <summary>
        /// Reflect changes in the collection of items contained by the e.Item to the tree
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments, e.Item contains object which was added / removed</param>
        void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e);

        // TODO: check it we need to replace these methods with OnKeyPressed()
        /// <summary>
        /// Indicates wether a nodeData can be removed from parentNodeData.
        /// </summary>
        /// <param name="parentNodeData"></param>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        bool CanRemove(object parentNodeData, object nodeData);

        /// <summary>
        /// Allow for deletion of node data from within the treeview.
        /// </summary>
        /// <param name="parentNodeData"></param>
        /// <param name="nodeData"></param>
        /// <returns>true if remove operation is processed</returns>
        bool RemoveNodeData(object parentNodeData, object nodeData);
    }
}