using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionViewTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void NormTextBox_Initialize_TextSetToData()
        {
            // Setup
            var norm = 3000;

            var distribution = new FailureMechanismContribution(norm);
            var distributionView = new FailureMechanismContributionView
            {
                Data = distribution
            };

            // Call
            var result = distributionView.Controls.Find("normInput", true)[0].Text;

            // Assert
            Assert.AreEqual(norm.ToString(), result);
        }

        [Test]
        public void ShowFormWithControl()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var norm = 3000;

            var distribution = new FailureMechanismContribution(norm);
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

            var norm = 3000;

            var distribution = new FailureMechanismContribution(norm);
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
            Assert.AreEqual(norm.ToString(), normTester.Text);

            // Call
            normTester.Properties.Text = 200.ToString();

            // Assert
            Assert.AreEqual(200, distribution.Norm);

            mockRepository.VerifyAll();
        }
    }
}