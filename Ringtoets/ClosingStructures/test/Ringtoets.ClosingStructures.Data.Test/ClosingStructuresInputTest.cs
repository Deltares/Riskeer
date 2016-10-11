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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new ClosingStructuresInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);

            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsNull(input.ClosingStructure);

            AssertEqualValue(double.NaN, input.StructureNormalOrientation);
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);

            Assert.IsNull(input.ForeshoreProfile);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(0, input.BreakWater.Height.Value);
            Assert.AreEqual(2, input.BreakWater.Height.NumberOfDecimalPlaces);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);

            Assert.IsNaN(input.FailureProbabilityOpenStructure);
            Assert.IsNaN(input.FailureProbabilityStructureWithErosion);
            Assert.IsNaN(input.FailureProbabilityReparation);

            Assert.IsNaN(input.FactorStormDurationOpenStructure);
            Assert.IsNaN(input.DeviationWaveDirection);

            AssertEqualValue(1.1, input.ModelFactorSuperCriticalFlow.Mean);
            AssertEqualValue(0.03, input.ModelFactorSuperCriticalFlow.StandardDeviation);
            Assert.IsNaN(input.ThresholdHeightOpenWeir.Mean);
            AssertEqualValue(0.1, input.ThresholdHeightOpenWeir.StandardDeviation);
            AssertEqualValue(1, input.DrainCoefficient.Mean);
            AssertEqualValue(0.2, input.DrainCoefficient.StandardDeviation);
            Assert.IsNaN(input.AreaFlowApertures.Mean);
            AssertEqualValue(0.01, input.AreaFlowApertures.StandardDeviation);
            Assert.IsNaN(input.LevelCrestStructureNotClosing.Mean);
            AssertEqualValue(0.05, input.LevelCrestStructureNotClosing.StandardDeviation);
            Assert.IsNaN(input.InsideWaterLevel.Mean);
            AssertEqualValue(0.1, input.InsideWaterLevel.StandardDeviation);
            Assert.IsNaN(input.AllowedLevelIncreaseStorage.Mean);
            AssertEqualValue(0.1, input.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(input.StorageStructureArea.Mean);
            AssertEqualValue(0.1, input.StorageStructureArea.CoefficientOfVariation);
            Assert.IsNaN(input.FlowWidthAtBottomProtection.Mean);
            AssertEqualValue(0.05, input.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(input.CriticalOvertoppingDischarge.Mean);
            AssertEqualValue(0.15, input.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.IsNaN(input.WidthFlowApertures.Mean);
            AssertEqualValue(0.05, input.WidthFlowApertures.CoefficientOfVariation);
            AssertEqualValue(6.0, input.StormDuration.Mean);
            AssertEqualValue(0.25, input.StormDuration.CoefficientOfVariation);
            Assert.AreEqual(1.0, input.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(0, input.IdenticalApertures);
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.VerticalWall)]
        [TestCase(ClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert)]
        public void InflowModelType_SetValue_ReturnSetValue(ClosingStructureInflowModelType inflowModelType)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            input.InflowModelType = inflowModelType;

            // Assert
            Assert.AreEqual(inflowModelType, input.InflowModelType);
        }

        [Test]
        public void Properties_HydraulicBoundaryLocation_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var location = new HydraulicBoundaryLocation(0, "test", 0, 0);

            // Call
            input.HydraulicBoundaryLocation = location;

            // Assert
            Assert.AreEqual(location, input.HydraulicBoundaryLocation);
        }

        [Test]
        [Combinatorial]
        public void ForeshoreProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new ClosingStructuresInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreGeometry = new List<Point2D>
            {
                new Point2D(2.2, 3.3)
            };

            if (withValidForeshore)
            {
                foreshoreGeometry.Add(new Point2D(4.4, 5.5));
            }

            BreakWater breakWater = null;
            if (withBreakWater)
            {
                var nonDefaultBreakWaterType = BreakWaterType.Wall;
                var nonDefaultBreakWaterHeight = 5.5;

                // Precondition
                Assert.AreNotEqual(nonDefaultBreakWaterType, input.BreakWater.Type);
                Assert.AreNotEqual(nonDefaultBreakWaterHeight, input.BreakWater.Height);

                breakWater = new BreakWater(nonDefaultBreakWaterType, nonDefaultBreakWaterHeight);
            }

            double orientation = 96;
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        foreshoreGeometry.ToArray(),
                                                        breakWater,
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = orientation
                                                        });

            // Call
            input.ForeshoreProfile = foreshoreProfile;

            // Assert
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withValidForeshore, input.UseForeshore);
            CollectionAssert.AreEqual(foreshoreProfile.Geometry, input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Foreshore_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new ClosingStructuresInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        new[]
                                                        {
                                                            new Point2D(3.3, 4.4),
                                                            new Point2D(5.5, 6.6)
                                                        },
                                                        new BreakWater(BreakWaterType.Caisson, 2.2),
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = 96
                                                        });

            input.ForeshoreProfile = foreshoreProfile;

            // Precondition
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.IsTrue(input.UseBreakWater);
            Assert.AreNotEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreNotEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsTrue(input.UseForeshore);
            CollectionAssert.IsNotEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);

            // Call
            input.ForeshoreProfile = null;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
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
            var input = new ClosingStructuresInput();

            // Call
            input.StructureNormalOrientation = (RoundedDouble) orientation;

            // Assert
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);
            AssertEqualValue(orientation, input.StructureNormalOrientation);
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
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.StructureNormalOrientation = (RoundedDouble) invalidValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik tussen [0, 360] graden liggen.");
        }

        [Test]
        public void Properties_ModelFactorSuperCriticalFlow_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution modelFactorSuperCriticalFlow = GenerateNormalDistribution();

            RoundedDouble initialStd = input.ModelFactorSuperCriticalFlow.StandardDeviation;

            // Call
            input.ModelFactorSuperCriticalFlow = modelFactorSuperCriticalFlow;

            // Assert
            Assert.AreEqual(modelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.Mean);
            AssertEqualValue(initialStd, input.ModelFactorSuperCriticalFlow.StandardDeviation);
        }

        [Test]
        public void Properties_FactorStormDurationOpenStructure_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var random = new Random(22);

            var factorStormDuration = new RoundedDouble(5, random.NextDouble());

            // Call
            input.FactorStormDurationOpenStructure = factorStormDuration;

            // Assert
            Assert.AreEqual(2, input.FactorStormDurationOpenStructure.NumberOfDecimalPlaces);
            AssertEqualValue(factorStormDuration, input.FactorStormDurationOpenStructure);
        }

        [Test]
        public void Properties_ThresholdHeightOpenWeir_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution thresholdHeightOpenWeir = GenerateNormalDistribution();

            // Call
            input.ThresholdHeightOpenWeir = thresholdHeightOpenWeir;

            // Assert
            Assert.AreNotSame(thresholdHeightOpenWeir, input.ThresholdHeightOpenWeir);
            Assert.AreEqual(thresholdHeightOpenWeir.Mean, input.ThresholdHeightOpenWeir.Mean);
            Assert.AreEqual(thresholdHeightOpenWeir.StandardDeviation, input.ThresholdHeightOpenWeir.StandardDeviation);
        }

        [Test]
        public void Properties_DrainCoefficient_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution drainCoefficient = GenerateNormalDistribution();

            RoundedDouble initialStd = input.DrainCoefficient.StandardDeviation;

            // Call
            input.DrainCoefficient = drainCoefficient;

            // Assert
            Assert.AreEqual(drainCoefficient.Mean, input.DrainCoefficient.Mean);
            AssertEqualValue(initialStd, input.DrainCoefficient.StandardDeviation);
        }

        [Test]
        public void Properties_AreaFlowApertures_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution areaFlowApertures = GenerateLogNormalDistribution();

            // Call
            input.AreaFlowApertures = areaFlowApertures;

            // Assert
            Assert.AreEqual(areaFlowApertures.Mean, input.AreaFlowApertures.Mean);
            AssertEqualValue(areaFlowApertures.StandardDeviation, input.AreaFlowApertures.StandardDeviation);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void Properties_FailureProbabilityOpenStructure_ThrowArgumentException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityOpenStructure = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityOpenStructure_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.FailureProbabilityOpenStructure = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityOpenStructure);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void Properties_FailureProbabilityReparation_ThrowArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityReparation = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityReparation_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.FailureProbabilityReparation = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityReparation);
        }

        [Test]
        public void Properties_IdenticalApertures_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var random = new Random(22);

            int identicalAperture = random.Next();

            // Call
            input.IdenticalApertures = identicalAperture;

            // Assert
            Assert.AreEqual(identicalAperture, input.IdenticalApertures);
        }

        [Test]
        public void Properties_LevelCrestStructureNotClosing_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution levelCrestStructureNotClosing = GenerateNormalDistribution();

            // Call
            input.LevelCrestStructureNotClosing = levelCrestStructureNotClosing;

            // Assert
            Assert.AreEqual(levelCrestStructureNotClosing.Mean, input.LevelCrestStructureNotClosing.Mean);
            Assert.AreEqual(levelCrestStructureNotClosing.StandardDeviation, input.LevelCrestStructureNotClosing.StandardDeviation);
        }

        [Test]
        public void Properties_InsideWaterLevel_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            NormalDistribution insideWaterLevel = GenerateNormalDistribution();

            // Call
            input.InsideWaterLevel = insideWaterLevel;

            // Assert
            Assert.AreEqual(insideWaterLevel.Mean, input.InsideWaterLevel.Mean);
            Assert.AreEqual(insideWaterLevel.StandardDeviation, input.InsideWaterLevel.StandardDeviation);
        }

        [Test]
        public void Properties_AllowedLevelIncreaseStorage_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution allowedIncrease = GenerateLogNormalDistribution();

            // Call
            input.AllowedLevelIncreaseStorage = allowedIncrease;

            // Assert
            Assert.AreEqual(allowedIncrease.Mean, input.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(allowedIncrease.StandardDeviation, input.AllowedLevelIncreaseStorage.StandardDeviation);
        }

        [Test]
        public void Properties_StorageStructureArea_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            VariationCoefficientLogNormalDistribution storageStructureArea = GenerateVariationCoefficientLogNormalDistribution();

            // Call
            input.StorageStructureArea = storageStructureArea;

            // Assert
            Assert.AreEqual(storageStructureArea.Mean, input.StorageStructureArea.Mean);
            Assert.AreEqual(storageStructureArea.CoefficientOfVariation, input.StorageStructureArea.CoefficientOfVariation);
        }

        [Test]
        public void Properties_FlowWidthAtBottomProtection_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            LogNormalDistribution flowWidthAtBottomProtection = GenerateLogNormalDistribution();

            // Call
            input.FlowWidthAtBottomProtection = flowWidthAtBottomProtection;

            // Assert
            Assert.AreEqual(flowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(flowWidthAtBottomProtection.StandardDeviation, input.FlowWidthAtBottomProtection.StandardDeviation);
        }

        [Test]
        public void Properties_CriticalOvertoppingDischarge_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = GenerateVariationCoefficientLogNormalDistribution();

            // Call
            input.CriticalOvertoppingDischarge = criticalOvertoppingDischarge;

            // Assert
            Assert.AreEqual(criticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.Mean);
            AssertEqualValue(criticalOvertoppingDischarge.CoefficientOfVariation, input.CriticalOvertoppingDischarge.CoefficientOfVariation);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void Properties_FailureProbabilityStructureWithErosion_ThrowArgumentOutOfRangeException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityStructureWithErosion = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_FailureProbabilityStructureWithErosion_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.FailureProbabilityStructureWithErosion = probability;

            // Assert
            Assert.AreEqual(probability, input.FailureProbabilityStructureWithErosion);
        }

        [Test]
        public void Properties_WidthFlowApertures_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            VariationCoefficientNormalDistribution widthApertures = GenerateVariationCoefficientNormalDistribution();

            // Call
            input.WidthFlowApertures = widthApertures;

            // Assert
            Assert.AreEqual(widthApertures.Mean, input.WidthFlowApertures.Mean);
            Assert.AreEqual(widthApertures.CoefficientOfVariation, input.WidthFlowApertures.CoefficientOfVariation);
        }

        [Test]
        public void Properties_DeviationWaveDirection_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            var random = new Random(22);

            var deviationWaveDirection = new RoundedDouble(5, random.NextDouble());

            // Call
            input.DeviationWaveDirection = deviationWaveDirection;

            // Assert
            Assert.AreEqual(2, input.DeviationWaveDirection.NumberOfDecimalPlaces);
            AssertEqualValue(deviationWaveDirection, input.DeviationWaveDirection);
        }

        [Test]
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var input = new ClosingStructuresInput();
            VariationCoefficientLogNormalDistribution stormDuration = GenerateVariationCoefficientLogNormalDistribution();

            RoundedDouble initialVariation = input.StormDuration.CoefficientOfVariation;

            // Call
            input.StormDuration = stormDuration;

            // Assert
            Assert.AreEqual(stormDuration.Mean, input.StormDuration.Mean);
            AssertEqualValue(initialVariation, input.StormDuration.CoefficientOfVariation);
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(2)]
        [TestCase(double.NaN)]
        public void Properties_ProbabilityOpenStructureBeforeFlooding_ThrowArgumentException(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call
            TestDelegate call = () => input.ProbabilityOpenStructureBeforeFlooding = probability;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik tussen [0, 1] liggen.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Properties_ProbabilityOpenStructureBeforeFlooding_ExpectedValues(double probability)
        {
            // Setup
            var input = new ClosingStructuresInput();

            // Call 
            input.ProbabilityOpenStructureBeforeFlooding = probability;

            // Assert
            Assert.AreEqual(probability, input.ProbabilityOpenStructureBeforeFlooding);
        }

        private static void AssertEqualValue(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        private static LogNormalDistribution GenerateLogNormalDistribution()
        {
            var random = new Random(22);
            return new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };
        }

        private static VariationCoefficientLogNormalDistribution GenerateVariationCoefficientLogNormalDistribution()
        {
            var random = new Random(22);
            return new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                CoefficientOfVariation = (RoundedDouble) random.NextDouble()
            };
        }

        private static NormalDistribution GenerateNormalDistribution()
        {
            var random = new Random(22);
            return new NormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                StandardDeviation = (RoundedDouble) random.NextDouble()
            };
        }

        private static VariationCoefficientNormalDistribution GenerateVariationCoefficientNormalDistribution()
        {
            var random = new Random(22);
            return new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) (0.01 + random.NextDouble()),
                CoefficientOfVariation = (RoundedDouble) random.NextDouble()
            };
        }
    }
}