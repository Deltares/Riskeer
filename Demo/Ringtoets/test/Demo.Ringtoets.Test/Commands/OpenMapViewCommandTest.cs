using Core.Common.Controls.Commands;
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
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.Stub<IDocumentViewController>();
            mocks.ReplayAll();

            // Call
            var command = new OpenMapViewCommand(documentViewController);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsTrue(command.Enabled);
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();;
        }

        [Test]
        public void Execute_Always_OpensViewForMapData()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewControllerMock = mocks.StrictMock<IDocumentViewController>();
            var viewResolverMock = mocks.StrictMock<IViewResolver>();
            documentViewControllerMock.Expect(g => g.DocumentViewsResolver).Return(viewResolverMock);
            viewResolverMock.Expect(vr => vr.OpenViewForData(Arg<MapData>.Matches(md => md.IsValid()), Arg<bool>.Matches(b => b == false))).Return(true);

            mocks.ReplayAll();

            var command = new OpenMapViewCommand(documentViewControllerMock);

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Enabled_Always_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.Stub<IDocumentViewController>();
            mocks.ReplayAll();

            // Call
            var command = new OpenMapViewCommand(documentViewController);

            // Assert
            Assert.IsTrue(command.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void Checked_Always_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.Stub<IDocumentViewController>();
            mocks.ReplayAll();

            // Call
            var command = new OpenMapViewCommand(documentViewController);

            // Assert
            Assert.IsFalse(command.Checked);
            mocks.VerifyAll();
        }
    }
}