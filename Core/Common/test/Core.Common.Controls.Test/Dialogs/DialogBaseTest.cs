using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.Test.Dialogs
{
    [TestFixture]
    public class DialogBaseTest : NUnitFormTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var icon = mocks.Stub<Icon>();

            mocks.ReplayAll();

            // Call
            var dialog = new TestDialog(null, icon, 1, 2);

            // Assert
            Assert.AreEqual(icon, dialog.Icon);
            Assert.IsTrue(dialog.ShowIcon);
            Assert.AreEqual(1, dialog.MinimumSize.Width);
            Assert.AreEqual(2, dialog.MinimumSize.Height);
            Assert.AreEqual(FormBorderStyle.Sizable, dialog.FormBorderStyle);
            Assert.AreEqual(FormStartPosition.CenterParent, dialog.StartPosition);
            Assert.IsFalse(dialog.ShowInTaskbar);
            Assert.IsTrue(dialog.ControlBox);
            Assert.IsFalse(dialog.MaximizeBox);
            Assert.IsFalse(dialog.MinimizeBox);
            Assert.IsNull(dialog.CancelButton);
        }

        [Test]
        public void ShowDialog_TestDialog_CancelButtonSet()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);

                openedDialog.Close();
            };

            var dialog = new TestDialog(null, null, 1, 2);

            // Call
            dialog.ShowDialog();

            // Assert
            Assert.IsNotNull(dialog.CancelButton);
            Assert.AreSame("Test button", ((Button) dialog.CancelButton).Name);
        }

        private class TestDialog : DialogBase
        {
            public TestDialog(IWin32Window owner, Icon icon, int minWidth, int minHeight)
                : base(owner, icon, minWidth, minHeight)
            {

            }

            protected override Button GetCancelButton()
            {
                return new Button
                {
                    Name = "Test button"
                };
            }
        }
    }
}
