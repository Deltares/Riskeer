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

            const string testText = "Some text for the dialog";

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