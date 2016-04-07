using System;
using System.Windows.Forms;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionViewTest : NUnitFormTest
    {
        private const string normInputTextBoxName = "normInput";
        private const string dataGridViewControlName = "probabilityDistributionGrid";
        private const string assessmentSectionCompositionComboBoxName = "assessmentSectionCompositionComboBox";

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
            mockRepository.ReplayAll();

            var contribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution, norm);

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);

            using (var contributionView = new FailureMechanismContributionView
            {
                Data = context
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

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var distribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution, norm);
            distribution.Attach(observerMock);

            var context = new FailureMechanismContributionContext(distribution, assessmentSection);

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = context
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
            var assessmentSection2 = mockRepository.Stub<IAssessmentSection>();

            mockRepository.ReplayAll();

            var initialContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), aValue);
            var newContribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), expectedValue);

            var initialContext = new FailureMechanismContributionContext(initialContribution, assessmentSection1);
            var newContext = new FailureMechanismContributionContext(newContribution, assessmentSection2);

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = initialContext
            })
            {
                ShowFormWithView(distributionView);
                var normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                Assert.AreEqual(aValue.ToString(), normTester.Properties.Text);

                // Call
                distributionView.Data = newContext;

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
            var assessmentSection1 = mockRepository.Stub<IAssessmentSection>();

            mockRepository.ReplayAll();

            var contribution = new FailureMechanismContribution(new[]
            {
                someMechanism
            }, random.Next(0, 100), initialValue);

            var context = new FailureMechanismContributionContext(contribution, assessmentSection1);

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = context
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
        public void GivenFailureMechanismContributionView_WhenSettingDataWithZeroContributionFailureMechanism_ProbabilitySpaceShowsAsNotApplicable()
        {
            // Given
            using (var view = new FailureMechanismContributionView())
            {
                // When
                var mockRepository = new MockRepository();
                var assessmentSection = mockRepository.Stub<IAssessmentSection>();

                var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
                failureMechanismStub.Stub(fm => fm.Name).Return("A");
                failureMechanismStub.Contribution = 0;
                mockRepository.ReplayAll();

                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100, 500);
                var context = new FailureMechanismContributionContext(contributionData, assessmentSection);

                view.Data = context;
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[2];
                Assert.AreEqual("n.v.t", probabilitySpaceCell.FormattedValue);

                mockRepository.VerifyAll();
            }
        }

        [Test]
        public void GivenFailureMechanismContributionView_WhenSettingDataWithNormalContributionFailureMechanism_ProbabilitySpaceShowsAsLocalisedText()
        {
            // Given
            using (var view = new FailureMechanismContributionView())
            {
                // When
                const double contribution = 25.0;
                const int norm = 500;

                var mockRepository = new MockRepository();
                var assessmentSection = mockRepository.Stub<IAssessmentSection>();

                var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
                failureMechanismStub.Stub(fm => fm.Name).Return("A");
                failureMechanismStub.Contribution = contribution;
                mockRepository.ReplayAll();

                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100.0 - contribution, norm);
                var context = new FailureMechanismContributionContext(contributionData, assessmentSection);

                view.Data = context;
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[2];
                Assert.AreEqual("1/#,#" , probabilitySpaceCell.InheritedStyle.Format);

                string expectedTextValue = new FailureMechanismContributionItem(failureMechanismStub, norm)
                    .ProbabilitySpace.ToString(probabilitySpaceCell.InheritedStyle.Format, probabilitySpaceCell.InheritedStyle.FormatProvider);
                Assert.AreEqual(expectedTextValue, probabilitySpaceCell.FormattedValue);

                mockRepository.VerifyAll();
            }
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

                var context = new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection);
                view.Data = context;

                // Call
                var compositionComboBox = (ComboBox)new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

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

                var context = new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection);

                view.Data = context;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                var compositionComboBox = (ComboBox)new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                int dataGridInvalidatedCallCount = 0;
                var contributionGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
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
                string expectedText = "Het veranderen van het traject type zorgt ervoor dat de uitvoer van alle berekeningen wordt gewist." + Environment.NewLine +
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

                var context = new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection);

                view.Data = context;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                int dataGridInvalidatedCallCount = 0;
                var contributionGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidatedCallCount++;

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

                var compositionComboBox = (ComboBox)new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                Assert.AreEqual(newComposition, compositionComboBox.SelectedValue);
                Assert.AreEqual(newComposition, assessmentSection.Composition);

                Assert.AreEqual(1, dataGridInvalidatedCallCount);
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.Distribution, contributionGridView.DataSource);
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

                var context = new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection);

                view.Data = context;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

                var compositionComboBox = (ComboBox)new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

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
            using (var view = new FailureMechanismContributionView())
            {
                var firstMockRepository = new MockRepository();

                var calculationItem1 = firstMockRepository.Stub<ICalculationItem>();
                calculationItem1.Expect(ci => ci.ClearOutput());
                calculationItem1.Expect(ci => ci.NotifyObservers());
                var calculationItem2 = firstMockRepository.Stub<ICalculationItem>();
                calculationItem2.Expect(ci => ci.ClearOutput());
                calculationItem2.Expect(ci => ci.NotifyObservers());

                var failureMechanism1 = firstMockRepository.Stub<IFailureMechanism>();
                failureMechanism1.Stub(fm => fm.CalculationItems).Return(new[]
                {
                    calculationItem1
                });
                failureMechanism1.Stub(fm => fm.Name).Return("A");
                var failureMechanism2 = firstMockRepository.Stub<IFailureMechanism>();
                failureMechanism2.Stub(fm => fm.CalculationItems).Return(new[]
                {
                    calculationItem2
                });
                failureMechanism2.Stub(fm => fm.Name).Return("B");

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
                assessmentSection.Expect(section => section.ChangeComposition(newComposition));
                secondMockRepository.ReplayAll();

                var context = new FailureMechanismContributionContext(failureMechanismContribution, assessmentSection);

                view.Data = context;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                int dataGridInvalidatedCallCount = 0;
                var contributionGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidatedCallCount++;

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

                var compositionComboBox = (ComboBox)new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // Call
                compositionComboBox.SelectedValue = newComposition;

                // Assert
                Assert.AreEqual(newComposition, compositionComboBox.SelectedValue);

                Assert.AreEqual(1, dataGridInvalidatedCallCount);

                firstMockRepository.VerifyAll(); // Expect ICalculationItem.ClearOutput and ICalculationItem.NotifyObservers
                secondMockRepository.VerifyAll();
            }
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
            using (var view = new FailureMechanismContributionView())
            {
                var firstMockRepository = new MockRepository();

                var calculationItem1 = firstMockRepository.Stub<ICalculationItem>();
                calculationItem1.Expect(ci => ci.ClearOutput()).Repeat.Never();
                var calculationItem2 = firstMockRepository.Stub<ICalculationItem>();
                calculationItem2.Expect(ci => ci.ClearOutput()).Repeat.Never();

                var failureMechanism = firstMockRepository.Stub<IFailureMechanism>();
                failureMechanism.Stub(fm => fm.CalculationItems).Return(new[]
                {
                    calculationItem1,
                    calculationItem2
                });
                failureMechanism.Stub(fm => fm.Name).Return("A");

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

                var context = new FailureMechanismContributionContext(failureMechanismContribution, assessmentSection);

                view.Data = context;
                ShowFormWithView(view);

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                int dataGridInvalidatedCallCount = 0;
                var contributionGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidatedCallCount++;

                var compositionComboBox = (ComboBox)new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

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

                secondMockRepository.VerifyAll();
            }
        }

        private void ShowFormWithView(FailureMechanismContributionView distributionView)
        {
            testForm.Controls.Add(distributionView);
            testForm.Show();
        }
    }
}