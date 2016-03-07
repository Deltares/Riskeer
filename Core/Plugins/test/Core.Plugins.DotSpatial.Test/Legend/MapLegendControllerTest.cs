using System;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
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
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(null, contextMenuBuilderProvider, parentWindow);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("toolViewController", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutContextMenuBuilderProvider_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var parentWindow = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(toolViewController, null, parentWindow);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("contextMenuBuilderProvider", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutParentWindow_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(toolViewController, contextMenuBuilderProvider, null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentWindow", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithToolViewControllerAndContextMenuBuilderProvider_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(toolViewController, contextMenuBuilderProvider, parentWindow);

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
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();
            plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new MapLegendController(plugin, contextMenuBuilderProvider, parentWindow);

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
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

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

            var controller = new MapLegendController(plugin, contextMenuBuilderProvider, parentWindow);

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