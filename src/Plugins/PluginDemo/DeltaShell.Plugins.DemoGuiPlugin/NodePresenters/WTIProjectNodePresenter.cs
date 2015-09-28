using System;
using System.Collections;
using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Utils.Collections;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects;

namespace DeltaShell.Plugins.DemoGuiPlugin.NodePresenters
{
    public class WTIProjectNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType { get { return typeof (WTIProject); } }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = ((WTIProject) nodeData).Name;
            node.Image = Properties.Resources.projection_screen;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var wtiProject = (WTIProject) parentNodeData;

            yield return wtiProject.ReferenceLine;
            yield return wtiProject.HydraulicBoundariesDatabase;
            yield return wtiProject.Assessments;
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
            var wtiProject = (WTIProject)nodeData;
            wtiProject.Name = newName;
            wtiProject.NotifyObservers();
        }

        public void OnNodeChecked(ITreeNode node) { }

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

        public void OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position) { }

        public void OnNodeSelected(object nodeData) { }

        public IMenuItem GetContextMenu(ITreeNode sender, object nodeData)
        {
            return null;
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e) { }

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e) {  }

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return true;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            var project = parentNodeData as Project;
            if (project != null)
            {
                project.Items.Remove(nodeData);
                project.NotifyObservers();

                return true;
            }

            return false;
        }
    }
}