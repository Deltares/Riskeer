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
using System.Threading;
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
    public class GuiUpdateHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_WithoutDialogParent_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GuiUpdateHandler(null, Enumerable.Empty<UpdateInfo>(), inquiryHelper);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutUpdateInfos_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GuiUpdateHandler(mainWindow, null, inquiryHelper);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("updateInfos", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanUpdateOn_HasNoFileUpdatersForTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiUpdateHandler(dialogParent, Enumerable.Empty<UpdateInfo>(), inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanUpdateOn(new object());

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanUpdateOn_HasOneUpdateInfoForTarget_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiUpdateHandler(dialogParent, new UpdateInfo[]
            {
                new UpdateInfo<object>()
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanUpdateOn_HasOneUpdateInfoForTargetThatIsNotEnabledForTarget_ReturnFalse()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiUpdateHandler(dialogParent, new UpdateInfo[]
            {
                new UpdateInfo<object>
                {
                    IsEnabled = data => false
                }
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanUpdateOn_HasMultipleUpdateInfosForTargetWhereAtLeastOneEnabledForTargetItem_ReturnTrue()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiUpdateHandler(dialogParent, new UpdateInfo[]
            {
                new UpdateInfo<object>
                {
                    IsEnabled = data => false
                },
                new UpdateInfo<object>
                {
                    IsEnabled = data => true
                }
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsTrue(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanUpdateOn_HasMultipleUpdateInfosForTargetThatCannotBeUsedForImporting_ReturnFalse()
        {
            // Setup
            var target = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiUpdateHandler(dialogParent, new UpdateInfo[]
            {
                new UpdateInfo<object>
                {
                    IsEnabled = data => false
                },
                new UpdateInfo<object>
                {
                    IsEnabled = data => false
                }
            }, inquiryHelper);

            // Call
            bool isImportPossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsFalse(isImportPossible);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateOn_NoUpdaterAvailable_GivesMessageBox()
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

            var importHandler = new GuiUpdateHandler(mainWindow, Enumerable.Empty<UpdateInfo>(), inquiryHelper);

            // Call
            importHandler.UpdateOn(3);

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Updater' is beschikbaar voor dit element.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_NoSupportedUpdateInfoAvailable_GivesMessageBox()
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

            var importHandler = new GuiUpdateHandler(mainWindow, new UpdateInfo[]
            {
                new UpdateInfo<double>()
            }, inquiryHelper);

            // Call
            importHandler.UpdateOn(string.Empty);

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Updater' is beschikbaar voor dit element.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_SupportedUpdateInfoAvailableVerifyUpdatesSuccesful_CreateFileImporterCalled()
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
                var importHandler = new GuiUpdateHandler(form, new UpdateInfo[]
                {
                    new UpdateInfo<double>
                    {
                        CreateFileImporter = (d, s) => fileImporterStub,
                        FileFilter = filter,
                        VerifyUpdates = d => true
                    }
                }, inquiryHelper);

                // Call
                importHandler.UpdateOn(3.2);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_SupportedUpdateInfoAvailableVerifyUpdatesUnsuccesful_FileImporterNotCreated()
        {
            // Setup
            var filter = new ExpectedFile();
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation(filter)).Return(new FileResult("/some/path"));
            mockRepository.ReplayAll();

            using (var form = new Form())
            {
                var importHandler = new GuiUpdateHandler(form, new UpdateInfo[]
                {
                    new UpdateInfo<double>
                    {
                        FileFilter = filter,
                        VerifyUpdates = d => false
                    }
                }, inquiryHelper);

                // Call
                importHandler.UpdateOn(3.2);
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