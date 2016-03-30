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

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

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

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;

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
            ShowConfiguredPipingCalculationsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

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

        private const int soilProfilesColumnIndex = 1;
        private const int hydraulicBoundaryLocationsColumnIndex = 2;

        private PipingCalculationsView ShowConfiguredPipingCalculationsView()
        {
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
                        SoilProfile = new PipingSoilProfile("Profile 1", -10.0, new[]
                        {
                            new PipingSoilLayer(-5.0),
                            new PipingSoilLayer(-2.0),
                            new PipingSoilLayer(1.0)
                        }, 1)
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
                        SoilProfile = new PipingSoilProfile("Profile 5", -10.0, new[]
                        {
                            new PipingSoilLayer(-5.0),
                            new PipingSoilLayer(-2.0),
                            new PipingSoilLayer(1.0)
                        }, 1)
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
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine1
                        }
                    },
                    new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
                    {
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine2
                        }
                    }
                }
            };

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