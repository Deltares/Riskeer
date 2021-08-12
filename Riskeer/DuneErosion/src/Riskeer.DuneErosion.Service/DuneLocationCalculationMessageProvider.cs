// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Service.MessageProviders;
using Riskeer.DuneErosion.Service.Properties;

namespace Riskeer.DuneErosion.Service
{
    /// <summary>
    /// This class provides messages used during a dune location calculation.
    /// </summary>
    public class DuneLocationCalculationMessageProvider : ICalculationMessageProvider
    {
        private readonly string calculationIdentifier;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationMessageProvider"/>.
        /// </summary>
        /// <param name="calculationIdentifier">The calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="calculationIdentifier"/> is <c>null</c> or empty.</exception>
        public DuneLocationCalculationMessageProvider(string calculationIdentifier)
        {
            if (string.IsNullOrEmpty(calculationIdentifier))
            {
                throw new ArgumentException($"'{nameof(calculationIdentifier)}' must have a value.");
            }

            this.calculationIdentifier = calculationIdentifier;
        }

        public string GetActivityDescription(string calculationSubject)
        {
            return string.Format(Resources.DuneLocationCalculationActivity_Calculate_hydraulic_boundary_conditions_for_DuneLocation_with_name_0_CalculationIdentifier_1,
                                 calculationSubject,
                                 calculationIdentifier);
        }

        public string GetCalculationFailedMessage(string calculationSubject)
        {
            return string.Format(Resources.DuneLocationCalculationService_Calculate_Error_in_DuneLocationCalculation_0_CalculationIdentifier_1_no_error_report,
                                 calculationSubject,
                                 calculationIdentifier);
        }

        public string GetCalculatedNotConvergedMessage(string calculationSubject)
        {
            return string.Format(Resources.DuneLocationCalculationService_CreateDuneLocationCalculationOutput_Calculation_for_DuneLocation_0_CalculationIdentifier_1_not_converged,
                                 calculationSubject,
                                 calculationIdentifier);
        }

        public string GetCalculationFailedWithErrorReportMessage(string calculationSubject, string errorReport)
        {
            return string.Format(Resources.DuneLocationCalculationService_Calculate_Error_in_DuneLocationCalculation_0_CalculationIdentifier_1_click_details_for_last_error_report_1,
                                 calculationSubject,
                                 calculationIdentifier,
                                 errorReport);
        }
    }
}