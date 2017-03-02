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
using Ringtoets.Piping.Plugin.Properties;

namespace Ringtoets.Piping.Plugin.ChangeHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateEntryAndExitPointsCalculationGroupChangeHandler : IConfirmDataChangeHandler
    {
        private readonly IEnumerable<PipingCalculation> calculations;
        private readonly IInquiryHelper inquiryHandler;
        
        /// <summary>
        /// Instantiates a <see cref="UpdateEntryAndExitPointsCalculationGroupChangeHandler"/>.
        /// </summary>
        /// <param name="calculations">The calculations for which to handle the changes in the entry and exit points.</param>
        /// <param name="inquiryHandler">Object responsible for inquiring required data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public UpdateEntryAndExitPointsCalculationGroupChangeHandler(IEnumerable<PipingCalculation> calculations, IInquiryHelper inquiryHandler)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }
            if (inquiryHandler == null)
            {
                throw new ArgumentNullException(nameof(inquiryHandler));
            }

            this.calculations = calculations;
            this.inquiryHandler = inquiryHandler;
        }

        public bool RequireConfirmation()
        {
            return calculations.Any(calc => calc.HasOutput);
        }

        public bool InquireConfirmation()
        {
            return inquiryHandler.InquireContinuation(
                Resources.UpdateEntryAndExitPointsCalculationGroupChangeHandler_InquireConfirmation_When_updating_entry_and_exit_points_definitions_assigned_to_calculations_output_will_be_cleared_confirm);
        }
    }
}
