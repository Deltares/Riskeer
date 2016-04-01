using System;
using System.Windows.Forms;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionViewTest
    {
        private const string normInputTextBoxName = "normInput";
        private const string dataGridViewControlName = "probabilityDistributionGrid";

        private MockRepository mockRepository;
        private FailureMechanismContribution distribution;
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository();
            var random = new Random(21);
            var norm = random.Next(1, 200000);
            var otherContribution = random.Next(1, 100);
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            distribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution, norm);

            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void NormTextBox_Initialize_TextSetToData()
        {
            // Setup
            mockRepository.ReplayAll();

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = distribution
            })
            {
                ShowFormWithView(distributionView);
                var normTester = new ControlTester(normInputTextBoxName);

                // Assert
                Assert.AreEqual(distribution.Norm.ToString(), normTester.Text);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void NormTextBox_ValueChanged_UpdatesDataWithNewValue()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            distribution.Attach(observerMock);
            using (var distributionView = new FailureMechanismContributionView
            {
                Data = distribution
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
            object aValue = 100;
            object expectedValue = 200;
            var random = new Random(21);

            var someMechanism = mockRepository.Stub<IFailureMechanism>();

            var failureMechanism = mockRepository.StrictMock<FailureMechanismContribution>(new[]
            {
                someMechanism
            }, random.Next(0, 100), aValue);
            var newFailureMechanism = mockRepository.StrictMock<FailureMechanismContribution>(new[]
            {
                someMechanism
            }, random.Next(0, 100), expectedValue);

            mockRepository.ReplayAll();

            using (var distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanism
            })
            {
                ShowFormWithView(distributionView);
                var normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                Assert.AreEqual(aValue.ToString(), normTester.Properties.Text);

                // Call
                distributionView.Data = newFailureMechanism;

                // Assert
                Assert.AreEqual(expectedValue.ToString(), normTester.Properties.Text);

                // Call
                failureMechanism.NotifyObservers();

                // Assert
                Assert.AreEqual(failureMechanism.Norm, aValue);
                Assert.AreEqual(newFailureMechanism.Norm, expectedValue);
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
                var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
                failureMechanismStub.Stub(fm => fm.Name).Return("A");
                failureMechanismStub.Contribution = 0;
                mockRepository.ReplayAll();

                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100, 500);

                view.Data = contributionData;
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[2];
                Assert.AreEqual("n.v.t", probabilitySpaceCell.FormattedValue);
            }
            mockRepository.VerifyAll();
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

                var failureMechanismStub = mockRepository.Stub<IFailureMechanism>();
                failureMechanismStub.Stub(fm => fm.Name).Return("A");
                failureMechanismStub.Contribution = contribution;
                mockRepository.ReplayAll();

                var contributionData = new FailureMechanismContribution(new[]
                {
                    failureMechanismStub
                }, 100.0 - contribution, norm);

                view.Data = contributionData;
                ShowFormWithView(view);

                // Then
                var dataGridView = (DataGridView)new ControlTester(dataGridViewControlName).TheObject;

                DataGridViewRow zeroContributionFailureMechanismRow = dataGridView.Rows[0];
                DataGridViewCell probabilitySpaceCell = zeroContributionFailureMechanismRow.Cells[2];

                string expectedTextValue = new FailureMechanismContributionItem(failureMechanismStub, norm)
                    .ProbabilitySpace.ToString(probabilitySpaceCell.InheritedStyle.Format, probabilitySpaceCell.InheritedStyle.FormatProvider);
                Assert.AreEqual(expectedTextValue, probabilitySpaceCell.FormattedValue);
            }
            mockRepository.VerifyAll();
        }

        private void ShowFormWithView(FailureMechanismContributionView distributionView)
        {
            testForm.Controls.Add(distributionView);
            testForm.Show();
        }
    }
}