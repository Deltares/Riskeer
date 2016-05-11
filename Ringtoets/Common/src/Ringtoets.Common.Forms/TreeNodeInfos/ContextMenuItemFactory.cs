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
using System.Linq;
using System.Windows.Forms;
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
    /// This class represents a factory for creating <see cref="ToolStripItem"/>.
    /// </summary>
    public static class ContextMenuItemFactory
    {
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
                (o, args) => CreateCalculationGroup(calculationGroup));

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
                (o, args) => addCalculation(calculationGroupContext));

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
                Resources.ClearIcon,
                (o, args) => ClearAllCalculationOutputInGroup(calculationGroup));

            if (!calculationGroup.HasOutput())
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
                Resources.CalculateAllIcon,
                (o, args) => calculateAll(calculationGroup, context));

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
            where TCalculation : ICalculation
            where TCalculationContext : ICalculationContext<ICalculation, IFailureMechanism>
        {
            var calculateItem = new StrictContextMenuItem(
                Resources.Calculate,
                Resources.Calculate_ToolTip,
                Resources.CalculateIcon,
                (o, args) => calculate(calculation, context));

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
                (o, args) => ClearCalculationOutput(calculation));

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
                (o, args) =>
                {
                    failureMechanismContext.WrappedData.IsRelevant = true;
                    failureMechanismContext.WrappedData.NotifyObservers();
                });

            builder.AddCustomItem(changeRelevancyItem);
        }

        private static void CreateCalculationGroup(CalculationGroup calculationGroup)
        {
            calculationGroup.Children.Add(new CalculationGroup
            {
                Name = NamingHelper.GetUniqueName(calculationGroup.Children, RingtoetsCommonDataResources.CalculationGroup_DefaultName, c => c.Name)
            });

            calculationGroup.NotifyObservers();
        }

        private static void ClearAllCalculationOutputInGroup(CalculationGroup calculationGroup)
        {
            if (MessageBox.Show(Resources.CalculationGroup_ClearOutput_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            calculationGroup.ClearCalculationOutput();
        }

        private static void ClearCalculationOutput(ICalculation calculation)
        {
            if (MessageBox.Show(Resources.Calculation_ContextMenuStrip_Are_you_sure_clear_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            calculation.ClearOutput();
            calculation.NotifyObservers();
        }
    }
}