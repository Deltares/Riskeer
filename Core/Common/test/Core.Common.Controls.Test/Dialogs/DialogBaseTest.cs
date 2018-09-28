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

            // Call
            TestDelegate test = () => new TestDialog(null, icon, 1, 2);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", paramName);
        }

        [Test]
        public void Constructor_IconEqualsNull_ArgumentNullExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            // Call
            TestDelegate test = () => new TestDialog(window, (Icon) null, 1, 2);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("icon", paramName);
        }

        [Test]
        public void Constructor_BitmapEqualsNull_ArgumentNullExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            // Call
            TestDelegate test = () => new TestDialog(window, (Bitmap) null, 1, 2);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("icon", paramName);
        }

        [Test]
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
            const string expectedMessage = "The minimum width of the dialog should be greater than 0";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void Constructor_IncorrectMinHeight_ArgumentExceptionIsThrown(int minHeight)
        {
            // Setup
            var mocks = new MockRepository();
            var icon = mocks.Stub<Icon>();
            var window = mocks.Stub<IWin32Window>();

            TestDelegate test = () => new TestDialog(window, icon, 1, minHeight);

            // Call & Assert
            const string expectedMessage = "The minimum height of the dialog should be greater than 0";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
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

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_TestDialog_MinimumSizeSet()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.StrictMock<IWin32Window>();
            window.Expect(w => w.Handle).Repeat.AtLeastOnce().Return(default(IntPtr));
            mocks.ReplayAll();

            Icon icon = IconStub();

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

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDialog_TestDialog_CancelButtonSet()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.StrictMock<IWin32Window>();
            window.Expect(w => w.Handle).Repeat.AtLeastOnce().Return(default(IntPtr));
            mocks.ReplayAll();

            Icon icon = IconStub();

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

            mocks.VerifyAll();
        }

        private static Icon IconStub()
        {
            var canvas = new Bitmap(16, 16);
            return Icon.FromHandle(canvas.GetHicon());
        }

        private class TestDialog : DialogBase
        {
            public TestDialog(IWin32Window dialogParent, Bitmap icon, int minWidth, int minHeight)
                : base(dialogParent, icon, minWidth, minHeight) {}

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