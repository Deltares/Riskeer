using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Plugin;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PlaceholderWithReadonlyNameTreeNodeInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PlaceholderWithReadonlyName));
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
            Assert.AreEqual(typeof(PlaceholderWithReadonlyName), info.TagType);
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
            var placeholder = new PlaceholderWithReadonlyName(testName);

            // Call
            var text = info.Text(placeholder);

            // Assert
            Assert.AreEqual(testName, text);
        }

        [Test]
        public void Image_OutputPlaceHolder_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image(new OutputPlaceholder(string.Empty));

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void Image_InputPlaceHolder_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image(new InputPlaceholder(string.Empty));

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void Image_PlaceHolder_ReturnsPlaceHolderIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.PlaceholderIcon, image);
        }

        [Test]
        public void ForeColor_Always_ReturnsGrayText()
        {
            // Call
            var textColor = info.ForeColor(null);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), textColor);
        }

        [Test]
        public void ContextMenuStrip_OutputPlaceHolder_CallsContextMenuBuilderMethods()
        {
            // Setup
            var outputPlaceholder = new OutputPlaceholder(string.Empty);
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(outputPlaceholder, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(outputPlaceholder, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_InputPlaceHolder_CallsContextMenuBuilderMethods()
        {
            // Setup
            var inputPlaceholder = new InputPlaceholder(string.Empty);
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(inputPlaceholder, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(inputPlaceholder, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_PlaceHolder_CallsContextMenuBuilderMethods()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(null, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }
    }
}