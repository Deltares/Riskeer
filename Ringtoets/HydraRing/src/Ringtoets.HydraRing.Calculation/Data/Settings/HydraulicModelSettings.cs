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

namespace Ringtoets.HydraRing.Calculation.Data.Settings
{
    /// <summary>
    /// Container for hydraulic model settings.
    /// </summary>
    internal class HydraulicModelSettings
    {
        private readonly HydraRingTimeIntegrationSchemeType hydraRingTimeIntegrationSchemeType;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicModelSettings"/>.
        /// </summary>
        /// <param name="hydraRingTimeIntegrationSchemeType">The time integration scheme type that should be used.</param>
        public HydraulicModelSettings(HydraRingTimeIntegrationSchemeType hydraRingTimeIntegrationSchemeType)
        {
            this.hydraRingTimeIntegrationSchemeType = hydraRingTimeIntegrationSchemeType;
        }

        /// <summary>
        /// Gets the time integration scheme type that should be used.
        /// </summary>
        public HydraRingTimeIntegrationSchemeType RingTimeIntegrationSchemeType
        {
            get
            {
                return hydraRingTimeIntegrationSchemeType;
            }
        }
    }
}