// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BruTile;
using BruTile.Wmts;
using Core.Common.Controls.DataGrid;
using Core.Common.Gui.Settings;
using Core.Common.Gui.TestUtil.Settings;
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;
using Core.Components.Gis.Forms.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.Gis.Forms.Test.Views
{
    [TestFixture]
    public class WmtsLocationControlTest : NUnitFormTest
    {
        private const int mapLayerIdColumnIndex = 0;
        private const int mapLayerFormatColumnIndex = 1;
        private const int mapLayerTitleColumnIndex = 2;
        private const int mapLayerCoordinateSystemColumnIndex = 3;
        private const string wmtsconnectioninfoConfigFile = "wmtsConnectionInfo.config";

        private static readonly TestDataPath testPath = TestDataPath.Core.Components.Gis.IO;

        private MockRepository mockRepository;
        private ITileSourceFactory tileFactory;
        private IWmtsCapabilityFactory wmtsCapabilityFactory;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            tileFactory = mockRepository.StrictMock<ITileSourceFactory>();
            wmtsCapabilityFactory = mockRepository.StrictMock<IWmtsCapabilityFactory>();
        }

        [TearDown]
        public override void TearDown()
        {
            mockRepository.VerifyAll();
            base.TearDown();
        }

        [Test]
        public void Constructor_WmtsCapabilityFactoryNull_ThrowArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new WmtsLocationControl(null, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("wmtsCapabilityFactory", exception.ParamName);
        }

        [Test]
        public void Constructor_ActiveWmtsMapDataNull_DefaultValues()
        {
            // Setup
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                {
                    // Assert
                    Assert.IsInstanceOf<UserControl>(control);
                    Assert.IsInstanceOf<IBackgroundMapDataSelectionControl>(control);
                    Assert.AreEqual("Web Map Tile Service (WMTS)", control.DisplayName);
                    Assert.IsNull(control.SelectedMapData);
                    Assert.AreSame(control, control.UserControl);
                }
            }
        }

        [Test]
        public void Constructor_ValidWmtsMapData_ExpectedProperties()
        {
            // Setup
            WmtsMapData activeWmtsMapData = WmtsMapData.CreateDefaultPdokMapData();

            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(activeWmtsMapData.SourceCapabilitiesUrl)).Return(new[]
            {
                CreateWmtsCapability(new TestWmtsTileSource(activeWmtsMapData))
            });
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                using (var control = new WmtsLocationControl(activeWmtsMapData, wmtsCapabilityFactory))
                {
                    // Assert
                    AssertAreEqual(activeWmtsMapData, control.SelectedMapData);
                }
            }
        }

        [Test]
        public void Show_AddedToForm_DefaultProperties()
        {
            // Setup
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
            using (var form = new Form())
            {
                // Call
                form.Controls.Add(control);

                // Assert
                var urlLocationLabel = new LabelTester("urlLocationLabel", form);
                Assert.AreEqual("Locatie (URL)", urlLocationLabel.Text);

                var urlLocations = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                Assert.AreEqual(ComboBoxStyle.DropDownList, urlLocations.DropDownStyle);
                Assert.IsInstanceOf<ICollection<WmtsConnectionInfo>>(urlLocations.DataSource);
                Assert.AreEqual("Name", urlLocations.DisplayMember);
                Assert.AreEqual("Url", urlLocations.ValueMember);
                Assert.IsTrue(urlLocations.Sorted);
                Assert.IsNull(urlLocations.SelectedItem);

                var buttonConnectTo = (Button) new ButtonTester("connectToButton", form).TheObject;
                Assert.AreEqual("Verbinding maken", buttonConnectTo.Text);
                Assert.IsFalse(buttonConnectTo.Enabled);

                var buttonAddLocation = new ButtonTester("addLocationButton", form);
                Assert.AreEqual("Locatie toevoegen...", buttonAddLocation.Text);

                var buttonEditLocation = (Button) new ButtonTester("editLocationButton", form).TheObject;
                Assert.AreEqual("Locatie aanpassen...", buttonEditLocation.Text);
            }
        }

        [Test]
        public void Show_AddedToFormWithWmtsMapData_DefaultProperties()
        {
            // Setup
            WmtsMapData activeWmtsMapData = WmtsMapData.CreateDefaultPdokMapData();
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(activeWmtsMapData.SourceCapabilitiesUrl))
                                 .Return(new[]
                                 {
                                     CreateWmtsCapability(new TestWmtsTileSource(activeWmtsMapData))
                                 });
            mockRepository.ReplayAll();

            var activeWmtsConnectionInfo = new WmtsConnectionInfo(activeWmtsMapData.Name, activeWmtsMapData.SourceCapabilitiesUrl);

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var control = new WmtsLocationControl(activeWmtsMapData, wmtsCapabilityFactory))
            using (var form = new Form())
            {
                // Call
                form.Controls.Add(control);

                // Assert
                var urlLocationLabel = new LabelTester("urlLocationLabel", form);
                Assert.AreEqual("Locatie (URL)", urlLocationLabel.Text);

                var urlLocations = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                Assert.AreEqual(ComboBoxStyle.DropDownList, urlLocations.DropDownStyle);
                var connectionInfos = (IList<WmtsConnectionInfo>) urlLocations.DataSource;
                Assert.Contains(activeWmtsConnectionInfo, connectionInfos.ToArray());

                Assert.AreEqual("Name", urlLocations.DisplayMember);
                Assert.AreEqual("Url", urlLocations.ValueMember);
                Assert.IsTrue(urlLocations.Sorted);
                Assert.IsNotNull(urlLocations.SelectedItem);

                var buttonConnectTo = (Button) new ButtonTester("connectToButton", form).TheObject;
                Assert.AreEqual("Verbinding maken", buttonConnectTo.Text);
                Assert.IsTrue(buttonConnectTo.Enabled);
            }
        }

        [Test]
        public void Constructor_NullMapData_DataGridViewCorrectlyInitialized()
        {
            // Setup
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                using (var form = new Form())
                {
                    form.Controls.Add(control);

                    // Assert
                    var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                    var dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();

                    Assert.AreEqual(DataGridViewSelectionMode.FullRowSelect, dataGridViewControl.SelectionMode);
                    Assert.IsFalse(dataGridViewControl.MultiSelect);
                    Assert.AreEqual(4, dataGridView.ColumnCount);

                    var mapLayerIdColumn = (DataGridViewTextBoxColumn) dataGridViewControl.GetColumnFromIndex(mapLayerIdColumnIndex);
                    Assert.AreEqual("Kaartlaag", mapLayerIdColumn.HeaderText);
                    Assert.AreEqual("Id", mapLayerIdColumn.DataPropertyName);
                    Assert.IsTrue(mapLayerIdColumn.ReadOnly);

                    var mapLayerFormatColumn = (DataGridViewTextBoxColumn) dataGridViewControl.GetColumnFromIndex(mapLayerFormatColumnIndex);
                    Assert.AreEqual("Formaat", mapLayerFormatColumn.HeaderText);
                    Assert.AreEqual("Format", mapLayerFormatColumn.DataPropertyName);
                    Assert.IsTrue(mapLayerFormatColumn.ReadOnly);

                    var mapLayerTitleColumn = (DataGridViewTextBoxColumn) dataGridViewControl.GetColumnFromIndex(mapLayerTitleColumnIndex);
                    Assert.AreEqual("Titel", mapLayerTitleColumn.HeaderText);
                    Assert.AreEqual("Title", mapLayerTitleColumn.DataPropertyName);
                    Assert.IsTrue(mapLayerTitleColumn.ReadOnly);

                    var mapLayerCoordinateSystemColumn = (DataGridViewTextBoxColumn) dataGridViewControl.GetColumnFromIndex(mapLayerCoordinateSystemColumnIndex);
                    Assert.AreEqual("Coördinatenstelsel", mapLayerCoordinateSystemColumn.HeaderText);
                    Assert.AreEqual("CoordinateSystem", mapLayerCoordinateSystemColumn.DataPropertyName);
                    Assert.IsTrue(mapLayerCoordinateSystemColumn.ReadOnly);
                }
            }
        }

        [Test]
        public void Constructor_ValidMapDataWithITileSources_ExpectedDataGrid()
        {
            // Setup
            WmtsMapData activeWmtsMapData = WmtsMapData.CreateDefaultPdokMapData();

            var capabilities = new[]
            {
                CreateWmtsCapability(new TestWmtsTileSource(WmtsMapData.CreateAlternativePdokMapData())),
                CreateWmtsCapability(new TestWmtsTileSource(activeWmtsMapData))
            };

            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(activeWmtsMapData.SourceCapabilitiesUrl)).Return(capabilities);
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                using (var control = new WmtsLocationControl(activeWmtsMapData, wmtsCapabilityFactory))
                using (var form = new Form())
                {
                    form.Controls.Add(control);

                    // Assert
                    var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                    Assert.AreEqual(2, dataGridViewControl.Rows.Count);
                    DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
                    Assert.IsNotNull(currentRow);
                    Assert.AreEqual(1, currentRow.Cells[0].RowIndex);
                }
            }
        }

        [Test]
        public void Constructor_ValidMapDataWithoutITileSources_DataGridEmpty()
        {
            // Setup
            WmtsMapData activeWmtsMapData = WmtsMapData.CreateDefaultPdokMapData();

            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(activeWmtsMapData.SourceCapabilitiesUrl)).Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                using (var control = new WmtsLocationControl(activeWmtsMapData, wmtsCapabilityFactory))
                using (var form = new Form())
                {
                    form.Controls.Add(control);

                    // Assert
                    var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                    Assert.IsEmpty(dataGridViewControl.Rows);
                }
            }
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                TestDelegate call = () =>
                {
                    using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                    {
                        control.Dispose();
                    }
                };

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void WmtsLocationControl_WithData_DataGridViewCorrectlyInitialized()
        {
            // Setup
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(null))
                                 .IgnoreArguments()
                                 .Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath, "noConfig")
            }))
            using (var form = new Form())
            {
                // Call
                using (ShowFullyConfiguredWmtsLocationControl(form, wmtsCapabilityFactory))
                {
                    // Assert
                    var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                    DataGridViewRowCollection rows = dataGridViewControl.Rows;
                    Assert.AreEqual(2, rows.Count);

                    DataGridViewCellCollection cells = rows[0].Cells;
                    Assert.AreEqual(4, cells.Count);
                    Assert.AreEqual("-", cells[mapLayerIdColumnIndex].FormattedValue);
                    Assert.AreEqual("image/png", cells[mapLayerFormatColumnIndex].FormattedValue);
                    Assert.AreEqual("-", cells[mapLayerTitleColumnIndex].FormattedValue);
                    Assert.AreEqual("-", cells[mapLayerCoordinateSystemColumnIndex].FormattedValue);

                    cells = rows[1].Cells;
                    Assert.AreEqual(4, cells.Count);
                    Assert.AreEqual("brtachtergrondkaart(EPSG:28992)", cells[mapLayerIdColumnIndex].FormattedValue);
                    Assert.AreEqual("image/png8", cells[mapLayerFormatColumnIndex].FormattedValue);
                    Assert.AreEqual("brtachtergrondkaart", cells[mapLayerTitleColumnIndex].FormattedValue);
                    Assert.AreEqual("EPSG:28992", cells[mapLayerCoordinateSystemColumnIndex].FormattedValue);
                }
            }
        }

        [Test]
        public void GetSelectedMapData_WithoutSelectedData_ReturnsNull()
        {
            // Setup
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var form = new Form())
            using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
            {
                form.Controls.Add(control);
                form.Show();

                // Call
                MapData selectedMapData = control.SelectedMapData;

                // Assert
                Assert.IsNull(selectedMapData);
            }
        }

        [Test]
        public void GetSelectedMapData_WithSelectedComboBoxWithoutSelectedRow_ReturnsNull()
        {
            // Setup
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(null)).IgnoreArguments().Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath, "noConfig")
            }))
            using (var form = new Form())
            using (WmtsLocationControl control = ShowFullyConfiguredWmtsLocationControl(form, wmtsCapabilityFactory))
            {
                // Call                
                MapData selectedMapData = control.SelectedMapData;

                // Assert
                Assert.IsNull(selectedMapData);
            }
        }

        [Test]
        public void GetSelectedMapData_WithSelectedData_ReturnsSelectedMapData()
        {
            // Setup
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(null)).IgnoreArguments().Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath, "noConfig")
            }))
            using (var form = new Form())
            using (WmtsLocationControl control = ShowFullyConfiguredWmtsLocationControl(form, wmtsCapabilityFactory))
            {
                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(1, 0));

                // Call
                WmtsMapData selectedMapData = control.SelectedMapData;

                // Assert
                Assert.IsNotNull(selectedMapData);
                Assert.AreEqual("PDOK achtergrondkaart", selectedMapData.Name);
                Assert.AreEqual("brtachtergrondkaart(EPSG:28992)", selectedMapData.SelectedCapabilityIdentifier);
                Assert.AreEqual("https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities",
                                selectedMapData.SourceCapabilitiesUrl);
                Assert.AreEqual("image/png8", selectedMapData.PreferredFormat);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidWmtsConnectionInfos_WhenConstructed_ThenExpectedProperties()
        {
            // Given
            const string url = "https://geodata.nationaalgeoregister.nl/tiles/service/wmts/ahn1?request=GetCapabilities";
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(url)).Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            var settingsHelper = new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath)
            };
            settingsHelper.SetApplicationVersion("twoValidWmtsConnectionInfos");

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (new UseCustomSettingsHelper(settingsHelper))
            {
                // When
                using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                using (var form = new Form())
                {
                    form.Controls.Add(control);

                    // Then
                    var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                    var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                    Assert.AreEqual(2, dataSource.Count);

                    var firstWmtsConnectionInfo = (WmtsConnectionInfo) comboBox.Items[0];
                    Assert.AreEqual("Actueel Hoogtebestand Nederland (AHN1)", firstWmtsConnectionInfo.Name);
                    Assert.AreEqual(url, firstWmtsConnectionInfo.Url);

                    var secondWmtsConnectionInfo = (WmtsConnectionInfo) comboBox.Items[1];
                    Assert.AreEqual("Zeegraskartering", secondWmtsConnectionInfo.Name);
                    Assert.AreEqual("https://geodata.nationaalgeoregister.nl/zeegraskartering/wfs?request=GetCapabilities",
                                    secondWmtsConnectionInfo.Url);
                }
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenInvalidWmtsConnectionInfos_WhenConstructed_ThenLogGenerated()
        {
            // Given
            mockRepository.ReplayAll();

            var settingsHelper = new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath)
            };
            settingsHelper.SetApplicationVersion("WmtsConnectionInfosWithoutWmtsConnectionsElement");

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (new UseCustomSettingsHelper(settingsHelper))
            {
                // When
                Action action = () =>
                {
                    using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                    using (var form = new Form())
                    {
                        form.Controls.Add(control);

                        // Then
                        var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                        var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                        Assert.AreEqual(0, dataSource.Count);
                    }
                };

                string wmtsConnectionInfoConfig = Path.Combine(TestHelper.GetTestDataPath(
                                                                   testPath,
                                                                   "WmtsConnectionInfosWithoutWmtsConnectionsElement"),
                                                               "wmtsConnectionInfo.config");
                var expectedMessage = $"Fout bij het lezen van bestand '{wmtsConnectionInfoConfig}': "
                                      + "het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(action, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenWmtsLocationControlAndAddLocationClicked_WhenDialogCanceled_ThenWmtsLocationsNotUpdated()
        {
            // Given
            mockRepository.ReplayAll();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath, "noConfig")
            }))
            using (var form = new Form())
            using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
            {
                form.Controls.Add(control);
                form.Show();

                var buttonAddLocation = new ButtonTester("addLocationButton", form);

                // When
                buttonAddLocation.Click();

                // Then
                var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                Assert.AreEqual(0, dataSource.Count);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenWmtsLocationControlAndAddLocationClicked_WhenValidDataInDialog_ThenWmtsLocationsUpdated()
        {
            // Given
            const string name = @"someName";
            const string url = @"someUrl";

            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(url)).Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (var formTester = new FormTester(formName))
                {
                    var dialog = (WmtsConnectionDialog) formTester.TheObject;
                    var nameTextBox = (TextBox) new TextBoxTester("nameTextBox", dialog).TheObject;
                    var urlTextBox = (TextBox) new TextBoxTester("urlTextBox", dialog).TheObject;
                    var actionButton = new ButtonTester("actionButton", dialog);

                    nameTextBox.Text = name;
                    urlTextBox.Text = url;

                    actionButton.Click();
                }
            };

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath, "noConfig")
            }))
            using (new FileDisposeHelper(Path.Combine(SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(),
                                                      wmtsconnectioninfoConfigFile)))
            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var form = new Form())
            using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
            {
                form.Controls.Add(control);
                form.Show();

                var buttonAddLocation = new ButtonTester("addLocationButton", form);

                // When
                buttonAddLocation.Click();

                // Then
                var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                Assert.AreEqual(1, dataSource.Count);
                var item = (WmtsConnectionInfo) comboBox.Items[0];
                Assert.AreEqual(name, item.Name);
                Assert.AreEqual(url, item.Url);

                var connectToButton = (Button) new ButtonTester("connectToButton", form).TheObject;
                Assert.IsTrue(connectToButton.Enabled);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenWmtsLocationControlAndAddLocationClicked_WhenInValidDataInDialog_ThenWmtsLocationsNotUpdated()
        {
            // Given
            mockRepository.ReplayAll();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (var formTester = new FormTester(formName))
                {
                    var dialog = (WmtsConnectionDialog) formTester.TheObject;
                    var actionButtonTester = new ButtonTester("actionButton", dialog);
                    var actionButton = (Button) actionButtonTester.TheObject;

                    actionButton.Enabled = true;
                    actionButtonTester.Click();
                }
            };

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath, "noConfig")
            }))
            using (var form = new Form())
            using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
            {
                form.Controls.Add(control);
                form.Show();

                var buttonAddLocation = new ButtonTester("addLocationButton", form);

                // When
                buttonAddLocation.Click();

                // Then
                var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                Assert.AreEqual(0, dataSource.Count);

                var connectToButton = (Button) new ButtonTester("connectToButton", form).TheObject;
                Assert.IsFalse(connectToButton.Enabled);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenWmtsLocationControlAndAddLocationClicked_WhenConfigFileInUse_ThenWmtsLocationsNotUpdatedAndLogGenerated()
        {
            // Given
            const string name = @"someName";
            const string url = @"someUrl";

            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(url)).Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (var formTester = new FormTester(formName))
                {
                    var dialog = (WmtsConnectionDialog) formTester.TheObject;
                    var nameTextBox = (TextBox) new TextBoxTester("nameTextBox", dialog).TheObject;
                    var urlTextBox = (TextBox) new TextBoxTester("urlTextBox", dialog).TheObject;
                    var actionButton = new ButtonTester("actionButton", dialog);

                    nameTextBox.Text = name;
                    urlTextBox.Text = url;

                    actionButton.Click();
                }
            };

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetTestDataPath(testPath, "noConfig")
            }))
            {
                string configFilePath = Path.Combine(SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(),
                                                     wmtsconnectioninfoConfigFile);

                using (var fileDisposeHelper = new FileDisposeHelper(configFilePath))
                using (new UseCustomTileSourceFactoryConfig(tileFactory))
                using (var form = new Form())
                using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                {
                    form.Controls.Add(control);
                    form.Show();

                    fileDisposeHelper.LockFiles();

                    var buttonAddLocation = new ButtonTester("addLocationButton", form);

                    // When
                    Action action = () => { buttonAddLocation.Click(); };

                    // Then
                    string exceptionMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{configFilePath}'.";
                    TestHelper.AssertLogMessageWithLevelIsGenerated(action, Tuple.Create(exceptionMessage, LogLevelConstant.Error));
                    var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                    var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                    Assert.AreEqual(1, dataSource.Count);
                }
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenWmtsLocationControlAndEditLocationClicked_WhenDialogCanceled_ThenWmtsLocationsNotUpdated()
        {
            // Given
            const string capabilitiesName = "oldName";
            const string capabilitiesUrl = "oldUrl";
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(capabilitiesUrl))
                                 .Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetScratchPadPath(nameof(GivenWmtsLocationControlAndEditLocationClicked_WhenDialogCanceled_ThenWmtsLocationsNotUpdated))
            }))
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(GivenWmtsLocationControlAndEditLocationClicked_WhenDialogCanceled_ThenWmtsLocationsNotUpdated)))
            {
                string filePath = Path.Combine(SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(),
                                               wmtsconnectioninfoConfigFile);

                WriteToFile(filePath, new WmtsConnectionInfo(capabilitiesName, capabilitiesUrl));

                using (new UseCustomTileSourceFactoryConfig(tileFactory))
                using (var form = new Form())
                using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                {
                    form.Controls.Add(control);
                    form.Show();

                    var editLocationButton = new ButtonTester("editLocationButton", form);

                    // When
                    editLocationButton.Click();

                    // Then
                    var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                    var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                    Assert.AreEqual(1, dataSource.Count);
                    var item = (WmtsConnectionInfo) comboBox.Items[0];
                    Assert.AreEqual(capabilitiesName, item.Name);
                    Assert.AreEqual(capabilitiesUrl, item.Url);
                }
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenWmtsLocationControlAndEditLocationClicked_WhenValidDataInDialog_ThenWmtsLocationsUpdated()
        {
            // Given
            const string newName = "newName";
            const string newUrl = "newUrl";
            const string oldUrl = "oldUrl";

            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(oldUrl)).Return(Enumerable.Empty<WmtsCapability>());
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(newUrl)).Return(Enumerable.Empty<WmtsCapability>());
            mockRepository.ReplayAll();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (var formTester = new FormTester(formName))
                {
                    var dialog = (WmtsConnectionDialog) formTester.TheObject;
                    var nameTextBox = (TextBox) new TextBoxTester("nameTextBox", dialog).TheObject;
                    var urlTextBox = (TextBox) new TextBoxTester("urlTextBox", dialog).TheObject;
                    var actionButton = new ButtonTester("actionButton", dialog);

                    nameTextBox.Text = newName;
                    urlTextBox.Text = newUrl;

                    actionButton.Click();
                }
            };

            using (new UseCustomSettingsHelper(new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetScratchPadPath(nameof(GivenWmtsLocationControlAndEditLocationClicked_WhenValidDataInDialog_ThenWmtsLocationsUpdated))
            }))
            {
                using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(GivenWmtsLocationControlAndEditLocationClicked_WhenValidDataInDialog_ThenWmtsLocationsUpdated)))
                {
                    string filePath = Path.Combine(SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(),
                                                   wmtsconnectioninfoConfigFile);

                    WriteToFile(filePath, new WmtsConnectionInfo("oldName", oldUrl));

                    using (new UseCustomTileSourceFactoryConfig(tileFactory))
                    using (var form = new Form())
                    using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                    {
                        form.Controls.Add(control);
                        form.Show();

                        var editLocationButton = new ButtonTester("editLocationButton", form);

                        // When
                        editLocationButton.Click();

                        // Then
                        var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                        var dataSource = (IList<WmtsConnectionInfo>) comboBox.DataSource;
                        Assert.AreEqual(1, dataSource.Count);
                        var item = (WmtsConnectionInfo) comboBox.Items[0];
                        Assert.AreEqual(newName, item.Name);
                        Assert.AreEqual(newUrl, item.Url);
                    }
                }
            }
        }

        [Test]
        public void GivenWmtsLocationControlAndConnectClicked_WhenValidDataFromUrl_ThenDataGridUpdated()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();

            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(backgroundMapData.SourceCapabilitiesUrl))
                                 .Return(new[]
                                 {
                                     CreateWmtsCapability(new TestWmtsTileSource(backgroundMapData))
                                 }).Repeat.Twice();
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var form = new Form())
            using (ShowValidWmtsLocationControl(form, wmtsCapabilityFactory))
            {
                form.Show();
                var connectToButton = new ButtonTester("connectToButton", form);

                // When
                connectToButton.Click();

                // Then
                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                DataGridViewRowCollection rows = dataGridViewControl.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("brtachtergrondkaart(EPSG:28992)", cells[mapLayerIdColumnIndex].FormattedValue);
                Assert.AreEqual("image/png", cells[mapLayerFormatColumnIndex].FormattedValue);
                Assert.AreEqual("Stub schema", cells[mapLayerTitleColumnIndex].FormattedValue);
                Assert.AreEqual("EPSG:28992", cells[mapLayerCoordinateSystemColumnIndex].FormattedValue);
            }
        }

        [Test]
        public void GivenWmtsLocationControlAndConnectClicked_WhenCannotFindTileSourceException_ThenErrorMessageShown()
        {
            // Given
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(null)).IgnoreArguments().Return(Enumerable.Empty<WmtsCapability>());
            wmtsCapabilityFactory.Expect(wcf => wcf.GetWmtsCapabilities(null)).IgnoreArguments().Throw(new CannotFindTileSourceException("error"));
            mockRepository.ReplayAll();

            string messageBoxTitle = null;
            string messageBoxText = null;
            DialogBoxHandler = (formName, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxTitle = messageBox.Title;
                messageBoxText = messageBox.Text;
                messageBox.ClickOk();
            };

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            using (var form = new Form())
            using (ShowValidWmtsLocationControl(form, wmtsCapabilityFactory))
            {
                form.Show();
                var connectToButton = new ButtonTester("connectToButton", form);

                // When
                connectToButton.Click();

                // Then
                Assert.AreEqual("Fout", messageBoxTitle);
                Assert.AreEqual("Gegevens ophalen van de locatie (URL) 'PDOK achtergrondkaart' is mislukt.", messageBoxText);
            }
        }

        [Test]
        public void Dispose_DisposedAlreadyCalled_DoesNotThrowException()
        {
            // Setup
            mockRepository.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(tileFactory))
            {
                // Call
                TestDelegate call = () =>
                {
                    using (var control = new WmtsLocationControl(null, wmtsCapabilityFactory))
                    {
                        control.Dispose();
                    }
                };

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        private static WmtsCapability CreateWmtsCapability(ITileSource tileSource)
        {
            var wmtsTileSchema = (WmtsTileSchema) tileSource.Schema;
            return new WmtsCapability(wmtsTileSchema.Identifier, wmtsTileSchema.Format,
                                      tileSource.Name, wmtsTileSchema.Srs);
        }

        private static void AssertAreEqual(WmtsMapData expected, WmtsMapData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.PreferredFormat, actual.PreferredFormat);
            Assert.AreEqual(expected.SelectedCapabilityIdentifier, actual.SelectedCapabilityIdentifier);
            Assert.AreEqual(expected.SourceCapabilitiesUrl, actual.SourceCapabilitiesUrl);
        }

        private static WmtsLocationControl ShowFullyConfiguredWmtsLocationControl(Form form, IWmtsCapabilityFactory wmtsCapabilityFactory)
        {
            WmtsLocationControl control = ShowValidWmtsLocationControl(form, wmtsCapabilityFactory);

            var capabilities = new[]
            {
                new WmtsCapability("-", "image/png", "-", "-"),
                new WmtsCapability("brtachtergrondkaart(EPSG:28992)", "image/png8", "brtachtergrondkaart", "EPSG:28992")
            };

            var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
            dataGridViewControl.SetDataSource(capabilities);

            return control;
        }

        private static WmtsLocationControl ShowValidWmtsLocationControl(Form form, IWmtsCapabilityFactory wmtsCapabilityFactory)
        {
            var control = new WmtsLocationControl(null, wmtsCapabilityFactory);
            form.Controls.Add(control);

            var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
            comboBox.DataSource = new List<WmtsConnectionInfo>
            {
                new WmtsConnectionInfo("PDOK achtergrondkaart", "https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities")
            };

            var connectToButton = (Button) new ButtonTester("connectToButton", form).TheObject;
            connectToButton.Enabled = true;
            return control;
        }

        private static void WriteToFile(string filePath, WmtsConnectionInfo wmtsConnectionInfo)
        {
            File.WriteAllText(filePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><WmtsConnections><WmtsConnection>" +
                                        $"<Name>{wmtsConnectionInfo.Name}</Name><URL>{wmtsConnectionInfo.Url}</URL>" +
                                        "</WmtsConnection></WmtsConnections>");
        }
    }
}