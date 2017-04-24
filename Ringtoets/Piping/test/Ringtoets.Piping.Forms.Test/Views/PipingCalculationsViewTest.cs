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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingCalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int stochasticSoilModelsColumnIndex = 1;
        private const int stochasticSoilProfilesColumnIndex = 2;
        private const int stochasticSoilProfilesProbabilityColumnIndex = 3;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 4;
        private const int dampingFactorExitMeanColumnIndex = 5;
        private const int phreaticLevelExitMeanColumnIndex = 6;
        private const int entryPointLColumnIndex = 7;
        private const int exitPointLColumnIndex = 8;
        private Form testForm;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            testForm.Dispose();
        }

        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var pipingCalculationsView = new PipingCalculationsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(pipingCalculationsView);
                Assert.IsInstanceOf<IView>(pipingCalculationsView);
                Assert.IsNull(pipingCalculationsView.Data);
                Assert.IsNull(pipingCalculationsView.PipingFailureMechanism);
                Assert.IsNull(pipingCalculationsView.AssessmentSection);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ShowPipingCalculationsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                Assert.IsFalse(dataGridView.AutoGenerateColumns);
                Assert.AreEqual(9, dataGridView.ColumnCount);

                foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
                {
                    Assert.AreEqual("This", column.ValueMember);
                    Assert.AreEqual("DisplayName", column.DisplayMember);
                }

                var soilProfilesCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[stochasticSoilProfilesColumnIndex];
                DataGridViewComboBoxCell.ObjectCollection soilProfilesComboboxItems = soilProfilesCombobox.Items;
                Assert.AreEqual(0, soilProfilesComboboxItems.Count); // Row dependent

                var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
                DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
                Assert.AreEqual(0, hydraulicBoundaryLocationComboboxItems.Count); // Row dependent
            }
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Call
            using (ShowPipingCalculationsView())
            {
                var listBox = (ListBox) new ControlTester("listBox").TheObject;

                // Assert
                Assert.AreEqual(0, listBox.Items.Count);
            }
        }

        [Test]
        public void Data_SetToNull_DoesNotThrow()
        {
            // Setup
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                // Call
                var testDelegate = new TestDelegate(() => pipingCalculationsView.Data = null);

                // Assert
                Assert.DoesNotThrow(testDelegate);
            }
        }

        [Test]
        public void AssessmentSection_WithHydraulicBoundaryDatabaseSurfaceLinesNull_SelectableHydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationsView = ShowSimplePipingCalculationsViewWithoutSurfaceLines(
                assessmentSection, new HydraulicBoundaryDatabase()))
            {
                // Call
                pipingCalculationsView.AssessmentSection = assessmentSection;

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
                DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
                Assert.AreEqual(3, hydraulicBoundaryLocationComboboxItems.Count);
                Assert.AreEqual("<geen>", hydraulicBoundaryLocationComboboxItems[0].ToString());
                Assert.AreEqual("Location 1", hydraulicBoundaryLocationComboboxItems[1].ToString());
                Assert.AreEqual("Location 2", hydraulicBoundaryLocationComboboxItems[2].ToString());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentSection_WithSurfaceLinesHydraulicBoundaryDatabaseNull_SelectableHydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationsView = ShowSimplePipingCalculationsViewWithSurfaceLines(
                assessmentSection))
            {
                // Call
                pipingCalculationsView.AssessmentSection = assessmentSection;

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
                DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
                Assert.AreEqual(1, hydraulicBoundaryLocationComboboxItems.Count);
                Assert.AreEqual("<geen>", hydraulicBoundaryLocationComboboxItems[0].ToString());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentSection_HydraulicBoundaryDatabaseWithLocationsAndSurfaceLines_SelectableHydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationsView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection))
            {
                // Call
                pipingCalculationsView.AssessmentSection = assessmentSection;

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
                DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
                Assert.AreEqual(7, hydraulicBoundaryLocationComboboxItems.Count);
                Assert.AreEqual("<geen>", hydraulicBoundaryLocationComboboxItems[0].ToString());
                Assert.AreEqual("Location 1", hydraulicBoundaryLocationComboboxItems[1].ToString());
                Assert.AreEqual("Location 2", hydraulicBoundaryLocationComboboxItems[2].ToString());
                Assert.AreEqual("Location 1 (2 m)", hydraulicBoundaryLocationComboboxItems[3].ToString());
                Assert.AreEqual("Location 2 (6 m)", hydraulicBoundaryLocationComboboxItems[4].ToString());
                Assert.AreEqual("Location 1 (4 m)", hydraulicBoundaryLocationComboboxItems[5].ToString());
                Assert.AreEqual("Location 2 (5 m)", hydraulicBoundaryLocationComboboxItems[6].ToString());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void PipingFailureMechanism_PipingFailureMechanismWithSections_SectionsListBoxCorrectlyInitialized()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new List<Point2D>
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            pipingFailureMechanism.AddSection(failureMechanismSection1);
            pipingFailureMechanism.AddSection(failureMechanismSection2);
            pipingFailureMechanism.AddSection(failureMechanismSection3);

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                // Call
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                // Assert
                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                Assert.AreEqual(3, listBox.Items.Count);
                Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
                Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
                Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            }
        }

        [Test]
        public void PipingCalculationsView_CalculationsWithCorrespondingStochasticSoilModel_StochasticSoilModelsComboboxCorrectlyInitialized()
        {
            // Setup & Call
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredPipingCalculationsView(assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewComboBoxCell.ObjectCollection stochasticSoilModelsComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex]).Items;
                Assert.AreEqual(1, stochasticSoilModelsComboboxItems.Count);
                Assert.AreEqual("Model A", stochasticSoilModelsComboboxItems[0].ToString());

                stochasticSoilModelsComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[1].Cells[stochasticSoilModelsColumnIndex]).Items;
                Assert.AreEqual(3, stochasticSoilModelsComboboxItems.Count);
                Assert.AreEqual("<geen>", stochasticSoilModelsComboboxItems[0].ToString());
                Assert.AreEqual("Model A", stochasticSoilModelsComboboxItems[1].ToString());
                Assert.AreEqual("Model E", stochasticSoilModelsComboboxItems[2].ToString());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void PipingCalculationsView_CalculationsWithCorrespondingSoilProfiles_SoilProfilesComboboxCorrectlyInitialized()
        {
            // Setup & Call
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredPipingCalculationsView(assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewComboBoxCell.ObjectCollection soilProfilesComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex]).Items;
                Assert.AreEqual(3, soilProfilesComboboxItems.Count);
                Assert.AreEqual("<geen>", soilProfilesComboboxItems[0].ToString());
                Assert.AreEqual("Profile 1", soilProfilesComboboxItems[1].ToString());
                Assert.AreEqual("Profile 2", soilProfilesComboboxItems[2].ToString());

                soilProfilesComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesColumnIndex]).Items;
                Assert.AreEqual(1, soilProfilesComboboxItems.Count);
                Assert.AreEqual("Profile 5", soilProfilesComboboxItems[0].ToString());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void PipingCalculationsView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredPipingCalculationsView(assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(9, cells.Count);
                Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Model A", cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("<geen>", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual("0", cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
                Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
                Assert.AreEqual(1.111.ToString(CultureInfo.CurrentCulture), cells[dampingFactorExitMeanColumnIndex].FormattedValue);
                Assert.AreEqual(2.222.ToString(CultureInfo.CurrentCulture), cells[phreaticLevelExitMeanColumnIndex].FormattedValue);
                Assert.AreEqual(3.33.ToString(CultureInfo.CurrentCulture), cells[entryPointLColumnIndex].FormattedValue);
                Assert.AreEqual(4.44.ToString(CultureInfo.CurrentCulture), cells[exitPointLColumnIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(9, cells.Count);
                Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Model E", cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("Profile 5", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual("30", cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
                Assert.AreEqual("Location 2 (5 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
                Assert.AreEqual(5.556.ToString(CultureInfo.CurrentCulture), cells[dampingFactorExitMeanColumnIndex].FormattedValue);
                Assert.AreEqual(6.667.ToString(CultureInfo.CurrentCulture), cells[phreaticLevelExitMeanColumnIndex].FormattedValue);
                Assert.AreEqual(7.78.ToString(CultureInfo.CurrentCulture), cells[entryPointLColumnIndex].FormattedValue);
                Assert.AreEqual(8.89.ToString(CultureInfo.CurrentCulture), cells[exitPointLColumnIndex].FormattedValue);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void PipingCalculationsView_SelectingCellInRow_SelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            PipingCalculationsView pipingCalculationsView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection);

            var selectionChangedCount = 0;
            pipingCalculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

            // Call                
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(1, 0));

            // Assert
            Assert.AreEqual(1, selectionChangedCount);

            mocks.VerifyAll();
        }

        [Test]
        public void PipingCalculationsView_ChangingListBoxSelection_DataGridViewCorrectlySyncedAndSelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationsView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection))
            {
                var selectionChangedCount = 0;
                pipingCalculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(2, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);

                // Call
                listBox.SelectedIndex = 1;

                // Assert
                Assert.AreEqual(1, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(1, selectionChangedCount);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("test", dampingFactorExitMeanColumnIndex)]
        [TestCase("test", phreaticLevelExitMeanColumnIndex)]
        [TestCase("test", entryPointLColumnIndex)]
        [TestCase("test", exitPointLColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", dampingFactorExitMeanColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", phreaticLevelExitMeanColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", entryPointLColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", exitPointLColumnIndex)]
        public void PipingCalculationsView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredPipingCalculationsView(assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

                // Assert
                Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1, dampingFactorExitMeanColumnIndex)]
        [TestCase(1e-2, dampingFactorExitMeanColumnIndex)]
        [TestCase(1e+6, dampingFactorExitMeanColumnIndex)]
        [TestCase(14.3, dampingFactorExitMeanColumnIndex)]
        [TestCase(1, phreaticLevelExitMeanColumnIndex)]
        [TestCase(1e-6, phreaticLevelExitMeanColumnIndex)]
        [TestCase(1e+6, phreaticLevelExitMeanColumnIndex)]
        [TestCase(14.3, phreaticLevelExitMeanColumnIndex)]
        [TestCase(2.2, entryPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(2.2)")]
        [TestCase(0.022e+2, entryPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(0.022e+2)")]
        [TestCase(220e-2, entryPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(220e-2)")]
        [TestCase(5.5, exitPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(5.5)")]
        [TestCase(0.055e+2, exitPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(0.055e+2)")]
        [TestCase(550e-2, exitPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(550e-2)")]
        public void FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            PipingFailureMechanism failureMechanism = ConfigureFailuremechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            var newRoundedvalue = (RoundedDouble) newValue;

            using (ShowFullyConfiguredPipingCalculationsView(assessmentSection, failureMechanism, calculationGroup))
            {
                mocks.ReplayAll();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = newRoundedvalue;

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateScenarios_WithoutSurfaceLines_ButtonDisabled()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, "path");

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;
                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];

                // Call
                bool state = button.Enabled;

                // Assert
                Assert.IsFalse(state);
            }
        }

        [Test]
        public void ButtonGenerateScenarios_WithoutSoilModels_ButtonDisabled()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine()
            }, "path");

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;
                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];

                // Call
                bool state = button.Enabled;

                // Assert
                Assert.IsFalse(state);
            }
        }

        [Test]
        public void ButtonGenerateScenarios_WithSurfaceLinesAndSoilModels_ButtonEnabled()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            const string arbitrarySourcePath = "path";
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine()
            }, arbitrarySourcePath);
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, arbitrarySourcePath);

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];

                // Call
                bool state = button.Enabled;

                // Assert
                Assert.IsTrue(state);
            }
        }

        [Test]
        public void GivenPipingCalculationsViewWithPipingFailureMechanism_WhenSectionsAddedAndPipingFailureMechanismNotified_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var pipingFailureMechanismWithSections = new PipingFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new List<Point2D>
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanismWithSections;

                var listBox = (ListBox) new ControlTester("listBox").TheObject;

                // Precondition
                Assert.AreEqual(0, listBox.Items.Count);

                pipingFailureMechanismWithSections.AddSection(failureMechanismSection1);
                pipingFailureMechanismWithSections.AddSection(failureMechanismSection2);
                pipingFailureMechanismWithSections.AddSection(failureMechanismSection3);

                // When
                pipingFailureMechanismWithSections.NotifyObservers();

                // Then
                Assert.AreEqual(3, listBox.Items.Count);
                Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
                Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
                Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            }
        }

        [Test]
        public void GivenPipingCalculationsView_WhenGenerateScenariosButtonClicked_ThenShowViewWithSurfaceLines()
        {
            // Given
            var pipingFailureMechanism = new PipingFailureMechanism();
            const string arbitraryFilePath = "path";
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line A"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line B"
                }
            }, arbitraryFilePath);
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, arbitraryFilePath);

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;
                pipingCalculationsView.Data = pipingFailureMechanism.CalculationsGroup;
                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                PipingSurfaceLineSelectionDialog selectionDialog = null;
                DataGridViewControl grid = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                // When
                button.Click();

                // Then
                Assert.NotNull(selectionDialog);
                Assert.NotNull(grid);
                Assert.AreEqual(2, grid.Rows.Count);
            }
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenNoCalculationGroupDefined_ThenCalculationGroupNotUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            const string arbitraryFilePath = "path";
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line A"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line B"
                }
            }, arbitraryFilePath);
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, arbitraryFilePath);
            pipingFailureMechanism.CalculationsGroup.Attach(observer);

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                // When
                button.Click();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase("DoForSelectedButton")]
        [TestCase("CustomCancelButton")]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenDialogClosed_ThenNotifyCalculationGroup(string buttonName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            const string arbitryFilePath = "path";
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line A"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line B"
                }
            }, arbitryFilePath);
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel(),
            }, arbitryFilePath);
            pipingFailureMechanism.CalculationsGroup.Attach(observer);

            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;
                pipingCalculationsView.Data = pipingFailureMechanism.CalculationsGroup;
                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                    selectionView.Rows[0].Cells[0].Value = true;

                    // When
                    new ButtonTester(buttonName, selectionDialog).Click();
                };

                button.Click();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                PipingFailureMechanism pipingFailureMechanism = ConfigureSimpleFailureMechanism();

                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;
                pipingCalculationsView.Data = pipingFailureMechanism.CalculationsGroup;

                // Precondition
                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                    selectionView.Rows[0].Cells[0].Value = true;

                    // When
                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                button.Click();

                // Then
                PipingCalculationScenario[] pipingCalculationScenarios = pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>().ToArray();
                PipingFailureMechanismSectionResult failureMechanismSectionResult1 = pipingCalculationsView.PipingFailureMechanism.SectionResults.First();
                PipingFailureMechanismSectionResult failureMechanismSectionResult2 = pipingCalculationsView.PipingFailureMechanism.SectionResults.ElementAt(1);

                Assert.AreEqual(2, failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios).Count());

                foreach (PipingCalculationScenario calculationScenario in failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios))
                {
                    Assert.IsInstanceOf<ICalculationScenario>(calculationScenario);
                }

                CollectionAssert.IsEmpty(failureMechanismSectionResult2.GetCalculationScenarios(pipingCalculationScenarios));
            }
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosCancelButtonClicked_WhenDialogClosed_CalculationsNotUpdated()
        {
            // Given
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                PipingFailureMechanism pipingFailureMechanism = ConfigureSimpleFailureMechanism();

                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;
                pipingCalculationsView.Data = pipingFailureMechanism.CalculationsGroup;

                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                    selectionView.Rows[0].Cells[0].Value = true;

                    // When
                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                button.Click();

                // Then
                Assert.IsEmpty(pipingCalculationsView.PipingFailureMechanism.Calculations);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSoilModelAndNotify_ThenButtonDisabled()
        {
            // Given
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                // When
                pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, "path");
                pipingFailureMechanism.NotifyObservers();

                // Then
                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndNotify_ThenButtonDisabled()
        {
            // Given
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                // When
                pipingFailureMechanism.SurfaceLines.AddRange(new[]
                {
                    new RingtoetsPipingSurfaceLine()
                }, "path");
                pipingFailureMechanism.NotifyObservers();

                // Then
                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndSoilModelAndDoNotNotifyObservers_ThenButtonDisabled()
        {
            // Given
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                // When
                const string arbitraryFilePath = "path";
                pipingFailureMechanism.SurfaceLines.AddRange(new[]
                {
                    new RingtoetsPipingSurfaceLine()
                }, arbitraryFilePath);
                pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, arbitraryFilePath);

                // Then
                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndSoilModelAndNotifyObservers_ThenButtonEnabled()
        {
            // Given
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                // When
                const string arbitraryFilePath = "path";
                pipingFailureMechanism.SurfaceLines.AddRange(new[]
                {
                    new RingtoetsPipingSurfaceLine()
                }, arbitraryFilePath);
                pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, arbitraryFilePath);
                pipingCalculationsView.PipingFailureMechanism.NotifyObservers();

                // Then
                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsTrue(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithSurfaceLinesAndSoilModels_WhenSurfaceLinesAndSoilModelsClearedAndNotifyObservers_ThenButtonDisabled()
        {
            // Given
            using (PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                const string arbitraryFilePath = "path";
                pipingFailureMechanism.SurfaceLines.AddRange(new[]
                {
                    new RingtoetsPipingSurfaceLine()
                }, arbitraryFilePath);
                pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, arbitraryFilePath);
                pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

                // When
                pipingFailureMechanism.SurfaceLines.Clear();
                pipingFailureMechanism.StochasticSoilModels.Clear();
                pipingCalculationsView.PipingFailureMechanism.NotifyObservers();

                // Then
                var button = (Button) pipingCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        [TestCase(entryPointLColumnIndex, 6.6)]
        [TestCase(entryPointLColumnIndex, 4.44)]
        [TestCase(exitPointLColumnIndex, 2.22)]
        [TestCase(exitPointLColumnIndex, 1.1)]
        public void PipingCalculationsView_InvalidEntryOrExitPoint_ShowsErrorTooltip(int cellIndex, double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var pipingCalculationObserver = mocks.StrictMock<IObserver>();
            var pipingCalculationInputObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            PipingFailureMechanism failureMechanism = ConfigureFailuremechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            var value = (RoundedDouble) newValue;

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection, failureMechanism, calculationGroup))
            {
                var data = (CalculationGroup) pipingCalculationView.Data;
                var pipingCalculation = (PipingCalculationScenario) data.Children.First();

                pipingCalculation.Attach(pipingCalculationObserver);
                pipingCalculation.InputParameters.Attach(pipingCalculationInputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = value;

                // Assert
                Assert.AreEqual("Het uittredepunt moet landwaarts van het intredepunt liggen.", dataGridView.Rows[0].ErrorText);
            }
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(entryPointLColumnIndex, -0.1)]
        [TestCase(entryPointLColumnIndex, -1.0)]
        [TestCase(exitPointLColumnIndex, 10.1)]
        [TestCase(exitPointLColumnIndex, 11.0)]
        public void PipingCalculationsView_EntryOrExitPointNotOnSurfaceLine_ShowsErrorToolTip(int cellIndex, double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var pipingCalculationObserver = mocks.StrictMock<IObserver>();
            var pipingCalculationInputObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            PipingFailureMechanism failureMechanism = ConfigureFailuremechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection, failureMechanism, calculationGroup))
            {
                var data = (CalculationGroup) pipingCalculationView.Data;
                var pipingCalculation = (PipingCalculationScenario) data.Children.First();

                pipingCalculation.Attach(pipingCalculationObserver);
                pipingCalculation.InputParameters.Attach(pipingCalculationInputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = (RoundedDouble) newValue;

                // Assert
                var expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 10,0]).";
                Assert.AreEqual(expectedMessage, dataGridView.Rows[0].ErrorText);
            }
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Selection_Always_ReturnsTheSelectedRowObject(int selectedRow)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[0];

                // Call
                object selection = pipingCalculationView.Selection;

                // Assert
                Assert.IsInstanceOf<PipingInputContext>(selection);
                var dataRow = (PipingCalculationRow) dataGridView.Rows[selectedRow].DataBoundItem;
                Assert.AreSame(dataRow.PipingCalculation, ((PipingInputContext) selection).PipingCalculation);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void PipingCalculationsView_EditingNameViaDataGridView_ObserversCorrectlyNotified(bool useCalculationWithOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var pipingCalculationObserver = mocks.StrictMock<IObserver>();
            var pipingCalculationInputObserver = mocks.StrictMock<IObserver>();

            pipingCalculationObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) pipingCalculationView.Data;
                var pipingCalculation = (PipingCalculationScenario) data.Children.First();

                if (useCalculationWithOutput)
                {
                    pipingCalculation.Output = new TestPipingOutput();
                }

                pipingCalculation.Attach(pipingCalculationObserver);
                pipingCalculation.InputParameters.Attach(pipingCalculationInputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[nameColumnIndex].Value = "New name";

                // Assert
                pipingCalculation.Output = null;
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(stochasticSoilProfilesColumnIndex, null, true)]
        [TestCase(stochasticSoilProfilesColumnIndex, null, false)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, true)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, false)]
        [TestCase(dampingFactorExitMeanColumnIndex, 1.1, true)]
        [TestCase(dampingFactorExitMeanColumnIndex, 1.1, false)]
        [TestCase(phreaticLevelExitMeanColumnIndex, 1.1, true)]
        [TestCase(phreaticLevelExitMeanColumnIndex, 1.1, false)]
        [TestCase(entryPointLColumnIndex, 1.1, true)]
        [TestCase(entryPointLColumnIndex, 1.1, false)]
        [TestCase(exitPointLColumnIndex, 8.0, true)]
        [TestCase(exitPointLColumnIndex, 8.0, false)]
        public void PipingCalculationsView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(
            int cellIndex,
            object newValue,
            bool useCalculationWithOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var pipingCalculationObserver = mocks.StrictMock<IObserver>();
            var pipingCalculationInputObserver = mocks.StrictMock<IObserver>();

            if (useCalculationWithOutput)
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new MessageBoxTester(wnd);
                    tester.ClickOk();
                };

                pipingCalculationObserver.Expect(o => o.UpdateObserver());
            }

            pipingCalculationInputObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            PipingFailureMechanism failureMechanism = ConfigureFailuremechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection, failureMechanism, calculationGroup))
            {
                mocks.ReplayAll();

                var data = (CalculationGroup) pipingCalculationView.Data;
                var pipingCalculation = (PipingCalculationScenario) data.Children[1];

                if (useCalculationWithOutput)
                {
                    pipingCalculation.Output = new TestPipingOutput();
                }

                pipingCalculation.Attach(pipingCalculationObserver);
                pipingCalculation.InputParameters.Attach(pipingCalculationInputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[1].Cells[cellIndex].Value = newValue is double ? (RoundedDouble) (double) newValue : newValue;

                // Assert
                pipingCalculation.Output = null;
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationWithHydraulicLocation_WhenLocationManualDesignWaterLevel_SelectableHydraulicLocationReadonly()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) pipingCalculationView.Data;
                var pipingCalculation = (PipingCalculationScenario) data.Children.First();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                var currentCell = (DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex];
                Assert.IsFalse(currentCell.ReadOnly);

                // When
                pipingCalculation.InputParameters.UseAssessmentLevelManualInput = true;
                pipingCalculation.InputParameters.NotifyObservers();

                // Then
                var currentCellUpdated = (DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex];
                Assert.IsTrue(currentCellUpdated.ReadOnly);

                DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = currentCellUpdated.Items;
                Assert.AreEqual(1, hydraulicBoundaryLocationComboboxItems.Count);
                Assert.AreEqual("<geen>", hydraulicBoundaryLocationComboboxItems[0].ToString());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationWithStochasticSoilProfile_WhenProbabilityChangesAndNotified_ThenNewProbabilityVisible()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) pipingCalculationView.Data;
                var pipingCalculation = (PipingCalculationScenario) data.Children[1];

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var refreshed = 0;

                // Precondition
                var currentCell = (DataGridViewTextBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex];
                Assert.AreEqual("30", currentCell.FormattedValue);

                StochasticSoilProfile stochasticSoilProfileToChange = pipingCalculation.InputParameters.StochasticSoilProfile;
                var updatedProfile = new StochasticSoilProfile(
                    0.5,
                    stochasticSoilProfileToChange.SoilProfileType,
                    stochasticSoilProfileToChange.SoilProfileId)
                {
                    SoilProfile = stochasticSoilProfileToChange.SoilProfile
                };
                dataGridView.Invalidated += (sender, args) => refreshed++;

                // When
                stochasticSoilProfileToChange.Update(updatedProfile);
                stochasticSoilProfileToChange.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshed);
                var cell = (DataGridViewTextBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex];
                Assert.AreEqual("50", cell.FormattedValue);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationViewWithCalculations_WhenSurfaceLineLocatedOutsideSectionAfterUpdateAndObserversNotified_ThenDataGridViewUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (PipingCalculationsView pipingCalculationView = ShowFullyConfiguredPipingCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) pipingCalculationView.Data;
                var pipingCalculation = (PipingCalculationScenario) data.Children[0];

                DataGridViewControl dataGridView = pipingCalculationView.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
                ListBox listBox = pipingCalculationView.Controls.Find("listBox", true).OfType<ListBox>().First();

                // Precondition
                listBox.SelectedIndex = 0;
                Assert.AreEqual(2, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);

                listBox.SelectedIndex = 1;
                Assert.AreEqual(1, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                RingtoetsPipingSurfaceLine surfaceLineToChange = pipingCalculation.InputParameters.SurfaceLine;
                var updatedSurfaceLine = new RingtoetsPipingSurfaceLine
                {
                    Name = surfaceLineToChange.Name,
                    ReferenceLineIntersectionWorldPoint = new Point2D(9.0, 0.0)
                };
                updatedSurfaceLine.SetGeometry(new[]
                {
                    new Point3D(9.0, 5.0, 0.0),
                    new Point3D(9.0, 0.0, 1.0),
                    new Point3D(9.0, -5.0, 0.0)
                });

                // When
                surfaceLineToChange.CopyProperties(updatedSurfaceLine);
                surfaceLineToChange.NotifyObservers();

                // Then
                listBox.SelectedIndex = 0;
                Assert.AreEqual(1, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                listBox.SelectedIndex = 1;
                Assert.AreEqual(2, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);
            }
            mocks.VerifyAll();
        }

        private PipingCalculationsView ShowFullyConfiguredPipingCalculationsView(
            IAssessmentSection assessmentSection)
        {
            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            PipingFailureMechanism failureMechanism = ConfigureFailuremechanism();

            return ShowFullyConfiguredPipingCalculationsView(assessmentSection,
                                                             failureMechanism,
                                                             ConfigureCalculationGroup(assessmentSection, failureMechanism));
        }

        private PipingCalculationsView ShowFullyConfiguredPipingCalculationsView(IAssessmentSection assessmentSection,
                                                                                 PipingFailureMechanism failureMechanism,
                                                                                 CalculationGroup calculationGroup)
        {
            PipingCalculationsView view = ShowPipingCalculationsView();
            view.Data = calculationGroup;
            view.AssessmentSection = assessmentSection;
            view.PipingFailureMechanism = failureMechanism;

            return view;
        }

        private static void ConfigureHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4);

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation1);
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation2);
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
        }

        private PipingCalculationsView ShowSimplePipingCalculationsViewWithSurfaceLines(IAssessmentSection assessmentSection)
        {
            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 2",
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, "path");

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView();

            pipingCalculationsView.Data = new CalculationGroup("Group", true)
            {
                Children =
                {
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine1
                        }
                    },
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine2
                        }
                    }
                }
            };

            pipingCalculationsView.AssessmentSection = assessmentSection;
            pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

            return pipingCalculationsView;
        }

        private PipingCalculationsView ShowSimplePipingCalculationsViewWithoutSurfaceLines(IAssessmentSection assessmentSection,
                                                                                           HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4);

            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation1);
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation2);

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            PipingCalculationsView pipingCalculationsView = ShowPipingCalculationsView();

            pipingCalculationsView.Data = new CalculationGroup("Group", true)
            {
                Children =
                {
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation1
                        }
                    },
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                        }
                    }
                }
            };

            pipingCalculationsView.AssessmentSection = assessmentSection;
            pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

            return pipingCalculationsView;
        }

        private static PipingFailureMechanism ConfigureSimpleFailureMechanism()
        {
            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 2",
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            const string arbirtraryFilePath = "path";
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, arbirtraryFilePath);
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel
                {
                    Geometry =
                    {
                        new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                    }
                }
            }, arbirtraryFilePath);

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            return pipingFailureMechanism;
        }

        private static CalculationGroup ConfigureCalculationGroup(IAssessmentSection assessmentSection, PipingFailureMechanism failureMechanism)
        {
            StochasticSoilModel stochasticSoilModelForCalculation2 = failureMechanism.StochasticSoilModels.Last();
            return new CalculationGroup("Group", true)
            {
                Children =
                {
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.First(),
                            StochasticSoilModel = failureMechanism.StochasticSoilModels.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble) 1.1111
                            },
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble) 2.2222
                            },
                            EntryPointL = (RoundedDouble) 3.3333,
                            ExitPointL = (RoundedDouble) 4.4444
                        }
                    },
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.Last(),
                            StochasticSoilModel = stochasticSoilModelForCalculation2,
                            StochasticSoilProfile = stochasticSoilModelForCalculation2.StochasticSoilProfiles.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Last(),
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble) 5.5555
                            },
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble) 6.6666
                            },
                            EntryPointL = (RoundedDouble) 7.7777,
                            ExitPointL = (RoundedDouble) 8.8888
                        }
                    }
                }
            };
        }

        private static PipingFailureMechanism ConfigureFailuremechanism()
        {
            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 2",
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            const string arbitraryFilePath = "path";
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, arbitraryFilePath);

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            var stochasticSoilProfile1 = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            };

            var stochasticSoilModelA = new StochasticSoilModel(1, "Model A", "Model B")
            {
                Geometry =
                {
                    new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                },
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile1,
                    new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                    {
                        SoilProfile = new PipingSoilProfile("Profile 2", -8.0, new[]
                        {
                            new PipingSoilLayer(-4.0),
                            new PipingSoilLayer(0.0),
                            new PipingSoilLayer(4.0)
                        }, SoilProfileType.SoilProfile1D, 2)
                    }
                }
            };

            var stochasticSoilProfile5 = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Profile 5", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            };

            var stochasticSoilModelE = new StochasticSoilModel(1, "Model E", "Model F")
            {
                Geometry =
                {
                    new Point2D(1.0, 0.0), new Point2D(6.0, 0.0)
                },
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile5
                }
            };

            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModelA,
                new StochasticSoilModel(1, "Model C", "Model D")
                {
                    Geometry =
                    {
                        new Point2D(1.0, 0.0), new Point2D(4.0, 0.0)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                        {
                            SoilProfile = new PipingSoilProfile("Profile 3", -10.0, new[]
                            {
                                new PipingSoilLayer(-5.0),
                                new PipingSoilLayer(-2.0),
                                new PipingSoilLayer(1.0)
                            }, SoilProfileType.SoilProfile1D, 1)
                        },
                        new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                        {
                            SoilProfile = new PipingSoilProfile("Profile 4", -8.0, new[]
                            {
                                new PipingSoilLayer(-4.0),
                                new PipingSoilLayer(0.0),
                                new PipingSoilLayer(4.0)
                            }, SoilProfileType.SoilProfile1D, 2)
                        }
                    }
                },
                stochasticSoilModelE
            }, arbitraryFilePath);
            return pipingFailureMechanism;
        }

        private PipingCalculationsView ShowPipingCalculationsView()
        {
            var pipingCalculationsView = new PipingCalculationsView();

            testForm.Controls.Add(pipingCalculationsView);
            testForm.Show();

            return pipingCalculationsView;
        }
    }
}