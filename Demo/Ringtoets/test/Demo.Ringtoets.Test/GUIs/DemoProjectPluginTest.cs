using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Demo.Ringtoets.GUIs;
using NUnit.Framework;

namespace Demo.Ringtoets.Test.GUIs
{
    [TestFixture]
    public class DemoProjectPluginTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var plugin = new DemoProjectPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsInstanceOf<IRibbonCommandHandler>(plugin.RibbonCommandHandler);
            }
        }
    }
}