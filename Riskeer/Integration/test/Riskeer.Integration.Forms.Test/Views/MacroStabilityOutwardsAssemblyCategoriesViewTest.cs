// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityOutwardsAssemblyCategoriesViewTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new MacroStabilityOutwardsAssemblyCategoriesView(failureMechanism,
                                                                                       null,
                                                                                       Enumerable.Empty<FailureMechanismSectionAssemblyCategory>);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_GetFailureMechanismSectionAssemblyCategoriesFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MacroStabilityOutwardsAssemblyCategoriesView(failureMechanism,
                                                                                       assessmentSection,
                                                                                       null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("getFailureMechanismSectionAssemblyCategoriesFunc", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithValidParameters_ExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (var view = new MacroStabilityOutwardsAssemblyCategoriesView(failureMechanism,
                                                                               assessmentSection,
                                                                               Enumerable.Empty<FailureMechanismSectionAssemblyCategory>))
            {
                // Assert
                Assert.IsInstanceOf<CloseForFailureMechanismView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreEqual(1, view.Controls.Count);

                Panel groupBoxPanel = ControlTestHelper.GetControls<Panel>(view, "groupBoxPanel").Single();
                Assert.AreEqual(1, groupBoxPanel.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, groupBoxPanel.Dock);

                GroupBox failureMechanismSectionAssemblyGroupBox = ControlTestHelper.GetControls<GroupBox>(groupBoxPanel, "failureMechanismSectionAssemblyGroupBox").Single();
                Assert.AreEqual(1, failureMechanismSectionAssemblyGroupBox.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, failureMechanismSectionAssemblyGroupBox.Dock);
                Assert.AreEqual("Categoriegrenzen per vak", failureMechanismSectionAssemblyGroupBox.Text);

                AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismSectionCategoriesTable(view);
                Assert.AreEqual(DockStyle.Fill, failureMechanismSectionCategoriesTable.Dock);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithValidParameters_FillTableWithData()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            const int nrOfCategories = 1;
            Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismSectionAssemblyCategory(random), nrOfCategories);

            // Call
            using (var view = new MacroStabilityOutwardsAssemblyCategoriesView(failureMechanism,
                                                                               assessmentSection,
                                                                               getFailureMechanismSectionCategories))
            {
                AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismSectionCategoriesTable(view);

                // Assert
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithValidData_WhenFailureMechanismUpdated_ThenDataTableUpdated()
        {
            // Given
            var random = new Random(21);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var nrOfCategories = 1;
            Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismSectionAssemblyCategory(random), nrOfCategories);

            using (var view = new MacroStabilityOutwardsAssemblyCategoriesView(failureMechanism,
                                                                               assessmentSection,
                                                                               getFailureMechanismSectionCategories))
            {
                AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismSectionCategoriesTable(view);

                // Precondition
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);

                // When
                nrOfCategories = 2;
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithValidData_WhenAssessmentSectionUpdated_ThenDataTableUpdated()
        {
            // Given
            var random = new Random(21);
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            var nrOfCategories = 1;
            Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismSectionAssemblyCategory(random), nrOfCategories);

            using (var view = new MacroStabilityOutwardsAssemblyCategoriesView(failureMechanism,
                                                                               assessmentSection,
                                                                               getFailureMechanismSectionCategories))
            {
                AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismSectionCategoriesTable(view);

                // Precondition
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);

                // When
                nrOfCategories = 2;
                assessmentSection.NotifyObservers();

                // Then
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);
            }
        }

        private static FailureMechanismSectionAssemblyCategory CreateRandomFailureMechanismSectionAssemblyCategory(Random random)
        {
            return new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                               random.NextDouble(),
                                                               random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
        }

        private static AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> GetFailureMechanismSectionCategoriesTable(
            MacroStabilityOutwardsAssemblyCategoriesView view)
        {
            return ControlTestHelper.GetControls<AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup>>(
                view, "failureMechanismSectionAssemblyCategoriesTable").Single();
        }
    }
}