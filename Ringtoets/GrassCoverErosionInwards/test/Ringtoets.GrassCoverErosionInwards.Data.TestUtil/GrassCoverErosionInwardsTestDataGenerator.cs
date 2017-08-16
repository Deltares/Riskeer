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
using Core.Common.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class GrassCoverErosionInwardsTestDataGenerator
    {
        /// <summary>
        /// This method creates a <see cref="GrassCoverErosionInwardsInput"/> object with random
        /// data values set for all properties.
        /// </summary>
        /// <returns>The created input object.</returns>
        public static GrassCoverErosionInwardsInput CreateRandomGrassCoverErosionInwardsInput()
        {
            var random = new Random(21);

            return new GrassCoverErosionInwardsInput
            {
                Orientation = random.NextRoundedDouble(),
                DikeHeight = random.NextRoundedDouble(),
                CriticalFlowRate = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                DikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>(),
                OvertoppingRateCalculationType = random.NextEnumValue<OvertoppingRateCalculationType>(),
                ShouldDikeHeightIllustrationPointsBeCalculated = random.NextBoolean(),
                ShouldOvertoppingRateIllustrationPointsBeCalculated = random.NextBoolean(),
                ShouldOvertoppingOutputIllustrationPointsBeCalculated = random.NextBoolean(),
                DikeProfile = new TestDikeProfile(),
                UseBreakWater = random.NextBoolean(),
                BreakWater =
                {
                    Type = random.NextEnumValue<BreakWaterType>(),
                    Height = random.NextRoundedDouble()
                },
                UseForeshore = random.NextBoolean()
            };
        }
    }
}