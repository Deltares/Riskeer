using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Commands;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test
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
            var plugin = mocks.StrictMock<IOxyPlotGuiPlugin>();
            plugin.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new LegendController(plugin);
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
            var plugin = mocks.StrictMock<IOxyPlotGuiPlugin>();

            // Open first
            plugin.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(false);
            plugin.Expect(p => p.OpenToolView(Arg<LegendView>.Matches(v => true)));

            // Then close
            plugin.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(true);
            plugin.Expect(p => p.CloseToolView(Arg<LegendView>.Matches(v => true)));

            mocks.ReplayAll();

            var controller = new LegendController(plugin);
            var command = new ToggleLegendViewCommand(controller);

            // Call
            command.Execute();
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}