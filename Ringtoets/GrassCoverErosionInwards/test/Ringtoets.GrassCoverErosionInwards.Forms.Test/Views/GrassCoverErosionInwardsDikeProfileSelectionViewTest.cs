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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsDikeProfileSelectionViewTest
    {
        private const int dikeProfileNameColumnIndex = 1;
        private const int selectedColumnIndex = 0;
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_DikeProfilesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsDikeProfileSelectionView(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dikeProfiles", parameter);
        }

        [Test]
        public void Constructor_DikeProfilesEmpty_DefaultProperties()
        {
            // Call
            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new List<DikeProfile>());

            // Assert
            ShowPipingCalculationsView(view);

            var dikeProfileDataGrid = (DataGridView) new ControlTester("DikeProfileDataGrid").TheObject;

            Assert.AreEqual(2, dikeProfileDataGrid.ColumnCount);
            Assert.IsFalse(dikeProfileDataGrid.RowHeadersVisible);

            var selectedColumn = (DataGridViewCheckBoxColumn) dikeProfileDataGrid.Columns[0];
            var dikeProfileNameColumn = (DataGridViewTextBoxColumn) dikeProfileDataGrid.Columns[1];

            Assert.AreEqual("Selected", selectedColumn.DataPropertyName);
            Assert.AreEqual("Gebruiken", selectedColumn.HeaderText);
            Assert.AreEqual(60, selectedColumn.Width);
            Assert.IsFalse(selectedColumn.ReadOnly);

            Assert.AreEqual("Name", dikeProfileNameColumn.DataPropertyName);
            Assert.AreEqual("Dijkprofiel", dikeProfileNameColumn.HeaderText);
            Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, dikeProfileNameColumn.AutoSizeMode);
            Assert.IsTrue(dikeProfileNameColumn.ReadOnly);

            Assert.AreEqual(0, dikeProfileDataGrid.RowCount);
        }

        [Test]
        public void Constructor_DikeProfilesOneEntry_OneRowInGrid()
        {
            // Setup
            var testname = "testName";
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile(testname);

            // Call
            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile
            });

            // Assert
            ShowPipingCalculationsView(view);
            var dikeProfileDataGrid = (DataGridView) new ControlTester("DikeProfileDataGrid").TheObject;

            Assert.AreEqual(1, dikeProfileDataGrid.RowCount);
            Assert.IsFalse((bool) dikeProfileDataGrid.Rows[0].Cells[selectedColumnIndex].Value);
            Assert.AreEqual(testname, (string) dikeProfileDataGrid.Rows[0].Cells[dikeProfileNameColumnIndex].Value);
        }

        [Test]
        public void OnSelectAllClicked_WithDikeProfiles_AllDikeProfilesSelected()
        {
            // Setup
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();

            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2
            });

            ShowPipingCalculationsView(view);
            var selectAllButtonTester = new ButtonTester("SelectAllButton");

            // Call
            selectAllButtonTester.Click();

            // Assert
            var dikeProfileDataGrid = (DataGridView) new ControlTester("DikeProfileDataGrid").TheObject;
            for (int i = 0; i < dikeProfileDataGrid.RowCount; i++)
            {
                DataGridViewRow row = dikeProfileDataGrid.Rows[i];
                Assert.IsTrue((bool) row.Cells[selectedColumnIndex].Value);
            }
        }

        [Test]
        public void OnSelectNoneClicked_WithDikeProfiles_AllDikeProfilesDeselected()
        {
            // Setup
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();

            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2
            });

            ShowPipingCalculationsView(view);
            var selectNoneButtonTester = new ButtonTester("SelectNoneButton");

            var dikeProfileDataGrid = (DataGridView) new ControlTester("DikeProfileDataGrid").TheObject;
            for (int i = 0; i < dikeProfileDataGrid.RowCount; i++)
            {
                DataGridViewRow row = dikeProfileDataGrid.Rows[i];
                row.Cells[selectedColumnIndex].Value = true;
            }

            // Call
            selectNoneButtonTester.Click();

            // Assert
            for (int i = 0; i < dikeProfileDataGrid.RowCount; i++)
            {
                DataGridViewRow row = dikeProfileDataGrid.Rows[i];
                Assert.IsFalse((bool) row.Cells[selectedColumnIndex].Value);
            }
        }

        [Test]
        public void GetSelectedDikeProfiles_WithDikeProfilesMultipleSelected_ReturnSelectedDikeProfiles()
        {
            // Setup
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile3 = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile4 = CreateTestDikeProfile();

            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2,
                ringtoetsGrassCoverErosionInwardsDikeProfile3,
                ringtoetsGrassCoverErosionInwardsDikeProfile4
            });

            ShowPipingCalculationsView(view);

            var dikeProfileDataGrid = (DataGridView) new ControlTester("DikeProfileDataGrid").TheObject;
            dikeProfileDataGrid.Rows[1].Cells[selectedColumnIndex].Value = true;
            dikeProfileDataGrid.Rows[3].Cells[selectedColumnIndex].Value = true;

            // Call
            IEnumerable<DikeProfile> dikeProfiles = view.GetSelectedDikeProfiles();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile2,
                ringtoetsGrassCoverErosionInwardsDikeProfile4
            }, dikeProfiles);
        }

        [Test]
        public void GetSelectedDikeProfiles_WithDikeProfilesNoneSelected_ReturnEmptyDikeProfilesCollection()
        {
            // Setup
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile3 = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile4 = CreateTestDikeProfile();

            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2,
                ringtoetsGrassCoverErosionInwardsDikeProfile3,
                ringtoetsGrassCoverErosionInwardsDikeProfile4
            });

            ShowPipingCalculationsView(view);

            // Call
            IEnumerable<DikeProfile> dikeProfiles = view.GetSelectedDikeProfiles();

            // Assert
            Assert.IsEmpty(dikeProfiles);
        }

        [Test]
        public void GetSelectedDikeProfiles_WithDikeProfilesAllSelected_ReturnAllDikeProfiles()
        {
            // Setup
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile3 = CreateTestDikeProfile();
            DikeProfile ringtoetsGrassCoverErosionInwardsDikeProfile4 = CreateTestDikeProfile();

            var dikeProfileCollection = new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2,
                ringtoetsGrassCoverErosionInwardsDikeProfile3,
                ringtoetsGrassCoverErosionInwardsDikeProfile4
            };
            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(dikeProfileCollection);

            ShowPipingCalculationsView(view);

            var dikeProfileDataGrid = (DataGridView) new ControlTester("DikeProfileDataGrid").TheObject;
            dikeProfileDataGrid.Rows[0].Cells[selectedColumnIndex].Value = true;
            dikeProfileDataGrid.Rows[1].Cells[selectedColumnIndex].Value = true;
            dikeProfileDataGrid.Rows[2].Cells[selectedColumnIndex].Value = true;
            dikeProfileDataGrid.Rows[3].Cells[selectedColumnIndex].Value = true;

            // Call
            IEnumerable<DikeProfile> dikeProfiles = view.GetSelectedDikeProfiles();

            // Assert
            CollectionAssert.AreEqual(dikeProfileCollection, dikeProfiles);
        }

        [Test]
        public void GetSelectedDikeProfiles_WithEmptyDikeProfiles_ReturnEmptyDikeProfilesCollection()
        {
            // Setup
            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(Enumerable.Empty<DikeProfile>());

            ShowPipingCalculationsView(view);

            // Call
            IEnumerable<DikeProfile> dikeProfiles = view.GetSelectedDikeProfiles();

            // Assert
            Assert.IsEmpty(dikeProfiles);
        }

        private DikeProfile CreateTestDikeProfile(string name = null)
        {
            return new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                   null, new DikeProfile.ConstructionProperties
                                   {
                                       Name = name
                                   });
        }

        private void ShowPipingCalculationsView(GrassCoverErosionInwardsDikeProfileSelectionView pipingCalculationsView)
        {
            testForm.Controls.Add(pipingCalculationsView);
            testForm.Show();
        }
    }
}