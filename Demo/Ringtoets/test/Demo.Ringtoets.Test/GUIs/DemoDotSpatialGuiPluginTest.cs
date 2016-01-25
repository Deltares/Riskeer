using Core.Common.Gui;
using Demo.Ringtoets.GUIs;
using Demo.Ringtoets.Ribbons;
using NUnit.Framework;

namespace Demo.Ringtoets.Test.GUIs
{
    [TestFixture]
    public class DemoDotSpatialGuiPluginTest
    {
        [Test]
        public void DefaultConstructor_Always_NoRibbonCommandHandlerSet()
        {
            // Call
            using (var plugin = new DemoDotSpatialGuiPlugin())
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
            using (var plugin = new DemoDotSpatialGuiPlugin())
            {
                // Call
                plugin.Activate();
                var commandHandler = plugin.RibbonCommandHandler;

                // Assert
                Assert.NotNull(commandHandler);
                Assert.IsInstanceOf<MapRibbon>(commandHandler);
            }
        }
    }
}