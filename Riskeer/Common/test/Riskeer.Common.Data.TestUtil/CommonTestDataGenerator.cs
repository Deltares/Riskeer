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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.Structures;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class CommonTestDataGenerator
    {
        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>, except
        /// for the property <see cref="StructuresInputBase{T}.Structure"/>.
        /// </summary>
        /// <typeparam name="T">The type of structure contained by the input.</typeparam>
        /// <param name="input">The input object to set the random data values to.</param>
        public static void SetRandomDataToStructuresInput<T>(StructuresInputBase<T> input) where T : StructureBase
        {
            var random = new Random(21);

            input.AllowedLevelIncreaseStorage = new LogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.StorageStructureArea = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.FlowWidthAtBottomProtection = new LogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.WidthFlowApertures = new NormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.StormDuration = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble()
            };

            input.StructureNormalOrientation = random.NextRoundedDouble();
            input.FailureProbabilityStructureWithErosion = random.NextDouble();
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.ShouldIllustrationPointsBeCalculated = random.NextBoolean();
            input.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            input.UseBreakWater = random.NextBoolean();
            input.BreakWater.Type = random.NextEnumValue<BreakWaterType>();
            input.BreakWater.Height = random.NextRoundedDouble();
            input.UseForeshore = random.NextBoolean();
        }

        /// <summary>
        /// Creates a random instance of <see cref="StructuresOutput"/>.
        /// </summary>
        /// <param name="generalResult">The general result to set to the output.</param>
        /// <returns>A random instance of <see cref="StructuresOutput"/>.</returns>
        public static StructuresOutput GetRandomStructuresOutput(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            var random = new Random(21);

            return new StructuresOutput(random.NextDouble(),
                                        generalResult);
        }
    }
}