﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.Views;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresCalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 1;
        private const int foreshoreProfileColumnIndex = 2;
        private const int useBreakWaterColumnIndex = 3;
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeShoreGeometryColumnIndex = 6;
        private const int inflowModelTypeColumnIndex = 7;
        private const int meanInsideWaterLevelColumnIndex = 8;
        private const int criticalOvertoppingDischargeColumnIndex = 9;
        private const int allowedLevelIncreaseStorageColumnIndex = 10;
        private Form testForm;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            ClosingStructuresCalculationsView view = ShowCalculationsView(new CalculationGroup(), new ClosingStructuresFailureMechanism(), new AssessmentSectionStub());

            // Assert
            Assert.IsInstanceOf<CalculationsView<StructuresCalculationScenario<ClosingStructuresInput>, ClosingStructuresInput, ClosingStructuresCalculationRow, ClosingStructuresFailureMechanism>>(view);

            var button = (Button)new ControlTester("generateButton").TheObject;
            Assert.AreEqual("Genereer &berekeningen...", button.Text);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);

            Assert.AreEqual(11, dataGridView.ColumnCount);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Hydraulische belastingenlocatie", dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex].HeaderText);
            Assert.AreEqual("Voorlandprofiel", dataGridView.Columns[foreshoreProfileColumnIndex].HeaderText);
            Assert.AreEqual("Gebruik dam", dataGridView.Columns[useBreakWaterColumnIndex].HeaderText);
            Assert.AreEqual("Damtype", dataGridView.Columns[breakWaterTypeColumnIndex].HeaderText);
            Assert.AreEqual("Damhoogte [m+NAP]", dataGridView.Columns[breakWaterHeightColumnIndex].HeaderText);
            Assert.AreEqual("Gebruik voorlandgeometrie", dataGridView.Columns[useForeShoreGeometryColumnIndex].HeaderText);
            Assert.AreEqual("Instroommodel", dataGridView.Columns[inflowModelTypeColumnIndex].HeaderText);
            Assert.AreEqual("Verwachtingswaarde\r\nBinnenwaterstand [m+NAP]", dataGridView.Columns[meanInsideWaterLevelColumnIndex].HeaderText);
            Assert.AreEqual("Verwachtingswaarde\r\nKritiek instromend debiet [m³/s/m]", dataGridView.Columns[criticalOvertoppingDischargeColumnIndex].HeaderText);
            Assert.AreEqual("Verwachtingswaarde\r\nToegestane peilverhoging komberging [m]", dataGridView.Columns[allowedLevelIncreaseStorageColumnIndex].HeaderText);

            foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>().ToArray())
            {
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }

            Assert.AreEqual("This", ((IReadOnlyList<DataGridViewComboBoxColumn>) dataGridView.Columns.OfType<DataGridViewComboBoxColumn>().ToArray())[0].ValueMember);
            Assert.AreEqual("This", ((IReadOnlyList<DataGridViewComboBoxColumn>) dataGridView.Columns.OfType<DataGridViewComboBoxColumn>().ToArray())[1].ValueMember);
            Assert.AreEqual("Value", ((IReadOnlyList<DataGridViewComboBoxColumn>) dataGridView.Columns.OfType<DataGridViewComboBoxColumn>().ToArray())[2].ValueMember);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(11, cells.Count);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("name", cells[foreshoreProfileColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useBreakWaterColumnIndex].FormattedValue);
            Assert.AreEqual("Havendam", cells[breakWaterTypeColumnIndex].FormattedValue);
            Assert.AreEqual(3.30.ToString("0.00", CultureInfo.CurrentCulture), cells[breakWaterHeightColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useForeShoreGeometryColumnIndex].FormattedValue);
            Assert.AreEqual("Verdronken koker", cells[inflowModelTypeColumnIndex].FormattedValue);
            Assert.AreEqual(0.30.ToString("0.00", CultureInfo.CurrentCulture), cells[meanInsideWaterLevelColumnIndex].FormattedValue);
            Assert.AreEqual(0.01.ToString("0.00", CultureInfo.CurrentCulture), cells[criticalOvertoppingDischargeColumnIndex].FormattedValue);
            Assert.AreEqual(100.0.ToString("0.00", CultureInfo.CurrentCulture), cells[allowedLevelIncreaseStorageColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(11, cells.Count);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1 (4 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("name", cells[foreshoreProfileColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useBreakWaterColumnIndex].FormattedValue);
            Assert.AreEqual("Havendam", cells[breakWaterTypeColumnIndex].FormattedValue);
            Assert.AreEqual(3.30.ToString("0.00", CultureInfo.CurrentCulture), cells[breakWaterHeightColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useForeShoreGeometryColumnIndex].FormattedValue);
            Assert.AreEqual("Lage drempel", cells[inflowModelTypeColumnIndex].FormattedValue);
            Assert.AreEqual(0.30.ToString("0.00", CultureInfo.CurrentCulture), cells[meanInsideWaterLevelColumnIndex].FormattedValue);
            Assert.AreEqual(0.01.ToString("0.00", CultureInfo.CurrentCulture), cells[criticalOvertoppingDischargeColumnIndex].FormattedValue);
            Assert.AreEqual(100.0.ToString("0.00", CultureInfo.CurrentCulture), cells[allowedLevelIncreaseStorageColumnIndex].FormattedValue);
            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateCalculations_StructuresPresent_ButtonEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var button = (Button)new ControlTester("generateButton").TheObject;

            // Call
            bool state = button.Enabled;

            // Assert
            Assert.IsTrue(state);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_ChangingStructures_ButtonCorrectState()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();

            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            // Precondition
            var button = (Button)new ControlTester("generateButton").TheObject;
            Assert.IsFalse(button.Enabled);

            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1
            });

            failureMechanism.ClosingStructures.AddRange(new List<ClosingStructure>
            {
                new TestClosingStructure(new Point2D(0.0, 0.0), "Structure 1"),
                new TestClosingStructure(new Point2D(0.0, 0.0), "Structure 2")
            }, string.Empty);

            // Call
            failureMechanism.ClosingStructures.NotifyObservers();

            // Assert
            Assert.IsTrue(button.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("test", breakWaterHeightColumnIndex)]
        [TestCase("test", meanInsideWaterLevelColumnIndex)]
        [TestCase("test", criticalOvertoppingDischargeColumnIndex)]
        [TestCase("test", allowedLevelIncreaseStorageColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", breakWaterHeightColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", meanInsideWaterLevelColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", criticalOvertoppingDischargeColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", allowedLevelIncreaseStorageColumnIndex)]
        public void CalculationsView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

            // Assert
            Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1, breakWaterHeightColumnIndex)]
        [TestCase(1e-2, breakWaterHeightColumnIndex)]
        [TestCase(1e+6, breakWaterHeightColumnIndex)]
        [TestCase(14.3, breakWaterHeightColumnIndex)]
        [TestCase(1, meanInsideWaterLevelColumnIndex)]
        [TestCase(1e+6, meanInsideWaterLevelColumnIndex)]
        [TestCase(14.3, meanInsideWaterLevelColumnIndex)]
        [TestCase(1, criticalOvertoppingDischargeColumnIndex)]
        [TestCase(1e+6, criticalOvertoppingDischargeColumnIndex)]
        [TestCase(14.3, criticalOvertoppingDischargeColumnIndex)]
        [TestCase(1, allowedLevelIncreaseStorageColumnIndex)]
        [TestCase(1e+6, allowedLevelIncreaseStorageColumnIndex)]
        [TestCase(14.3, allowedLevelIncreaseStorageColumnIndex)]
        public void FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
            mocks.VerifyAll();
        }
        
        [Test]
        [TestCase(0, criticalOvertoppingDischargeColumnIndex)]
        [TestCase(-123.45, criticalOvertoppingDischargeColumnIndex)]
        [TestCase(1e-5, criticalOvertoppingDischargeColumnIndex)]
        [TestCase(0, allowedLevelIncreaseStorageColumnIndex)]
        [TestCase(-123.45, allowedLevelIncreaseStorageColumnIndex)]
        [TestCase(1e-5, allowedLevelIncreaseStorageColumnIndex)]
        public void CalculationsView_InvalidOvertoppingAndLevelIncreseStorage_ShowsErrorTooltip(double newValue, int index)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var inputObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (StructuresCalculationScenario<ClosingStructuresInput>) calculationGroup.Children.First();

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(inputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[index].Value = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual("Gemiddelde moet groter zijn dan 0.", dataGridView.Rows[0].ErrorText);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [TestCase(breakWaterHeightColumnIndex, 8.0, true)]
        [TestCase(breakWaterHeightColumnIndex, 8.0, false)]
        [TestCase(meanInsideWaterLevelColumnIndex, 8.0, true)]
        [TestCase(meanInsideWaterLevelColumnIndex, 8.0, false)]
        [TestCase(criticalOvertoppingDischargeColumnIndex, 8.0, true)]
        [TestCase(criticalOvertoppingDischargeColumnIndex, 8.0, false)]
        [TestCase(allowedLevelIncreaseStorageColumnIndex, 8.0, true)]
        [TestCase(allowedLevelIncreaseStorageColumnIndex, 8.0, false)]
        public void CalculationsView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(
            int cellIndex,
            object newValue,
            bool useCalculationWithOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var inputObserver = mocks.StrictMock<IObserver>();

            if (useCalculationWithOutput)
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new MessageBoxTester(wnd);
                    tester.ClickOk();
                };

                calculationObserver.Expect(o => o.UpdateObserver());
            }

            inputObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculationScenario = (StructuresCalculationScenario<ClosingStructuresInput>) calculationGroup.Children[1];

            if (useCalculationWithOutput)
            {
                calculationScenario.Output = new TestStructuresOutput();
            }

            calculationScenario.Attach(calculationObserver);
            calculationScenario.InputParameters.Attach(inputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[1].Cells[cellIndex].Value = newValue is double value ? (RoundedDouble) value : newValue;

            // Assert
            calculationScenario.Output = null;
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void CalculationsView_UseBreakWaterState_HasCorrespondingColumnState(bool newValue, bool expectedState)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            // This step is necessary because setting the same value would not change the view state.
            var calculation = (StructuresCalculationScenario<ClosingStructuresInput>) calculationGroup.GetCalculations().First();
            calculation.InputParameters.UseBreakWater = !newValue;

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[useBreakWaterColumnIndex].Value = newValue;

            // Assert
            Assert.AreEqual(expectedState, dataGridView.Rows[0].Cells[breakWaterTypeColumnIndex].ReadOnly);
            Assert.AreEqual(expectedState, dataGridView.Rows[0].Cells[breakWaterHeightColumnIndex].ReadOnly);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Selection_Always_ReturnsTheSelectedRowObject(int selectedRow)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ClosingStructuresFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ClosingStructuresCalculationsView view = ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[0];

            // Call
            object selection = view.Selection;

            // Assert
            Assert.IsInstanceOf<ClosingStructuresInputContext>(selection);
            var dataRow = (ClosingStructuresCalculationRow) dataGridView.Rows[selectedRow].DataBoundItem;
            Assert.AreSame(dataRow.Calculation, ((ClosingStructuresInputContext) selection).Calculation);
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

        private ClosingStructuresCalculationsView ShowCalculationsView(CalculationGroup calculationGroup, ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var calculationsView = new ClosingStructuresCalculationsView(calculationGroup, failureMechanism, assessmentSection);
            testForm.Controls.Add(calculationsView);
            testForm.Show();

            return calculationsView;
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

        private static CalculationGroup ConfigureCalculationGroup(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var random = new Random(12);
            return new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculationScenario<ClosingStructuresInput>
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Comment for Calculation 1"
                        },
                        InputParameters =
                        {
                            Structure = new TestClosingStructure(new Point2D(0.0, 0.0)),
                            InsideWaterLevel =
                            {
                                Mean = (RoundedDouble) 0.30
                            },
                            CriticalOvertoppingDischarge =
                            {
                                Mean = (RoundedDouble) 0.01
                            },
                            AllowedLevelIncreaseStorage =
                            {
                                Mean = (RoundedDouble) 100.0
                            },
                            InflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>(),
                            ForeshoreProfile = failureMechanism.ForeshoreProfiles.FirstOrDefault(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            BreakWater =
                            {
                                Height = (RoundedDouble) 3.3,
                                Type = BreakWaterType.Dam
                            },
                            UseBreakWater = false,
                            UseForeshore = false
                        },
                        Output = null
                    },
                    new StructuresCalculationScenario<ClosingStructuresInput>
                    {
                        Name = "Calculation 2",
                        Comments =
                        {
                            Body = "Comment for Calculation 2"
                        },
                        InputParameters =
                        {
                            Structure = new TestClosingStructure(new Point2D(5.0, 0.0)),
                            InsideWaterLevel =
                            {
                                Mean = (RoundedDouble) 0.30
                            },
                            CriticalOvertoppingDischarge =
                            {
                                Mean = (RoundedDouble) 0.01
                            },
                            AllowedLevelIncreaseStorage =
                            {
                                Mean = (RoundedDouble) 100.0
                            },
                            InflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>(),
                            ForeshoreProfile = failureMechanism.ForeshoreProfiles.FirstOrDefault(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            BreakWater =
                            {
                                Height = (RoundedDouble) 3.3,
                                Type = BreakWaterType.Dam
                            },
                            UseBreakWater = false,
                            UseForeshore = false
                        },
                        Output = null
                    }
                }
            };
        }

        private static ClosingStructuresFailureMechanism ConfigureFailureMechanism()
        {
            var failureMechanism = new ClosingStructuresFailureMechanism();

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

            failureMechanism.ForeshoreProfiles.AddRange(new List<ForeshoreProfile>
            {
                new TestForeshoreProfile("Profiel 1"),
                new TestForeshoreProfile("Profiel 2")
            }, string.Empty);

            failureMechanism.ClosingStructures.AddRange(new List<ClosingStructure>
            {
                new TestClosingStructure(new Point2D(0.0, 0.0), "Structure 1"),
                new TestClosingStructure(new Point2D(0.0, 0.0), "Structure 2")
            }, string.Empty);

            return failureMechanism;
        }
    }
}