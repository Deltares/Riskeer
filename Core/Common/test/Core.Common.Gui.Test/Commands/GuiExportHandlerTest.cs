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
using System.IO;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
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
            TestHelper.AssertLogMessageIsGenerated(call, string.Format("Ringtoets kan de huidige selectie ({0}) niet exporteren.", sourceTypeName));
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Ringtoets kan de huidige selectie niet exporteren.", messageBoxText);
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
            TestHelper.AssertLogMessageIsGenerated(call, string.Format("Ringtoets kan de huidige selectie ({0}) niet exporteren.", sourceTypeName));
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Ringtoets kan de huidige selectie niet exporteren.", messageBoxText);
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ExportFrom_SupportedExporterAvailableCancelClicked_AbortsExport()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var exporterMock = mockRepository.StrictMock<IFileExporter>();
            mockRepository.ReplayAll();

            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.ClickCancel();
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<int>
                {
                    CreateFileExporter = (o, s) => exporterMock
                }
            });

            // Call
            exportHandler.ExportFrom(1234);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on exporter mock
        }

        [Test]
        [RequiresSTA]
        public void ExportFrom_SupportedExporterAvailableWhichRunsSuccessfulSaveClicked_CallsExportAndLogsMessages()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var exporterMock = mockRepository.StrictMock<IFileExporter>();

            exporterMock.Stub(e => e.Export()).Return(true);

            mockRepository.ReplayAll();

            const int expectedData = 1234;
            string targetExportFileName = Path.GetFullPath("exportFile.txt");

            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.SaveFile(targetExportFileName);
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<int>
                {
                    CreateFileExporter = (data, filePath) =>
                    {
                        Assert.AreEqual(expectedData, data);
                        Assert.AreEqual(targetExportFileName, filePath);
                        return exporterMock;
                    }
                }
            });

            // Call
            Action call = () => exportHandler.ExportFrom(expectedData);

            // Assert
            var finalMessage = string.Format("Exporteren naar '{0}' is afgerond.", targetExportFileName);
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                "Exporteren gestart.",
                finalMessage
            });
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ExportFrom_SupportedExporterAvailableWhichFailsSaveClicked_CallsExportAndLogsMessages()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            var exporterMock = mockRepository.StrictMock<IFileExporter>();

            exporterMock.Stub(e => e.Export()).Return(false);

            mockRepository.ReplayAll();

            var targetExportFileName = Path.GetFullPath("exportFile.txt");
            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.SaveFile(targetExportFileName);
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                new ExportInfo<int>
                {
                    CreateFileExporter = (data, filePath) => exporterMock
                }
            });

            // Call
            Action call = () => exportHandler.ExportFrom(1234);

            // Assert
            TestHelper.AssertLogMessagesAreGenerated(call, new[]
            {
                "Exporteren gestart.",
                "Exporteren is mislukt."
            });
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ExportFrom_MultipleSupportedExportersAvailable_GivesSelectionDialog()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.Stub<IMainWindow>();
            mockRepository.ReplayAll();

            var dialogText = "";

            ModalFormHandler = (name, wnd, form) =>
            {
                var dialog = new FormTester(name);

                dialogText = dialog.Text;

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
            Assert.AreEqual("Selecteer wat u wilt exporteren", dialogText);
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

            var commandHandler = new GuiExportHandler(dialogParent, new List<ExportInfo>
            {
                new ExportInfo<object>()
            });

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

            var commandHandler = new GuiExportHandler(dialogParent, new List<ExportInfo>
            {
                new ExportInfo<object>(),
                new ExportInfo<object>()
            });

            // Call
            var isExportPossible = commandHandler.CanExportFrom(new object());

            // Assert
            Assert.IsTrue(isExportPossible);
            mocks.VerifyAll();
        }
    }
}