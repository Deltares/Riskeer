using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Plugin;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.NodePresenters;
using Ringtoets.Integration.Forms.PropertyClasses;

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
                Assert.AreEqual(1, propertyInfos.Length);

                var assessmentSectionProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(AssessmentSectionBase));
                Assert.AreEqual(typeof(AssessmentSectionBaseProperties), assessmentSectionProperties.PropertyType);
                Assert.IsNull(assessmentSectionProperties.AdditionalDataCheck);
                Assert.IsNull(assessmentSectionProperties.GetObjectPropertiesData);
                Assert.IsNull(assessmentSectionProperties.AfterCreate);
            }
        }

        [Test]
        public void GetProjectTreeViewNodePresenters_ReturnsSupportedNodePresenters()
        {
            // setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();

            var guiStub = mocks.DynamicMultiMock<IGui>(typeof(IGui), typeof(IContextMenuBuilderProvider));
            var contextMenuProviderMock = mocks.DynamicMock<IContextMenuBuilderProvider>();

            guiStub.Expect(g => g.ApplicationCore).Return(applicationCore).Repeat.Any();
            guiStub.Expect(g => g.ContextMenuProvider).Return(contextMenuProviderMock).Repeat.Any();

            mocks.ReplayAll();

            using (var guiPlugin = new RingtoetsGuiPlugin
            {
                Gui = guiStub
            })
            {
                // call
                ITreeNodePresenter[] nodePresenters = guiPlugin.GetProjectTreeViewNodePresenters().ToArray();

                // assert
                Assert.AreEqual(5, nodePresenters.Length);
                Assert.IsTrue(nodePresenters.Any(np => np is AssessmentSectionBaseNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PlaceholderWithReadonlyNameNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is FailureMechanismNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is CategoryTreeFolderNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is FailureMechanismContributionNodePresenter));
            }
            mocks.VerifyAll();
        }
    }
}