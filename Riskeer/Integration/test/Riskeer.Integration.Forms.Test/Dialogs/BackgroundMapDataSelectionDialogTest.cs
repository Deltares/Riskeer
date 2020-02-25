// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using Core.Common.Util.TestUtil.Settings;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using Core.Components.Gis.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Forms.Dialogs;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Test.Dialogs
{
    [TestFixture]
    public class BackgroundMapDataSelectionDialogTest : NUnitFormTestWithHiddenDesktop
    {
        private MockRepository mockRepository;
        private ITileSourceFactory tileFactory;
        private static readonly string testPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms);

        [Test]
        public void Constructor_MapDataNull_DefaultProperties()
        {
            // Setup
            var dialogParent = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
            {
                // Call
                using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
                {
                    // Assert
                    Assert.IsInstanceOf<DialogBase>(dialog);
                    Assert.AreEqual("Selecteer achtergrondkaart", dialog.Text);

                    Icon icon = BitmapToIcon(RiskeerCommonFormsResources.SelectionDialogIcon);
                    Bitmap expectedImage = icon.ToBitmap();
                    Bitmap actualImage = dialog.Icon.ToBitmap();
                    TestHelper.AssertImagesAreEqual(expectedImage, actualImage);

                    AssertMapDataControls<WellKnownMapDataControl>(dialog);
                }
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
            mockRepository.ReplayAll();

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

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

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
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

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
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

                var buttonSelect = (Button) new ButtonTester("selectButton", dialog).TheObject;
                Assert.AreEqual("Selecteren", buttonSelect.Text);
                Assert.IsTrue(buttonSelect.Enabled);

                var buttonCancel = (Button) new ButtonTester("cancelButton", dialog).TheObject;
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

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
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

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
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

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, null))
            {
                dialog.Show();

                var selectButton = (Button) new ButtonTester("selectButton", dialog).TheObject;
                var comboBox = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                var wellKnownMapDataControl = GetComboBoxItem<WellKnownMapDataControl>(comboBox);
                var wmtsLocationControl = GetComboBoxItem<WmtsLocationControl>(comboBox);

                // Precondition state
                comboBox.SelectedItem = wmtsLocationControl;
                DataGridViewControl wmtsDataGridViewControl = dialog.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
                Assert.IsNull(wmtsDataGridViewControl.CurrentRow);
                Assert.IsFalse(selectButton.Enabled);

                // When
                comboBox.SelectedItem = wellKnownMapDataControl;
                DataGridViewControl wellKnownDataGridViewControl = dialog.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
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
            WmtsMapData activeWmtsMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            var capabilities = new[]
            {
                new TestWmtsTileSource(WmtsMapDataTestHelper.CreateAlternativePdokMapData()),
                new TestWmtsTileSource(activeWmtsMapData)
            };

            tileFactory.Expect(tf => tf.GetWmtsTileSources(activeWmtsMapData.SourceCapabilitiesUrl)).Return(capabilities);
            mockRepository.ReplayAll();

            var wmtsLocationControlSelectedMapDataChanged = 0;

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, activeWmtsMapData))
            {
                dialog.Show();

                var comboBox = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                var wellKnownMapDataControl = GetComboBoxItem<WellKnownMapDataControl>(comboBox);
                var wmtsLocationControl = GetComboBoxItem<WmtsLocationControl>(comboBox);

                comboBox.SelectedItem = wmtsLocationControl;

                Button connectButton = dialog.Controls.Find("connectToButton", true).OfType<Button>().First();
                connectButton.PerformClick();

                DataGridViewControl wmtsDataGridViewControl = dialog.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
                wmtsLocationControl.SelectedMapDataChanged += (sender, args) => wmtsLocationControlSelectedMapDataChanged++;

                comboBox.SelectedItem = wellKnownMapDataControl;
                DataGridViewControl wellKnownDataGridViewControl = dialog.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
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
            WmtsMapData activeWmtsMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            mockRepository.ReplayAll();

            var wellKnownSelectedMapDataChanged = 0;

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var dialogParent = new Form())
            using (var dialog = new BackgroundMapDataSelectionDialog(dialogParent, activeWmtsMapData))
            {
                dialog.Show();

                var comboBox = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
                var wellKnownControl = GetComboBoxItem<WellKnownMapDataControl>(comboBox);

                comboBox.SelectedItem = wellKnownControl;

                DataGridViewControl wellKnownDataGridViewControl = dialog.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
                wellKnownControl.SelectedMapDataChanged += (sender, args) => wellKnownSelectedMapDataChanged++;

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

            string settingsDirectory = Path.Combine(testPath, "EmptyWmtsConnectionInfo");
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = settingsDirectory
            }))
            {
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
        }

        public override void Setup()
        {
            mockRepository = new MockRepository();
            tileFactory = mockRepository.StrictMock<ITileSourceFactory>();
        }

        public override void TearDown()
        {
            mockRepository.VerifyAll();
            base.TearDown();
        }

        private static void AssertMapDataControls<T>(BackgroundMapDataSelectionDialog dialog)
            where T : BackgroundMapDataSelectionControl
        {
            var mapLayers = (ComboBox) new ComboBoxTester("mapLayerComboBox", dialog).TheObject;
            Assert.AreEqual("DisplayName", mapLayers.DisplayMember);
            Assert.AreEqual(ComboBoxStyle.DropDownList, mapLayers.DropDownStyle);
            Assert.IsTrue(mapLayers.Sorted);
            Assert.IsInstanceOf<T>(mapLayers.SelectedItem);

            var backgroundMapDataSelectionControls = (BackgroundMapDataSelectionControl[]) mapLayers.DataSource;
            Assert.AreEqual(2, backgroundMapDataSelectionControls.Length);
            Assert.IsInstanceOf<WellKnownMapDataControl>(backgroundMapDataSelectionControls[0]);
            Assert.IsInstanceOf<WmtsLocationControl>(backgroundMapDataSelectionControls[1]);

            Assert.IsInstanceOf<BackgroundMapDataSelectionControl[]>(mapLayers.DataSource);
            Assert.AreEqual("DisplayName", mapLayers.DisplayMember);

            var groupBoxProperties = (GroupBox) new ControlTester("propertiesGroupBox", dialog).TheObject;
            Assert.AreEqual(DockStyle.Fill, groupBoxProperties.Dock);
            Assert.AreEqual(1, groupBoxProperties.Controls.Count);
            Assert.AreSame(mapLayers.SelectedItem, groupBoxProperties.Controls[0]);
        }

        private static T GetComboBoxItem<T>(ComboBox comboBox)
        {
            return ((BackgroundMapDataSelectionControl[]) comboBox.DataSource).OfType<T>().First();
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}