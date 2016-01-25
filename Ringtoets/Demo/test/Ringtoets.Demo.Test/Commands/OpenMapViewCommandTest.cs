using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Demo.Commands;

namespace Ringtoets.Demo.Test.Commands
{
    [TestFixture]
    public class OpenMapViewCommandTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var command = new OpenMapViewCommand();

            // Assert
            Assert.IsInstanceOf<IGuiCommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            Assert.IsNull(command.Gui);
        }

        [Test]
        public void Execute_Always_OpensViewForStringCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var guiMock = mocks.StrictMock<IGui>();
            var viewResolverMock = mocks.StrictMock<IViewResolver>();
            guiMock.Expect(g => g.DocumentViewsResolver).Return(viewResolverMock);
            viewResolverMock.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            var command = new OpenMapViewCommand();
            command.Gui = guiMock;

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Enabled_Always_ReturnsTrue()
        {
            // Call
            var command = new OpenMapViewCommand();

            // Assert
            Assert.IsTrue(command.Enabled);
        }

        [Test]
        public void Checked_Always_ReturnsFalse()
        {
            // Call
            var command = new OpenMapViewCommand();

            // Assert
            Assert.IsFalse(command.Checked);
        }
    }
}