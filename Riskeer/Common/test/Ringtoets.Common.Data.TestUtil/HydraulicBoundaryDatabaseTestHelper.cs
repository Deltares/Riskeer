// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil
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
        public static void SetHydraulicBoundaryLocationConfigurationSettings(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.SetValues("some\\Path\\ToHlcd",
                                                                                       "ScenarioName",
                                                                                       1337,
                                                                                       "Scope",
                                                                                       "SeaLevel",
                                                                                       "RiverDischarge",
                                                                                       "LakeLevel",
                                                                                       "WindDirection",
                                                                                       "WindSpeed",
                                                                                       "Comment");
        }
    }
}