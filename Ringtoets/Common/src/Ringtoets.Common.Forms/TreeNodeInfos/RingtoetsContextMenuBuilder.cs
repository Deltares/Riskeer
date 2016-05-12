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
using System.Windows.Forms;
using Core.Common.Gui.ContextMenu;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.TreeNodeInfos
{
    /// <summary>
    /// Decorator for <see cref="ContextMenuBuilder"/>.
    /// </summary>
    public class RingtoetsContextMenuBuilder
    {
        private readonly IContextMenuBuilder contextMenuBuilder;
        private readonly RingtoetsContextMenuItemFactory ringtoetsContextMenuItemFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsContextMenuBuilder"/>.
        /// </summary>
        /// <param name="contextMenuBuilder">The context menu builder to decorate.</param>
        public RingtoetsContextMenuBuilder(IContextMenuBuilder contextMenuBuilder)
        {
            this.contextMenuBuilder = contextMenuBuilder;

            ringtoetsContextMenuItemFactory = new RingtoetsContextMenuItemFactory();
        }

        public RingtoetsContextMenuBuilder AddCreateCalculationGroupItem(CalculationGroup calculationGroup)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup));
            return this;
        }

        public RingtoetsContextMenuBuilder AddCreateCalculationItem<TCalculationContext>(
            TCalculationContext calculationGroupContext,
            Action<TCalculationContext> addCalculation)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, addCalculation));
            return this;
        }

        public RingtoetsContextMenuBuilder AddClearAllCalculationOutputInGroupItem(CalculationGroup calculationGroup)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup));
            return this;
        }

        public RingtoetsContextMenuBuilder AddPerformAllCalculationsInGroupItem<TCalculationContext>(
            CalculationGroup calculationGroup,
            TCalculationContext context,
            Action<CalculationGroup, TCalculationContext> calculateAll)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, context, calculateAll));
            return this;
        }

        public RingtoetsContextMenuBuilder AddPerformCalculationItem<TCalculation, TCalculationContext>(
            TCalculation calculation,
            TCalculationContext context,
            Action<TCalculation, TCalculationContext> calculate)
            where TCalculationContext : ICalculationContext<TCalculation, IFailureMechanism>
            where TCalculation : ICalculation
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, context, calculate));
            return this;
        }

        public RingtoetsContextMenuBuilder AddClearCalculationOutputItem(ICalculation calculation)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculation));
            return this;
        }

        public RingtoetsContextMenuBuilder AddDisabledChangeRelevancyItem(IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
        {
            contextMenuBuilder.AddCustomItem(ringtoetsContextMenuItemFactory.CreateDisabledChangeRelevancyItem(failureMechanismContext));
            return this;
        }

        # region Decorated interface members

        public RingtoetsContextMenuBuilder AddRenameItem()
        {
            contextMenuBuilder.AddRenameItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddDeleteItem()
        {
            contextMenuBuilder.AddDeleteItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddExpandAllItem()
        {
            contextMenuBuilder.AddExpandAllItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddCollapseAllItem()
        {
            contextMenuBuilder.AddCollapseAllItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddOpenItem()
        {
            contextMenuBuilder.AddOpenItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddExportItem()
        {
            contextMenuBuilder.AddExportItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddImportItem()
        {
            contextMenuBuilder.AddImportItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddPropertiesItem()
        {
            contextMenuBuilder.AddPropertiesItem();
            return this;
        }

        public RingtoetsContextMenuBuilder AddSeparator()
        {
            contextMenuBuilder.AddSeparator();
            return this;
        }

        public RingtoetsContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            contextMenuBuilder.AddCustomItem(item);
            return this;
        }

        public ContextMenuStrip Build()
        {
            return contextMenuBuilder.Build();
        }

        # endregion
    }
}
