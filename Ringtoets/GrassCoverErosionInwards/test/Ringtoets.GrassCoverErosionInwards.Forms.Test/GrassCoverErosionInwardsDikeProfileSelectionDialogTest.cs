﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsDikeProfileSelectionDialogTest
    {
        private const int locationSelectionColumnIndex = 0;
        private const int locationColumnIndex = 1;

        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsDikeProfileSelectionDialog(null, Enumerable.Empty<DikeProfile>());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithoutDikeProfiles_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                TestDelegate test = () => new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, null);

                // Assert
                string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("dikeProfiles", parameter);
            }
        }

        [Test]
        public void Constructor_WithParentAndDikeProfiles_DefaultProperties()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, Enumerable.Empty<DikeProfile>()))
                {
                    // Assert
                    Assert.IsInstanceOf<SelectionDialogBase<DikeProfile>>(dialog);
                    Assert.IsEmpty(dialog.SelectedItems);
                    Assert.AreEqual("Selecteer dijkprofielen", dialog.Text);
                }
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (var viewParent = new Form())
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, Enumerable.Empty<DikeProfile>()))
            {
                dialog.Show();

                // Assert
                Assert.IsEmpty(dialog.SelectedItems);

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                var dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();
                Assert.AreEqual(2, dataGridView.ColumnCount);

                var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationSelectionColumnIndex];
                Assert.AreEqual("Gebruik", locationCalculateColumn.HeaderText);
                Assert.AreEqual("Selected", locationCalculateColumn.DataPropertyName);
                Assert.IsFalse(locationCalculateColumn.ReadOnly);

                var nameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
                Assert.AreEqual("Dijkprofiel", nameColumn.HeaderText);
                Assert.AreEqual("Name", nameColumn.DataPropertyName);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, nameColumn.AutoSizeMode);
                Assert.IsTrue(nameColumn.ReadOnly);
            }
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationOneEntry_OneRowInGrid()
        {
            // Setup
            const string testname = "testName";

            // Call
            using (var viewParent = new Form())
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, new DikeProfile[]
            {
                new TestDikeProfile(testname)
            }))
            {
                // Assert
                dialog.Show();

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl").TheObject;
                Assert.AreEqual(1, dataGridViewControl.Rows.Count);
                Assert.IsFalse((bool) dataGridViewControl.Rows[0].Cells[locationSelectionColumnIndex].Value);
                Assert.AreEqual(testname, (string) dataGridViewControl.Rows[0].Cells[locationColumnIndex].Value);
            }
        }
    }
}