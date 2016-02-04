using System;
using Core.Common.Gui;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapLegendControllerTest
    {
        [Test]
        public void Constructor_WithoutToolViewController_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLegendController(null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("toolViewController", exception.ParamName);
        }

        [Test]
        public void Constructor_WithToolViewController_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(toolViewController);

            // Assert
            Assert.DoesNotThrow(test);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsLegendViewOpen_LegendViewOpenAndClosedState_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();
            plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new MapLegendController(plugin);

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
            var plugin = mocks.StrictMock<IToolViewController>();
            if (open)
            {
                plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(false);
                plugin.Expect(p => p.OpenToolView(Arg<MapLegendView>.Matches(c => true)));
                plugin.Expect(p => p.CloseToolView(Arg<MapLegendView>.Matches(c => true)));
            }
            else
            {
                plugin.Expect(p => p.OpenToolView(Arg<MapLegendView>.Matches(c => true)));
            }
            plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new MapLegendController(plugin);

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