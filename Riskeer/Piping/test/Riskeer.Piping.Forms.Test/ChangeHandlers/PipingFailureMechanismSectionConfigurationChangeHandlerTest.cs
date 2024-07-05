// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.Forms.ChangeHandlers;

namespace Riskeer.Piping.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class PipingFailureMechanismSectionConfigurationChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => new PipingFailureMechanismSectionConfigurationChangeHandler(null, failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfiguration", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new PipingFailureMechanism();

            // Call
            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);

            // Assert
            Assert.IsInstanceOf<IObservablePropertyChangeHandler>(changeHandler);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueNull_ThrowArgumentNullException()
        {
            // Setup
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new PipingFailureMechanism();

            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);

            // Call
            void Call() => changeHandler.SetPropertyValueAfterConfirmation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("setValue", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequired_SetsExpectedPropertiesOnMessageBox()
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

            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(sectionConfiguration.Section);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);

            // Call
            changeHandler.SetPropertyValueAfterConfirmation(() => new object());

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u deze parameter wijzigt, zal de uitvoer van alle probabilistische berekeningen in dit vak verwijderd worden."
                                     + Environment.NewLine + Environment.NewLine +
                                     "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequiredAndNotGiven_SetValueNotCalledAndCalculationNotUpdated()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(sectionConfiguration.Section);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);
            var nrOfPropertyChanges = 0;

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(() => nrOfPropertyChanges++);

            // Assert
            Assert.AreEqual(0, nrOfPropertyChanges);
            Assert.IsTrue(calculation.HasOutput);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequiredAndGiven_ReturnsExpectedAffectedObjectsWithClearedOutputsAndCallsSetValue()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(10.0, 0.0)
            }));
            var otherSectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(15.0, 0.0),
                new Point2D(20.0, 0.0)
            }));
            var affectedCalculation = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(sectionConfiguration.Section);
            var semiProbabilisticCalculation = SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(sectionConfiguration.Section);
            var unaffectedCalculation = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(otherSectionConfiguration.Section);
            var calculationWithoutOutput = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(sectionConfiguration.Section);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                affectedCalculation,
                semiProbabilisticCalculation,
                unaffectedCalculation,
                calculationWithoutOutput
            });

            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);
            var nrOfPropertyChanges = 0;

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(() => nrOfPropertyChanges++);

            // Assert
            Assert.AreEqual(1, nrOfPropertyChanges);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                sectionConfiguration,
                affectedCalculation
            }, affectedObjects);

            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.IsTrue(semiProbabilisticCalculation.HasOutput);
        }

        [Test]
        [TestCaseSource(nameof(GetInquiryNotRequiredTestCases))]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotRequired_ReturnsExpectedAffectedObjectsAndCallsSetValue(
            Func<FailureMechanismSection, IPipingCalculationScenario<PipingInput>> getCalculationScenarioFunc)
        {
            // Setup
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            IPipingCalculationScenario<PipingInput> calculation = getCalculationScenarioFunc(sectionConfiguration.Section);
            bool hasOutput = calculation.HasOutput;

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);
            var nrOfPropertyChanges = 0;

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(() => nrOfPropertyChanges++);

            // Assert
            Assert.AreEqual(1, nrOfPropertyChanges);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                sectionConfiguration
            }, affectedObjects);

            Assert.AreEqual(hasOutput, calculation.HasOutput);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequiredAndGivenExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(sectionConfiguration.Section);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);
            var expectedException = new Exception();

            // Call
            void Call() => changeHandler.SetPropertyValueAfterConfirmation(() => throw expectedException);

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreSame(expectedException, exception);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotRequiredExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var calculation = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(sectionConfiguration.Section);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var changeHandler = new PipingFailureMechanismSectionConfigurationChangeHandler(sectionConfiguration, failureMechanism);
            var expectedException = new Exception();

            // Call
            void Call() => changeHandler.SetPropertyValueAfterConfirmation(() => throw expectedException);

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreSame(expectedException, exception);
        }

        private static IEnumerable<TestCaseData> GetInquiryNotRequiredTestCases()
        {
            yield return new TestCaseData(new Func<FailureMechanismSection, IPipingCalculationScenario<PipingInput>>(SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>))
                .SetName("Semi-probabilistic calculation");

            yield return new TestCaseData(new Func<FailureMechanismSection, IPipingCalculationScenario<PipingInput>>(ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>))
                .SetName("Probabilistic calculation without output");

            FailureMechanismSection otherSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(10.0, 0),
                new Point2D(120.0, 0)
            });
            yield return new TestCaseData(new Func<FailureMechanismSection, IPipingCalculationScenario<PipingInput>>(s => ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(otherSection)))
                .SetName("Probabilistic calculation with output not intersecting");
        }
    }
}