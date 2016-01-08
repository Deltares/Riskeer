using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.Utils.Events;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Defines logic for building treeViewNodes based on a certain node datatype.
    /// </summary>
    public interface ITreeNodePresenter
    {
        /// <summary>
        /// TreeView containing the nodes.
        /// </summary>
        TreeView TreeView { get; set; }

        /// <summary>
        /// Gets supported type of the object for this builder.
        /// </summary>
        Type NodeTagType { get; }

        /// <summary>
        /// Apply properties to newly created newNode.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="node"></param>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        void UpdateNode(TreeNode parentNode, TreeNode node, object nodeData);

        /// <summary>
        /// Returns array of child objects for <paramref name="parentNodeData"/> for which nodes should be added.
        /// </summary>
        /// <param name="parentNodeData">The data belonging to the parent node.</param>
        /// <returns>
        ///   The collection of child objects.
        ///   Post condition: Shall not return null.
        /// </returns>
        IEnumerable GetChildNodeObjects(object parentNodeData);

        /// <summary>
        /// Indicates whether the node text property is editable.
        /// </summary>
        /// <param name="node">The node to be renamed.</param>
        /// <returns>True if <paramref name="node"/> can be renamed, false otherwise.</returns>
        /// <remarks>Any <see cref="ITreeNodePresenter"/> that can return true with this method, should use <see cref="OnNodeRenamed"/> for renaming logic.</remarks>
        /// <seealso cref="OnNodeRenamed"/>
        bool CanRenameNode(TreeNode node);

        /// <summary>
        /// Indicates that the can be renamed and newName is a valid name.
        /// </summary>
        /// <param name="node">The node to be renamed.</param>
        /// <param name="newName">Suggested new name for <paramref name="node"/>.</param>
        /// <returns>True if <paramref name="node"/> can be renamed, false otherwise.</returns>
        /// <remarks>Any <see cref="ITreeNodePresenter"/> that can return true with this method, should use <see cref="OnNodeRenamed"/> for renaming logic.</remarks>
        /// <seealso cref="OnNodeRenamed"/>
        bool CanRenameNodeTo(TreeNode node, string newName);

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
        void OnNodeChecked(TreeNode node);

        /// <summary>
        /// Indicates if a node can be dragged to another location.
        /// </summary>
        /// <param name="nodeData">The data contained within the dragged node.</param>
        /// <returns> The return value indicates which operation is valid.</returns>
        DragOperations CanDrag(object nodeData);

        /// <summary>
        /// Indicates if a node can be dropped to another location.
        /// </summary>
        /// <param name="item">The data-object being dragged.</param>
        /// <param name="sourceNode">The node corresponding with <paramref name="item"/>.</param>
        /// <param name="targetNode">The node being considered as drop target for <paramref name="item"/>.</param>
        /// <param name="validOperations">The supported drop operations of <paramref name="item"/>.</param>
        /// <returns>The return value indicates what operation is valid when the sourceNode is dropped onto the targetNode.
        /// This can only one of the following values
        /// None, Move
        /// ControlKeys pressed by the user should also be considered.
        /// examples:
        /// CanDrag: Folder can be moved
        ///          shift key is move
        ///          default is move for folder in same project
        /// </returns>
        /// <remarks>This method should be called for the node presenter of <paramref name="targetNode"/>.</remarks>
        /// TODO: change item, sourceParentNodeData, targetParentNodeData to TreeNode!
        DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations);

        /// <summary>
        /// Checks if insertion at a specific index into the target is allowed.
        /// </summary>
        /// <param name="item">The data-object being dragged.</param>
        /// <param name="sourceNode">The node corresponding with <paramref name="item"/>.</param>
        /// <param name="targetNode">The node being considered as drop target for <paramref name="item"/>.</param>
        /// <returns>True if insertion is supported; false otherwise.</returns>
        bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode);

        /// <summary>
        /// Handles dropping a piece of data onto another node.
        /// </summary>
        /// <param name="item">The data-object being dropped.</param>
        /// <param name="itemParent">The owner of <paramref name="item"/>.</param>
        /// <param name="target">The data-object onto which <paramref name="item"/> is being dropped.</param>
        /// <param name="operation">The type of drag operation performed.</param>
        /// <param name="position"></param>
        /// <remarks>This method should be called for the node presenter of the node corresponding
        /// with <paramref name="target"/>.</remarks>
        /// TODO: change item, itemParent, target to TreeNode!
        void OnDragDrop(object item, object itemParent, object target, DragOperations operation, int position);

        /// <summary>
        /// In case a node is selected by the user it might be necessary to notify observers.
        /// </summary>
        /// <param name="nodeData"></param>
        void OnNodeSelected(object nodeData);

        /// <summary>
        /// Returns context menu based on current data
        /// </summary>
        ContextMenuStrip GetContextMenu(TreeNode node, object nodeData);

        /// <summary>
        /// Updates node due to it's property change.
        /// </summary>
        void OnPropertyChanged(object sender, TreeNode node, PropertyChangedEventArgs e);

        /// <summary>
        /// Reflect changes in the collection of items contained by the e.Item to the tree
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments, e.Item contains object which was added / removed</param>
        void OnCollectionChanged(object sender, NotifyCollectionChangeEventArgs e);

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