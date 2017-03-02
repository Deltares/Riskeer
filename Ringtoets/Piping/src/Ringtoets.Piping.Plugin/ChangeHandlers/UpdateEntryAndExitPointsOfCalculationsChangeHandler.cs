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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui;
using Ringtoets.Common.IO;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Plugin.ChangeHandlers
{
    /// <summary>
    /// Class which can, if required, inquire the user for a confirmation when a change to the
    /// entry and exit points requires calculation results to be altered.
    /// </summary>
    public class UpdateEntryAndExitPointsOfCalculationsChangeHandler : IConfirmDataChangeHandler
    {
        private readonly IEnumerable<PipingCalculation> calculations;
        private readonly IInquiryHelper inquiryHandler;
        private readonly string query;

        /// <summary>
        /// Instantiates a <see cref="UpdateEntryAndExitPointsOfCalculationsChangeHandler"/>.
        /// </summary>
        /// <param name="calculations">The calculations for which to handle the changes in the entry and exit points.</param>
        /// <param name="query">The query which should be displayed when inquiring for a confirmation.</param>
        /// <param name="inquiryHandler">Object responsible for inquiring required data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public UpdateEntryAndExitPointsOfCalculationsChangeHandler(IEnumerable<PipingCalculation> calculations,
                                                                   string query,
                                                                   IInquiryHelper inquiryHandler)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (inquiryHandler == null)
            {
                throw new ArgumentNullException(nameof(inquiryHandler));
            }

            this.calculations = calculations;
            this.query = query;
            this.inquiryHandler = inquiryHandler;
        }

        public bool RequireConfirmation()
        {
            return calculations.Any(calc => calc.HasOutput);
        }

        public bool InquireConfirmation()
        {
            return inquiryHandler.InquireContinuation(query);
        }
    }
}