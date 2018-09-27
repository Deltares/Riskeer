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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Service;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Forms.ProgressDialog;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ProgressDialog
{
    [TestFixture]
    public class ActivityProgressDialogTest : NUnitFormTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            // Call
            using (var dialog = new ActivityProgressDialog(window, Enumerable.Empty<Activity>()))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsNotNull(dialog.Icon);
                Assert.IsTrue(dialog.ShowIcon);
                Assert.AreEqual(0, dialog.MinimumSize.Width); // Set during load
                Assert.AreEqual(0, dialog.MinimumSize.Height); // Set during load
                Assert.AreEqual(FormBorderStyle.FixedDialog, dialog.FormBorderStyle);
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
        public void ShowDialog_ActivityProgressDialog_MinimumSizeSet()
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

            using (var dialog = new ActivityProgressDialog(window, Enumerable.Empty<Activity>()))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(520, dialog.MinimumSize.Width);
                Assert.AreEqual(150, dialog.MinimumSize.Height);
            }

            mocks.VerifyAll();
        }
    }
}