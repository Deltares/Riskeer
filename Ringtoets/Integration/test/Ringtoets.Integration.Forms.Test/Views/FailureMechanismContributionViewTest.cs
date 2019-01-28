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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionViewTest : NUnitFormTest
    {
        private const string returnPeriodLabelName = "returnPeriodLabel";
        private const string dataGridViewControlName = "dataGridView";
        private const string assessmentSectionConfigurationLabelName = "assessmentSectionCompositionLabel";
        private const int isRelevantColumnIndex = 0;
        private const int nameColumnIndex = 1;
        private const int codeColumnIndex = 2;
        private const int contributionColumnIndex = 3;
        private const int probabilitySpaceColumnIndex = 4;
        private Form testForm;

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionView(null, viewCommands);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionView(assessmentSection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("viewCommands", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            // Call
            using (var contributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(contributionView);

                var dataGridView = (DataGridViewControl) new ControlTester("probabilityDistributionGrid").TheObject;
                var groupBoxView = (GroupBox) new ControlTester("groupBoxAssessmentSectionDetails").TheObject;

                // Assert
                Assert.IsInstanceOf<IView>(contributionView);
                Assert.IsInstanceOf<UserControl>(contributionView);

                Assert.AreEqual(new Size(0, 0), dataGridView.MinimumSize);
                Assert.AreEqual(DockStyle.Fill, dataGridView.Dock);
                Assert.IsFalse(dataGridView.AutoScroll);

                Assert.AreEqual(new Size(0, 0), groupBoxView.MinimumSize);
                Assert.AreEqual(DockStyle.Top, groupBoxView.Dock);

                Assert.IsFalse(contributionView.AutoScroll);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_SetReturnPeriodTextBox()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            // Call
            using (var contributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(contributionView);

                // Assert
                var returnPeriodLabel = new ControlTester(returnPeriodLabelName);

                int returnPeriod = Convert.ToInt32(1.0 / failureMechanismContribution.Norm);
                string expectedReturnPeriodLabel = $"Norm van het dijktraject: 1 / {returnPeriod.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(expectedReturnPeriodLabel, returnPeriodLabel.Properties.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_CorrectHeaders()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            using (var distributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                // Call
                ShowFormWithView(distributionView);

                // Assert
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                string isRelevantColumnHeaderText = dataGridView.Columns[isRelevantColumnIndex].HeaderText;
                Assert.AreEqual("Is relevant", isRelevantColumnHeaderText);

                string nameColumnHeaderText = dataGridView.Columns[nameColumnIndex].HeaderText;
                Assert.AreEqual("Toetsspoor", nameColumnHeaderText);

                string codeColumnHeaderText = dataGridView.Columns[codeColumnIndex].HeaderText;
                Assert.AreEqual("Label", codeColumnHeaderText);

                string contributionColumnHeaderText = dataGridView.Columns[contributionColumnIndex].HeaderText;
                Assert.AreEqual("Toegestane bijdrage aan faalkans [%]", contributionColumnHeaderText);

                string probabilitySpaceColumnHeaderText = dataGridView.Columns[probabilitySpaceColumnIndex].HeaderText;
                Assert.AreEqual("Faalkansruimte [1/jaar]", probabilitySpaceColumnHeaderText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_ShowsColumnsWithData()
        {
            // Setup
            var random = new Random(21);
            int otherContribution = random.Next(0, 100);

            const string testName = "testName";
            const string testCode = "testCode";
            double testContribution = 100 - otherContribution;

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<FailureMechanismBase>(testName, testCode, 1);
            failureMechanism.Contribution = testContribution;
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.Detach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.GetContributingFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            // Call
            using (var distributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(distributionView);

                // Assert
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow row = dataGridView.Rows[0];
                var nameCell = (DataGridViewTextBoxCell) row.Cells[nameColumnIndex];
                Assert.AreEqual(testName, nameCell.Value);

                var codeCell = (DataGridViewTextBoxCell) row.Cells[codeColumnIndex];
                Assert.AreEqual(testCode, codeCell.Value);

                var contributionCell = (DataGridViewTextBoxCell) row.Cells[contributionColumnIndex];
                Assert.AreEqual(testContribution, contributionCell.Value);

                var probabilitySpaceCell = (DataGridViewTextBoxCell) row.Cells[probabilitySpaceColumnIndex];
                Assert.AreEqual(100 / (assessmentSection.FailureMechanismContribution.Norm * failureMechanism.Contribution), probabilitySpaceCell.Value);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_Always_ProperlyInitializeRelevancyColumn(bool isFailureMechanismRelevant)
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Stub(section => section.Attach(null)).IgnoreArguments();
            failureMechanism.Stub(section => section.Detach(null)).IgnoreArguments();
            failureMechanism.Contribution = 100;
            failureMechanism.IsRelevant = isFailureMechanismRelevant;
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.Detach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.GetContributingFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            // When
            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow row = dataGridView.Rows[0];
                var isRelevantGridCell = (DataGridViewCheckBoxCell) row.Cells[isRelevantColumnIndex];
                Assert.AreEqual(isFailureMechanismRelevant, isRelevantGridCell.Value);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismWithZeroContribution_ProbabilitySpaceShowsAsNotApplicable()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Stub(section => section.Attach(null)).IgnoreArguments();
            failureMechanism.Stub(section => section.Detach(null)).IgnoreArguments();
            failureMechanism.Contribution = 0;
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.Detach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.GetContributingFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            // Call
            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                // Assert
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[probabilitySpaceColumnIndex];
                Assert.AreEqual("n.v.t", probabilitySpaceCell.FormattedValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismWithContribution_ProbabilitySpaceShowsAsLocalisedText()
        {
            // Setup
            const double contribution = 25.0;

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Stub(section => section.Attach(null)).IgnoreArguments();
            failureMechanism.Stub(section => section.Detach(null)).IgnoreArguments();
            failureMechanism.Contribution = contribution;
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.Detach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.GetContributingFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            assessmentSection.FailureMechanismContribution.NormativeNorm = NormType.Signaling;

            // Call
            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                // Assert
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[probabilitySpaceColumnIndex];
                Assert.AreEqual("1/#,#", probabilitySpaceCell.InheritedStyle.Format);

                double expectedProbabilitySpace = 100 / (assessmentSection.FailureMechanismContribution.Norm * failureMechanism.Contribution);
                string expectedTextValue = expectedProbabilitySpace.ToString(probabilitySpaceCell.InheritedStyle.Format, probabilitySpaceCell.InheritedStyle.FormatProvider);
                Assert.AreEqual(expectedTextValue, probabilitySpaceCell.FormattedValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenFailureMechanismContributionNotified_ThenReturnPeriodTextBoxUpdated()
        {
            // Setup
            const int lowerLimitNorm = 100;
            const int signalingNorm = 1000;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike, 1.0 / lowerLimitNorm, 1.0 / signalingNorm);

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var distributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(distributionView);
                var returnPeriodLabel = new ControlTester(returnPeriodLabelName);

                // Precondition
                string initialReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {lowerLimitNorm.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(initialReturnPeriodLabelText, returnPeriodLabel.Properties.Text);

                // Call
                assessmentSection.FailureMechanismContribution.NormativeNorm = NormType.Signaling;
                assessmentSection.FailureMechanismContribution.NotifyObservers();

                // Assert
                string newReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {signalingNorm.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(newReturnPeriodLabelText, returnPeriodLabel.Properties.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismContributionView_WhenFailureMechanismContributionNotified_ThenDataGridViewInvalidated()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.Detach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.GetContributingFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                // Precondition
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                var invalidated = false;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // When
                assessmentSection.FailureMechanismContribution.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, "Dijk")]
        [TestCase(AssessmentSectionComposition.Dune, "Duin")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, "Dijk / Duin")]
        public void Constructor_Always_CompositionComboBoxSelectedDisplayTextAndValueCorrect(AssessmentSectionComposition composition, string expectedDisplayText)
        {
            // Setup
            var assessmentSection = new AssessmentSection(composition);

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                // Assert
                var compositionLabel = (Label) new ControlTester(assessmentSectionConfigurationLabelName).TheObject;
                string expectedLabelValue = $"Trajecttype: {expectedDisplayText}";
                Assert.AreEqual(expectedLabelValue, compositionLabel.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void GivenView_WhenAssessmentSectionCompositionChangedAndNotified_ThenCompositionComboBoxItemUpdated(
            AssessmentSectionComposition initialComposition,
            AssessmentSectionComposition newComposition)
        {
            // Given
            var assessmentSection = new AssessmentSection(initialComposition);

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                var compositionLabel = (Label) new ControlTester(assessmentSectionConfigurationLabelName).TheObject;

                // Precondition
                string initialCompositionDisplayName = new EnumDisplayWrapper<AssessmentSectionComposition>(initialComposition).DisplayName;
                Assert.AreEqual($"Trajecttype: {initialCompositionDisplayName}", compositionLabel.Text);

                // When
                assessmentSection.ChangeComposition(newComposition);
                assessmentSection.NotifyObservers();

                // Then
                string compositionDisplayName = new EnumDisplayWrapper<AssessmentSectionComposition>(newComposition).DisplayName;
                Assert.AreEqual($"Trajecttype: {compositionDisplayName}", compositionLabel.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenMakingFailureMechanismIrrelevant_UpdateFailureMechanismAndNotifyObserversAndCloseRelatedViews()
        {
            // Given
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("b");
            failureMechanism.Stub(fm => fm.Attach(null)).IgnoreArguments();
            failureMechanism.Stub(fm => fm.Detach(null)).IgnoreArguments();
            failureMechanism.IsRelevant = true;
            failureMechanism.Expect(fm => fm.NotifyObservers());
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.Detach(null)).IgnoreArguments();
            assessmentSection.Stub(section => section.GetContributingFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(c => c.RemoveAllViewsForItem(failureMechanism));
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];

                // When
                row.Cells[isRelevantColumnIndex].Value = false;

                // Then
                Assert.IsFalse(failureMechanism.IsRelevant);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCellFormattingStates))]
        public void GivenFailureMechanismResultView_WhenCellFormattingEventFired_ThenCellStyleSetToColumnDefinition(
            bool readOnly, CellStyle style)
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var failureMechanisms = new List<IFailureMechanism>
            {
                failureMechanism
            };
            var assessmentSection = new AssessmentSectionStub(failureMechanisms);

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                var row = (FailureMechanismContributionItemRow) dataGridView.Rows[0].DataBoundItem;
                DataGridViewColumnStateDefinition definition = row.ColumnStateDefinitions[0];
                definition.ReadOnly = readOnly;
                definition.Style = style;

                // When
                failureMechanism.NotifyObservers();

                // Then
                DataGridViewCell cell = dataGridView.Rows[0].Cells[0];
                Assert.AreEqual(readOnly, cell.ReadOnly);
                Assert.AreEqual(style.BackgroundColor, cell.Style.BackColor);
                Assert.AreEqual(style.TextColor, cell.Style.ForeColor);
            }
        }

        [Test]
        public void GivenView_WhenRowUpdatedEventFiredAndFailureMechanismNotified_ThenRowNotUpdatedAndViewInvalidated()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var failureMechanisms = new List<IFailureMechanism>
            {
                failureMechanism
            };
            var assessmentSection = new AssessmentSectionStub(failureMechanisms);

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                var row = (FailureMechanismContributionItemRow) dataGridView.Rows[0].DataBoundItem;
                var invalidated = false;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);

                // When
                row.RowUpdated?.Invoke(row, EventArgs.Empty);
                failureMechanism.NotifyObservers();
                row.RowUpdateDone?.Invoke(row, EventArgs.Empty);

                // Then
                Assert.IsTrue(invalidated);
            }
        }

        [Test]
        public void GivenView_WhenFailureMechanismNotified_ThenDataGridViewInvalidated()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var failureMechanisms = new List<IFailureMechanism>
            {
                failureMechanism
            };
            var assessmentSection = new AssessmentSectionStub(failureMechanisms);

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                var invalidated = false;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);

                // When
                failureMechanism.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
            }
        }

        [Test]
        public void GivenView_WhenFailureMechanismRemovedAndAssessmentSectionNotified_ThenEventHandlersDisconnected()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var failureMechanisms = new List<IFailureMechanism>
            {
                failureMechanism
            };
            var assessmentSection = new AssessmentSectionStub(failureMechanisms);

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                var row = (FailureMechanismContributionItemRow) dataGridView.Rows[0].DataBoundItem;

                // Precondition
                Assert.IsNotNull(row.RowUpdated);
                Assert.IsNotNull(row.RowUpdateDone);

                // When
                failureMechanisms.Remove(failureMechanism);
                assessmentSection.NotifyObservers();

                // Then
                Assert.IsNull(row.RowUpdated);
                Assert.IsNull(row.RowUpdateDone);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenAssessmentSectionNotified_ThenFailureMechanismObserversResubscribed()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism1 = new TestFailureMechanism();
            var failureMechanism2 = new TestFailureMechanism();
            var failureMechanisms = new List<IFailureMechanism>
            {
                failureMechanism1
            };
            var assessmentSection = new AssessmentSectionStub(failureMechanisms);

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                failureMechanisms.Remove(failureMechanism1);
                failureMechanisms.Add(failureMechanism2);
                assessmentSection.NotifyObservers();

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                var invalidated = false;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);

                // When
                failureMechanism1.NotifyObservers();

                // Then
                Assert.IsFalse(invalidated);
            }

            mocks.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetCellFormattingStates()
        {
            yield return new TestCaseData(true, CellStyle.Disabled);
            yield return new TestCaseData(false, CellStyle.Enabled);
        }

        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        public override void TearDown()
        {
            testForm.Dispose();

            base.TearDown();
        }

        private void ShowFormWithView(FailureMechanismContributionView distributionView)
        {
            testForm.Controls.Add(distributionView);
            testForm.Show();
        }
    }
}