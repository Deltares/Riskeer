﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.Forms.TreeNodeInfos
{
    /// <summary>
    /// Factory for creating <see cref="TreeNodeInfo"/> objects.
    /// </summary>
    public static class TreeNodeInfoFactory
    {
        /// <summary>
        /// Creates a <see cref="TreeNodeInfo"/> object for a calculation group context of the type <typeparamref name="TCalculationGroupContext"/>.
        /// </summary>
        /// <typeparam name="TCalculationGroupContext">The type of calculation group context to create a <see cref="TreeNodeInfo"/> object for.</typeparam>
        /// <param name="childNodeObjects">The function for obtaining child node objects.</param>
        /// <param name="addCalculation">The action for adding a calculation to the calculation group.</param>
        /// <param name="gui">The gui to use.</param>
        /// <returns>A <see cref="TreeNodeInfo"/> object.</returns>
        public static TreeNodeInfo<TCalculationGroupContext> CreateCalculationGroupContextTreeNodeInfo<TCalculationGroupContext>(
            Func<TCalculationGroupContext, object[]> childNodeObjects,
            Action<TCalculationGroupContext> addCalculation,
            IGui gui)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            return new TreeNodeInfo<TCalculationGroupContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => Resources.GeneralFolderIcon,
                EnsureVisibleOnCreate = context => true,
                ChildNodeObjects = childNodeObjects,
                ContextMenuStrip = (context, parentData, treeViewControl) => ContextMenuStrip(context, parentData, treeViewControl, addCalculation, gui),
                CanRename = (context, parentData) => IsNestedGroup(parentData),
                OnNodeRenamed = (context, newName) =>
                {
                    context.WrappedData.Name = newName;
                    context.NotifyObservers();
                },
                CanRemove = (context, parentData) => IsNestedGroup(parentData),
                OnNodeRemoved = (context, parentData) =>
                {
                    var parentGroup = (ICalculationContext<CalculationGroup, IFailureMechanism>) parentData;

                    parentGroup.WrappedData.Children.Remove(context.WrappedData);
                    parentGroup.NotifyObservers();
                },
                CanDrag = (context, parentData) => IsNestedGroup(parentData),
                CanInsert = CanDropOrInsert,
                CanDrop = CanDropOrInsert,
                OnDrop = OnDrop
            };
        }

        private static bool IsNestedGroup(object parentData)
        {
            return parentData is ICalculationContext<CalculationGroup, IFailureMechanism>;
        }

        private static ContextMenuStrip ContextMenuStrip<TCalculationGroupContext>(TCalculationGroupContext nodeData, object parentData, TreeViewControl treeViewControl, Action<TCalculationGroupContext> addCalculation, IGui gui) where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            var group = nodeData.WrappedData;

            var addCalculationGroupItem = new StrictContextMenuItem(
                Resources.CalculationGroup_Add_CalculationGroup,
                Resources.Add_calculation_group_to_calculation_group_tooltip,
                Resources.AddFolderIcon,
                (o, args) =>
                {
                    var calculation = new CalculationGroup
                    {
                        Name = NamingHelper.GetUniqueName(group.Children, RingtoetsCommonDataResources.CalculationGroup_DefaultName, c => c.Name)
                    };
                    group.Children.Add(calculation);
                    nodeData.WrappedData.NotifyObservers();
                });

            var addCalculationItem = new StrictContextMenuItem(
                Resources.CalculationGroup_Add_Calculation,
                Resources.Add_calculation_to_calculation_group_tooltip,
                Resources.FailureMechanismIcon,
                (o, args) => { addCalculation(nodeData); });

            var builder = gui.Get(nodeData, treeViewControl);

            var isNestedGroup = IsNestedGroup(parentData);

            if (!isNestedGroup)
            {
                builder
                    .AddOpenItem()
                    .AddSeparator();
            }

            builder
                .AddCustomItem(addCalculationGroupItem)
                .AddCustomItem(addCalculationItem)
                .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
                builder.AddDeleteItem();
                builder.AddSeparator();
            }

            return builder
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        private static bool CanDropOrInsert(object draggedData, object targetData)
        {
            var calculationContext = draggedData as ICalculationContext<ICalculationBase, IFailureMechanism>;
            return calculationContext != null && ReferenceEquals(calculationContext.FailureMechanism, ((ICalculationContext<CalculationGroup, IFailureMechanism>)targetData).FailureMechanism);
        }

        private static void OnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
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
                ? (DroppingCalculationInContainerStrategy)new DroppingCalculationWithinSameContainer(sourceCalculationGroup, targetCalculationGroup)
                : new DroppingCalculationToNewContainer(sourceCalculationGroup, targetCalculationGroup);
        }

        #region Nested Types: DroppingPipingCalculationInContainerStrategy and implementations

        /// <summary>
        /// Strategy pattern implementation for dealing with drag & dropping a <see cref="ICalculation"/>
        /// onto <see cref="CalculationGroup"/> data.
        /// </summary>
        private abstract class DroppingCalculationInContainerStrategy
        {
            private readonly CalculationGroup sourceCalculationGroup;
            protected readonly CalculationGroup targetCalculationGroup;

            protected DroppingCalculationInContainerStrategy(CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup)
            {
                this.sourceCalculationGroup = sourceCalculationGroup;
                this.targetCalculationGroup = targetCalculationGroup;
            }

            /// <summary>
            /// Perform the drag & drop operation.
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
        /// within a <see cref="CalculationGroup"/> through a drag & drop action.
        /// </summary>
        private class DroppingCalculationWithinSameContainer : DroppingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingCalculationWithinSameContainer"/> class.
            /// </summary>
            /// <param name="sourceCalculationGroup">The calculation group that is the target of the drag & drop operation.</param>
            /// <param name="targetCalculationGroup">The calculation group that is the original owner of the dragged item.</param>
            public DroppingCalculationWithinSameContainer(CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup) :
                base(sourceCalculationGroup, targetCalculationGroup) { }
        }

        /// <summary>
        /// Strategy implementation for moving an <see cref="ICalculation"/> from
        /// one <see cref="CalculationGroup"/> to another using a drag & drop action.
        /// </summary>
        private class DroppingCalculationToNewContainer : DroppingCalculationInContainerStrategy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DroppingCalculationToNewContainer"/> class.
            /// </summary>
            /// <param name="sourceCalculationGroup">The calculation group that is the original owner of the dragged item.</param>
            /// <param name="targetCalculationGroup">The calculation group that is the target of the drag & drop operation.</param>
            public DroppingCalculationToNewContainer(CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup) :
                base(sourceCalculationGroup, targetCalculationGroup) { }

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

        #endregion
    }
}