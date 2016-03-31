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
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class StochasticSoilProfileTreeNodeInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StochasticSoilProfile));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(StochasticSoilProfile), info.TagType);
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            var testName = "ttt";
            var pipingSoilLayer = mocks.StrictMock<PipingSoilLayer>(10);
            var pipingSoilProfile = mocks.StrictMock<PipingSoilProfile>(testName, 0, new[]
            {
                pipingSoilLayer
            }, SoilProfileType.SoilProfile2D, 0);
            var stochasticSoilProfile = mocks.StrictMock<StochasticSoilProfile>(10.0, SoilProfileType.SoilProfile1D, 1234L);
            stochasticSoilProfile.SoilProfile = pipingSoilProfile;
            mocks.ReplayAll();

            // Call
            var text = info.Text(stochasticSoilProfile);

            // Assert
            Assert.AreEqual(testName, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var pipingSoilLayer = mocks.StrictMock<PipingSoilLayer>(10);
            var pipingSoilProfile = mocks.StrictMock<PipingSoilProfile>("", 0, new[]
            {
                pipingSoilLayer
            }, SoilProfileType.SoilProfile1D, 0);
            var stochasticSoilProfile = mocks.StrictMock<StochasticSoilProfile>(10.0, SoilProfileType.SoilProfile1D, 1234L);
            stochasticSoilProfile.SoilProfile = pipingSoilProfile;
            mocks.ReplayAll();

            // Call
            var image = info.Image(stochasticSoilProfile);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.PipingSoilProfileIcon, image);

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

            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
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