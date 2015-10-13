using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using DelftTools.Controls;
using DelftTools.Utils.Collections;

using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// Tree node presenter representing the collection of surfacelines available for piping
    /// calculations.
    /// </summary>
    public class PipingSurfaceLineCollectionNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(IEnumerable<PipingSurfaceLine>);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = Resources.PipingSurfaceLinesCollectionName;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.folder;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var surfaceLines = (IEnumerable<PipingSurfaceLine>)parentNodeData;
            foreach (var pipingSurfaceLine in surfaceLines)
            {
                yield return pipingSurfaceLine;
            }
        }

        public bool CanRenameNode(ITreeNode node)
        {
            return false;
        }

        public bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return false;
        }

        public void OnNodeRenamed(object nodeData, string newName)
        {
            throw new InvalidOperationException(string.Format("Cannot rename tree node of type {0}.", GetType().Name));
        }

        public void OnNodeChecked(ITreeNode node)
        {
            
        }

        public DragOperations CanDrag(object nodeData)
        {
            return DragOperations.None;
        }

        public DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return DragOperations.None;
        }

        public bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return false;
        }

        public void OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position)
        {
            
        }

        public void OnNodeSelected(object nodeData)
        {
            
        }

        public IMenuItem GetContextMenu(ITreeNode sender, object nodeData)
        {
            return null;
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e)
        {
            
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            
        }

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return false;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            throw new InvalidOperationException(String.Format("Cannot delete node of type {0}.", GetType().Name));
        }
    }
}