﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui.Commands;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test
{
    [TestFixture]
    public class FailureMechanismContributionViewIntegrationTest
    {
        private const string dataGridViewControlName = "dataGridView";
        private const string assessmentSectionCompositionLabelName = "assessmentSectionCompositionLabel";
        private const int isRelevantColumnIndex = 0;
        private const int nameColumnIndex = 1;
        private const int codeColumnIndex = 2;
        private const int contributionColumnIndex = 3;
        private const int probabilitySpaceColumnIndex = 4;

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void GivenViewWithAssessmentSection_WhenChangingCompositionAndNotify_ThenUpdateAssessmentSectionContributionAndView(
            AssessmentSectionComposition initialComposition,
            AssessmentSectionComposition newComposition)
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(initialComposition);

            using (var form = new Form())
            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                form.Controls.Add(view);
                form.Show();

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                var dataGridInvalidated = false;
                var contributionGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidated = true;

                // When
                assessmentSection.ChangeComposition(newComposition);
                assessmentSection.NotifyObservers();

                // Then
                var compositionLabel = (Label) new ControlTester(assessmentSectionCompositionLabelName).TheObject;

                string compositionDisplayName = new EnumDisplayWrapper<AssessmentSectionComposition>(newComposition).DisplayName;
                string newCompositionValue = $"Trajecttype: {compositionDisplayName}";
                Assert.AreEqual(newCompositionValue, compositionLabel.Text);

                Assert.IsTrue(dataGridInvalidated,
                              "Expect the DataGridView to be flagged for redrawing.");
                AssertDataGridViewDataSource(assessmentSection.GetContributingFailureMechanisms(), contributionGridView, assessmentSection.FailureMechanismContribution.Norm);
            }

            mocks.VerifyAll();
        }

        private static void AssertDataGridViewDataSource(IEnumerable<IFailureMechanism> expectedElements, DataGridView dataGridView, double norm)
        {
            IFailureMechanism[] itemArray = expectedElements.ToArray();
            Assert.AreEqual(itemArray.Length, dataGridView.RowCount);
            for (var i = 0; i < itemArray.Length; i++)
            {
                IFailureMechanism expectedElement = itemArray[i];
                DataGridViewRow row = dataGridView.Rows[i];
                Assert.AreEqual(expectedElement.IsRelevant, row.Cells[isRelevantColumnIndex].Value);
                Assert.AreEqual(expectedElement.Name, row.Cells[nameColumnIndex].Value);
                Assert.AreEqual(expectedElement.Code, row.Cells[codeColumnIndex].Value);
                Assert.AreEqual(expectedElement.Contribution, row.Cells[contributionColumnIndex].Value);
                Assert.AreEqual(100 / (expectedElement.Contribution * norm), row.Cells[probabilitySpaceColumnIndex].Value);
            }
        }
    }
}