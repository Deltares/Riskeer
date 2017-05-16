﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.ChangeHandlers;
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
    public static class RingtoetsContextMenuItemFactory
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
        /// <typeparam name="TCalculationGroupContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroup">The calculation group to perform all calculations for.</param>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <param name="enableMenuItemFunction">The function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreatePerformAllCalculationsInGroupItem<TCalculationGroupContext>(
            CalculationGroup calculationGroup,
            TCalculationGroupContext calculationGroupContext,
            Action<CalculationGroup, TCalculationGroupContext> calculateAllAction,
            Func<TCalculationGroupContext, string> enableMenuItemFunction)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            var menuItem = new StrictContextMenuItem(
                Resources.Calculate_all,
                Resources.CalculationGroup_CalculateAll_ToolTip,
                Resources.CalculateAllIcon,
                (o, args) => calculateAllAction(calculationGroup, calculationGroupContext));

            if (!calculationGroupContext.WrappedData.GetCalculations().Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = Resources.CalculationGroup_CalculateAll_No_calculations_to_run;
            }
            else
            {
                SetStateWithEnableFunction(calculationGroupContext, enableMenuItemFunction, menuItem);
            }

            return menuItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of validating the input of each calculation 
        /// in a calculation group.
        /// </summary>
        /// <typeparam name="TCalculationGroupContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="validateAllAction">The action that validates all calculations.</param>
        /// <param name="enableMenuItemFunction">The function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateValidateAllCalculationsInGroupItem<TCalculationGroupContext>(
            TCalculationGroupContext calculationGroupContext,
            Action<TCalculationGroupContext> validateAllAction,
            Func<TCalculationGroupContext, string> enableMenuItemFunction)
            where TCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            CalculationGroup calculationGroup = calculationGroupContext.WrappedData;
            var menuItem = new StrictContextMenuItem(
                Resources.Validate_all,
                Resources.CalculationGroup_Validate_all_ToolTip,
                Resources.ValidateAllIcon,
                (o, args) => validateAllAction(calculationGroupContext));

            if (!calculationGroup.GetCalculations().Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = Resources.ValidateAll_No_calculations_to_validate;
            }
            else
            {
                SetStateWithEnableFunction(calculationGroupContext, enableMenuItemFunction, menuItem);
            }

            return menuItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of performing a calculation.
        /// </summary>
        /// <typeparam name="TCalculation">The type of the calculation.</typeparam>
        /// <typeparam name="TCalculationContext">The type of the calculation context.</typeparam>
        /// <param name="calculation">The calculation to perform.</param>
        /// <param name="calculationContext">The calculation context belonging to the calculation.</param>
        /// <param name="calculateAction">The action that performs the calculation.</param>
        /// <param name="enableMenuItemFunction">The function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreatePerformCalculationItem<TCalculation, TCalculationContext>(
            TCalculation calculation,
            TCalculationContext calculationContext,
            Action<TCalculation, TCalculationContext> calculateAction,
            Func<TCalculationContext, string> enableMenuItemFunction)
            where TCalculationContext : ICalculationContext<TCalculation, IFailureMechanism>
            where TCalculation : ICalculation
        {
            var menuItem = new StrictContextMenuItem(
                Resources.Calculate,
                Resources.Calculate_ToolTip,
                Resources.CalculateIcon,
                (o, args) => calculateAction(calculation, calculationContext));

            SetStateWithEnableFunction(calculationContext, enableMenuItemFunction, menuItem);
            return menuItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of validating the input of a calculation.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation context.</typeparam>
        /// <param name="calculation">The context containing the calculation to validate.</param>
        /// <param name="validateAction">The action that performs the validation.</param>
        /// <param name="enableMenuItemFunction">The function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateValidateCalculationItem<TCalculationContext>(
            TCalculationContext calculation,
            Action<TCalculationContext> validateAction,
            Func<TCalculationContext, string> enableMenuItemFunction)
            where TCalculationContext : ICalculationContext<ICalculation, IFailureMechanism>
        {
            var menuItem = new StrictContextMenuItem(
                Resources.Validate,
                Resources.Validate_ToolTip,
                Resources.ValidateIcon,
                (o, args) => validateAction(calculation));

            SetStateWithEnableFunction(calculation, enableMenuItemFunction, menuItem);
            return menuItem;
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
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of clearing the output of all calculations in a failure mechanism.
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
        /// <param name="failureMechanismContext">The failure mechanism context belonging to the failure mechanism.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <param name="enableMenuItemFunction">The function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        /// <remarks>When <paramref name="enableMenuItemFunction"/> returns a <c>string</c>, the item will be disabled and the <c>string</c> will be shown in the tooltip.</remarks>
        public static StrictContextMenuItem CreatePerformAllCalculationsInFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanismContext,
            Action<TFailureMechanismContext> calculateAllAction,
            Func<TFailureMechanismContext, string> enableMenuItemFunction)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            var menuItem = new StrictContextMenuItem(
                Resources.Calculate_all,
                Resources.Calculate_all_ToolTip,
                Resources.CalculateAllIcon,
                (o, args) => calculateAllAction(failureMechanismContext));

            if (!failureMechanismContext.WrappedData.Calculations.Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = Resources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run;
            }
            else
            {
                SetStateWithEnableFunction(failureMechanismContext, enableMenuItemFunction, menuItem);
            }

            return menuItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of performing all calculations in a failure mechanism.
        /// </summary>
        /// <typeparam name="TFailureMechanismContext">The type of the failure mechanism.</typeparam>
        /// <param name="failureMechanism">The failure mechanism to validate the calculations of.</param>
        /// <param name="validateAllAction">The action that validates all calculations.</param>
        /// <param name="enableMenuItemFunction">The function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateValidateAllCalculationsInFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanism,
            Action<TFailureMechanismContext> validateAllAction,
            Func<TFailureMechanismContext, string> enableMenuItemFunction)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            var menuItem = new StrictContextMenuItem(
                Resources.Validate_all,
                Resources.FailureMechanism_Validate_all_ToolTip,
                Resources.ValidateAllIcon,
                (o, args) => validateAllAction(failureMechanism));

            if (!failureMechanism.WrappedData.Calculations.Any())
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = Resources.ValidateAll_No_calculations_to_validate;
            }
            else
            {
                SetStateWithEnableFunction(failureMechanism, enableMenuItemFunction, menuItem);
            }

            return menuItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action of changing the relevance of a failure mechanism.
        /// </summary>
        /// <typeparam name="TFailureMechanismContext">The type of the failure mechanism context.</typeparam>
        /// <param name="failureMechanismContext">The failure mechanism context belonging to the failure mechanism.</param>
        /// <param name="onChangeAction">The action to perform when relevance changes.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateToggleRelevancyOfFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanismContext,
            Action<TFailureMechanismContext> onChangeAction)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            bool isRelevant = failureMechanismContext.WrappedData.IsRelevant;
            Bitmap checkboxImage = isRelevant ? Resources.Checkbox_ticked : Resources.Checkbox_empty;
            return new StrictContextMenuItem(
                Resources.FailureMechanismContextMenuStrip_Is_relevant,
                Resources.FailureMechanism_IsRelevant_Description,
                checkboxImage,
                (sender, args) =>
                {
                    onChangeAction?.Invoke(failureMechanismContext);

                    failureMechanismContext.WrappedData.IsRelevant = !isRelevant;
                    failureMechanismContext.WrappedData.NotifyObservers();
                });
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action when updating
        /// the <see cref="ForeshoreProfile"/> of a <paramref name="calculation"/>.
        /// </summary>
        /// <typeparam name="TCalculationInput">The type of calculation input that has can have a foreshore profile.</typeparam>
        /// <param name="calculation">The calculation to update.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <param name="updateAction">The action to perform when the foreshore profile is updated.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateUpdateForshoreProfileOfCalculationItem<TCalculationInput>(
            ICalculation<TCalculationInput> calculation,
            IInquiryHelper inquiryHelper,
            Action<ICalculation<TCalculationInput>> updateAction)
            where TCalculationInput : ICalculationInput, IHasForeshoreProfile
        {
            TCalculationInput input = calculation.InputParameters;
            bool hasForeshoreProfile = input.ForeshoreProfile != null && !input.IsForeshoreProfileInputSynchronized;
            string toolTipMessage = hasForeshoreProfile
                                        ? Resources.CreateUpdateForshoreProfileOfCalculationItem_Update_calculation_with_ForeshoreProfile_ToolTip
                                        : Resources.CreateUpdateForshoreProfileOfCalculationItem_Update_calculation_no_ForeshoreProfile_ToolTip;

            string confirmOutputMessage = Resources.UpdateForshoreProfileOfCalculation_Confirm_calculation_output_cleared_when_updating_ForeshoreProfile_dependent_data;

            var menuItem = new StrictContextMenuItem(
                Resources.CreateUpdateForshoreProfileOfCalculationItem_Update_ForeshoreProfile_data,
                toolTipMessage,
                Resources.UpdateItemIcon,
                (o, args) =>
                {
                    UpdateForeshoreProfileDependentDataOfCalculation(new[]
                                                                     {
                                                                         calculation
                                                                     },
                                                                     inquiryHelper,
                                                                     confirmOutputMessage,
                                                                     updateAction);
                })
            {
                Enabled = hasForeshoreProfile
            };

            return menuItem;
        }

        /// <summary>
        /// Creates a <see cref="StrictContextMenuItem"/> which is bound to the action when updating
        /// the <see cref="ForeshoreProfile"/> of the <paramref name="calculations"/>.
        /// </summary>
        /// <typeparam name="TCalculationInput">The type of calculation input that has can have a foreshore profile.</typeparam>
        /// <param name="calculations">The calculations to update.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <param name="updateAction">The action to perform when the foreshore profile is updated.</param>
        /// <returns>The created <see cref="StrictContextMenuItem"/>.</returns>
        public static StrictContextMenuItem CreateUpdateForshoreProfileOfCalculationsItem<TCalculationInput>(
            IEnumerable<ICalculation<TCalculationInput>> calculations,
            IInquiryHelper inquiryHelper,
            Action<ICalculation<TCalculationInput>> updateAction)
            where TCalculationInput : ICalculationInput, IHasForeshoreProfile
        {
            ICalculation<TCalculationInput>[] calculationsWithForeshoreProfileChanges = calculations.Where(
                c => c.InputParameters.ForeshoreProfile != null
                     && !c.InputParameters.IsForeshoreProfileInputSynchronized).ToArray();

            bool hasForeshoreProfileChanges = calculationsWithForeshoreProfileChanges.Any();

            string toolTipMessage = hasForeshoreProfileChanges
                                        ? Resources.CreateUpdateForshoreProfileOfCalculationsItem_Update_calculations_with_ForeshoreProfile_ToolTip
                                        : Resources.CreateUpdateForshoreProfileOfCalculationsItem_Update_calculations_no_ForeshoreProfile_changes_ToolTip;

            string confirmOutputMessage = Resources.UpdateForshoreProfileOfCalculations_Confirm_calculation_outputs_cleared_when_updating_ForeshoreProfile_dependent_data;

            var menuItem = new StrictContextMenuItem(
                Resources.CreateUpdateForshoreProfileOfCalculationsItem_Update_ForeshoreProfile_data,
                toolTipMessage,
                Resources.UpdateItemIcon,
                (o, args) =>
                {
                    UpdateForeshoreProfileDependentDataOfCalculation(calculationsWithForeshoreProfileChanges,
                                                                     inquiryHelper,
                                                                     confirmOutputMessage,
                                                                     updateAction);
                })
            {
                Enabled = hasForeshoreProfileChanges
            };

            return menuItem;
        }

        private static void UpdateForeshoreProfileDependentDataOfCalculation<TCalculationInput>(
            ICalculation<TCalculationInput>[] calculations,
            IInquiryHelper inquiryHelper,
            string confirmOutputMessage,
            Action<ICalculation<TCalculationInput>> updateAction)
            where TCalculationInput : ICalculationInput, IHasForeshoreProfile
        {
            if (ForeshoreProfileDependentDataShouldUpdate(calculations, confirmOutputMessage, inquiryHelper))
            {
                calculations.ForEachElementDo(updateAction);
            }
        }

        private static bool ForeshoreProfileDependentDataShouldUpdate(IEnumerable<ICalculation> calculations,
                                                                      string query,
                                                                      IInquiryHelper inquiryHelper)
        {
            var changeHandler = new CalculationChangeHandler(calculations,
                                                             query,
                                                             inquiryHelper);

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void SetStateWithEnableFunction<T>(T context, Func<T, string> enableFunction, StrictContextMenuItem menuItem)
        {
            string validationText = enableFunction?.Invoke(context);
            if (!string.IsNullOrEmpty(validationText))
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = validationText;
            }
        }

        private static void ClearAllCalculationOutputInFailureMechanism(IFailureMechanism failureMechanism)
        {
            if (MessageBox.Show(Resources.FailureMechanism_ContextMenuStrip_Are_you_sure_clear_all_output, BaseResources.Confirm, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            foreach (ICalculation calc in failureMechanism.Calculations.Where(c => c.HasOutput))
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