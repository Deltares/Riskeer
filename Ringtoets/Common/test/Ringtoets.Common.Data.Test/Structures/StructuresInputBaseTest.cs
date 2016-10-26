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
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresInputBaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
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
            var input = new SimpleStructuresInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsInstanceOf<IUseBreakWater>(input);
            Assert.IsInstanceOf<IUseForeshore>(input);

            Assert.IsNull(input.Structure);
            Assert.IsNull(input.HydraulicBoundaryLocation);

            AssertAreEqual(double.NaN, input.StructureNormalOrientation);
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);

            Assert.IsNull(input.ForeshoreProfile);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(0, input.BreakWater.Height.Value);
            Assert.AreEqual(2, input.BreakWater.Height.NumberOfDecimalPlaces);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);

            DistributionAssert.AreEqual(modelFactorSuperCriticalFlow, input.ModelFactorSuperCriticalFlow);
            DistributionAssert.AreEqual(allowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(storageStructureArea, input.StorageStructureArea);
            DistributionAssert.AreEqual(flowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(criticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(widthFlowApertures, input.WidthFlowApertures);
            DistributionAssert.AreEqual(stormDuration, input.StormDuration);

            Assert.AreEqual(0, input.FailureProbabilityStructureWithErosion);
        }

        [Test]
        public void Properties_Structure_UpdateValuesAccordingly()
        {
            // Setup
            var structure = new SimpleStructure(new StructureBase.ConstructionProperties
            {
                Name = "<awesome name>",
                Location = new Point2D(0, 0),
                Id = "id"
            });

            var input = new SimpleStructuresInput();

            // Precondition
            Assert.IsNull(input.Structure);
            Assert.IsFalse(input.Updated);

            // Call
            input.Structure = structure;

            // Assert
            Assert.AreSame(structure, input.Structure);
            Assert.IsTrue(input.Updated);
        }

        #region Model factors

        [Test]
        public void Properties_ModelFactorSuperCriticalFlow_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
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

        #endregion

        #region Hydraulic data

        [Test]
        public void Properties_HydraulicBoundaryLocation_ExpectedValues()
        {
            // Setup
            var input = new SimpleStructuresInput();
            var location = new HydraulicBoundaryLocation(0, "test", 0, 0);

            // Call
            input.HydraulicBoundaryLocation = location;

            // Assert
            Assert.AreSame(location, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Properties_StormDuration_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
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

        #endregion

        #region Schematization

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
            var input = new SimpleStructuresInput();

            // Call
            TestDelegate call = () => input.StructureNormalOrientation = (RoundedDouble) invalidValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik [0, 360] liggen.");
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
            var input = new SimpleStructuresInput();

            // Call
            input.StructureNormalOrientation = (RoundedDouble) orientation;

            // Assert
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);
            AssertAreEqual(orientation, input.StructureNormalOrientation);
        }

        [Test]
        public void Properties_AllowedLevelIncreaseStorage_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
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
            var input = new SimpleStructuresInput();
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
            var input = new SimpleStructuresInput();
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
            var input = new SimpleStructuresInput();
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
            var input = new SimpleStructuresInput();

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
            var input = new SimpleStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityStructureWithErosion = (RoundedDouble) failureProbabilityStructureWithErosion;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik [0, 1] liggen.");
        }

        [Test]
        public void Properties_WidthFlowApertures_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
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

        #endregion

        #region Foreshore profile

        [Test]
        [Combinatorial]
        public void ForeshoreProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new SimpleStructuresInput();
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
            Assert.AreSame(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void ForeshoreProfile_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new SimpleStructuresInput();
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
            Assert.AreSame(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);

            // Call
            input.ForeshoreProfile = null;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreSame(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        #endregion

        #region Helpers

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

        #endregion

        private class SimpleStructuresInput : StructuresInputBase<StructureBase>
        {
            public bool Updated { get; private set; }

            protected override void UpdateStructureParameters()
            {
                Updated = true;
            }
        }

        private class SimpleStructure : StructureBase
        {
            public SimpleStructure(ConstructionProperties constructionProperties) : base(constructionProperties) { }
        }
    }
}