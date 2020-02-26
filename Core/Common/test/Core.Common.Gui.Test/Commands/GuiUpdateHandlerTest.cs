// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Common.Util;
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
        public void Constructor_WithoutInquiryHelper_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GuiUpdateHandler(mainWindow, Enumerable.Empty<UpdateInfo>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("inquiryHelper", paramName);
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
            bool isUpdatePossible = commandHandler.CanUpdateOn(new object());

            // Assert
            Assert.IsFalse(isUpdatePossible);
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
            bool isUpdatePossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsTrue(isUpdatePossible);
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
            bool isUpdatePossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsFalse(isUpdatePossible);
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
            bool isUpdatePossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsTrue(isUpdatePossible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanUpdateOn_HasMultipleUpdateInfosForTargetThatCannotBeUsedForUpdating_ReturnFalse()
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
            bool isUpdatePossible = commandHandler.CanUpdateOn(target);

            // Assert
            Assert.IsFalse(isUpdatePossible);
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

            var updateHandler = new GuiUpdateHandler(mainWindow, Enumerable.Empty<UpdateInfo>(), inquiryHelper);

            // Call
            updateHandler.UpdateOn(3);

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

            var updateHandler = new GuiUpdateHandler(mainWindow, new UpdateInfo[]
            {
                new UpdateInfo<double>()
            }, inquiryHelper);

            // Call
            updateHandler.UpdateOn(string.Empty);

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Updater' is beschikbaar voor dit element.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_SupportedUpdateInfoAvailableVerifyUpdatesSuccessful_ExpectedUpdateInfoFunctionsCalledActivityCreated()
        {
            // Setup
            const string filePath = "/some/path";
            var generator = new FileFilterGenerator();
            var targetObject = new object();

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Stub(ih => ih.GetSourceFileLocation(generator.Filter)).Return(filePath);
            var fileImporter = mockRepository.Stub<IFileImporter>();
            mockRepository.ReplayAll();

            const string dataDescription = "Random data";
            var isCreateFileImporterCalled = false;
            var isVerifyUpdatedCalled = false;

            DialogBoxHandler = (name, wnd) =>
            {
                // Activity closes itself
            };

            using (var form = new Form())
            {
                var updateHandler = new GuiUpdateHandler(form, new UpdateInfo[]
                {
                    new UpdateInfo<object>
                    {
                        Name = dataDescription,
                        CreateFileImporter = (o, s) =>
                        {
                            Assert.AreSame(o, targetObject);
                            Assert.AreEqual(filePath, s);
                            isCreateFileImporterCalled = true;
                            return fileImporter;
                        },
                        FileFilterGenerator = generator,
                        VerifyUpdates = o =>
                        {
                            Assert.AreSame(o, targetObject);
                            isVerifyUpdatedCalled = true;
                            return true;
                        }
                    }
                }, inquiryHelper);

                // Call
                Action call = () => updateHandler.UpdateOn(targetObject);

                // Assert
                TestHelper.AssertLogMessagesAreGenerated(call, new[]
                {
                    $"Bijwerken van '{dataDescription}' is gestart.",
                    $"Bijwerken van '{dataDescription}' is mislukt."
                });
            }

            // Assert
            Assert.IsTrue(isCreateFileImporterCalled);
            Assert.IsTrue(isVerifyUpdatedCalled);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_InquiryHelperReturnsNoPathAndCurrentPathNotSet_UpdateCancelledWithLogMessage()
        {
            // Setup
            var generator = new FileFilterGenerator();
            var targetObject = new object();

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Stub(ih => ih.GetSourceFileLocation(generator.Filter)).Return(null);
            var fileImporter = mockRepository.Stub<IFileImporter>();
            mockRepository.ReplayAll();

            using (var form = new Form())
            {
                var updateHandler = new GuiUpdateHandler(form, new UpdateInfo[]
                {
                    new UpdateInfo<object>
                    {
                        CreateFileImporter = (o, s) =>
                        {
                            Assert.Fail("CreateFileImporter is not expected to be called when no file path is chosen.");
                            return fileImporter;
                        },
                        FileFilterGenerator = generator,
                        VerifyUpdates = o => true
                    }
                }, inquiryHelper);

                // Call
                Action call = () => updateHandler.UpdateOn(targetObject);

                // Assert
                const string expectedLogMessage = "Bijwerken van gegevens is geannuleerd.";
                Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage,
                                                                                          LogLevelConstant.Info);
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_InquiryHelperReturnsNoPathAndCurrentPathSet_UpdateCancelledWithLogMessage()
        {
            // Setup
            var generator = new FileFilterGenerator();
            var targetObject = new object();

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Stub(ih => ih.GetSourceFileLocation(generator.Filter)).Return(null);
            var fileImporter = mockRepository.Stub<IFileImporter>();
            mockRepository.ReplayAll();

            const string currentPath = "FilePath/to/Update";
            using (var form = new Form())
            {
                var updateHandler = new GuiUpdateHandler(form, new UpdateInfo[]
                {
                    new UpdateInfo<object>
                    {
                        CreateFileImporter = (o, s) =>
                        {
                            Assert.Fail("CreateFileImporter is not expected to be called when no file path is chosen.");
                            return fileImporter;
                        },
                        FileFilterGenerator = generator,
                        VerifyUpdates = o => true,
                        CurrentPath = o => currentPath
                    }
                }, inquiryHelper);

                // Call
                Action call = () => updateHandler.UpdateOn(targetObject);

                // Assert
                string expectedLogMessage = $"Bijwerken van gegevens in '{currentPath}' is geannuleerd.";
                Tuple<string, LogLevelConstant> expectedLogMessageAndLevel = Tuple.Create(expectedLogMessage,
                                                                                          LogLevelConstant.Info);
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessageAndLevel);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_SupportedUpdateInfoAvailableVerifyUpdatesUnsuccessful_ActivityNotCreated()
        {
            // Setup
            var generator = new FileFilterGenerator();
            var targetObject = new object();
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Stub(ih => ih.GetSourceFileLocation(generator.Filter)).Return("/some/path");
            var fileImporter = mockRepository.Stub<IFileImporter>();
            mockRepository.ReplayAll();

            var isVerifyUpdatedCalled = false;

            using (var form = new Form())
            {
                var updateHandler = new GuiUpdateHandler(form, new UpdateInfo[]
                {
                    new UpdateInfo<object>
                    {
                        CreateFileImporter = (o, s) =>
                        {
                            Assert.Fail("CreateFileImporter is not expected to be called when VerifyUpdates function returns false.");
                            return fileImporter;
                        },
                        FileFilterGenerator = generator,
                        VerifyUpdates = o =>
                        {
                            Assert.AreSame(o, targetObject);
                            isVerifyUpdatedCalled = true;
                            return false;
                        }
                    }
                }, inquiryHelper);

                // Call
                updateHandler.UpdateOn(targetObject);
            }

            // Assert
            Assert.IsTrue(isVerifyUpdatedCalled);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateOn_MultipleSupportedUpdateInfoAvailable_ShowsDialogWithOptions()
        {
            // Setup
            const string updateInfoAName = "nameA";
            var updateInfoA = new UpdateInfo<object>
            {
                Name = updateInfoAName
            };
            const string updateInfoBName = "nameB";
            var updateInfoB = new UpdateInfo<object>
            {
                Name = updateInfoBName
            };

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            var listViewItems = new ListViewItem[0];

            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    var listView = (ListView) new ControlTester("listViewItemTypes").TheObject;
                    listViewItems = listView.Items.OfType<ListViewItem>().ToArray();
                }
            };

            using (var form = new Form())
            {
                var updateHandler = new GuiUpdateHandler(form, new UpdateInfo[]
                {
                    updateInfoA,
                    updateInfoB
                }, inquiryHelper);

                // Call
                updateHandler.UpdateOn(new object());
            }

            // Assert
            Assert.AreEqual(2, listViewItems.Length);
            Assert.AreEqual(updateInfoAName, listViewItems[0].Name);
            Assert.AreEqual(updateInfoBName, listViewItems[1].Name);
            mockRepository.VerifyAll();
        }
    }
}