using System;
using System.Collections;
using System.ComponentModel;

using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Utils.Collections;

using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    public class WtiProjectNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(WtiProject);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            var wtiProject = (WtiProject)nodeData;
            node.Text = wtiProject.Name;
            node.Image = Resources.WtiProjectFolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            yield break;
        }

        public bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return true;
        }

        public void OnNodeRenamed(object nodeData, string newName)
        {
            var project = (WtiProject)nodeData;
            project.Name = newName;
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
            return true;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            var parentProject = (Project)parentNodeData;
            var wtiProject = (WtiProject)nodeData;

            parentProject.Items.Remove(wtiProject);
            parentProject.NotifyObservers();

            return true;
        }
    }
}