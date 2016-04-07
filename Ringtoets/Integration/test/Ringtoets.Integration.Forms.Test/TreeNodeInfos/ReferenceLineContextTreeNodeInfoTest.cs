using System.Drawing;
using System.Linq;

using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;

using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class ReferenceLineContextTreeNodeInfoTest
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
                Assert.AreEqual(typeof(ReferenceLineContext), info.TagType);
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(referenceLineContext);

                // Assert
                Assert.AreEqual("Referentielijn", text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(referenceLineContext);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ReferenceLineIcon, image);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocks.StrictMultiMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            gui.Expect(g => g.Get(null, treeViewControlMock)).Return(menuBuilderMock);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

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

        [Test]
        public void ForeColor_ContextHasNoReferenceLine_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(referenceLineContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasReferenceLineData_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var referenceLineContext = new ReferenceLineContext(assessmentSection);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(referenceLineContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mocks.VerifyAll();
        }

        private TreeNodeInfo GetInfo(RingtoetsGuiPlugin guiPlugin)
        {
            return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ReferenceLineContext));
        }
    }
}