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
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Common.Service.MessageProviders
{
    /// <summary>
    /// This class provides messages used during a design water level calculation.
    /// </summary>
    public class DesignWaterLevelCalculationMessageProvider : ICalculationMessageProvider
    {
        private readonly string categoryBoundaryName;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationMessageProvider"/>.
        /// </summary>
        /// <param name="categoryBoundaryName">The category boundary name.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public DesignWaterLevelCalculationMessageProvider(string categoryBoundaryName)
        {
            if (string.IsNullOrEmpty(categoryBoundaryName))
            {
                throw new ArgumentException($"'{nameof(categoryBoundaryName)}' must have a value.");
            }

            this.categoryBoundaryName = categoryBoundaryName;
        }

        public string GetActivityDescription(string calculationSubject)
        {
            return string.Format(Resources.DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_HydraulicBoundaryLocation_0_Category_1_,
                                 calculationSubject,
                                 categoryBoundaryName);
        }

        public string GetCalculationFailedMessage(string calculationSubject)
        {
            return string.Format(Resources.DesignWaterLevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation_0_Category_1_no_error_report,
                                 calculationSubject,
                                 categoryBoundaryName);
        }

        public string GetCalculatedNotConvergedMessage(string calculationSubject)
        {
            return string.Format(Resources.DesignWaterLevelCalculationActivity_DesignWaterLevelCalculation_for_HydraulicBoundaryLocation_0_Category_1_not_converged,
                                 calculationSubject,
                                 categoryBoundaryName);
        }

        public string GetCalculationFailedWithErrorReportMessage(string calculationSubject, string errorReport)
        {
            return string.Format(Resources.DesignWaterLevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation_0_Category_1_click_details_for_last_error_report_2,
                                 calculationSubject,
                                 categoryBoundaryName,
                                 errorReport);
        }
    }
}