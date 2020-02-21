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

using Core.Common.Util.Attributes;
using Riskeer.GrassCoverErosionOutwards.Data.Properties;

namespace Riskeer.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Defines the various types of grass cover erosion outwards wave conditions calculations.
    /// </summary>
    public enum GrassCoverErosionOutwardsWaveConditionsCalculationType
    {
        /// <summary>
        /// Calculate the wave run up.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_DisplayName))]
        WaveRunUp = 1,

        /// <summary>
        /// Calculate the wave impact.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveImpact_DisplayName))]
        WaveImpact = 2,

        /// <summary>
        /// Calculate the tailor made wave impact.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationType_TailorMadeWaveImpact_DisplayName))]
        TailorMadeWaveImpact = 3,

        /// <summary>
        /// Calculate both the wave run up and wave impact.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationType_Both_DisplayName))]
        Both = 4,

        /// <summary>
        /// Calculate both the wave run up and tailor made wave impact.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_TailorMadeWaveImpact_DisplayName))]
        WaveRunUpAndTailorMadeWaveImpact = 5,

        /// <summary>
        /// Calculate the wave run up, wave impact and tailor made wave impact.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsCalculationType_All_DisplayName))]
        All = 6
    }
}