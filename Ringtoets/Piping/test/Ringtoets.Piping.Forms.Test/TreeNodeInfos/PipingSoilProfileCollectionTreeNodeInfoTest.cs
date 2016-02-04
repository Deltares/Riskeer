using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class PipingSoilProfileCollectionTreeNodeInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(IEnumerable<PipingSoilProfile>));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(IEnumerable<PipingSoilProfile>), info.TagType);
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
            var pipingSoilProfiles = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();

            mocks.ReplayAll();

            // Call
            var text = info.Text(pipingSoilProfiles);

            // Assert
            Assert.AreEqual(Resources.PipingSoilProfilesCollection_DisplayName, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var pipingSoilProfiles = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();

            mocks.ReplayAll();

            // Call
            var image = info.Image(pipingSoilProfiles);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.FolderIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionWithoutSoilProfiles_ReturnsGrayText()
        {
            // Setup
            var pipingSoilProfiles = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();

            pipingSoilProfiles.Expect(collection => collection.GetEnumerator());

            mocks.ReplayAll();

            // Call
            var foreColor = info.ForeColor(pipingSoilProfiles);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_CollectionWithSoilProfiles_ReturnsControlText()
        {
            // Setup
            IEnumerable<PipingSoilProfile> pipingSoilProfiles = new[]
            {
                new PipingSoilProfile("", 0, new List<PipingSoilLayer> { new PipingSoilLayer(10) }),
                new PipingSoilProfile("", 0, new List<PipingSoilLayer> { new PipingSoilLayer(10) })
            };

            // Call
            var foreColor = info.ForeColor(pipingSoilProfiles);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var pipingSoilProfile1 = new PipingSoilProfile("", 0, new List<PipingSoilLayer> { new PipingSoilLayer(10) });
            var pipingSoilProfile2 = new PipingSoilProfile("", 0, new List<PipingSoilLayer> { new PipingSoilLayer(10) });
            IEnumerable<PipingSoilProfile> pipingSoilProfiles = new[]
            {
                pipingSoilProfile1,
                pipingSoilProfile2
            };

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(pipingSoilProfiles);

            // Assert
            CollectionAssert.AreEqual(new[] { pipingSoilProfile1, pipingSoilProfile2 }, objects);

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