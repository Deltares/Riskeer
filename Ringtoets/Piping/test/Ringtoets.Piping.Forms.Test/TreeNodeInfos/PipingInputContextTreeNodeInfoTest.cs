﻿using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingInputContextTreeNodeInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingInputContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(PipingInputContext), info.TagType);
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

        [Test]
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingInputContext = new PipingInputContext(
                new PipingInput(new GeneralPipingInput()),
                new PipingCalculationScenario(new GeneralPipingInput()),
                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                Enumerable.Empty<StochasticSoilModel>(),
                new PipingFailureMechanism(),
                assessmentSection);

            mocks.ReplayAll();

            // Call
            var text = info.Text(pipingInputContext);

            // Assert
            Assert.AreEqual(Resources.PipingInputContext_NodeDisplayName, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingInputContext = new PipingInputContext(
                new PipingInput(new GeneralPipingInput()),
                new PipingCalculationScenario(new GeneralPipingInput()),
                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                Enumerable.Empty<StochasticSoilModel>(),
                new PipingFailureMechanism(),
                assessmentSection);

            mocks.ReplayAll();

            // Call
            var image = info.Image(pipingInputContext);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.PipingInputIcon, image);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocks.StrictMultiMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Expect(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }
    }
}