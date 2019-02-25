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
using System.IO;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Test helper for dealing with the <see cref="HydraulicBoundaryDatabase"/>
    /// </summary>
    public static class HydraulicBoundaryDatabaseTestHelper
    {
        /// <summary>
        /// Sets valid values on the <see cref="HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> to set the values to.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="HydraulicBoundaryDatabase.FilePath"/> is <c>null</c>.</exception>
        public static void SetHydraulicBoundaryLocationConfigurationSettings(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase.FilePath == null)
            {
                throw new ArgumentException("FilePath must be set.");
            }

            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(hydraulicBoundaryDatabase.FilePath), "hlcd.sqlite");

            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.SetValues(hlcdFilePath,
                                                                                       "ScenarioName",
                                                                                       1337,
                                                                                       "Scope",
                                                                                       false,
                                                                                       "SeaLevel",
                                                                                       "RiverDischarge",
                                                                                       "LakeLevel",
                                                                                       "WindDirection",
                                                                                       "WindSpeed",
                                                                                       "Comment");
        }
    }
}