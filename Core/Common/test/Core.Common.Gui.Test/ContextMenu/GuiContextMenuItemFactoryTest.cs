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
            TestDelegate test = () => new GuiContextMenuItemFactory(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui, message);
            StringAssert.EndsWith("commandHandler", message);

        }

        [Test]
        public void Constructor_WithGuiAndTreeNode_NewInstance()
        {
            // Call
            var result = new GuiContextMenuItemFactory(mocks.StrictMock<IGuiCommandHandler>());

            // Assert
            Assert.IsInstanceOf<GuiContextMenuItemFactory>(result);
        }

        [Test]
        public void CreateOpenItem_NoViewersForType_Disabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGuiCommandHandler>();
            guiMock.Expect(ch => ch.CanOpenDefaultViewForSelection()).Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock);

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
            commandHandlerMock.Expect(ch => ch.CanOpenDefaultViewForSelection()).Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

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
            commandHandlerMock.Expect(ch => ch.CanExportFromGuiSelection()).Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

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
            commandHandlerMock.Expect(ch => ch.CanExportFromGuiSelection()).Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

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
            commandHandlerMock.Expect(ch => ch.CanImportToGuiSelection()).Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

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
            commandHandlerMock.Expect(ch => ch.CanImportToGuiSelection()).Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

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
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesForGuiSelection()).Return(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

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
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesForGuiSelection()).Return(false);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

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
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesForGuiSelection()).Return(true);
            commandHandlerMock.Expect(ch => ch.ShowProperties());

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock);

            mocks.ReplayAll();

            var item = contextMenuFactory.CreatePropertiesItem();
            
            // Call & Assert
            item.PerformClick();
            
            mocks.VerifyAll();
        }
    }
}