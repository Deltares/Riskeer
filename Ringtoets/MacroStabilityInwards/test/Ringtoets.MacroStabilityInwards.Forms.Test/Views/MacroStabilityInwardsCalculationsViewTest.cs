// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int stochasticSoilModelsColumnIndex = 1;
        private const int stochasticSoilProfilesColumnIndex = 2;
        private const int stochasticSoilProfilesProbabilityColumnIndex = 3;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 4;
        private Form testForm;

        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        public override void TearDown()
        {
            base.TearDown();

            testForm.Dispose();
        }

        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var calculationsView = new MacroStabilityInwardsCalculationsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(calculationsView);
                Assert.IsInstanceOf<IView>(calculationsView);
                Assert.IsInstanceOf<ISelectionProvider>(calculationsView);
                Assert.IsNull(calculationsView.Data);
                Assert.IsNull(calculationsView.MacroStabilityInwardsFailureMechanism);
                Assert.IsNull(calculationsView.AssessmentSection);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ShowMacroStabilityInwardsCalculationsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                Assert.IsFalse(dataGridView.AutoGenerateColumns);
                Assert.AreEqual(5, dataGridView.ColumnCount);

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
            using (ShowMacroStabilityInwardsCalculationsView())
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
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                // Call
                var testDelegate = new TestDelegate(() => macroStabilityInwardsCalculationsView.Data = null);

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

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowSimpleMacroStabilityInwardsCalculationsViewWithoutSurfaceLines(
                assessmentSection, new HydraulicBoundaryDatabase()))
            {
                // Call
                macroStabilityInwardsCalculationsView.AssessmentSection = assessmentSection;

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

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowSimpleMacroStabilityInwardsCalculationsViewWithSurfaceLines(
                assessmentSection))
            {
                // Call
                macroStabilityInwardsCalculationsView.AssessmentSection = assessmentSection;

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

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection))
            {
                // Call
                macroStabilityInwardsCalculationsView.AssessmentSection = assessmentSection;

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
        public void MacroStabilityInwardsFailureMechanism_FailureMechanismWithSections_SectionsListBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
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

            failureMechanism.AddSection(failureMechanismSection1);
            failureMechanism.AddSection(failureMechanismSection2);
            failureMechanism.AddSection(failureMechanismSection3);

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                // Call
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

                // Assert
                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                Assert.AreEqual(3, listBox.Items.Count);
                Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
                Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
                Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            }
        }

        [Test]
        public void MacroStabilityInwardsCalculationsView_CalculationsWithCorrespondingStochasticSoilModel_StochasticSoilModelsComboboxCorrectlyInitialized()
        {
            // Setup & Call
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredMacroStabilityInwardsCalculationsView(assessmentSection))
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
        public void MacroStabilityInwardsCalculationsView_CalculationsWithCorrespondingSoilProfiles_SoilProfilesComboboxCorrectlyInitialized()
        {
            // Setup & Call
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredMacroStabilityInwardsCalculationsView(assessmentSection))
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
        public void MacroStabilityInwardsCalculationsView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredMacroStabilityInwardsCalculationsView(assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(5, cells.Count);
                Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Model A", cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("<geen>", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual("0", cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
                Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(5, cells.Count);
                Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Model E", cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("Profile 5", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual("30", cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
                Assert.AreEqual("Location 2 (5 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void MacroStabilityInwardsCalculationsView_SelectingCellInRow_SelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection);

            var selectionChangedCount = 0;
            macroStabilityInwardsCalculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

            // Call                
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(1, 0));

            // Assert
            Assert.AreEqual(1, selectionChangedCount);

            mocks.VerifyAll();
        }

        [Test]
        public void MacroStabilityInwardsCalculationsView_ChangingListBoxSelection_DataGridViewCorrectlySyncedAndSelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection))
            {
                var selectionChangedCount = 0;
                macroStabilityInwardsCalculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

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
                Assert.AreEqual(2, selectionChangedCount);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateScenarios_WithoutSurfaceLines_ButtonDisabled()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, "path");

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;
                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];

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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine()
            }, "path");

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;
                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];

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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbitrarySourcePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine()
            }, arbitrarySourcePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, arbitrarySourcePath);

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];

                // Call
                bool state = button.Enabled;

                // Assert
                Assert.IsTrue(state);
            }
        }

        [Test]
        public void GivenMacroStabilityInwardsCalculationsViewWithFailureMechanism_WhenSectionsAddedAndFailureMechanismNotified_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var failureMechanismWithSections = new MacroStabilityInwardsFailureMechanism();
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

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanismWithSections;

                var listBox = (ListBox) new ControlTester("listBox").TheObject;

                // Precondition
                Assert.AreEqual(0, listBox.Items.Count);

                failureMechanismWithSections.AddSection(failureMechanismSection1);
                failureMechanismWithSections.AddSection(failureMechanismSection2);
                failureMechanismWithSections.AddSection(failureMechanismSection3);

                // When
                failureMechanismWithSections.NotifyObservers();

                // Then
                Assert.AreEqual(3, listBox.Items.Count);
                Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
                Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
                Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            }
        }

        [Test]
        public void GivenMacroStabilityInwardsCalculationsView_WhenGenerateScenariosButtonClicked_ThenShowViewWithSurfaceLines()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbitraryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine
                {
                    Name = "Line A"
                },
                new MacroStabilityInwardsSurfaceLine
                {
                    Name = "Line B"
                }
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, arbitraryFilePath);

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;
                macroStabilityInwardsCalculationsView.Data = failureMechanism.CalculationsGroup;
                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                MacroStabilityInwardsSurfaceLineSelectionDialog selectionDialog = null;
                DataGridViewControl grid = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
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
        public void GivenMacroStabilityInwardsCalculationsViewGenerateScenariosButtonClicked_WhenNoCalculationGroupDefined_ThenCalculationGroupNotUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbitraryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine
                {
                    Name = "Line A"
                },
                new MacroStabilityInwardsSurfaceLine
                {
                    Name = "Line B"
                }
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, arbitraryFilePath);
            failureMechanism.CalculationsGroup.Attach(observer);

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

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
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenDialogClosed_ThenNotifyCalculationGroup(string buttonName)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbitryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine
                {
                    Name = "Line A"
                },
                new MacroStabilityInwardsSurfaceLine
                {
                    Name = "Line B"
                }
            }, arbitryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel()
            }, arbitryFilePath);
            failureMechanism.CalculationsGroup.Attach(observer);

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;
                macroStabilityInwardsCalculationsView.Data = failureMechanism.CalculationsGroup;
                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
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
        public void GivenMacroStabilityInwardsCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = ConfigureSimpleFailureMechanism();

                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = macroStabilityInwardsFailureMechanism;
                macroStabilityInwardsCalculationsView.Data = macroStabilityInwardsFailureMechanism.CalculationsGroup;

                // Precondition
                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                    selectionView.Rows[0].Cells[0].Value = true;

                    // When
                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                button.Click();

                // Then
                MacroStabilityInwardsCalculationScenario[] macroStabilityInwardsCalculationScenarios = macroStabilityInwardsFailureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>().ToArray();
                MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult1 = macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism.SectionResults.First();
                MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult2 = macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism.SectionResults.ElementAt(1);

                Assert.AreEqual(2, failureMechanismSectionResult1.GetCalculationScenarios(macroStabilityInwardsCalculationScenarios).Count());

                foreach (MacroStabilityInwardsCalculationScenario calculationScenario in failureMechanismSectionResult1.GetCalculationScenarios(macroStabilityInwardsCalculationScenarios))
                {
                    Assert.IsInstanceOf<ICalculationScenario>(calculationScenario);
                }

                CollectionAssert.IsEmpty(failureMechanismSectionResult2.GetCalculationScenarios(macroStabilityInwardsCalculationScenarios));
            }
        }

        [Test]
        public void GivenMacroStabilityInwardsCalculationsViewGenerateScenariosCancelButtonClicked_WhenDialogClosed_CalculationsNotUpdated()
        {
            // Given
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = ConfigureSimpleFailureMechanism();

                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = macroStabilityInwardsFailureMechanism;
                macroStabilityInwardsCalculationsView.Data = macroStabilityInwardsFailureMechanism.CalculationsGroup;

                var button = new ButtonTester("buttonGenerateScenarios", testForm);

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                    selectionView.Rows[0].Cells[0].Value = true;

                    // When
                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                button.Click();

                // Then
                Assert.IsEmpty(macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism.Calculations);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSoilModelAndNotify_ThenButtonDisabled()
        {
            // Given
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

                // When
                failureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, "path");
                failureMechanism.NotifyObservers();

                // Then
                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndNotify_ThenButtonDisabled()
        {
            // Given
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

                // When
                failureMechanism.SurfaceLines.AddRange(new[]
                {
                    new MacroStabilityInwardsSurfaceLine()
                }, "path");
                failureMechanism.NotifyObservers();

                // Then
                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndSoilModelAndDoNotNotifyObservers_ThenButtonDisabled()
        {
            // Given
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

                // When
                const string arbitraryFilePath = "path";
                failureMechanism.SurfaceLines.AddRange(new[]
                {
                    new MacroStabilityInwardsSurfaceLine()
                }, arbitraryFilePath);
                failureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, arbitraryFilePath);

                // Then
                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndSoilModelAndNotifyObservers_ThenButtonEnabled()
        {
            // Given
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

                // When
                const string arbitraryFilePath = "path";
                failureMechanism.SurfaceLines.AddRange(new[]
                {
                    new MacroStabilityInwardsSurfaceLine()
                }, arbitraryFilePath);
                failureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, arbitraryFilePath);
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism.NotifyObservers();

                // Then
                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsTrue(button.Enabled);
            }
        }

        [Test]
        public void GivenFailureMechanismWithSurfaceLinesAndSoilModels_WhenSurfaceLinesAndSoilModelsClearedAndNotifyObservers_ThenButtonDisabled()
        {
            // Given
            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                const string arbitraryFilePath = "path";
                failureMechanism.SurfaceLines.AddRange(new[]
                {
                    new MacroStabilityInwardsSurfaceLine()
                }, arbitraryFilePath);
                failureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    new TestStochasticSoilModel()
                }, arbitraryFilePath);
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

                // When
                failureMechanism.SurfaceLines.Clear();
                failureMechanism.StochasticSoilModels.Clear();
                macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism.NotifyObservers();

                // Then
                var button = (Button) macroStabilityInwardsCalculationsView.Controls.Find("buttonGenerateScenarios", true)[0];
                Assert.IsFalse(button.Enabled);
            }
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

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[0];

                // Call
                object selection = macroStabilityInwardsCalculationView.Selection;

                // Assert
                Assert.IsInstanceOf<MacroStabilityInwardsInputContext>(selection);
                var dataRow = (MacroStabilityInwardsCalculationRow) dataGridView.Rows[selectedRow].DataBoundItem;
                Assert.AreSame(dataRow.MacroStabilityInwardsCalculation, ((MacroStabilityInwardsInputContext) selection).MacroStabilityInwardsCalculation);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void MacroStabilityInwardsCalculationsView_EditingNameViaDataGridView_ObserversCorrectlyNotified(bool useCalculationWithOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();

            calculationObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) macroStabilityInwardsCalculationView.Data;
                var calculation = (MacroStabilityInwardsCalculationScenario) data.Children.First();

                if (useCalculationWithOutput)
                {
                    calculation.Output = new TestMacroStabilityInwardsOutput();
                }

                calculation.Attach(calculationObserver);
                calculation.InputParameters.Attach(calculationInputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[nameColumnIndex].Value = "New name";

                // Assert
                calculation.Output = null;
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(stochasticSoilProfilesColumnIndex, null, true)]
        [TestCase(stochasticSoilProfilesColumnIndex, null, false)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, true)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, false)]
        public void MacroStabilityInwardsCalculationsView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(
            int cellIndex,
            object newValue,
            bool useCalculationWithOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();

            if (useCalculationWithOutput)
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new MessageBoxTester(wnd);
                    tester.ClickOk();
                };

                calculationObserver.Expect(o => o.UpdateObserver());
            }

            calculationInputObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailuremechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection, failureMechanism, calculationGroup))
            {
                mocks.ReplayAll();

                var data = (CalculationGroup) macroStabilityInwardsCalculationView.Data;
                var calculation = (MacroStabilityInwardsCalculationScenario) data.Children[1];

                if (useCalculationWithOutput)
                {
                    calculation.Output = new TestMacroStabilityInwardsOutput();
                }

                calculation.Attach(calculationObserver);
                calculation.InputParameters.Attach(calculationInputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[1].Cells[cellIndex].Value = newValue is double ? (RoundedDouble) (double) newValue : newValue;

                // Assert
                calculation.Output = null;
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenMacroStabilityInwardsCalculationWithHydraulicLocation_WhenLocationManualDesignWaterLevel_SelectableHydraulicLocationReadonly()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) macroStabilityInwardsCalculationView.Data;
                var calculation = (MacroStabilityInwardsCalculationScenario) data.Children.First();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                var currentCell = (DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex];
                Assert.IsFalse(currentCell.ReadOnly);

                // When
                calculation.InputParameters.UseAssessmentLevelManualInput = true;
                calculation.InputParameters.NotifyObservers();

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
        public void GivenMacroStabilityInwardsCalculationWithStochasticSoilProfile_WhenProbabilityChangesAndNotified_ThenNewProbabilityVisible()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) macroStabilityInwardsCalculationView.Data;
                var calculation = (MacroStabilityInwardsCalculationScenario) data.Children[1];

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var refreshed = 0;

                // Precondition
                var currentCell = (DataGridViewTextBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex];
                Assert.AreEqual("30", currentCell.FormattedValue);

                StochasticSoilProfile stochasticSoilProfileToChange = calculation.InputParameters.StochasticSoilProfile;
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
        public void GivenMacroStabilityInwardsCalculationViewWithCalculations_WhenSurfaceLineLocatedOutsideSectionAfterUpdateAndObserversNotified_ThenDataGridViewUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationView = ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) macroStabilityInwardsCalculationView.Data;
                var calculation = (MacroStabilityInwardsCalculationScenario) data.Children[0];

                DataGridViewControl dataGridView = macroStabilityInwardsCalculationView.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
                ListBox listBox = macroStabilityInwardsCalculationView.Controls.Find("listBox", true).OfType<ListBox>().First();

                // Precondition
                listBox.SelectedIndex = 0;
                Assert.AreEqual(2, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);

                listBox.SelectedIndex = 1;
                Assert.AreEqual(1, dataGridView.Rows.Count);
                Assert.AreEqual("Calculation 2", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                MacroStabilityInwardsSurfaceLine surfaceLineToChange = calculation.InputParameters.SurfaceLine;
                var updatedSurfaceLine = new MacroStabilityInwardsSurfaceLine
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

        private MacroStabilityInwardsCalculationsView ShowFullyConfiguredMacroStabilityInwardsCalculationsView(
            IAssessmentSection assessmentSection)
        {
            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailuremechanism();

            return ShowFullyConfiguredMacroStabilityInwardsCalculationsView(assessmentSection,
                                                                            failureMechanism,
                                                                            ConfigureCalculationGroup(assessmentSection, failureMechanism));
        }

        private MacroStabilityInwardsCalculationsView ShowFullyConfiguredMacroStabilityInwardsCalculationsView(IAssessmentSection assessmentSection,
                                                                                                               MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                                                               CalculationGroup calculationGroup)
        {
            MacroStabilityInwardsCalculationsView view = ShowMacroStabilityInwardsCalculationsView();
            view.Data = calculationGroup;
            view.AssessmentSection = assessmentSection;
            view.MacroStabilityInwardsFailureMechanism = failureMechanism;

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

        private MacroStabilityInwardsCalculationsView ShowSimpleMacroStabilityInwardsCalculationsViewWithSurfaceLines(IAssessmentSection assessmentSection)
        {
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine
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

            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, "path");

            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView();

            macroStabilityInwardsCalculationsView.Data = new CalculationGroup("Group", true)
            {
                Children =
                {
                    new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine1
                        }
                    },
                    new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine2
                        }
                    }
                }
            };

            macroStabilityInwardsCalculationsView.AssessmentSection = assessmentSection;
            macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

            return macroStabilityInwardsCalculationsView;
        }

        private MacroStabilityInwardsCalculationsView ShowSimpleMacroStabilityInwardsCalculationsViewWithoutSurfaceLines(IAssessmentSection assessmentSection,
                                                                                                                         HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4);

            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation1);
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation2);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            MacroStabilityInwardsCalculationsView macroStabilityInwardsCalculationsView = ShowMacroStabilityInwardsCalculationsView();

            macroStabilityInwardsCalculationsView.Data = new CalculationGroup("Group", true)
            {
                Children =
                {
                    new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation1
                        }
                    },
                    new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                        }
                    }
                }
            };

            macroStabilityInwardsCalculationsView.AssessmentSection = assessmentSection;
            macroStabilityInwardsCalculationsView.MacroStabilityInwardsFailureMechanism = failureMechanism;

            return macroStabilityInwardsCalculationsView;
        }

        private static MacroStabilityInwardsFailureMechanism ConfigureSimpleFailureMechanism()
        {
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine
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

            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbirtraryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, arbirtraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel
                {
                    Geometry =
                    {
                        new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                    }
                }
            }, arbirtraryFilePath);

            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            return failureMechanism;
        }

        private static CalculationGroup ConfigureCalculationGroup(IAssessmentSection assessmentSection, MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            StochasticSoilModel stochasticSoilModelForCalculation2 = failureMechanism.StochasticSoilModels.Last();
            return new CalculationGroup("Group", true)
            {
                Children =
                {
                    new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.First(),
                            StochasticSoilModel = failureMechanism.StochasticSoilModels.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First()
                        }
                    },
                    new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.Last(),
                            StochasticSoilModel = stochasticSoilModelForCalculation2,
                            StochasticSoilProfile = stochasticSoilModelForCalculation2.StochasticSoilProfiles.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Last()
                        }
                    }
                }
            };
        }

        private static MacroStabilityInwardsFailureMechanism ConfigureFailuremechanism()
        {
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine
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

            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbitraryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, arbitraryFilePath);

            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            var stochasticSoilProfile1 = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(-5.0),
                    new MacroStabilityInwardsSoilLayer1D(-2.0),
                    new MacroStabilityInwardsSoilLayer1D(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            };

            var stochasticSoilModelA = new StochasticSoilModel("Model A")
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
                        SoilProfile = new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
                        {
                            new MacroStabilityInwardsSoilLayer1D(-4.0),
                            new MacroStabilityInwardsSoilLayer1D(0.0),
                            new MacroStabilityInwardsSoilLayer1D(4.0)
                        }, SoilProfileType.SoilProfile1D, 2)
                    }
                }
            };

            var stochasticSoilProfile5 = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D("Profile 5", -10.0, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(-5.0),
                    new MacroStabilityInwardsSoilLayer1D(-2.0),
                    new MacroStabilityInwardsSoilLayer1D(1.0)
                }, SoilProfileType.SoilProfile1D, 1)
            };

            var stochasticSoilModelE = new StochasticSoilModel("Model E")
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

            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModelA,
                new StochasticSoilModel("Model C")
                {
                    Geometry =
                    {
                        new Point2D(1.0, 0.0), new Point2D(4.0, 0.0)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                        {
                            SoilProfile = new MacroStabilityInwardsSoilProfile1D("Profile 3", -10.0, new[]
                            {
                                new MacroStabilityInwardsSoilLayer1D(-5.0),
                                new MacroStabilityInwardsSoilLayer1D(-2.0),
                                new MacroStabilityInwardsSoilLayer1D(1.0)
                            }, SoilProfileType.SoilProfile1D, 1)
                        },
                        new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                        {
                            SoilProfile = new MacroStabilityInwardsSoilProfile1D("Profile 4", -8.0, new[]
                            {
                                new MacroStabilityInwardsSoilLayer1D(-4.0),
                                new MacroStabilityInwardsSoilLayer1D(0.0),
                                new MacroStabilityInwardsSoilLayer1D(4.0)
                            }, SoilProfileType.SoilProfile1D, 2)
                        }
                    }
                },
                stochasticSoilModelE
            }, arbitraryFilePath);
            return failureMechanism;
        }

        private MacroStabilityInwardsCalculationsView ShowMacroStabilityInwardsCalculationsView()
        {
            var calculationsView = new MacroStabilityInwardsCalculationsView();

            testForm.Controls.Add(calculationsView);
            testForm.Show();

            return calculationsView;
        }
    }
}