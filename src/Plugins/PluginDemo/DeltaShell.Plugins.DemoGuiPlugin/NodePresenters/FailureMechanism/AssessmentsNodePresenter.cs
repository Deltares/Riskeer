using System;
using System.Collections;
using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Drawing;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.FailureMechanism;

namespace DeltaShell.Plugins.DemoGuiPlugin.NodePresenters.FailureMechanism
{
    public class AssessmentsNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType { get { return typeof(IEventedList<IAssessment>); } }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = "Assessments";
            node.Image = Properties.Resources.folder.AddOverlayImage(Properties.Resources.gear_small, 5, 1);
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            return (IEventedList<IAssessment>) parentNodeData;
        }

        public bool CanRenameNode(ITreeNode node)
        {
            return false;
        }

        public bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return false;
        }

        public void OnNodeRenamed(object nodeData, string newName) { }

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

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e) { }

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return false;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            return false;
        }
    }
}