// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Gui.Clipboard;
using Core.Gui.Commands;
using Core.Gui.TestUtil.Clipboard;
using NSubstitute;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Gui.Test
{
    [TestFixture]
    public class ExceptionDialogTest : NUnitFormTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Setup
            var window = Substitute.For<IWin32Window>();

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
        }

        [Test]
        public void Show_Always_ExpectedValues()
        {
            // Setup
            var window = Substitute.For<IWin32Window>();

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
        }

        [Test]
        public void Show_WithException_ExceptionMessageSetToTextBox()
        {
            // Setup
            var window = Substitute.For<IWin32Window>();

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
        }

        [Test]
        public void GivenExceptionDialog_WhenNoOpenLogActionSet_ThenOpenLogButtonDisabled()
        {
            // Setup
            var window = Substitute.For<IWin32Window>();

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.Show();

                // Assert
                var button = new ButtonTester("buttonOpenLog");
                var buttonOpenLog = (Button) button.TheObject;
                Assert.IsFalse(buttonOpenLog.Enabled);
            }
        }

        [Test]
        public void GivenExceptionDialog_WhenOpenLogActionSet_ThenOpenLogButtonEnabled()
        {
            // Setup
            var window = Substitute.For<IWin32Window>();

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
        }

        [Test]
        public void GivenExceptionDialog_WhenRestartButtonClicked_ThenDialogResultOk()
        {
            // Setup
            var window = Substitute.For<IWin32Window>();

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                dialog.Show();
                var button = new ButtonTester("buttonRestart");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(DialogResult.OK, dialog.DialogResult);
            }
        }

        [Test]
        public void GivenExceptionDialog_WhenExitButtonClicked_ThenDialogResultCancel()
        {
            // Setup
            var window = Substitute.For<IWin32Window>();

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                dialog.Show();
                var button = new ButtonTester("buttonExit");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(DialogResult.Cancel, dialog.DialogResult);
            }
        }

        [Test]
        public void GivenExceptionDialog_WhenOpenLogButtonClicked_ThenPerformsOpenLogClickedAction()
        {
            // Setup
            var counter = 0;

            var window = Substitute.For<IWin32Window>();

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
        }

        [Test]
        public void GivenExceptionDialog_WhenCopyToClipboardClicked_ThenExceptionTextCopiedToClipboard()
        {
            // Setup
            var exception = new Exception("Test");
            using (new ClipboardConfig())
            using (var dialog = new ExceptionDialog(new UserControl(), null, exception))
            {
                dialog.Show();

                var button = new ButtonTester("buttonCopyTextToClipboard");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(exception.ToString(), ClipboardProvider.Clipboard.GetText());
            }
        }

        [Test]
        [TestCase(true, "Project is opgeslagen", "Opslaan van project is gelukt.")]
        [TestCase(false, "Project is niet opgeslagen", "Opslaan van project is mislukt.")]
        public void GivenExceptionDialog_WhenSaveProjectClicked_ThenSaveProjectAsCalledAndMessageBoxShown(bool saveSuccessful, string expectedDialogTitle, string expectedDialogMessage)
        {
            // Setup
            var commandsOwner = Substitute.For<ICommandsOwner>();
            var commands = Substitute.For<IStorageCommands>();
            commandsOwner.StorageCommands.Returns(commands);
            commands.SaveProjectAs().Returns(saveSuccessful);

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
            _ = commandsOwner.Received().StorageCommands;
            commands.Received().SaveProjectAs();
        }

        [Test]
        public void GivenExceptionDialog_WhenSaveProjectClickedAndExceptionThrown_ThenUnsuccessfulMessageShown()
        {
            // Setup
            var commandsOwner = Substitute.For<ICommandsOwner>();
            var commands = Substitute.For<IStorageCommands>();
            commandsOwner.StorageCommands.Returns(commands);
            commands.SaveProjectAs().Returns(_ => throw new Exception());

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
            _ = commandsOwner.Received().StorageCommands;
            commands.Received().SaveProjectAs();
        }
    }
}