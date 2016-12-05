// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Gui.Commands;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionViewTest : NUnitFormTest
    {
        private const string returnPeriodInputTextBoxName = "returnPeriodInput";
        private const string dataGridViewControlName = "dataGridView";
        private const string assessmentSectionCompositionComboBoxName = "assessmentSectionCompositionComboBox";
        private const int isRelevantColumnIndex = 0;
        private const int nameColumnIndex = 1;
        private const int codeColumnIndex = 2;
        private const int contributionColumnIndex = 3;
        private const int probabilitySpaceColumnIndex = 4;
        private Form testForm;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        [TearDown]
        public override void TearDown()
        {
            testForm.Dispose();

            base.TearDown();
        }

        [Test]
        public void Constructor_NormChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var compositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionView(null, compositionChangeHandler, viewCommands);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("normChangeHandler", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CompositionChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionView(normChangeHandler, null, viewCommands);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("compositionChangeHandler", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ViewCommandsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var normChangeHandler = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var compositionChangeHandler = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionView(normChangeHandler, compositionChangeHandler, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("viewCommands", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SetsDefaults()
        {
            // Setup
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            using (var contributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                ShowFormWithView(contributionView);

                var dataGridView = (DataGridViewControl) new ControlTester("probabilityDistributionGrid").TheObject;
                var tableLayout = (TableLayoutPanel) new ControlTester("tableLayoutPanel").TheObject;

                // Assert
                Assert.AreEqual(new Size(775, 350), dataGridView.MinimumSize);
                Assert.AreEqual(DockStyle.Fill, dataGridView.Dock);
                Assert.IsFalse(dataGridView.AutoScroll);

                Assert.AreEqual(new Size(0, 0), tableLayout.MinimumSize);
                Assert.AreEqual(DockStyle.Fill, tableLayout.Dock);
                Assert.IsTrue(tableLayout.AutoScroll);

                Assert.IsFalse(contributionView.AutoScroll);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ReturnPeriodTextBox_Initialize_TextSetToData()
        {
            // Setup
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            // Call
            using (var contributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(contributionView);

                // Assert
                var returnPeriodTester = new ControlTester(returnPeriodInputTextBoxName);
                var returnPeriodControl = returnPeriodTester.TheObject as NumericUpDown;

                int returnPeriod = Convert.ToInt32(1.0/failureMechanismContribution.Norm);

                Assert.NotNull(returnPeriodControl);
                Assert.AreEqual(returnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Text);
                Assert.AreEqual(1000000, returnPeriodControl.Maximum);
                Assert.AreEqual(100, returnPeriodControl.Minimum);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ReturnPeriodTextBox_ValueChangedAndUserConfirmsChange_UpdatesDataWithNewValue()
        {
            // Setup
            const int returnPeriod = 200;
            const double norm = 1.0/returnPeriod;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            int initialReturnPeriod = Convert.ToInt32(1.0/failureMechanismContribution.Norm);

            MockRepository mockRepository = new MockRepository();
            var observable1 = mockRepository.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mockRepository.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var handler1 = mockRepository.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            handler1.Expect(h => h.ConfirmNormChange()).Return(true);
            handler1.Expect(h => h.ChangeNorm(assessmentSection, norm))
                    .Return(new[]
                    {
                        observable1,
                        observable2
                    });
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                ControlTester returnPeriodTester = new ControlTester(returnPeriodInputTextBoxName);

                // Precondition
                Assert.AreEqual(initialReturnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Text);

                // Call
                SimulateUserCommittingReturnPeriodValue(returnPeriodTester, returnPeriod);
            }
            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ReturnPeriodTextBox_ValueChangedAndUserDisallowsChange_NothingHappens()
        {
            // Setup
            const int newReturnPeriod = 200;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            int initialReturnPeriod = Convert.ToInt32(1.0/failureMechanismContribution.Norm);

            MockRepository mockRepository = new MockRepository();
            var handler1 = mockRepository.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            handler1.Expect(h => h.ConfirmNormChange()).Return(false);
            handler1.Expect(h => h.ChangeNorm(assessmentSection, newReturnPeriod))
                    .Return(Enumerable.Empty<IObservable>())
                    .Repeat.Never();
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                ControlTester returnPeriodTester = new ControlTester(returnPeriodInputTextBoxName);

                // Precondition
                Assert.AreEqual(initialReturnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Text);

                // Call
                SimulateUserCommittingReturnPeriodValue(returnPeriodTester, newReturnPeriod);

                // Assert
                Assert.AreEqual(initialReturnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Properties.Text);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_Always_CorrectHeaders()
        {
            // Setup
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands))
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
        public void Data_SetToSomeContribution_ShowsColumnsWithData()
        {
            // Setup
            var random = new Random(21);
            var otherContribution = random.Next(0, 100);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            const string testName = "testName";
            const string testCode = "testCode";
            double testContribution = 100 - otherContribution;

            var mockRepository = new MockRepository();
            var handler1 = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();

            var someMechanism = mockRepository.StrictMock<FailureMechanismBase>(testName, testCode);
            someMechanism.Contribution = testContribution;

            mockRepository.ReplayAll();

            var initialContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, otherContribution, 0.01);

            using (var distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);

                // Call
                distributionView.Data = initialContribution;

                // Assert
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow row = dataGridView.Rows[0];
                var nameCell = (DataGridViewTextBoxCell) row.Cells[nameColumnIndex];
                Assert.AreEqual(testName, nameCell.Value);

                var codeCell = (DataGridViewTextBoxCell) row.Cells[codeColumnIndex];
                Assert.AreEqual(testCode, codeCell.Value);

                var contributionCell = (DataGridViewTextBoxCell) row.Cells[contributionColumnIndex];
                Assert.AreEqual(testContribution, contributionCell.Value);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetNewData_DetachesFromOldData()
        {
            // Setup
            const int initialReturnPeriod = 100;
            const int newReturnPeriod = 200;
            var random = new Random(21);

            var assessmentSection1 = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSection2 = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var handler1 = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var initialContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), 1.0/initialReturnPeriod);
            var newContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), 1.0/newReturnPeriod);

            using (var distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = initialContribution,
                AssessmentSection = assessmentSection1
            })
            {
                ShowFormWithView(distributionView);
                var returnPeriodTester = new ControlTester(returnPeriodInputTextBoxName);

                // Precondition
                Assert.AreEqual(initialReturnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Properties.Text);

                // Call
                distributionView.Data = newContribution;
                distributionView.AssessmentSection = assessmentSection2;

                // Assert
                Assert.AreEqual(newReturnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Properties.Text);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateObserver_ChangeReturnPeriodAndNotify_UpdateReturnPeriodTextBox()
        {
            // Setup
            const int initialReturnPeriod = 100;
            const int newReturnPeriod = 200;
            var random = new Random(21);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var handler1 = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var contribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), 1.0/initialReturnPeriod);

            using (var distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                var returnPeriodTester = new ControlTester(returnPeriodInputTextBoxName);

                // Precondition
                Assert.AreEqual(initialReturnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Properties.Text);

                // Call
                contribution.Norm = 1.0/newReturnPeriod;
                contribution.NotifyObservers();

                // Assert
                Assert.AreEqual(newReturnPeriod.ToString(CultureInfo.CurrentCulture), returnPeriodTester.Properties.Text);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFailureMechanismContributionView_WhenSettingData_ProperlyInitializeRelevancyColumn(bool isFailureMechanismRelevant)
        {
            // Given
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var handler1 = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
            failureMechanismStub.Stub(fm => fm.Code).Return("C");
            failureMechanismStub.Contribution = 100;
            failureMechanismStub.IsRelevant = isFailureMechanismRelevant;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100, 1.0/500);

                view.Data = contributionData;
                view.AssessmentSection = assessmentSection;
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow row = dataGridView.Rows[0];
                var isRelevantGridCell = (DataGridViewCheckBoxCell) row.Cells[isRelevantColumnIndex];
                Assert.AreEqual(isFailureMechanismRelevant, isRelevantGridCell.Value);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismContributionView_WhenSettingDataWithZeroContributionFailureMechanism_ProbabilitySpaceShowsAsNotApplicable()
        {
            // Given
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var handler1 = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
            failureMechanismStub.Stub(fm => fm.Code).Return("C");
            failureMechanismStub.Contribution = 0;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100, 1.0/500);

                view.Data = contributionData;
                view.AssessmentSection = assessmentSection;
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[probabilitySpaceColumnIndex];
                Assert.AreEqual("n.v.t", probabilitySpaceCell.FormattedValue);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismContributionView_WhenSettingDataWithNormalContributionFailureMechanism_ProbabilitySpaceShowsAsLocalisedText()
        {
            // Given
            const double contribution = 25.0;
            const double norm = 1.0/500;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var handler1 = mockRepository.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mockRepository.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
            failureMechanismStub.Stub(fm => fm.Code).Return("C");
            failureMechanismStub.Contribution = contribution;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100.0 - contribution, norm);

                view.Data = contributionData;
                view.AssessmentSection = assessmentSection;
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[probabilitySpaceColumnIndex];
                Assert.AreEqual("1/#,#", probabilitySpaceCell.InheritedStyle.Format);

                string expectedTextValue = new FailureMechanismContributionItem(failureMechanismStub, norm)
                    .ProbabilitySpace.ToString(probabilitySpaceCell.InheritedStyle.Format, probabilitySpaceCell.InheritedStyle.FormatProvider);
                Assert.AreEqual(expectedTextValue, probabilitySpaceCell.FormattedValue);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, "Dijk")]
        [TestCase(AssessmentSectionComposition.Dune, "Duin")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, "Dijk / Duin")]
        public void CompositionComboBox_WithDataSet_SelectedDisplayTextAndValueCorrect(AssessmentSectionComposition composition, string expectedDisplayText)
        {
            // Setup
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                ShowFormWithView(view);

                var assessmentSection = new AssessmentSection(composition);

                view.Data = assessmentSection.FailureMechanismContribution;
                view.AssessmentSection = assessmentSection;

                // Call
                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Assert
                Assert.AreEqual(expectedDisplayText, compositionComboBox.Text);
                Assert.AreEqual(composition, compositionComboBox.SelectedValue);
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
        public void CompositionComboBox_ChangeCompositionAndOk_ChangeCompositionAndNotifyAffectedObjects(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(initialComposition);

            var mocks = new MockRepository();
            var observable1 = mocks.StrictMock<IObservable>();
            observable1.Expect(o => o.NotifyObservers());
            var observable2 = mocks.StrictMock<IObservable>();
            observable2.Expect(o => o.NotifyObservers());

            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.StrictMock<IAssessmentSectionCompositionChangeHandler>();
            handler2.Expect(h => h.ConfirmCompositionChange())
                    .Return(true);
            handler2.Expect(h => h.ChangeComposition(assessmentSection, newComposition))
                    .Return(new[]
                    {
                        observable1,
                        observable2
                    });
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                ControlsTestHelper.FakeUserSelectingNewValue(compositionComboBox, newComposition);

                // Assert
                Assert.AreEqual(newComposition, compositionComboBox.SelectedValue);
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
        public void CompositionComboBox_ChangeCompositionAndCancel_ComboBoxStillAtOriginalValue(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(initialComposition);

            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.StrictMock<IAssessmentSectionCompositionChangeHandler>();
            handler2.Expect(h => h.ConfirmCompositionChange())
                    .Return(false);
            handler2.Expect(h => h.ChangeComposition(null, AssessmentSectionComposition.Dike))
                    .IgnoreArguments()
                    .Return(new IObservable[0])
                    .Repeat.Never();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                ControlsTestHelper.FakeUserSelectingNewValue(compositionComboBox, newComposition);

                // Assert
                Assert.AreEqual(initialComposition, compositionComboBox.SelectedValue,
                                "The ComboBox should be reset to the original composition value, as change was not accepted by user.");
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenSettingRelevantFailureMechanism_RowIsStylesAsEnabled()
        {
            // Given
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = true;
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                ShowFormWithView(view);

                var failureMechanisms = new[]
                {
                    failureMechanism
                };

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 1.0/30000);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (i == isRelevantColumnIndex)
                    {
                        continue;
                    }

                    DataGridViewCell cell = row.Cells[i];
                    AssertIsCellStyledAsEnabled(cell);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenSettingFailureMechanismThatIsIrrelevant_RowIsStylesAsGreyedOut()
        {
            // Given
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = false;
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                ShowFormWithView(view);

                var failureMechanisms = new[]
                {
                    failureMechanism
                };

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 1.0/30000);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (i == isRelevantColumnIndex)
                    {
                        continue;
                    }

                    DataGridViewCell cell = row.Cells[i];
                    AssertIsCellStyleGreyedOut(cell);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenView_IsRelevantPropertyChangeNotified_RowStylesUpdates(bool initialIsRelevant)
        {
            // Given
            List<IObserver> failureMechanismObservers = new List<IObserver>();
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.IsRelevant = initialIsRelevant;
            failureMechanism.Stub(fm => fm.Attach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation => { failureMechanismObservers.Add((IObserver) invocation.Arguments[0]); });
            failureMechanism.Stub(fm => fm.NotifyObservers())
                            .WhenCalled(invocation => { failureMechanismObservers[0].UpdateObserver(); });
            failureMechanism.Stub(fm => fm.Detach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation => { failureMechanismObservers.Remove((IObserver) invocation.Arguments[0]); });

            var failureMechanisms = new[]
            {
                failureMechanism
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                ShowFormWithView(view);

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 1.0/30000);

                view.Data = contribution;
                view.AssessmentSection = assessmentSection;

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (i != isRelevantColumnIndex)
                    {
                        DataGridViewCell cell = row.Cells[i];
                        if (failureMechanism.IsRelevant)
                        {
                            AssertIsCellStyledAsEnabled(cell);
                        }
                        else
                        {
                            AssertIsCellStyleGreyedOut(cell);
                        }
                    }
                }

                // When
                failureMechanism.IsRelevant = !initialIsRelevant;
                failureMechanism.NotifyObservers();

                // Then
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (i != isRelevantColumnIndex)
                    {
                        DataGridViewCell cell = row.Cells[i];
                        if (failureMechanism.IsRelevant)
                        {
                            AssertIsCellStyledAsEnabled(cell);
                        }
                        else
                        {
                            AssertIsCellStyleGreyedOut(cell);
                        }
                    }
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenMakingFailureMechanismIrrelevant_UpdateFailureMechanismAndNotifyObserversAndCloseRelatedViews()
        {
            // Given
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("b");
            failureMechanism.IsRelevant = true;
            failureMechanism.Expect(fm => fm.NotifyObservers());
            var viewCommands = mocks.Stub<IViewCommands>();
            viewCommands.Expect(c => c.RemoveAllViewsForItem(failureMechanism));
            mocks.ReplayAll();

            var failureMechanisms = new[]
            {
                failureMechanism
            };
            var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 1.0/30000);

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                ShowFormWithView(view);

                view.Data = contribution;

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
        public void GivenView_WhenSettingFailureMechanismThatIsAlwaysRelevant_IsRelevantFlagTrueAndReadonly()
        {
            // Given
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                ShowFormWithView(view);

                var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 1.0/30000);

                // Precondition:
                FailureMechanismContributionItem[] contributionItems = contribution.Distribution.ToArray();
                Assert.AreEqual(1, contributionItems.Length);
                Assert.IsTrue(contributionItems[0].IsAlwaysRelevant);
                Assert.IsTrue(contributionItems[0].IsRelevant);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];
                DataGridViewCell isRelevantCell = row.Cells[isRelevantColumnIndex];
                Assert.IsTrue((bool) isRelevantCell.Value);
                Assert.IsTrue(isRelevantCell.ReadOnly);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenEscapeAfterEnteringDifferentNormNotCommited_RevertNormAndNoChangedToData()
        {
            // Given
            var mocks = new MockRepository();
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            handler1.Stub(h => h.ChangeNorm(null, 1))
                    .IgnoreArguments()
                    .Return(Enumerable.Empty<IObservable>());
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            const int returnPeriod = 200;
            int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(view);
                ControlTester normTester = new ControlTester(returnPeriodInputTextBoxName);

                // When
                var normInput = (NumericUpDown) normTester.TheObject;
                view.ActiveControl = normInput;
                normInput.Value = returnPeriod;
                var keyEventArgs = new KeyEventArgs(Keys.Escape);
                EventHelper.RaiseEvent(normInput.Controls.OfType<TextBox>().First(), "KeyDown", keyEventArgs);

                // Then
                Assert.IsTrue(keyEventArgs.Handled);
                Assert.IsTrue(keyEventArgs.SuppressKeyPress);

                Assert.AreEqual(originalReturnPeriodValue, normInput.Value);
                Assert.AreEqual(originalReturnPeriodValue, normInput.Value);
                Assert.AreNotSame(normInput, view.ActiveControl);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenEnterAfterEnteringDifferentReturnPeriodNotCommitted_CommitValueAndChangeData()
        {
            // Given
            const int returnPeriod = 200;
            const double norm = 1.0/returnPeriod;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mocks = new MockRepository();
            var handler1 = mocks.StrictMock<IFailureMechanismContributionNormChangeHandler>();
            handler1.Expect(h => h.ConfirmNormChange())
                    .Return(true);
            handler1.Expect(h => h.ChangeNorm(assessmentSection, norm))
                    .Return(Enumerable.Empty<IObservable>());
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            handler2.Stub(h => h.ConfirmCompositionChange())
                    .Return(false);
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(view);
                ControlTester normTester = new ControlTester(returnPeriodInputTextBoxName);

                // When
                var returnPeriodInput = (NumericUpDown) normTester.TheObject;
                view.ActiveControl = returnPeriodInput;
                returnPeriodInput.Value = returnPeriod;
                var keyEventArgs = new KeyEventArgs(Keys.Enter);
                EventHelper.RaiseEvent(returnPeriodInput.Controls.OfType<TextBox>().First(), "KeyDown", keyEventArgs);

                // Then
                Assert.IsTrue(keyEventArgs.Handled);
                Assert.IsTrue(keyEventArgs.SuppressKeyPress);

                Assert.AreEqual(returnPeriod, returnPeriodInput.Value);
                Assert.AreNotSame(returnPeriodInput, view.ActiveControl);
            }
            mocks.VerifyAll();
        }

        private static void SimulateUserCommittingReturnPeriodValue(ControlTester returnPeriodTester, int returnPeriod)
        {
            var returnPeriodInput = (NumericUpDown) returnPeriodTester.TheObject;
            returnPeriodInput.Value = returnPeriod;
            var eventArgs = new CancelEventArgs();
            EventHelper.RaiseEvent(returnPeriodTester.TheObject, "Validating", eventArgs);
            if (!eventArgs.Cancel)
            {
                returnPeriodTester.FireEvent("Validated");
            }
        }

        private void ShowFormWithView(FailureMechanismContributionView distributionView)
        {
            testForm.Controls.Add(distributionView);
            testForm.Show();
        }

        private static void AssertIsCellStyledAsEnabled(DataGridViewCell cell)
        {
            var enabledBackColor = Color.FromKnownColor(KnownColor.White);
            var enabledForeColor = Color.FromKnownColor(KnownColor.ControlText);

            Assert.IsTrue(cell.ReadOnly);
            Assert.AreEqual(enabledBackColor, cell.Style.BackColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
            Assert.AreEqual(enabledForeColor, cell.Style.ForeColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
        }

        private static void AssertIsCellStyleGreyedOut(DataGridViewCell cell)
        {
            var irrelevantMechanismBackColor = Color.FromKnownColor(KnownColor.DarkGray);
            var irrelevantMechanismForeColor = Color.FromKnownColor(KnownColor.GrayText);

            Assert.IsTrue(cell.ReadOnly);
            Assert.AreEqual(irrelevantMechanismBackColor, cell.Style.BackColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
            Assert.AreEqual(irrelevantMechanismForeColor, cell.Style.ForeColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
        }
    }
}