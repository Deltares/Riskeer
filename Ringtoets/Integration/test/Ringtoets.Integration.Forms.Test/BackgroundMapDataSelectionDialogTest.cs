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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class BackgroundMapDataSelectionDialogTest : NUnitFormTest
    {
        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new BackgroundMapDataSelectionDialog(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithParent_DefaultProperties()
        {
            // Setup
            using (var dialogParent = new Form())
            {
                // Call
                using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent))
                {
                    // Assert
                    Assert.IsInstanceOf<DialogBase>(dialog);
                    Assert.AreEqual(@"Selecteer achtergrondkaart", dialog.Text);
                    Assert.IsNull(dialog.SelectedMapData);

                    Icon icon = BitmapToIcon(RingtoetsCommonFormsResources.SelectionDialogIcon);
                    Bitmap expectedImage = icon.ToBitmap();
                    Bitmap actualImage = dialog.Icon.ToBitmap();
                    TestHelper.AssertImagesAreEqual(expectedImage, actualImage);
                }
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShowDialog_Always_DefaultProperties()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name)) {}
            };

            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent))
            {
                // Call
                dialog.ShowDialog();

                // Assert
                var mapLayerLabel = new LabelTester("mapLayerLabel", dialog);
                Assert.AreEqual("Type kaartlaag", mapLayerLabel.Text);

                var mapLayers = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                Assert.IsFalse(mapLayers.Enabled);

                var groupBoxProperties = new ControlTester("propertiesGroupBox", dialog);
                Assert.AreEqual("Eigenschappen", groupBoxProperties.Text);

                Button buttonSelect = (Button) new ButtonTester("selectButton", dialog).TheObject;
                Assert.AreEqual("Selecteren", buttonSelect.Text);
                Assert.IsFalse(buttonSelect.Enabled);

                Button buttonCancel = (Button) new ButtonTester("cancelButton", dialog).TheObject;
                Assert.AreEqual("Annuleren", buttonCancel.Text);

                Assert.AreEqual(500, dialog.MinimumSize.Width);
                Assert.AreEqual(350, dialog.MinimumSize.Height);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidDialog_WhenCancelPressed_ThenSelectedMapDataNull()
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
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent))
            {
                // When
                dialog.ShowDialog();

                // Then
                Assert.IsNull(dialog.SelectedMapData);
                Assert.AreEqual(dialog.CancelButton, cancelButton);
            }
        }

        [Test]
        public void Dispose_DisposedAlreadyCalled_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () =>
            {
                using (var dialogParent = new Form())
                using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent))
                {
                    dialog.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}