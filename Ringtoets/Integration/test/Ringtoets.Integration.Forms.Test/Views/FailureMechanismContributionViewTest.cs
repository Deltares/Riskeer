using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
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

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository();
            var random = new Random(21);
            var norm = random.Next(0, 200000);
            var otherContribution = random.Next(0,100);
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            distribution = new FailureMechanismContribution(new[] { failureMechanism }, otherContribution, norm);
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

            // Call
            var result = distributionView.Controls.Find("normInput", true)[0].Text;

            // Assert
            Assert.AreEqual(distribution.Norm.ToString(), result);
        }

        [Test]
        public void ShowFormWithControl()
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

            WindowsFormsTestHelper.ShowModal(distributionView);
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

            var f = new Form();
            f.Controls.Add(distributionView);
            f.Show();

            var normTester = new ControlTester("normInput");

            // Precondition
            Assert.AreEqual(distribution.Norm.ToString(), normTester.Text);

            // Call
            normTester.Properties.Text = 200.ToString();

            // Assert
            Assert.AreEqual(200, distribution.Norm);

            mockRepository.VerifyAll();
        }
    }
}