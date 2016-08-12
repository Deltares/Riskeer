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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui.Commands;
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
        private Form testForm;

        private const string normInputTextBoxName = "normInput";
        private const string dataGridViewControlName = "dataGridView";
        private const string assessmentSectionCompositionComboBoxName = "assessmentSectionCompositionComboBox";
        private const int isRelevantColumnIndex = 0;
        private const int nameColumnIndex = 1;
        private const int codeColumnIndex = 2;
        private const int contributionColumnIndex = 3;
        private const int probabilitySpaceColumnIndex = 4;

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
        public void NormTextBox_Initialize_TextSetToData()
        {
            // Setup
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            // Call
            using (var contributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(contributionView);

                // Assert
                var normTester = new ControlTester(normInputTextBoxName);
                var normControl = normTester.TheObject as NumericUpDown;

                Assert.NotNull(normControl);
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);
                Assert.AreEqual(300000, normControl.Maximum);
                Assert.AreEqual(100, normControl.Minimum);
            }
        }

        [Test]
        public void NormTextBox_ValueChanged_UpdatesDataWithNewValue()
        {
            // Setup
            const int normValue = 200;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);

            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                ControlTester normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);

                // Call
                normTester.Properties.Text = normValue.ToString();

                // Assert
                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_Always_CorrectHeaders()
        {
            // Setup
            using (var distributionView = new FailureMechanismContributionView())
            {
                // Call
                ShowFormWithView(distributionView);

                // Assert
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;

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
        }

        [Test]
        public void Data_SetToSomeContribution_ShowsColumnsWithData()
        {
            // Setup
            var random = new Random(21);
            var otherContribution = random.Next(0, 100);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var testName = "testName";
            var testCode = "testCode";
            double testContribution = 100 - otherContribution;

            var mockRepository = new MockRepository();
            var someMechanism = mockRepository.StrictMock<FailureMechanismBase>(testName, testCode);
            someMechanism.Contribution = testContribution;

            mockRepository.ReplayAll();

            var initialContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, otherContribution, 100);

            using (var distributionView = new FailureMechanismContributionView
            {
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);

                // Call
                distributionView.Data = initialContribution;

                // Assert
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow row = dataGridView.Rows[0];
                var nameCell = (DataGridViewTextBoxCell)row.Cells[nameColumnIndex];
                Assert.AreEqual(testName, nameCell.Value);

                var codeCell = (DataGridViewTextBoxCell)row.Cells[codeColumnIndex];
                Assert.AreEqual(testCode, codeCell.Value);

                var contributionCell = (DataGridViewTextBoxCell)row.Cells[contributionColumnIndex];
                Assert.AreEqual(testContribution, contributionCell.Value);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetNewData_DetachesFromOldData()
        {
            // Setup
            var aValue = 100;
            var expectedValue = 200;
            var random = new Random(21);

            var assessmentSection1 = new AssessmentSection(AssessmentSectionComposition.Dike);
            var assessmentSection2 = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();
            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var initialContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), aValue);
            var newContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), expectedValue);

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = initialContribution,
                AssessmentSection = assessmentSection1
            })
            {
                ShowFormWithView(distributionView);
                var normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                Assert.AreEqual(aValue.ToString(), normTester.Properties.Text);

                // Call
                distributionView.Data = newContribution;
                distributionView.AssessmentSection = assessmentSection2;

                // Assert
                Assert.AreEqual(expectedValue.ToString(), normTester.Properties.Text);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateObserver_ChangeNormAndNotify_UpdateNormTextBox()
        {
            // Setup
            const int initialValue = 100;
            const int expectedValue = 200;
            var random = new Random(21);
            
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            
            var mockRepository = new MockRepository();
            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var contribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), initialValue);

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                var normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                Assert.AreEqual(initialValue.ToString(), normTester.Properties.Text);

                // Call
                contribution.Norm = expectedValue;
                contribution.NotifyObservers();

                // Assert
                Assert.AreEqual(expectedValue.ToString(), normTester.Properties.Text);
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
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
            failureMechanismStub.Stub(fm => fm.Code).Return("C");
            failureMechanismStub.Contribution = 100;
            failureMechanismStub.IsRelevant = isFailureMechanismRelevant;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView())
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100, 500);

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
            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
            failureMechanismStub.Stub(fm => fm.Code).Return("C");
            failureMechanismStub.Contribution = 0;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView())
            {
                // When
                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100, 500);

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
            const int norm = 500;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mockRepository = new MockRepository();

            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
            failureMechanismStub.Stub(fm => fm.Code).Return("C");
            failureMechanismStub.Contribution = contribution;
            mockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView())
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
            using (var view = new FailureMechanismContributionView())
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
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void CompositionComboBox_ChangeCompositionAndOk_UpdateAssessmentSectionContributionAndView(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            using (var view = new FailureMechanismContributionView())
            {
                var assessmentSection = new AssessmentSection(initialComposition);

                view.Data = assessmentSection.FailureMechanismContribution;
                view.AssessmentSection = assessmentSection;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                bool dataGridInvalidated = false;
                var contributionGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidated = true;

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                Assert.AreEqual(newComposition, compositionComboBox.SelectedValue);
                Assert.AreEqual(newComposition, assessmentSection.Composition);

                Assert.IsTrue(dataGridInvalidated,
                              "Expect the DataGridView to be flagged for redrawing.");
                AssertDataGridViewDataSource(assessmentSection.FailureMechanismContribution.Distribution, contributionGridView);
            }
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void CompositionComboBox_ChangeComposition_NotifyAssessmentSectionObservers(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            using (var view = new FailureMechanismContributionView())
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                var assessmentSection = new AssessmentSection(initialComposition);
                assessmentSection.Attach(observer);

                view.Data = assessmentSection.FailureMechanismContribution;
                view.AssessmentSection = assessmentSection;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                mocks.VerifyAll(); // Expect UpdateObserver call
            }
        }

        [Test]
        public void GivenView_WhenSettingRelevantFailureMechanism_RowIsStylesAsEnabled()
        {
            // Given
            using (var view = new FailureMechanismContributionView())
            {
                ShowFormWithView(view);

                var mocks = new MockRepository();
                var failureMechanism = mocks.Stub<IFailureMechanism>();
                failureMechanism.IsRelevant = true;
                mocks.ReplayAll();

                var failureMechanisms = new[]
                {
                    failureMechanism
                };

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 30000);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
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
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenView_WhenSettingFailureMechanismThatIsIrrelevant_RowIsStylesAsGreyedOut()
        {
            // Given
            using (var view = new FailureMechanismContributionView())
            {
                ShowFormWithView(view);

                var mocks = new MockRepository();
                var failureMechanism = mocks.Stub<IFailureMechanism>();
                failureMechanism.IsRelevant = false;
                mocks.ReplayAll();

                var failureMechanisms = new[]
                {
                    failureMechanism
                };

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 30000);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
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
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenView_IsRelevantPropertyChangeNotified_RowStylesUpdates(bool initialIsRelevant)
        {
            // Given
            List<IObserver> failureMechanismObservers = new List<IObserver>();
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.IsRelevant = initialIsRelevant;
            failureMechanism.Stub(fm => fm.Attach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObservers.Add((IObserver)invocation.Arguments[0]);
                            });
            failureMechanism.Stub(fm => fm.NotifyObservers())
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObservers[0].UpdateObserver();
                            });
            failureMechanism.Stub(fm => fm.Detach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObservers.Remove((IObserver)invocation.Arguments[0]);
                            });

            var failureMechanisms = new[]
                {
                    failureMechanism
                };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView())
            {
                ShowFormWithView(view);

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 30000);

                view.Data = contribution;
                view.AssessmentSection = assessmentSection;

                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
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
        [TestCase(true)]
        [TestCase(false)]
        public void GivenViewWithViewCommands_IsRelevantPropertyChangeNotified_CloseViewsForIrrelevantFailureMechanisms(bool initialIsRelevant)
        {
            // Given
            List<IObserver> failureMechanismObservers = new List<IObserver>();
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Code).Return("C");
            failureMechanism.IsRelevant = initialIsRelevant;
            failureMechanism.Stub(fm => fm.Attach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObservers.Add((IObserver) invocation.Arguments[0]);
                            });
            failureMechanism.Stub(fm => fm.NotifyObservers())
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObservers[1].UpdateObserver();
                            });
            failureMechanism.Stub(fm => fm.Detach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObservers.Remove((IObserver)invocation.Arguments[0]);
                            });

            var relevantFailureMechanism = mocks.Stub<IFailureMechanism>();
            relevantFailureMechanism.Stub(fm => fm.Name).Return("B");
            relevantFailureMechanism.Stub(fm => fm.Code).Return("C");
            relevantFailureMechanism.IsRelevant = true;
            relevantFailureMechanism.Stub(fm => fm.Attach(null))
                            .IgnoreArguments();
            relevantFailureMechanism.Stub(fm => fm.Detach(null))
                            .IgnoreArguments();

            var irrelevantFailureMechanism = mocks.Stub<IFailureMechanism>();
            irrelevantFailureMechanism.Stub(fm => fm.Name).Return("C");
            irrelevantFailureMechanism.Stub(fm => fm.Code).Return("C");
            irrelevantFailureMechanism.IsRelevant = false;
            irrelevantFailureMechanism.Stub(fm => fm.Attach(null))
                            .IgnoreArguments();
            irrelevantFailureMechanism.Stub(fm => fm.Detach(null))
                            .IgnoreArguments();

            var failureMechanisms = new[]
                {
                    failureMechanism,
                    relevantFailureMechanism,
                    irrelevantFailureMechanism
                };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);

            IViewCommands viewCommandsStub = mocks.Stub<IViewCommands>();
            if (initialIsRelevant)
            {
                viewCommandsStub.Expect(vc => vc.RemoveAllViewsForItem(failureMechanism));
            }
            viewCommandsStub.Expect(vc => vc.RemoveAllViewsForItem(relevantFailureMechanism)).Repeat.Never();
            viewCommandsStub.Expect(vc => vc.RemoveAllViewsForItem(irrelevantFailureMechanism));
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView
            {
                ViewCommands = viewCommandsStub
            })
            {
                ShowFormWithView(view);

                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 30000);

                view.Data = contribution;
                view.AssessmentSection = assessmentSection;

                // When
                failureMechanism.IsRelevant = !initialIsRelevant;
                failureMechanism.NotifyObservers();
            }
            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenView_WhenSettingFailureMechanismThatIsAlwaysRelevant_IsRelevantFlagTrueAndReadonly()
        {
            // Given
            using (var view = new FailureMechanismContributionView())
            {
                ShowFormWithView(view);

                var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
                var contribution = new FailureMechanismContribution(failureMechanisms, 50.0, 30000);

                // Precondition:
                FailureMechanismContributionItem[] contributionItems = contribution.Distribution.ToArray();
                Assert.AreEqual(1, contributionItems.Length);
                Assert.IsTrue(contributionItems[0].IsAlwaysRelevant);
                Assert.IsTrue(contributionItems[0].IsRelevant);

                // When
                view.Data = contribution;

                // Then
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
                DataGridViewRow row = dataGridView.Rows[0];
                DataGridViewCell isRelevantCell = row.Cells[isRelevantColumnIndex];
                Assert.IsTrue((bool)isRelevantCell.Value);
                Assert.IsTrue(isRelevantCell.ReadOnly);
            }
        }

        private void AssertDataGridViewDataSource(IEnumerable<FailureMechanismContributionItem> expectedDistributionElements, DataGridView dataGridView)
        {
            FailureMechanismContributionItem[] itemArray = expectedDistributionElements.ToArray();
            Assert.AreEqual(itemArray.Length, dataGridView.RowCount);
            for (int i = 0; i < itemArray.Length; i++)
            {
                FailureMechanismContributionItem expectedElement = itemArray[i];
                DataGridViewRow row = dataGridView.Rows[i];
                Assert.AreEqual(expectedElement.IsRelevant, row.Cells[isRelevantColumnIndex].Value);
                Assert.AreEqual(expectedElement.Assessment, row.Cells[nameColumnIndex].Value);
                Assert.AreEqual(expectedElement.AssessmentCode, row.Cells[codeColumnIndex].Value);
                Assert.AreEqual(expectedElement.Contribution, row.Cells[contributionColumnIndex].Value);
                Assert.AreEqual(expectedElement.ProbabilitySpace, row.Cells[probabilitySpaceColumnIndex].Value);
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