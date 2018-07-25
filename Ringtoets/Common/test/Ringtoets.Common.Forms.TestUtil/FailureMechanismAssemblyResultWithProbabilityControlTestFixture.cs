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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Test fixture class for testing data and styling in a view with a <see cref="FailureMechanismAssemblyControl"/>.
    /// </summary>
    /// <typeparam name="TView">The type of the view to test.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism the view belongs to.</typeparam>
    /// <typeparam name="TSectionResult">The type of the section results shown in the view.</typeparam>
    /// <typeparam name="TResultRow">The type of the presentation objects used in the view.</typeparam>
    /// <typeparam name="TCalculation">The type of calculations to get the input from.</typeparam>
    [TestFixture]
    public abstract class FailureMechanismAssemblyResultWithProbabilityControlTestFixture<TView, TFailureMechanism, TSectionResult, TResultRow, TCalculation>
        where TView : FailureMechanismResultView<TSectionResult, TResultRow, TFailureMechanism, FailureMechanismAssemblyControl>
        where TFailureMechanism : IFailureMechanism, IHasSectionResults<TSectionResult>, ICalculatableFailureMechanism, new()
        where TSectionResult : FailureMechanismSectionResult
        where TResultRow : FailureMechanismSectionResultRow<TSectionResult>
        where TCalculation : ICalculation<ICalculationInput>
    {
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void FailureMechanismResultsView_Always_FailureMechanismResultControlCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new TFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Assert
                var assemblyResultPanel = (TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject;
                var assemblyResultControl = (FailureMechanismAssemblyControl) assemblyResultPanel.GetControlFromPosition(1, 0);

                Assert.IsInstanceOf<FailureMechanismAssemblyControl>(assemblyResultControl);
                Assert.AreEqual(DockStyle.Left, assemblyResultControl.Dock);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenNoExceptionThrownByCalculator_ThenErrorSetToControl()
        {
            // Given
            var failureMechanism = new TFailureMechanism();

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                FailureMechanismAssemblyControl assemblyControl = GetFailureMechanismAssemblyControl();
                ErrorProvider errorProvider = GetErrorProvider(assemblyControl);
                Assert.IsEmpty(errorProvider.GetError(assemblyControl));

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual("Message", errorProvider.GetError(assemblyControl));
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithAssemblyResult_WhenCalculatorThrowsException_ThenFailureMechanismAssemblyResultCleared()
        {
            // Given
            var failureMechanism = new TFailureMechanism();

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                BorderedLabel assemblyGroupLabel = GetGroupLabel();
                BorderedLabel probabilityLabel = GetProbabilityLabelControl();
                Assert.AreEqual("1/1", probabilityLabel.Text);
                Assert.AreEqual("IIIt", assemblyGroupLabel.Text);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;
                failureMechanism.NotifyObservers();

                // Assert
                Assert.AreEqual("-", probabilityLabel.Text);
                Assert.IsEmpty(assemblyGroupLabel.Text);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithError_WhenNoExceptionThrownByCalculator_ThenErrorCleared()
        {
            // Given
            var failureMechanism = new TFailureMechanism();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                using (ShowFailureMechanismResultsView(failureMechanism))
                {
                    // Precondition
                    FailureMechanismAssemblyControl assemblyControl = GetFailureMechanismAssemblyControl();
                    ErrorProvider errorProvider = GetErrorProvider(assemblyControl);
                    Assert.AreEqual("Message", errorProvider.GetError(assemblyControl));

                    // When
                    calculator.ThrowExceptionOnCalculate = false;
                    failureMechanism.NotifyObservers();

                    // Then
                    Assert.IsEmpty(errorProvider.GetError(assemblyControl));
                }
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithAssemblyResult_WhenFailureMechanismAssemblyResultChangedAndSectionResultNotified_ThenFailureMechanismAssemblyResultUpdated()
        {
            // Given
            var failureMechanism = new TFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                BorderedLabel assemblyGroupLabel = GetGroupLabel();
                BorderedLabel assemblyProbabilityLabel = GetProbabilityLabelControl();
                Assert.AreEqual("1/1", assemblyProbabilityLabel.Text);
                Assert.AreEqual("IIIt", assemblyGroupLabel.Text);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(0.5, FailureMechanismAssemblyCategoryGroup.VIt);
                failureMechanism.SectionResults.Single().NotifyObservers();

                // Then
                Assert.AreEqual("1/2", assemblyProbabilityLabel.Text);
                Assert.AreEqual("VIt", assemblyGroupLabel.Text);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithAssemblyResult_WhenFailureMechanismAssemblyResultChangedAndCalculationNotified_ThenFailureMechanismAssemblyResultUpdated()
        {
            // Given
            var failureMechanism = new TFailureMechanism();
            TCalculation calculation = CreateCalculation();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                BorderedLabel assemblyGroupLabel = GetGroupLabel();
                BorderedLabel assemblyProbabilityLabel = GetProbabilityLabelControl();
                Assert.AreEqual("1/1", assemblyProbabilityLabel.Text);
                Assert.AreEqual("IIIt", assemblyGroupLabel.Text);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(0.5, FailureMechanismAssemblyCategoryGroup.VIt);
                calculation.NotifyObservers();

                // Then
                Assert.AreEqual("1/2", assemblyProbabilityLabel.Text);
                Assert.AreEqual("VIt", assemblyGroupLabel.Text);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithAssemblyResult_WhenFailureMechanismAssemblyResultChangedAndCalculationInputNotified_ThenFailureMechanismAssemblyResultUpdated()
        {
            // Given
            var failureMechanism = new TFailureMechanism();
            TCalculation calculation = CreateCalculation();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                BorderedLabel assemblyGroupLabel = GetGroupLabel();
                BorderedLabel assemblyProbabilityLabel = GetProbabilityLabelControl();
                Assert.AreEqual("1/1", assemblyProbabilityLabel.Text);
                Assert.AreEqual("IIIt", assemblyGroupLabel.Text);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(0.5, FailureMechanismAssemblyCategoryGroup.VIt);
                calculation.InputParameters.NotifyObservers();

                // Then
                Assert.AreEqual("1/2", assemblyProbabilityLabel.Text);
                Assert.AreEqual("VIt", assemblyGroupLabel.Text);
            }
        }

        /// <summary>
        /// Method for creating an instance of <typeparamref name="TView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create the view for.</param>
        /// <returns>A new <typeparamref name="TView"/>.</returns>
        protected abstract TView CreateResultView(TFailureMechanism failureMechanism);

        /// <summary>
        /// Method for creating an instance of <typeparamref name="TCalculation"/>.
        /// </summary>
        /// <returns>A new <typeparamref name="TCalculation"/>.</returns>
        protected abstract TCalculation CreateCalculation();

        private static BorderedLabel GetGroupLabel()
        {
            return (BorderedLabel) new ControlTester("GroupLabel").TheObject;
        }

        private static BorderedLabel GetProbabilityLabelControl()
        {
            return (BorderedLabel) new ControlTester("ProbabilityLabel").TheObject;
        }

        private static ErrorProvider GetErrorProvider(FailureMechanismAssemblyControl control)
        {
            return TypeUtils.GetField<ErrorProvider>(control, "errorProvider");
        }

        private static FailureMechanismAssemblyControl GetFailureMechanismAssemblyControl()
        {
            return (FailureMechanismAssemblyControl) ((TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject).GetControlFromPosition(1, 0);
        }

        private TView ShowFailureMechanismResultsView(TFailureMechanism failureMechanism)
        {
            TView failureMechanismResultView = CreateResultView(failureMechanism);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}