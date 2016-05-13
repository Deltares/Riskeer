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
using System.Windows.Forms;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.TreeNodeInfos
{
    /// <summary>
    /// Decorator for <see cref="IContextMenuBuilder"/>.
    /// </summary>
    public class RingtoetsContextMenuBuilder
    {
        private readonly IContextMenuBuilder contextMenuBuilder;
        private readonly RingtoetsContextMenuItemFactory ringtoetsContextMenuItemFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsContextMenuBuilder"/> class.
        /// </summary>
        /// <param name="contextMenuBuilder">The context menu builder to decorate.</param>
        public RingtoetsContextMenuBuilder(IContextMenuBuilder contextMenuBuilder)
        {
            this.contextMenuBuilder = contextMenuBuilder;

            ringtoetsContextMenuItemFactory = new RingtoetsContextMenuItemFactory();
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which adds a new calculation group to a calculation group.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to add the new calculation groups to.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddCreateCalculationGroupItem(CalculationGroup calculationGroup)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which adds a new calculation to a calculation group.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="addCalculationAction">The action for adding a calculation to the calculation group.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddCreateCalculationItem<TCalculationContext>(
            TCalculationContext calculationGroupContext,
            Action<TCalculationContext> addCalculationAction)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, addCalculationAction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which clears the output of all calculations in a calculation group.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to clear the output for.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddClearAllCalculationOutputInGroupItem(CalculationGroup calculationGroup)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which performs all calculations in a calculation group.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroup">The calculation group to perform all calculations for.</param>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddPerformAllCalculationsInGroupItem<TCalculationContext>(
            CalculationGroup calculationGroup,
            TCalculationContext calculationGroupContext,
            Action<CalculationGroup, TCalculationContext> calculateAllAction)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, calculateAllAction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which performs a calculation.
        /// </summary>
        /// <typeparam name="TCalculation">The type of the calculation.</typeparam>
        /// <typeparam name="TCalculationContext">The type of the calculation context.</typeparam>
        /// <param name="calculation">The calculation to perform.</param>
        /// <param name="calculationContext">The calculation context belonging to the calculation.</param>
        /// <param name="calculateAction">The action that performs the calculation.</param>
        /// <param name="isEnabledFunc">The func that checks if the item is enabled.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddPerformCalculationItem<TCalculation, TCalculationContext>(
            TCalculation calculation,
            TCalculationContext calculationContext,
            Action<TCalculation, TCalculationContext> calculateAction,
            Func<TCalculationContext, bool> isEnabledFunc)
            where TCalculationContext : ICalculationContext<TCalculation, IFailureMechanism>
            where TCalculation : ICalculation
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, calculateAction, isEnabledFunc));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which clears the output of a calculation.
        /// </summary>
        /// <param name="calculation">The calculation to clear the output for.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddClearCalculationOutputItem(ICalculation calculation)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculation));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which enables a disabled failure mechanism.
        /// </summary>
        /// <param name="failureMechanismContext">The failure mechanism context belonging to the failure mechanism.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddDisabledChangeRelevancyItem(IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateDisabledChangeRelevancyItem(failureMechanismContext));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which clears the output of all calculations in the failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to clear the output for.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddClearAllCalculationOutputInFailureMechanismItem(IFailureMechanism failureMechanism)
        {
            contextMenuBuilder.AddCustomItem(RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which performs all calculations in a failure mechanism.
        /// </summary>
        /// <typeparam name="TFailureMechanismContext">The type of the failure mechanism context.</typeparam>
        /// <param name="failureMechanismContext">The failure mechanism to perform all calculations for.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddPerformAllCalculationsInFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanismContext,
            Action<TFailureMechanismContext> calculateAllAction)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, calculateAllAction));
            return this;
        }

        # region Decorated members

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which starts edit mode for the name of <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddRenameItem()
        {
            contextMenuBuilder.AddRenameItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which deletes the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddDeleteItem()
        {
            contextMenuBuilder.AddDeleteItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which expands the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddExpandAllItem()
        {
            contextMenuBuilder.AddExpandAllItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which collapses the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddCollapseAllItem()
        {
            contextMenuBuilder.AddCollapseAllItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which opens a view for the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddOpenItem()
        {
            contextMenuBuilder.AddOpenItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which exports the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddExportItem()
        {
            contextMenuBuilder.AddExportItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which imports to the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddImportItem()
        {
            contextMenuBuilder.AddImportItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which shows properties of the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddPropertiesItem()
        {
            contextMenuBuilder.AddPropertiesItem();
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ToolStripSeparator"/> to the <see cref="ContextMenuStrip"/>. A <see cref="ToolStripSeparator"/>
        /// is only added if the last item that was added to the <see cref="ContextMenuStrip"/> exists and is not a 
        /// <see cref="ToolStripSeparator"/>.
        /// </summary>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddSeparator()
        {
            contextMenuBuilder.AddSeparator();
            return this;
        }

        /// <summary>
        /// Adds a custom item to the <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="item">The custom <see cref="StrictContextMenuItem"/> to add to the <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="RingtoetsContextMenuBuilder"/> itself.</returns>
        public RingtoetsContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            contextMenuBuilder.AddCustomItem(item);
            return this;
        }

        /// <summary>
        /// Obtain the <see cref="ContextMenuStrip"/>, which has been constructed by using the other methods of
        /// <see cref="RingtoetsContextMenuBuilder"/>.
        /// </summary>
        /// <returns>The constructed <see cref="ContextMenuStrip"/>.</returns>
        public ContextMenuStrip Build()
        {
            return contextMenuBuilder.Build();
        }

        # endregion
    }
}