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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Test
{
    [TestFixture]
    public class StabilityStoneCoverHydraulicBoundaryLocationSelectionDialogTest
    {
        private const int locationSelectionColumnIndex = 0;
        private const int locationColumnIndex = 1;

        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(null, Enumerable.Empty<IHydraulicBoundaryLocation>());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithoutHydraulicBoundaryLocations_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                TestDelegate test = () => new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, null);

                // Assert
                string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("hydraulicBoundaryLocations", parameter);
            }
        }

        [Test]
        public void Constructor_WithParentAndLocations_DefaultProperties()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, Enumerable.Empty<IHydraulicBoundaryLocation>()))
                {
                    // Assert
                    Assert.IsInstanceOf<DialogBase>(dialog);
                    Assert.IsEmpty(dialog.SelectedLocations);
                    Assert.AreEqual("Selecteer hydraulische randvoorwaardenlocaties", dialog.Text);

                    var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                    var dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();
                    Assert.AreEqual(2, dataGridView.ColumnCount);

                    var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationSelectionColumnIndex];
                    const string expectedLocationCalculateHeaderText = "Gebruik";
                    Assert.AreEqual(expectedLocationCalculateHeaderText, locationCalculateColumn.HeaderText);
                    Assert.AreEqual("Selected", locationCalculateColumn.DataPropertyName);
                    Assert.IsFalse(locationCalculateColumn.ReadOnly);

                    var locationColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
                    const string expectedLocationHeaderText = "Hydraulische randvoorwaardenlocatie";
                    Assert.AreEqual(expectedLocationHeaderText, locationColumn.HeaderText);
                    Assert.AreEqual("Name", locationColumn.DataPropertyName);
                    Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, locationColumn.AutoSizeMode);
                    Assert.IsTrue(locationColumn.ReadOnly);

                    var buttonTester = new ButtonTester("GenerateForSelectedButton", dialog);
                    var button = (Button) buttonTester.TheObject;
                    Assert.IsFalse(button.Enabled);
                }
            }
        }

        [Test]
        public void OnLoad_Always_SetMinimumSize()
        {
            // Setup
            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, Enumerable.Empty<IHydraulicBoundaryLocation>()))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(370, dialog.MinimumSize.Width);
                Assert.AreEqual(550, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void GivenDialogWithSelectedLocations_WhenCloseWithoutConfirmation_ThenReturnsEmptyCollection()
        {
            // Given
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "1", 1, 1),
                new HydraulicBoundaryLocation(2, "2", 2, 2)
            };

            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, locations))
            {
                var selectionView = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                dialog.Close();

                // Then
                Assert.IsEmpty(dialog.SelectedLocations);
            }
        }

        [Test]
        public void GivenDialogWithSelectedLocations_WhenCancelButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedLocation = new HydraulicBoundaryLocation(1, "1", 1, 1);
            var locations = new[]
            {
                selectedLocation,
                new HydraulicBoundaryLocation(2, "2", 2, 2)
            };

            using (var viewParent = new Form())
            {
                using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, locations))
                {
                    var selectionView = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;

                    dialog.Show();
                    selectionView.Rows[0].Cells[0].Value = true;

                    // When
                    var cancelButton = new ButtonTester("CustomCancelButton", dialog);
                    cancelButton.Click();

                    // Then
                    Assert.IsEmpty(dialog.SelectedLocations);
                }
            }
        }

        [Test]
        public void GivenDialogWithSelectedLocations_WhenGenerateButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedLocation = new HydraulicBoundaryLocation(1, "1", 1, 1);
            var locations = new[]
            {
                selectedLocation,
                new HydraulicBoundaryLocation(2, "2", 2, 2)
            };

            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, locations))
            {
                var selectionView = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var generateButton = new ButtonTester("GenerateForSelectedButton", dialog);
                generateButton.Click();

                // Then
                var result = dialog.SelectedLocations;

                CollectionAssert.AreEqual(new[]
                {
                    selectedLocation
                }, result);
            }
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllLocationsSelected()
        {
            // Setup
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "1", 1, 1),
                new HydraulicBoundaryLocation(2, "2", 2, 2)
            };

            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, locations))
            {
                dialog.Show();

                var dataGridView = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                var rows = dataGridView.Rows;
                var button = new ButtonTester("SelectAllButton", dialog);

                // Precondition
                Assert.IsFalse((bool) rows[0].Cells[locationSelectionColumnIndex].Value);
                Assert.IsFalse((bool) rows[1].Cells[locationSelectionColumnIndex].Value);

                // Call
                button.Click();

                // Assert
                Assert.IsTrue((bool) rows[0].Cells[locationSelectionColumnIndex].Value);
                Assert.IsTrue((bool) rows[1].Cells[locationSelectionColumnIndex].Value);
            }
        }

        [Test]
        public void DeselectAllButton_AllLocationsSelectedDeselectAllButtonClicked_AllLocationsNotSelected()
        {
            // Setup
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "1", 1, 1),
                new HydraulicBoundaryLocation(2, "2", 2, 2)
            };

            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, locations))
            {
                dialog.Show();

                var dataGridView = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                var rows = dataGridView.Rows;
                var button = new ButtonTester("DeselectAllButton", dialog);

                foreach (DataGridViewRow row in rows)
                {
                    row.Cells[locationSelectionColumnIndex].Value = true;
                }

                // Precondition
                Assert.IsTrue((bool) rows[0].Cells[locationSelectionColumnIndex].Value);
                Assert.IsTrue((bool) rows[1].Cells[locationSelectionColumnIndex].Value);

                // Call
                button.Click();

                // Assert
                Assert.IsFalse((bool) rows[0].Cells[locationSelectionColumnIndex].Value);
                Assert.IsFalse((bool) rows[1].Cells[locationSelectionColumnIndex].Value);
            }
        }

        [Test]
        public void GenerateForSelectedButton_NoneSelected_GenerateForSelectedButtonDisabled()
        {
            // Setup
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "1", 1, 1),
                new HydraulicBoundaryLocation(2, "2", 2, 2)
            };

            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, locations))
            {
                dialog.Show();
                var buttonTester = new ButtonTester("GenerateForSelectedButton", dialog);

                // Call
                var button = (Button) buttonTester.TheObject;

                // Assert
                Assert.IsFalse(button.Enabled);
                Assert.IsEmpty(dialog.SelectedLocations);
            }
        }

        [Test]
        public void GenerateForSelectedButton_OneSelected_ReturnsSelectedLocations()
        {
            // Setup
            var selectedLocation = new HydraulicBoundaryLocation(1, "1", 1, 1);
            var locations = new[]
            {
                selectedLocation,
                new HydraulicBoundaryLocation(2, "2", 2, 2)
            };

            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, locations))
            {
                dialog.Show();
                var dataGridView = (DataGridViewControl) new ControlTester("dataGridViewControl", dialog).TheObject;
                var rows = dataGridView.Rows;
                rows[0].Cells[locationSelectionColumnIndex].Value = true;
                var buttonTester = new ButtonTester("GenerateForSelectedButton", dialog);

                // Call
                buttonTester.Click();

                // Assert
                Assert.AreEqual(1, dialog.SelectedLocations.Count());
                Assert.AreEqual(selectedLocation, dialog.SelectedLocations.First());
            }
        }
    }
}