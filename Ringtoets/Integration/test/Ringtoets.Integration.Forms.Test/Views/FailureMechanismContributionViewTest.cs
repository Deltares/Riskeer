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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
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
            var random = new Random(21);
            var norm = random.Next(1, 200000);
            var otherContribution = random.Next(1, 100);

            var mockRepository = new MockRepository();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);
            mockRepository.ReplayAll();

            var contribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution, norm);

            using (var contributionView = new FailureMechanismContributionView
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(contributionView);
                var normTester = new ControlTester(normInputTextBoxName);

                // Assert
                Assert.AreEqual(contribution.Norm.ToString(), normTester.Text);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void NormTextBox_ValueChanged_UpdatesDataWithNewValue()
        {
            // Setup
            var random = new Random(21);
            var norm = random.Next(1, 200000);
            var otherContribution = random.Next(1, 100);

            var mockRepository = new MockRepository();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var distribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution, norm);
            distribution.Attach(observerMock);

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = distribution,
                AssessmentSection = assessmentSection
            })
            {
                ShowFormWithView(distributionView);
                var normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                Assert.AreEqual(distribution.Norm.ToString(), normTester.Text);

                // Call
                normTester.Properties.Text = 200.ToString();

                // Assert
                Assert.AreEqual(200, distribution.Norm);
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

            var mockRepository = new MockRepository();

            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            var assessmentSection1 = mockRepository.Stub<IAssessmentSection>();
            assessmentSection1.Stub(section => section.GetFailureMechanisms())
                              .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection1.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.Dike);

            var assessmentSection2 = mockRepository.Stub<IAssessmentSection>();
            assessmentSection2.Stub(section => section.GetFailureMechanisms())
                              .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection2.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.Dike);

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

            var mockRepository = new MockRepository();

            var someMechanism = mockRepository.Stub<IFailureMechanism>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);

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
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);

            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
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
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);

            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
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

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);

            var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
            failureMechanismStub.Stub(fm => fm.Name).Return("A");
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
        public void CompositionComboBox_ChangeComposition_ShowMessageBoxWithExpectedText(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
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

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                int dataGridInvalidatedCallCount = 0;
                var contributionGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidatedCallCount++;

                string messageBoxTitle = null, messageBoxText = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBoxTitle = messageBox.Title;
                    messageBoxText = messageBox.Text;

                    messageBox.ClickOk();
                };

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                Assert.AreEqual("Bevestigen", messageBoxTitle);
                string expectedText = "U heeft een ander trajecttype geselecteerd. Als gevolg hiervan moet de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden." + Environment.NewLine +
                                      Environment.NewLine +
                                      "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedText, messageBoxText);
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

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

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
        public void CompositionComboBox_ChangeCompositionAndOk_NotifyAssessmentSectionObservers(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
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

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                mocks.VerifyAll(); // Expect UpdateObserver call
            }
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void CompositionComboBox_ChangeCompositionAndOk_ClearOutputAndNotify(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            var firstMockRepository = new MockRepository();

            var calculationItem1 = firstMockRepository.Stub<ICalculation>();
            calculationItem1.Expect(ci => ci.ClearOutput());
            calculationItem1.Expect(ci => ci.NotifyObservers());

            // Expect no clear output as failure mechanism doesn't have different Contribution:
            var calculationItem2 = firstMockRepository.StrictMock<ICalculation>();

            double contributionBeforeChange = 1.1, contributionAfterChange = 2.2;

            var failureMechanism1 = firstMockRepository.Stub<IFailureMechanism>();
            failureMechanism1.Stub(fm => fm.Calculations).Return(new[]
                {
                    calculationItem1
                });
            failureMechanism1.Contribution = contributionBeforeChange;
            failureMechanism1.Stub(fm => fm.Name).Return("A");
            failureMechanism1.Stub(fm => fm.Attach(null)).IgnoreArguments();
            failureMechanism1.Stub(fm => fm.Detach(null)).IgnoreArguments();

            var failureMechanism2 = firstMockRepository.Stub<IFailureMechanism>();
            failureMechanism2.Stub(fm => fm.Calculations).Return(new[]
                {
                    calculationItem2
                });
            failureMechanism2.Stub(fm => fm.Name).Return("B");
            failureMechanism2.Stub(fm => fm.Attach(null)).IgnoreArguments();
            failureMechanism2.Stub(fm => fm.Detach(null)).IgnoreArguments();

            firstMockRepository.ReplayAll();

            var failureMechanisms = new[]
                {
                    failureMechanism1,
                    failureMechanism2
                };

            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, 100.0, 30000);

            var secondMockRepository = new MockRepository();
            var assessmentSection = secondMockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(failureMechanismContribution);
            assessmentSection.Stub(section => section.Composition).Return(initialComposition);
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(section => section.NotifyObservers());
            assessmentSection.Expect(section => section.ChangeComposition(newComposition))
                             .WhenCalled(invocation => failureMechanism1.Contribution = contributionAfterChange);
            secondMockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView())
            {
                

                view.Data = failureMechanismContribution;
                view.AssessmentSection = assessmentSection;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                bool dataGridViewInvalidated = false;
                var contributionGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridViewInvalidated = true;

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                Assert.AreEqual(newComposition, compositionComboBox.SelectedValue);

                Assert.IsTrue(dataGridViewInvalidated,
                              "Expect the data grid view to be marked for redrawing.");
            }

            firstMockRepository.VerifyAll(); // Expect ICalculation.ClearOutput and ICalculation.NotifyObservers
            secondMockRepository.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void CompositionComboBox_ChangeCompositionAndCancel_KeepOriginalCompositionAndCalculationResults(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Setup
            var firstMockRepository = new MockRepository();

            var calculationItem1 = firstMockRepository.Stub<ICalculation>();
            calculationItem1.Expect(ci => ci.ClearOutput()).Repeat.Never();
            var calculationItem2 = firstMockRepository.Stub<ICalculation>();
            calculationItem2.Expect(ci => ci.ClearOutput()).Repeat.Never();

            var failureMechanism = firstMockRepository.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
                {
                    calculationItem1,
                    calculationItem2
                });
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Stub(fm => fm.Attach(null)).IgnoreArguments();
            failureMechanism.Stub(fm => fm.Detach(null)).IgnoreArguments();

            firstMockRepository.ReplayAll();

            var failureMechanisms = new[]
                {
                    failureMechanism
                };

            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, 100.0, 30000);

            var secondMockRepository = new MockRepository();
            var assessmentSection = secondMockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(failureMechanismContribution);
            assessmentSection.Stub(section => section.Composition).Return(initialComposition);
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Expect(section => section.ChangeComposition(newComposition)).Repeat.Never();
            secondMockRepository.ReplayAll();

            using (var view = new FailureMechanismContributionView())
            {
                view.Data = failureMechanismContribution;
                view.AssessmentSection = assessmentSection;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                int dataGridInvalidatedCallCount = 0;
                var contributionGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidatedCallCount++;

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickCancel();
                };

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                Assert.AreEqual(0, dataGridInvalidatedCallCount);
                Assert.AreEqual(initialComposition, compositionComboBox.SelectedValue);
            }

            firstMockRepository.VerifyAll();
            secondMockRepository.VerifyAll();
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
            IObserver failureMechanismObserver = null;
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.IsRelevant = initialIsRelevant;
            failureMechanism.Stub(fm => fm.Attach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObserver = (IObserver)invocation.Arguments[0];
                            });
            failureMechanism.Stub(fm => fm.NotifyObservers())
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObserver.UpdateObserver();
                            });
            failureMechanism.Stub(fm => fm.Detach(null))
                            .IgnoreArguments()
                            .WhenCalled(invocation =>
                            {
                                failureMechanismObserver = null;
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
                Assert.AreEqual(expectedElement.IsRelevant, row.Cells[0].Value);
                Assert.AreEqual(expectedElement.Assessment, row.Cells[1].Value);
                Assert.AreEqual(expectedElement.Contribution, row.Cells[2].Value);
                Assert.AreEqual(expectedElement.ProbabilitySpace, row.Cells[3].Value);
            }
        }

        private const string normInputTextBoxName = "normInput";
        private const string dataGridViewControlName = "probabilityDistributionGrid";
        private const string assessmentSectionCompositionComboBoxName = "assessmentSectionCompositionComboBox";
        private const int isRelevantColumnIndex = 0;
        private const int probabilitySpaceColumnIndex = 3;

        private void ShowFormWithView(FailureMechanismContributionView distributionView)
        {
            testForm.Controls.Add(distributionView);
            testForm.Show();
        }

        private static void AssertIsCellStyledAsEnabled(DataGridViewCell cell)
        {
            var enabledBackColor = Color.FromKnownColor(KnownColor.White);
            var enabledForeColor = Color.FromKnownColor(KnownColor.ControlText);

            Assert.AreEqual(enabledBackColor, cell.Style.BackColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
            Assert.AreEqual(enabledForeColor, cell.Style.ForeColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
        }

        private static void AssertIsCellStyleGreyedOut(DataGridViewCell cell)
        {
            var irrelevantMechanismBackColor = Color.FromKnownColor(KnownColor.DarkGray);
            var irrelevantMechanismForeColor = Color.FromKnownColor(KnownColor.GrayText);

            Assert.AreEqual(irrelevantMechanismBackColor, cell.Style.BackColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
            Assert.AreEqual(irrelevantMechanismForeColor, cell.Style.ForeColor,
                            "Color does not match for column index: " + cell.ColumnIndex);
        }
    }
}