using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class CategoryTreeFolderTreeNodeInfoTest
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
                Assert.AreEqual(typeof(CategoryTreeFolder), info.TagType);
                Assert.IsNull(info.EnsureVisibleOnCreate);
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            var testname = "testName";
            var categoryTreeFolder = new CategoryTreeFolder(testname, new object[0]);
            
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(categoryTreeFolder);

                // Assert
                Assert.AreEqual(testname, text);
            }
        }

        [Test]
        public void Image_TreeFolderOfCategoryGeneral_ReturnsGeneralFolderIcon()
        {
            // Setup
            var categoryTreeFolder = new CategoryTreeFolder("", new object[0]);
            
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void Image_TreeFolderOfCategoryInput_ReturnsInputFolderIcon()
        {
            // Setup
            var categoryTreeFolder = new CategoryTreeFolder("", new object[0], TreeFolderCategory.Input);
            
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.InputFolderIcon, image);
            }
        }

        [Test]
        public void Image_TreeFolderOfCategoryOutput_ReturnsOutputFolderIcon()
        {
            // Setup
            var categoryTreeFolder = new CategoryTreeFolder("", new object[0], TreeFolderCategory.Output);
            
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(categoryTreeFolder);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.OutputFolderIcon, image);
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var object1 = new object();
            var object2 = new object();
            var categoryTreeFolder = new CategoryTreeFolder("", new[] { object1, object2 });
            
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var objects = info.ChildNodeObjects(categoryTreeFolder);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    object1,
                    object2
                }, objects);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(null, treeViewControlMock)).Return(menuBuilderMock);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            mocks.ReplayAll();
            
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControlMock);
            }
            // Assert
            mocks.VerifyAll();
        }


        private TreeNodeInfo GetInfo(RingtoetsGuiPlugin guiPlugin)
        {
            return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(CategoryTreeFolder));
        }
    }
}
