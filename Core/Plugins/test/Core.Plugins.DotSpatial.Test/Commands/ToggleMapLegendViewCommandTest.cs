using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Plugins.DotSpatial.Commands;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test.Commands
{
    [TestFixture]
    public class ToggleMapLegendViewCommandTest
    {
        [Test]
        public void Constructor_Always_CreatesICommand()
        {
            // Call
            var command = new ToggleMapLegendViewCommand(null);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
        }

        [Test]
        public void Enabled_Always_ReturnsTrue()
        {
            // Call
            var command = new ToggleMapLegendViewCommand(null);

            // Assert
            Assert.IsTrue(command.Enabled);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Checekd_LegendViewOpenOrClosed_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();
            plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new MapLegendController(plugin);
            var command = new ToggleMapLegendViewCommand(controller);

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
                plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(false);
                plugin.Expect(p => p.OpenToolView(Arg<MapLegendView>.Matches(v => true)));

                // Then close
                plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(true);
                plugin.Expect(p => p.CloseToolView(Arg<MapLegendView>.Matches(v => true)));
            }
            mocks.ReplayAll();

            var controller = new MapLegendController(plugin);
            var command = new ToggleMapLegendViewCommand(controller);

            // Call
            command.Execute();
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}
