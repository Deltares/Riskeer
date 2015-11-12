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
    /// Node presenter for <see cref="RingtoetsProject"/> items in the tree view.
    /// </summary>
    public class RingtoetsProjectNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(RingtoetsProject);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            var ringtoetsProject = (RingtoetsProject) nodeData;
            node.Text = ringtoetsProject.Name;
            node.Image = Resources.RingtoetsProjectFolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var ringtoetsProject = (RingtoetsProject) parentNodeData;
            if (ringtoetsProject.PipingFailureMechanism != null)
            {
                yield return ringtoetsProject.PipingFailureMechanism;
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
            var project = (RingtoetsProject) nodeData;

            project.Name = newName;
            project.NotifyObservers();
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
            var ringtoetsProject = (RingtoetsProject) nodeData;

            var contextMenu = new ContextMenuStrip();
            if (ringtoetsProject.CanAddPipingFailureMechanism())
            {
                contextMenu.AddMenuItem(Resources.RingtoetsProjectNodePresenter_ContextMenu_Add_PipingFailureMechanism,
                    Resources.RingtoetsProjectNodePresenter_ContextMenu_Add_PipingFailureMechanism_Tooltip,
                    Resources.PipingIcon,
                    InitializePipingFailureMechanismForRingtoetsProject).Tag = nodeData;
            }
            else
            {
                contextMenu.AddMenuItem(Resources.RingtoetsProjectNodePresenter_ContextMenu_Add_PipingFailureMechanism,
                    Resources.RingtoetsProjectNodePresenter_ContextMenu_PipingFailureMechanism_Already_Added_Tooltip,
                    Resources.PipingIcon,
                    InitializePipingFailureMechanismForRingtoetsProject).Enabled = false;
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
            var ringtoetsProject = (RingtoetsProject) nodeData;

            parentProject.Items.Remove(ringtoetsProject);
            parentProject.NotifyObservers();

            return true;
        }

        private void InitializePipingFailureMechanismForRingtoetsProject(object sender, EventArgs e)
        {
            var treeNode = (ToolStripItem) sender;
            if (treeNode != null)
            {
                var ringtoetsProject = (RingtoetsProject) treeNode.Tag;
                ringtoetsProject.InitializePipingFailureMechanism();
                ringtoetsProject.NotifyObservers();
            }
        }
    }
}