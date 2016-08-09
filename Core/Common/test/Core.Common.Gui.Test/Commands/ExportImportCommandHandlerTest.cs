﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
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
            var fileImporters = Enumerable.Empty<IFileImporter>();
            var exportInfos = Enumerable.Empty<ExportInfo>();
            mocks.ReplayAll();

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, exportInfos);

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
            mocks.ReplayAll();

            var fileImporters = new List<IFileImporter>
            {
                objectImporter
            };
            var exportInfos = Enumerable.Empty<ExportInfo>();

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, exportInfos);

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
            mocks.ReplayAll();

            var fileImporters = new List<IFileImporter>
            {
                objectImporter
            };
            var exportInfos = Enumerable.Empty<ExportInfo>();

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, exportInfos);

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
            mocks.ReplayAll();

            var fileImporters = new List<IFileImporter>
            {
                objectImporter1, objectImporter2
            };
            var exportInfos = Enumerable.Empty<ExportInfo>();

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, exportInfos);

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
            mocks.ReplayAll();

            var fileImporters = new List<IFileImporter>
            {
                objectImporter1, objectImporter2
            };
            var exportInfos = Enumerable.Empty<ExportInfo>();

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, exportInfos);

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

            var fileImporters = Enumerable.Empty<IFileImporter>();
            var exportInfos = Enumerable.Empty<ExportInfo>();

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, exportInfos);

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
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var fileImporters = Enumerable.Empty<IFileImporter>();
            var exportInfos = new List<ExportInfo>
            {
                new ExportInfo<object>()
            };

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, exportInfos);

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
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var fileImporters = Enumerable.Empty<IFileImporter>();
            var fileExporters = new List<ExportInfo>
            {
                new ExportInfo<object>(),
                new ExportInfo<object>()
            };

            var commandHandler = new ExportImportCommandHandler(dialogParent, fileImporters, fileExporters);

            // Call
            var isExportPossible = commandHandler.CanExportFrom(new object());

            // Assert
            Assert.IsTrue(isExportPossible);
            mocks.VerifyAll();
        }
    }
}