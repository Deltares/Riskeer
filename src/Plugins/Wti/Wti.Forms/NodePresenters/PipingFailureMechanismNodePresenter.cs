using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Utils.Collections;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    public class PipingFailureMechanismNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(PipingFailureMechanism);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = Resources.PipingFailureMechanismDisplayName;
            node.Image = Resources.PipingIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            PipingFailureMechanism failureMechanism = (PipingFailureMechanism) parentNodeData;
            yield return failureMechanism.SoilProfiles;
            yield return failureMechanism.SurfaceLines;

            if (failureMechanism.PipingData != null)
            {
                yield return failureMechanism.PipingData;
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
            return nodeData is PipingFailureMechanism;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            if (nodeData is PipingFailureMechanism)
            {
                var wtiProject = (WtiProject)parentNodeData;

                wtiProject.ClearPipingFailureMechanism();
                wtiProject.NotifyObservers();

                return true;
            }
            return false;
        }
    }
}