using System.Windows.Forms;
using Core.Common.Gui.Forms.MessageWindow;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.MessageWindow
{
    [TestFixture]
    public class MessageWindowDialogTest
    {
        [Test]
        public void MessageWindowDialog_Always_SetProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var parent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            string testText = "Some text for the dialog";

            // Call
            var dialog = new MessageWindowDialog(parent, testText);

            // Assert
            var textBox = (TextBox) dialog.Controls.Find("textBox", true)[0];

            Assert.AreEqual(testText, textBox.Text);
            Assert.IsFalse(textBox.TabStop);
            Assert.IsTrue(textBox.ReadOnly);

            mocks.VerifyAll();
        }
    }
}