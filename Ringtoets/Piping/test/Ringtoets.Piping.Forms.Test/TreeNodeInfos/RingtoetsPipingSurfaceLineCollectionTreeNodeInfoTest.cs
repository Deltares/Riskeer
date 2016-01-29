using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Plugin;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineCollectionTreeNodeInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(IEnumerable<RingtoetsPipingSurfaceLine>));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(IEnumerable<RingtoetsPipingSurfaceLine>), info.TagType);
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
            // Setup
            var ringtoetsPipingSurfaceLines = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();

            mocks.ReplayAll();

            // Call
            var text = info.Text(ringtoetsPipingSurfaceLines);

            // Assert
            Assert.AreEqual(Resources.PipingSurfaceLinesCollection_DisplayName, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var ringtoetsPipingSurfaceLines = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();

            mocks.ReplayAll();

            // Call
            var image = info.Image(ringtoetsPipingSurfaceLines);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.FolderIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionWithoutSurfaceLines_ReturnsGrayText()
        {
            // Setup
            var ringtoetsPipingSurfaceLines = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();

            ringtoetsPipingSurfaceLines.Expect(collection => collection.GetEnumerator());

            mocks.ReplayAll();

            // Call
            var foreColor = info.ForeColor(ringtoetsPipingSurfaceLines);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionWithSurfaceLines_ReturnsControlText()
        {
            // Setup
            IEnumerable<RingtoetsPipingSurfaceLine> ringtoetsPipingSurfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };

            // Call
            var foreColor = info.ForeColor(ringtoetsPipingSurfaceLines);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var ringtoetsPipingSurfaceLine1 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();
            IEnumerable<RingtoetsPipingSurfaceLine> ringtoetsPipingSurfaceLines = new[]
            {
                ringtoetsPipingSurfaceLine1,
                ringtoetsPipingSurfaceLine2
            };

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(ringtoetsPipingSurfaceLines);

            // Assert
            CollectionAssert.AreEqual(new[] { ringtoetsPipingSurfaceLine1, ringtoetsPipingSurfaceLine2 }, objects);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var treeNode = new TreeNode();
            var gui = mocks.StrictMultiMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            gui.Expect(g => g.Get(treeNode, info)).Return(menuBuilderMock);

            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, treeNode, info);

            // Assert
            mocks.VerifyAll();
        }
    }
}