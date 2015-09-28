using System;

using DelftTools.Shell.Core;

using Mono.Addins;
using Mono.Addins.Description;

using NUnit.Framework;
using PluginResources = Wti.Plugin.Properties.Resources;

namespace Wti.Plugin.Test
{
    [TestFixture]
    public class WtiApplicationPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var wtiPlugin = new WtiApplicationPlugin();

            // assert
            Assert.IsInstanceOf<ApplicationPlugin>(wtiPlugin);
            Assert.AreEqual(PluginResources.WtiApplicationName, wtiPlugin.Name);
            Assert.AreEqual(PluginResources.WtiApplicationDisplayName, wtiPlugin.DisplayName);
            Assert.AreEqual(PluginResources.WtiApplicationDescription, wtiPlugin.Description);
            Assert.AreEqual("0.5.0.0", wtiPlugin.Version);
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