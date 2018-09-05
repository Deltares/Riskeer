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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresInputBaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new SimpleStructuresInput();

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(input);
            Assert.IsInstanceOf<IStructuresCalculationInput<StructureBase>>(input);
            Assert.IsInstanceOf<IUseBreakWater>(input);
            Assert.IsInstanceOf<IUseForeshore>(input);
            Assert.IsInstanceOf<IHasForeshoreProfile>(input);

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

            var expectedAllowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            var expectedStorageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            var expectedFlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            var expectedCriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            var expectedWidthFlowApertures = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            var expectedStormDuration = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 6.0,
                CoefficientOfVariation = (RoundedDouble) 0.25
            };

            DistributionAssert.AreEqual(expectedAllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedStorageStructureArea, input.StorageStructureArea);
            DistributionAssert.AreEqual(expectedFlowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(expectedCriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(expectedWidthFlowApertures, input.WidthFlowApertures);
            DistributionAssert.AreEqual(expectedStormDuration, input.StormDuration);

            Assert.AreEqual(1.0, input.FailureProbabilityStructureWithErosion);

            Assert.IsFalse(input.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        public void Structure_Always_ExpectedValues()
        {
            // Setup
            var structure = new TestStructure();
            var input = new SimpleStructuresInput();

            // Precondition
            Assert.IsFalse(input.Synchronized);

            // Call
            input.Structure = structure;

            // Assert
            Assert.AreSame(structure, input.Structure);
            Assert.IsTrue(input.Synchronized);
        }

        [Test]
        public void ClearStructure_ClearsStructure()
        {
            // Setup
            var input = new SimpleStructuresInput
            {
                Structure = new TestStructure()
            };

            // Call
            input.ClearStructure();

            // Assert
            Assert.IsNull(input.Structure);
        }

        [Test]
        public void GivenInputWithStructure_WhenStructureNull_ThenSchematizationPropertiesSynedToDefaults()
        {
            // Given
            var structure = new TestStructure();
            var input = new SimpleStructuresInput
            {
                Structure = structure,
                FailureProbabilityStructureWithErosion = 0.99
            };

            VariationCoefficientLogNormalDistribution expectedStormDuration = input.StormDuration;
            NormalDistribution expectedModelFactorSuperCriticalFlow = input.ModelFactorSuperCriticalFlow;
            double expectedFailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;

            // Precondition
            Assert.AreSame(structure, input.Structure);

            // When
            input.Structure = null;

            // Then
            DistributionAssert.AreEqual(expectedStormDuration, input.StormDuration);
            DistributionAssert.AreEqual(expectedModelFactorSuperCriticalFlow, input.ModelFactorSuperCriticalFlow);

            AssertAreEqual(double.NaN, input.StructureNormalOrientation);
            Assert.AreEqual(2, input.StructureNormalOrientation.NumberOfDecimalPlaces);

            var expectedAllowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            var expectedStorageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            var expectedFlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            var expectedCriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            var expectedWidthFlowApertures = new NormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            DistributionAssert.AreEqual(expectedAllowedLevelIncreaseStorage, input.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedStorageStructureArea, input.StorageStructureArea);
            DistributionAssert.AreEqual(expectedFlowWidthAtBottomProtection, input.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(expectedCriticalOvertoppingDischarge, input.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(expectedWidthFlowApertures, input.WidthFlowApertures);

            Assert.AreEqual(expectedFailureProbabilityStructureWithErosion, input.FailureProbabilityStructureWithErosion);
        }

        #region Model factors

        [Test]
        public void ModelFactorSuperCriticalFlow_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
            RoundedDouble mean = random.NextRoundedDouble(0.01, 1.0);
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = input.ModelFactorSuperCriticalFlow.StandardDeviation
            };
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = random.NextRoundedDouble()
            };

            // Call
            input.ModelFactorSuperCriticalFlow = distributionToSet;

            // Assert
            DistributionTestHelper.AssertDistributionCorrectlySet(input.ModelFactorSuperCriticalFlow, distributionToSet, expectedDistribution);
        }

        #endregion

        #region Hydraulic data

        [Test]
        public void StormDuration_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
            RoundedDouble mean = random.NextRoundedDouble(0.01, 1.0);
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = input.StormDuration.CoefficientOfVariation
            };
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            // Call
            input.StormDuration = distributionToSet;

            // Assert
            DistributionTestHelper.AssertDistributionCorrectlySet(input.StormDuration, distributionToSet, expectedDistribution);
        }

        #endregion

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new SimpleStructuresInput
            {
                Structure = new TestStructure()
            };

            CommonTestDataGenerator.SetRandomDataToStructuresInput(original);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }

        private class SimpleStructuresInput : StructuresInputBase<StructureBase>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return Synchronized;
                }
            }

            public bool Synchronized { get; private set; }

            public override void SynchronizeStructureInput()
            {
                Synchronized = true;
            }
        }

        #region Schematization

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-0.005)]
        [TestCase(-23)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void StructureNormalOrientation_InvalidValues_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var input = new SimpleStructuresInput();

            // Call
            TestDelegate call = () => input.StructureNormalOrientation = (RoundedDouble) invalidValue;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen.");
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        [TestCase(double.NaN)]
        public void StructureNormalOrientation_ValidValues_ExpectedValues(double orientation)
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
        public void AllowedLevelIncreaseStorage_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.AllowedLevelIncreaseStorage = distributionToSet;

            // Assert
            var expectedDistribution = new LogNormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(input.AllowedLevelIncreaseStorage, distributionToSet, expectedDistribution);
        }

        [Test]
        public void StorageStructureArea_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.StorageStructureArea = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(input.StorageStructureArea, distributionToSet, expectedDistribution);
        }

        [Test]
        public void FlowWidthAtBottomProtection_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.FlowWidthAtBottomProtection = distributionToSet;

            // Assert
            var expectedDistribution = new LogNormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(input.FlowWidthAtBottomProtection, distributionToSet, expectedDistribution);
        }

        [Test]
        public void CriticalOvertoppingDischarge_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var variation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };

            // Call
            input.CriticalOvertoppingDischarge = distributionToSet;

            // Assert
            var expectedDistribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = mean,
                CoefficientOfVariation = variation
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(input.CriticalOvertoppingDischarge, distributionToSet, expectedDistribution);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1)]
        public void FailureProbabilityStructureWithErosion_ValidValues_ExpectedValues(double failureProbabilityStructureWithErosion)
        {
            // Setup
            var input = new SimpleStructuresInput();

            // Call
            input.FailureProbabilityStructureWithErosion = failureProbabilityStructureWithErosion;

            // Assert
            Assert.AreEqual(failureProbabilityStructureWithErosion, input.FailureProbabilityStructureWithErosion);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(double.NaN)]
        public void FailureProbabilityStructureWithErosion_InvalidValues_ThrowArgumentOutOfRangeException(double failureProbabilityStructureWithErosion)
        {
            // Setup
            var input = new SimpleStructuresInput();

            // Call
            TestDelegate call = () => input.FailureProbabilityStructureWithErosion = (RoundedDouble) failureProbabilityStructureWithErosion;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.");
        }

        [Test]
        public void WidthFlowApertures_Always_ExpectedValues()
        {
            // Setup
            var random = new Random(22);
            var input = new SimpleStructuresInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var distributionToSet = new NormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.WidthFlowApertures = distributionToSet;

            // Assert
            var expectedDistribution = new NormalDistribution(2)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            DistributionTestHelper.AssertDistributionCorrectlySet(input.WidthFlowApertures, distributionToSet, expectedDistribution);
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
                const BreakWaterType nonDefaultBreakWaterType = BreakWaterType.Wall;
                const double nonDefaultBreakWaterHeight = 5.5;

                // Precondition
                Assert.AreNotEqual(nonDefaultBreakWaterType, input.BreakWater.Type);
                Assert.AreNotEqual(nonDefaultBreakWaterHeight, input.BreakWater.Height);

                breakWater = new BreakWater(nonDefaultBreakWaterType, nonDefaultBreakWaterHeight);
            }

            const double orientation = 96;
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        foreshoreGeometry.ToArray(),
                                                        breakWater,
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id",
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
                                                            Id = "id",
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

        [Test]
        public void IsForeshoreProfileInputSynchronized_ForeshoreProfileNotSet_ReturnFalse()
        {
            // Setup
            var input = new SimpleStructuresInput();

            // Call
            bool isSynchronized = input.IsForeshoreProfileInputSynchronized;

            // Assert
            Assert.IsFalse(isSynchronized);
        }

        [Test]
        public void IsForeshoreProfileInputSynchronized_ForeshoreProfileAndInputInSync_ReturnTrue()
        {
            // Setup
            var input = new SimpleStructuresInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            // Call
            bool isSynchronized = input.IsForeshoreProfileInputSynchronized;

            // Assert
            Assert.IsTrue(isSynchronized);
        }

        [Test]
        [TestCaseSource(typeof(ForeshoreProfilePermutationHelper),
            nameof(ForeshoreProfilePermutationHelper.DifferentForeshoreProfilesWithSameIdNameOrientationAndX0),
            new object[]
            {
                "IsForeshoreProfileInputSynchronized",
                "ReturnFalse"
            })]
        public void IsForeshoreProfileInputSynchronized_ForeshoreProfilesOutOfSync_ReturnFalse(ForeshoreProfile modifiedProfile)
        {
            // Setup
            var input = new SimpleStructuresInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            input.ForeshoreProfile.CopyProperties(modifiedProfile);

            // Call
            bool isSynchronized = input.IsForeshoreProfileInputSynchronized;

            // Assert
            Assert.IsFalse(isSynchronized);
        }

        [Test]
        public void SynchronizeForeshoreProfileInput_ForeshoreProfileNotSet_ExpectedValues()
        {
            // Setup
            var input = new SimpleStructuresInput
            {
                UseBreakWater = true,
                UseForeshore = true,
                BreakWater =
                {
                    Height = (RoundedDouble) 1.0,
                    Type = BreakWaterType.Caisson
                }
            };

            // Call
            input.SynchronizeForeshoreProfileInput();

            // Assert
            AssertForeshoreProfilePropertiesOfInput(null, input);
        }

        [Test]
        public void SynchronizeForeshoreProfileInput_ChangedForeshoreProfile_ExpectedValues()
        {
            // Setup
            var differentProfile = new TestForeshoreProfile(true);
            var input = new SimpleStructuresInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            input.ForeshoreProfile.CopyProperties(differentProfile);

            // Precondition
            AssertForeshoreProfilePropertiesOfInput(new TestForeshoreProfile(), input);

            // Call
            input.SynchronizeForeshoreProfileInput();

            // Assert
            AssertForeshoreProfilePropertiesOfInput(differentProfile, input);
        }

        #endregion

        #region Helpers

        private static void AssertForeshoreProfilePropertiesOfInput(ForeshoreProfile expectedForeshoreProfile, SimpleStructuresInput input)
        {
            var defaultInput = new SimpleStructuresInput();
            if (expectedForeshoreProfile == null)
            {
                Assert.AreEqual(defaultInput.UseBreakWater, input.UseBreakWater);
                Assert.AreEqual(defaultInput.UseForeshore, input.UseForeshore);
            }
            else
            {
                Assert.AreEqual(expectedForeshoreProfile.Geometry.Count() > 1, input.UseForeshore);
                Assert.AreEqual(expectedForeshoreProfile.HasBreakWater, input.UseBreakWater);
            }

            if (expectedForeshoreProfile?.BreakWater == null)
            {
                Assert.AreEqual(defaultInput.BreakWater.Type, input.BreakWater.Type);
                Assert.AreEqual(defaultInput.BreakWater.Height, input.BreakWater.Height);
            }
            else
            {
                Assert.AreEqual(expectedForeshoreProfile.BreakWater.Type, input.BreakWater.Type);
                Assert.AreEqual(expectedForeshoreProfile.BreakWater.Height, input.BreakWater.Height);
            }
        }

        private static void AssertAreEqual(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        #endregion
    }
}