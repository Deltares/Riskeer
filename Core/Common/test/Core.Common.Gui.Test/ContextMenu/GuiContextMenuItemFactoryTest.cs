using System;
using System.Windows.Forms;

using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
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
            TestDelegate test = () => new GuiContextMenuItemFactory(null, null,  null, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui, message);
            StringAssert.EndsWith("applicationFeatureCommandHandler", message);
        }

        [Test]
        public void Constructor_WithoutTreeNode_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandler = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, exportImportCommandHandler, viewCommandsMock, null);
        
            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node, message);
            StringAssert.EndsWith("treeNode", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutExportImportHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, null, viewCommandsMock, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_exportImport_handler, message);
            StringAssert.EndsWith("exportImportCommandHandler", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutViewCommandsHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, exportImportCommandHandlerMock, null, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_view_commands, message);
            StringAssert.EndsWith("viewCommandsHandler", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithAllInput_DoesNotThrow()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeNodeMock = mocks.Stub<TreeNode>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, exportImportCommandHandlerMock, viewCommandsMock, treeNodeMock);

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
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            var nodeStub = mocks.Stub<TreeNode>();
            viewCommandsMock.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(hasViewersForNodeData);
            if (hasViewersForNodeData)
            {
                viewCommandsMock.Expect(ch => ch.OpenView(nodeData));
            }

            mocks.ReplayAll();

            nodeStub.Tag = nodeData;

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeStub);

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
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            var nodeStub = mocks.Stub<TreeNode>();
            exportImportCommandHandlerMock.Expect(ch => ch.CanExportFrom(nodeData)).Return(hasExportersForNodeData);
            if (hasExportersForNodeData)
            {
                exportImportCommandHandlerMock.Expect(ch => ch.ExportFrom(nodeData));
            }

            mocks.ReplayAll();

            nodeStub.Tag = nodeData;

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeStub);

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
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            var nodeStub = mocks.Stub<TreeNode>();
            exportImportCommandHandlerMock.Expect(ch => ch.CanImportOn(nodeData)).Return(hasImportersForNodeData);
            if (hasImportersForNodeData)
            {
                exportImportCommandHandlerMock.Expect(ch => ch.ImportOn(nodeData));
            }

            mocks.ReplayAll();

            nodeStub.Tag = nodeData;

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeStub);

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
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            var nodeStub = mocks.Stub<TreeNode>();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(hasPropertyInfoForNodeData);
            if (hasPropertyInfoForNodeData)
            {
                commandHandlerMock.Expect(ch => ch.ShowPropertiesFor(nodeData));
            }

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeStub);

            mocks.ReplayAll();

            nodeStub.Tag = nodeData;

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