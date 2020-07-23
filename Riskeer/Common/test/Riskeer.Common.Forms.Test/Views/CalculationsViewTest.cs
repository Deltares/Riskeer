// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class CalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 1;

        private Form testForm;

        [Test]
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestCalculationsView(null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestCalculationsView(new CalculationGroup(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestCalculationsView(new CalculationGroup(), failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new TestCalculationsView(new CalculationGroup(), new TestFailureMechanism(), new AssessmentSectionStub()))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsInstanceOf<IView>(view);
            }
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            TestFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), failureMechanism, assessmentSection);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Assert
            Assert.AreEqual(2, listBox.Items.Count);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            TestFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(2, dataGridView.ColumnCount);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Hydraulische belastingenlocatie", dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex].HeaderText);

            foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("DisplayName", column.DisplayMember);
                Assert.AreEqual("This", ((IReadOnlyList<DataGridViewComboBoxColumn>) dataGridView.Columns.OfType<DataGridViewComboBoxColumn>().ToArray())[0].ValueMember);
            }

            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(2, cells.Count);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(2, cells.Count);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 2 (6 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseWithLocations_SelectableHydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), ConfigureFailureMechanism(), assessmentSection);

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(7, hydraulicBoundaryLocationComboboxItems.Count);
            Assert.AreEqual("<selecteer>", hydraulicBoundaryLocationComboboxItems[0].ToString());
            Assert.AreEqual("Location 1", hydraulicBoundaryLocationComboboxItems[1].ToString());
            Assert.AreEqual("Location 2", hydraulicBoundaryLocationComboboxItems[2].ToString());
            Assert.AreEqual("Location 1 (2 m)", hydraulicBoundaryLocationComboboxItems[3].ToString());
            Assert.AreEqual("Location 2 (6 m)", hydraulicBoundaryLocationComboboxItems[4].ToString());
            Assert.AreEqual("Location 1 (4 m)", hydraulicBoundaryLocationComboboxItems[5].ToString());
            Assert.AreEqual("Location 2 (5 m)", hydraulicBoundaryLocationComboboxItems[6].ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismWithSections_SectionsListBoxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            var failureMechanism = new TestFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), failureMechanism, assessmentSection);

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_ChangingListBoxSelection_DataGridViewCorrectlySyncedAndSelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TestCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(assessmentSection);

            var selectionChangedCount = 0;
            calculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

            var listBox = (ListBox)new ControlTester("listBox").TheObject;
            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

            // Precondition
            Assert.AreEqual(2, dataGridView.Rows.Count);
            Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);

            // Call
            listBox.SelectedIndex = 1;

            // Assert
            Assert.AreEqual(2, dataGridView.Rows.Count);
            Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(2, selectionChangedCount);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculationsView_SelectingCellInRow_SelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TestCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(assessmentSection);

            var selectionChangedCount = 0;
            calculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

            var control = TypeUtils.GetField<DataGridViewControl>(calculationsView, "dataGridViewControl");
            WindowsFormsTestHelper.Show(control);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

            // Call                
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(1, 0));

            // Assert
            Assert.AreEqual(1, selectionChangedCount);
            WindowsFormsTestHelper.CloseAll();
            mocks.VerifyAll();
        }

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

        private static void ConfigureHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2),
                    new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4)
                }
            });
        }

        private static TestFailureMechanism ConfigureFailureMechanism()
        {
            var failureMechanism = new TestFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new[]
                {
                    new Point2D(5.0, 0.0),
                    new Point2D(10.0, 0.0)
                })
            });

            return failureMechanism;
        }

        private static CalculationGroup ConfigureCalculationGroup(IAssessmentSection assessmentSection)
        {
            return new CalculationGroup
            {
                Children =
                {
                    new TestCalculation
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First()
                        },
                        Output = null
                    },
                    new TestCalculation
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Last()
                        }
                    }
                }
            };
        }

        private TestCalculationsView ShowFullyConfiguredCalculationsView(IAssessmentSection assessmentSection)
        {
            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            TestFailureMechanism failureMechanism = ConfigureFailureMechanism();

            return ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), failureMechanism, assessmentSection);
        }

        private TestCalculationsView ShowCalculationsView(CalculationGroup calculationGroup, IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var calculationsView = new TestCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            testForm.Controls.Add(calculationsView);
            testForm.Show();

            return calculationsView;
        }

        private class TestCalculationsView : CalculationsView<TestCalculation, TestCalculationRow>
        {
            public TestCalculationsView(CalculationGroup calculationGroup, IFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(calculationGroup, failureMechanism, assessmentSection) {}

            protected override IEnumerable<Point2D> GetReferenceLocations()
            {
                return new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                };
            }

            protected override bool IsCalculationIntersectionWithReferenceLineInSection(TestCalculation calculation, IEnumerable<Segment2D> lineSegments)
            {
                return true;
            }

            protected override TestCalculationRow CreateRow(TestCalculation calculation)
            {
                return new TestCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, calculation.InputParameters));
            }
        }
    }
}