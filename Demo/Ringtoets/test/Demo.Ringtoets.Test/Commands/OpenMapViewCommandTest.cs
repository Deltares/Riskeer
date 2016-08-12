using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
using Demo.Ringtoets.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Ringtoets.Test.Commands
{
    [TestFixture]
    public class OpenMapViewCommandTest
    {
        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.Stub<IViewController>();
            mocks.ReplayAll();

            // Call
            var command = new OpenMapViewCommand(viewController);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_Always_OpensViewForMapData()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            viewController.Expect(g => g.DocumentViewController).Return(documentViewController);
            documentViewController.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            var command = new OpenMapViewCommand(viewController);

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

            // Call
            var command = new OpenMapViewCommand(viewController);

            // Assert
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }
    }
}