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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputTest
    {
        private static IEnumerable<TestCaseData> DifferentDikeProfileProperties
        {
            get
            {
                DikeProfile defaultDikeProfile = CreateTestDikeProfile();
                Point2D defaultLocation = defaultDikeProfile.WorldReferencePoint;
                IEnumerable<RoughnessPoint> defaultRoughnessPoints = defaultDikeProfile.DikeGeometry;
                RoundedDouble defaultDikeHeight = defaultDikeProfile.DikeHeight;
                RoundedPoint2DCollection defaultForeshoreGeometry = defaultDikeProfile.ForeshoreGeometry;
                RoundedDouble defaultBreakWaterHeight = defaultDikeProfile.BreakWater.Height;
                BreakWaterType defaultBreakWaterType = defaultDikeProfile.BreakWater.Type;
                RoundedDouble defaultOrientation = defaultDikeProfile.Orientation;
                string defaultId = defaultDikeProfile.Id;

                yield return new TestCaseData(
                        new DikeProfile(defaultLocation,
                                        defaultRoughnessPoints,
                                        defaultForeshoreGeometry,
                                        new BreakWater(BreakWaterType.Caisson, defaultBreakWaterHeight),
                                        new DikeProfile.ConstructionProperties
                                        {
                                            Id = defaultId,
                                            Orientation = defaultOrientation,
                                            DikeHeight = defaultDikeHeight
                                        }))
                    .SetName("DifferentBreakWaterType");
                yield return new TestCaseData(
                        new DikeProfile(defaultLocation,
                                        defaultRoughnessPoints,
                                        defaultForeshoreGeometry,
                                        new BreakWater(defaultBreakWaterType, 4),
                                        new DikeProfile.ConstructionProperties
                                        {
                                            Id = defaultId,
                                            Orientation = defaultOrientation,
                                            DikeHeight = defaultDikeHeight
                                        }))
                    .SetName("DifferentBreakWaterHeight");
                yield return new TestCaseData(
                        new DikeProfile(defaultLocation,
                                        defaultRoughnessPoints,
                                        defaultForeshoreGeometry,
                                        new BreakWater(defaultBreakWaterType, defaultBreakWaterHeight),
                                        new DikeProfile.ConstructionProperties
                                        {
                                            Id = defaultId,
                                            Orientation = defaultOrientation,
                                            DikeHeight = 2
                                        }))
                    .SetName("DifferentDikeHeight");
                yield return new TestCaseData(
                        new DikeProfile(defaultLocation,
                                        defaultRoughnessPoints,
                                        new Point2D[0],
                                        new BreakWater(defaultBreakWaterType, defaultBreakWaterHeight),
                                        new DikeProfile.ConstructionProperties
                                        {
                                            Id = defaultId,
                                            Orientation = defaultOrientation,
                                            DikeHeight = defaultDikeHeight
                                        }))
                    .SetName("DifferentUseForeshore");
                yield return new TestCaseData(
                        new DikeProfile(defaultLocation,
                                        defaultRoughnessPoints,
                                        defaultForeshoreGeometry,
                                        null,
                                        new DikeProfile.ConstructionProperties
                                        {
                                            Id = defaultId,
                                            Orientation = defaultOrientation,
                                            DikeHeight = defaultDikeHeight
                                        }))
                    .SetName("DifferentUseBreakWater");
                yield return new TestCaseData(
                        new DikeProfile(defaultLocation,
                                        defaultRoughnessPoints,
                                        defaultForeshoreGeometry,
                                        null,
                                        new DikeProfile.ConstructionProperties
                                        {
                                            Id = defaultId,
                                            Orientation = 12,
                                            DikeHeight = defaultDikeHeight
                                        }))
                    .SetName("DifferentOrientation");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup 
            var criticalFlowRate = new LogNormalDistribution(4)
            {
                Mean = (RoundedDouble) 0.004,
                StandardDeviation = (RoundedDouble) 0.0006
            };

            // Call
            var input = new GrassCoverErosionInwardsInput();

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(input);
            Assert.IsInstanceOf<ICalculationInputWithLocation>(input);
            Assert.IsInstanceOf<IUseBreakWater>(input);
            Assert.IsInstanceOf<IUseForeshore>(input);

            Assert.AreEqual(2, input.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(2, input.DikeHeight.NumberOfDecimalPlaces);

            AssertDikeProfileInput(null, input);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.AreEqual(DikeHeightCalculationType.NoCalculation, input.DikeHeightCalculationType);
            Assert.AreEqual(OvertoppingRateCalculationType.NoCalculation, input.OvertoppingRateCalculationType);

            DistributionAssert.AreEqual(criticalFlowRate, input.CriticalFlowRate);
        }

        [Test]
        public void DikeProfile_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            DikeProfile dikeProfile = CreateTestDikeProfile();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = dikeProfile
            };

            // Call
            input.DikeProfile = null;

            // Assert
            AssertDikeProfileInput(null, input);
        }

        [Test]
        [TestCaseSource(nameof(DifferentDikeProfileProperties))]
        public void DikeProfile_SetNewValue_InputSyncedAccordingly(DikeProfile newDikeProfile)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            // Call
            input.DikeProfile = newDikeProfile;

            // Assert
            AssertDikeProfileInput(newDikeProfile, input);
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        [TestCase(double.NaN)]
        public void Orientation_SetNewValue_ValueIsRounded(double validOrientation)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            int originalNumberOfDecimalPlaces = input.Orientation.NumberOfDecimalPlaces;

            // Call
            input.Orientation = new RoundedDouble(5, validOrientation);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(validOrientation, input.Orientation.Value, input.Orientation.GetAccuracy());
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-0.005)]
        [TestCase(-23)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Orientation_SetInvalidValue_ThrowsArgumentOutOfRangeException(double invalidOrientation)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            // Call
            TestDelegate call = () => input.Orientation = (RoundedDouble) invalidOrientation;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen.");
        }

        [Test]
        public void DikeHeight_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            int originalNumberOfDecimalPlaces = input.DikeHeight.NumberOfDecimalPlaces;

            // Call
            input.DikeHeight = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.DikeHeight.Value);
        }

        [Test]
        public void CriticalFlowRate_SetNewValue_GetNewValues()
        {
            // Setup
            var random = new Random(22);
            var input = new GrassCoverErosionInwardsInput();
            RoundedDouble mean = random.NextRoundedDouble(0.01, double.MaxValue);
            RoundedDouble standardDeviation = random.NextRoundedDouble(0.01, double.MaxValue);
            var expectedDistribution = new LogNormalDistribution(4)
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
            input.CriticalFlowRate = distributionToSet;

            // Assert
            DistributionTestHelper.AssertDistributionCorrectlySet(input.CriticalFlowRate,
                                                                  distributionToSet,
                                                                  expectedDistribution);
        }

        [Test]
        public void IsDikeProfileInputSynchronized_DikeProfileNotSet_ReturnFalse()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            // Call
            bool synchronized = input.IsDikeProfileInputSynchronized;

            // Assert
            Assert.IsFalse(synchronized);
        }

        [Test]
        public void IsDikeProfileInputSynchronized_InputParametersAndDikeProfileInSync_ReturnTrue()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new[]
                {
                    new Point2D(3.3, 4.4),
                    new Point2D(5.5, 6.6)
                })
            };

            // Call
            bool synchronized = input.IsDikeProfileInputSynchronized;

            // Assert
            Assert.IsTrue(synchronized);
        }

        [Test]
        [TestCaseSource(nameof(DifferentDikeProfileProperties))]
        public void IsDikeProfileInputSynchronized_InputParametersAndDikeProfileNotInSync_ReturnFalse(DikeProfile newDikeProfile)
        {
            // Setup
            DikeProfile dikeProfile = CreateTestDikeProfile();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = dikeProfile
            };

            dikeProfile.CopyProperties(newDikeProfile);

            // Call
            bool synchronized = input.IsDikeProfileInputSynchronized;

            // Assert
            Assert.IsFalse(synchronized);
        }

        [Test]
        public void SynchronizeDikeProfileInput_DikeProfileNotSet_ExpectedValues()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            // Call
            input.SynchronizeDikeProfileInput();

            // Assert
            AssertDikeProfileInput(null, input);
        }

        [Test]
        [TestCaseSource(nameof(DifferentDikeProfileProperties))]
        public void SynchronizeDikeProfileInput_ChangedDikeProfile_ExpectedValues(DikeProfile newDikeProfile)
        {
            // Setup
            DikeProfile dikeProfile = CreateTestDikeProfile();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = dikeProfile
            };

            dikeProfile.CopyProperties(newDikeProfile);

            // Call
            input.SynchronizeDikeProfileInput();

            // Assert
            AssertDikeProfileInput(newDikeProfile, input);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new GrassCoverErosionInwardsInput();

            GrassCoverErosionInwardsTestDataGenerator.SetRandomDataToGrassCoverErosionInwardsInput(original);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new GrassCoverErosionInwardsInput();

            GrassCoverErosionInwardsTestDataGenerator.SetRandomDataToGrassCoverErosionInwardsInput(original);

            original.DikeProfile = null;
            original.HydraulicBoundaryLocation = null;

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        private static void AssertDikeProfileInput(DikeProfile expectedDikeProfile, GrassCoverErosionInwardsInput input)
        {
            var defaultInput = new GrassCoverErosionInwardsInput();

            if (expectedDikeProfile == null)
            {
                Assert.IsNull(input.DikeProfile);
                AssertAreEqual(defaultInput.Orientation, input.Orientation);

                Assert.AreEqual(defaultInput.UseBreakWater, input.UseBreakWater);
                Assert.AreEqual(defaultInput.BreakWater.Type, input.BreakWater.Type);
                Assert.AreEqual(defaultInput.BreakWater.Height, input.BreakWater.Height);
                Assert.AreEqual(defaultInput.UseForeshore, input.UseForeshore);
                CollectionAssert.AreEqual(defaultInput.ForeshoreGeometry, input.ForeshoreGeometry);
                CollectionAssert.AreEqual(defaultInput.DikeGeometry, input.DikeGeometry);
                AssertAreEqual(defaultInput.DikeHeight, input.DikeHeight);
            }
            else
            {
                AssertAreEqual(expectedDikeProfile.Orientation, input.Orientation);

                Assert.AreEqual(expectedDikeProfile.HasBreakWater, input.UseBreakWater);
                Assert.AreEqual(expectedDikeProfile.HasBreakWater ? expectedDikeProfile.BreakWater.Type : defaultInput.BreakWater.Type, input.BreakWater.Type);
                Assert.AreEqual(expectedDikeProfile.HasBreakWater ? expectedDikeProfile.BreakWater.Height : defaultInput.BreakWater.Height, input.BreakWater.Height);
                Assert.AreEqual(expectedDikeProfile.ForeshoreGeometry.Any(), input.UseForeshore);
                CollectionAssert.AreEqual(expectedDikeProfile.ForeshoreGeometry, input.ForeshoreGeometry);
                CollectionAssert.AreEqual(expectedDikeProfile.DikeGeometry, input.DikeGeometry);
                AssertAreEqual(expectedDikeProfile.DikeHeight, input.DikeHeight);
            }
        }

        private static void AssertAreEqual(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        private static DikeProfile CreateTestDikeProfile()
        {
            return new DikeProfile(new Point2D(0, 0),
                                   new[]
                                   {
                                       new RoughnessPoint(new Point2D(6.6, 7.7), 0.8)
                                   },
                                   new[]
                                   {
                                       new Point2D(2.2, 3.3),
                                       new Point2D(4.4, 5.5)
                                   },
                                   new BreakWater(BreakWaterType.Wall, 5.5),
                                   new DikeProfile.ConstructionProperties
                                   {
                                       Id = "id",
                                       Orientation = 1.1,
                                       DikeHeight = 4.4
                                   });
        }
    }
}