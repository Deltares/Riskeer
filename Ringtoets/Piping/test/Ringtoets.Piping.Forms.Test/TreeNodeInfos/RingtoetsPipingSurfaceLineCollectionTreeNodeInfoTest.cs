using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;

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
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLinesContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(RingtoetsPipingSurfaceLinesContext), info.TagType);
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

        [Test]
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var ringtoetsPipingSurfaceLines = mocks.StrictMock<RingtoetsPipingSurfaceLinesContext>(failureMechanism, assessmentSection);

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
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var ringtoetsPipingSurfaceLines = mocks.StrictMock<RingtoetsPipingSurfaceLinesContext>(failureMechanism, assessmentSection);

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
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var ringtoetsPipingSurfaceLines = mocks.StrictMock<RingtoetsPipingSurfaceLinesContext>(failureMechanism, assessmentSection);

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
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var ringtoetsPipingSurfaceLine1 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.Add(ringtoetsPipingSurfaceLine1);
            failureMechanism.SurfaceLines.Add(ringtoetsPipingSurfaceLine2);

            var ringtoetsPipingSurfaceLineContext = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

            // Call
            var foreColor = info.ForeColor(ringtoetsPipingSurfaceLineContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var ringtoetsPipingSurfaceLine1 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.Add(ringtoetsPipingSurfaceLine1);
            failureMechanism.SurfaceLines.Add(ringtoetsPipingSurfaceLine2);

            var ringtoetsPipingSurfaceLineContext = new RingtoetsPipingSurfaceLinesContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(ringtoetsPipingSurfaceLineContext);

            // Assert
            CollectionAssert.AreEqual(new[] { ringtoetsPipingSurfaceLine1, ringtoetsPipingSurfaceLine2 }, objects);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocks.StrictMultiMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            gui.Expect(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);

            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, null, treeViewControl);

            // Assert
            mocks.VerifyAll();
        }
    }
}