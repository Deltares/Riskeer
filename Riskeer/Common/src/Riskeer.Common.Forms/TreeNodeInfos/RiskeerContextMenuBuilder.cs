// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.TreeNodeInfos
{
    /// <summary>
    /// Decorator for <see cref="IContextMenuBuilder"/>.
    /// </summary>
    public class RiskeerContextMenuBuilder
    {
        private readonly IContextMenuBuilder contextMenuBuilder;

        /// <summary>
        /// Creates a new instance of the <see cref="RiskeerContextMenuBuilder"/> class.
        /// </summary>
        /// <param name="contextMenuBuilder">The context menu builder to decorate.</param>
        public RiskeerContextMenuBuilder(IContextMenuBuilder contextMenuBuilder)
        {
            this.contextMenuBuilder = contextMenuBuilder;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which adds a new calculation group to a calculation group.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to add the new calculation groups to.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddCreateCalculationGroupItem(CalculationGroup calculationGroup)
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which adds a new calculation to a calculation group.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="addCalculationAction">The action for adding a calculation to the calculation group.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddCreateCalculationItem<TCalculationContext>(
            TCalculationContext calculationGroupContext,
            Action<TCalculationContext> addCalculationAction)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, addCalculationAction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which clears the output of all calculations in a calculation group.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to clear the output for.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddClearAllCalculationOutputInGroupItem(CalculationGroup calculationGroup)
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which performs all calculations in a calculation group.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroup">The calculation group to perform all calculations for.</param>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <param name="enableMenuItemFunction">An optional function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddPerformAllCalculationsInGroupItem<TCalculationContext>(
            CalculationGroup calculationGroup,
            TCalculationContext calculationGroupContext,
            Action<CalculationGroup, TCalculationContext> calculateAllAction,
            Func<TCalculationContext, string> enableMenuItemFunction = null)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, calculateAllAction, enableMenuItemFunction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which validates all calculations in a calculation group.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation group context.</typeparam>
        /// <param name="calculationGroupContext">The calculation group context belonging to the calculation group.</param>
        /// <param name="validateAllAction">The action that validates all calculations.</param>
        /// <param name="enableMenuItemFunction">An optional function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddValidateAllCalculationsInGroupItem<TCalculationContext>(TCalculationContext calculationGroupContext,
                                                                                                    Action<TCalculationContext> validateAllAction,
                                                                                                    Func<TCalculationContext, string> enableMenuItemFunction = null)
            where TCalculationContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, validateAllAction, enableMenuItemFunction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which duplicates a calculation item.
        /// </summary>
        /// <typeparam name="TCalculationItem">The type of the calculation item.</typeparam>
        /// <typeparam name="TCalculationItemContext">The type of the calculation item context.</typeparam>
        /// <param name="calculationItem">The calculation item to duplicate.</param>
        /// <param name="calculationItemContext">The calculation item context belonging to the calculation item.</param>
        /// <exception cref="ArgumentException">Thrown when the parent calculation group of
        /// <paramref name="calculationItem"/> equals <c>null</c>.</exception>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddDuplicateCalculationItem<TCalculationItem, TCalculationItemContext>(
            TCalculationItem calculationItem,
            TCalculationItemContext calculationItemContext)
            where TCalculationItemContext : ICalculationContext<TCalculationItem, IFailureMechanism>
            where TCalculationItem : ICalculationBase
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateDuplicateCalculationItem(calculationItem, calculationItemContext));
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
        /// <param name="enableMenuItemFunction">An optional function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddPerformCalculationItem<TCalculation, TCalculationContext>(
            TCalculation calculation,
            TCalculationContext calculationContext,
            Action<TCalculation, TCalculationContext> calculateAction,
            Func<TCalculationContext, string> enableMenuItemFunction = null)
            where TCalculationContext : ICalculationContext<TCalculation, IFailureMechanism>
            where TCalculation : ICalculation
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, calculateAction, enableMenuItemFunction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which validates a calculation.
        /// </summary>
        /// <typeparam name="TCalculationContext">The type of the calculation context.</typeparam>
        /// <param name="calculationContext">The context containing the calculation to validate.</param>
        /// <param name="validateAction">The action that validates the calculation.</param>
        /// <param name="enableMenuItemFunction">An optional function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddValidateCalculationItem<TCalculationContext>(
            TCalculationContext calculationContext,
            Action<TCalculationContext> validateAction,
            Func<TCalculationContext, string> enableMenuItemFunction = null)
            where TCalculationContext : ICalculationContext<ICalculation, IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, validateAction, enableMenuItemFunction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which clears the output of a calculation.
        /// </summary>
        /// <param name="calculation">The calculation to clear the output for.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddClearCalculationOutputItem(ICalculation calculation)
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateClearCalculationOutputItem(calculation));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which clears the output of all calculations in the failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to clear the output for.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddClearAllCalculationOutputInFailureMechanismItem(IFailureMechanism failureMechanism)
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which performs all calculations in a failure mechanism.
        /// </summary>
        /// <typeparam name="TFailureMechanismContext">The type of the failure mechanism context.</typeparam>
        /// <param name="failureMechanismContext">The failure mechanism context belonging to the failure mechanism.</param>
        /// <param name="calculateAllAction">The action that performs all calculations.</param>
        /// <param name="enableMenuItemFunction">An optional function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        /// <remarks>When <paramref name="enableMenuItemFunction"/> returns a <c>string</c>, the item will be disabled and the <c>string</c> will be shown in the tooltip.</remarks>
        public RiskeerContextMenuBuilder AddPerformAllCalculationsInFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanismContext,
            Action<TFailureMechanismContext> calculateAllAction,
            Func<TFailureMechanismContext, string> enableMenuItemFunction = null)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, calculateAllAction, enableMenuItemFunction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which validates all calculations in a failure mechanism.
        /// </summary>
        /// <typeparam name="TFailureMechanismContext">The type of the failure mechanism context.</typeparam>
        /// <param name="failureMechanism">The failure mechanism context belonging to the failure mechanism.</param>
        /// <param name="validateAllAction">The action that validates all calculations.</param>
        /// <param name="enableMenuItemFunction">An optional function which determines whether the item should be enabled. If the 
        /// item should not be enabled, then the reason for that should be returned by the function and will be shown as a tooltip. 
        /// If the item should be enabled then the function should return a <c>null</c> or empty string.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        /// <remarks>When <paramref name="enableMenuItemFunction"/> returns a <c>string</c>, the item will be disabled and the <c>string</c> will be shown in the tooltip.</remarks>
        public RiskeerContextMenuBuilder AddValidateAllCalculationsInFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanism,
            Action<TFailureMechanismContext> validateAllAction,
            Func<TFailureMechanismContext, string> enableMenuItemFunction = null)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(failureMechanism, validateAllAction, enableMenuItemFunction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which sets if the failure mechanism is relevant.
        /// </summary>
        /// <typeparam name="TFailureMechanismContext">The type of the failure mechanism context.</typeparam>
        /// <param name="failureMechanismContext">The failure mechanism context belonging to the failure mechanism.</param>
        /// <param name="onChangeAction">The action to perform when relevance changes.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddToggleRelevancyOfFailureMechanismItem<TFailureMechanismContext>(
            TFailureMechanismContext failureMechanismContext,
            Action<TFailureMechanismContext> onChangeAction)
            where TFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateToggleRelevancyOfFailureMechanismItem(failureMechanismContext, onChangeAction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which is bound to the action 
        /// of updating the <see cref="ForeshoreProfile"/> of a <paramref name="calculation"/>.
        /// </summary>
        /// <typeparam name="TCalculationInput">The type of calculation input that can have 
        /// a foreshore profile.</typeparam>
        /// <param name="calculation">The calculation to update.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <param name="updateAction">The action to perform when the foreshore profile is updated.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddUpdateForeshoreProfileOfCalculationItem<TCalculationInput>(
            ICalculation<TCalculationInput> calculation,
            IInquiryHelper inquiryHelper,
            Action<ICalculation<TCalculationInput>> updateAction)
            where TCalculationInput : ICalculationInput, IHasForeshoreProfile
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationItem(
                                                 calculation,
                                                 inquiryHelper,
                                                 updateAction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which is bound to the action 
        /// of updating the <see cref="ForeshoreProfile"/> of the <paramref name="calculations"/>.
        /// </summary>
        /// <typeparam name="TCalculationInput">The type of calculation input that can have 
        /// a foreshore profile.</typeparam>
        /// <param name="calculations">The calculations to update.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring the required data.</param>
        /// <param name="updateAction">The action to perform when the foreshore profile is updated.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddUpdateForeshoreProfileOfCalculationsItem<TCalculationInput>(
            IEnumerable<ICalculation<TCalculationInput>> calculations,
            IInquiryHelper inquiryHelper,
            Action<ICalculation<TCalculationInput>> updateAction)
            where TCalculationInput : ICalculationInput, IHasForeshoreProfile
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationsItem(
                                                 calculations,
                                                 inquiryHelper,
                                                 updateAction));
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/> which is bound to the action
        /// of clearing illustration points from collections of calculations.
        /// </summary>
        /// <param name="isEnabledFunc">The function to determine whether this item should be enabled.</param>
        /// <param name="changeHandler">Object responsible for clearing the illustration point results.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddClearIllustrationPointsOfCalculationsItem(Func<bool> isEnabledFunc,
                                                                                      IClearIllustrationPointsOfCalculationCollectionChangeHandler changeHandler)
        {
            contextMenuBuilder.AddCustomItem(RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsItem(isEnabledFunc, changeHandler));
            return this;
        }

        #region Decorated members

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which starts edit mode for the name of <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddRenameItem()
        {
            contextMenuBuilder.AddRenameItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which deletes the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddDeleteItem()
        {
            contextMenuBuilder.AddDeleteItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which removes all children of the given 
        /// <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddRemoveAllChildrenItem()
        {
            contextMenuBuilder.AddDeleteChildrenItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which expands the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddExpandAllItem()
        {
            contextMenuBuilder.AddExpandAllItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which collapses the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddCollapseAllItem()
        {
            contextMenuBuilder.AddCollapseAllItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which opens a view for the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddOpenItem()
        {
            contextMenuBuilder.AddOpenItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which exports the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddExportItem()
        {
            contextMenuBuilder.AddExportItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which imports to the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddImportItem()
        {
            contextMenuBuilder.AddImportItem();
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which imports to the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="text">The text of the import item.</param>
        /// <param name="toolTip">The tooltip of the import item.</param>
        /// <param name="image">The image of the import item.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddCustomImportItem(string text, string toolTip, Image image)
        {
            contextMenuBuilder.AddCustomImportItem(text, toolTip, image);
            return this;
        }

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which shows properties of the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddPropertiesItem()
        {
            contextMenuBuilder.AddPropertiesItem();
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ToolStripSeparator"/> to the <see cref="ContextMenuStrip"/>. A <see cref="ToolStripSeparator"/>
        /// is only added if the last item that was added to the <see cref="ContextMenuStrip"/> exists and is not a 
        /// <see cref="ToolStripSeparator"/>.
        /// </summary>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddSeparator()
        {
            contextMenuBuilder.AddSeparator();
            return this;
        }

        /// <summary>
        /// Adds a custom item to the <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="item">The custom <see cref="StrictContextMenuItem"/> to add to the <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="RiskeerContextMenuBuilder"/> itself.</returns>
        public RiskeerContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            contextMenuBuilder.AddCustomItem(item);
            return this;
        }

        /// <summary>
        /// Obtain the <see cref="ContextMenuStrip"/>, which has been constructed by using the other methods of
        /// <see cref="RiskeerContextMenuBuilder"/>.
        /// </summary>
        /// <returns>The constructed <see cref="ContextMenuStrip"/>.</returns>
        public ContextMenuStrip Build()
        {
            return contextMenuBuilder.Build();
        }

        #endregion
    }
}