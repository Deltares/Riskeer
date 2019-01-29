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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Components.Gis.Forms.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.Gis.Forms.Test.Views
{
    [TestFixture]
    public class WmtsConnectionDialogTest : NUnitFormTest
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
        public void WmtsConnectionInfoConstructor_WithoutDialogParent_ThrowsArgumentNullException()
        {
            // Setup
            var info = new WmtsConnectionInfo("name", "url");

            // Call
            TestDelegate test = () => new WmtsConnectionDialog(null, info);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", paramName);
        }

        [Test]
        public void Constructor_WithDialogParent_ExpectedProperties()
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
                Assert.IsNull(dialog.WmtsConnectionName);
                Assert.IsNull(dialog.WmtsConnectionUrl);

                Assert.AreEqual("Nieuwe WMTS locatie toevoegen", dialog.Text);

                var nameLabel = new LabelTester("nameLabel", dialog);
                Assert.AreEqual("Omschrijving:", nameLabel.Text);

                var urlLabel = new LabelTester("urlLabel", dialog);
                Assert.AreEqual("URL:", urlLabel.Text);

                var actionButton = (Button) new ButtonTester("actionButton", dialog).TheObject;
                Assert.AreEqual("Opslaan", actionButton.Text);
                Assert.IsFalse(actionButton.Enabled);

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
        public void WmtsConnectionInfoConstructor_WithDialogParent_ExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();
            const string connectionName = @"name";
            const string connectionUrl = @"url";
            var info = new WmtsConnectionInfo(connectionName, connectionUrl);

            // Call
            using (var dialog = new WmtsConnectionDialog(dialogParent, info))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsNull(dialog.WmtsConnectionName);
                Assert.IsNull(dialog.WmtsConnectionUrl);

                Assert.AreEqual("WMTS locatie aanpassen", dialog.Text);

                var nameLabel = new LabelTester("nameLabel", dialog);
                Assert.AreEqual("Omschrijving:", nameLabel.Text);

                var urlLabel = new LabelTester("urlLabel", dialog);
                Assert.AreEqual("URL:", urlLabel.Text);

                var actionButton = (Button) new ButtonTester("actionButton", dialog).TheObject;
                Assert.AreEqual("Opslaan", actionButton.Text);
                Assert.IsTrue(actionButton.Enabled);

                var cancelButton = new ButtonTester("cancelButton", dialog);
                Assert.AreEqual("Annuleren", cancelButton.Text);

                var nameTextBox = new TextBoxTester("nameTextBox", dialog);
                Assert.AreEqual(connectionName, nameTextBox.Text);

                var urlTextBox = new TextBoxTester("urlTextBox", dialog);
                Assert.AreEqual(connectionUrl, urlTextBox.Text);
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
                using (new FormTester(name)) {}
            };

            using (var dialogParent = new Form())
            using (var dialog = new WmtsConnectionDialog(dialogParent))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                Assert.AreEqual(400, dialog.MinimumSize.Width);
                Assert.AreEqual(150, dialog.MinimumSize.Height);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ActionButton_WithoutValidText_ButtonIsDisabled(
            [Values("", "  ", null)] string name,
            [Values("", "  ", null)] string url)
        {
            // Setup
            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            using (var dialogParent = new Form())
            using (var dialog = new WmtsConnectionDialog(dialogParent))
            {
                dialog.ShowDialog();

                var nameTextBox = (TextBox) new TextBoxTester("nameTextBox", dialog).TheObject;
                var urlTextBox = (TextBox) new TextBoxTester("urlTextBox", dialog).TheObject;
                var actionButton = (Button) new ButtonTester("actionButton", dialog).TheObject;

                // Call
                nameTextBox.Text = name;
                urlTextBox.Text = url;

                // Assert
                Assert.IsFalse(actionButton.Enabled);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ActionButton_WithValidText_ButtonIsEnabled()
        {
            // Setup
            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            using (var dialogParent = new Form())
            using (var dialog = new WmtsConnectionDialog(dialogParent))
            {
                dialog.ShowDialog();

                var nameTextBox = (TextBox) new TextBoxTester("nameTextBox", dialog).TheObject;
                var urlTextBox = (TextBox) new TextBoxTester("urlTextBox", dialog).TheObject;
                var actionButton = (Button) new ButtonTester("actionButton", dialog).TheObject;

                // Call
                nameTextBox.Text = @"nameTextBox";
                urlTextBox.Text = @"urlTextBox";

                // Assert
                Assert.IsTrue(actionButton.Enabled);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ActionButtonCick_WithValidText_SetsPropertiesAndClosesForm()
        {
            // Setup
            const string urltextbox = @"urlTextBox";
            const string nametextbox = @"nameTextBox";

            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName))
                {
                    var nameTextBox = (TextBox) new TextBoxTester("nameTextBox", formName).TheObject;
                    var urlTextBox = (TextBox) new TextBoxTester("urlTextBox", formName).TheObject;
                    nameTextBox.Text = nametextbox;
                    urlTextBox.Text = urltextbox;

                    var actionButton = new ButtonTester("actionButton", formName);

                    // Call
                    actionButton.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var dialog = new WmtsConnectionDialog(dialogParent))
            {
                DialogResult dialogResult = dialog.ShowDialog();

                // Assert
                Assert.AreEqual(nametextbox, dialog.WmtsConnectionName);
                Assert.AreEqual(urltextbox, dialog.WmtsConnectionUrl);
                Assert.AreEqual(DialogResult.OK, dialogResult);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidDialog_WhenCancelPressed_ThenWmtsConnectionDataNull()
        {
            // Given
            Button cancelButton = null;

            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    var button = new ButtonTester("cancelButton", name);
                    cancelButton = (Button) button.TheObject;
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var dialog = new WmtsConnectionDialog(dialogParent))
            {
                // When
                DialogResult dialogResult = dialog.ShowDialog();

                // Then
                Assert.IsNull(dialog.WmtsConnectionName);
                Assert.IsNull(dialog.WmtsConnectionUrl);

                Assert.AreEqual(dialog.CancelButton, cancelButton);
                Assert.AreEqual(DialogResult.Cancel, dialogResult);
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