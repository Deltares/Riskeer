using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Plugins.ProjectExplorer.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test.TreeNodeInfos
{
    [TestFixture]
    public class ProjectTreeNodeInfoTest
    {
        private MockRepository mocks;
        private ProjectExplorerGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new ProjectExplorerGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(Project));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(Project), info.TagType);
            Assert.IsNull(info.ForeColor);
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            var project = mocks.StrictMock<Project>();
            var testName = "ttt";
            project.Name = testName;

            mocks.ReplayAll();

            // Call
            var text = info.Text(project);

            // Assert
            Assert.AreEqual(testName, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var project = mocks.StrictMock<Project>();

            mocks.ReplayAll();

            // Call
            var image = info.Image(project);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.ProjectIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var object1 = new object();
            var object2 = new object();
            var project = mocks.StrictMock<Project>();

            project.Items.Add(object1);
            project.Items.Add(object2);

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(project);

            // Assert
            CollectionAssert.AreEqual(new [] { object1, object2 }, objects);

            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var treeNode = new TreeNode();
            var gui = mocks.StrictMultiMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            gui.Expect(g => g.Get(treeNode, treeViewControlMock)).Return(menuBuilderMock);
            gui.Expect(g => g.ViewCommands).Return(viewCommandsMock);
            gui.Expect(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, treeNode, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void GetContextMenu_Always_AddsAddItem()
        {
            // Setup
            var project = new Project();
            var treeNode = new TreeNode();
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var projectCommandsMock = mocks.StrictMock<IProjectCommands>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            guiMock.Stub(g => g.Get(treeNode, treeViewControlMock)).Return(menuBuilder);
            guiMock.Stub(g => g.ProjectCommands).Return(projectCommandsMock);
            guiMock.Stub(g => g.ViewCommands).Return(viewCommandsMock);
            guiMock.Stub(g => g.GetTreeNodeInfos()).Return(Enumerable.Empty<TreeNodeInfo>());
            projectCommandsMock.Expect(g => g.AddNewItem(project));

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var result = info.ContextMenuStrip(project, treeNode, treeViewControlMock);

            result.Items[0].PerformClick();

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.AddItem, Resources.AddItem_ToolTip, Resources.PlusIcon);
            mocks.VerifyAll();
        }
    }
}