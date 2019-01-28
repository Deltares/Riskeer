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

using System;
using Core.Common.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

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
            input.DikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            input.UseBreakWater = random.NextBoolean();
            input.BreakWater.Type = random.NextEnumValue<BreakWaterType>();
            input.BreakWater.Height = random.NextRoundedDouble();
            input.UseForeshore = random.NextBoolean();
        }

        /// <summary>
        /// Creates a random instance of <see cref="GrassCoverErosionInwardsOutput"/>.
        /// </summary>
        /// <returns>A random instance of <see cref="GrassCoverErosionInwardsOutput"/>.</returns>
        public static GrassCoverErosionInwardsOutput GetRandomGrassCoverErosionInwardsOutput()
        {
            OvertoppingOutput overtoppingOutput = GetRandomOvertoppingOutput(new TestGeneralResultFaultTreeIllustrationPoint());
            DikeHeightOutput dikeHeightOutput = GetRandomDikeHeightOutput(new TestGeneralResultFaultTreeIllustrationPoint());
            OvertoppingRateOutput overtoppingRateOutput = GetRandomOvertoppingRateOutput(new TestGeneralResultFaultTreeIllustrationPoint());
            return new GrassCoverErosionInwardsOutput(overtoppingOutput, dikeHeightOutput, overtoppingRateOutput);
        }

        /// <summary>
        /// Creates a random instance of <see cref="OvertoppingRateOutput"/>.
        /// </summary>
        /// <param name="generalResult">The general result to set to the output.</param>
        /// <returns>A random instance of <see cref="OvertoppingRateOutput"/>.</returns>
        public static OvertoppingRateOutput GetRandomOvertoppingRateOutput(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            var random = new Random(21);
            return new OvertoppingRateOutput(random.NextDouble(),
                                             random.NextDouble(),
                                             random.NextDouble(),
                                             random.NextDouble(),
                                             random.NextDouble(),
                                             random.NextEnumValue<CalculationConvergence>(),
                                             generalResult);
        }

        /// <summary>
        /// Creates a random instance of <see cref="OvertoppingOutput"/>.
        /// </summary>
        /// <param name="generalResult">The general result to set to the output.</param>
        /// <returns>A random instance of <see cref="OvertoppingOutput"/>.</returns>
        public static OvertoppingOutput GetRandomOvertoppingOutput(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            var random = new Random(21);
            return new OvertoppingOutput(random.NextDouble(),
                                         random.NextBoolean(),
                                         random.NextDouble(),
                                         generalResult);
        }

        /// <summary>
        /// Creates a random instance of <see cref="DikeHeightOutput"/>.
        /// </summary>
        /// <param name="generalResult">The general result to set to the output.</param>
        /// <returns>A random instance of <see cref="DikeHeightOutput"/>.</returns>
        public static DikeHeightOutput GetRandomDikeHeightOutput(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            var random = new Random(21);
            return new DikeHeightOutput(random.NextDouble(),
                                        random.NextDouble(),
                                        random.NextDouble(),
                                        random.NextDouble(),
                                        random.NextDouble(),
                                        random.NextEnumValue<CalculationConvergence>(),
                                        generalResult);
        }

        /// <summary>
        /// Creates a random instance of <see cref="HydraulicLoadsOutput"/>.
        /// </summary>
        /// <param name="generalResult">The general result to set to the output.</param>
        /// <returns>A random instance of <see cref="HydraulicLoadsOutput"/>.</returns>
        public static HydraulicLoadsOutput GetRandomHydraulicLoadsOutput(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            var random = new Random(21);

            return new TestHydraulicLoadsOutput(random.NextDouble(),
                                                random.NextDouble(),
                                                random.NextDouble(),
                                                random.NextDouble(),
                                                random.NextEnumValue<CalculationConvergence>(),
                                                generalResult);
        }
    }
}