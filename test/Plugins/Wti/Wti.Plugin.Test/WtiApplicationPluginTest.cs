using System;
using System.Linq;
using DelftTools.Shell.Core;
using Mono.Addins;
using NUnit.Framework;
using Wti.Data;
using PluginResources = Wti.Plugin.Properties.Resources;
using FormsResources = Wti.Forms.Properties.Resources;

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
            var extensionAttribute = (ExtensionAttribute) attribute;
            Assert.AreEqual(typeof(IPlugin), extensionAttribute.Type);
        }

        [Test]
        public void GetDataItemInfos_ReturnsExpectedDataItemDefinitions()
        {
            // setup
            var plugin = new WtiApplicationPlugin();

            // call
            var dataItemDefinitions = plugin.GetDataItemInfos().ToArray();

            // assert
            Assert.AreEqual(1, dataItemDefinitions.Length);

            DataItemInfo projectDataItemDefinition = dataItemDefinitions.Single(did => did.ValueType == typeof(WtiProject));
            Assert.AreEqual("WTI project", projectDataItemDefinition.Name);
            Assert.AreEqual("WTI", projectDataItemDefinition.Category);
            Assert.AreEqual(16, projectDataItemDefinition.Image.Width);
            Assert.AreEqual(16, projectDataItemDefinition.Image.Height);
            Assert.IsNull(projectDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<WtiProject>(projectDataItemDefinition.CreateData(null));
            Assert.IsNull(projectDataItemDefinition.AddExampleData);
        }
    }
}