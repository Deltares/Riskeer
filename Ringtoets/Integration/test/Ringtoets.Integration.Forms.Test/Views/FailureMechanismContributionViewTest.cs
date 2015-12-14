using System;
using System.Windows.Forms;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionViewTest
    {
        private MockRepository mockRepository;
        private FailureMechanismContribution distribution;
        private Form testForm;
        private ControlTester normTester;

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

            var distributionView = new FailureMechanismContributionView
            {
                Data = distribution
            };

            ShowFormWithView(distributionView);

            // Assert
            Assert.AreEqual(distribution.Norm.ToString(), normTester.Text);

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
            var distributionView = new FailureMechanismContributionView
            {
                Data = distribution
            };

            ShowFormWithView(distributionView);

            // Precondition
            Assert.AreEqual(distribution.Norm.ToString(), normTester.Text);

            // Call
            normTester.Properties.Text = 200.ToString();

            // Assert
            Assert.AreEqual(200, distribution.Norm);

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

            var distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanism
            };

            ShowFormWithView(distributionView);

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

            mockRepository.VerifyAll();
        }

        private void ShowFormWithView(FailureMechanismContributionView distributionView)
        {
            testForm.Controls.Add(distributionView);
            testForm.Show();

            normTester = new ControlTester("normInput");
        }
    }
}