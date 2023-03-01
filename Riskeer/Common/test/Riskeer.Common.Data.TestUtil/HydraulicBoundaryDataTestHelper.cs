// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.IO;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Test helper for dealing with <see cref="HydraulicBoundaryData"/>.
    /// </summary>
    public static class HydraulicBoundaryDataTestHelper
    {
        /// <summary>
        /// Sets valid values on <see cref="HydraulicBoundaryData.HydraulicLocationConfigurationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The <see cref="HydraulicBoundaryData"/> to set the values to.</param>
        /// <param name="usePreprocessorClosure">Indicator whether to use the preprocessor closure.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="HydraulicBoundaryData.FilePath"/> is <c>null</c>.</exception>
        public static void SetHydraulicLocationConfigurationSettings(HydraulicBoundaryData hydraulicBoundaryData,
                                                                     bool usePreprocessorClosure = false)
        {
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(hydraulicBoundaryData.FilePath), "hlcd.sqlite");

            HydraulicLocationConfigurationSettings hydraulicLocationConfigurationSettings = hydraulicBoundaryData.HydraulicLocationConfigurationSettings;

            hydraulicLocationConfigurationSettings.FilePath = hlcdFilePath;
            hydraulicLocationConfigurationSettings.ScenarioName = "ScenarioName";
            hydraulicLocationConfigurationSettings.Year = 1337;
            hydraulicLocationConfigurationSettings.Scope = "Scope";
            hydraulicLocationConfigurationSettings.SeaLevel = "SeaLevel";
            hydraulicLocationConfigurationSettings.RiverDischarge = "RiverDischarge";
            hydraulicLocationConfigurationSettings.LakeLevel = "LakeLevel";
            hydraulicLocationConfigurationSettings.WindDirection = "WindDirection";
            hydraulicLocationConfigurationSettings.WindSpeed = "WindSpeed";
            hydraulicLocationConfigurationSettings.Comment = "Comment";
            hydraulicLocationConfigurationSettings.UsePreprocessorClosure = usePreprocessorClosure;
        }
    }
}