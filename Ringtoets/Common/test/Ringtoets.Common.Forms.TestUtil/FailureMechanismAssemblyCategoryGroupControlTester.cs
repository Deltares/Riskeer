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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class for testing data and styling in a view with a <see cref="FailureMechanismAssemblyCategoryGroupControl"/>.
    /// </summary>
    /// <typeparam name="TView">The type of the view to test.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism the view belongs to.</typeparam>
    /// <typeparam name="TSectionResult">The type of the section results shown in the view.</typeparam>
    /// <typeparam name="TResultRow">The type of the presentation objects used in the view.</typeparam>
    public abstract class FailureMechanismAssemblyCategoryGroupControlTester<TView, TFailureMechanism, TSectionResult, TResultRow>
        where TView : FailureMechanismResultView<TSectionResult, TResultRow, TFailureMechanism>
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
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

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
        public void GivenFailureMechanismResultsView_WhenResultChangedAndSectionResultNotified_FailureMechanismAssemblyResultUpdated()
        {
            // Given
            var failureMechanism = new TFailureMechanism();
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                var assemblyGroupLabel = (BorderedLabel) new ControlTester("GroupLabel").TheObject;
                Assert.AreEqual("IIt", assemblyGroupLabel.Text);
                Assert.AreEqual(Color.FromArgb(118, 147, 60), assemblyGroupLabel.BackColor);

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.FailureMechanismAssemblyCategoryGroupOutput = FailureMechanismAssemblyCategoryGroup.VIt;
                failureMechanism.SectionResults.Single().NotifyObservers();

                // Assert
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

        private TView ShowFailureMechanismResultsView(TFailureMechanism failureMechanism)
        {
            TView failureMechanismResultView = CreateResultView(failureMechanism);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}