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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Groups;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionAssemblyGroupsViewTest
    {
        [Test]
        public void Constructor_FailureMechanismContributionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionAssemblyGroupsView(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanismContribution", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanismContribution_CreatesViewAndTableWithData()
        {
            // Setup
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new AssessmentSectionAssemblyGroupsView(failureMechanismContribution))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(failureMechanismContribution, view.FailureMechanismContribution);

                AssemblyGroupsTable<AssessmentSectionAssemblyGroup> tableControl = GetCategoriesTable(view);
                Assert.AreEqual(DockStyle.Fill, tableControl.Dock);

                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyGroupBoundariesCalculator;
                Assert.AreEqual(calculator.AssessmentSectionAssemblyGroupBoundariesOutput.Count(), tableControl.Rows.Count);
            }
        }

        [Test]
        public void GivenViewWithFailureMechanismContribution_WhenFailureMechanismContributionUpdated_ThenDataTableUpdated()
        {
            // Given
            var random = new Random(21);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();
            failureMechanismContribution.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new AssessmentSectionAssemblyGroupsView(failureMechanismContribution))
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyGroupBoundariesCalculator;

                AssemblyGroupsTable<AssessmentSectionAssemblyGroup> groupsTable = GetCategoriesTable(view);

                // Precondition
                Assert.AreEqual(calculator.AssessmentSectionAssemblyGroupBoundariesOutput.Count(), groupsTable.Rows.Count);

                // When
                var newOutput = new[]
                {
                    new AssessmentSectionAssemblyGroupBoundaries(random.NextDouble(),
                                                                 random.NextDouble(),
                                                                 random.NextEnumValue<AssessmentSectionAssemblyGroup>())
                };
                calculator.AssessmentSectionAssemblyGroupBoundariesOutput = newOutput;
                failureMechanismContribution.NotifyObservers();

                // Then
                Assert.AreEqual(newOutput.Length, groupsTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        private static AssemblyGroupsTable<AssessmentSectionAssemblyGroup> GetCategoriesTable(AssessmentSectionAssemblyGroupsView view)
        {
            return ControlTestHelper.GetControls<AssemblyGroupsTable<AssessmentSectionAssemblyGroup>>(
                view, "assemblyGroupsTable").Single();
        }
    }
}