// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Gui.Commands;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class ExceptionDialogTest : NUnitFormTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            // Call
            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Assert
                Assert.IsNotNull(dialog.Icon);
                Assert.IsTrue(dialog.ShowIcon);
                Assert.AreEqual(0, dialog.MinimumSize.Width); // Set during load
                Assert.AreEqual(0, dialog.MinimumSize.Height); // Set during load
                Assert.AreEqual(FormBorderStyle.Sizable, dialog.FormBorderStyle);
                Assert.AreEqual(FormStartPosition.CenterParent, dialog.StartPosition);
                Assert.IsFalse(dialog.ShowInTaskbar);
                Assert.IsTrue(dialog.ControlBox);
                Assert.IsFalse(dialog.MaximizeBox);
                Assert.IsFalse(dialog.MinimizeBox);
                Assert.IsNull(dialog.CancelButton);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Show_Always_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(470, dialog.MinimumSize.Width);
                Assert.AreEqual(200, dialog.MinimumSize.Height);

                var textBox = new RichTextBoxTester("exceptionTextBox");
                string exceptionText = textBox.Text;
                Assert.AreEqual("", exceptionText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Show_WithException_ExceptionMessageSetToTextBox()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var exception = new Exception("Test", new Exception("Test inner"));
            using (var dialog = new ExceptionDialog(window, null, exception))
            {
                // Call
                dialog.Show();

                // Assert
                var textBox = new RichTextBoxTester("exceptionTextBox");
                string exceptionText = textBox.Text;
                Assert.AreEqual(exception.ToString().Replace(Environment.NewLine, "\n"), exceptionText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenExceptionDialog_WhenNoOpenLogActionSet_ThenOpenLogButtonDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.Show();

                // Assert
                var button = new ButtonTester("buttonOpenLog");
                var buttonOpenLog = (Button) button.TheObject;
                Assert.IsFalse(buttonOpenLog.Enabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenExceptionDialog_WhenOpenLogActionSet_ThenOpenLogButtonEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new ExceptionDialog(window, null, null)
            {
                OpenLogClicked = () => {}
            })
            {
                // Call
                dialog.Show();

                // Assert
                var button = new ButtonTester("buttonOpenLog");
                var buttonOpenLog = (Button) button.TheObject;
                Assert.IsTrue(buttonOpenLog.Enabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenExceptionDialog_WhenRestartButtonClicked_ThenDialogResultOk()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                dialog.Show();
                var button = new ButtonTester("buttonRestart");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(DialogResult.OK, dialog.DialogResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenExceptionDialog_WhenExitButtonClicked_ThenDialogResultCancel()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                dialog.Show();
                var button = new ButtonTester("buttonExit");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(DialogResult.Cancel, dialog.DialogResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenExceptionDialog_WhenOpenLogButtonClicked_ThenPerformsOpenLogClickedAction()
        {
            // Setup
            var counter = 0;

            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new ExceptionDialog(window, null, null)
            {
                OpenLogClicked = () => counter++
            })
            {
                dialog.Show();
                var button = new ButtonTester("buttonOpenLog");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(1, counter);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)] // Don't remove: test will hang otherwise due to copy to clipboard
        public void GivenExceptionDialog_WhenCopyToClipboardClicked_ThenExceptionTextCopiedToClipboard()
        {
            // Setup
            var exception = new Exception("Test");
            using (var dialog = new ExceptionDialog(new UserControl(), null, exception))
            {
                dialog.Show();

                var button = new ButtonTester("buttonCopyTextToClipboard");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(exception.ToString(), Clipboard.GetText());
            }
        }

        [Test]
        [TestCase(true, "Project is opgeslagen", "Opslaan van project is gelukt.")]
        [TestCase(false, "Project is niet opgeslagen", "Opslaan van project is mislukt.")]
        public void GivenExceptionDialog_WhenSaveProjectClicked_ThenSaveProjectAsCalledAndMessageBoxShown(bool saveSuccessful, string expectedDialogTitle, string expectedDialogMessage)
        {
            // Setup
            var mocks = new MockRepository();
            var commandsOwner = mocks.StrictMock<ICommandsOwner>();
            var commands = mocks.StrictMock<IStorageCommands>();
            commandsOwner.Expect(co => co.StorageCommands).Return(commands);
            commands.Expect(c => c.SaveProjectAs()).Return(saveSuccessful);
            mocks.ReplayAll();

            var messageBoxTitle = "";
            var messageBoxText = "";

            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new MessageBoxTester(wnd);
                messageBoxTitle = openedDialog.Title;
                messageBoxText = openedDialog.Text;
                openedDialog.ClickOk();
            };

            var exception = new Exception("Test");
            using (var dialog = new ExceptionDialog(new UserControl(), commandsOwner, exception))
            {
                dialog.Show();

                var buttonTester = new ButtonTester("buttonSaveProject");

                // Call
                buttonTester.Click();

                // Assert
                Assert.AreEqual(expectedDialogTitle, messageBoxTitle);
                Assert.AreEqual(expectedDialogMessage, messageBoxText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenExceptionDialog_WhenSaveProjectClickedAndExceptionThrown_ThenUnsuccessfulMessageShown()
        {
            // Setup
            var mocks = new MockRepository();
            var commandsOwner = mocks.StrictMock<ICommandsOwner>();
            var commands = mocks.StrictMock<IStorageCommands>();
            commandsOwner.Expect(co => co.StorageCommands).Return(commands);
            commands.Expect(c => c.SaveProjectAs()).Throw(new Exception());
            mocks.ReplayAll();

            var messageBoxTitle = "";
            var messageBoxText = "";

            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new MessageBoxTester(wnd);
                messageBoxTitle = openedDialog.Title;
                messageBoxText = openedDialog.Text;
                openedDialog.ClickOk();
            };

            var exception = new Exception("Test");
            using (var dialog = new ExceptionDialog(new UserControl(), commandsOwner, exception))
            {
                dialog.Show();
                var buttonTester = new ButtonTester("buttonSaveProject");

                // Call
                buttonTester.Click();

                // Assert
                Assert.AreEqual("Project is niet opgeslagen", messageBoxTitle);
                Assert.AreEqual("Opslaan van project is mislukt.", messageBoxText);
            }

            mocks.VerifyAll();
        }
    }
}