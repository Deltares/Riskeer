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

using System;
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
        public void Constructor_WithoutDialogParent_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GuiImportHandler(null, Enumerable.Empty<ImportInfo>(), inquiryHelper);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutImportInfos_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GuiImportHandler(mainWindow, null, inquiryHelper);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importInfos", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutInquiryHelper_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GuiImportHandler(mainWindow, Enumerable.Empty<ImportInfo>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("inquiryHelper", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasNoFileImportersForTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, Enumerable.Empty<ImportInfo>(), inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanImportOn(new object());

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasOneImportInfoForTarget_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, new ImportInfo[]
            {
                new ImportInfo<object>()
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasOneImportInfoForTargetThatIsNotEnabledForTarget_ReturnFalse()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, new ImportInfo[]
            {
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                }
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasMultipleImportInfosForTargetWhereAtLeastOneEnabledForTargetItem_ReturnTrue()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, new ImportInfo[]
            {
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                },
                new ImportInfo<object>
                {
                    IsEnabled = data => true
                }
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_HasMultipleImportInfosForTargetThatCannotBeUsedForImporting_ReturnFalse()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, new ImportInfo[]
            {
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                },
                new ImportInfo<object>
                {
                    IsEnabled = data => false
                }
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanImportOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void ImportOn_NoImporterAvailable_GivesMessageBox()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(mainWindow, Enumerable.Empty<ImportInfo>(), inquiryHelper);

            // Call
            importHandler.ImportOn(3);

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
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(mainWindow, new ImportInfo[]
            {
                new ImportInfo<double>()
            }, inquiryHelper);

            // Call
            importHandler.ImportOn(string.Empty);

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Importer' is beschikbaar voor dit element.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_SupportedImportInfoAvailableVerifyUpdatesSuccesful_CreateFileImporterCalled()
        {
            // Setup
            var filter = new ExpectedFile();
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation(filter)).Return(new FileResult("/some/path"));
            IFileImporter fileImporterStub = CreateStubFileImporter(mockRepository);
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity closes itself
            };

            using (var form = new Form())
            {
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    new ImportInfo<double>
                    {
                        CreateFileImporter = (d, s) => fileImporterStub,
                        FileFilter = filter,
                        VerifyUpdates = d => true
                    }
                }, inquiryHelper);

                // Call
                importHandler.ImportOn(3.2);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_SupportedImportInfoAvailableVerifyUpdatesUnsuccesful_FileImporterNotCreated()
        {
            // Setup
            var filter = new ExpectedFile();
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation(filter)).Return(new FileResult("/some/path"));
            mockRepository.ReplayAll();

            using (var form = new Form())
            {
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    new ImportInfo<double>
                    {
                        FileFilter = filter,
                        VerifyUpdates = d => false
                    }
                }, inquiryHelper);

                // Call
                importHandler.ImportOn(3.2);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        private static IFileImporter CreateStubFileImporter(MockRepository mockRepository)
        {
            var fileImporterStub = mockRepository.Stub<IFileImporter>();
            fileImporterStub.Expect(fi => fi.Import());
            fileImporterStub.Expect(fi => fi.DoPostImport());
            fileImporterStub.Expect(fi => fi.SetProgressChanged(null)).IgnoreArguments();
            return fileImporterStub;
        }
    }
}