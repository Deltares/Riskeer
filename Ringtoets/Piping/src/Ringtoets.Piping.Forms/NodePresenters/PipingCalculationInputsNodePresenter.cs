using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.BaseDelftTools.Workflow;
using Core.Common.Controls;
using Core.Common.Utils.Collections;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Extensions;
using Ringtoets.Piping.Service;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingData"/> as a node in a <see cref="ITreeView"/> and
    /// implements the way the user can interact with the node.
    /// </summary>
    public class PipingCalculationInputsNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(PipingCalculationInputs);
            }
        }

        /// <summary>
        /// Injection points for a method to cause an <see cref="IActivity"/> to be scheduled for execution.
        /// </summary>
        public Action<IActivity> RunActivityAction { private get; set; }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = ((PipingCalculationInputs)nodeData).PipingData.Name;
            node.Image = Resources.PipingIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var pipingData = ((PipingCalculationInputs) parentNodeData).PipingData.Output;
            if (pipingData != null)
            {
                yield return pipingData;
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
            var pipingCalculationInputs = ((PipingCalculationInputs)nodeData);
            pipingCalculationInputs.PipingData.Name = newName;
            pipingCalculationInputs.PipingData.NotifyObservers();
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
            PipingData pipingData = ((PipingCalculationInputs) nodeData).PipingData;

            var contextMenu = new ContextMenuStrip();
            contextMenu.AddMenuItem(Resources.Validate,
                                    null,
                                    null,
                                    (o, args) =>
                                    {
                                        PipingCalculationService.Validate(pipingData);
                                    });
            contextMenu.AddMenuItem(Resources.Calculate,
                                    null,
                                    Resources.Play,
                                    (o, args) =>
                                    {
                                        RunActivityAction(new PipingCalculationActivity(pipingData));
                                    });

            return contextMenu;
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
    }
}