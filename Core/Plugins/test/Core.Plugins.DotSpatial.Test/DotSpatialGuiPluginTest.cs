using System.Linq;
using Core.Common.Gui;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Forms;
using NUnit.Framework;
using Ringtoets.Demo;

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
        public void Activate_Always_InitializesRibbon()
        {
            // Setup
            using (var plugin = new DotSpatialGuiPlugin())
            {
                // Call
                plugin.Activate();
                var commandHandler = plugin.RibbonCommandHandler;

                // Assert
                Assert.NotNull(commandHandler);
                Assert.IsInstanceOf<MapRibbon>(commandHandler);
            }
        }

        [Test]
        public void GetViewInfoObjects_Always_ReturnsMapDataViewInfo()
        {
             // Setup
            using (var plugin = new DotSpatialGuiPlugin())
            {
                var view = new MapDataView();

                // Call
                var views = plugin.GetViewInfoObjects().ToArray();

                // Assert
                Assert.AreEqual(1, views.Length);
                Assert.AreEqual(typeof(MapData), views[0].DataType);
                Assert.AreEqual(typeof(MapDataView), views[0].ViewType);
                Assert.AreEqual("Kaart", views[0].GetViewName(view, null));
            }
        }
    }
}