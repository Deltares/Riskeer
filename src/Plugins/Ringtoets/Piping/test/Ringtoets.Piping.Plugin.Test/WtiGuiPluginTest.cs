using System;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using Mono.Addins;
using NUnit.Framework;

using Rhino.Mocks;

using Wti.Data;
using Wti.Forms.NodePresenters;
using Wti.Forms.PresentationObjects;
using Wti.Forms.PropertyClasses;
using GuiPluginResources = Wti.Plugin.Properties.Resources;

namespace Wti.Plugin.Test
{
    [TestFixture]
    public class WtiGuiPluginTest
    {
        [Test]
        [STAThread] // For creation of XAML UI component
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
                Assert.IsInstanceOf<WtiRibbon>(wtiGuiPlugin.RibbonCommandHandler);
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
                Assert.AreEqual(4, propertyInfos.Length);

                var wtiProjectProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(WtiProject));
                Assert.AreEqual(typeof(WtiProjectProperties), wtiProjectProperties.PropertyType);
                Assert.IsNull(wtiProjectProperties.AdditionalDataCheck);
                Assert.IsNull(wtiProjectProperties.GetObjectPropertiesData);
                Assert.IsNull(wtiProjectProperties.AfterCreate);

                var pipingDataProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(PipingCalculationInputs));
                Assert.AreEqual(typeof(PipingCalculationInputsProperties), pipingDataProperties.PropertyType);
                Assert.IsNull(pipingDataProperties.AdditionalDataCheck);
                Assert.IsNull(pipingDataProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingDataProperties.AfterCreate);

                var pipingOutputProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(PipingOutput));
                Assert.AreEqual(typeof(PipingOutputProperties), pipingOutputProperties.PropertyType);
                Assert.IsNull(pipingOutputProperties.AdditionalDataCheck);
                Assert.IsNull(pipingOutputProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingOutputProperties.AfterCreate);

                var pipingSurfaceLineProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(RingtoetsPipingSurfaceLine));
                Assert.AreEqual(typeof(PipingSurfaceLineProperties), pipingSurfaceLineProperties.PropertyType);
                Assert.IsNull(pipingSurfaceLineProperties.AdditionalDataCheck);
                Assert.IsNull(pipingSurfaceLineProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingSurfaceLineProperties.AfterCreate);
            }
        }

        [Test]
        public void GetProjectTreeViewNodePresenters_ReturnsSupportedNodePresenters()
        {
            // setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.CommandHandler = mocks.Stub<IGuiCommandHandler>();
            mocks.ReplayAll();

            using (var guiPlugin = new WtiGuiPlugin { Gui = guiStub })
            {
                // call
                ITreeNodePresenter[] nodePresenters = guiPlugin.GetProjectTreeViewNodePresenters().ToArray();

                // assert
                Assert.AreEqual(7, nodePresenters.Length);
                Assert.IsTrue(nodePresenters.Any(np => np is WtiProjectNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSurfaceLineCollectionNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSurfaceLineNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSoilProfileCollectionNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingCalculationInputsNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingFailureMechanismNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingOutputNodePresenter));
            }
            mocks.VerifyAll();
        }
    }
}