using System;
using Core.Common.Controls;
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
            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(mocks.StrictMock<IGuiCommandHandler>(), null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node, message);
            StringAssert.EndsWith("treeNode", message);
        }

        [Test]
        public void Constructor_WithGuiAndTreeNode_NewInstance()
        {
            // Call
            var result = new GuiContextMenuItemFactory(mocks.Stub<IGuiCommandHandler>(), mocks.Stub<ITreeNode>());

            // Assert
            Assert.IsInstanceOf<GuiContextMenuItemFactory>(result);
        }

        [Test]
        public void CreateOpenItem_NoViewersForType_Disabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            guiMock.Expect(ch => ch.CanOpenDefaultViewFor(null)).IgnoreArguments().Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateOpenItem();

            // Assert
            Assert.AreEqual(Resources.Open, item.Text);
            Assert.AreEqual(Resources.Open_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, item.Image);
            Assert.IsFalse(item.Enabled);
        }

        [Test]
        public void CreateOpenItem_ViewersForType_Enabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanOpenDefaultViewFor(null)).IgnoreArguments().Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateOpenItem();

            // Assert
            Assert.AreEqual(Resources.Open, item.Text);
            Assert.AreEqual(Resources.Open_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, item.Image);
            Assert.IsTrue(item.Enabled);
        }

        [Test]
        public void CreateExportItem_NoImporterExportersForType_Disabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanExportFrom(null)).IgnoreArguments().Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateExportItem();

            // Assert
            Assert.AreEqual(Resources.Export, item.Text);
            Assert.AreEqual(Resources.Export_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, item.Image);
            Assert.IsFalse(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportItem_ExportersForType_Enabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanExportFrom(null)).IgnoreArguments().Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateExportItem();

            // Assert
            Assert.AreEqual(Resources.Export, item.Text);
            Assert.AreEqual(Resources.Export_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, item.Image);
            Assert.IsTrue(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItem_NoImportersForType_Disabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanImportOn(null)).IgnoreArguments().Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateImportItem();

            // Assert
            Assert.AreEqual(Resources.Import, item.Text);
            Assert.AreEqual(Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.IsFalse(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItem_ImportersForType_Enabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanImportOn(null)).IgnoreArguments().Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateImportItem();

            // Assert
            Assert.AreEqual(Resources.Import, item.Text);
            Assert.AreEqual(Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.IsTrue(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_PropertieInfoForType_Enabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesFor(null)).IgnoreArguments().Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreatePropertiesItem();

            // Assert
            Assert.AreEqual(Resources.Properties, item.Text);
            Assert.AreEqual(Resources.Properties_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.PropertiesIcon, item.Image);
            Assert.IsTrue(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_NoPropertieInfoForType_Disabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesFor(null)).IgnoreArguments().Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreatePropertiesItem();

            // Assert
            Assert.AreEqual(Resources.Properties, item.Text);
            Assert.AreEqual(Resources.Properties_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.PropertiesIcon, item.Image);
            Assert.IsFalse(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_ClickWithHandler_NoExceptions()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var nodeStub = mocks.Stub<ITreeNode>();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesFor(null)).IgnoreArguments().Return(true);
            commandHandlerMock.Expect(ch => ch.ShowPropertiesFor(null)).IgnoreArguments();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, nodeStub);

            mocks.ReplayAll();

            var item = contextMenuFactory.CreatePropertiesItem();
            
            // Call & Assert
            item.PerformClick();
            
            mocks.VerifyAll();
        }
    }
}