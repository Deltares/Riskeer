// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.TreeNodeInfos
{
    /// <summary>
    /// Factory for creating calculation related <see cref="TreeNodeInfo"/> objects.
    /// </summary>
    public static class CalculationTreeNodeInfoFactory
    {
        /// <summary>
        /// Creates a <see cref="TreeNodeInfo"/> object for a calculation group context of the type <typeparamref name="TCalculationGroupContext"/>.
        /// </summary>
        /// <typeparam name="TCalculationGroupContext">The type of calculation group context to create a <see cref="TreeNodeInfo"/> object for.</typeparam>
        /// <param name="childNodeObjects">The function for obtaining the child node objects.</param>
        /// <param name="contextMenuStrip">The function for obtaining the context menu strip.</param>
        /// <param name="onNodeRemoved">The action to perform on removing a node.</param>
        /// <returns>A <see cref="TreeNodeInfo"/> object.</returns>
        public static TreeNodeInfo<TCalculationGroupContext> CreateCalculationGroupContextTreeNodeInfo<TCalculationGroupContext>(
            Func<TCalculationGroupContext, object[]> childNodeObjects,
            Func<TCalculationGroupContext, object, TreeViewControl, ContextMenuStrip> contextMenuStrip,
            Action<TCalculationGroupContext, object> onNodeRemoved)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            return new TreeNodeInfo<TCalculationGroupContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => Resources.GeneralFolderIcon,
                EnsureVisibleOnCreate = (context, parent) => !(parent is IFailureMechanismContext<IFailureMechanism>),
                ChildNodeObjects = childNodeObjects,
                ContextMenuStrip = contextMenuStrip,
                CanRename = (context, parentData) => IsNestedGroup(parentData),
                OnNodeRenamed = (context, newName) =>
                {
                    context.WrappedData.Name = newName;
                    context.NotifyObservers();
                },
                CanRemove = (context, parentData) => IsNestedGroup(parentData),
                OnNodeRemoved = onNodeRemoved,
                CanDrag = (context, parentData) => IsNestedGroup(parentData),
                CanInsert = CalculationGroupCanDropOrInsert,
                CanDrop = CalculationGroupCanDropOrInsert,
                OnDrop = CalculationGroupOnDrop
            };
        }

        /// <summary>
        /// Creates a <see cref="TreeNodeInfo"/> object for a calculation context of the type <typeparamref name="TCalculationContext"/>. 
        /// </summary>
        /// <param name="icon">The icon of the <see cref="TreeNodeInfo"/>.</param>
        /// <param name="childeNodeObjects">The function for obtaining the child node objects.</param>
        /// <param name="contextMenuStrip">The function for obtaining the context menu strip.</param>
        /// <param name="onNodeRemoved">The action to perform on removing a node.</param>
        /// <typeparam name="TCalculationContext">The type of calculation context to create a <see cref="TreeNodeInfo"/> object for.</typeparam>
        /// <returns>A <see cref="TreeNodeInfo"/> object.</returns>
        public static TreeNodeInfo<TCalculationContext> CreateCalculationContextTreeNodeInfo<TCalculationContext>(
            Bitmap icon,
            Func<TCalculationContext, object[]> childeNodeObjects,
            Func<TCalculationContext, object, TreeViewControl, ContextMenuStrip> contextMenuStrip,
            Action<TCalculationContext, object> onNodeRemoved)
            where TCalculationContext : ICalculationContext<ICalculation, IFailureMechanism>
        {
            return new TreeNodeInfo<TCalculationContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => icon,
                EnsureVisibleOnCreate = (context, parent) => true,
                ChildNodeObjects = childeNodeObjects,
                ContextMenuStrip = contextMenuStrip,
                CanRename = (context, parent) => true,
                OnNodeRenamed = (context, newName) =>
                {
                    context.WrappedData.Name = newName;
                    context.WrappedData.NotifyObservers();
                },
                CanRemove = (context, parentData) => CalculationContextCanRemove(context, parentData),
                OnNodeRemoved = onNodeRemoved,
                CanDrag = (context, parentData) => true
            };
        }

        /// <summary>
        /// Creates a <see cref="TreeNodeInfo"/> object for a failure mechanism context of the type <typeparamref name="TFailureMechanismContext"/>. 
        /// </summary>
        /// <param name="enabledChildeNodeObjects">The function for obtaining the child node objects when <see cref="IFailureMechanism.IsRelevant"/> is <c>true</c>.</param>
        /// <param name="disabledChildeNodeObjects">The function for obtaining the child node objects when <see cref="IFailureMechanism.IsRelevant"/> is <c>false</c>.</param>
        /// <param name="enabledContextMenuStrip">The function for obtaining the context menu strip when <see cref="IFailureMechanism.IsRelevant"/> is <c>true</c>.</param>
        /// <param name="disabledContextMenuStrip">The function for obtaining the context menu strip when <see cref="IFailureMechanism.IsRelevant"/> is <c>false</c>.</param>
        /// <typeparam name="TFailureMechanismContext">The type of failure mechanism context to create a <see cref="TreeNodeInfo"/> object for.</typeparam>
        /// <returns>A <see cref="TreeNodeInfo"/> object.</returns>
        public static TreeNodeInfo<TFailureMechanismContext> CreateFailureMechanismContextTreeNodeInfo<TFailureMechanismContext>(
            Func<TFailureMechanismContext, object[]> enabledChildeNodeObjects,
            Func<TFailureMechanismContext, object[]> disabledChildeNodeObjects,
            Func<TFailureMechanismContext, object, TreeViewControl, ContextMenuStrip> enabledContextMenuStrip,
            Func<TFailureMechanismContext, object, TreeViewControl, ContextMenuStrip> disabledContextMenuStrip)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            return new TreeNodeInfo<TFailureMechanismContext>
            {
                Text = context => context.WrappedData.Name,
                ForeColor = context => context.WrappedData.IsRelevant
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                Image = context => Resources.FailureMechanismIcon,
                ChildNodeObjects = context => context.WrappedData.IsRelevant
                                                  ? enabledChildeNodeObjects(context)
                                                  : disabledChildeNodeObjects(context),
                ContextMenuStrip = (context, parentData, treeViewControl) => context.WrappedData.IsRelevant
                                                                                 ? enabledContextMenuStrip(context, parentData, treeViewControl)
                                                                                 : disabledContextMenuStrip(context, parentData, treeViewControl)
            };
        }

        /// <summary>
        /// This method adds a context menu item for creating new calculation groups.
        /// </summary>
        /// <param name="builder">The builder to add the context menu item to.</param>
        /// <param name="calculationGroup">The calculation group involved.</param>
        public static void AddCreateCalculationGroupItem(IContextMenuBuilder builder, CalculationGroup calculationGroup)
        {
            var createCalculationGroupItem = new StrictContextMenuItem(
                Resources.CalculationGroup_Add_CalculationGroup,
                Resources.CalculationGroup_Add_CalculationGroup_Tooltip,
                Resources.AddFolderIcon,
                (o, args) =>
                {
                    var calculation = new CalculationGroup
                    {
                        Name = NamingHelper.GetUniqueName(calculationGroup.Children, RingtoetsCommonDataResources.CalculationGroup_DefaultName, c => c.Name)
                    };
                    calculationGroup.Children.Add(calculation);
                    calculationGroup.NotifyObservers();
                });

            builder.AddCustomItem(createCalculationGroupItem);
        }

        /// <summary>
        /// This method adds a context menu item for creating new calculations.
        /// </summary>
        /// <param name="builder">The builder to add the context menu item to.</param>
        /// <param name="calculationGroupContext">The calculation group context involved.</param>
        /// <param name="addCalculation">The action for adding a calculation to the calculation group.</param>
        public static void AddCreateCalculationItem<TCalculationGroupContext>(IContextMenuBuilder builder, TCalculationGroupContext calculationGroupContext, Action<TCalculationGroupContext> addCalculation) 
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            var createCalculationItem = new StrictContextMenuItem(
                Resources.CalculationGroup_Add_Calculation,
                Resources.CalculationGroup_Add_Calculation_Tooltip,
                Resources.FailureMechanismIcon,
                (o, args) => { addCalculation(calculationGroupContext); });

            builder.AddCustomItem(createCalculationItem);
        }

        /// <summary>
        /// This method adds a context menu item for clearing the output of all calculations in the calculation group.
        /// </summary>
        /// <param name="builder">The builder to add the context menu item to.</param>
        /// <param name="calculationGroup">The calculation group involved.</param>
        public static void AddClearAllCalculationOutputInGroupItem(IContextMenuBuilder builder, CalculationGroup calculationGroup)
        {
            var clearAllItem = new StrictContextMenuItem(
                Resources.Clear_all_output,
                Resources.CalculationGroup_ClearOutput_ToolTip,
                Resources.ClearIcon, (o, args) =>
                {
                    if (MessageBox.Show(Resources.CalculationGroup_ClearOutput_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return;
                    }

                    foreach (var calc in calculationGroup.GetCalculations().Where(c => c.HasOutput))
                    {
                        calc.ClearOutput();
                        calc.NotifyObservers();
                    }
                });

            if (!calculationGroup.GetCalculations().Any(c => c.HasOutput))
            {
                clearAllItem.Enabled = false;
                clearAllItem.ToolTipText = Resources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear;
            }

            builder.AddCustomItem(clearAllItem);
        }

        /// <summary>
        /// This method adds a context menu item for performing all calculations in the calculation group.
        /// </summary>
        /// <param name="builder">The builder to add the context menu item to.</param>
        /// <param name="calculationGroup">The calculation group involved.</param>
        /// <param name="context">The calculation group context belonging to the calculation group.</param>
        /// <param name="calculateAll">The action that performs all calculations.</param>
        public static void AddPerformAllCalculationsInGroupItem<TCalculationGroupContext>
            (IContextMenuBuilder builder, CalculationGroup calculationGroup, TCalculationGroupContext context, Action<CalculationGroup, TCalculationGroupContext> calculateAll)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            var performAllItem = new StrictContextMenuItem(
                Resources.Calculate_all,
                Resources.CalculationGroup_CalculateAll_ToolTip,
                Resources.CalculateAllIcon, (o, args) => { calculateAll(calculationGroup, context); });

            if (!calculationGroup.GetCalculations().Any())
            {
                performAllItem.Enabled = false;
                performAllItem.ToolTipText = Resources.CalculationGroup_CalculateAll_No_calculations_to_run;
            }

            builder.AddCustomItem(performAllItem);
        }

        /// <summary>
        /// This method adds a context menu item for performing a calculation.
        /// </summary>
        /// <param name="builder">The builder to add the context menu item to.</param>
        /// <param name="calculation">The calculation involved.</param>
        /// <param name="context">The calculation context belonging to the calculation.</param>
        /// <param name="calculate">The action that performs the calculation.</param>
        public static void AddPerformCalculationItem<TCalculation, TCalculationContext>(
            IContextMenuBuilder builder, TCalculation calculation, TCalculationContext context, Action<TCalculation, TCalculationContext> calculate)
            where TCalculation : ICalculation where TCalculationContext : ICalculationContext<ICalculation, IFailureMechanism>
        {
            var calculateItem = new StrictContextMenuItem(
                Resources.Calculate,
                Resources.Calculate_ToolTip,
                Resources.CalculateIcon,
                (o, args) => { calculate(calculation, context); });

            builder.AddCustomItem(calculateItem);
        }

        /// <summary>
        /// This method adds a context menu item for clearing the output of a calculation.
        /// </summary>
        /// <param name="builder">The builder to add the context menu item to.</param>
        /// <param name="calculation">The calculation involved.</param>
        public static void AddClearCalculationOutputItem(IContextMenuBuilder builder, ICalculation calculation)
        {
            var clearOutputItem = new StrictContextMenuItem(
                Resources.Clear_output,
                Resources.Clear_output_ToolTip,
                Resources.ClearIcon,
                (o, args) =>
                {
                    if (MessageBox.Show(Resources.Calculation_ContextMenuStrip_Are_you_sure_clear_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return;
                    }

                    calculation.ClearOutput();
                    calculation.NotifyObservers();
                });

            if (!calculation.HasOutput)
            {
                clearOutputItem.Enabled = false;
                clearOutputItem.ToolTipText = Resources.ClearOutput_No_output_to_clear;
            }

            builder.AddCustomItem(clearOutputItem);
        }

        /// <summary>
        /// This method adds a context menu item for changing the relevancy state of a disabled failure mechanism.
        /// </summary>
        /// <param name="builder">The builder to add the context menu item to.</param>
        /// <param name="failureMechanismContext">The failure mechanism context involved.</param>
        public static void AddDisabledChangeRelevancyItem<TFailureMechanismContext>(IContextMenuBuilder builder, TFailureMechanismContext failureMechanismContext)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            var changeRelevancyItem = new StrictContextMenuItem(
                Resources.FailureMechanismContextMenuStrip_Is_relevant,
                Resources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                Resources.Checkbox_empty,
                (sender, args) =>
                {
                    failureMechanismContext.WrappedData.IsRelevant = true;
                    failureMechanismContext.WrappedData.NotifyObservers();
                });

            builder.AddCustomItem(changeRelevancyItem);
        }

        # region Helper methods for CreateCalculationGroupContextTreeNodeInfo

        private static bool IsNestedGroup(object parentData)
        {
            return parentData is ICalculationContext<CalculationGroup, IFailureMechanism>;
        }

        private static bool CalculationGroupCanDropOrInsert(object draggedData, object targetData)
        {
            var calculationContext = draggedData as ICalculationContext<ICalculationBase, IFailureMechanism>;
            return calculationContext != null && ReferenceEquals(calculationContext.FailureMechanism, ((ICalculationContext<CalculationGroup, IFailureMechanism>) targetData).FailureMechanism);
        }

        private static void CalculationGroupOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            ICalculationBase calculationItem = ((ICalculationContext<ICalculationBase, IFailureMechanism>) droppedData).WrappedData;
            var originalOwnerContext = oldParentData as ICalculationContext<CalculationGroup, IFailureMechanism>;
            var targetContext = newParentData as ICalculationContext<CalculationGroup, IFailureMechanism>;

            if (calculationItem != null && originalOwnerContext != null && targetContext != null)
            {
                var sourceCalculationGroup = originalOwnerContext.WrappedData;
                var targetCalculationGroup = targetContext.WrappedData;

                var isMoveWithinSameContainer = ReferenceEquals(sourceCalculationGroup, targetCalculationGroup);

                DroppingCalculationInContainerStrategy dropHandler = GetDragDropStrategy(isMoveWithinSameContainer, sourceCalculationGroup, targetCalculationGroup);
                dropHandler.Execute(droppedData, calculationItem, position, treeViewControl);
            }
        }

        private static DroppingCalculationInContainerStrategy GetDragDropStrategy(bool isMoveWithinSameContainer, CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup)
        {
            return isMoveWithinSameContainer
                       ? (DroppingCalculationInContainerStrategy) new DroppingCalculationWithinSameContainer(sourceCalculationGroup, targetCalculationGroup)
                       : new DroppingCalculationToNewContainer(sourceCalculationGroup, targetCalculationGroup);
        }

        # region Nested types: DroppingCalculationInContainerStrategy and implementations

        /// <summary>
        /// Strategy pattern implementation for dealing with drag and drop of a <see cref="ICalculation"/>
        /// onto <see cref="CalculationGroup"/> data.
        /// </summary>
        private abstract class DroppingCalculationInContainerStrategy
        {
            protected readonly CalculationGroup targetCalculationGroup;
            private readonly CalculationGroup sourceCalculationGroup;

            protected DroppingCalculationInContainerStrategy(CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup)
            {
                this.sourceCalculationGroup = sourceCalculationGroup;
                this.targetCalculationGroup = targetCalculationGroup;
            }

            /// <summary>
            /// Performs the drag and drop operation.
            /// </summary>
            /// <param name="draggedData">The dragged data.</param>
            /// <param name="calculationBase">The calculation item wrapped by <see cref="draggedData"/>.</param>
            /// <param name="newPosition">The index of the new position within the new owner's collection.</param>
            /// <param name="treeViewControl">The tree view control which is at stake.</param>
            public virtual void Execute(object draggedData, ICalculationBase calculationBase, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(calculationBase, newPosition);

                NotifyObservers();
            }

            /// <summary>
            /// Moves the <see cref="ICalculationBase"/> instance to its new location.
            /// </summary>
            /// <param name="calculationBase">The instance to be relocated.</param>
            /// <param name="position">The index in the new <see cref="CalculationGroup"/>
            /// owner within its <see cref="CalculationGroup.Children"/>.</param>
            protected void MoveCalculationItemToNewOwner(ICalculationBase calculationBase, int position)
            {
                sourceCalculationGroup.Children.Remove(calculationBase);
                targetCalculationGroup.Children.Insert(position, calculationBase);
            }

            /// <summary>
            /// Notifies observers of the change in state.
            /// </summary>
            protected virtual void NotifyObservers()
            {
                sourceCalculationGroup.NotifyObservers();
            }
        }

        /// <summary>
        /// Strategy implementation for rearranging the order of an <see cref="ICalculation"/>
        /// within a <see cref="CalculationGroup"/> through a drag and drop action.
        /// </summary>
        private class DroppingCalculationWithinSameContainer : DroppingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingCalculationWithinSameContainer"/> class.
            /// </summary>
            /// <param name="sourceCalculationGroup">The calculation group that is the target of the drag and drop operation.</param>
            /// <param name="targetCalculationGroup">The calculation group that is the original owner of the dragged item.</param>
            public DroppingCalculationWithinSameContainer(CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup) :
                base(sourceCalculationGroup, targetCalculationGroup) {}
        }

        /// <summary>
        /// Strategy implementation for moving an <see cref="ICalculation"/> from
        /// one <see cref="CalculationGroup"/> to another using a drag and drop action.
        /// </summary>
        private class DroppingCalculationToNewContainer : DroppingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingCalculationToNewContainer"/> class.
            /// </summary>
            /// <param name="sourceCalculationGroup">The calculation group that is the original owner of the dragged item.</param>
            /// <param name="targetCalculationGroup">The calculation group that is the target of the drag and drop operation.</param>
            public DroppingCalculationToNewContainer(CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup) :
                base(sourceCalculationGroup, targetCalculationGroup) {}

            public override void Execute(object draggedData, ICalculationBase calculationBase, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(calculationBase, newPosition);

                NotifyObservers();

                // Try to start a name edit action when an item with the same name was already present
                if (targetCalculationGroup.Children.Except(new[]
                {
                    calculationBase
                }).Any(c => c.Name.Equals(calculationBase.Name)))
                {
                    treeViewControl.TryRenameNodeForData(draggedData);
                }
            }

            protected override void NotifyObservers()
            {
                base.NotifyObservers();
                targetCalculationGroup.NotifyObservers();
            }
        }

        # endregion

        # endregion

        #region Helper methods for CreateCalculationContextTreeNodeInfo

        private static bool CalculationContextCanRemove(ICalculationContext<ICalculation, IFailureMechanism> calculationContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as ICalculationContext<CalculationGroup, IFailureMechanism>;
            return calculationGroupContext != null && calculationGroupContext.WrappedData.Children.Contains(calculationContext.WrappedData);
        }

        #endregion
    }
}