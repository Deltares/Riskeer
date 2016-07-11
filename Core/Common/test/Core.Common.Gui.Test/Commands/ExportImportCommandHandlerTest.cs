// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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