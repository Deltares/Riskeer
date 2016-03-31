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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Gui.Selection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingCalculationsViewTest
    {
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
        public void Constructor_DefaultValues()
        {
            // Call
            var pipingCalculationsView = new PipingCalculationsView();

            // Assert
            Assert.IsInstanceOf<UserControl>(pipingCalculationsView);
            Assert.IsInstanceOf<IView>(pipingCalculationsView);
            Assert.IsNull(pipingCalculationsView.Data);
            Assert.IsNull(pipingCalculationsView.PipingFailureMechanism);
            Assert.IsNull(pipingCalculationsView.AssessmentSection);
            Assert.IsNull(pipingCalculationsView.ApplicationSelection);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowPipingCalculationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(7, dataGridView.ColumnCount);

            foreach (var column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("This", column.ValueMember);
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }

            foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
            {
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, column.HeaderCell.Style.Alignment);
            }

            var soilProfilesCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[soilProfilesColumnIndex];
            var soilProfilesComboboxItems = soilProfilesCombobox.Items;
            Assert.AreEqual(0, soilProfilesComboboxItems.Count); // Row dependend

            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[hydraulicBoundaryLocationsColumnIndex];
            var hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(1, hydraulicBoundaryLocationComboboxItems.Count);
            Assert.AreEqual("<geen>", hydraulicBoundaryLocationComboboxItems[0].ToString());
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup & Call
            ShowPipingCalculationsView();

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Assert
            Assert.AreEqual(0, listBox.Items.Count);
        }

        [Test]
        public void Dispose_PipingCalculationViewWithAdditionalPropertiesSet_AdditionalPropertiesSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();

            mocks.ReplayAll();

            var pipingCalculationsView = new PipingCalculationsView
            {
                PipingFailureMechanism = pipingFailureMechanism,
                AssessmentSection = assessmentSection,
                ApplicationSelection = applicationSelection
            };

            // Precondition
            Assert.IsNotNull(pipingCalculationsView.PipingFailureMechanism);
            Assert.IsNotNull(pipingCalculationsView.AssessmentSection);
            Assert.IsNotNull(pipingCalculationsView.ApplicationSelection);

            // Call
            pipingCalculationsView.Dispose();

            // Assert
            Assert.IsNull(pipingCalculationsView.PipingFailureMechanism);
            Assert.IsNull(pipingCalculationsView.AssessmentSection);
            Assert.IsNull(pipingCalculationsView.ApplicationSelection);
        }

        [Test]
        public void AssessmentSection_HydraulicBoundaryDatabaseNull_HydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var pipingCalculationsView = ShowPipingCalculationsView();

            mocks.ReplayAll();

            // Call
            pipingCalculationsView.AssessmentSection = assessmentSection;

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[hydraulicBoundaryLocationsColumnIndex];
            var hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(1, hydraulicBoundaryLocationComboboxItems.Count);
            Assert.AreEqual("<geen>", hydraulicBoundaryLocationComboboxItems[0].ToString());
        }

        [Test]
        public void AssessmentSection_HydraulicBoundaryDatabaseWithLocations_HydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var hydraulicBoundaryDatabase = mocks.StrictMock<HydraulicBoundaryDatabase>();

            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2));
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4));

            var pipingCalculationsView = ShowPipingCalculationsView();

            // Call
            pipingCalculationsView.AssessmentSection = assessmentSection;

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[hydraulicBoundaryLocationsColumnIndex];
            var hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(3, hydraulicBoundaryLocationComboboxItems.Count);
            Assert.AreEqual("<geen>", hydraulicBoundaryLocationComboboxItems[0].ToString());
            Assert.AreEqual("Location 1", hydraulicBoundaryLocationComboboxItems[1].ToString());
            Assert.AreEqual("Location 2", hydraulicBoundaryLocationComboboxItems[2].ToString());
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

            var pipingCalculationsView = ShowPipingCalculationsView();

            // Call
            pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
        }

        [Test]
        public void PipingCalculationsView_CalculationsWithCorrespondingSoilProfiles_SoilProfilesComboboxCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredPipingCalculationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            var soilProfilesComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[soilProfilesColumnIndex]).Items;
            Assert.AreEqual(3, soilProfilesComboboxItems.Count);
            Assert.AreEqual("<geen>", soilProfilesComboboxItems[0].ToString());
            Assert.AreEqual("Profile 1", soilProfilesComboboxItems[1].ToString());
            Assert.AreEqual("Profile 2", soilProfilesComboboxItems[2].ToString());

            soilProfilesComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[1].Cells[soilProfilesColumnIndex]).Items;
            Assert.AreEqual(4, soilProfilesComboboxItems.Count);
            Assert.AreEqual("<geen>", soilProfilesComboboxItems[0].ToString());
            Assert.AreEqual("Profile 1", soilProfilesComboboxItems[1].ToString());
            Assert.AreEqual("Profile 2", soilProfilesComboboxItems[2].ToString());
            Assert.AreEqual("Profile 5", soilProfilesComboboxItems[3].ToString());
        }

        [Test]
        public void PipingCalculationsView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredPipingCalculationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            var rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            var cells = rows[0].Cells;
            Assert.AreEqual(7, cells.Count);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Profile 1", cells[soilProfilesColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1", cells[hydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("1.111", cells[dampingFactorExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual("2.222", cells[phreaticLevelExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual("3.33", cells[entryPointLColumnIndex].FormattedValue);
            Assert.AreEqual("4.44", cells[exitPointLColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(7, cells.Count);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Profile 5", cells[soilProfilesColumnIndex].FormattedValue);
            Assert.AreEqual("Location 2", cells[hydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("5.556", cells[dampingFactorExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual("6.667", cells[phreaticLevelExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual("7.78", cells[entryPointLColumnIndex].FormattedValue);
            Assert.AreEqual("8.89", cells[exitPointLColumnIndex].FormattedValue);
        }

        [TestCase(nameColumnIndex, "New name", 1, 0)]
        [TestCase(soilProfilesColumnIndex, null, 0, 1)]
        [TestCase(hydraulicBoundaryLocationsColumnIndex, null, 0, 1)]
        [TestCase(dampingFactorExitMeanColumnIndex, 1.1, 0, 1)]
        [TestCase(phreaticLevelExitMeanColumnIndex, 1.1, 0, 1)]
        [TestCase(entryPointLColumnIndex, 1.1, 0, 1)]
        [TestCase(exitPointLColumnIndex, 1.1, 0, 1)]
        public void PipingCalculationsView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(int cellIndex, object newValue, int expectedPipingCalculationCounter, int expectedPipingCalculationInputCounter)
        {
            // Setup & Call
            var pipingCalculationView = ShowFullyConfiguredPipingCalculationsView();

            var data = (PipingCalculationGroup) pipingCalculationView.Data;
            var pipingCalculation = (PipingCalculation) data.Children.First();
            var pipingCalculationCounter = 0;
            var pipingCalculationInputCounter = 0;
            var pipingCalculationObserver = new Observer(() => pipingCalculationCounter++);
            var pipingCalculationInputObserver = new Observer(() => pipingCalculationInputCounter++);

            pipingCalculation.Attach(pipingCalculationObserver);
            pipingCalculation.InputParameters.Attach(pipingCalculationInputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue is double ? (RoundedDouble) (double) newValue : newValue;
            Assert.AreEqual(expectedPipingCalculationCounter, pipingCalculationCounter);
            Assert.AreEqual(expectedPipingCalculationInputCounter, pipingCalculationInputCounter);
        }

        private const int nameColumnIndex = 0;
        private const int soilProfilesColumnIndex = 1;
        private const int hydraulicBoundaryLocationsColumnIndex = 2;
        private const int dampingFactorExitMeanColumnIndex = 3;
        private const int phreaticLevelExitMeanColumnIndex = 4;
        private const int entryPointLColumnIndex = 5;
        private const int exitPointLColumnIndex = 6;

        private PipingCalculationsView ShowFullyConfiguredPipingCalculationsView()
        {
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var hydraulicBoundaryDatabase = mocks.StrictMock<HydraulicBoundaryDatabase>();
            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4);

            mocks.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation1);
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation2);

            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
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
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            var pipingSoilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, 1);

            var pipingSoilProfile5 = new PipingSoilProfile("Profile 5", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, 1);

            pipingFailureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(1, "A", "B")
            {
                Geometry =
                {
                    new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                    {
                        SoilProfile = pipingSoilProfile1
                    },
                    new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                    {
                        SoilProfile = new PipingSoilProfile("Profile 2", -8.0, new[]
                        {
                            new PipingSoilLayer(-4.0),
                            new PipingSoilLayer(0.0),
                            new PipingSoilLayer(4.0)
                        }, 2)
                    }
                }
            });

            pipingFailureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(1, "C", "D")
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
                        }, 1)
                    },
                    new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, 2)
                    {
                        SoilProfile = new PipingSoilProfile("Profile 4", -8.0, new[]
                        {
                            new PipingSoilLayer(-4.0),
                            new PipingSoilLayer(0.0),
                            new PipingSoilLayer(4.0)
                        }, 2)
                    }
                }
            });

            pipingFailureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(1, "E", "F")
            {
                Geometry =
                {
                    new Point2D(1.0, 0.0), new Point2D(6.0, 0.0)
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 1)
                    {
                        SoilProfile = pipingSoilProfile5
                    }
                }
            });

            var pipingCalculationsView = ShowPipingCalculationsView();

            pipingCalculationsView.Data = new PipingCalculationGroup("Group", true)
            {
                Children =
                {
                    new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine1,
                            SoilProfile = pipingSoilProfile1,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation1,
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
                    new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine2,
                            SoilProfile = pipingSoilProfile5,
                            HydraulicBoundaryLocation = hydraulicBoundaryLocation2,
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

            pipingCalculationsView.AssessmentSection = assessmentSection;
            pipingCalculationsView.PipingFailureMechanism = pipingFailureMechanism;

            return pipingCalculationsView;
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