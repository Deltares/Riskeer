using System;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using Mono.Addins;
using NUnit.Framework;
using Wti.Data;
using Wti.Forms.NodePresenters;
using Wti.Forms.PropertyClasses;
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
            using (var wtiGuiPlugin = new WtiGuiPlugin())
            {
                // assert
                Assert.IsInstanceOf<GuiPlugin>(wtiGuiPlugin);
                Assert.AreEqual(GuiPluginResources.WtiApplicationGuiName, wtiGuiPlugin.Name);
                Assert.AreEqual(GuiPluginResources.wtiGuiPluginDisplayName, wtiGuiPlugin.DisplayName);
                Assert.AreEqual(GuiPluginResources.wtiGuiPluginDescription, wtiGuiPlugin.Description);
                Assert.AreEqual("0.5.0.0", wtiGuiPlugin.Version);
            }
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
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var guiPlugin = new WtiGuiPlugin())
            {
                // call
                PropertyInfo[] propertyInfos = guiPlugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(3, propertyInfos.Length);

                var wtiProjectProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(WtiProject));
                Assert.AreEqual(typeof(WtiProjectProperties), wtiProjectProperties.PropertyType);
                Assert.IsNull(wtiProjectProperties.AdditionalDataCheck);
                Assert.IsNull(wtiProjectProperties.GetObjectPropertiesData);
                Assert.IsNull(wtiProjectProperties.AfterCreate);

                var pipingDataProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(PipingData));
                Assert.AreEqual(typeof(PipingDataProperties), pipingDataProperties.PropertyType);
                Assert.IsNull(pipingDataProperties.AdditionalDataCheck);
                Assert.IsNull(pipingDataProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingDataProperties.AfterCreate);

                var pipingOutputProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(PipingOutput));
                Assert.AreEqual(typeof(PipingOutputProperties), pipingOutputProperties.PropertyType);
                Assert.IsNull(pipingOutputProperties.AdditionalDataCheck);
                Assert.IsNull(pipingOutputProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingOutputProperties.AfterCreate);
            }
        }

        [Test]
        public void GetProjectTreeViewNodePresenters_ReturnsSupportedNodePresenters()
        {
            // setup
            using (var guiPlugin = new WtiGuiPlugin())
            {
                // call
                ITreeNodePresenter[] nodePresenters = guiPlugin.GetProjectTreeViewNodePresenters().ToArray();

                // assert
                Assert.AreEqual(5, nodePresenters.Length);
                Assert.IsTrue(nodePresenters.Any(np => np is WtiProjectNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSurfaceLineCollectionNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingDataNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingFailureMechanismNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingOutputNodePresenter));
            }
        }
    }
}