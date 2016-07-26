// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Test
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
        public void ShowDialog_ExceptionDialog_MinimumSizeSet()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);

                openedDialog.Close();
            };

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(470, dialog.MinimumSize.Width);
                Assert.AreEqual(200, dialog.MinimumSize.Height);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_ExceptionDialogWithException_ExceptionMessageSetToTextBox()
        {
            // Setup
            var exceptionText = "";
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var textBox = new RichTextBoxTester("exceptionTextBox");

                exceptionText = textBox.Text;

                openedDialog.Close();
            };

            var exception = new Exception("Test", new Exception("Test inner"));

            using (var dialog = new ExceptionDialog(window, null, exception))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(exception.ToString().Replace(Environment.NewLine, "\n"), exceptionText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_ExceptionDialogWithoutException_NoExceptionMessageSetToTextBox()
        {
            // Setup
            var exceptionText = "";
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var textBox = new RichTextBoxTester("exceptionTextBox");

                exceptionText = textBox.Text;

                openedDialog.Close();
            };

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual("", exceptionText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_ExceptionDialogWithoutOpenLogAction_OpenLogButtonNotEnabled()
        {
            // Setup
            Button buttonOpenLog = null;
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var button = new ButtonTester("buttonOpenLog");

                buttonOpenLog = (Button) button.TheObject;

                openedDialog.Close();
            };

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.IsFalse(buttonOpenLog.Enabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_ExceptionDialogWithOpenLogAction_OpenLogButtonEnabled()
        {
            // Setup
            Button buttonOpenLog = null;
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var button = new ButtonTester("buttonOpenLog");

                buttonOpenLog = (Button) button.TheObject;

                openedDialog.Close();
            };

            using (var dialog = new ExceptionDialog(window, null, null)
            {
                OpenLogClicked = () => { }
            })
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.IsTrue(buttonOpenLog.Enabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_ExceptionDialog_RestartButtonClickResultsInDialogResultOk()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var button = new ButtonTester("buttonRestart");

                button.Click();
            };

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(DialogResult.OK, dialog.DialogResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_ExceptionDialog_ExitButtonClickResultsInDialogResultCancel()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var button = new ButtonTester("buttonExit");

                button.Click();
            };

            using (var dialog = new ExceptionDialog(window, null, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(DialogResult.Cancel, dialog.DialogResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_ExceptionDialog_OpenLogButtonClickPerformsOpenLogClickedAction()
        {
            // Setup
            var counter = 0;
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var button = new ButtonTester("buttonOpenLog");

                button.Click();

                openedDialog.Close();
            };

            using (var dialog = new ExceptionDialog(window, null, null)
            {
                OpenLogClicked = () => counter++
            })
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(1, counter);
            }

            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA] // Don't remove: test will hang otherwise due to copy to clipboard
        public void ShowDialog_ExceptionDialog_CopyToClipboardClickCopiesExceptionTextToClipboard()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var button = new ButtonTester("buttonCopyTextToClipboard");

                button.Click();

                openedDialog.Close();
            };

            var exception = new Exception("Test");
            using (var dialog = new ExceptionDialog(new UserControl(), null, exception))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(exception.ToString(), Clipboard.GetText());
            }
        }
    }
}