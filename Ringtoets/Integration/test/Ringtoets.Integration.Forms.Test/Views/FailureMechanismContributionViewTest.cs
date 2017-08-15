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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Gui.Commands;
using Core.Common.Utils;
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
        private const string returnPeriodLabelName = "returnPeriodLabel";
        private const string dataGridViewControlName = "dataGridView";
        private const string assessmentSectionConfigurationLabelName = "assessmentSectionCompositionLabel";
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
        public void Constructor_ViewCommandsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionView(null);

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
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            using (var contributionView = new FailureMechanismContributionView(viewCommands))
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
            using (var contributionView = new FailureMechanismContributionView(viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
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
        public void Data_Always_CorrectHeaders()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var distributionView = new FailureMechanismContributionView(viewCommands))
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
            int otherContribution = random.Next(0, 100);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            const string testName = "testName";
            const string testCode = "testCode";
            double testContribution = 100 - otherContribution;

            var mockRepository = new MockRepository();
            var viewCommands = mockRepository.Stub<IViewCommands>();

            var someMechanism = mockRepository.StrictMock<FailureMechanismBase>(testName, testCode);
            someMechanism.Contribution = testContribution;

            mockRepository.ReplayAll();

            var initialContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, otherContribution);

            using (var distributionView = new FailureMechanismContributionView(viewCommands)
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
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var initialContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100))
            {
                LowerLimitNorm = 1.0 / initialReturnPeriod
            };

            var newContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100))
            {
                LowerLimitNorm = 1.0 / newReturnPeriod
            };

            using (var distributionView = new FailureMechanismContributionView(viewCommands)
            {
                Data = initialContribution,
                AssessmentSection = assessmentSection1
            })
            {
                ShowFormWithView(distributionView);
                var returnPeriodLabel = new ControlTester(returnPeriodLabelName);

                // Precondition
                string initialReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {initialReturnPeriod.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(initialReturnPeriodLabelText, returnPeriodLabel.Properties.Text);

                // Call
                distributionView.Data = newContribution;
                distributionView.AssessmentSection = assessmentSection2;

                // Assert
                string newReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {newReturnPeriod.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(newReturnPeriodLabelText, returnPeriodLabel.Properties.Text);
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
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var contribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100))
            {
                LowerLimitNorm = 1.0 / initialReturnPeriod
            };

            using (var distributionView = new FailureMechanismContributionView(viewCommands)
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                var returnPeriodLabel = new ControlTester(returnPeriodLabelName);

                // Precondition
                string initialReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {initialReturnPeriod.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(initialReturnPeriodLabelText, returnPeriodLabel.Properties.Text);

                // Call
                contribution.LowerLimitNorm = 1.0 / newReturnPeriod;
                contribution.NotifyObservers();

                // Assert
                string newReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {newReturnPeriod.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(newReturnPeriodLabelText, returnPeriodLabel.Properties.Text);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateObserver_ChangeNormativeNormAndNotify_UpdateReturnPeriodTextBox()
        {
            // Setup
            const int lowerLimitNorm = 100;
            const int signalingNorm = 1000;

            var random = new Random(21);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var contribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100))
            {
                LowerLimitNorm = 1.0 / lowerLimitNorm,
                SignalingNorm = 1.0 / signalingNorm
            };

            using (var distributionView = new FailureMechanismContributionView(viewCommands)
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                var returnPeriodLabel = new ControlTester(returnPeriodLabelName);

                // Precondition
                string initialReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {lowerLimitNorm.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(initialReturnPeriodLabelText, returnPeriodLabel.Properties.Text);

                // Call
                contribution.NormativeNorm = NormType.Signaling;
                contribution.NotifyObservers();

                // Assert
                string newReturnPeriodLabelText = $"Norm van het dijktraject: 1 / {signalingNorm.ToString(CultureInfo.CurrentCulture)}";
                Assert.AreEqual(newReturnPeriodLabelText, returnPeriodLabel.Properties.Text);
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
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Contribution = 100;
            failureMechanism.IsRelevant = isFailureMechanismRelevant;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanism
                }, 100);

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
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Contribution = 0;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanism
                }, 100);

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
            const double norm = 1.0 / 30000;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.Contribution = contribution;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanism
                }, 100.0 - contribution);

                view.Data = contributionData;
                view.AssessmentSection = assessmentSection;
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
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                ShowFormWithView(view);

                var assessmentSection = new AssessmentSection(composition);

                view.Data = assessmentSection.FailureMechanismContribution;
                view.AssessmentSection = assessmentSection;

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

            using (var view = new FailureMechanismContributionView(viewCommands)
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                // Call
                assessmentSection.ChangeComposition(newComposition);
                assessmentSection.FailureMechanismContribution.NotifyObservers();

                // Assert
                var compositionLabel = (Label) new ControlTester(assessmentSectionConfigurationLabelName).TheObject;

                string compositionDisplayName = new EnumDisplayWrapper<AssessmentSectionComposition>(newComposition).DisplayName;
                string newCompositionValue = $"Trajecttype: {compositionDisplayName}";
                Assert.AreEqual(newCompositionValue, compositionLabel.Text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenSettingRelevantFailureMechanism_RowIsStylesAsEnabled()
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = true;
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                ShowFormWithView(view);

                var failureMechanisms = new[]
                {
                    failureMechanism
                };

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];

                for (var i = 0; i < row.Cells.Count; i++)
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
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = false;
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                ShowFormWithView(view);

                var failureMechanisms = new[]
                {
                    failureMechanism
                };

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];

                for (var i = 0; i < row.Cells.Count; i++)
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
            var failureMechanismObservers = new List<IObserver>();
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.IsRelevant = initialIsRelevant;
            failureMechanism.Stub(fm => fm.Attach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation => failureMechanismObservers.Add((IObserver) invocation.Arguments[0]));
            failureMechanism.Stub(fm => fm.NotifyObservers())
                            .WhenCalled(invocation => failureMechanismObservers[0].UpdateObserver());
            failureMechanism.Stub(fm => fm.Detach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation => failureMechanismObservers.Remove((IObserver) invocation.Arguments[0]));

            var failureMechanisms = new[]
            {
                failureMechanism
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                ShowFormWithView(view);

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0);

                view.Data = contribution;
                view.AssessmentSection = assessmentSection;

                var dataGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];

                for (var i = 0; i < row.Cells.Count; i++)
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
                for (var i = 0; i < row.Cells.Count; i++)
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
            var contribution = new FailureMechanismContribution(failureMechanisms, 50.0);

            using (var view = new FailureMechanismContributionView(viewCommands))
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
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(viewCommands))
            {
                ShowFormWithView(view);

                IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();
                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0);

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