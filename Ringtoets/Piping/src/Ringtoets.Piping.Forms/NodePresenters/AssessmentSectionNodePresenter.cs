using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Utils.Collections;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Extensions;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="AssessmentSection"/> items in the tree view.
    /// </summary>
    public class AssessmentSectionNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(AssessmentSection);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            var assessmentSection = (AssessmentSection) nodeData;
            node.Text = assessmentSection.Name;
            node.Image = Resources.AssessmentSectionFolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var assessmentSection = (AssessmentSection) parentNodeData;
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
            var assessmentSection = (AssessmentSection) nodeData;

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
            var assessmentSection = (AssessmentSection) nodeData;

            var contextMenu = new ContextMenuStrip();
            if (assessmentSection.CanAddPipingFailureMechanism())
            {
                contextMenu.AddMenuItem(Resources.AssessmentSectionNodePresenter_ContextMenu_Add_PipingFailureMechanism,
                    Resources.AssessmentSectionNodePresenter_ContextMenu_Add_PipingFailureMechanism_Tooltip,
                    Resources.PipingIcon,
                    InitializePipingFailureMechanismForAssessmentSection).Tag = nodeData;
            }
            else
            {
                contextMenu.AddMenuItem(Resources.AssessmentSectionNodePresenter_ContextMenu_Add_PipingFailureMechanism,
                    Resources.AssessmentSectionNodePresenter_ContextMenu_PipingFailureMechanism_Already_Added_Tooltip,
                    Resources.PipingIcon,
                    InitializePipingFailureMechanismForAssessmentSection).Enabled = false;
            }

            return contextMenu;
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
            var assessmentSection = (AssessmentSection) nodeData;

            parentProject.Items.Remove(assessmentSection);
            parentProject.NotifyObservers();

            return true;
        }

        private void InitializePipingFailureMechanismForAssessmentSection(object sender, EventArgs e)
        {
            var treeNode = (ToolStripItem) sender;
            if (treeNode != null)
            {
                var assessmentSection = (AssessmentSection) treeNode.Tag;
                assessmentSection.InitializePipingFailureMechanism();
                assessmentSection.NotifyObservers();
            }
        }
    }
}