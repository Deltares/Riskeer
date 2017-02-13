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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test.Views
{
    [TestFixture]
    public class WmtsLocationControlTest : NUnitFormTest
    {
        private const int mapLayerIdColumnIndex = 0;
        private const int mapLayerFormatColumnIndex = 1;
        private const int mapLayerTitleColumnIndex = 2;
        private const int mapLayerCoordinateSystemColumnIndex = 3;

        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var control = new WmtsLocationControl())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(control);
                Assert.IsInstanceOf<IView>(control);
                Assert.IsNull(control.Data);
            }
        }

        [Test]
        public void Show_AddedToForm_DefaultProperties()
        {
            // Setup
            using (var control = new WmtsLocationControl())
            using (var form = new Form())
            {
                // Call
                form.Controls.Add(control);

                // Assert
                var urlLocationLabel = new LabelTester("urlLocationLabel", form);
                Assert.AreEqual("Locatie (URL)", urlLocationLabel.Text);

                var urlLocations = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                Assert.AreEqual(ComboBoxStyle.DropDownList, urlLocations.DropDownStyle);
                Assert.IsInstanceOf<List<WmtsConnectionInfo>>(urlLocations.DataSource);
                Assert.AreEqual("Name", urlLocations.DisplayMember);
                Assert.AreEqual("Url", urlLocations.ValueMember);
                Assert.IsTrue(urlLocations.Sorted);

                var buttonConnectTo = new ButtonTester("connectToButton", form);
                Assert.AreEqual("Verbinding maken", buttonConnectTo.Text);

                var buttonAddLocation = new ButtonTester("addLocationButton", form);
                Assert.AreEqual("Locatie toevoegen...", buttonAddLocation.Text);

                var buttonEditLocation = new ButtonTester("editLocationButton", form);
                Assert.AreEqual("Locatie aanpassen...", buttonEditLocation.Text);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (var control = new WmtsLocationControl())
            using (var form = new Form())
            {
                form.Controls.Add(control);

                // Assert
                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                var dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();

                Assert.AreEqual(DataGridViewSelectionMode.FullRowSelect, dataGridView.SelectionMode);
                Assert.IsFalse(dataGridView.MultiSelect);
                Assert.AreEqual(4, dataGridView.ColumnCount);

                var mapLayerIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerIdColumnIndex];
                Assert.AreEqual("Kaartlaag", mapLayerIdColumn.HeaderText);
                Assert.AreEqual("Id", mapLayerIdColumn.DataPropertyName);
                Assert.IsTrue(mapLayerIdColumn.ReadOnly);

                var mapLayerFormatColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerFormatColumnIndex];
                Assert.AreEqual("Formaat", mapLayerFormatColumn.HeaderText);
                Assert.AreEqual("Format", mapLayerFormatColumn.DataPropertyName);
                Assert.IsTrue(mapLayerFormatColumn.ReadOnly);

                var mapLayerTitleColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerTitleColumnIndex];
                Assert.AreEqual("Titel", mapLayerTitleColumn.HeaderText);
                Assert.AreEqual("Title", mapLayerTitleColumn.DataPropertyName);
                Assert.IsTrue(mapLayerTitleColumn.ReadOnly);

                var mapLayerCoordinateSystemColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerCoordinateSystemColumnIndex];
                Assert.AreEqual("Coördinatenstelsel", mapLayerCoordinateSystemColumn.HeaderText);
                Assert.AreEqual("CoordinateSystem", mapLayerCoordinateSystemColumn.DataPropertyName);
                Assert.IsTrue(mapLayerCoordinateSystemColumn.ReadOnly);
            }
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () =>
            {
                using (var control = new WmtsLocationControl())
                {
                    control.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Data_WmtsCapabilityRow_DataSet()
        {
            // Setup
            using (var view = new WmtsLocationControl())
            {
                var capabilityRows = Enumerable.Empty<WmtsCapabilityRow>();

                // Call
                view.Data = capabilityRows;

                // Assert
                Assert.AreSame(capabilityRows, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanWmtsCapabilityRow_DataNull()
        {
            // Setup
            using (var view = new WmtsLocationControl())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void WmtsLocationControl_WithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (Form form = ShowFullyConfiguredWmtsLocationControl())
            {
                // Assert
                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                var rows = dataGridViewControl.Rows;
                Assert.AreEqual(2, rows.Count);

                var cells = rows[0].Cells;
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

        [Test]
        public void GivenWmtsLocationControlAndAddLocationClicked_WhenDialogCanceled_ThenWmtsLocationsNotUpdated()
        {
            // Given
            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            using (var form = new Form())
            using (var control = new WmtsLocationControl())
            {
                form.Controls.Add(control);
                form.Show();

                var buttonAddLocation = new ButtonTester("addLocationButton", form);

                // When
                buttonAddLocation.Click();

                // Then
                var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                var dataSource = (List<WmtsConnectionInfo>) comboBox.DataSource;
                Assert.AreEqual(0, dataSource.Count);
            }
        }

        [Test]
        public void GivenWmtsLocationControlAndAddLocationClicked_WhenValidDataInDialog_ThenWmtsLocationsUpdated()
        {
            // Given
            const string name = @"someName";
            const string url = @"someUrl";

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

            using (var form = new Form())
            using (var control = new WmtsLocationControl())
            {
                form.Controls.Add(control);
                form.Show();

                var buttonAddLocation = new ButtonTester("addLocationButton", form);

                // When
                buttonAddLocation.Click();

                // Then
                var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                var dataSource = (List<WmtsConnectionInfo>) comboBox.DataSource;
                Assert.AreEqual(1, dataSource.Count);
                var item = (WmtsConnectionInfo) comboBox.Items[0];
                Assert.AreEqual(name, item.Name);
                Assert.AreEqual(url, item.Url);
            }
        }

        [Test]
        public void GivenWmtsLocationControlAndEditLocationClicked_WhenDialogCanceled_ThenWmtsLocationsNotUpdated()
        {
            // Given
            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            using (var form = new Form())
            using (var control = new WmtsLocationControl())
            {
                form.Controls.Add(control);
                form.Show();

                var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                comboBox.DataSource = new List<WmtsConnectionInfo>
                {
                    new WmtsConnectionInfo("oldName", "oldUrl")
                };

                var buttonAddLocation = new ButtonTester("editLocationButton", form);

                // When
                buttonAddLocation.Click();

                // Then
                var dataSource = (List<WmtsConnectionInfo>) comboBox.DataSource;
                Assert.AreEqual(1, dataSource.Count);
                var item = (WmtsConnectionInfo) comboBox.Items[0];
                Assert.AreEqual("oldName", item.Name);
                Assert.AreEqual("oldUrl", item.Url);
            }
        }

        [Test]
        public void GivenWmtsLocationControlAndEditLocationClicked_WhenValidDataInDialog_ThenWmtsLocationsUpdated()
        {
            // Given
            const string newName = @"newName";
            const string newUrl = @"newUrl";

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

            using (var form = new Form())
            using (var control = new WmtsLocationControl())
            {
                form.Controls.Add(control);
                form.Show();

                var comboBox = (ComboBox) new ComboBoxTester("urlLocationComboBox", form).TheObject;
                comboBox.DataSource = new List<WmtsConnectionInfo>
                {
                    new WmtsConnectionInfo("oldName", "oldUrl")
                };

                var buttonAddLocation = new ButtonTester("editLocationButton", form);

                // When
                buttonAddLocation.Click();

                // Then
                var dataSource = (List<WmtsConnectionInfo>) comboBox.DataSource;
                Assert.AreEqual(1, dataSource.Count);
                var item = (WmtsConnectionInfo) comboBox.Items[0];
                Assert.AreEqual(newName, item.Name);
                Assert.AreEqual(newUrl, item.Url);
            }
        }

        [Test]
        public void Dispose_DisposedAlreadyCalled_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () =>
            {
                using (var control = new WmtsLocationControl())
                {
                    control.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        private static Form ShowFullyConfiguredWmtsLocationControl()
        {
            var form = new Form();
            var control = new WmtsLocationControl();

            var capabilities = new List<WmtsCapabilityRow>
            {
                new WmtsCapabilityRow(new WmtsCapability("-", "image/png", "-", "-")),
                new WmtsCapabilityRow(new WmtsCapability("brtachtergrondkaart(EPSG:28992)", "image/png8", "brtachtergrondkaart", "EPSG:28992"))
            };

            control.Data = capabilities;

            form.Controls.Add(control);
            return form;
        }
    }
}