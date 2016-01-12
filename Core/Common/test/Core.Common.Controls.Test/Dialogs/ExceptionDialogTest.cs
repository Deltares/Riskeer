using System;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.Test.Dialogs
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
            using (var dialog = new ExceptionDialog(window, null))
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

            using (var dialog = new ExceptionDialog(window, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(470, dialog.MinimumSize.Width);
                Assert.AreEqual(200, dialog.MinimumSize.Height);
            }
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

            using (var dialog = new ExceptionDialog(window, exception))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(exception.ToString().Replace(Environment.NewLine, "\n"), exceptionText);
            }
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

            using (var dialog = new ExceptionDialog(window, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual("", exceptionText);
            }
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

            using (var dialog = new ExceptionDialog(window, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.IsFalse(buttonOpenLog.Enabled);
            }
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

            using (var dialog = new ExceptionDialog(window, null)
            {
                OpenLogClicked = () => { }
            })
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.IsTrue(buttonOpenLog.Enabled);
            }
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

            using (var dialog = new ExceptionDialog(window, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(DialogResult.OK, dialog.DialogResult);
            }
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

            using (var dialog = new ExceptionDialog(window, null))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(DialogResult.Cancel, dialog.DialogResult);
            }
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

            using (var dialog = new ExceptionDialog(window, null)
            {
                OpenLogClicked = () => counter++
            })
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(1, counter);
            }
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
            using (var dialog = new ExceptionDialog(new UserControl(), exception))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(exception.ToString(), Clipboard.GetText());
            }
        }
    }
}