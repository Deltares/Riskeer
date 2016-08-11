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
using System.Linq;
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
        public void ExportFrom_NoExporterAvailable_GivesMessageBoxAndLogsMessage(object exportFrom)
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
                GetUnsupportedExportInfo()
            });

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

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                GetIntegerExportInfo()
            });

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

            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.SaveFile(TestHelper.GetTestDataPath(TestDataPath.Core.Common.Gui, "exportFile.txt"));
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                GetIntegerExportInfo()
            });

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
            mockRepository.ReplayAll();

            ModalFormHandler = (name, wnd, form) =>
            {
                var messageBox = new SaveFileDialogTester(wnd);
                messageBox.SaveFile(TestHelper.GetTestDataPath(TestDataPath.Core.Common.Gui, "exportFile.txt"));
            };

            var exportHandler = new GuiExportHandler(mainWindow, new List<ExportInfo>
            {
                GetUnsupportedExportInfo(),
                GetIntegerExportInfo()
            });

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
        public void CanExportFrom_HasNoFileExportersForTarget_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var commandHandler = new GuiExportHandler(dialogParent, Enumerable.Empty<ExportInfo>());

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

        private static ExportInfo GetUnsupportedExportInfo()
        {
            return new ExportInfo<string>();
        }

        private static ExportInfo GetIntegerExportInfo()
        {
            return new ExportInfo<int>
            {
                CreateFileExporter = (data, filePath) => new IntegerFileExporter()
            };
        }

        private class IntegerFileExporter : IFileExporter
        {
            public bool Export()
            {
                return true;
            }
        }
    }
}