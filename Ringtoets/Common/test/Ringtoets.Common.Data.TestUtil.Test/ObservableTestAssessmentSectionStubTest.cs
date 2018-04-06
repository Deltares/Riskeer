﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class ObservableTestAssessmentSectionStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            // Assert
            Assert.IsInstanceOf<IAssessmentSection>(assessmentSection);
            Assert.IsInstanceOf<Observable>(assessmentSection);

            Assert.IsNull(assessmentSection.Id);
            Assert.IsNull(assessmentSection.Name);
            Assert.IsNull(assessmentSection.Comments);
            Assert.AreEqual(0, Convert.ToInt32(assessmentSection.Composition));
            Assert.IsNull(assessmentSection.ReferenceLine);
            Assert.IsNotNull(assessmentSection.BackgroundData);
            Assert.AreEqual("Background data", assessmentSection.BackgroundData.Name);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);

            FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
            Assert.AreEqual(NormType.LowerLimit, contribution.NormativeNorm);
            Assert.AreEqual(1.0 / 30000, contribution.SignalingNorm);
            Assert.AreEqual(1.0 / 30000, contribution.LowerLimitNorm);

            FailureMechanismContributionItem[] contributionItems = contribution.Distribution.ToArray();
            Assert.AreEqual(1, contributionItems.Length);

            FailureMechanismContributionItem failureMechanismContributionItem = contributionItems[0];
            Assert.AreEqual(0, failureMechanismContributionItem.Contribution);
            Assert.IsInstanceOf<OtherFailureMechanism>(failureMechanismContributionItem.FailureMechanism);

            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);
        }

        [Test]
        public void GetFailureMechanisms_Always_ReturnEmpty()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            // Call
            IEnumerable<IFailureMechanism> failureMechanism = assessmentSection.GetFailureMechanisms();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism);
        }

        [Test]
        public void ChangeComposition_Call_ThrowsNotImplementedException()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            // Call
            TestDelegate call = () => assessmentSection.ChangeComposition(AssessmentSectionComposition.Dike);

            // Assert
            string message = Assert.Throws<NotImplementedException>(call).Message;
            const string expectedMessage = "Stub only verifies Observable and basic behaviour, use a proper stub when this function is necessary.";
            Assert.AreEqual(expectedMessage, message);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetHydraulicBoundaryLocations_Always_LocationAndCalculationsAdded(bool setCalculationOutput)
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            // Call
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations, setCalculationOutput);

            // Assert
            CollectionAssert.AreEqual(locations, assessmentSection.HydraulicBoundaryDatabase.Locations);

            for (var i = 0; i < locations.Length; i++)
            {
                AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i), locations[i], setCalculationOutput);
                AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(i), locations[i], setCalculationOutput);
                AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(i), locations[i], setCalculationOutput);
                AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(i), locations[i], setCalculationOutput);
                AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ElementAt(i), locations[i], setCalculationOutput);
                AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForSignalingNorm.ElementAt(i), locations[i], setCalculationOutput);
                AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ElementAt(i), locations[i], setCalculationOutput);
                AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(i), locations[i], setCalculationOutput);
            }
        }

        [Test]
        public void SetHydraulicBoundaryLocations_LocationsAlreadyAdded_ClearOldLocationsAndCalculations()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            var originalLocations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(originalLocations);

            // Precondition
            CollectionAssert.AreEqual(originalLocations, assessmentSection.HydraulicBoundaryDatabase.Locations);

            var newLocations = new[]
            {
                new TestHydraulicBoundaryLocation("Test")
            };

            // Call
            assessmentSection.SetHydraulicBoundaryLocationCalculations(newLocations);

            // Assert
            foreach (TestHydraulicBoundaryLocation location in originalLocations)
            {
                CollectionAssert.DoesNotContain(assessmentSection.HydraulicBoundaryDatabase.Locations, location);
                CollectionAssert.DoesNotContain(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
                CollectionAssert.DoesNotContain(assessmentSection.WaterLevelCalculationsForSignalingNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
                CollectionAssert.DoesNotContain(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
                CollectionAssert.DoesNotContain(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
                CollectionAssert.DoesNotContain(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
                CollectionAssert.DoesNotContain(assessmentSection.WaveHeightCalculationsForSignalingNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
                CollectionAssert.DoesNotContain(assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
                CollectionAssert.DoesNotContain(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Select(calculation => calculation.HydraulicBoundaryLocation), location);
            }
        }

        private static void AssertHydraulicBoundaryCalculations(HydraulicBoundaryLocationCalculation calculation,
                                                                TestHydraulicBoundaryLocation expectedHydraulicBoundaryLocation,
                                                                bool expectedHasOutput)
        {
            Assert.AreSame(expectedHydraulicBoundaryLocation, calculation.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedHasOutput, calculation.HasOutput);
        }
    }
}