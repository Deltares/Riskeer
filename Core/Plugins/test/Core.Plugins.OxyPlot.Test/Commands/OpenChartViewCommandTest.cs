using Core.Common.Gui;
using Core.Plugins.OxyPlot.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class OpenChartViewCommandTest
    {
        [Test]
        public void Execute_Always_OpensViewForIChartData()
        {
            // Setup
            var mocks = new MockRepository();
            var guiMock = mocks.StrictMock<IGui>();
            var viewResolverMock = mocks.StrictMock<IViewResolver>();
            guiMock.Expect(g => g.DocumentViewsResolver).Return(viewResolverMock);
            viewResolverMock.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            var command = new OpenChartViewCommand();
            command.Gui = guiMock;

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Enabled_Always_ReturnsTrue()
        {
            // Setup
            var command = new OpenChartViewCommand();

            // Call & Assert
            Assert.IsTrue(command.Enabled);
        }

        [Test]
        public void Checked_Always_ReturnsFalse()
        {
            // Setup
            var command = new OpenChartViewCommand();

            // Call & Assert
            Assert.IsFalse(command.Checked);
        }
    }
}