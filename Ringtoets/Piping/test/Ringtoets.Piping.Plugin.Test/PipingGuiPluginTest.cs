using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Workflow;
using Core.Common.Controls;
using Core.Common.Gui;

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
    public class PipingGuiPluginTest
    {
        [Test]
        [STAThread] // For creation of XAML UI component (PipingRibbon)
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var ringtoetsGuiPlugin = new PipingGuiPlugin())
            {
                // assert
                Assert.IsInstanceOf<GuiPlugin>(ringtoetsGuiPlugin);
                Assert.IsInstanceOf<PipingRibbon>(ringtoetsGuiPlugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var guiPlugin = new PipingGuiPlugin())
            {
                // call
                PropertyInfo[] propertyInfos = guiPlugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(4, propertyInfos.Length);

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

            using (var guiPlugin = new PipingGuiPlugin { Gui = guiStub })
            {
                // call
                ITreeNodePresenter[] nodePresenters = guiPlugin.GetProjectTreeViewNodePresenters().ToArray();

                // assert
                Assert.AreEqual(7, nodePresenters.Length);
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