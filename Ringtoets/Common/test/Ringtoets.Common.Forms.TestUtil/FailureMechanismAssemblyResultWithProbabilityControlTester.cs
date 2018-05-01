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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
    /// Class for testing data and styling in a view with a <see cref="FailureMechanismAssemblyResultWithProbabilityControl"/>.
    /// </summary>
    /// <typeparam name="TView">The type of the view to test.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism the view belongs to.</typeparam>
    /// <typeparam name="TSectionResult">The type of the section results shown in the view.</typeparam>
    /// <typeparam name="TResultRow">The type of the presentation objects used in the view.</typeparam>
    /// <typeparam name="TCalculation">The type of calculations to get the input from.</typeparam>
    /// <typeparam name="TCalculationInput">The type of the  input of a calculation.</typeparam>
    public abstract class FailureMechanismAssemblyResultWithProbabilityControlTester<TView, TFailureMechanism, TSectionResult, TResultRow, TCalculation, TCalculationInput>
        where TView : FailureMechanismResultView<TSectionResult, TResultRow, TFailureMechanism>
        where TFailureMechanism : IFailureMechanism, IHasSectionResults<TSectionResult>, ICalculatableFailureMechanism, new()
        where TSectionResult : FailureMechanismSectionResult
        where TResultRow : FailureMechanismSectionResultRow<TSectionResult>
        where TCalculation : ICalculation
        where TCalculationInput : ICalculationInput
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
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Assert
                var assemblyResultPanel = (TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject;
                var assemblyResultControl = (FailureMechanismAssemblyResultWithProbabilityControl) assemblyResultPanel.GetControlFromPosition(0, 0);

                Assert.IsInstanceOf<FailureMechanismAssemblyResultWithProbabilityControl>(assemblyResultControl);
                Assert.AreEqual(DockStyle.Left, assemblyResultControl.Dock);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenResultChangedAndSectionResultNotified_FailureMechanismAssemblyResultUpdated()
        {
            // Given
            var failureMechanism = new TFailureMechanism();
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                BorderedLabel assemblyGroupLabel = GetGroupLabel();
                BorderedLabel assemblyProbabilityLabel = GetProbabilityLabelControl();
                Assert.AreEqual("1/1", assemblyProbabilityLabel.Text);
                Assert.AreEqual("IIIt", assemblyGroupLabel.Text);
                Assert.AreEqual(Color.FromArgb(255, 255, 0), assemblyGroupLabel.BackColor);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(0.5, FailureMechanismAssemblyCategoryGroup.VIt);
                failureMechanism.SectionResults.Single().NotifyObservers();

                // Assert
                Assert.AreEqual("1/2", assemblyProbabilityLabel.Text);
                Assert.AreEqual("VIt", assemblyGroupLabel.Text);
                Assert.AreEqual(Color.FromArgb(255, 0, 0), assemblyGroupLabel.BackColor);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenResultChangedAndCalculationNotified_FailureMechanismAssemblyResultUpdated()
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
                Assert.AreEqual(Color.FromArgb(255, 255, 0), assemblyGroupLabel.BackColor);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(0.5, FailureMechanismAssemblyCategoryGroup.VIt);
                calculation.NotifyObservers();

                // Assert
                Assert.AreEqual("1/2", assemblyProbabilityLabel.Text);
                Assert.AreEqual("VIt", assemblyGroupLabel.Text);
                Assert.AreEqual(Color.FromArgb(255, 0, 0), assemblyGroupLabel.BackColor);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenResultChangedAndCalculationInputNotified_FailureMechanismAssemblyResultUpdated()
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
                Assert.AreEqual(Color.FromArgb(255, 255, 0), assemblyGroupLabel.BackColor);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(0.5, FailureMechanismAssemblyCategoryGroup.VIt);
                GetInput(failureMechanism.CalculationsGroup.Children.Cast<TCalculation>().Single()).NotifyObservers();

                // Assert
                Assert.AreEqual("1/2", assemblyProbabilityLabel.Text);
                Assert.AreEqual("VIt", assemblyGroupLabel.Text);
                Assert.AreEqual(Color.FromArgb(255, 0, 0), assemblyGroupLabel.BackColor);
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

        /// <summary>
        /// Method to get the <typeparamref name="TCalculationInput"/> from a
        /// <typeparamref name="TCalculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to get the input from.</param>
        /// <returns>A new <typeparamref name="TCalculationInput"/>.</returns>
        protected abstract TCalculationInput GetInput(TCalculation calculation);

        private static BorderedLabel GetGroupLabel()
        {
            return (BorderedLabel) new ControlTester("GroupLabel").TheObject;
        }

        private static BorderedLabel GetProbabilityLabelControl()
        {
            return (BorderedLabel) new ControlTester("probabilityLabel").TheObject;
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