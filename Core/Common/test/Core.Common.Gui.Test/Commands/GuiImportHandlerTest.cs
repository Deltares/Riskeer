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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class GuiImportHandlerTest : NUnitFormTest
    {
        [Test]
        public void ImportOn_NoImporterAvailable_GivesMessageBox()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(mainWindow, Enumerable.Empty<IFileImporter>(), Enumerable.Empty<ImportInfo>());

            // Call
            importHandler.ImportOn(typeof(long));

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Importer' is beschikbaar voor dit element.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_NoSupportedImporterAvailable_GivesMessageBox()
        {
            // Setup
            var fileImporters = new List<IFileImporter>
            {
                new UnsupportedFileImporter()
            };
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(mainWindow, fileImporters, Enumerable.Empty<ImportInfo>());

            // Call
            importHandler.ImportOn(typeof(long));

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Importer' is beschikbaar voor dit element.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_NoSupportedImportInfoAvailable_GivesMessageBox()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(mainWindow, Enumerable.Empty<IFileImporter>(), new ImportInfo[]
            {
                new ImportInfo<double>()
            });

            // Call
            importHandler.ImportOn(typeof(long));

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Importer' is beschikbaar voor dit element.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasNoFileImportersForTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, Enumerable.Empty<IFileImporter>(), Enumerable.Empty<ImportInfo>());

            // Call
            bool isImportPossible = commandHandler.CanImportOn(new object());

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

            var commandHandler = new GuiImportHandler(dialogParent, fileImporters, Enumerable.Empty<ImportInfo>());

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasOneImportInfoForTarget_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, Enumerable.Empty<IFileImporter>(), new ImportInfo[]
            {
                new ImportInfo<object>()
            });

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

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

            var commandHandler = new GuiImportHandler(dialogParent, fileImporters, Enumerable.Empty<ImportInfo>());

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasOneImporterInfoForTargetThatIsNotEnabledForTarget_ReturnFalse()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, Enumerable.Empty<IFileImporter>(), new ImportInfo[]
            {
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                }
            });

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

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

            var commandHandler = new GuiImportHandler(dialogParent, fileImporters, Enumerable.Empty<ImportInfo>());

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasMultipleImportInfosForTargetWhereAtLeastOneEnabledForTargetItem_ReturnTrue()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, Enumerable.Empty<IFileImporter>(), new ImportInfo[]
            {
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                },
                new ImportInfo<object>
                {
                    IsEnabled = data => true
                }
            });

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

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

            var commandHandler = new GuiImportHandler(dialogParent, fileImporters, Enumerable.Empty<ImportInfo>());

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasMultipleImportInfosForTargetThatCannotBeUsedForImporting_ReturnFalse()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, Enumerable.Empty<IFileImporter>(), new ImportInfo[]
            {
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                },
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                }
            });

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        private class UnsupportedFileImporter : IFileImporter
        {
            public string Name { get; private set; }

            public string Category { get; private set; }

            public Bitmap Image { get; private set; }

            public string FileFilter { get; private set; }

            public ProgressChangedDelegate ProgressChanged { get; set; }

            public bool CanImportOn(object targetItem)
            {
                return false;
            }

            public bool Import(object targetItem, string filePath)
            {
                return false;
            }

            public void Cancel() {}

            public void DoPostImportUpdates(object targetItem) {}
        }
    }
}