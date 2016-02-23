using System;
using System.Linq;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Plugins.DotSpatial.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Placeholder;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsGuiPluginTest
    {
        [Test]
        [STAThread] // For creation of XAML UI component
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var ringtoetsGuiPlugin = new RingtoetsGuiPlugin())
            {
                // assert
                Assert.IsInstanceOf<GuiPlugin>(ringtoetsGuiPlugin);
                Assert.IsInstanceOf<RingtoetsRibbon>(ringtoetsGuiPlugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var guiPlugin = new RingtoetsGuiPlugin())
            {
                // call
                PropertyInfo[] propertyInfos = guiPlugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(2, propertyInfos.Length);

                var assessmentSectionProperties = propertyInfos.Single(pi => pi.DataType == typeof(AssessmentSectionBase));
                Assert.AreEqual(typeof(AssessmentSectionBaseProperties), assessmentSectionProperties.PropertyObjectType);
                Assert.IsNull(assessmentSectionProperties.AdditionalDataCheck);
                Assert.IsNull(assessmentSectionProperties.GetObjectPropertiesData);
                Assert.IsNull(assessmentSectionProperties.AfterCreate);

                var hydraulicBoundaryDatabase = propertyInfos.Single(pi => pi.DataType == typeof(HydraulicBoundaryDatabaseContext));
                Assert.AreEqual(typeof(HydraulicBoundaryDatabaseProperties), hydraulicBoundaryDatabase.PropertyObjectType);
                Assert.IsNull(hydraulicBoundaryDatabase.AdditionalDataCheck);
                Assert.IsNull(hydraulicBoundaryDatabase.GetObjectPropertiesData);
                Assert.IsNull(hydraulicBoundaryDatabase.AfterCreate);
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfoClasses()
        {
            // Setup
            using (var guiPlugin = new RingtoetsGuiPlugin())
            {
                // Call
                ViewInfo[] viewInfos = guiPlugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(2, viewInfos.Length);

                var contributionViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismContribution));
                Assert.AreEqual(typeof(FailureMechanismContributionView), contributionViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GenericInputOutputIcon, contributionViewInfo.Image);

                var mapViewInfo = viewInfos.Single(vi => vi.DataType == typeof(AssessmentSectionBase));
                Assert.AreEqual(typeof(AssessmentSectionView), mapViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.Map, mapViewInfo.Image);
            }
        }
				

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();

            var guiStub = mocks.DynamicMultiMock<IGui>(typeof(IGui), typeof(IContextMenuBuilderProvider));

            guiStub.Expect(g => g.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            using (var guiPlugin = new RingtoetsGuiPlugin
            {
                Gui = guiStub
            })
            {
                // call
                TreeNodeInfo[] treeNodeInfos = guiPlugin.GetTreeNodeInfos().ToArray();

                // assert
                Assert.AreEqual(7, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssessmentSectionBase)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ReferenceLineContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PlaceholderWithReadonlyName)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismPlaceholder)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CategoryTreeFolder)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismContribution)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetChildDataWithViewDefinitions_AssessmentSectionBase_ReturnFailureMechanismContribution()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionBase = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var guiPlugin = new RingtoetsGuiPlugin();

            // Call
            var childrenWithViewDefinitions = guiPlugin.GetChildDataWithViewDefinitions(assessmentSectionBase);

            // Assert
            CollectionAssert.AreEqual(new object[]
            {
                assessmentSectionBase.FailureMechanismContribution
            }, childrenWithViewDefinitions);
            mocks.VerifyAll();
        }

        [Test]
        public void GetChildDataWithViewDefinitions_UnsupportedData_ReturnEmpty()
        {
            // Setup
            var guiPlugin = new RingtoetsGuiPlugin();

            // Call
            var childrenWithViewDefinitions = guiPlugin.GetChildDataWithViewDefinitions(1);

            // Assert
            CollectionAssert.IsEmpty(childrenWithViewDefinitions);
        }
    }
}