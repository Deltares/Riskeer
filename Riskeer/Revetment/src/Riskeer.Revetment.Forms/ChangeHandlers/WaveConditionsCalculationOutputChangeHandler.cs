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
using Core.Common.Gui.Helpers;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing wave conditions calculation output.
    /// </summary>
    public class WaveConditionsCalculationOutputChangeHandler : IClearCalculationOutputChangeHandler
    {
        private readonly IEnumerable<ICalculation<WaveConditionsInput>> calculations;
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationOutputChangeHandler"/>.
        /// </summary>
        /// <param name="calculations">The calculations to clear the output for.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaveConditionsCalculationOutputChangeHandler(IEnumerable<ICalculation<WaveConditionsInput>> calculations,
                                                            IInquiryHelper inquiryHelper)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            this.calculations = calculations;
            this.inquiryHelper = inquiryHelper;
        }

        public bool InquireConfirmation(string confirmationMessage)
        {
            return inquiryHelper.InquireContinuation(confirmationMessage);
        }

        public IEnumerable<IObservable> ClearCalculations()
        {
            var affectedCalculations = new List<ICalculation>();

            foreach (ICalculation<WaveConditionsInput> calculation in calculations)
            {
                calculation.ClearOutput();
                affectedCalculations.Add(calculation);
            }

            return affectedCalculations;
        }
    }
}