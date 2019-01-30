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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoriesViewTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismAssemblyCategoriesView(failureMechanism,
                                                                                 null,
                                                                                 Enumerable.Empty<FailureMechanismAssemblyCategory>,
                                                                                 Enumerable.Empty<FailureMechanismSectionAssemblyCategory>);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetFailureMechanismAssemblyCategoriesFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismAssemblyCategoriesView(failureMechanism,
                                                                                 assessmentSection,
                                                                                 null,
                                                                                 Enumerable.Empty<FailureMechanismSectionAssemblyCategory>);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("getFailureMechanismAssemblyCategoriesFunc", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetFailureMechanismSectionAssemblyCategoriesFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismAssemblyCategoriesView(failureMechanism,
                                                                                 assessmentSection,
                                                                                 Enumerable.Empty<FailureMechanismAssemblyCategory>,
                                                                                 null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("getFailureMechanismSectionAssemblyCategoriesFunc", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithValidParameters_CreatesViewAndTableWithData()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (var view = new FailureMechanismAssemblyCategoriesView(failureMechanism,
                                                                         assessmentSection,
                                                                         Enumerable.Empty<FailureMechanismAssemblyCategory>,
                                                                         Enumerable.Empty<FailureMechanismSectionAssemblyCategory>))
            {
                // Assert
                Assert.IsInstanceOf<CloseForFailureMechanismView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreEqual(1, view.Controls.Count);

                TableLayoutPanel tableLayoutPanel = ControlTestHelper.GetControls<TableLayoutPanel>(view, "tableLayoutPanel").Single();
                Assert.AreEqual(2, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(1, tableLayoutPanel.RowCount);
                Assert.AreEqual(DockStyle.Fill, tableLayoutPanel.Dock);

                var failureMechanismAssemblyGroupBox = (GroupBox) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.AreEqual(1, failureMechanismAssemblyGroupBox.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, failureMechanismAssemblyGroupBox.Dock);
                Assert.AreEqual("Categoriegrenzen voor dit traject", failureMechanismAssemblyGroupBox.Text);

                var failureMechanismSectionAssemblyGroupBox = (GroupBox) tableLayoutPanel.GetControlFromPosition(1, 0);
                Assert.AreEqual(1, failureMechanismSectionAssemblyGroupBox.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, failureMechanismSectionAssemblyGroupBox.Dock);
                Assert.AreEqual("Categoriegrenzen per vak", failureMechanismSectionAssemblyGroupBox.Text);

                AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> failureMechanismCategoriesTable = GetFailureMechanismCategoriesTable(view);
                Assert.AreEqual(DockStyle.Fill, failureMechanismCategoriesTable.Dock);
                AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismSectionCategoriesTable(view);
                Assert.AreEqual(DockStyle.Fill, failureMechanismSectionCategoriesTable.Dock);
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

            var failureMechanism = new TestFailureMechanism();

            var nrOfCategories = 1;
            Func<IEnumerable<FailureMechanismAssemblyCategory>> getFailureMechanismCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismAssemblyCategory(random), nrOfCategories);
            Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismSectionAssemblyCategory(random), nrOfCategories);

            using (var view = new FailureMechanismAssemblyCategoriesView(failureMechanism,
                                                                         assessmentSection,
                                                                         getFailureMechanismCategories,
                                                                         getFailureMechanismSectionCategories))
            {
                AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> failureMechanismCategoriesTable = GetFailureMechanismCategoriesTable(view);
                AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismSectionCategoriesTable(view);

                // Precondition
                Assert.AreEqual(nrOfCategories, failureMechanismCategoriesTable.Rows.Count);
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);

                // When
                nrOfCategories = 2;
                failureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(nrOfCategories, failureMechanismCategoriesTable.Rows.Count);
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithValidData_WhenAssessmentSectionUpdated_ThenDataTableUpdated()
        {
            // Given
            var random = new Random(21);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var nrOfCategories = 1;
            Func<IEnumerable<FailureMechanismAssemblyCategory>> getFailureMechanismCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismAssemblyCategory(random), nrOfCategories);
            Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionCategories =
                () => Enumerable.Repeat(CreateRandomFailureMechanismSectionAssemblyCategory(random), nrOfCategories);

            using (var view = new FailureMechanismAssemblyCategoriesView(failureMechanism,
                                                                         assessmentSection,
                                                                         getFailureMechanismCategories,
                                                                         getFailureMechanismSectionCategories))
            {
                AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> failureMechanismCategoriesTable = GetFailureMechanismCategoriesTable(view);
                AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionCategoriesTable = GetFailureMechanismSectionCategoriesTable(view);

                // Precondition
                Assert.AreEqual(nrOfCategories, failureMechanismCategoriesTable.Rows.Count);
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);

                // When
                nrOfCategories = 2;
                assessmentSection.NotifyObservers();

                // Then
                Assert.AreEqual(nrOfCategories, failureMechanismCategoriesTable.Rows.Count);
                Assert.AreEqual(nrOfCategories, failureMechanismSectionCategoriesTable.Rows.Count);
            }

            mocks.VerifyAll();
        }

        private static FailureMechanismSectionAssemblyCategory CreateRandomFailureMechanismSectionAssemblyCategory(Random random)
        {
            return new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                               random.NextDouble(),
                                                               random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
        }

        private static FailureMechanismAssemblyCategory CreateRandomFailureMechanismAssemblyCategory(Random random)
        {
            return new FailureMechanismAssemblyCategory(random.NextDouble(),
                                                        random.NextDouble(),
                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
        }

        private static AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> GetFailureMechanismCategoriesTable(
            FailureMechanismAssemblyCategoriesView view)
        {
            return ControlTestHelper.GetControls<AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup>>(
                view, "failureMechanismAssemblyCategoriesTable").Single();
        }

        private static AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> GetFailureMechanismSectionCategoriesTable(
            FailureMechanismAssemblyCategoriesView view)
        {
            return ControlTestHelper.GetControls<AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup>>(
                view, "failureMechanismSectionAssemblyCategoriesTable").Single();
        }
    }
}