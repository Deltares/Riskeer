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
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Base.IO;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class GuiExportHandlerTest : NUnitFormTest
    {
        [Test]
        [TestCase(1234)]
        [TestCase(null)]
        public void ExportFrom_NoExporterAvailable_GivesMessageBoxAndLogsMessage(object exportFrom)
        {
            // Setup
            var fileImporters = new List<IFileExporter>();
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

            var exportHandler = new GuiExportHandler(mainWindow, fileImporters);

            // Call
            Action call = () => exportHandler.ExportFrom(exportFrom);

            // Assert
            var itemToExportType = exportFrom == null ? "null" : exportFrom.GetType().FullName;
            TestHelper.AssertLogMessageIsGenerated(call, string.Format("Ringtoets kan de huidige selectie ({0}) niet exporteren.", itemToExportType));
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Ringtoets kan de huidige selectie niet exporteren.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ExportFrom_NoSupportedExporterAvailable_GivesMessageBoxAndLogsMessage()
        {
            // Setup
            var mockRepository = new MockRepository();
            var unsupportedFileExporter = mockRepository.Stub<IFileExporter>();
            unsupportedFileExporter.Stub(i => i.SupportedItemType).Return(typeof(String)); // Wrong type
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

            var fileImporters = new List<IFileExporter>
            {
                unsupportedFileExporter
            };
            var exportHandler = new GuiExportHandler(mainWindow, fileImporters);

            // Call
            Action call = () => exportHandler.ExportFrom(null);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, string.Format("Ringtoets kan de huidige selectie ({0}) niet exporteren.", "null"));
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Ringtoets kan de huidige selectie niet exporteren.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ExportFrom_SupportedExporterAvailableCancelClicked_AbortsExportAndLogsMessage()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.ClickCancel();
            };

            var fileImporters = new List<IFileExporter>
            {
                new IntegerFileExporter()
            };
            var exportHandler = new GuiExportHandler(mainWindow, fileImporters);

            // Call
            Action call = () => exportHandler.ExportFrom(1234);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Exporteren gestart.");
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ExportFrom_SupportedExporterAvailableSaveClicked_ExportsAndLogsMessages()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            var exportFile = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Gui, "exportFile.txt");
            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.SaveFile(exportFile);
            };

            var fileImporters = new List<IFileExporter>
            {
                new IntegerFileExporter()
            };

            var exportHandler = new GuiExportHandler(mainWindow, fileImporters);

            // Call
            Action call = () => exportHandler.ExportFrom(1234);

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                "Exporteren gestart.",
                "Exporteren afgerond."
            });
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ExportFrom_SupportedAndUnsupportedExporterAvailableSaveClicked_ExportsAndLogsMessages()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var unsupportedFileExporter = mockRepository.Stub<IFileExporter>();
            unsupportedFileExporter.Stub(i => i.SupportedItemType).Return(typeof(String)); // Wrong type
            mockRepository.ReplayAll();

            var exportFile = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Gui, "exportFile.txt");
            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.SaveFile(exportFile);
            };

            var fileImporters = new List<IFileExporter>
            {
                unsupportedFileExporter,
                new IntegerFileExporter()
            };
            var exportHandler = new GuiExportHandler(mainWindow, fileImporters);

            // Call
            Action call = () => exportHandler.ExportFrom(1234);

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                "Exporteren gestart.",
                "Exporteren afgerond."
            });
            mockRepository.VerifyAll();
        }

        private class IntegerFileExporter : IFileExporter
        {
            public string Name
            {
                get
                {
                    return "IntegerFileExporter";
                }
            }

            public string Category { get; private set; }
            public Bitmap Image { get; private set; }

            public Type SupportedItemType
            {
                get
                {
                    return typeof(Int32);
                }
            }

            public string FileFilter
            {
                get
                {
                    return "Text files (*.txt)|*.txt";
                }
            }

            public bool Export(object sourceItem, string filePath)
            {
                return true;
            }
        }
    }
}