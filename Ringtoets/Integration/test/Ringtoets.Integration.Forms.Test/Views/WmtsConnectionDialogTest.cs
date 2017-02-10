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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class WmtsConnectionDialogTest :NUnitFormTest
    {
        [Test]
        public void Constructor_WithoutDialogParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WmtsConnectionDialog(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", paramName);
        }

        [Test]
        public void WmtsConnectionAddDialog_WithDialogParent_ExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var dialog = new WmtsConnectionDialog(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsEmpty(dialog.WmtsConnectionName);
                Assert.IsEmpty(dialog.WmtsConnectionUrl);

                var nameLabel = new LabelTester("nameLabel", dialog);
                Assert.AreEqual("Omschrijving", nameLabel.Text);
                
                var urlLabel = new LabelTester("urlLabel", dialog);
                Assert.AreEqual("URL", urlLabel.Text);
                
                var actionButton = new ButtonTester("actionButton", dialog);
                Assert.AreEqual("Toevoegen", actionButton.Text);
                
                var cancelButton = new ButtonTester("cancelButton", dialog);
                Assert.AreEqual("Annuleren", cancelButton.Text);
                
                var nameTextBox = new TextBoxTester("nameTextBox", dialog);
                Assert.IsEmpty(nameTextBox.Text);
                
                var urlTextBox = new TextBoxTester("urlTextBox", dialog);
                Assert.IsEmpty(urlTextBox.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShowDialog_DefaultProperties()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name)){}
            };

            using (var dialogParent = new Form())
            using (var dialog = new WmtsConnectionDialog(dialogParent))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(250, dialog.MinimumSize.Width);
                Assert.AreEqual(150, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () =>
            {
                using (var control = new WmtsConnectionDialog(dialogParent))
                {
                    control.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
            mocks.VerifyAll();
        }
    }
}