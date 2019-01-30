// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Test fixture class for testing data and styling in a view with a <see cref="FailureMechanismAssemblyCategoryGroupControl"/>.
    /// </summary>
    /// <typeparam name="TView">The type of the view to test.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism the view belongs to.</typeparam>
    /// <typeparam name="TSectionResult">The type of the section results shown in the view.</typeparam>
    /// <typeparam name="TResultRow">The type of the presentation objects used in the view.</typeparam>
    [TestFixture]
    public abstract class FailureMechanismAssemblyCategoryGroupControlTestFixture<TView, TFailureMechanism, TSectionResult, TResultRow>
        where TView : FailureMechanismResultView<TSectionResult, TResultRow, TFailureMechanism, FailureMechanismAssemblyCategoryGroupControl>
        where TFailureMechanism : IFailureMechanism, IHasSectionResults<TSectionResult>, new()
        where TSectionResult : FailureMechanismSectionResult
        where TResultRow : FailureMechanismSectionResultRow<TSectionResult>
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
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Assert
                var assemblyResultPanel = (TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject;
                var assemblyResultControl = (FailureMechanismAssemblyCategoryGroupControl) assemblyResultPanel.GetControlFromPosition(1, 0);

                Assert.IsInstanceOf<FailureMechanismAssemblyCategoryGroupControl>(assemblyResultControl);
                Assert.AreEqual(DockStyle.Left, assemblyResultControl.Dock);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenCalculatorThrowsException_ThenErrorSetToControl()
        {
            // Given
            var failureMechanism = new TFailureMechanism();

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                FailureMechanismAssemblyCategoryGroupControl assemblyControl = GetFailureMechanismAssemblyCategoryGroupControl();
                ErrorProvider errorProvider = GetErrorProvider(assemblyControl);
                Assert.IsEmpty(errorProvider.GetError(assemblyControl));

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;
                failureMechanism.NotifyObservers();

                // Assert
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
                var assemblyGroupLabel = (BorderedLabel) new ControlTester("GroupLabel").TheObject;
                Assert.AreEqual("IIt", assemblyGroupLabel.Text);

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;
                failureMechanism.NotifyObservers();

                // Assert
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
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                using (ShowFailureMechanismResultsView(failureMechanism))
                {
                    // Precondition
                    FailureMechanismAssemblyCategoryGroupControl assemblyControl = GetFailureMechanismAssemblyCategoryGroupControl();
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
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                var assemblyGroupLabel = (BorderedLabel) new ControlTester("GroupLabel").TheObject;
                Assert.AreEqual("IIt", assemblyGroupLabel.Text);

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyCategoryGroupOutput = FailureMechanismAssemblyCategoryGroup.VIt;
                failureMechanism.SectionResults.Single().NotifyObservers();

                // Then
                Assert.AreEqual("VIt", assemblyGroupLabel.Text);
            }
        }

        /// <summary>
        /// Method for creating an instance of <typeparamref name="TView"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create the view for.</param>
        /// <returns>A new <typeparamref name="TView"/>.</returns>
        protected abstract TView CreateResultView(TFailureMechanism failureMechanism);

        private TView ShowFailureMechanismResultsView(TFailureMechanism failureMechanism)
        {
            TView failureMechanismResultView = CreateResultView(failureMechanism);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private static ErrorProvider GetErrorProvider(FailureMechanismAssemblyCategoryGroupControl control)
        {
            return TypeUtils.GetField<ErrorProvider>(control, "errorProvider");
        }

        private static FailureMechanismAssemblyCategoryGroupControl GetFailureMechanismAssemblyCategoryGroupControl()
        {
            return (FailureMechanismAssemblyCategoryGroupControl) ((TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject).GetControlFromPosition(1, 0);
        }
    }
}