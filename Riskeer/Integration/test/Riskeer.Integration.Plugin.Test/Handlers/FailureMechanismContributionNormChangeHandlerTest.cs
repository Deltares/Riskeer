// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util.Extensions;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data.SemiProbabilistic;

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
            Assert.IsInstanceOf<IFailureMechanismContributionNormChangeHandler>(handler);
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
        public void ChangeNormativeNormType_ActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            void Call() => handler.ChangeNormativeNormType(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("action", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormativeNormType_WithAction_ConfirmationRequired()
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

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            handler.ChangeNormativeNormType(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden de rekenresultaten van semi-probabilistische berekeningen zonder handmatig toetspeil verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithOutput_WhenChangingNormativeNormType_ThenAllDependingOutputClearedAndActionPerformedAndAllAffectedObjectsNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            ICalculation[] expectedAffectedCalculations = assessmentSection.Piping
                                                                           .Calculations
                                                                           .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                           .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput)
                                                                           .Concat<ICalculation>(
                                                                               assessmentSection.MacroStabilityInwards
                                                                                                .Calculations
                                                                                                .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                                .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput))
                                                                           .ToArray();

            IEnumerable<IObservable> expectedAffectedObjects =
                expectedAffectedCalculations.Concat(new IObservable[]
                                            {
                                                assessmentSection.FailureMechanismContribution
                                            })
                                            .ToArray();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(expectedAffectedObjects.Count());
            mocks.ReplayAll();

            expectedAffectedObjects.ForEachElementDo(obj => obj.Attach(observer));

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNormType(() => actionPerformed = true);

            // Then
            var expectedMessages = new[]
            {
                $"De resultaten van {expectedAffectedCalculations.Length} semi-probabilistische berekeningen zonder handmatige waterstand zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 1);
            Assert.IsTrue(actionPerformed);
            CollectionAssert.IsEmpty(expectedAffectedCalculations.Where(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithoutOutput_WhenChangingNormativeNormType_ThenActionPerformedAndContributionNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            ICalculation[] calculations = assessmentSection.Piping
                                                           .Calculations
                                                           .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                           .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput)
                                                           .Concat<ICalculation>(
                                                               assessmentSection.MacroStabilityInwards
                                                                                .Calculations
                                                                                .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput))
                                                           .ToArray();

            calculations.ForEachElementDo(c => c.ClearOutput());

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            assessmentSection.FailureMechanismContribution.Attach(observer);

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNormType(() => actionPerformed = true);

            // Then
            TestHelper.AssertLogMessagesCount(Call, 0);
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormativeNormActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            void Call() => handler.ChangeNormativeNorm(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("action", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormativeNorm_WithAction_ConfirmationRequired()
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

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            handler.ChangeNormativeNorm(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm en semi-probabilistische berekeningen zonder handmatig toetspeil verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithOutput_WhenChangingNormativeNorm_ThenAllDependingOutputClearedAndActionPerformedAndAllAffectedObjectsNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            ICalculation[] expectedAffectedCalculations = assessmentSection.Piping
                                                                           .Calculations
                                                                           .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                           .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput)
                                                                           .Concat<ICalculation>(
                                                                               assessmentSection.MacroStabilityInwards
                                                                                                .Calculations
                                                                                                .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                                .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput))
                                                                           .ToArray();

            IEnumerable<IObservable> expectedAffectedObjects =
                expectedAffectedCalculations.Concat(new IObservable[]
                                            {
                                                assessmentSection.FailureMechanismContribution
                                            })
                                            .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations)
                                            .ToArray();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(expectedAffectedObjects.Count());
            mocks.ReplayAll();

            expectedAffectedObjects.ForEachElementDo(obj => obj.Attach(observer));

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Precondition
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations.Where(c => c.HasOutput));

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNorm(() => actionPerformed = true);

            // Then
            var expectedMessages = new[]
            {
                "Alle berekende hydraulische belastingen behorende bij de gewijzigde norm zijn verwijderd.",
                $"De resultaten van {expectedAffectedCalculations.Length} semi-probabilistische berekeningen zonder handmatige waterstand zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 2);

            Assert.IsTrue(actionPerformed);
            CollectionAssert.IsEmpty(expectedAffectedCalculations.Where(c => c.HasOutput));
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations.Where(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithoutOutput_WhenChangingNormativeNorm_ThenActionPerformedAndContributionNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            ICalculation[] calculations = assessmentSection.Piping
                                                           .Calculations
                                                           .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                           .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput)
                                                           .Concat<ICalculation>(
                                                               assessmentSection.MacroStabilityInwards
                                                                                .Calculations
                                                                                .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput))
                                                           .ToArray();
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            calculations.ForEachElementDo(c =>
            {
                c.ClearOutput();
                c.Attach(observer);
            });
            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations.ForEachElementDo(c =>
            {
                c.Output = null;
                c.Attach(observer);
            });
            assessmentSection.FailureMechanismContribution.Attach(observer);

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNorm(() => actionPerformed = true);

            // Then
            TestHelper.AssertLogMessagesCount(Call, 0);
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            void Call() => handler.ChangeNorm(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("action", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNorm_WithAction_ConfirmationRequired()
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

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            handler.ChangeNorm(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithOutput_WhenChangingNorm_ThenAllDependingOutputClearedAndActionPerformedAndAllAffectedObjectsNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            IEnumerable<IObservable> expectedAffectedObjects = new IObservable[]
                                                               {
                                                                   assessmentSection.FailureMechanismContribution
                                                               }
                                                               .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations)
                                                               .ToArray();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(expectedAffectedObjects.Count());
            mocks.ReplayAll();

            expectedAffectedObjects.ForEachElementDo(obj => obj.Attach(observer));

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Precondition
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations.Where(c => c.HasOutput));

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNorm(() => actionPerformed = true);

            // Then
            var expectedMessages = new[]
            {
                "Alle berekende hydraulische belastingen behorende bij de gewijzigde norm zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 1);

            Assert.IsTrue(actionPerformed);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations.Where(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithoutOutput_WhenChangingNorm_ThenActionPerformedAndContributionNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.HydraulicBoundaryLocationCalculations.ForEachElementDo(c =>
            {
                c.Output = null;
                c.Attach(observer);
            });
            assessmentSection.FailureMechanismContribution.Attach(observer);

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNorm(() => actionPerformed = true);

            // Then
            TestHelper.AssertLogMessagesCount(Call, 0);
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }
    }
}