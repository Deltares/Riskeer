﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data.TestUtil;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new HeightStructuresInput();

            // Assert
            Assert.IsInstanceOf<StructuresInputBase<HeightStructure>>(input);

            var expectedLevelCrestStructure = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };
            DistributionAssert.AreEqual(expectedLevelCrestStructure, input.LevelCrestStructure);

            Assert.AreEqual(2, input.DeviationWaveDirection.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, input.DeviationWaveDirection, input.DeviationWaveDirection.GetAccuracy());
        }

        [Test]
        public void Structure_Null_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            input.Structure = null;

            // Assert
            AssertHeightStructure(null, input);
        }

        [Test]
        public void GivenInputWithStructure_WhenStructureNull_ThenSchematizationPropertiesSynedToDefaults()
        {
            // Given
            var structure = new TestHeightStructure();
            var input = new HeightStructuresInput()
            {
                Structure = structure
            };

            RoundedDouble expectedDeviationWaveDirection = input.DeviationWaveDirection;

            // Pre-condition
            AssertHeightStructure(structure, input);

            // When
            input.Structure = null;

            // Then
            AssertAreEqual(expectedDeviationWaveDirection, input.DeviationWaveDirection);
            Assert.AreEqual(1.0, input.FailureProbabilityStructureWithErosion);

            var expectedLevelCrestStructure = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            DistributionAssert.AreEqual(expectedLevelCrestStructure, input.LevelCrestStructure);
        }

        [Test]
        public void Structure_NotNull_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            TestHeightStructure structure = new TestHeightStructure();

            // Call
            input.Structure = structure;

            // Assert
            AssertHeightStructure(structure, input);
        }

        #region Schematization

        [Test]
        public void LevelCrestStructure_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.LevelCrestStructure = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            AssertDistributionCorrectlySet(input.LevelCrestStructure, distributionToSet, expectedDistribution);
        }

        #endregion

        #region Hydraulic data

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-360.005)]
        [TestCase(-400)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void DeviationWaveDirection_InvalidValues_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            TestDelegate call = () => input.DeviationWaveDirection = (RoundedDouble) invalidValue;

            // Assert
            const string expectedMessage = "De waarde voor de afwijking van de golfrichting moet in het bereik [-360,00, 360,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-360.004)]
        [TestCase(double.NaN)]
        public void DeviationWaveDirection_ValidValues_ExpectedValues(double validValue)
        {
            // Setup
            var input = new HeightStructuresInput();
            var deviationWaveDirection = new RoundedDouble(5, validValue);

            // Call
            input.DeviationWaveDirection = deviationWaveDirection;

            // Assert
            Assert.AreEqual(2, input.DeviationWaveDirection.NumberOfDecimalPlaces);
            AssertAreEqual(deviationWaveDirection, input.DeviationWaveDirection);
        }

        #endregion

        #region Helpers

        private static void AssertHeightStructure(HeightStructure expectedHeightStructure, HeightStructuresInput input)
        {
            if (expectedHeightStructure == null)
            {
                Assert.IsNull(input.Structure);
                var defaultInput = new HeightStructuresInput();
                AssertAreEqual(defaultInput.StructureNormalOrientation, input.StructureNormalOrientation);

                DistributionAssert.AreEqual(defaultInput.LevelCrestStructure, input.LevelCrestStructure);
                DistributionAssert.AreEqual(defaultInput.CriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
                DistributionAssert.AreEqual(defaultInput.WidthFlowApertures, input.WidthFlowApertures);

                Assert.AreEqual(defaultInput.FailureProbabilityStructureWithErosion, input.FailureProbabilityStructureWithErosion);

                DistributionAssert.AreEqual(defaultInput.StorageStructureArea, input.StorageStructureArea);
                DistributionAssert.AreEqual(defaultInput.AllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
            }
            else
            {
                AssertAreEqual(expectedHeightStructure.StructureNormalOrientation, input.StructureNormalOrientation);

                DistributionAssert.AreEqual(expectedHeightStructure.LevelCrestStructure, input.LevelCrestStructure);
                DistributionAssert.AreEqual(expectedHeightStructure.CriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
                DistributionAssert.AreEqual(expectedHeightStructure.WidthFlowApertures, input.WidthFlowApertures);

                Assert.AreEqual(expectedHeightStructure.FailureProbabilityStructureWithErosion, input.FailureProbabilityStructureWithErosion);

                DistributionAssert.AreEqual(expectedHeightStructure.StorageStructureArea, input.StorageStructureArea);
                DistributionAssert.AreEqual(expectedHeightStructure.AllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
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

        #endregion
    }
}