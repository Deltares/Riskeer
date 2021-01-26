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
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Base class for handling clearing calculation output.
    /// </summary>
    /// <typeparam name="TCalculationScenario">The type of calculation scenarios.</typeparam>
    public abstract class ClearCalculationOutputChangeHandlerBase<TCalculationScenario> : IClearCalculationOutputChangeHandler
        where TCalculationScenario : ICalculationScenario
    {
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Creates a new instance of <see cref="ClearCalculationOutputChangeHandlerBase{TCalculation}"/>.
        /// </summary>
        /// <param name="calculations">The calculations to clear the output for.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="viewCommands">The view commands used to close views for the calculation output.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected ClearCalculationOutputChangeHandlerBase(IEnumerable<TCalculationScenario> calculations,
                                                          IInquiryHelper inquiryHelper,
                                                          IViewCommands viewCommands)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            Calculations = calculations;
            this.inquiryHelper = inquiryHelper;
            ViewCommands = viewCommands;
        }

        public bool InquireConfirmation(string confirmationMessage)
        {
            return inquiryHelper.InquireContinuation(confirmationMessage);
        }

        public IEnumerable<IObservable> ClearCalculations()
        {
            DoPreUpdateActions();

            var affectedCalculations = new List<ICalculation>();

            foreach (TCalculationScenario calculation in Calculations)
            {
                calculation.ClearOutput();
                affectedCalculations.Add(calculation);
            }

            return affectedCalculations;
        }

        /// <summary>
        /// Gets the calculations.
        /// </summary>
        protected IEnumerable<TCalculationScenario> Calculations { get; }

        /// <summary>
        /// Gets the <see cref="IViewCommands"/>.
        /// </summary>
        protected IViewCommands ViewCommands { get; }

        /// <summary>
        /// Performs pre-update actions
        /// </summary>
        protected abstract void DoPreUpdateActions();
    }
}