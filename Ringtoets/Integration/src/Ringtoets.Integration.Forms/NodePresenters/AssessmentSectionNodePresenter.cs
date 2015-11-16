using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Utils.Collections;

using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Integration.Data;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="Ringtoets.Integration.Data.DikeAssessmentSection"/> items in the tree view.
    /// </summary>
    public class AssessmentSectionNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(DikeAssessmentSection);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            var assessmentSection = (DikeAssessmentSection)nodeData;
            node.Text = assessmentSection.Name;
            node.Image = RingtoetsFormsResources.AssessmentSectionFolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var assessmentSection = (DikeAssessmentSection)parentNodeData;
            if (assessmentSection.PipingFailureMechanism != null)
            {
                yield return assessmentSection.PipingFailureMechanism;
            }
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
            var assessmentSection = (DikeAssessmentSection)nodeData;

            assessmentSection.Name = newName;
            assessmentSection.NotifyObservers();
        }

        public void OnNodeChecked(ITreeNode node) {}

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

        public void OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position) {}

        public void OnNodeSelected(object nodeData) {}

        public ContextMenuStrip GetContextMenu(ITreeNode sender, object nodeData)
        {
            return null;
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e) {}

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e) {}

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return true;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            var parentProject = (Project) parentNodeData;
            var assessmentSection = (DikeAssessmentSection) nodeData;

            parentProject.Items.Remove(assessmentSection);
            parentProject.NotifyObservers();

            return true;
        }
    }
}