using System;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class LegendControllerTest
    {
        [Test]
        public void Constructor_WithoutPlugin_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new LegendController(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithPlugin_DoesNotThrow()
        {
            // Setup
            using (var plugin = new OxyPlotGuiPlugin())
            {
                // Call
                TestDelegate test = () => new LegendController(plugin);

                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsLegendViewOpen_LegendViewOpenAndClosedState_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IOxyPlotGuiPlugin>();
            plugin.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new LegendController(plugin);

            // Call
            var result = controller.IsLegendViewOpen();

            // Assert
            Assert.AreEqual(open, result);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleLegendView_LegendViewOpenAndClosedState_TogglesStateOfLegendView(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IOxyPlotGuiPlugin>();
            if (open)
            {
                plugin.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(false);
                plugin.Expect(p => p.OpenToolView(Arg<LegendView>.Matches(c => true)));
                plugin.Expect(p => p.CloseToolView(Arg<LegendView>.Matches(c => true)));
            }
            else
            {
                plugin.Expect(p => p.OpenToolView(Arg<LegendView>.Matches(c => true)));
            }
            plugin.Expect(p => p.IsToolWindowOpen<LegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new LegendController(plugin);

            if (open)
            {
                controller.ToggleLegend();
            }

            // Call
            controller.ToggleLegend();

            // Assert
            mocks.VerifyAll();
        }
    }
}