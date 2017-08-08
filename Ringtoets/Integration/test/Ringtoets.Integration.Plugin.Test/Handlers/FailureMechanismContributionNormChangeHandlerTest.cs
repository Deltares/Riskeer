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
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.DuneErosion.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtils;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class FailureMechanismContributionNormChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservablePropertyChangeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FailureMechanismContributionNormChangeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueNull_ThrowArgumentNullException()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput();
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            TestDelegate test = () => handler.SetPropertyValueAfterConfirmation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setValue", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_Always_ConfirmationRequired()
        {
            // Setup
            var title = "";
            var message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;

                tester.ClickCancel();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput();
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            handler.SetPropertyValueAfterConfirmation(() => assessmentSection.FailureMechanismContribution.Norm = 0.1);

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden alle rekenresultaten van alle hydraulische randvoorwaarden en toetssporen verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine +
                                     "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_FullyConfiguredAssessmentSectionConfirmationGiven_AllCalculationOutputClearedAndContributionsUpdatedAndReturnsAllAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            ICalculation[] expectedAffectedCalculations = assessmentSection.GetFailureMechanisms()
                                                                           .SelectMany(fm => fm.Calculations)
                                                                           .Where(c => c.HasOutput)
                                                                           .ToArray();

            IEnumerable<IObservable> expectedAffectedObjects =
                expectedAffectedCalculations.Cast<IObservable>()
                                            .Concat(assessmentSection.GetFailureMechanisms())
                                            .Concat(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
                                            .Concat(assessmentSection.HydraulicBoundaryDatabase.Locations)
                                            .Concat(assessmentSection.DuneErosion.DuneLocations.Where(dl => dl.Output != null))
                                            .Concat(new IObservable[]
                                            {
                                                assessmentSection.FailureMechanismContribution,
                                                assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations,
                                                assessmentSection.HydraulicBoundaryDatabase,
                                                assessmentSection.DuneErosion.DuneLocations
                                            }).ToList();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            IEnumerable<IObservable> affectedObjects = null;
            const double norm = 1.0 / 1000;

            // Call
            Action call = () => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => assessmentSection.FailureMechanismContribution.Norm = norm);

            // Assert
            var expectedMessages = new[]
            {
                "De resultaten van 36 berekeningen zijn verwijderd.",
                "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);

            Assert.AreEqual(norm, assessmentSection.FailureMechanismContribution.Norm);

            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput),
                                     "There should be no calculations with output.");

            foreach (HydraulicBoundaryLocation location in assessmentSection.HydraulicBoundaryDatabase.Locations
                                                                            .Concat(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations))
            {
                Assert.IsFalse(location.DesignWaterLevelCalculation.HasOutput);
                Assert.IsFalse(location.WaveHeightCalculation.HasOutput);
            }
            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_FullyConfiguredAssessmentSectionWithoutCalculationOutputConfirmationGiven_NormChangedContributionsUpdatedAndReturnsAllAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput();

            IEnumerable<IObservable> expectedAffectedObjects =
                assessmentSection.GetFailureMechanisms().Cast<IObservable>()
                                 .Concat(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
                                 .Concat(assessmentSection.HydraulicBoundaryDatabase.Locations)
                                 .Concat(assessmentSection.DuneErosion.DuneLocations.Where(dl => dl.Output != null))
                                 .Concat(new IObservable[]
                                 {
                                     assessmentSection.FailureMechanismContribution,
                                     assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations,
                                     assessmentSection.HydraulicBoundaryDatabase,
                                     assessmentSection.DuneErosion.DuneLocations
                                 }).ToList();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            IEnumerable<IObservable> affectedObjects = null;

            const double newNormValue = 0.01234;

            // Call
            Action call = () => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => assessmentSection.FailureMechanismContribution.Norm = newNormValue);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd.", 1);

            Assert.AreEqual(newNormValue, assessmentSection.FailureMechanismContribution.Norm);

            foreach (HydraulicBoundaryLocation location in assessmentSection.HydraulicBoundaryDatabase.Locations
                                                                            .Concat(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations))
            {
                Assert.IsFalse(location.DesignWaterLevelCalculation.HasOutput);
                Assert.IsFalse(location.WaveHeightCalculation.HasOutput);
            }
            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_FullyConfiguredAssessmentSectionWithoutCalculatedHydraulicBoundaryLocationsConfirmationGiven_AllFailureMechanismCalculationOutputClearedAndContributionsUpdatedAndReturnsAllAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput();

            ICalculation[] expectedAffectedCalculations = assessmentSection.GetFailureMechanisms()
                                                                           .SelectMany(fm => fm.Calculations)
                                                                           .Where(c => c.HasOutput)
                                                                           .ToArray();
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            IEnumerable<IObservable> affectedObjects = null;
            const double norm = 1.0 / 1000;

            // Call
            Action call = () => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => assessmentSection.FailureMechanismContribution.Norm = norm);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "De resultaten van 36 berekeningen zijn verwijderd.", 1);

            Assert.AreEqual(norm, assessmentSection.FailureMechanismContribution.Norm);

            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput),
                                     "There should be no calculations with output.");

            IEnumerable<IObservable> expectedAffectedObjects = expectedAffectedCalculations.Cast<IObservable>()
                                                                                           .Concat(assessmentSection.GetFailureMechanisms())
                                                                                           .Concat(new IObservable[]
                                                                                           {
                                                                                               assessmentSection.FailureMechanismContribution
                                                                                           });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotGiven_SetValueNotCalledNoAffectedObjects()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            var propertySet = 0;

            // Call
            IEnumerable<IObservable> affectedObjects = handler.SetPropertyValueAfterConfirmation(() => propertySet++);

            // Assert
            Assert.AreEqual(0, propertySet);
            foreach (HydraulicBoundaryLocation location in assessmentSection.HydraulicBoundaryDatabase.Locations
                                                                            .Concat(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations))
            {
                Assert.IsTrue(location.DesignWaterLevelCalculation.HasOutput);
                Assert.IsTrue(location.WaveHeightCalculation.HasOutput);
            }
            Assert.IsNotNull(assessmentSection.DuneErosion.DuneLocations[1].Output);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationGivenExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => handler.SetPropertyValueAfterConfirmation(() => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
        }
    }
}