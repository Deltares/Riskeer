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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.Service;
using Ringtoets.Integration.TestUtils;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class FailureMechanismContributionNormChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new FailureMechanismContributionNormChangeHandler();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismContributionNormChangeHandler>(handler);
        }

        [Test]
        public void ConfirmNormChange_Always_ShownMessageBoxForConfirmation()
        {
            // Setup
            string title = "";
            string message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;
                tester.ClickOk();
            };

            var handler = new FailureMechanismContributionNormChangeHandler();

            // Call
            handler.ConfirmNormChange();

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Na het aanpassen van de norm zullen alle rekenresultaten van hydraulische randvoorwaarden en faalmechanismen verwijderd worden."
                                     + Environment.NewLine
                                     + Environment.NewLine +
                                     "Wilt u doorgaan?";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ConfirmNormChange_MessageBoxOk_ReturnTrue()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var handler = new FailureMechanismContributionNormChangeHandler();

            // Call
            bool isConfirmed = handler.ConfirmNormChange();

            // Assert
            Assert.IsTrue(isConfirmed);
        }

        [Test]
        public void ConfirmNormChange_MessageBoxCancel_ReturnFalse()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            var handler = new FailureMechanismContributionNormChangeHandler();

            // Call
            bool isConfirmed = handler.ConfirmNormChange();

            // Assert
            Assert.IsFalse(isConfirmed);
        }

        [Test]
        public void ChangeNorm_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var handler = new FailureMechanismContributionNormChangeHandler();
            const double norm = 1.0/1000;

            // Call
            TestDelegate call = () => handler.ChangeNorm(null, norm);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void ChangeNorm_InvalidNorm_ThrowArgumentOutOfRangeException(
            [Values(150, 1 + 1e-6, -1e-6, -150, double.NaN)] double invalidNorm)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new FailureMechanismContributionNormChangeHandler();

            // Call
            TestDelegate call = () => handler.ChangeNorm(assessmentSection, invalidNorm);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void ChangeNorm_FullyConfiguredAssessmentSection_AllCalculationOutputClearedAndContributionsUpdatedAndReturnsAllAffectedObjects()
        {
            // Setup
            AssessmentSection section = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            var expectedAffectedCalculations = section.GetFailureMechanisms()
                                                      .SelectMany(fm => fm.Calculations)
                                                      .Where(c => c.HasOutput)
                                                      .ToArray();
            var handler = new FailureMechanismContributionNormChangeHandler();

            IEnumerable<IObservable> affectedObjects = null;
            const double norm = 1.0/1000;

            // Call
            Action call = () => affectedObjects = handler.ChangeNorm(section, norm);

            // Assert
            var expectedMessages = new[]
            {
                "De resultaten van 32 berekeningen zijn verwijderd.",
                "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, 2);

            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);

            CollectionAssert.IsEmpty(section.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput),
                                     "There should be no calculations with output.");

            var expectedAffectedObjects = expectedAffectedCalculations.Cast<IObservable>()
                                                                      .Concat(section.GetFailureMechanisms())
                                                                      .Concat(new IObservable[]
                                                                      {
                                                                          section.FailureMechanismContribution,
                                                                          section.GrassCoverErosionOutwards.HydraulicBoundaryLocations,
                                                                          section.HydraulicBoundaryDatabase
                                                                      });
            foreach (HydraulicBoundaryLocation location in section.HydraulicBoundaryDatabase.Locations
                                                                  .Concat(section.GrassCoverErosionOutwards.HydraulicBoundaryLocations))
            {
                Assert.IsNaN(location.DesignWaterLevel);
                Assert.IsNaN(location.WaveHeight);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.WaveHeightCalculationConvergence);
            }
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void ChangeNorm_FullyConfiguredAssessmentSectionWithoutCalculationOutput_NormChangedContributionsUpdatedAndReturnsAllAffectedObjects()
        {
            // Setup
            AssessmentSection section = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(section);

            // Precondition
            CollectionAssert.IsEmpty(section.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput));

            var handler = new FailureMechanismContributionNormChangeHandler();

            IEnumerable<IObservable> affectedObjects = null;

            const double newNormValue = 0.1234;

            // Call
            Action call = () => affectedObjects = handler.ChangeNorm(section, newNormValue);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd.", 1);

            Assert.AreEqual(newNormValue, section.FailureMechanismContribution.Norm);

            var expectedAffectedObjects = section.GetFailureMechanisms()
                                                 .Concat(new IObservable[]
                                                 {
                                                     section.FailureMechanismContribution,
                                                     section.GrassCoverErosionOutwards.HydraulicBoundaryLocations,
                                                     section.HydraulicBoundaryDatabase
                                                 });
            foreach (HydraulicBoundaryLocation location in section.HydraulicBoundaryDatabase.Locations
                                                                  .Concat(section.GrassCoverErosionOutwards.HydraulicBoundaryLocations))
            {
                Assert.IsNaN(location.DesignWaterLevel);
                Assert.IsNaN(location.WaveHeight);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.WaveHeightCalculationConvergence);
            }
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void ChangeNorm_FullyConfiguredAssessmentSectionWithoutCalculatedHydraulicBoundaryLocations_AllFailureMechanismCalculationOutputClearedAndContributionsUpdatedAndReturnsAllAffectedObjects()
        {
            // Setup
            AssessmentSection section = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(section.HydraulicBoundaryDatabase,
                                                                                     section.GrassCoverErosionOutwards);

            // Precondition
            CollectionAssert.IsEmpty(section.HydraulicBoundaryDatabase.Locations.Where(HasCalculatedHydraulicBoundaryLocationValues));
            CollectionAssert.IsEmpty(section.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Where(HasCalculatedHydraulicBoundaryLocationValues));

            var expectedAffectedCalculations = section.GetFailureMechanisms()
                                                      .SelectMany(fm => fm.Calculations)
                                                      .Where(c => c.HasOutput)
                                                      .ToArray();
            var handler = new FailureMechanismContributionNormChangeHandler();

            IEnumerable<IObservable> affectedObjects = null;
            const double norm = 1.0/1000;

            // Call
            Action call = () => affectedObjects = handler.ChangeNorm(section, norm);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "De resultaten van 32 berekeningen zijn verwijderd.", 1);

            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);

            CollectionAssert.IsEmpty(section.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput),
                                     "There should be no calculations with output.");

            var expectedAffectedObjects = expectedAffectedCalculations.Cast<IObservable>()
                                                                      .Concat(section.GetFailureMechanisms())
                                                                      .Concat(new IObservable[]
                                                                      {
                                                                          section.FailureMechanismContribution,
                                                                      });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        private bool HasCalculatedHydraulicBoundaryLocationValues(HydraulicBoundaryLocation l)
        {
            return !double.IsNaN(l.DesignWaterLevel) || !double.IsNaN(l.WaveHeight);
        }
    }
}