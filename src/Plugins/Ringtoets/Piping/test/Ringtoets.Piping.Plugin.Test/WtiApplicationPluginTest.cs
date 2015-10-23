using System;
using System.Linq;
using DelftTools.Shell.Core;
using Mono.Addins;
using NUnit.Framework;
using Wti.Data;
using Wti.Plugin.FileImporter;

using PluginResources = Wti.Plugin.Properties.Resources;
using FormsResources = Wti.Forms.Properties.Resources;

using WtiFormsResources = Wti.Forms.Properties.Resources;
using ApplicationResources = Wti.Plugin.Properties.Resources;

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
            Assert.AreEqual(WtiFormsResources.WtiProjectPropertiesDisplayName, projectDataItemDefinition.Name);
            Assert.AreEqual(ApplicationResources.WtiApplicationName, projectDataItemDefinition.Category);
            Assert.AreEqual(16, projectDataItemDefinition.Image.Width);
            Assert.AreEqual(16, projectDataItemDefinition.Image.Height);
            Assert.IsNull(projectDataItemDefinition.AdditionalOwnerCheck);
            Assert.IsInstanceOf<WtiProject>(projectDataItemDefinition.CreateData(null));
            Assert.IsNull(projectDataItemDefinition.AddExampleData);
        }

        [Test]
        public void GetFileImporters_Always_ReturnExpectedFileImporters()
        {
            // Setup
            var plugin = new WtiApplicationPlugin();

            // Call
            var importers = plugin.GetFileImporters().ToArray();

            // Assert
            Assert.AreEqual(2, importers.Length);
            Assert.IsInstanceOf<PipingSurfaceLinesCsvImporter>(importers[0]);
            Assert.IsInstanceOf<PipingSoilProfilesImporter>(importers[1]);
        }
    }
}