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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BruTile;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class BackgroundMapDataSelectionDialogTest : NUnitFormTest
    {
        private MockRepository mockRepository;
        private ITileSourceFactory tileFactory;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            tileFactory = mockRepository.StrictMock<ITileSourceFactory>();
        }

        [TearDown]
        public override void TearDown()
        {
            mockRepository.VerifyAll();
            base.TearDown();
        }

        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();

            // Call
            TestDelegate test = () => new BackgroundMapDataSelectionDialog(null, mapData);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_MapDataNull_DefaultProperties()
        {
            // Setup
            var dialogParent = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            // Call
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
            {
                // Assert
                Assert.IsNull(dialog.SelectedMapData);
            }
        }

        [Test]
        public void MapDataConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();

            // Call
            TestDelegate test = () => new BackgroundMapDataSelectionDialog(null, mapData);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void MapDataConstructor_WithParents_DefaultProperties()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            tileFactory.Expect(tf => tf.GetWmtsTileSources(mapData.SourceCapabilitiesUrl)).Return(Enumerable.Empty<ITileSource>());
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var dialogParent = new Form())
            {
                // Call
                using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, mapData))
                {
                    // Assert
                    Assert.IsInstanceOf<DialogBase>(dialog);
                    Assert.AreEqual(@"Selecteer achtergrondkaart", dialog.Text);
                    Assert.AreSame(mapData, dialog.SelectedMapData);

                    Icon icon = BitmapToIcon(RingtoetsCommonFormsResources.SelectionDialogIcon);
                    Bitmap expectedImage = icon.ToBitmap();
                    Bitmap actualImage = dialog.Icon.ToBitmap();
                    TestHelper.AssertImagesAreEqual(expectedImage, actualImage);

                    var mapLayers = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                    Assert.AreEqual(ComboBoxStyle.DropDownList, mapLayers.DropDownStyle);
                    Assert.IsInstanceOf<List<IBackgroundMapDataSelectionControl>>(mapLayers.DataSource);
                    Assert.AreEqual("DisplayName", mapLayers.DisplayMember);
                    Assert.IsTrue(mapLayers.Sorted);
                }
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShowDialog_Always_DefaultProperties()
        {
            // Setup
            mockRepository.ReplayAll();
            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name)) {}
            };

            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
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
            mockRepository.ReplayAll();
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
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
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
            // Setup
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () =>
            {
                using (var dialogParent = new Form())
                using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
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