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

using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service.Properties;

namespace Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders
{
    /// <summary>
    /// This class provides messages used during the wave height calculation for the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider : ICalculationMessageProvider
    {
        public string GetCalculationName(string locationName)
        {
            return string.Format(Resources.GrassCoverErosionOutwardsPlugin_Name_WaveHeight_for_location_0_, locationName);
        }

        public string GetActivityDescription(string locationName)
        {
            return string.Format(Resources.GrassCoverErosionOutwardsPlugin_Name_Calculate_WaveHeight_for_location_0_, locationName);
        }

        public string GetCalculationFailedMessage(string locationName, string failureMessage)
        {
            return string.Format(Resources.GrassCoverErosionOutwardsPlugin_Calculate_Error_in_WaveHeight_0_calculation_click_details_for_last_error_report_1, locationName, failureMessage);
        }

        public string GetCalculationFailedUnexplainedMessage(string locationName)
        {
            return string.Format(Resources.GrassCoverErosionOutwardsPlugin_Calculate_Error_in_WaveHeight_0_calculation_no_error_report, locationName);
        }

        public string GetCalculatedNotConvergedMessage(string locationName)
        {
            return string.Format(Resources.GrassCoverErosionOutwardsPlugin_WaveHeight_calculation_for_location_0_not_converged, locationName);
        }
    }
}