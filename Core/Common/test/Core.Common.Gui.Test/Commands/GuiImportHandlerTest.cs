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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class GuiImportHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_DialogParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new GuiImportHandler(null, Enumerable.Empty<ImportInfo>(), inquiryHelper);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dialogParent", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ImportInfosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new GuiImportHandler(mainWindow, null, inquiryHelper);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("importInfos", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new GuiImportHandler(mainWindow, Enumerable.Empty<ImportInfo>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSupportedImportInfos_NoImportInfos_ReturnsEmptyEnumeration()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, Enumerable.Empty<ImportInfo>(), inquiryHelper);

            // Call
            IEnumerable<ImportInfo> supportedImportInfos = commandHandler.GetSupportedImportInfos(new object());

            // Assert
            CollectionAssert.IsEmpty(supportedImportInfos);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSupportedImportInfos_NoImportInfosForTargetType_ReturnsEmptyEnumeration()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var commandHandler = new GuiImportHandler(dialogParent, new ImportInfo[]
            {
                new ImportInfo<TestClassA>(),
                new ImportInfo<TestClassB>()
            }, inquiryHelper);

            // Call
            IEnumerable<ImportInfo> supportedImportInfos = commandHandler.GetSupportedImportInfos(new TestClassC());

            // Assert
            CollectionAssert.IsEmpty(supportedImportInfos);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSupportedImportInfos_MultipleImportInfos_ReturnsEnumerationBasedOnTargetType()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var firstImportInfo = new ImportInfo<TestClassA>
            {
                Name = "1"
            };

            var secondImportInfo = new ImportInfo<TestClassB>
            {
                Name = "2"
            };

            var thirdImportInfo = new ImportInfo<TestClassC>
            {
                Name = "3"
            };

            var fourthImportInfo = new ImportInfo<TestClassB>
            {
                Name = "4"
            };

            var commandHandler = new GuiImportHandler(dialogParent, new ImportInfo[]
            {
                firstImportInfo,
                secondImportInfo,
                thirdImportInfo,
                fourthImportInfo
            }, inquiryHelper);

            // Call
            IEnumerable<ImportInfo> supportedImportInfos = commandHandler.GetSupportedImportInfos(new TestClassB());

            // Assert
            var expectedImportInfos = new List<ImportInfo>
            {
                firstImportInfo,
                secondImportInfo,
                fourthImportInfo
            };

            CollectionAssert.AreEqual(expectedImportInfos, supportedImportInfos, new ImportInfoNameComparer());
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void GetSupportedImportInfos_MultipleImportInfosForTargetType_ReturnsEnumerationBasedOnEnabledState(
            bool firstImportInfoEnabled,
            bool secondImportInfoEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var firstImportInfo = new ImportInfo<object>
            {
                Name = "1",
                IsEnabled = o => firstImportInfoEnabled
            };

            var secondImportInfo = new ImportInfo<object>
            {
                Name = "2",
                IsEnabled = o => secondImportInfoEnabled
            };

            var commandHandler = new GuiImportHandler(dialogParent, new ImportInfo[]
            {
                firstImportInfo,
                secondImportInfo
            }, inquiryHelper);

            // Call
            IEnumerable<ImportInfo> supportedImportInfos = commandHandler.GetSupportedImportInfos(new object());

            // Assert
            var expectedImportInfos = new List<ImportInfo>();

            if (firstImportInfoEnabled)
            {
                expectedImportInfos.Add(firstImportInfo);
            }

            if (secondImportInfoEnabled)
            {
                expectedImportInfos.Add(secondImportInfo);
            }

            CollectionAssert.AreEqual(expectedImportInfos, supportedImportInfos, new ImportInfoNameComparer());
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
            var generator = new FileFilterGenerator();
            var targetObject = new object();

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation(generator.Filter)).Return(filePath);
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
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    new ImportInfo<object>
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
                Action call = () => importHandler.ImportOn(targetObject);

                // Assert
                TestHelper.AssertLogMessagesAreGenerated(call, new[]
                {
                    $"Importeren van '{dataDescription}' is gestart.",
                    $"Importeren van '{dataDescription}' is mislukt."
                });
            }

            // Assert
            Assert.IsTrue(isCreateFileImporterCalled);
            Assert.IsTrue(isVerifyUpdatedCalled);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_InquiryHelperReturnsNoPath_ImportCancelledWithLogMessage()
        {
            // Setup
            var generator = new FileFilterGenerator();
            var targetObject = new object();

            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation(generator.Filter)).Return(null);
            var fileImporter = mockRepository.Stub<IFileImporter>();
            mockRepository.ReplayAll();

            using (var form = new Form())
            {
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    new ImportInfo<object>
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
                Action call = () => importHandler.ImportOn(targetObject);

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, "Importeren van gegevens is geannuleerd.");
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ImportOn_MultipleSupportedImportInfoAvailableVerifyUpdatesUnsuccessful_ActivityNotCreated()
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
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    new ImportInfo<object>
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
                importHandler.ImportOn(targetObject);
            }

            // Assert
            Assert.IsTrue(isVerifyUpdatedCalled);
            mockRepository.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportOn_MultipleSupportedImportInfoAvailable_ShowsDialogWithOptions(bool hasFileFilterGenerator)
        {
            // Setup
            const string importInfoAName = "nameA";
            var importInfoA = new ImportInfo<object>
            {
                Name = importInfoAName,
                FileFilterGenerator = hasFileFilterGenerator ? new FileFilterGenerator("extensionA") : null
            };
            const string importInfoBName = "nameB";
            var importInfoB = new ImportInfo<object>
            {
                Name = importInfoBName,
                FileFilterGenerator = hasFileFilterGenerator ? new FileFilterGenerator("extensionB") : null
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
                var importHandler = new GuiImportHandler(form, new ImportInfo[]
                {
                    importInfoA,
                    importInfoB
                }, inquiryHelper);

                // Call
                importHandler.ImportOn(new object());
            }

            // Assert
            Assert.AreEqual(2, listViewItems.Length);
            string expectedItemNameA = hasFileFilterGenerator
                                           ? $"{importInfoA.Name} (*.{importInfoA.FileFilterGenerator.Extension})"
                                           : importInfoA.Name;
            Assert.AreEqual(expectedItemNameA, listViewItems[0].Name);
            string expectedItemNameB = hasFileFilterGenerator
                                           ? $"{importInfoB.Name} (*.{importInfoB.FileFilterGenerator.Extension})"
                                           : importInfoB.Name;
            Assert.AreEqual(expectedItemNameB, listViewItems[1].Name);

            mockRepository.VerifyAll();
        }

        private class TestClassA {}

        private class TestClassB : TestClassA {}

        private class TestClassC {}

        private class ImportInfoNameComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return string.CompareOrdinal(((ImportInfo) x)?.Name, ((ImportInfo) y)?.Name);
            }
        }
    }
}