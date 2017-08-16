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
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToGrassCoverErosionInwardsInput(GrassCoverErosionInwardsInput input)
        {
            var random = new Random(21);

            input.Orientation = random.NextRoundedDouble();
            input.DikeHeight = random.NextRoundedDouble();
            input.CriticalFlowRate = new LogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };
            input.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            input.DikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>();
            input.OvertoppingRateCalculationType = random.NextEnumValue<OvertoppingRateCalculationType>();
            input.ShouldDikeHeightIllustrationPointsBeCalculated = random.NextBoolean();
            input.ShouldOvertoppingRateIllustrationPointsBeCalculated = random.NextBoolean();
            input.ShouldOvertoppingOutputIllustrationPointsBeCalculated = random.NextBoolean();
            input.DikeProfile = new TestDikeProfile();
            input.UseBreakWater = random.NextBoolean();
            input.BreakWater.Type = random.NextEnumValue<BreakWaterType>();
            input.BreakWater.Height = random.NextRoundedDouble();
            input.UseForeshore = random.NextBoolean();
        }
    }
}