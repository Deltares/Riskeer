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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie buitentalud", failureMechanism.Name);
            Assert.AreEqual("GEBU", failureMechanism.Code);
            Assert.AreEqual(3, failureMechanism.Group);
            Assert.IsInstanceOf<GeneralGrassCoverErosionOutwardsInput>(failureMechanism.GeneralInput);

            Assert.AreEqual("Berekeningen", failureMechanism.WaveConditionsCalculationGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);

            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
        }

        [Test]
        public void SetSections_WithSection_SetsSectionResults()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionResults_SectionResultsCleared()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                })
            });

            // Precondition
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnGrassCoverErosionOutwardsWaveConditionsCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                WaveConditionsCalculationGroup =
                {
                    Children =
                    {
                        new CalculationGroup(),
                        new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                        mocks.Stub<ICalculation>(),
                        new GrassCoverErosionOutwardsWaveConditionsCalculation()
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            List<ICalculation> calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            Assert.IsTrue(calculations.All(c => c is GrassCoverErosionOutwardsWaveConditionsCalculation));
            mocks.VerifyAll();
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.SetHydraulicBoundaryLocationCalculations(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocations", paramName);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_Always_PreviousCalculationsCleared()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            failureMechanism.SetHydraulicBoundaryLocationCalculations(new HydraulicBoundaryLocation[]
            {
                new TestHydraulicBoundaryLocation()
            });

            // Precondition
            CollectionAssert.IsNotEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsNotEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsNotEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            CollectionAssert.IsNotEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsNotEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsNotEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);

            // Call
            failureMechanism.SetHydraulicBoundaryLocationCalculations(Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_MultipleHydraulicBoundaryLocations_SetsExpectedCalculations()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            TestHydraulicBoundaryLocation[] hydraulicBoundaryLocations =
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            };

            // Call
            failureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            // Assert
            AssertNumberOfHydraulicBoundaryLocationCalculations(failureMechanism, 2);
            AssertDefaultHydraulicBoundaryLocationCalculations(failureMechanism, 0, hydraulicBoundaryLocation1);
            AssertDefaultHydraulicBoundaryLocationCalculations(failureMechanism, 1, hydraulicBoundaryLocation2);
        }

        private static void AssertNumberOfHydraulicBoundaryLocationCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism, int expectedNumberOfCalculations)
        {
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Count());
        }

        private static void AssertDefaultHydraulicBoundaryLocationCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism, int index, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            AssertDefaultHydraulicBoundaryLocationCalculation(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.ElementAt(index), hydraulicBoundaryLocation);
        }

        private static void AssertDefaultHydraulicBoundaryLocationCalculation(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                                              HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            Assert.AreSame(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation.HydraulicBoundaryLocation);
            Assert.IsNull(hydraulicBoundaryLocationCalculation.Output);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
        }
    }
}