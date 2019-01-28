// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.TreeNodeInfos
{
    /// <summary>
    /// Factory for creating calculation related <see cref="TreeNodeInfo"/> objects.
    /// </summary>
    public static class RingtoetsTreeNodeInfoFactory
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
                EnsureVisibleOnCreate = (context, parent) => parent is ICalculationContext<CalculationGroup, IFailureMechanism>,
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
        /// <typeparam name="TCalculationContext">The type of calculation context to create a <see cref="TreeNodeInfo"/> object for.</typeparam>
        /// <param name="childNodeObjects">The function for obtaining the child node objects.</param>
        /// <param name="contextMenuStrip">The function for obtaining the context menu strip.</param>
        /// <param name="onNodeRemoved">The action to perform on removing a node.</param>
        /// <returns>A <see cref="TreeNodeInfo"/> object.</returns>
        public static TreeNodeInfo<TCalculationContext> CreateCalculationContextTreeNodeInfo<TCalculationContext>(
            Func<TCalculationContext, object[]> childNodeObjects,
            Func<TCalculationContext, object, TreeViewControl, ContextMenuStrip> contextMenuStrip,
            Action<TCalculationContext, object> onNodeRemoved)
            where TCalculationContext : ICalculationContext<ICalculation, IFailureMechanism>
        {
            return new TreeNodeInfo<TCalculationContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => Resources.CalculationIcon,
                EnsureVisibleOnCreate = (context, parent) => true,
                ChildNodeObjects = childNodeObjects,
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
        /// <typeparam name="TFailureMechanismContext">The type of failure mechanism context to create a <see cref="TreeNodeInfo"/> object for.</typeparam>
        /// <param name="enabledChildNodeObjects">The function for obtaining the child node objects when <see cref="IFailureMechanism.IsRelevant"/> is <c>true</c>.</param>
        /// <param name="disabledChildNodeObjects">The function for obtaining the child node objects when <see cref="IFailureMechanism.IsRelevant"/> is <c>false</c>.</param>
        /// <param name="enabledContextMenuStrip">The function for obtaining the context menu strip when <see cref="IFailureMechanism.IsRelevant"/> is <c>true</c>.</param>
        /// <param name="disabledContextMenuStrip">The function for obtaining the context menu strip when <see cref="IFailureMechanism.IsRelevant"/> is <c>false</c>.</param>
        /// <returns>A <see cref="TreeNodeInfo"/> object.</returns>
        public static TreeNodeInfo<TFailureMechanismContext> CreateFailureMechanismContextTreeNodeInfo<TFailureMechanismContext>(
            Func<TFailureMechanismContext, object[]> enabledChildNodeObjects,
            Func<TFailureMechanismContext, object[]> disabledChildNodeObjects,
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
                                                  ? enabledChildNodeObjects(context)
                                                  : disabledChildNodeObjects(context),
                ContextMenuStrip = (context, parentData, treeViewControl) => context.WrappedData.IsRelevant
                                                                                 ? enabledContextMenuStrip(context, parentData, treeViewControl)
                                                                                 : disabledContextMenuStrip(context, parentData, treeViewControl)
            };
        }

        #region Helper methods for CreateCalculationContextTreeNodeInfo

        private static bool CalculationContextCanRemove(ICalculationContext<ICalculation, IFailureMechanism> calculationContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as ICalculationContext<CalculationGroup, IFailureMechanism>;
            return calculationGroupContext != null && calculationGroupContext.WrappedData.Children.Contains(calculationContext.WrappedData);
        }

        #endregion

        #region Helper methods for CreateCalculationGroupContextTreeNodeInfo

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
                CalculationGroup sourceCalculationGroup = originalOwnerContext.WrappedData;
                CalculationGroup targetCalculationGroup = targetContext.WrappedData;

                bool isMoveWithinSameContainer = ReferenceEquals(sourceCalculationGroup, targetCalculationGroup);

                DroppingCalculationInContainerStrategy dropHandler = GetDragDropStrategy(isMoveWithinSameContainer, sourceCalculationGroup, targetCalculationGroup);
                dropHandler.Execute(droppedData, calculationItem, position, treeViewControl);
            }
        }

        private static DroppingCalculationInContainerStrategy GetDragDropStrategy(bool isMoveWithinSameContainer, CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup)
        {
            if (isMoveWithinSameContainer)
            {
                return new DroppingCalculationWithinSameContainer(sourceCalculationGroup, targetCalculationGroup);
            }

            return new DroppingCalculationToNewContainer(sourceCalculationGroup, targetCalculationGroup);
        }

        #region Nested types: DroppingCalculationInContainerStrategy and implementations

        /// <summary>
        /// Strategy pattern implementation for dealing with drag and drop of a <see cref="ICalculation"/>
        /// onto <see cref="CalculationGroup"/> data.
        /// </summary>
        private abstract class DroppingCalculationInContainerStrategy
        {
            private readonly CalculationGroup sourceCalculationGroup;

            /// <summary>
            /// Creates a new instance of <see cref="DroppingCalculationInContainerStrategy"/>.
            /// </summary>
            /// <param name="sourceCalculationGroup">The source <see cref="CalculationGroup"/>.</param>
            /// <param name="targetCalculationGroup">The target <see cref="CalculationGroup"/>.</param>
            protected DroppingCalculationInContainerStrategy(CalculationGroup sourceCalculationGroup, CalculationGroup targetCalculationGroup)
            {
                this.sourceCalculationGroup = sourceCalculationGroup;
                TargetCalculationGroup = targetCalculationGroup;
            }

            /// <summary>
            /// Performs the drag and drop operation.
            /// </summary>
            /// <param name="draggedData">The dragged data.</param>
            /// <param name="calculationBase">The calculation item wrapped by <see cref="draggedData"/>.</param>
            /// <param name="newPosition">The index of the new position within the new owner's collection.</param>
            /// <param name="treeViewControl">The tree view control in which the drag and drop operation is performed.</param>
            public virtual void Execute(object draggedData, ICalculationBase calculationBase, int newPosition, TreeViewControl treeViewControl)
            {
                MoveCalculationItemToNewOwner(calculationBase, newPosition);

                NotifyObservers();
            }

            /// <summary>
            /// Gets the target <see cref="CalculationGroup"/>.
            /// </summary>
            protected CalculationGroup TargetCalculationGroup { get; }

            /// <summary>
            /// Moves the <see cref="ICalculationBase"/> instance to its new location.
            /// </summary>
            /// <param name="calculationBase">The instance to be relocated.</param>
            /// <param name="position">The index in the new <see cref="CalculationGroup"/>
            /// owner within its <see cref="CalculationGroup.Children"/>.</param>
            protected void MoveCalculationItemToNewOwner(ICalculationBase calculationBase, int position)
            {
                sourceCalculationGroup.Children.Remove(calculationBase);
                TargetCalculationGroup.Children.Insert(position, calculationBase);
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
                if (TargetCalculationGroup.Children.Except(new[]
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
                TargetCalculationGroup.NotifyObservers();
            }
        }

        #endregion

        #endregion
    }
}