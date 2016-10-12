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

using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresInputTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup
            var levelCrestStructure = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            // Call
            var input = new HeightStructuresInput();

            // Assert
            Assert.IsInstanceOf<StructuresInputBase<HeightStructure>>(input);

            DistributionAssert.AreEqual(levelCrestStructure, input.LevelCrestStructure);

            Assert.AreEqual(2, input.DeviationWaveDirection.NumberOfDecimalPlaces);
            Assert.IsNaN(input.DeviationWaveDirection);
        }

        [Test]
        public void Properties_DeviationWaveDirection_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var random = new Random(22);

            RoundedDouble deviationWaveDirection = new RoundedDouble(5, random.NextDouble());

            // Call
            input.DeviationWaveDirection = deviationWaveDirection;

            // Assert
            Assert.AreEqual(2, input.DeviationWaveDirection.NumberOfDecimalPlaces);
            AssertAreEqual(deviationWaveDirection, input.DeviationWaveDirection);
        }

        [Test]
        public void Properties_LevelCrestStructure_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.LevelCrestStructure = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.LevelCrestStructure, distributionToSet, expectedDistribution);
        }
       
        [Test]
        public void Properties_StructureNull_DoesNotChangeValues()
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            input.Structure = null;

            // Assert
            AssertHeightStructure(null, input);
        }

        [Test]
        public void Properties_Structure_UpdateValuesAccordingly()
        {
            // Setup
            var input = new HeightStructuresInput();
            TestHeightStructure structure = new TestHeightStructure();

            // Call
            input.Structure = structure;

            // Assert
            AssertHeightStructure(structure, input);
        }

        private static void AssertHeightStructure(HeightStructure expectedHeightStructure, HeightStructuresInput input)
        {
            if (expectedHeightStructure == null)
            {
                Assert.IsNull(input.Structure);
                var defaultInput = new HeightStructuresInput();
                AssertAreEqual(defaultInput.StructureNormalOrientation, input.StructureNormalOrientation);

                Assert.AreEqual(defaultInput.LevelCrestStructure.Mean, input.LevelCrestStructure.Mean);
                Assert.AreEqual(defaultInput.LevelCrestStructure.StandardDeviation,
                                input.LevelCrestStructure.StandardDeviation);

                Assert.AreEqual(defaultInput.CriticalOvertoppingDischarge.Mean,
                                input.CriticalOvertoppingDischarge.Mean);
                Assert.AreEqual(defaultInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                                input.CriticalOvertoppingDischarge.CoefficientOfVariation);

                Assert.AreEqual(defaultInput.WidthFlowApertures.Mean, input.WidthFlowApertures.Mean);
                Assert.AreEqual(defaultInput.WidthFlowApertures.CoefficientOfVariation,
                                input.WidthFlowApertures.CoefficientOfVariation);

                Assert.AreEqual(defaultInput.FailureProbabilityStructureWithErosion,
                                input.FailureProbabilityStructureWithErosion);

                Assert.AreEqual(defaultInput.StorageStructureArea.Mean, input.StorageStructureArea.Mean);
                Assert.AreEqual(defaultInput.StorageStructureArea.CoefficientOfVariation,
                                input.StorageStructureArea.CoefficientOfVariation);

                Assert.AreEqual(defaultInput.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.Mean);
                Assert.AreEqual(defaultInput.AllowedLevelIncreaseStorage.Shift, input.AllowedLevelIncreaseStorage.Shift);
                Assert.AreEqual(defaultInput.AllowedLevelIncreaseStorage.StandardDeviation,
                                input.AllowedLevelIncreaseStorage.StandardDeviation);
            }
            else
            {
                AssertAreEqual(expectedHeightStructure.StructureNormalOrientation, input.StructureNormalOrientation);

                Assert.AreEqual(expectedHeightStructure.LevelCrestStructure.Mean, input.LevelCrestStructure.Mean);
                Assert.AreEqual(expectedHeightStructure.LevelCrestStructure.StandardDeviation,
                                input.LevelCrestStructure.StandardDeviation);

                Assert.AreEqual(expectedHeightStructure.CriticalOvertoppingDischarge.Mean,
                                input.CriticalOvertoppingDischarge.Mean);
                Assert.AreEqual(expectedHeightStructure.CriticalOvertoppingDischarge.CoefficientOfVariation,
                                input.CriticalOvertoppingDischarge.CoefficientOfVariation);

                Assert.AreEqual(expectedHeightStructure.WidthFlowApertures.Mean, input.WidthFlowApertures.Mean);
                Assert.AreEqual(expectedHeightStructure.WidthFlowApertures.CoefficientOfVariation,
                                input.WidthFlowApertures.CoefficientOfVariation);

                Assert.AreEqual(expectedHeightStructure.FailureProbabilityStructureWithErosion,
                                input.FailureProbabilityStructureWithErosion);

                Assert.AreEqual(expectedHeightStructure.StorageStructureArea.Mean, input.StorageStructureArea.Mean);
                Assert.AreEqual(expectedHeightStructure.StorageStructureArea.CoefficientOfVariation,
                                input.StorageStructureArea.CoefficientOfVariation);

                Assert.AreEqual(expectedHeightStructure.AllowedLevelIncreaseStorage.Mean,
                                input.AllowedLevelIncreaseStorage.Mean);
                Assert.AreEqual(expectedHeightStructure.AllowedLevelIncreaseStorage.Shift,
                                input.AllowedLevelIncreaseStorage.Shift);
                Assert.AreEqual(expectedHeightStructure.AllowedLevelIncreaseStorage.StandardDeviation,
                                input.AllowedLevelIncreaseStorage.StandardDeviation);
            }
        }
        
        private static void AssertAreEqual(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        private static void AssertDistributionCorrectlySet(IDistribution distributionToAssert, IDistribution setDistribution, IDistribution expectedDistribution)
        {
            Assert.AreNotSame(setDistribution, distributionToAssert);
            DistributionAssert.AreEqual(expectedDistribution, distributionToAssert);
        }
    }
}