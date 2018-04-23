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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionCombinedAssemblyResultViewTest
    {
        private const int failureMechanismNameColumnIndex = 0;
        private const int failureMechanismCodeColumnIndex = 1;
        private const int failureMechanismGroupColumnIndex = 2;
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
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionCombinedAssemblyResultView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            using (var view = new AssessmentSectionCombinedAssemblyResultView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreEqual(1, view.Controls.Count);

                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void GivenFormWithAssessmentSection_ThenExpectedColumnsAreVisible()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Then
            using (var view = new AssessmentSectionCombinedAssemblyResultView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(3, dataGridView.ColumnCount);

                DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismNameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismCodeColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismGroupColumnIndex]);

                Assert.AreEqual("Toetsspoor", dataGridViewColumns[failureMechanismNameColumnIndex].HeaderText);
                Assert.AreEqual("Label", dataGridViewColumns[failureMechanismCodeColumnIndex].HeaderText);
                Assert.AreEqual("Groep", dataGridViewColumns[failureMechanismGroupColumnIndex].HeaderText);

                Assert.IsTrue(dataGridViewColumns[failureMechanismNameColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismCodeColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismGroupColumnIndex].ReadOnly);
            }
        }
    }
}