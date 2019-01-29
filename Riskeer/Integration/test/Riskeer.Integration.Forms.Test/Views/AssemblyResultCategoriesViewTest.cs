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
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultCategoriesViewTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyResultCategoriesView(null, () => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_GetAssemblyCategoriesFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyResultCategoriesView(new AssessmentSection(AssessmentSectionComposition.Dike), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getAssemblyCategoriesFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            using (var view = new AssemblyResultCategoriesView(assessmentSection, Enumerable.Empty<FailureMechanismAssemblyCategory>))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(assessmentSection, view.AssessmentSection);

                Assert.AreEqual(1, view.Controls.Count);

                Panel groupBoxPanel = ControlTestHelper.GetControls<Panel>(view, "groupBoxPanel").Single();
                Assert.AreEqual(1, groupBoxPanel.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, groupBoxPanel.Dock);

                GroupBox groupBox = ControlTestHelper.GetControls<GroupBox>(groupBoxPanel, "groupBox").Single();
                Assert.AreEqual(1, groupBox.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, groupBox.Dock);
                Assert.AreEqual("Categoriegrenzen voor de gecombineerde toetssporen 1 en 2", groupBox.Text);

                AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismCategoriesTable(view);
                Assert.AreEqual(DockStyle.Fill, failureMechanismSectionCategoriesTable.Dock);
            }
        }

        [Test]
        public void Constructor_WithValidParameters_FillTableWithData()
        {
            // Setup
            var random = new Random(21);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            const int nrOfCategories = 1;
            Func<IEnumerable<FailureMechanismAssemblyCategory>> getAssemblyCategoriesFunc =
                () => Enumerable.Repeat(CreateRandomFailureMechanismAssemblyCategory(random), nrOfCategories);

            // Call
            using (var view = new AssemblyResultCategoriesView(assessmentSection, getAssemblyCategoriesFunc))
            {
                AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismCategoriesTable(view);

                // Assert
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);
            }
        }

        [Test]
        public void GivenViewWithValidData_WhenFailureMechanismContributionUpdated_ThenDataTableUpdated()
        {
            // Given
            var random = new Random(21);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var nrOfCategories = 1;
            Func<IEnumerable<FailureMechanismAssemblyCategory>> getFailureMechanismCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismAssemblyCategory(random), nrOfCategories);

            using (var view = new AssemblyResultCategoriesView(assessmentSection, getFailureMechanismCategories))
            {
                AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismCategoriesTable(view);

                // Precondition
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);

                // When
                nrOfCategories = 2;
                assessmentSection.FailureMechanismContribution.NotifyObservers();

                // Then
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);
            }
        }

        private static AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> GetFailureMechanismCategoriesTable(
            AssemblyResultCategoriesView view)
        {
            return ControlTestHelper.GetControls<AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup>>(
                view, "assemblyCategoriesTable").Single();
        }

        private static FailureMechanismAssemblyCategory CreateRandomFailureMechanismAssemblyCategory(Random random)
        {
            return new FailureMechanismAssemblyCategory(random.NextDouble(),
                                                        random.NextDouble(),
                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
        }
    }
}