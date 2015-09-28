using System;

using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;

using Mono.Addins;

using NUnit.Framework;

using GuiPluginResources = Wti.Plugin.Properties.Resources;

namespace Wti.Plugin.Test
{
    [TestFixture]
    public class WtiGuiPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var wtiGuiPlugin = new WtiGuiPlugin();

            // assert
            Assert.IsInstanceOf<GuiPlugin>(wtiGuiPlugin);
            Assert.AreEqual(GuiPluginResources.WtiApplicationName, wtiGuiPlugin.Name);
            Assert.AreEqual(GuiPluginResources.wtiGuiPluginDisplayName, wtiGuiPlugin.DisplayName);
            Assert.AreEqual(GuiPluginResources.wtiGuiPluginDescription, wtiGuiPlugin.Description);
            Assert.AreEqual("0.5.0.0", wtiGuiPlugin.Version);
        }

        [Test]
        public void WtiApplicationPlugin_ShouldBeDeltaShellPlugin()
        {
            // call
            var attribute = Attribute.GetCustomAttribute(typeof(WtiApplicationPlugin), typeof(ExtensionAttribute));

            // assert
            Assert.IsInstanceOf<ExtensionAttribute>(attribute);
            var extensionAttribute = (ExtensionAttribute)attribute;
            Assert.AreEqual(typeof(IPlugin), extensionAttribute.Type);
        }
    }
}