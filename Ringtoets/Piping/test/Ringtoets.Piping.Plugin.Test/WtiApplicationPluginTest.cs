using System;
using System.Linq;
using Core.Common.BaseDelftTools;
using Mono.Addins;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using PluginResources = Ringtoets.Piping.Plugin.Properties.Resources;
using WtiFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test
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
            Assert.AreEqual(PluginResources.Wti_application_name, wtiPlugin.Name);
            Assert.AreEqual(PluginResources.Wti_application_DisplayName, wtiPlugin.DisplayName);
            Assert.AreEqual(PluginResources.Wti_application_Description, wtiPlugin.Description);
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
            Assert.AreEqual(WtiFormsResources.WtiProjectProperties_DisplayName, projectDataItemDefinition.Name);
            Assert.AreEqual(PluginResources.Wti_application_name, projectDataItemDefinition.Category);
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