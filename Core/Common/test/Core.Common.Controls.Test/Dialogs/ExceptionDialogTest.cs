using System;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.Test.Dialogs
{
    [TestFixture]
    public class ExceptionDialogTest : NUnitFormTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Setup / Call
            var dialog = new ExceptionDialog(null, null);

            // Assert
            Assert.IsNotNull(dialog.Icon);
            Assert.IsTrue(dialog.ShowIcon);
            Assert.AreEqual(470, dialog.MinimumSize.Width);
            Assert.AreEqual(200, dialog.MinimumSize.Height);
            Assert.AreEqual(FormBorderStyle.Sizable, dialog.FormBorderStyle);
            Assert.AreEqual(FormStartPosition.CenterParent, dialog.StartPosition);
            Assert.IsFalse(dialog.ShowInTaskbar);
            Assert.IsTrue(dialog.ControlBox);
            Assert.IsFalse(dialog.MaximizeBox);
            Assert.IsFalse(dialog.MinimizeBox);
            Assert.IsNull(dialog.CancelButton);
        }

        [Test]
        public void ShowDialog_ExceptionDialogWithException_ExceptionMessageSetToTextBox()
        {
            // Setup
            var exceptionText = "";

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var textBox = new RichTextBoxTester("exceptionTextBox");

                exceptionText = textBox.Text;

                openedDialog.Close();
            };

            var exception = new Exception("Test", new Exception("Test inner"));

            var dialog = new ExceptionDialog(null, exception);

            // Call
            dialog.ShowDialog();

            // Assert
            Assert.AreEqual(exception.ToString().Replace("\r\n", "\n"), exceptionText);
        }

        [Test]
        public void ShowDialog_ExceptionDialogWithoutException_NoExceptionMessageSetToTextBox()
        {
            // Setup
            var exceptionText = "";

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);
                var textBox = new RichTextBoxTester("exceptionTextBox");

                exceptionText = textBox.Text;

                openedDialog.Close();
            };

            var dialog = new ExceptionDialog(null, null);

            // Call
            dialog.ShowDialog();

            // Assert
            Assert.AreEqual("", exceptionText);
        }
    }
}
