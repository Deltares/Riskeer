using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Plugin;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsIntegrationDataResources = Ringtoets.Integration.Data.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class FailureMechanismContributionTreeNodeInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(FailureMechanismContribution));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismContribution), info.TagType);

            Assert.IsNull(info.ForeColor);
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

        [Test]
        public void Text_Always_ReturnsTextFromResource()
        {
            // Call
            var text = info.Text(null);

            // Assert
            Assert.AreEqual(RingtoetsIntegrationDataResources.FailureMechanismContribution_DisplayName, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.GenericInputOutputIcon, image);

            mocks.VerifyAll();
        }


        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var nodeMock = mocks.StrictMock<TreeNode>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(nodeMock, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();
            
            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, nodeMock, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }
    }
}