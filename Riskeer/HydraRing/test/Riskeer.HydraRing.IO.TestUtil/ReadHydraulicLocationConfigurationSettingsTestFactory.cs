﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Riskeer.HydraRing.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ReadHydraulicLocationConfigurationSettings"/> instances
    /// that can be used for testing.
    /// </summary>
    public static class ReadHydraulicLocationConfigurationSettingsTestFactory
    {
        /// <summary>
        /// Creates a <see cref="ReadHydraulicLocationConfigurationSettings"/>.
        /// </summary>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationSettings"/>.</returns>
        public static ReadHydraulicLocationConfigurationSettings Create()
        {
            return new ReadHydraulicLocationConfigurationSettings("scenarioName", 1337, "scope", "seaLevel",
                                                                  "riverDischarge", "lakeLevel", "windDirection",
                                                                  "windSpeed", "comment");
        }
    }
}