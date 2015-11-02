using System;
using System.Linq;
using Core.Common.BaseDelftTools;
using Core.Common.BaseDelftTools.Workflow;
using Core.Common.Controls;
using Core.Common.Gui;

using Mono.Addins;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

using GuiPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test
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
                Assert.AreEqual(GuiPluginResources.Wti_application_gui_name, wtiGuiPlugin.Name);
                Assert.AreEqual(GuiPluginResources.WtiGuiPlugin_DisplayName, wtiGuiPlugin.DisplayName);
                Assert.AreEqual(GuiPluginResources.WtiGuiPlugin_DisplayName, wtiGuiPlugin.Description);
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
                Assert.AreEqual(5, propertyInfos.Length);

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
                Assert.AreEqual(typeof(RingtoetsPipingSurfaceLineProperties), pipingSurfaceLineProperties.PropertyType);
                Assert.IsNull(pipingSurfaceLineProperties.AdditionalDataCheck);
                Assert.IsNull(pipingSurfaceLineProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingSurfaceLineProperties.AfterCreate);

                var pipingSoilProfileProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(PipingSoilProfile));
                Assert.AreEqual(typeof(PipingSoilProfileProperties), pipingSoilProfileProperties.PropertyType);
                Assert.IsNull(pipingSoilProfileProperties.AdditionalDataCheck);
                Assert.IsNull(pipingSoilProfileProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingSoilProfileProperties.AfterCreate);
            }
        }

        [Test]
        public void GetProjectTreeViewNodePresenters_ReturnsSupportedNodePresenters()
        {
            // setup
            var mocks = new MockRepository();
            var application = mocks.Stub<IApplication>();
            application.Stub(a => a.ActivityRunner).Return(mocks.Stub<IActivityRunner>());

            var guiStub = mocks.Stub<IGui>();
            guiStub.CommandHandler = mocks.Stub<IGuiCommandHandler>();
            guiStub.Application = application;

            mocks.ReplayAll();

            using (var guiPlugin = new WtiGuiPlugin { Gui = guiStub })
            {
                // call
                ITreeNodePresenter[] nodePresenters = guiPlugin.GetProjectTreeViewNodePresenters().ToArray();

                // assert
                Assert.AreEqual(8, nodePresenters.Length);
                Assert.IsTrue(nodePresenters.Any(np => np is WtiProjectNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSurfaceLineCollectionNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSurfaceLineNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSoilProfileCollectionNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingSoilProfileNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingCalculationInputsNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingFailureMechanismNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PipingOutputNodePresenter));
            }
            mocks.VerifyAll();
        }
    }
}