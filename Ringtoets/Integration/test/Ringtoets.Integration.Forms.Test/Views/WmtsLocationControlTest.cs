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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
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
            }
        }

        [Test]
        public void Show_DefaultProperties()
        {
            // Call
            using (var control = new WmtsLocationControl())
            using (var form = new Form())
            {
                form.Controls.Add(control);

                // Assert
                var urlLocationLabel = new LabelTester("urlLocationLabel", form);
                Assert.AreEqual("Locatie (URL)", urlLocationLabel.Text);

                var urlLocations = new ComboBoxTester("urlLocationComboBox", form);
                Assert.IsNotNull(urlLocations);

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
                Assert.AreEqual(4, dataGridView.ColumnCount);

                var mapLayerIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerIdColumnIndex];
                Assert.AreEqual("Kaartlaag", mapLayerIdColumn.HeaderText);
                Assert.IsTrue(mapLayerIdColumn.ReadOnly);

                var mapLayerFormatColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerFormatColumnIndex];
                Assert.AreEqual("Formaat", mapLayerFormatColumn.HeaderText);
                Assert.IsTrue(mapLayerFormatColumn.ReadOnly);

                var mapLayerTitleColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerTitleColumnIndex];
                Assert.AreEqual("Titel", mapLayerTitleColumn.HeaderText);
                Assert.IsTrue(mapLayerTitleColumn.ReadOnly);

                var mapLayerCoordinateSystemColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[mapLayerCoordinateSystemColumnIndex];
                Assert.AreEqual("Coördinatenstelsel", mapLayerCoordinateSystemColumn.HeaderText);
                Assert.IsTrue(mapLayerCoordinateSystemColumn.ReadOnly);
            }
        }
    }
}