using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsDikeProfileSelectionViewTest
    {
        private Form testForm;
        private const int dikeProfileNameColumnIndex = 1;
        private const int selectedColumnIndex = 0;

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

            var dikeProfileDataGrid = (DataGridView)new ControlTester("DikeProfileDataGrid").TheObject;

            Assert.AreEqual(2, dikeProfileDataGrid.ColumnCount);
            Assert.IsFalse(dikeProfileDataGrid.RowHeadersVisible);

            var selectedColumn = dikeProfileDataGrid.Columns[0] as DataGridViewCheckBoxColumn;
            var dikeProfileNameColumn = dikeProfileDataGrid.Columns[1] as DataGridViewTextBoxColumn;

            Assert.NotNull(selectedColumn);
            Assert.AreEqual("Selected", selectedColumn.DataPropertyName);
            Assert.AreEqual("Gebruiken", selectedColumn.HeaderText);
            Assert.AreEqual(60, selectedColumn.Width);
            Assert.IsFalse(selectedColumn.ReadOnly);

            Assert.NotNull(dikeProfileNameColumn);
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
            var ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            ringtoetsGrassCoverErosionInwardsDikeProfile.Name = testname;

            // Call
            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile
            });

            // Assert
            ShowPipingCalculationsView(view);
            var dikeProfileDataGrid = (DataGridView)new ControlTester("DikeProfileDataGrid").TheObject;

            Assert.AreEqual(1, dikeProfileDataGrid.RowCount);
            Assert.IsFalse((bool)dikeProfileDataGrid.Rows[0].Cells[selectedColumnIndex].Value);
            Assert.AreEqual(testname, (string)dikeProfileDataGrid.Rows[0].Cells[dikeProfileNameColumnIndex].Value);
        }

        [Test]
        public void OnSelectAllClicked_WithDikeProfiles_AllDikeProfilesSelected()
        {
            // Setup
            var ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();

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
            var dikeProfileDataGrid = (DataGridView)new ControlTester("DikeProfileDataGrid").TheObject;
            for (int i = 0; i < dikeProfileDataGrid.RowCount; i++)
            {
                var row = dikeProfileDataGrid.Rows[i];
                Assert.IsTrue((bool)row.Cells[selectedColumnIndex].Value);
            }
        }

        [Test]
        public void OnSelectNoneClicked_WithDikeProfiles_AllDikeProfilesDeselected()
        {
            // Setup
            var ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();

            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2
            });

            ShowPipingCalculationsView(view);
            var selectNoneButtonTester = new ButtonTester("SelectNoneButton");

            var dikeProfileDataGrid = (DataGridView)new ControlTester("DikeProfileDataGrid").TheObject;
            for (int i = 0; i < dikeProfileDataGrid.RowCount; i++)
            {
                var row = dikeProfileDataGrid.Rows[i];
                row.Cells[selectedColumnIndex].Value = true;
            }

            // Call
            selectNoneButtonTester.Click();

            // Assert
            for (int i = 0; i < dikeProfileDataGrid.RowCount; i++)
            {
                var row = dikeProfileDataGrid.Rows[i];
                Assert.IsFalse((bool)row.Cells[selectedColumnIndex].Value);
            }
        }

        [Test]
        public void GetSelectedDikeProfiles_WithDikeProfilesMultipleSelected_ReturnSelectedDikeProfiles()
        {
            // Setup
            var ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile3 = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile4 = CreateTestDikeProfile();

            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2,
                ringtoetsGrassCoverErosionInwardsDikeProfile3,
                ringtoetsGrassCoverErosionInwardsDikeProfile4
            });

            ShowPipingCalculationsView(view);

            var dikeProfileDataGrid = (DataGridView)new ControlTester("DikeProfileDataGrid").TheObject;
            dikeProfileDataGrid.Rows[1].Cells[selectedColumnIndex].Value = true;
            dikeProfileDataGrid.Rows[3].Cells[selectedColumnIndex].Value = true;

            // Call
            IEnumerable<DikeProfile> dikeProfiles = view.GetSelectedDikeProfiles();

            // Assert
            CollectionAssert.AreEqual(new[] { ringtoetsGrassCoverErosionInwardsDikeProfile2, ringtoetsGrassCoverErosionInwardsDikeProfile4 }, dikeProfiles);
        }

        [Test]
        public void GetSelectedDikeProfiles_WithDikeProfilesNoneSelected_ReturnEmptyDikeProfilesCollection()
        {
            // Setup
            var ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile3 = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile4 = CreateTestDikeProfile();

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
            var ringtoetsGrassCoverErosionInwardsDikeProfile = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile2 = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile3 = CreateTestDikeProfile();
            var ringtoetsGrassCoverErosionInwardsDikeProfile4 = CreateTestDikeProfile();

            var dikeProfileCollection = new[]
            {
                ringtoetsGrassCoverErosionInwardsDikeProfile,
                ringtoetsGrassCoverErosionInwardsDikeProfile2,
                ringtoetsGrassCoverErosionInwardsDikeProfile3,
                ringtoetsGrassCoverErosionInwardsDikeProfile4
            };
            var view = new GrassCoverErosionInwardsDikeProfileSelectionView(dikeProfileCollection);

            ShowPipingCalculationsView(view);

            var dikeProfileDataGrid = (DataGridView)new ControlTester("DikeProfileDataGrid").TheObject;
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

        private DikeProfile CreateTestDikeProfile()
        {
            return new DikeProfile(new Point2D(0,0));
        }

        private void ShowPipingCalculationsView(GrassCoverErosionInwardsDikeProfileSelectionView pipingCalculationsView)
        {
            testForm.Controls.Add(pipingCalculationsView);
            testForm.Show();
        } 
    }
}