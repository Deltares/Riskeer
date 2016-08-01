using System.Collections.Generic;
using System.Linq;

using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class StochasticSoilModelTreeNodeInfoTest
    {
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StochasticSoilModel));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(StochasticSoilModel), info.TagType);
            Assert.IsNull(info.EnsureVisibleOnCreate);
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
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            const string name = "test test 123";
            var model = new StochasticSoilModel(1, name, "a");

            // Call
            var text = info.Text(model);

            // Assert
            Assert.AreEqual(name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var model = new StochasticSoilModel(1, "A", "B");

            // Call
            var image = info.Image(model);

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.StochasticSoilModelIcon, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOnData()
        {
            // Setup
            var stochasticSoilProfile1 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("pipingSoilProfile1", 0, new List<PipingSoilLayer>
                {
                    new PipingSoilLayer(10)
                }, SoilProfileType.SoilProfile1D, 0)
            };
            var stochasticSoilProfile2 = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("pipingSoilProfile2", 0, new List<PipingSoilLayer>
                {
                    new PipingSoilLayer(10)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var stochasticSoilModel = new StochasticSoilModel(0, "Name", "Name");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile1);
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile2);

            // Call
            var objects = info.ChildNodeObjects(stochasticSoilModel);

            // Assert
            var expectedChildren = new[]
            {
                stochasticSoilProfile1,
                stochasticSoilProfile2
            };
            CollectionAssert.AreEqual(expectedChildren, objects);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var model = new StochasticSoilModel(1, "A", "B");

            var mocks = new MockRepository();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.StrictMultiMock<IGui>();
                gui.Expect(g => g.Get(model, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(model, null, treeViewControl);
            }
            // Assert
            mocks.VerifyAll();
        }
    }
}