using System;
using System.Linq;
using System.Windows;
using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Components.DotSpatial.Forms;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Core.Plugins.DotSpatial.Forms;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test
{
    [TestFixture]
    public class DotSpatialGuiPluginTest
    {
        [Test]
        public void DefaultConstructor_Always_NoRibbonCommandHandlerSet()
        {
            // Call
            using (var plugin = new DotSpatialGuiPlugin())
            {
                // Assert
                Assert.IsInstanceOf<GuiPlugin>(plugin);
                Assert.IsNull(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithoutGui_ThrowsArgumentNullException()
        {
            // Setup
            using (var plugin = new DotSpatialGuiPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("toolViewController", exception.ParamName);
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void Activate_WithGui_InitializeComponentsWithIMapViewAndBindsActiveViewChanged(bool useMapView)
        {
            // Setup
            var mocks = new MockRepository();

            IView view;

            if (useMapView)
            {
                var mapView = mocks.Stub<IMapView>();
                var map = mocks.Stub<IMapControl>();
                map.Data = new TestMapData("test data");
                mapView.Stub(v => v.Map).Return(map);
                view = mapView;
            }
            else
            {
                view = mocks.StrictMock<IView>();
            }

            using (var plugin = new DotSpatialGuiPlugin())
            {
                var gui = mocks.StrictMock<IGui>();

                var mainWindow = mocks.StrictMock<IMainWindow>();

                gui.Stub(g => g.IsToolWindowOpen<MapLegendView>()).Return(false);

                gui.Expect(g => g.OpenToolView(Arg<MapLegendView>.Matches(c => true)));
                gui.Expect(g => g.ActiveViewChanged += null).IgnoreArguments();
                gui.Expect(g => g.ActiveViewChanged -= null).IgnoreArguments();
                gui.Expect(g => g.ActiveView).Return(view);
                gui.Expect(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                plugin.Activate();

                // Assert
                Assert.IsInstanceOf<GuiPlugin>(plugin);
                Assert.NotNull(plugin.RibbonCommandHandler);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfoObjects_Always_ReturnsMapDataViewInfo()
        {
            // Setup
            using (var plugin = new DotSpatialGuiPlugin())
            {
                using (var view = new MapDataView())
                {
                    // Call
                    var views = plugin.GetViewInfos().ToArray();

                    // Assert
                    Assert.AreEqual(1, views.Length);
                    Assert.AreEqual(typeof(MapData), views[0].DataType);
                    Assert.AreEqual(typeof(MapDataView), views[0].ViewType);
                    Assert.AreEqual("Kaart", views[0].GetViewName(view, null));
                }
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenConfiguredGui_WhenActiveViewChangesToViewWithMap_ThenRibbonSetVisibility(bool visible)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                var plugin = new DotSpatialGuiPlugin();
                var testMapView = new TestMapView();
                var map = new MapControl();
                IView viewMock = visible ? (IView) testMapView : new TestView();

                testMapView.Data = map;

                gui.Plugins.Add(plugin);
                plugin.Gui = gui;
                gui.Run();

                // When
                gui.DocumentViews.Add(viewMock);
                gui.DocumentViews.ActiveView = viewMock;

                // Then
                Assert.AreEqual(visible ? Visibility.Visible : Visibility.Collapsed, plugin.RibbonCommandHandler.GetRibbonControl().ContextualGroups[0].Visibility);
                mocks.VerifyAll();
            }
        }
    }
}