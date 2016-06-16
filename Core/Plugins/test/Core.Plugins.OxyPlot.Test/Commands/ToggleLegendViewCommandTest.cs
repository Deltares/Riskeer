using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Commands
{
    [TestFixture]
    public class ToggleLegendViewCommandTest
    {
        [Test]
        public void Constructor_Always_CreatesICommand()
        {
            // Call
            var command = new ToggleLegendViewCommand(null);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
        }

        [Test]
        public void Enabled_Always_ReturnsTrue()
        {
            // Setup
            var command = new ToggleLegendViewCommand(null);

            // Call & Assert
            Assert.IsTrue(command.Enabled);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Checked_LegendViewOpenOrClosed_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();
            plugin.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new ChartLegendController(plugin);
            var command = new ToggleLegendViewCommand(controller);

            // Call
            var result = command.Checked;

            // Assert
            Assert.AreEqual(open, result);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_Always_TogglesLegend()
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();

            // Open first
            using (mocks.Ordered())
            {
                plugin.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(false);
                plugin.Expect(p => p.OpenToolView(Arg<ChartLegendView>.Matches(v => true)));

                // Then close
                plugin.Expect(p => p.IsToolWindowOpen<ChartLegendView>()).Return(true);
                plugin.Expect(p => p.CloseToolView(Arg<ChartLegendView>.Matches(v => true)));
            }
            mocks.ReplayAll();

            var controller = new ChartLegendController(plugin);
            var command = new ToggleLegendViewCommand(controller);

            // Call
            command.Execute();
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}