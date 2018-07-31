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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
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
using Ringtoets.Integration.Forms.Views;

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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            using (var contributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(contributionView);

                var dataGridView = (DataGridViewControl) new ControlTester("probabilityDistributionGrid").TheObject;
                var groupBoxView = (GroupBox) new ControlTester("groupBoxAssessmentSectionDetails").TheObject;

                // Assert
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
        public void ReturnPeriodTextBox_Initialize_TextSetToData()
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var distributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                // Call
                ShowFormWithView(distributionView);

                // Assert
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                string isRelevantColumnHeaderText = dataGridView.Columns[isRelevantColumnIndex].HeaderText;
                Assert.AreEqual("Algemeen filter", isRelevantColumnHeaderText);

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
            var someMechanism = mocks.StrictMock<FailureMechanismBase>(testName, testCode, 1);
            someMechanism.Contribution = testContribution;
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(someMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
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
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.Distribution.Single(d => d.FailureMechanism == someMechanism).ProbabilitySpace, probabilitySpaceCell.Value);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_ChangeReturnPeriodAndNotify_UpdateReturnPeriodTextBox()
        {
            // Setup
            const int initialReturnPeriod = 100;
            const int newReturnPeriod = 200;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike, 1.0 / initialReturnPeriod, 1.0 / 300);

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var distributionView = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(distributionView);
                var returnPeriodLabel = new ControlTester(returnPeriodLabelName);

                // Precondition
                string initialReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {initialReturnPeriod.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(initialReturnPeriodLabelText, returnPeriodLabel.Properties.Text);

                // Call
                assessmentSection.FailureMechanismContribution.LowerLimitNorm = 1.0 / newReturnPeriod;
                assessmentSection.FailureMechanismContribution.NotifyObservers();

                // Assert
                string newReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {newReturnPeriod.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(newReturnPeriodLabelText, returnPeriodLabel.Properties.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_ChangeNormativeNormAndNotify_UpdateReturnPeriodTextBox()
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
        public void GivenFailureMechanismContributionView_WhenObserverNotified_UpdatesDataSource()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
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
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFailureMechanismContributionView_WhenSettingData_ProperlyInitializeRelevancyColumn(bool isFailureMechanismRelevant)
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                // When
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
        public void GivenFailureMechanismContributionView_WhenSettingDataWithZeroContributionFailureMechanism_ProbabilitySpaceShowsAsNotApplicable()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Stub(section => section.Attach(null)).IgnoreArguments();
            failureMechanism.Stub(section => section.Detach(null)).IgnoreArguments();
            failureMechanism.Contribution = 0;
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                // When
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[probabilitySpaceColumnIndex];
                Assert.AreEqual("n.v.t", probabilitySpaceCell.FormattedValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismContributionView_WhenSettingDataWithNormalContributionFailureMechanism_ProbabilitySpaceShowsAsLocalisedText()
        {
            // Given
            const double contribution = 25.0;
            const double norm = 1.0 / 30000;

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Stub(section => section.Attach(null)).IgnoreArguments();
            failureMechanism.Stub(section => section.Detach(null)).IgnoreArguments();
            failureMechanism.Contribution = contribution;
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            assessmentSection.FailureMechanismContribution.NormativeNorm = NormType.Signaling;
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                // When
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[probabilitySpaceColumnIndex];
                Assert.AreEqual("1/#,#", probabilitySpaceCell.InheritedStyle.Format);

                string expectedTextValue = new FailureMechanismContributionItem(failureMechanism, norm)
                                           .ProbabilitySpace.ToString(probabilitySpaceCell.InheritedStyle.Format, probabilitySpaceCell.InheritedStyle.FormatProvider);
                Assert.AreEqual(expectedTextValue, probabilitySpaceCell.FormattedValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, "Dijk")]
        [TestCase(AssessmentSectionComposition.Dune, "Duin")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, "Dijk / Duin")]
        public void CompositionComboBox_WithDataSet_SelectedDisplayTextAndValueCorrect(AssessmentSectionComposition composition, string expectedDisplayText)
        {
            // Setup
            var assessmentSection = new AssessmentSection(composition);

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                ShowFormWithView(view);

                // Call
                var compositionLabel = (Label) new ControlTester(assessmentSectionConfigurationLabelName).TheObject;

                // Assert
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
        public void UpdateObserver_ChangeAssessmentSectionCompositionAndNotify_ChangeCompositionComboBoxItem(
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

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                // Call
                assessmentSection.ChangeComposition(newComposition);
                assessmentSection.NotifyObservers();

                // Assert
                var compositionLabel = (Label) new ControlTester(assessmentSectionConfigurationLabelName).TheObject;

                string compositionDisplayName = new EnumDisplayWrapper<AssessmentSectionComposition>(newComposition).DisplayName;
                string newCompositionValue = $"Trajecttype: {compositionDisplayName}";
                Assert.AreEqual(newCompositionValue, compositionLabel.Text);
            }

            mocks.VerifyAll();
        }

//        [Test]
//        public void GivenView_WhenSettingRelevantFailureMechanism_RowIsStyledAsEnabled()
//        {
//            // Given
//            var mocks = new MockRepository();
//            var viewCommands = mocks.Stub<IViewCommands>();
//            var failureMechanism = mocks.Stub<IFailureMechanism>();
//            failureMechanism.IsRelevant = true;
//            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
//            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
//            mocks.ReplayAll();
//
//            // When
//            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
//            {
//                ShowFormWithView(view);
//                
//                // Then
//                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
//                DataGridViewRow row = dataGridView.Rows[0];
//
//                for (var i = 0; i < row.Cells.Count; i++)
//                {
//                    if (i == isRelevantColumnIndex)
//                    {
//                        continue;
//                    }
//
//                    DataGridViewCell cell = row.Cells[i];
//                    AssertIsCellStyledAsEnabled(cell);
//                }
//            }
//
//            mocks.VerifyAll();
//        }

//        [Test]
//        public void GivenView_WhenSettingFailureMechanismThatIsIrrelevant_RowIsStyledAsGreyedOut()
//        {
//            // Given
//            var mocks = new MockRepository();
//            var viewCommands = mocks.Stub<IViewCommands>();
//            var failureMechanism = mocks.Stub<IFailureMechanism>();
//            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
//            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
//            mocks.ReplayAll();
//
//            failureMechanism.IsRelevant = false;
//
//            // When
//            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
//            {
//                ShowFormWithView(view);
//
//                // Then
//                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
//                DataGridViewRow row = dataGridView.Rows[0];
//
//                for (var i = 0; i < row.Cells.Count; i++)
//                {
//                    if (i == isRelevantColumnIndex)
//                    {
//                        continue;
//                    }
//
//                    DataGridViewCell cell = row.Cells[i];
//                    AssertIsCellStyleGreyedOut(cell);
//                }
//            }
//
//            mocks.VerifyAll();
//        }

//        [Test]
//        [TestCase(true)]
//        [TestCase(false)]
//        public void GivenView_IsRelevantPropertyChangeNotified_RowStylesUpdates(bool initialIsRelevant)
//        {
//            // Given
//            var mocks = new MockRepository();
//            var viewCommands = mocks.Stub<IViewCommands>();
//            var failureMechanism = mocks.Stub<IFailureMechanism>();
//            failureMechanism.Stub(fm => fm.Name).Return("A");
//            failureMechanism.Stub(fm => fm.Code).Return("C");
//            failureMechanism.IsRelevant = initialIsRelevant;
//            failureMechanism.Stub(fm => fm.Attach(null)).IgnoreArguments();
//            failureMechanism.Stub(fm => fm.Detach(null)).IgnoreArguments();
//            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
//            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
//            mocks.ReplayAll();
//
//            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
//            {
//                ShowFormWithView(view);
//
//                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
//                DataGridViewRow row = dataGridView.Rows[0];
//
//                for (var i = 0; i < row.Cells.Count; i++)
//                {
//                    if (i != isRelevantColumnIndex)
//                    {
//                        DataGridViewCell cell = row.Cells[i];
//                        if (failureMechanism.IsRelevant)
//                        {
//                            AssertIsCellStyledAsEnabled(cell);
//                        }
//                        else
//                        {
//                            AssertIsCellStyleGreyedOut(cell);
//                        }
//                    }
//                }
//
//                // When
//                failureMechanism.IsRelevant = !initialIsRelevant;
//                failureMechanism.NotifyObservers();
//
//                // Then
//                for (var i = 0; i < row.Cells.Count; i++)
//                {
//                    if (i != isRelevantColumnIndex)
//                    {
//                        DataGridViewCell cell = row.Cells[i];
//                        if (failureMechanism.IsRelevant)
//                        {
//                            AssertIsCellStyledAsEnabled(cell);
//                        }
//                        else
//                        {
//                            AssertIsCellStyleGreyedOut(cell);
//                        }
//                    }
//                }
//            }
//
//            mocks.VerifyAll();
//        }

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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            var viewCommands = mocks.Stub<IViewCommands>();
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

//        [Test]
//        public void GivenView_WhenSettingFailureMechanismThatIsAlwaysRelevant_IsRelevantFlagTrueAndReadonly()
//        {
//            // Given
//            var mocks = new MockRepository();
//            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
//            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
//            var viewCommands = mocks.Stub<IViewCommands>();
//            mocks.ReplayAll();
//
//            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
//            {
//                ShowFormWithView(view);
//
//                FailureMechanismContribution contribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();
//
//                // Precondition:
//                FailureMechanismContributionItem[] contributionItems = contribution.Distribution.ToArray();
//                Assert.AreEqual(1, contributionItems.Length);
//                Assert.IsTrue(contributionItems[0].IsAlwaysRelevant);
//                Assert.IsTrue(contributionItems[0].IsRelevant);
//
//                // When
//
//                // Then
//                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
//                DataGridViewRow row = dataGridView.Rows[0];
//                DataGridViewCell isRelevantCell = row.Cells[isRelevantColumnIndex];
//                Assert.IsTrue((bool) isRelevantCell.Value);
//                Assert.IsTrue(isRelevantCell.ReadOnly);
//            }
//
//            mocks.VerifyAll();
//        }

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

        private static void AssertIsCellStyledAsEnabled(DataGridViewCell cell)
        {
            Color enabledBackColor = Color.FromKnownColor(KnownColor.White);
            Color enabledForeColor = Color.FromKnownColor(KnownColor.ControlText);

            Assert.IsTrue(cell.ReadOnly);
            Assert.AreEqual(enabledBackColor, cell.Style.BackColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
            Assert.AreEqual(enabledForeColor, cell.Style.ForeColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
        }

        private static void AssertIsCellStyleGreyedOut(DataGridViewCell cell)
        {
            Color irrelevantMechanismBackColor = Color.FromKnownColor(KnownColor.DarkGray);
            Color irrelevantMechanismForeColor = Color.FromKnownColor(KnownColor.GrayText);

            Assert.IsTrue(cell.ReadOnly);
            Assert.AreEqual(irrelevantMechanismBackColor, cell.Style.BackColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
            Assert.AreEqual(irrelevantMechanismForeColor, cell.Style.ForeColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
        }
    }
}