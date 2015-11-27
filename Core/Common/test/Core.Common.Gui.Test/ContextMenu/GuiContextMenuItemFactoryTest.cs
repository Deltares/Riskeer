using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Tests.ContextMenu
{
    [TestFixture]
    public class ContextMenuItemFactoryTest
    {
        private MockRepository mocks;
        private readonly IList<GuiPlugin> pluginList = new GuiPlugin[]
        {
            new TestGuiPlugin()
        };

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutGui_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(null, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui, message);
            StringAssert.EndsWith("gui", message);

        }

        [Test]
        public void Constructor_WithoutTreeNode_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(mocks.StrictMock<IGui>(), null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node, message);
            StringAssert.EndsWith("treeNode", message);
        }

        [Test]
        public void Constructor_WithGuiAndTreeNode_NewInstance()
        {
            // Call
            var result = new GuiContextMenuItemFactory(mocks.StrictMock<IGui>(), mocks.StrictMock<ITreeNode>());

            // Assert
            Assert.IsInstanceOf<GuiContextMenuItemFactory>(result);
        }

        [Test]
        public void CreateOpenItem_NoViewersForType_Disabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            treeNodeMock.Tag = "";
            guiMock.Expect(g => g.Plugins).Return(pluginList);

            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
        public void CreateOpentem_ImportersForType_Enabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            treeNodeMock.Tag = 0;
            guiMock.Expect(g => g.Plugins).Return(pluginList);

            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            var applicationCore = new ApplicationCore();
            var data = 0;
            treeNodeMock.Tag = data;

            var testApplicationPlugin = new TestApplicationPlugin();
            testApplicationPlugin.ExporterMock = mocks.StrictMock<IFileExporter>();
            testApplicationPlugin.ExporterMock.Expect(e => e.SourceTypes()).Return(new Type[0]);

            applicationCore.AddPlugin(testApplicationPlugin);
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);

            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            var applicationCore = new ApplicationCore();
            var data = 0;
            treeNodeMock.Tag = data;

            var testApplicationPlugin = new TestApplicationPlugin();
            testApplicationPlugin.ExporterMock = mocks.StrictMock<IFileExporter>();
            testApplicationPlugin.ExporterMock.Expect(e => e.SourceTypes()).Return(new[] { data.GetType() });
            testApplicationPlugin.ExporterMock.Expect(e => e.CanExportFor(data)).Return(true);

            applicationCore.AddPlugin(testApplicationPlugin);
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);

            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            var applicationCore = new ApplicationCore();
            var data = 0;
            treeNodeMock.Tag = data;

            var testApplicationPlugin = new TestApplicationPlugin();
            testApplicationPlugin.ImporterMock = mocks.StrictMock<IFileImporter>();
            testApplicationPlugin.ImporterMock.Expect(e => e.SupportedItemTypes).Return(new Type[0]) ;

            applicationCore.AddPlugin(testApplicationPlugin);
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);
            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            var applicationCore = new ApplicationCore();
            var data = 0;
            treeNodeMock.Tag = data;

            var testApplicationPlugin = new TestApplicationPlugin();
            testApplicationPlugin.ImporterMock = mocks.StrictMock<IFileImporter>();
            testApplicationPlugin.ImporterMock.Expect(e => e.SupportedItemTypes).Return(new[] { data.GetType() });
            testApplicationPlugin.ImporterMock.Expect(e => e.CanImportOn(data)).Return(true);

            applicationCore.AddPlugin(testApplicationPlugin);
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);
            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            treeNodeMock.Tag = 0;
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(null);
            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            treeNodeMock.Tag = "";
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(null);
            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

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
        public void CreatePropertiesItem_ClickWithoutHandler_NoExceptions()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            treeNodeMock.Tag = 0;
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(null);
            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

            mocks.ReplayAll();

            var item = contextMenuFactory.CreatePropertiesItem();
            
            // Call & Assert
            item.PerformClick();
            
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_ClickWithHandler_NoExceptions()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeNodeMock = mocks.Stub<ITreeNode>();
            treeNodeMock.Tag = 0;
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(commandHandlerMock);
            commandHandlerMock.Expect(ch => ch.ShowProperties());
            var contextMenuFactory = new GuiContextMenuItemFactory(guiMock, treeNodeMock);

            mocks.ReplayAll();

            var item = contextMenuFactory.CreatePropertiesItem();
            
            // Call & Assert
            item.PerformClick();
            
            mocks.VerifyAll();
        }
    }

    public class TestApplicationPlugin : ApplicationPlugin
    {
        public IFileExporter ExporterMock { get; set; }
        public IFileImporter ImporterMock { get; set; }
        
        public override IEnumerable<IFileExporter> GetFileExporters()
        {
            yield return ExporterMock;
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return ImporterMock;
        }
    }

    public class TestGuiPlugin : GuiPlugin
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo
            {
                ObjectType = typeof(int)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo
            {
                DataType = typeof(int)
            };
        }
    }
}