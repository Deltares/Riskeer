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
    public class RingtoetsContextMenuItemFactory
    {
        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of adding new calculation groups.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to add the new calculation groups to.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateAddCalculationGroupItem(CalculationGroup calculationGroup)
        {
            return new StrictContextMenuItem(
                Resources.CalculationGroup_Add_CalculationGroup,
                Resources.CalculationGroup_Add_CalculationGroup_Tooltip,
                Resources.AddFolderIcon,
                (o, args) => CreateCalculationGroup(calculationGroup));
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of adding new calculations.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="addCalculationAction">The action for adding a calculation to the calculation group.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateAddCalculationItem<TCalculationContext>(
            TCalculationContext calculationGroupContext,
            Action<TCalculationContext> addCalculationAction)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            return new StrictContextMenuItem(
                Resources.CalculationGroup_Add_Calculation,
                Resources.CalculationGroup_Add_Calculation_Tooltip,
                Resources.FailureMechanismIcon,
                (o, args) => addCalculationAction(calculationGroupContext));
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of clearing the output of all calculations in the calculation group.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to clear the output for.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateClearAllCalculationOutputInGroupItem(CalculationGroup calculationGroup)
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

            return clearAllItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of performing all calculations in a calculation group.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroup">The calculation group to perform all calculations for.</param>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <param name="isEnabledFunc">The func that checks if the item is enabled.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreatePerformAllCalculationsInGroupItem<TCalculationContext>(
            CalculationGroup calculationGroup,
            TCalculationContext calculationGroupContext,
            Action<CalculationGroup, TCalculationContext> calculateAllAction,
            Func<TCalculationContext, bool> isEnabledFunc)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            var performAllItem = new StrictContextMenuItem(
                Resources.Calculate_all,
                Resources.CalculationGroup_CalculateAll_ToolTip,
                Resources.CalculateAllIcon,
                (o, args) => calculateAllAction(calculationGroup, calculationGroupContext))
            {
                Enabled = isEnabledFunc(calculationGroupContext)
            };

            if (!performAllItem.Enabled)
            {
                return performAllItem;
            }

            if (!calculationGroup.GetCalculations().Any())
            {
                performAllItem.Enabled = false;
                performAllItem.ToolTipText = Resources.CalculationGroup_CalculateAll_No_calculations_to_run;
            }

            return performAllItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of performing a calculation.
        /// </summary>
        /// <typeparam name="TCalculation">The type of the calculation.</typeparam>
        /// <typeparam name="TCalculationContext">The type of the calculation context.</typeparam>
        /// <param name="calculation">The calculation to perform.</param>
        /// <param name="calculationContext">The calculation context belonging to the calculation.</param>
        /// <param name="calculateAction">The action that performs the calculation.</param>
        /// <param name="isEnabledFunc">The func that checks if the item is enabled.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreatePerformCalculationItem<TCalculation, TCalculationContext>(
            TCalculation calculation,
            TCalculationContext calculationContext,
            Action<TCalculation, TCalculationContext> calculateAction,
            Func<TCalculationContext, bool> isEnabledFunc)
            where TCalculationContext : ICalculationContext<TCalculation, IFailureMechanism>
            where TCalculation : ICalculation
        {
            return new StrictContextMenuItem(
                Resources.Calculate,
                Resources.Calculate_ToolTip,
                Resources.CalculateIcon,
                (o, args) => calculateAction(calculation, calculationContext))
            {
                Enabled = isEnabledFunc(calculationContext)
            };
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of clearing the output of a calculation.
        /// </summary>
        /// <param name="calculation">The calculation to clear the output for.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateClearCalculationOutputItem(ICalculation calculation)
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

            return clearOutputItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of changing the relevancy state of a disabled failure mechanism.
        /// </summary>
        /// <param name="failureMechanismContext">The failure mechanism context belonging to the failure mechanism.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateDisabledChangeRelevancyItem(IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
        {
            return new StrictContextMenuItem(
                Resources.FailureMechanismContextMenuStrip_Is_relevant,
                Resources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                Resources.Checkbox_empty,
                (o, args) =>
                {
                    failureMechanismContext.WrappedData.IsRelevant = true;
                    failureMechanismContext.WrappedData.NotifyObservers();
                });
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of clearing the output of all calculations in the failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to clear the output for.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateClearAllCalculationOutputInFailureMechanismItem(IFailureMechanism failureMechanism)
        {
            var clearAllItem = new StrictContextMenuItem(
                Resources.Clear_all_output,
                Resources.Clear_all_output_ToolTip,
                Resources.ClearIcon,
                (o, args) => ClearAllCalculationOutputInFailureMechanism(failureMechanism));

            if (!failureMechanism.Calculations.Any(c => c.HasOutput))
            {
                clearAllItem.Enabled = false;
                clearAllItem.ToolTipText = Resources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear;
            }
            return clearAllItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of performing all calculations in a failure mechanism.
        /// </summary>
        /// <typeparam name="TFailureMechanismContext">The type of the failure mechanism context.</typeparam>
        /// <param name="failureMechanismContext">The failure mechanism to perform all calculations for.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreatePerformAllCalculationsInFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanismContext,
            Action<TFailureMechanismContext> calculateAllAction)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            var performAllItem = new StrictContextMenuItem(
                Resources.Calculate_all,
                Resources.Calculate_all_ToolTip,
                Resources.CalculateAllIcon,
                (o, args) => calculateAllAction(failureMechanismContext)
                );

            if (!failureMechanismContext.WrappedData.Calculations.Any())
            {
                performAllItem.Enabled = false;
                performAllItem.ToolTipText = Resources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run;
            }

            return performAllItem;
        }

        private static void ClearAllCalculationOutputInFailureMechanism(IFailureMechanism failureMechanism)
        {
            if (MessageBox.Show(Resources.FailureMechanism_ContextMenuStrip_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            foreach (var calc in failureMechanism.Calculations.Where(c => c.HasOutput))
            {
                calc.ClearOutput();
                calc.NotifyObservers();
            }
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