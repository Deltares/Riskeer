using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Tests
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
        public void CreateExportItem_NoImportersForType_Disabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(new TestApplicationPlugin(mocks));
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateExportItem("");

            // Assert
            Assert.AreEqual(Properties.Resources.Export, item.Text);
            Assert.AreEqual(Properties.Resources.Export_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Properties.Resources.ExportIcon, item.Image);
            Assert.IsFalse(item.Enabled);
        }

        [Test]
        public void CreateExportItem_ImportersForType_Enabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(new TestApplicationPlugin(mocks));
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateExportItem(0);

            // Assert
            Assert.AreEqual(Properties.Resources.Export, item.Text);
            Assert.AreEqual(Properties.Resources.Export_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Properties.Resources.ExportIcon, item.Image);
            Assert.IsTrue(item.Enabled);
        }

        [Test]
        public void CreateImportItem_NoImportersForType_Disabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(new TestApplicationPlugin(mocks));
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateImportItem("");

            // Assert
            Assert.AreEqual(Properties.Resources.Import, item.Text);
            Assert.AreEqual(Properties.Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Properties.Resources.ImportIcon, item.Image);
            Assert.IsFalse(item.Enabled);
        }

        [Test]
        public void CreateImportItem_ImportersForType_Enabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(new TestApplicationPlugin(mocks));
            guiMock.Expect(g => g.ApplicationCore).Return(applicationCore);
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreateImportItem(0);

            // Assert
            Assert.AreEqual(Properties.Resources.Import, item.Text);
            Assert.AreEqual(Properties.Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Properties.Resources.ImportIcon, item.Image);
            Assert.IsTrue(item.Enabled);
        }

        [Test]
        public void CreatePropertiesItem_PropertieInfoForType_Enabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(null);
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreatePropertiesItem(0);

            // Assert
            Assert.AreEqual(Properties.Resources.Properties, item.Text);
            Assert.AreEqual(Properties.Resources.Properties_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Properties.Resources.PropertiesIcon, item.Image);
            Assert.IsTrue(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_NoPropertieInfoForType_Disabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(null);
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreatePropertiesItem("");

            // Assert
            Assert.AreEqual(Properties.Resources.Properties, item.Text);
            Assert.AreEqual(Properties.Resources.Properties_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Properties.Resources.PropertiesIcon, item.Image);
            Assert.IsFalse(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_ClickWithoutHandler_NoExceptions()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(null);
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            var item = contextMenuFactory.CreatePropertiesItem(0);
            
            // Call & Assert
            item.PerformClick();
            
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_ClickWithHandler_NoExceptions()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            guiMock.Expect(g => g.Plugins).Return(pluginList);
            guiMock.Expect(g => g.CommandHandler).Return(commandHandlerMock);
            commandHandlerMock.Expect(ch => ch.ShowProperties());
            var contextMenuFactory = new ContextMenuItemFactory(guiMock);

            mocks.ReplayAll();

            var item = contextMenuFactory.CreatePropertiesItem(0);
            
            // Call & Assert
            item.PerformClick();
            
            mocks.VerifyAll();
        }
    }

    public class TestApplicationPlugin : ApplicationPlugin
    {
        private readonly IFileExporter exporterMock;
        private readonly IFileImporter importerMock;

        public TestApplicationPlugin(MockRepository mocks)
        {
            var typeList = new[]
            {
                0.GetType(),
                "".GetType()
            };
            exporterMock = mocks.DynamicMock<IFileExporter>();
            exporterMock.Expect(e => e.CanExportFor(0)).Return(true);
            exporterMock.Expect(e => e.CanExportFor("")).Return(false);
            exporterMock.Expect(e => e.SourceTypes()).Return(typeList);
            importerMock = mocks.DynamicMock<IFileImporter>();
            importerMock.Expect(e => e.CanImportOn(0)).Return(true);
            importerMock.Expect(e => e.CanImportOn("")).Return(false);
            importerMock.Expect(e => e.SupportedItemTypes).Return(typeList);
        }

        public override IEnumerable<IFileExporter> GetFileExporters()
        {
            yield return exporterMock;
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return importerMock;
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
    }
}