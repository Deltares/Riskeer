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

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionAssemblyCategoriesViewTest
    {
        [Test]
        public void Constructor_FailureMechanismContributionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new AssessmentSectionAssemblyCategoriesView(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanismContribution", paramName);
        }

        [Test]
        public void Constructor_WithData_CreatesViewAndTableWithData()
        {
            // Setup
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new AssessmentSectionAssemblyCategoriesView(failureMechanismContribution))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(failureMechanismContribution, view.FailureMechanismContribution);

                AssessmentSectionAssemblyCategoriesTable tableControl = GetCategoriesTable(view);
                Assert.NotNull(tableControl);
                Assert.AreEqual(DockStyle.Fill, tableControl.Dock);
                Assert.AreEqual(3, tableControl.Rows.Count);
            }
        }

        [Test]
        public void GivenViewWithFailureMechanismContribution_WhenFailureMechanismContributionUpdated_ThenDataTableUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new AssessmentSectionAssemblyCategoriesView(failureMechanismContribution))
            {
                AssessmentSectionAssemblyCategoriesTable categoriesTable = GetCategoriesTable(view);
                failureMechanismContribution.Attach(observer);

                // Precondition
                Assert.AreEqual(3, categoriesTable.Rows.Count);

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                calculator.AssessmentSectionCategoriesOutput = new[]
                {
                    new AssessmentSectionAssemblyCategory(1, 2, AssessmentSectionAssemblyCategoryGroup.A)
                };
                failureMechanismContribution.NotifyObservers();

                // Then
                Assert.AreEqual(1, categoriesTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        private static AssessmentSectionAssemblyCategoriesTable GetCategoriesTable(AssessmentSectionAssemblyCategoriesView view)
        {
            return ControlTestHelper.GetControls<AssessmentSectionAssemblyCategoriesTable>(view, "assessmentSectionAssemblyCategoriesTable").Single();
        }
    }
}