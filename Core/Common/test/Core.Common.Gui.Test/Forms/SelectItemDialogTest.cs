// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Test.Properties;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms
{
    [TestFixture]
    public class SelectItemDialogTest : NUnitFormTestWithHiddenDesktop
    {
        [Test]
        public void Constructor_WithoutDialogParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SelectItemDialog(null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", paramName);
        }

        [Test]
        public void Constructor_WithDialogParent_SetProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var parent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var dialog = new SelectItemDialog(parent, "Dialog text"))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsNull(dialog.SelectedItemTag);
                Assert.IsNull(dialog.SelectedItemTypeName);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_SetMinimumSize()
        {
            // Setup
            var mocks = new MockRepository();
            var parent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new SelectItemDialog(parent, "Dialog text"))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(320, dialog.MinimumSize.Width);
                Assert.AreEqual(220, dialog.MinimumSize.Height);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OnLoad_Text_SetsText()
        {
            // Setup
            var mocks = new MockRepository();
            var parent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();
            const string text = "Dialog text";

            using (var dialog = new SelectItemDialog(parent, text))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(text, dialog.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ButtonOkClick_NoItemSelected_ShowDialogWithText()
        {
            // Setup
            var mocks = new MockRepository();
            var parent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            string messageText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageText = messageBox.Text;
                messageBox.ClickOk();
            };

            using (var dialog = new SelectItemDialog(parent, "Dialog text"))
            {
                dialog.Show();

                // Call
                var okButton = new ButtonTester("buttonOk", dialog);
                okButton.Click();

                // Assert
                Assert.IsNull(dialog.SelectedItemTag);
                Assert.IsNull(dialog.SelectedItemTypeName);
                Assert.AreEqual("Kies een type", messageText);
                Assert.AreEqual(DialogResult.None, dialog.DialogResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void SelectedItemTag_ItemSelected_ReturnsSelectedItem()
        {
            // Setup
            var mocks = new MockRepository();
            var parent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new SelectItemDialog(parent, "Dialog text"))
            {
                var tag = new object();
                dialog.AddItemType("aName", "aCategory", Resources.abacus, tag);
                var listView = (ListView) new ControlTester("listViewItemTypes", dialog).TheObject;
                dialog.Show();

                // Call
                listView.Items[0].Selected = true;

                // Assert
                Assert.AreEqual(tag, dialog.SelectedItemTag);
                Assert.AreEqual("aName", dialog.SelectedItemTypeName);
            }

            mocks.VerifyAll();
        }
    }
}