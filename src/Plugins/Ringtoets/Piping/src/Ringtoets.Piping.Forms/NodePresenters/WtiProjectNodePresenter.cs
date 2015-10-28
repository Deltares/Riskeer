using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Utils.Collections;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Extensions;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter for <see cref="WtiProject"/> items in the tree view.
    /// </summary>
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
            var wtiProject = (WtiProject) nodeData;
            node.Text = wtiProject.Name;
            node.Image = Resources.WtiProjectFolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var wtiProject = (WtiProject) parentNodeData;
            if (wtiProject.PipingFailureMechanism != null)
            {
                yield return wtiProject.PipingFailureMechanism;
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
            var project = (WtiProject) nodeData;

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
            var wtiProject = (WtiProject) nodeData;

            var contextMenu = new ContextMenuStrip();
            if (wtiProject.CanAddPipingFailureMechanism())
            {
                contextMenu.AddMenuItem(Resources.AddPipingFailureMechanismContextMenuItem,
                    Resources.WtiProjectTooltipAddPipingFailureMechanism,
                    Resources.PipingIcon,
                    InitializePipingFailureMechanismForWtiProject).Tag = nodeData;
            }
            else
            {
                contextMenu.AddMenuItem(Resources.AddPipingFailureMechanismContextMenuItem,
                    Resources.WtiProjectTooltipPipingFailureMechanismAlreadyAdded,
                    Resources.PipingIcon,
                    InitializePipingFailureMechanismForWtiProject).Enabled = false;
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
            var wtiProject = (WtiProject) nodeData;

            parentProject.Items.Remove(wtiProject);
            parentProject.NotifyObservers();

            return true;
        }

        private void InitializePipingFailureMechanismForWtiProject(object sender, EventArgs e)
        {
            var treeNode = (ToolStripItem) sender;
            if (treeNode != null)
            {
                var wtiProject = (WtiProject) treeNode.Tag;
                wtiProject.InitializePipingFailureMechanism();
                wtiProject.NotifyObservers();
            }
        }
    }
}