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
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Base class for handling clearing illustration points from a calculation.
    /// </summary>
    /// <typeparam name="TCalculation">The type of calculation.</typeparam>
    public abstract class ClearIllustrationPointsOfCalculationChangeHandlerBase<TCalculation> : IClearIllustrationPointsOfCalculationChangeHandler
        where TCalculation : ICalculation
    {
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfCalculationChangeHandlerBase{TCalculation}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to clear the illustration points for.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="viewCommands">The view commands used to close views for the illustration points.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        protected ClearIllustrationPointsOfCalculationChangeHandlerBase(
            TCalculation calculation, IInquiryHelper inquiryHelper, IViewCommands viewCommands)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            this.inquiryHelper = inquiryHelper;
            Calculation = calculation;
            ViewCommands = viewCommands;
        }

        public bool InquireConfirmation()
        {
            return inquiryHelper.InquireContinuation(Resources.ClearIllustrationPointsOfCalculation_InquireConfirmation);
        }

        public abstract bool ClearIllustrationPoints();

        public void DoPostUpdateActions()
        {
            Calculation.NotifyObservers();
        }

        /// <summary>
        /// Gets the <see cref="TCalculation"/>.
        /// </summary>
        protected TCalculation Calculation { get; }

        /// <summary>
        /// Gets the <see cref="IViewCommands"/>.
        /// </summary>
        protected IViewCommands ViewCommands { get; }
    }
}