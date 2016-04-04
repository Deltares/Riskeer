using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

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

                var assessmentSectionProperties = propertyInfos.Single(pi => pi.DataType == typeof(IAssessmentSection));
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
                Assert.AreEqual(4, viewInfos.Length);

                var contributionViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismContribution));
                Assert.AreEqual(typeof(FailureMechanismContributionView), contributionViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, contributionViewInfo.Image);

                var mapViewInfo = viewInfos.Single(vi => vi.DataType == typeof(IAssessmentSection));
                Assert.AreEqual(typeof(AssessmentSectionView), mapViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.Map, mapViewInfo.Image);

                var resultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext));
                Assert.AreEqual(typeof(IEnumerable<FailureMechanismSectionResult>), resultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(FailureMechanismResultView), resultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, resultViewInfo.Image);

                var commentView = viewInfos.Single(vi => vi.DataType == typeof(AssessmentSectionComment));
                Assert.AreEqual(typeof(AssessmentSectionCommentView), commentView.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, commentView.Image);
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
                Assert.AreEqual(10, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(IAssessmentSection)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ReferenceLineContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PlaceholderWithReadonlyName)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismPlaceholderContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CategoryTreeFolder)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismContribution)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssessmentSectionComment)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetChildDataWithViewDefinitions_AssessmentSectionBase_ReturnFailureMechanismContribution()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionBase = mocks.Stub<IAssessmentSection>();
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