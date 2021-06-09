﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Gui.Commands;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using Core.Gui.Properties;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.Commands
{
    [TestFixture]
    public class GuiExportHandlerTest : NUnitFormTest
    {
        [Test]
        [TestCase(1234)]
        [TestCase(null)]
        public void ExportFrom_NoExporterAvailable_GivesMessageBoxAndLogsMessage(object source)
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            string messageBoxText = null;
            string messageBoxTitle = null;

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>());

            // Call
            Action call = () => exportHandler.ExportFrom(source);

            // Assert
            string sourceTypeName = source == null ? "null" : source.GetType().FullName;
            TestHelper.AssertLogMessageIsGenerated(call, $"Riskeer kan de huidige selectie ({sourceTypeName}) niet exporteren.");
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Riskeer kan de huidige selectie niet exporteren.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(1234)]
        [TestCase(null)]
        public void ExportFrom_NoSupportedExporterAvailable_GivesMessageBoxAndLogsMessage(object source)
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            string messageBoxText = null;
            string messageBoxTitle = null;

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<string>()
            });

            // Call
            Action call = () => exportHandler.ExportFrom(source);

            // Assert
            string sourceTypeName = source == null ? "null" : source.GetType().FullName;
            TestHelper.AssertLogMessageIsGenerated(call, $"Riskeer kan de huidige selectie ({sourceTypeName}) niet exporteren.");
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Riskeer kan de huidige selectie niet exporteren.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExportFrom_SupportedExporterAvailableNoFilePathGiven_AbortsExport()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var exporter = mockRepository.StrictMock<IFileExporter>();
            mockRepository.ReplayAll();

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<int>
                {
                    CreateFileExporter = (o, s) => exporter,
                    GetExportPath = () => null
                }
            });

            // Call
            exportHandler.ExportFrom(1234);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on exporter mock
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExportFrom_SupportedExporterAvailableAndFilePathGivenAndExporterRunsSuccessful_CallsExportAndLogsMessages()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var exporter = mockRepository.StrictMock<IFileExporter>();
            exporter.Stub(e => e.Export()).Return(true);
            mockRepository.ReplayAll();

            const int expectedData = 1234;
            string targetExportFileName = Path.GetFullPath("exportFile.txt");

            const string exportInfoName = "Random data";
            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<int>
                {
                    Name = exportInfoName,
                    CreateFileExporter = (data, filePath) =>
                    {
                        Assert.AreEqual(expectedData, data);
                        Assert.AreEqual(targetExportFileName, filePath);
                        return exporter;
                    },
                    GetExportPath = () => targetExportFileName
                }
            });

            // Call
            Action call = () => exportHandler.ExportFrom(expectedData);

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                $"Exporteren van '{exportInfoName}' is gestart.",
                $"Gegevens zijn geëxporteerd naar bestand '{targetExportFileName}'.",
                $"Exporteren van '{exportInfoName}' is gelukt."
            });
            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExportFrom_SupportedExporterAvailableAndFilePathGivenAndExporterFails_CallsExportAndLogsMessages()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var exporter = mockRepository.StrictMock<IFileExporter>();
            exporter.Stub(e => e.Export()).Return(false);
            mockRepository.ReplayAll();

            string targetExportFileName = Path.GetFullPath("exportFile.txt");

            const string exportInfoName = "Random data";
            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<int>
                {
                    Name = exportInfoName,
                    CreateFileExporter = (data, filePath) => exporter,
                    GetExportPath = () => targetExportFileName
                }
            });

            // Call
            Action call = () => exportHandler.ExportFrom(1234);

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                $"Exporteren van '{exportInfoName}' is gestart.",
                $"Exporteren van '{exportInfoName}' is mislukt."
            });
            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExportFrom_MultipleSupportedExportersAvailableWithDefaultSelectionDialogStyling_GivesExpectedSelectionDialog()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            var dialogText = "";
            TestListViewItem[] listViewItems = null;

            ModalFormHandler = (name, wnd, form) =>
            {
                var dialog = new FormTester(name);
                var imageList = TypeUtils.GetField<ImageList>(dialog.TheObject, "imageList");
                var listView = (ListView) new ControlTester("listViewItemTypes").TheObject;

                dialogText = dialog.Text;
                listViewItems = listView.Items
                                        .OfType<ListViewItem>()
                                        .Select(lvi => new TestListViewItem(lvi.Name, lvi.Group.Name, imageList.Images[lvi.ImageIndex]))
                                        .ToArray();

                dialog.Close();
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<int>(),
                new ExportInfo<int>()
            });

            // Call
            exportHandler.ExportFrom(1234);

            // Assert
            Assert.AreEqual("Kies wat u wilt exporteren", dialogText);

            Assert.AreEqual(2, listViewItems.Length);
            Assert.AreEqual("", listViewItems[0].Name);
            Assert.AreEqual("Algemeen", listViewItems[0].Group);
            Assert.AreEqual("", listViewItems[1].Name);
            Assert.AreEqual("Algemeen", listViewItems[1].Group);

            mockRepository.VerifyAll();
        }

        [Test]
        public void CanExportFrom_HasNoFileExportersForTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var commandHandler = new GuiExportHandler(dialogParent, new List<ExportInfo>
            {
                new ExportInfo<int>(), // Wrong object type
                new ExportInfo<object> // Disabled
                {
                    IsEnabled = o => false
                }
            });

            // Call
            bool isExportPossible = commandHandler.CanExportFrom(new object());

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

            var commandHandler = new GuiExportHandler(dialogParent, new List<ExportInfo>
            {
                new ExportInfo<object>()
            });

            // Call
            bool isExportPossible = commandHandler.CanExportFrom(new object());

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

            var commandHandler = new GuiExportHandler(dialogParent, new List<ExportInfo>
            {
                new ExportInfo<object>(),
                new ExportInfo<object>()
            });

            // Call
            bool isExportPossible = commandHandler.CanExportFrom(new object());

            // Assert
            Assert.IsTrue(isExportPossible);
            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Apartment(ApartmentState.STA)]
        public void ExportFrom_MultipleSupportedExportersAvailableWithCustomSelectionDialogStyling_GivesExpectedSelectionDialog(bool hasFileExtension)
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            var dialogText = "";
            TestListViewItem[] listViewItems = null;

            ModalFormHandler = (name, wnd, form) =>
            {
                var dialog = new FormTester(name);
                var imageList = TypeUtils.GetField<ImageList>(dialog.TheObject, "imageList");
                var listView = (ListView) new ControlTester("listViewItemTypes").TheObject;

                dialogText = dialog.Text;
                listViewItems = listView.Items
                                        .OfType<ListViewItem>()
                                        .Select(lvi => new TestListViewItem(lvi.Name, lvi.Group.Name, imageList.Images[lvi.ImageIndex]))
                                        .ToArray();

                dialog.Close();
            };

            var exportInfo1 = new ExportInfo<int>
            {
                Name = "Name 1",
                Category = "Category 1",
                Image = Resources.Busy_indicator,
                Extension = hasFileExtension ? "extension 1" : null
            };

            var exportInfo2 = new ExportInfo<int>
            {
                Name = "Name 2",
                Category = "Category 2",
                Image = Resources.DeleteIcon,
                Extension = hasFileExtension ? "extension 2" : null
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                exportInfo1,
                exportInfo2
            });

            // Call
            exportHandler.ExportFrom(1234);

            // Assert
            Assert.AreEqual("Kies wat u wilt exporteren", dialogText);

            Assert.AreEqual(2, listViewItems.Length);
            string expectedItemName1 = hasFileExtension
                                           ? $"{exportInfo1.Name} (*.{exportInfo1.Extension})"
                                           : exportInfo1.Name;
            Assert.AreEqual(expectedItemName1, listViewItems[0].Name);
            Assert.AreEqual(exportInfo1.Category, listViewItems[0].Group);
            string expectedItemName2 = hasFileExtension
                                           ? $"{exportInfo2.Name} (*.{exportInfo2.Extension})"
                                           : exportInfo2.Name;
            Assert.AreEqual(expectedItemName2, listViewItems[1].Name);
            Assert.AreEqual(exportInfo2.Category, listViewItems[1].Group);

            mockRepository.VerifyAll();
        }

        private class TestListViewItem
        {
            public TestListViewItem(string name, string group, Image image)
            {
                Name = name;
                Group = group;
                Image = image;
            }

            public string Name { get; }

            public string Group { get; }

            public Image Image { get; }
        }
    }
}