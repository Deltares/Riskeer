using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test
{
    [TestFixture]
    public class FailureMechanismPropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void ConfirmPropertyChange_Always_ShowMessageBox()
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

            var handler = new FailureMechanismPropertyChangeHandler();

            // Call
            handler.ConfirmPropertyChange();

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u een parameter in een toetsspoor wijzigt, zal de uitvoer van alle berekeningen in dit toetsspoor verwijderd worden." + Environment.NewLine +
                                     Environment.NewLine +
                                     "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ConfirmPropertyChange_MessageBoxOk_ReturnTrue()
        {
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var handler = new FailureMechanismPropertyChangeHandler();

            // Call
            bool result = handler.ConfirmPropertyChange();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ConfirmPropertyChange_MessageBoxCancel_ReturnFalse()
        {
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            var handler = new FailureMechanismPropertyChangeHandler();

            // Call
            bool result = handler.ConfirmPropertyChange();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ChangeComposition_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new FailureMechanismPropertyChangeHandler();

            // Call
            TestDelegate test = () => handler.PropertyChanged(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        [TestCaseSource("ChangePropertyTestCases")]
        public void ChangeComposition_FailureMechanismWithDifferentCalculationCollections_ReturnsCalculationsWhichHadOutput(ChangePropertyTestCase testCase)
        {
            // Setup
            var handler = new FailureMechanismPropertyChangeHandler();
            IFailureMechanism failureMechanism = new TestFailureMechanism(testCase.Calculations);

            // Call
            IEnumerable<IObservable> result = handler.PropertyChanged(failureMechanism);

            // Assert
            CollectionAssert.AreEquivalent(testCase.ExpectedAffectedCalculations, result);
        }

        public class ChangePropertyTestCase
        {
            public ChangePropertyTestCase(ICollection<TestCalculation> calculations)
            {
                Calculations = calculations;
                ExpectedAffectedCalculations = calculations.Where(c => c.HasOutput).ToArray();
            }

            public ICollection<TestCalculation> Calculations { get; private set; }
            public ICollection<TestCalculation> ExpectedAffectedCalculations { get; private set; }
        }

        static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput()
                             })
            ).SetName("ChangeComposition Single calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("ChangeComposition Single calculation without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithoutOutput(),
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("ChangeComposition Two calculations without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("ChangeComposition Calculation without and calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithOutput()
                             })
            ).SetName("ChangeComposition Two calculations with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("ChangeComposition Two calculations with and one calculation without output");
        }

        private static TestCalculation CreateCalculationWithoutOutput()
        {
            return new TestCalculation();
        }

        private static TestCalculation CreateCalculationWithOutput()
        {
            return new TestCalculation
            {
                Output = new object()
            };
        }
    }

}