using System;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class ContextMenuItemFactoryTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutCommandHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(null, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui, message);
            StringAssert.EndsWith("commandHandler", message);
        }

        [Test]
        public void Constructor_WithoutTreeNode_ThrowsArgumentNullException()
        {
            // Setup
            var guiCommandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(guiCommandHandlerMock, null);
        
            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node, message);
            StringAssert.EndsWith("treeNode", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithGuiAndTreeNode_DoesNotThrow()
        {
            // Setup
            var guiCommandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var treeNodeMock = mocks.Stub<ITreeNode>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(guiCommandHandlerMock, treeNodeMock);

            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateOpenItem_Always_Disabled(bool hasViewersForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanOpenDefaultViewFor(null)).IgnoreArguments().Return(hasViewersForNodeData);
            if (hasViewersForNodeData)
            {
                commandHandlerMock.Expect(ch => ch.OpenView(null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            // Call
            var item = contextMenuFactory.CreateOpenItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Open, item.Text);
            Assert.AreEqual(Resources.Open_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, item.Image);
            Assert.AreEqual(hasViewersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExportItem_Always_ItemWithPropertiesSet(bool hasExportersForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanExportFrom(null)).IgnoreArguments().Return(hasExportersForNodeData);
            if (hasExportersForNodeData)
            {
                commandHandlerMock.Expect(ch => ch.ExportFrom(null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            // Call
            var item = contextMenuFactory.CreateExportItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Export, item.Text);
            Assert.AreEqual(Resources.Export_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, item.Image);
            Assert.AreEqual(hasExportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItem_Always_ItemWithPropertiesSet(bool hasImportersForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanImportOn(null)).IgnoreArguments().Return(hasImportersForNodeData);
            if (hasImportersForNodeData)
            {
                commandHandlerMock.Expect(ch => ch.ImportOn(null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            // Call
            var item = contextMenuFactory.CreateImportItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Import, item.Text);
            Assert.AreEqual(Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.AreEqual(hasImportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreatePropertiesItem_Always_ItemWithPropertiesSet(bool hasPropertyInfoForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesFor(null)).IgnoreArguments().Return(hasPropertyInfoForNodeData);
            if (hasPropertyInfoForNodeData)
            {
                commandHandlerMock.Expect(ch => ch.ShowPropertiesFor(null)).IgnoreArguments();
            }

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreatePropertiesItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Properties, item.Text);
            Assert.AreEqual(Resources.Properties_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.PropertiesIcon, item.Image);
            Assert.AreEqual(hasPropertyInfoForNodeData, item.Enabled);

            mocks.VerifyAll();
        }
    }
}