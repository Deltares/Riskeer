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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BruTile;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.TestUtil.Settings;
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using Core.Components.Gis.TestUtil;
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
        public void Constructor_MapDataNull_DefaultProperties()
        {
            // Setup
            var dialogParent = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            // Call
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.AreEqual("Selecteer achtergrondkaart", dialog.Text);

                Icon icon = BitmapToIcon(RingtoetsCommonFormsResources.SelectionDialogIcon);
                Bitmap expectedImage = icon.ToBitmap();
                Bitmap actualImage = dialog.Icon.ToBitmap();
                TestHelper.AssertImagesAreEqual(expectedImage, actualImage);

                AssertMapDataControls<WellKnownMapDataControl>(dialog);
            }
        }

        [Test]
        public void MapDataConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();
            var imageBasedMapData = new TestImageBasedMapData("someMapData", true);

            // Call
            TestDelegate test = () => new BackgroundMapDataSelectionDialog(null, imageBasedMapData);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void MapDataConstructor_WithWmtsMapData_DefaultProperties()
        {
            // Setup
            var dialogParent = mockRepository.Stub<IWin32Window>();
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            tileFactory.Expect(tf => tf.GetWmtsTileSources(mapData.SourceCapabilitiesUrl)).Return(Enumerable.Empty<ITileSource>());
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, mapData))
                {
                    // Assert
                    AssertMapDataControls<WmtsLocationControl>(dialog);
                }
            }
        }

        [Test]
        public void MapDataConstructor_WithWellKnownMapData_DefaultProperties()
        {
            // Setup
            var dialogParent = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var random = new Random(124);
            var mapData = new WellKnownTileSourceMapData(random.NextEnumValue<WellKnownTileSource>());

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, mapData))
                {
                    // Assert
                    AssertMapDataControls<WellKnownMapDataControl>(dialog);
                }
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShowDialog_Always_DefaultProperties()
        {
            // Setup
            mockRepository.ReplayAll();

            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
            {
                // Call
                dialog.Show();

                // Assert
                var mapLayerLabel = new LabelTester("mapLayerLabel", dialog);
                Assert.AreEqual("Type kaartlaag", mapLayerLabel.Text);

                var mapLayers = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                Assert.IsTrue(mapLayers.Enabled);

                var groupBoxProperties = new ControlTester("propertiesGroupBox", dialog);
                Assert.AreEqual("Eigenschappen", groupBoxProperties.Text);

                Button buttonSelect = (Button) new ButtonTester("selectButton", dialog).TheObject;
                Assert.AreEqual("Selecteren", buttonSelect.Text);
                Assert.IsTrue(buttonSelect.Enabled);

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
                DialogResult dialogResult = dialog.ShowDialog();

                // Then
                Assert.AreEqual(DialogResult.Cancel, dialogResult);
                Assert.IsNull(dialog.SelectedMapData);
                Assert.AreEqual(dialog.CancelButton, cancelButton);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidDialog_WhenSelectPressed_ThenSelectedMapDataSet()
        {
            // Given
            mockRepository.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    var button = new ButtonTester("selectButton", name);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
            {
                // When
                DialogResult dialogResult = dialog.ShowDialog();

                // Then
                Assert.AreEqual(DialogResult.OK, dialogResult);
                Assert.IsNotNull(dialog.SelectedMapData);
            }
        }

        [Test]
        public void GivenValidDialogWithoutMapData_WhenBackgroundMapDataSelectionControlSwitchedBackAndForth_ThenSelectButtonAsExpected()
        {
            // Given
            mockRepository.ReplayAll();

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "noConfig")
            }))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
            {
                dialog.Show();

                var selectButton = (Button) new ButtonTester("selectButton", dialog).TheObject;
                var comboBox = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                WellKnownMapDataControl wellKnownMapDataControl = GetComboBoxItem<WellKnownMapDataControl>(comboBox);
                WmtsLocationControl wmtsLocationControl = GetComboBoxItem<WmtsLocationControl>(comboBox);

                // Precondition state
                comboBox.SelectedItem = wmtsLocationControl;
                var wmtsDataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                Assert.IsNull(wmtsDataGridViewControl.CurrentRow);
                Assert.IsFalse(selectButton.Enabled);

                // When
                comboBox.SelectedItem = wellKnownMapDataControl;
                var wellKnownDataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                DataGridViewRow currentRow = wellKnownDataGridViewControl.CurrentRow;
                Assert.AreEqual(0, currentRow.Index);
                Assert.IsTrue(selectButton.Enabled);

                // Then
                comboBox.SelectedItem = wmtsLocationControl;
                Assert.IsNull(wmtsDataGridViewControl.CurrentRow);
                Assert.IsFalse(selectButton.Enabled);
            }
        }

        [Test]
        public void GivenValidDialog_WhenControlSwitched_ThenDoesNotListenToEventOfOldControl()
        {
            // Given
            WmtsMapData activeWmtsMapData = WmtsMapData.CreateDefaultPdokMapData();

            var capabilities = new[]
            {
                new TestWmtsTileSource(WmtsMapData.CreateAlternativePdokMapData()),
                new TestWmtsTileSource(activeWmtsMapData)
            };

            tileFactory.Expect(tf => tf.GetWmtsTileSources(activeWmtsMapData.SourceCapabilitiesUrl)).Return(capabilities);
            mockRepository.ReplayAll();

            var wmtsLocationControlSelectedMapDataChanged = 0;

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "noConfig")
            }))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, activeWmtsMapData))
            {
                dialog.Show();

                var comboBox = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                WellKnownMapDataControl wellKnownMapDataControl = GetComboBoxItem<WellKnownMapDataControl>(comboBox);
                WmtsLocationControl wmtsLocationControl = GetComboBoxItem<WmtsLocationControl>(comboBox);

                comboBox.SelectedItem = wmtsLocationControl;
                var wmtsDataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                wmtsLocationControl.SelectedMapDataChanged += (sender, args) => { wmtsLocationControlSelectedMapDataChanged++; };

                comboBox.SelectedItem = wellKnownMapDataControl;
                var wellKnownDataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                wellKnownDataGridViewControl.ClearCurrentCell();

                var button = (Button) new ButtonTester("selectButton", dialog).TheObject;
                Assert.IsFalse(button.Enabled);

                // When
                wmtsDataGridViewControl.SetCurrentCell(wmtsDataGridViewControl.GetCell(0, 0));

                // Then
                Assert.IsFalse(button.Enabled);
                Assert.AreEqual(1, wmtsLocationControlSelectedMapDataChanged);
            }
        }

        [Test]
        public void GivenValidDialog_WhenControlSwitched_ThenListenToEventOfNewControl()
        {
            // Given
            WmtsMapData activeWmtsMapData = WmtsMapData.CreateDefaultPdokMapData();

            var capabilities = new[]
            {
                new TestWmtsTileSource(WmtsMapData.CreateAlternativePdokMapData()),
                new TestWmtsTileSource(activeWmtsMapData)
            };

            tileFactory.Expect(tf => tf.GetWmtsTileSources(activeWmtsMapData.SourceCapabilitiesUrl)).Return(capabilities);
            mockRepository.ReplayAll();

            var wellKnownSelectedMapDataChanged = 0;

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "noConfig")
            }))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, activeWmtsMapData))
            {
                dialog.Show();

                var comboBox = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                var wellKnownControl = GetComboBoxItem<WellKnownMapDataControl>(comboBox);

                comboBox.SelectedItem = wellKnownControl;

                var wellKnownDataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                wellKnownControl.SelectedMapDataChanged += (sender, args) => { wellKnownSelectedMapDataChanged++; };

                // When
                wellKnownDataGridViewControl.SetCurrentCell(wellKnownDataGridViewControl.GetCell(4, 0));
                var button = new ButtonTester("selectButton", dialog);
                button.Click();

                // Then
                Assert.IsInstanceOf<WellKnownTileSourceMapData>(dialog.SelectedMapData);
                Assert.AreEqual(1, wellKnownSelectedMapDataChanged);
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

        private static void AssertMapDataControls<T>(BackgroundMapDataSelectionDialog dialog)
            where T : UserControl, IBackgroundMapDataSelectionControl
        {
            var mapLayers = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
            Assert.AreEqual("DisplayName", mapLayers.DisplayMember);
            Assert.AreEqual("UserControl", mapLayers.ValueMember);
            Assert.AreEqual(ComboBoxStyle.DropDownList, mapLayers.DropDownStyle);
            Assert.IsTrue(mapLayers.Sorted);
            Assert.IsInstanceOf<T>(mapLayers.SelectedItem);

            var backgroundMapDataSelectionControls = (IBackgroundMapDataSelectionControl[]) mapLayers.DataSource;
            Assert.AreEqual(2, backgroundMapDataSelectionControls.Length);
            Assert.IsInstanceOf<WellKnownMapDataControl>(backgroundMapDataSelectionControls[0]);
            Assert.IsInstanceOf<WmtsLocationControl>(backgroundMapDataSelectionControls[1]);

            Assert.IsInstanceOf<IBackgroundMapDataSelectionControl[]>(mapLayers.DataSource);
            Assert.AreEqual("DisplayName", mapLayers.DisplayMember);

            var groupBoxProperties = (GroupBox) new ControlTester("propertiesGroupBox", dialog).TheObject;
            Assert.AreEqual(DockStyle.Fill, groupBoxProperties.Dock);
            Assert.AreEqual(1, groupBoxProperties.Controls.Count);
            Assert.AreSame(mapLayers.SelectedItem, groupBoxProperties.Controls[0]);
        }

        private static T GetComboBoxItem<T>(ComboBox comboBox)
        {
            return ((IBackgroundMapDataSelectionControl[]) comboBox.DataSource).OfType<T>().First();
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}