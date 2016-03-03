using System.Windows.Forms;

using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.Gui.Commands;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class ExportImportCommandHandlerTest
    {
        [Test]
        public void CanImportOn_HasNoFileImportersForTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isImportPossible = commandHandler.CanImportOn(new object());

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasOneFileImporterForTarget_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var objectImporter = mocks.Stub<IFileImporter>();
            objectImporter.Stub(i => i.CanImportOn(target)).Return(true);

            var objectApplicationPluginMock = mocks.Stub<ApplicationPlugin>();
            objectApplicationPluginMock.Stub(p => p.Activate());
            objectApplicationPluginMock.Expect(p => p.GetFileImporters())
                                       .Return(new[]
                                       {
                                           objectImporter
                                       });
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(objectApplicationPluginMock);

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasOneFileImporterForTargetThatCannotImportOnTarget_ReturnFalse()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var objectImporter = mocks.Stub<IFileImporter>();
            objectImporter.Stub(i => i.CanImportOn(target)).Return(false);

            var objectApplicationPluginMock = mocks.Stub<ApplicationPlugin>();
            objectApplicationPluginMock.Stub(p => p.Activate());
            objectApplicationPluginMock.Expect(p => p.GetFileImporters())
                                       .Return(new[]
                                       {
                                           objectImporter
                                       });
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(objectApplicationPluginMock);

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasMultipleFileImporterForTargetWhereAtLeastOneCanHandleTargetItem_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var objectImporter1 = mocks.Stub<IFileImporter>();
            objectImporter1.Stub(i => i.CanImportOn(target)).Return(false);
            var objectImporter2 = mocks.Stub<IFileImporter>();
            objectImporter2.Stub(i => i.CanImportOn(target)).Return(true);

            var objectApplicationPluginMock = mocks.Stub<ApplicationPlugin>();
            objectApplicationPluginMock.Stub(p => p.Activate());
            objectApplicationPluginMock.Expect(p => p.GetFileImporters())
                                       .Return(new[]
                                       {
                                           objectImporter1,
                                           objectImporter2
                                       });
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(objectApplicationPluginMock);

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasMultipleFileImporterForTargetThatCannotBeUsedForImporting_ReturnFalse()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var objectImporter1 = mocks.Stub<IFileImporter>();
            objectImporter1.Stub(i => i.CanImportOn(target)).Return(false);
            var objectImporter2 = mocks.Stub<IFileImporter>();
            objectImporter2.Stub(i => i.CanImportOn(target)).Return(false);

            var objectApplicationPluginMock = mocks.Stub<ApplicationPlugin>();
            objectApplicationPluginMock.Stub(p => p.Activate());
            objectApplicationPluginMock.Expect(p => p.GetFileImporters())
                                       .Return(new[]
                                       {
                                           objectImporter1,
                                           objectImporter2
                                       });
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(objectApplicationPluginMock);

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanExportFrom_HasNoFileExportersForTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isExportPossible = commandHandler.CanExportFrom(new object());

            // Assert
            Assert.IsFalse(isExportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanExportFrom_HasOneFileExporterForTarget_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var objectExporter = mocks.Stub<IFileExporter>();
            objectExporter.Stub(i => i.SupportedItemType)
                          .Return(target.GetType());

            var objectApplicationPluginMock = mocks.Stub<ApplicationPlugin>();
            objectApplicationPluginMock.Stub(p => p.Activate());
            objectApplicationPluginMock.Expect(p => p.GetFileExporters())
                                       .Return(new[]
                                       {
                                           objectExporter
                                       });
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(objectApplicationPluginMock);

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isExportPossible = commandHandler.CanExportFrom(new object());

            // Assert
            Assert.IsTrue(isExportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanExportFrom_HasMultipleFileExportersForTarget_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var objectExporter1 = mocks.Stub<IFileExporter>();
            objectExporter1.Stub(i => i.SupportedItemType)
                           .Return(target.GetType());
            var objectExporter2 = mocks.Stub<IFileExporter>();
            objectExporter2.Stub(i => i.SupportedItemType)
                           .Return(target.GetType());

            var objectApplicationPluginMock = mocks.Stub<ApplicationPlugin>();
            objectApplicationPluginMock.Stub(p => p.Activate());
            objectApplicationPluginMock.Expect(p => p.GetFileExporters())
                                       .Return(new[]
                                       {
                                           objectExporter1,
                                           objectExporter2
                                       });
            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            applicationCore.AddPlugin(objectApplicationPluginMock);

            var commandHandler = new ExportImportCommandHandler(dialogParent, applicationCore);

            // Call
            var isExportPossible = commandHandler.CanExportFrom(new object());

            // Assert
            Assert.IsTrue(isExportPossible);
            mocks.VerifyAll();
        }
    }
}