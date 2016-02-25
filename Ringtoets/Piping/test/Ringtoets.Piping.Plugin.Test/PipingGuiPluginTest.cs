using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Views;
using GuiPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

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
        public void GetViewInfos_ReturnsSupportedViews()
        {
            // Setup
            using (var guiPlugin = new PipingGuiPlugin())
            {
                // Call
                ViewInfo[] viewInfos = guiPlugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(1, viewInfos.Length);

                var contributionViewInfo = viewInfos.Single(vi => vi.DataType == typeof(PipingFailureMechanism));
                Assert.AreEqual(typeof(PipingFailureMechanismView), contributionViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, contributionViewInfo.Image);
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
                Assert.AreEqual(6, propertyInfos.Length);

                var pipingCalculationContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingCalculationContext));
                Assert.AreEqual(typeof(PipingCalculationContextProperties), pipingCalculationContextProperties.PropertyObjectType);
                Assert.IsNull(pipingCalculationContextProperties.AdditionalDataCheck);
                Assert.IsNull(pipingCalculationContextProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingCalculationContextProperties.AfterCreate);

                var pipingCalculationGroupProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingCalculationGroupContext));
                Assert.AreEqual(typeof(PipingCalculationGroupContextProperties), pipingCalculationGroupProperties.PropertyObjectType);
                Assert.IsNull(pipingCalculationGroupProperties.AdditionalDataCheck);
                Assert.IsNull(pipingCalculationGroupProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingCalculationGroupProperties.AfterCreate);

                var pipingInputContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingInputContext));
                Assert.AreEqual(typeof(PipingInputContextProperties), pipingInputContextProperties.PropertyObjectType);
                Assert.IsNull(pipingInputContextProperties.AdditionalDataCheck);
                Assert.IsNull(pipingInputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingInputContextProperties.AfterCreate);

                var pipingOutputProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingOutput));
                Assert.AreEqual(typeof(PipingOutputProperties), pipingOutputProperties.PropertyObjectType);
                Assert.IsNull(pipingOutputProperties.AdditionalDataCheck);
                Assert.IsNull(pipingOutputProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingOutputProperties.AfterCreate);

                var pipingSurfaceLineProperties = propertyInfos.Single(pi => pi.DataType == typeof(RingtoetsPipingSurfaceLine));
                Assert.AreEqual(typeof(RingtoetsPipingSurfaceLineProperties), pipingSurfaceLineProperties.PropertyObjectType);
                Assert.IsNull(pipingSurfaceLineProperties.AdditionalDataCheck);
                Assert.IsNull(pipingSurfaceLineProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingSurfaceLineProperties.AfterCreate);

                var pipingSoilProfileProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingSoilProfile));
                Assert.AreEqual(typeof(PipingSoilProfileProperties), pipingSoilProfileProperties.PropertyObjectType);
                Assert.IsNull(pipingSoilProfileProperties.AdditionalDataCheck);
                Assert.IsNull(pipingSoilProfileProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingSoilProfileProperties.AfterCreate);
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();

            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());

            Expect.Call(guiStub.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            using (var guiPlugin = new PipingGuiPlugin
            {
                Gui = guiStub
            })
            {
                // call
                TreeNodeInfo[] treeNodeInfos = guiPlugin.GetTreeNodeInfos().ToArray();

                // assert
                Assert.AreEqual(11, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(IEnumerable<RingtoetsPipingSurfaceLine>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLine)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(IEnumerable<PipingSoilProfile>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingSoilProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingFailureMechanism)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyPipingOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyPipingCalculationReport)));
            }
            mocks.VerifyAll();
        }
    }
}