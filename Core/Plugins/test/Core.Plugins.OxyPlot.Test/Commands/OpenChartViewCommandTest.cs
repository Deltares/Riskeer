using Core.Common.Gui;
using Core.Plugins.OxyPlot.Commands;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Commands
{
    [TestFixture]
    public class OpenChartViewCommandTest
    {
        [Test]
        public void Execute_Always_OpensViewForIChartData()
        {
            // Setup
            var mocks = new MockRepository();
            var guiMock = mocks.StrictMock<IDocumentViewController>();
            var viewResolverMock = mocks.StrictMock<IViewResolver>();
            guiMock.Expect(g => g.DocumentViewsResolver).Return(viewResolverMock);
            viewResolverMock.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            var command = new OpenChartViewCommand(guiMock);

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
            var documentViewControler = mocks.Stub<IDocumentViewController>();
            mocks.ReplayAll();

            var command = new OpenChartViewCommand(documentViewControler);

            // Call
            var isEnabled = command.Enabled;

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void Checked_Always_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.Stub<IDocumentViewController>();
            mocks.ReplayAll();

            var command = new OpenChartViewCommand(documentViewController);

            // Call
            var isChecked = command.Checked;

            // Assert
            Assert.IsFalse(isChecked);
            mocks.VerifyAll();
        }
    }
}