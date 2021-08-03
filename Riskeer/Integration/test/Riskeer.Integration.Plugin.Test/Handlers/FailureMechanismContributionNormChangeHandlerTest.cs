﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;

namespace Riskeer.Integration.Plugin.Test.Handlers
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
            void Call() => new FailureMechanismContributionNormChangeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueNull_ThrowArgumentNullException()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput();
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            void Call() => handler.SetPropertyValueAfterConfirmation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden alle rekenresultaten van alle hydraulische belastingenlocaties en toetssporen verwijderd."
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
                                            .Concat(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm)
                                            .Concat(assessmentSection.WaterLevelCalculationsForSignalingNorm)
                                            .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                                            .Concat(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm)
                                            .Concat(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm)
                                            .Concat(assessmentSection.WaveHeightCalculationsForSignalingNorm)
                                            .Concat(assessmentSection.WaveHeightCalculationsForLowerLimitNorm)
                                            .Concat(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm)
                                            .Concat(GetAllAffectedDuneLocationCalculations(assessmentSection.DuneErosion))
                                            .Concat(new IObservable[]
                                            {
                                                assessmentSection.FailureMechanismContribution
                                            }).ToArray();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            IEnumerable<IObservable> affectedObjects = null;

            // Call
            void Call() => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            var expectedMessages = new[]
            {
                "De resultaten van 36 berekeningen zijn verwijderd.",
                "Alle berekende hydraulische belastingen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 2);

            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput),
                                     "There should be no calculations with output.");

            AssertHydraulicBoundaryLocationCalculationOutput(assessmentSection, false);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(assessmentSection.DuneErosion);

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
                                 .Concat(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm)
                                 .Concat(assessmentSection.WaterLevelCalculationsForSignalingNorm)
                                 .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                                 .Concat(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm)
                                 .Concat(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm)
                                 .Concat(assessmentSection.WaveHeightCalculationsForSignalingNorm)
                                 .Concat(assessmentSection.WaveHeightCalculationsForLowerLimitNorm)
                                 .Concat(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm)
                                 .Concat(GetAllAffectedDuneLocationCalculations(assessmentSection.DuneErosion))
                                 .Concat(new IObservable[]
                                 {
                                     assessmentSection.FailureMechanismContribution
                                 }).ToArray();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            IEnumerable<IObservable> affectedObjects = null;

            // Call
            void Call() => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, "Alle berekende hydraulische belastingen zijn verwijderd.", 1);

            AssertHydraulicBoundaryLocationCalculationOutput(assessmentSection, false);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(assessmentSection.DuneErosion);

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

            // Call
            void Call() => affectedObjects = handler.SetPropertyValueAfterConfirmation(() => {});

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, "De resultaten van 36 berekeningen zijn verwijderd.", 1);

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
        public void SetPropertyValueAfterConfirmation_FullyConfiguredAssessmentSectionConfirmationGiven_HandlerExecuted()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            var handlerExecuted = false;

            // Call
            handler.SetPropertyValueAfterConfirmation(() => handlerExecuted = true);

            // Assert
            Assert.IsTrue(handlerExecuted);
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

            AssertHydraulicBoundaryLocationCalculationOutput(assessmentSection, true);

            Assert.IsNotNull(assessmentSection.DuneErosion.CalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output);
            Assert.IsNotNull(assessmentSection.DuneErosion.CalculationsForMechanismSpecificSignalingNorm.First().Output);
            Assert.IsNotNull(assessmentSection.DuneErosion.CalculationsForMechanismSpecificLowerLimitNorm.First().Output);
            Assert.IsNotNull(assessmentSection.DuneErosion.CalculationsForLowerLimitNorm.First().Output);
            Assert.IsNotNull(assessmentSection.DuneErosion.CalculationsForFactorizedLowerLimitNorm.First().Output);

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
            void Call() => handler.SetPropertyValueAfterConfirmation(() => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreSame(expectedException, exception);
        }

        private static IEnumerable<IObservable> GetAllAffectedDuneLocationCalculations(DuneErosionFailureMechanism failureMechanism)
        {
            return DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);
        }

        private static void AssertHydraulicBoundaryLocationCalculationOutput(AssessmentSection assessmentSection, bool hasOutput)
        {
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForSignalingNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForLowerLimitNorm.All(c => c.HasOutput == hasOutput));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.All(c => c.HasOutput == hasOutput));
        }
    }
}