// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Integration.Service.MessageProviders
{
    /// <summary>
    /// This class provides messages used during the design water level calculation.
    /// </summary>
    public class DesignWaterLevelCalculationMessageProvider : ICalculationMessageProvider
    {
        public string GetActivityDescription(string calculationSubject)
        {
            return string.Format(Resources.DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_HydraulicBoundaryLocation_0_, calculationSubject);
        }

        public string GetCalculationFailedMessage(string calculationSubject)
        {
            return string.Format(Resources.DesignwaterlevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation_0_no_error_report, calculationSubject);
        }

        public string GetCalculatedNotConvergedMessage(string calculationSubject)
        {
            return string.Format(Resources.DesignWaterLevelCalculationActivity_DesignWaterLevelCalculation_for_HydraulicBoundaryLocation_0_not_converged, calculationSubject);
        }

        public string GetCalculationFailedWithErrorReportMessage(string calculationSubject, string errorReport)
        {
            return string.Format(Resources.DesignwaterlevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation_0_click_details_for_last_error_report_1, calculationSubject, errorReport);
        }
    }
}