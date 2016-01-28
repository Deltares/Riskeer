using Core.Common.Gui;
using Core.Components.DotSpatial.Data;
using Demo.Ringtoets.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Ringtoets.Test.Commands
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
        public void Execute_Always_OpensViewForMapData()
        {
            // Setup
            var mocks = new MockRepository();
            var guiMock = mocks.StrictMock<IGui>();
            var viewResolverMock = mocks.StrictMock<IViewResolver>();
            guiMock.Expect(g => g.DocumentViewsResolver).Return(viewResolverMock);
            viewResolverMock.Expect(vr => vr.OpenViewForData(Arg<MapData>.Matches(md => md.IsValid()), Arg<bool>.Matches(b => b == false))).Return(true);

            mocks.ReplayAll();

            var command = new OpenMapViewCommand
            {
                Gui = guiMock
            };

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