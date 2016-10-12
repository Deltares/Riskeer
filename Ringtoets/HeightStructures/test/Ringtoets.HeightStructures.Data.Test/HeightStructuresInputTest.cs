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

            var modelFactorSuperCriticalFlow = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 0.03
            };

            var allowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.1
            };

            var storageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            var flowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                StandardDeviation = (RoundedDouble) 0.05
            };

            var criticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.15
            };

            var widthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) double.NaN,
                CoefficientOfVariation = (RoundedDouble) 0.05
            };

            var stormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            // Call
            var input = new HeightStructuresInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsInstanceOf<IUseBreakWater>(input);
            Assert.IsInstanceOf<IUseForeshore>(input);

            Assert.IsNull(input.HeightStructure);
            AssertForeshoreProfile(null, input);

            Assert.IsNull(input.HydraulicBoundaryLocation);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);

            AssertAreEqual(double.NaN, input.StructureNormalOrientation);
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);

            DistributionAssert.AreEqual(levelCrestStructure, input.LevelCrestStructure);
            DistributionAssert.AreEqual(modelFactorSuperCriticalFlow, input.ModelFactorSuperCriticalFlow);
            DistributionAssert.AreEqual(allowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(storageStructureArea, input.StorageStructureArea);
            DistributionAssert.AreEqual(flowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(criticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(widthFlowApertures, input.WidthFlowApertures);
            DistributionAssert.AreEqual(stormDuration, input.StormDuration);

            Assert.IsNaN(input.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(2, input.DeviationWaveDirection.NumberOfDecimalPlaces);
            Assert.IsNaN(input.DeviationWaveDirection);
        }

        [Test]
        public void Properties_ModelFactorSuperCriticalFlow_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = input.ModelFactorSuperCriticalFlow.StandardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.ModelFactorSuperCriticalFlow = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.ModelFactorSuperCriticalFlow, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_HydraulicBoundaryLocation_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput();
            var location = new HydraulicBoundaryLocation(0, "test", 0, 0);

            // Call
            input.HydraulicBoundaryLocation = location;

            // Assert
            Assert.AreSame(location, input.HydraulicBoundaryLocation);
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
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = input.StormDuration.CoefficientOfVariation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = (RoundedDouble) random.NextDouble()
            };

            // Call
            input.StormDuration = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.StormDuration, distributionToSet, expectedDistribution);
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
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        [TestCase(double.NaN)]
        public void Properties_StructureNormalOrientationValidValues_NewValueSet(double orientation)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            input.StructureNormalOrientation = (RoundedDouble) orientation;

            // Assert
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);
            AssertAreEqual(orientation, input.StructureNormalOrientation);
        }

        [Test]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-0.005)]
        [TestCase(-23)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Properties_StructureNormalOrientationInValidValues_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            TestDelegate call = () => input.StructureNormalOrientation = (RoundedDouble) invalidValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik tussen [0, 360] graden liggen.");
        }

        [Test]
        public void Properties_AllowedLevelIncreaseStorage_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new LogNormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.AllowedLevelIncreaseStorage = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.AllowedLevelIncreaseStorage, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_StorageStructureArea_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.StorageStructureArea = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.StorageStructureArea, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_FlowWidthAtBottomProtection_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new LogNormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.FlowWidthAtBottomProtection = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.FlowWidthAtBottomProtection, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_CriticalOvertoppingDischarge_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.CriticalOvertoppingDischarge = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.CriticalOvertoppingDischarge, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1)]
        public void Properties_ValidFailureProbabilityStructureWithErosion_ExpectedValues(double failureProbabilityStructureWithErosion)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            input.FailureProbabilityStructureWithErosion = failureProbabilityStructureWithErosion;

            // Assert
            Assert.AreEqual(failureProbabilityStructureWithErosion, input.FailureProbabilityStructureWithErosion);
        }

        [Test]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(double.NaN)]
        public void Properties_InvalidFailureProbabilityStructureWithErosion_ThrowArgumentOutOfRangeException(double failureProbabilityStructureWithErosion)
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityStructureWithErosion = (RoundedDouble) failureProbabilityStructureWithErosion;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        public void Properties_WidthFlowApertures_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new HeightStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new VariationCoefficientNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            var distributionToSet = new VariationCoefficientNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.WidthFlowApertures = distributionToSet;

            // Assert
            AssertDistributionCorrectlySet(input.WidthFlowApertures, distributionToSet, expectedDistribution);
        }

        [Test]
        public void Properties_ForeshoreProfileIsNull_ExpectedValues()
        {
            // Setup
            var input = new HeightStructuresInput
            {
                UseForeshore = true,
                UseBreakWater = true
            };

            // Call
            input.ForeshoreProfile = null;

            // Assert
            AssertForeshoreProfile(null, input);
        }

        [Test]
        public void Properties_ForeshoreProfileWithoutBreakwater_ExpectedValues()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new[]
            {
                new Point2D(1, 1)
            }, null, new ForeshoreProfile.ConstructionProperties());
            var input = new HeightStructuresInput();

            // Call
            input.ForeshoreProfile = foreshoreProfile;

            // Assert
            AssertForeshoreProfile(foreshoreProfile, input);
        }

        [Test]
        public void Properties_ForeshoreProfileWithBreakwater_ExpectedValues()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new[]
            {
                new Point2D(1, 1)
            }, new BreakWater(BreakWaterType.Wall, 2), new ForeshoreProfile.ConstructionProperties());
            var input = new HeightStructuresInput();

            // Call
            input.ForeshoreProfile = foreshoreProfile;

            // Assert
            AssertForeshoreProfile(foreshoreProfile, input);
        }

        [Test]
        public void Properties_HeightStructureNull_DoesNotChangeValues()
        {
            // Setup
            var input = new HeightStructuresInput();

            // Call
            input.HeightStructure = null;

            // Assert
            AssertHeightStructure(null, input);
        }

        [Test]
        public void Properties_HeightStructure_UpdateValuesAccordingly()
        {
            // Setup
            var input = new HeightStructuresInput();
            TestHeightStructure structure = new TestHeightStructure();

            // Call
            input.HeightStructure = structure;

            // Assert
            AssertHeightStructure(structure, input);
        }

        private static void AssertHeightStructure(HeightStructure expectedHeightStructure, HeightStructuresInput input)
        {
            if (expectedHeightStructure == null)
            {
                Assert.IsNull(input.HeightStructure);
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

        private static void AssertForeshoreProfile(ForeshoreProfile expectedForeshoreProfile, HeightStructuresInput input)
        {
            if (expectedForeshoreProfile == null)
            {
                Assert.IsNull(input.ForeshoreProfile);
                Assert.IsFalse(input.UseForeshore);

                Assert.IsFalse(input.UseBreakWater);
                BreakWater breakWater = GetDefaultBreakWater();
                Assert.AreEqual(breakWater.Type, input.BreakWater.Type);
                AssertAreEqual(breakWater.Height, input.BreakWater.Height);
            }
            else
            {
                Assert.AreEqual(expectedForeshoreProfile, input.ForeshoreProfile);
                Assert.AreEqual(expectedForeshoreProfile.Geometry.Count() > 1, input.UseForeshore);

                Assert.AreEqual(expectedForeshoreProfile.HasBreakWater, input.UseBreakWater);
                BreakWater breakWater = expectedForeshoreProfile.HasBreakWater ?
                                            new BreakWater(expectedForeshoreProfile.BreakWater.Type, expectedForeshoreProfile.BreakWater.Height) :
                                            GetDefaultBreakWater();
                Assert.AreEqual(breakWater.Type, input.BreakWater.Type);
                AssertAreEqual(breakWater.Height, input.BreakWater.Height);
            }
        }

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
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

        private static void AssertDistributionCorrectlySet(IVariationCoefficientDistribution distributionToAssert, IVariationCoefficientDistribution setDistribution, IVariationCoefficientDistribution expectedDistribution)
        {
            Assert.AreNotSame(setDistribution, distributionToAssert);
            DistributionAssert.AreEqual(expectedDistribution, distributionToAssert);
        }
    }
}