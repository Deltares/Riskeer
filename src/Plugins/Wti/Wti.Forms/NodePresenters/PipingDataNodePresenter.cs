using System;
using System.Collections;
using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using DelftTools.Utils.Collections;
using Wti.Data;
using Wti.Forms.Properties;
using Wti.Service;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingData"/> as a node in a <see cref="ITreeView"/> and
    /// implements the way the user can interact with the node.
    /// </summary>
    public class PipingDataNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(PipingData);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = Resources.PipingDataDisplayName;
            node.Image = Resources.PipingIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var pipingData = ((PipingData) parentNodeData).Output;
            if (pipingData != null)
            {
                yield return pipingData;
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

        public IMenuItem GetContextMenu(ITreeNode sender, object nodeData)
        {
            var contextMenu = new PipingContextMenuStrip((PipingData) nodeData);
            var contextMenuAdapter = new MenuItemContextMenuStripAdapter(contextMenu);

            contextMenu.OnCalculationClick += PerformPipingCalculation;
            contextMenu.OnValidationClick += PerformPipingValidation;

            return contextMenuAdapter;
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e) {}

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e) {}

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return false;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            throw new InvalidOperationException(String.Format("Cannot delete node of type {0}.", GetType().Name));
        }

        private void PerformPipingValidation(PipingData pipingData)
        {
            PipingCalculationService.Validate(pipingData);
        }

        private void PerformPipingCalculation(PipingData pipingData)
        {
            PipingCalculationService.PerfromValidatedCalculation(pipingData);
            pipingData.NotifyObservers();
        }
    }
}