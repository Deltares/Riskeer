using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
using Demo.Ringtoets.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Ringtoets.Test.Commands
{
    [TestFixture]
    public class OpenChartViewCommandTest
    {
        [Test]
        public void Execute_Always_OpensViewForIChartData()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            viewController.Expect(g => g.DocumentViewController).Return(documentViewController);
            documentViewController.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            var command = new OpenChartViewCommand(viewController);

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Checked_Always_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.Stub<IViewController>();
            mocks.ReplayAll();

            var command = new OpenChartViewCommand(viewController);

            // Call
            var isChecked = command.Checked;

            // Assert
            Assert.IsFalse(isChecked);
            mocks.VerifyAll();
        }
    }
}