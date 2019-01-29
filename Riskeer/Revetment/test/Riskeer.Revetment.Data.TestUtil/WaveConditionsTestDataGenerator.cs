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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.Revetment.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class WaveConditionsTestDataGenerator
    {
        /// <summary>
        /// Creates a random instance of <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <returns>A random instance of <see cref="WaveConditionsOutput"/>.</returns>
        public static WaveConditionsOutput GetRandomWaveConditionsOutput()
        {
            var random = new Random(21);
            return new WaveConditionsOutput(random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextDouble(),
                                            random.NextEnumValue<CalculationConvergence>());
        }

        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToWaveConditionsInput(AssessmentSectionCategoryWaveConditionsInput input)
        {
            var random = new Random(21);

            input.CategoryType = random.NextEnumValue<AssessmentSectionCategoryType>();

            SetRandomDataToWaveConditionsInput((WaveConditionsInput) input);
        }

        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToWaveConditionsInput(FailureMechanismCategoryWaveConditionsInput input)
        {
            var random = new Random(21);

            input.CategoryType = random.NextEnumValue<FailureMechanismCategoryType>();

            SetRandomDataToWaveConditionsInput((WaveConditionsInput) input);
        }

        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToWaveConditionsInput(WaveConditionsInput input)
        {
            var random = new Random(21);

            input.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            input.Orientation = random.NextRoundedDouble();
            input.LowerBoundaryRevetment = random.NextRoundedDouble(0, 2);
            input.UpperBoundaryRevetment = random.NextRoundedDouble(2, 4);
            input.StepSize = random.NextEnumValue<WaveConditionsInputStepSize>();
            input.LowerBoundaryWaterLevels = random.NextRoundedDouble(0, 2);
            input.UpperBoundaryWaterLevels = random.NextRoundedDouble(2, 4);
            input.UseBreakWater = random.NextBoolean();
            input.UseForeshore = random.NextBoolean();
            input.ForeshoreProfile = new TestForeshoreProfile();
        }
    }
}