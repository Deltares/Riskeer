using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsIntegrationDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class FailureMechanismContributionContextTreeNodeInfoTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(FailureMechanismContributionContext), info.TagType);
                Assert.IsNull(info.ForeColor);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(null);

                // Assert
                Assert.AreEqual(RingtoetsIntegrationDataResources.FailureMechanismContribution_DisplayName, text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 150000);

            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(cmp => cmp.Get(contribution, treeViewControlMock)).Return(menuBuilderMock);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);

            
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControlMock);
            }
            // Assert
            mocks.VerifyAll();
        }

        private TreeNodeInfo GetInfo(RingtoetsGuiPlugin guiPlugin)
        {
            return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(FailureMechanismContributionContext));
        }
    }
}