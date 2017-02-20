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
        public void ImportOn_SupportedImportInfoAvailableVerifyUpdatesSuccessful_ExpectedImportInfoFunctionsCalledActivityCreated()
        {
            // Setup
            const string filePath = "/some/path";
            var filter = new FileFilterGenerator();
            var targetObject = new object();

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation(filter)).Return(filePath);
            var fileImporter = mockRepository.Stub<IFileImporter>();
            mockRepository.ReplayAll();

            var isCreateFileImporterCalled = false;
            var isVerifyUpdatedCalled = false;

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity closes itself
            };

            using (var form = new Form())
            {
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    new ImportInfo<object>
                    {
                        CreateFileImporter = (o, s) =>
                        {
                            Assert.AreSame(o, targetObject);
                            Assert.AreEqual(filePath, s);
                            isCreateFileImporterCalled = true;
                            return fileImporter;
                        },
                        FileFilter = filter,
                        VerifyUpdates = o =>
                        {
                            Assert.AreSame(o, targetObject);
                            isVerifyUpdatedCalled = true;
                            return true;
                        }
                    }
                }, inquiryHelper);

                // Call
                importHandler.ImportOn(targetObject);
            }

            // Assert
            Assert.IsTrue(isCreateFileImporterCalled);
            Assert.IsTrue(isVerifyUpdatedCalled);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_MultipleSupportedImportInfoAvailableVerifyUpdatesUnsuccesful_ActivityNotCreated()
        {
            // Setup
            var filter = new FileFilterGenerator();
            var targetObject = new object();

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Stub(ih => ih.GetSourceFileLocation(filter)).Return("/some/path");
            var fileImporter = mockRepository.Stub<IFileImporter>();
            mockRepository.ReplayAll();

            var isVerifyUpdatedCalled = false;

            using (var form = new Form())
            {
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    new ImportInfo<object>
                    {
                        CreateFileImporter = (o, s) =>
                        {
                            Assert.Fail("CreateFileImporter is not expected to be called when VerifyUpdates function returns false.");
                            return fileImporter;
                        },
                        FileFilter = filter,
                        VerifyUpdates = o =>
                        {
                            Assert.AreSame(o, targetObject);
                            isVerifyUpdatedCalled = true;
                            return false;
                        }
                    }
                }, inquiryHelper);

                // Call
                importHandler.ImportOn(targetObject);
            }

            // Assert
            Assert.IsTrue(isVerifyUpdatedCalled);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_MultipleSupportedImportInfoAvailable_ShowsDialogWithOptions()
        {
            // Setup
            const string importInfoAName = "nameA";
            var importInfoA = new ImportInfo<object>
            {
                Name = importInfoAName
            };
            const string importInfoBName = "nameB";
            var importInfoB = new ImportInfo<object>
            {
                Name = importInfoBName
            };

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var formTester = new FormTester(name);
                var listView = (ListView) new ControlTester("listViewItemTypes").TheObject;
                try
                {
                    Assert.AreEqual(2, listView.Items.Count);
                    Assert.AreEqual(importInfoAName, listView.Items[0].Name);
                    Assert.AreEqual(importInfoBName, listView.Items[1].Name);
                }
                finally
                {
                    formTester.Close();
                }
            };

            using (var form = new Form())
            {
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    importInfoA,
                    importInfoB
                }, inquiryHelper);

                // Call
                importHandler.ImportOn(new object());
            }

            // Assert
            mockRepository.VerifyAll();
        }
    }
}