using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.Service;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;

using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Service;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter class for <see cref="PipingCalculationGroup"/> instances.
    /// </summary>
    public class PipingCalculationGroupContextNodePresenter : RingtoetsNodePresenterBase<PipingCalculationGroupContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="EmptyPipingCalculationReportNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PipingCalculationGroupContextNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}

        /// <summary>
        /// Injection points for a method to cause an <see cref="Activity"/> to be scheduled for execution.
        /// </summary>
        public Action<IEnumerable<Activity>> RunActivitiesAction { private get; set; }

        public override DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            if (GetAsIPipingCalculationItem(item) != null && NodesHaveSameParentFailureMechanism(sourceNode, targetNode))
            {
                return validOperations;
            }

            return base.CanDrop(item, sourceNode, targetNode, validOperations);
        }

        public override bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return GetAsIPipingCalculationItem(item) != null && NodesHaveSameParentFailureMechanism(sourceNode, targetNode);
        }

        public override bool CanRenameNode(ITreeNode node)
        {
            return node.Parent == null || !(node.Parent.Tag is PipingFailureMechanism);
        }

        public override bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return true;
        }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PipingCalculationGroupContext nodeData)
        {
            node.Text = nodeData.WrappedData.Name;
            node.Image = PipingFormsResources.FolderIcon;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        protected override void OnNodeRenamed(PipingCalculationGroupContext nodeData, string newName)
        {
            nodeData.WrappedData.Name = newName;
            nodeData.NotifyObservers();
        }

        protected override bool CanRemove(object parentNodeData, PipingCalculationGroupContext nodeData)
        {
            var group = parentNodeData as PipingCalculationGroupContext;
            if (group != null)
            {
                return group.WrappedData.Children.Contains(nodeData.WrappedData);
            }

            return base.CanRemove(parentNodeData, nodeData);
        }

        protected override bool RemoveNodeData(object parentNodeData, PipingCalculationGroupContext nodeData)
        {
            var group = parentNodeData as PipingCalculationGroupContext;
            if (group != null)
            {
                var removeNodeData = group.WrappedData.Children.Remove(nodeData.WrappedData);
                group.NotifyObservers();
                return removeNodeData;
            }

            return base.RemoveNodeData(parentNodeData, nodeData);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, PipingCalculationGroupContext nodeData)
        {
            var group = nodeData.WrappedData;
            var addCalculationGroupItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup_ToolTip,
                PipingFormsResources.AddFolderIcon, (o, args) =>
                {
                    var newGroup = new PipingCalculationGroup
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, Resources.PipingCalculationGroup_DefaultName, c => c.Name)
                    };

                    group.Children.Add(newGroup);
                    nodeData.NotifyObservers();

                    SelectNewlyAddedItemInTreeView(node);
                });

            var addCalculationItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation_ToolTip,
                PipingFormsResources.PipingIcon, (o, args) =>
                {
                    var calculation = new PipingCalculation
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, Resources.PipingCalculation_DefaultName, c => c.Name)
                    };

                    group.Children.Add(calculation);
                    nodeData.NotifyObservers();

                    SelectNewlyAddedItemInTreeView(node);
                });

            var validateAllItem = new StrictContextMenuItem(
                PipingFormsResources.PipingCalculationItem_Validate,
                PipingFormsResources.PipingCalculationGroup_Validate_ToolTip,
                PipingFormsResources.ValidationIcon, (o, args) =>
                {
                    foreach (PipingCalculation calculation in group.Children.GetPipingCalculations())
                    {
                        PipingCalculationService.Validate(calculation);
                    }
                });

            var calculateAllItem = new StrictContextMenuItem(
                RingtoetsFormsResources.Calculate_all,
                PipingFormsResources.PipingCalculationGroup_CalculateAll_ToolTip,
                RingtoetsFormsResources.CalculateAllIcon, (o, args) => { RunActivitiesAction(group.GetPipingCalculations().Select(pc => new PipingCalculationActivity(pc))); });

            var clearAllItem = new StrictContextMenuItem(
                RingtoetsFormsResources.Clear_all_output,
                PipingFormsResources.PipingCalculationGroup_ClearOutput_ToolTip,
                RingtoetsFormsResources.ClearIcon, (o, args) =>
                {
                    if (MessageBox.Show(PipingFormsResources.PipingCalculationGroupContextNodePresenter_GetContextMenu_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return;
                    }

                    foreach (PipingCalculation calc in group.GetPipingCalculations().Where(c => c.HasOutput))
                    {
                        calc.ClearOutput();
                        calc.NotifyObservers();
                    }
                });

            if (!nodeData.WrappedData.GetPipingCalculations().Any(c => c.HasOutput))
            {
                clearAllItem.Enabled = false;
                clearAllItem.ToolTipText = PipingFormsResources.PipingCalculationGroup_ClearOutput_No_calculation_with_output_to_clear;
            }

            return contextMenuBuilderProvider.Get(node)
                                             .AddCustomItem(addCalculationGroupItem)
                                             .AddCustomItem(addCalculationItem)
                                             .AddSeparator()
                                             .AddCustomItem(validateAllItem)
                                             .AddCustomItem(calculateAllItem)
                                             .AddCustomItem(clearAllItem)
                                             .AddSeparator()
                                             .AddImportItem()
                                             .AddExportItem()
                                             .AddSeparator()
                                             .AddExpandAllItem()
                                             .AddCollapseAllItem()
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
        }

        protected override IEnumerable GetChildNodeObjects(PipingCalculationGroupContext nodeData)
        {
            foreach (IPipingCalculationItem item in nodeData.WrappedData.Children)
            {
                var calculation = item as PipingCalculation;
                var group = item as PipingCalculationGroup;

                if (calculation != null)
                {
                    yield return new PipingCalculationContext(calculation,
                                                              nodeData.AvailablePipingSurfaceLines,
                                                              nodeData.AvailablePipingSoilProfiles);
                }
                else if (group != null)
                {
                    yield return new PipingCalculationGroupContext(group,
                                                                   nodeData.AvailablePipingSurfaceLines,
                                                                   nodeData.AvailablePipingSoilProfiles);
                }
                else
                {
                    yield return item;
                }
            }
        }

        protected override DragOperations CanDrag(PipingCalculationGroupContext nodeData)
        {
            if (TreeView.GetNodeByTag(nodeData).Parent.Tag is PipingFailureMechanism)
            {
                return DragOperations.None;
            }
            return DragOperations.Move;
        }

        protected override void OnDragDrop(object item, object itemParent, PipingCalculationGroupContext target, DragOperations operation, int position)
        {
            IPipingCalculationItem pipingCalculationItem = GetAsIPipingCalculationItem(item);
            var originalOwnerContext = itemParent as PipingCalculationGroupContext;
            if (pipingCalculationItem != null && originalOwnerContext != null)
            {
                var isMoveWithinSameContainer = ReferenceEquals(target, originalOwnerContext);
                DroppingPipingCalculationInContainerStrategy dropHandler = GetDragDropStrategy(isMoveWithinSameContainer, originalOwnerContext, target);
                dropHandler.Execute(item, pipingCalculationItem, position);
            }
            else
            {
                base.OnDragDrop(item, itemParent, target, operation, position);
            }
        }

        private static IPipingCalculationItem GetAsIPipingCalculationItem(object item)
        {
            var calculationContext = item as PipingCalculationContext;
            if (calculationContext != null)
            {
                return calculationContext.WrappedData;
            }

            var groupContext = item as PipingCalculationGroupContext;
            if (groupContext != null)
            {
                return groupContext.WrappedData;
            }

            return null;
        }

        private DroppingPipingCalculationInContainerStrategy GetDragDropStrategy(bool isMoveWithinSameContainer, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target)
        {
            if (isMoveWithinSameContainer)
            {
                return new DroppingPipingCalculationWithinSameContainer(TreeView, originalOwnerContext, target);
            }
            return new DroppingPipingCalculationToNewContainer(TreeView, originalOwnerContext, target);
        }

        private bool NodesHaveSameParentFailureMechanism(ITreeNode sourceNode, ITreeNode targetNode)
        {
            var sourceFailureMechanism = GetParentFailureMechanism(sourceNode);
            var targetFailureMechanism = GetParentFailureMechanism(targetNode);

            return ReferenceEquals(sourceFailureMechanism, targetFailureMechanism);
        }

        private static PipingFailureMechanism GetParentFailureMechanism(ITreeNode sourceNode)
        {
            PipingFailureMechanism sourceFailureMechanism;
            var node = sourceNode;
            while ((sourceFailureMechanism = node.Tag as PipingFailureMechanism) == null)
            {
                // No parent found, go search higher up hierarchy!
                node = node.Parent;
                if (node == null)
                {
                    break;
                }
            }
            return sourceFailureMechanism;
        }

        private void SelectNewlyAddedItemInTreeView(ITreeNode node)
        {
            // Expand parent of 'newItem' to ensure its selected state is visible.
            if (!node.IsExpanded)
            {
                node.Expand();
            }
            ITreeNode newlyAppendedNodeForNewItem = node.Nodes.Last();
            TreeView.SelectedNode = newlyAppendedNodeForNewItem;
        }

        #region Nested Types: DroppingPipingCalculationInContainerStrategy and implementations

        /// <summary>
        /// Strategy pattern implementation for dealing with drag & dropping a <see cref="IPipingCalculationItem"/>
        /// onto <see cref="PipingCalculationGroup"/> data.
        /// </summary>
        private abstract class DroppingPipingCalculationInContainerStrategy
        {
            protected readonly ITreeView treeView;
            protected readonly PipingCalculationGroupContext target;
            private readonly PipingCalculationGroupContext originalOwnerContext;

            protected DroppingPipingCalculationInContainerStrategy(ITreeView treeView, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target)
            {
                this.treeView = treeView;
                this.originalOwnerContext = originalOwnerContext;
                this.target = target;
            }

            /// <summary>
            /// Perform the drag & drop operation.
            /// </summary>
            /// <param name="draggedDataObject">The actual dragged data object.</param>
            /// <param name="pipingCalculationItem">The piping calculation item corresponding with <see cref="draggedDataObject"/>.</param>
            /// <param name="newPosition">The index of the new position within the new owner's collection.</param>
            public virtual void Execute(object draggedDataObject, IPipingCalculationItem pipingCalculationItem, int newPosition)
            {
                var targetRecordedNodeState = new TreeNodeExpandCollapseState(treeView.GetNodeByTag(target));

                MoveCalculationItemToNewOwner(pipingCalculationItem, newPosition);

                NotifyObservers();

                ITreeNode draggedNode = treeView.GetNodeByTag(draggedDataObject);
                UpdateTreeView(draggedNode, targetRecordedNodeState);
            }

            /// <summary>
            /// Moves the <see cref="IPipingCalculationItem"/> instance to its new location.
            /// </summary>
            /// <param name="pipingCalculationItem">The instance to be relocated.</param>
            /// <param name="position">The index in the new <see cref="PipingCalculationGroup"/>
            /// owner within its <see cref="PipingCalculationGroup.Children"/>.</param>
            protected void MoveCalculationItemToNewOwner(IPipingCalculationItem pipingCalculationItem, int position)
            {
                originalOwnerContext.WrappedData.Children.Remove(pipingCalculationItem);
                target.WrappedData.Children.Insert(position, pipingCalculationItem);
            }

            /// <summary>
            /// Notifies observers of the change in state.
            /// </summary>
            protected virtual void NotifyObservers()
            {
                originalOwnerContext.NotifyObservers();
            }

            /// <summary>
            /// Updates the <see cref="System.Windows.Forms.TreeView"/> where the drag & drop
            /// operation took place.
            /// </summary>
            /// <param name="draggedNode">The dragged node.</param>
            /// <param name="targetRecordedNodeState">Recorded state of the target node
            /// before the drag & drop operation.</param>
            protected virtual void UpdateTreeView(ITreeNode draggedNode, TreeNodeExpandCollapseState targetRecordedNodeState)
            {
                HandlePostDragExpandCollapseOfNewOwner(draggedNode, targetRecordedNodeState);
                treeView.SelectedNode = draggedNode;
            }

            private static void HandlePostDragExpandCollapseOfNewOwner(ITreeNode draggedNode, TreeNodeExpandCollapseState newOwnerRecordedNodeState)
            {
                ITreeNode newParentOfDraggedNode = draggedNode.Parent;
                newOwnerRecordedNodeState.Restore(newParentOfDraggedNode);

                // Expand parent of 'draggedNode' to ensure 'draggedNode' is visible.
                if (!newParentOfDraggedNode.IsExpanded)
                {
                    newParentOfDraggedNode.Expand();
                }
            }
        }

        /// <summary>
        /// Strategy implementation for rearranging the order of an <see cref="IPipingCalculationItem"/>
        /// within a <see cref="PipingCalculationGroup"/> through a drag & drop action.
        /// </summary>
        private class DroppingPipingCalculationWithinSameContainer : DroppingPipingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingPipingCalculationWithinSameContainer"/> class.
            /// </summary>
            /// <param name="treeView">The tree view where the drag & drop operation occurs.</param>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingPipingCalculationWithinSameContainer(ITreeView treeView, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target) :
                base(treeView, originalOwnerContext, target) {}
        }

        /// <summary>
        /// Strategy implementation for moving an <see cref="IPipingCalculationItem"/> from
        /// one <see cref="PipingCalculationGroup"/> to another using a drag & drop action.
        /// </summary>
        private class DroppingPipingCalculationToNewContainer : DroppingPipingCalculationInContainerStrategy
        {
            private TreeNodeExpandCollapseState recordedNodeState;

            private bool renamed;

            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingPipingCalculationToNewContainer"/> class.
            /// </summary>
            /// <param name="treeView">The tree view where the drag & drop operation occurs.</param>
            /// <param name="originalOwnerContext">The calculation group context that is 
            /// the original owner of the dragged item.</param>
            /// <param name="target">The calculation group context that is the target
            /// of the drag & drop operation.</param>
            public DroppingPipingCalculationToNewContainer(ITreeView treeView, PipingCalculationGroupContext originalOwnerContext, PipingCalculationGroupContext target) :
                base(treeView, originalOwnerContext, target) {}

            public override void Execute(object draggedDataObject, IPipingCalculationItem pipingCalculationItem, int newPosition)
            {
                var targetRecordedNodeState = new TreeNodeExpandCollapseState(treeView.GetNodeByTag(target));

                recordedNodeState = new TreeNodeExpandCollapseState(treeView.GetNodeByTag(draggedDataObject));
                renamed = EnsureDraggedNodeHasUniqueNameInNewOwner(pipingCalculationItem, target);

                MoveCalculationItemToNewOwner(pipingCalculationItem, newPosition);

                NotifyObservers();

                ITreeNode draggedNode = treeView.GetNodeByTag(draggedDataObject);
                UpdateTreeView(draggedNode, targetRecordedNodeState);
            }

            protected override void UpdateTreeView(ITreeNode draggedNode, TreeNodeExpandCollapseState targetRecordedNodeState)
            {
                base.UpdateTreeView(draggedNode, targetRecordedNodeState);
                recordedNodeState.Restore(draggedNode);
                if (renamed)
                {
                    treeView.StartLabelEdit();
                }
            }

            protected override void NotifyObservers()
            {
                base.NotifyObservers();
                target.NotifyObservers();
            }

            private static bool EnsureDraggedNodeHasUniqueNameInNewOwner(IPipingCalculationItem pipingCalculationItem, PipingCalculationGroupContext newOwner)
            {
                bool renamed = false;
                string uniqueName = NamingHelper.GetUniqueName(newOwner.WrappedData.Children, pipingCalculationItem.Name, pci => pci.Name);
                if (!pipingCalculationItem.Name.Equals(uniqueName))
                {
                    renamed = TryRenameTo(pipingCalculationItem, uniqueName);
                }
                return renamed;
            }

            private static bool TryRenameTo(IPipingCalculationItem pipingCalculationItem, string newName)
            {
                var calculation = pipingCalculationItem as PipingCalculation;
                if (calculation != null)
                {
                    calculation.Name = newName;
                    return true;
                }

                var group = pipingCalculationItem as PipingCalculationGroup;
                if (group != null && group.IsNameEditable)
                {
                    group.Name = newName;
                    return true;
                }

                return false;
            }
        }

        #endregion
    }
}