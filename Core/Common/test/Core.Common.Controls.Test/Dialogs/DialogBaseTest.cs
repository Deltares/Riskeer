using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.Test.Dialogs
{
    [TestFixture]
    public class DialogBaseTest : NUnitFormTest
    {
        [Test]
        public void Constructor_OwnerEqualsNull_ArgumentNullExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var icon = mocks.Stub<Icon>();

            TestDelegate test = () => new TestDialog(null, icon, 1, 2);

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("dialogParent", message);
        }

        [Test]
        public void Constructor_IconEqualsNull_ArgumentNullExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            TestDelegate test = () => new TestDialog(window, (Icon) null, 1, 2);

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("icon", message);
        }

        [Test]
        public void Constructor_BitmapEqualsNull_ArgumentNullExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            TestDelegate test = () => new TestDialog(window, (Bitmap) null, 1, 2);

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("icon", message);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Constructor_IncorrectMinWidth_ArgumentExceptionIsThrown(int minWidth)
        {
            // Setup
            var mocks = new MockRepository();
            var icon = mocks.Stub<Icon>();
            var window = mocks.Stub<IWin32Window>();

            TestDelegate test = () => new TestDialog(window, icon, minWidth, 1);

            // Call & Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "The minimum width of the dialog should be greater than 0");
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Constructor_IncorrectMinHeigth_ArgumentExceptionIsThrown(int minHeight)
        {
            // Setup
            var mocks = new MockRepository();
            var icon = mocks.Stub<Icon>();
            var window = mocks.Stub<IWin32Window>();

            TestDelegate test = () => new TestDialog(window, icon, 1, minHeight);

            // Call & Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "The minimum height of the dialog should be greater than 0");
        }

        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            var icon = mocks.Stub<Icon>();

            mocks.ReplayAll();

            // Call
            using (var dialog = new TestDialog(window, icon, 1, 2))
            {
                // Assert
                Assert.AreEqual(icon, dialog.Icon);
                Assert.IsTrue(dialog.ShowIcon);
                Assert.AreEqual(0, dialog.MinimumSize.Width); // Set during load
                Assert.AreEqual(0, dialog.MinimumSize.Height); // Set during load
                Assert.AreEqual(FormBorderStyle.Sizable, dialog.FormBorderStyle);
                Assert.AreEqual(FormStartPosition.CenterParent, dialog.StartPosition);
                Assert.IsFalse(dialog.ShowInTaskbar);
                Assert.IsTrue(dialog.ControlBox);
                Assert.IsFalse(dialog.MaximizeBox);
                Assert.IsFalse(dialog.MinimizeBox);
                Assert.IsNull(dialog.CancelButton); // Set during load
            }
        }

        [Test]
        public void ShowDialog_TestDialog_MinimumSizeSet()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            var icon = mocks.Stub<Icon>();

            icon.Stub(i => i.Handle).Return(new IntPtr());
            icon.Stub(i => i.Size).Return(new Size(16, 16));

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);

                openedDialog.Close();
            };

            using (var dialog = new TestDialog(window, icon, 1, 2))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(1, dialog.MinimumSize.Width);
                Assert.AreEqual(2, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void ShowDialog_TestDialog_CancelButtonSet()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();
            var icon = mocks.Stub<Icon>();

            icon.Stub(i => i.Handle).Return(new IntPtr());
            icon.Stub(i => i.Size).Return(new Size(16, 16));

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var openedDialog = new FormTester(name);

                openedDialog.Close();
            };

            using (var dialog = new TestDialog(window, icon, 1, 2))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.IsNotNull(dialog.CancelButton);
                Assert.AreSame("Test button", ((Button) dialog.CancelButton).Name);
            }
        }

        private class TestDialog : DialogBase
        {
            public TestDialog(IWin32Window dialogParent, Bitmap icon, int minWidth, int minHeight)
                : base(dialogParent, icon, minWidth, minHeight) { }

            public TestDialog(IWin32Window dialogParent, Icon icon, int minWidth, int minHeight)
                : base(dialogParent, icon, minWidth, minHeight) {}

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