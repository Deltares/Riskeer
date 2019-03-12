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
using Core.Common.Gui;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Base class for handling clearing illustration points from collections of calculations.
    /// </summary>
    public abstract class ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase
        : IClearIllustrationPointsOfCalculationCollectionChangeHandler
    {
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase(IInquiryHelper inquiryHelper)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            this.inquiryHelper = inquiryHelper;
        }

        public bool InquireConfirmation()
        {
            return inquiryHelper.InquireContinuation(GetConfirmationMessage());
        }

        public abstract IEnumerable<IObservable> ClearIllustrationPoints();

        /// <summary>
        /// Gets the message that should be displayed when inquiring the confirmation.
        /// </summary>
        /// <returns>The message that should be displayed when inquiring the confirmation.</returns>
        protected abstract string GetConfirmationMessage();
    }
}