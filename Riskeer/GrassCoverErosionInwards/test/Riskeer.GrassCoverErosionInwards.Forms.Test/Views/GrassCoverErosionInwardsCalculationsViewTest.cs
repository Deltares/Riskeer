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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 1;
        private const int dikeProfileColumnIndex = 2;
        private const int useBreakWaterColumnIndex = 3;
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeShoreGeometryColumnIndex = 6;
        private const int dikeHeightColumnIndex = 7;
        private const int meanCriticalFlowRateColumnIndex = 8;
        private const int standardDeviationCriticalFlowRateColumnIndex = 9;
        private Form testForm;

        [Test]
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionInwardsCalculationsView(null, new TestGrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            void Call() => new GrassCoverErosionInwardsCalculationsView(new CalculationGroup(), null, assessmentSection);

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
            mocks.ReplayAll();
            
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();

            // Call
            void Call() => new GrassCoverErosionInwardsCalculationsView(new CalculationGroup(), failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            using (var calculationsView = new GrassCoverErosionInwardsCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(calculationsView);
                Assert.IsInstanceOf<IView>(calculationsView);
                Assert.IsInstanceOf<ISelectionProvider>(calculationsView);
            }

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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(10, dataGridView.ColumnCount);

            AssertColumnMembers(dataGridView.Columns.OfType<DataGridViewComboBoxColumn>().ToArray());

            AssertDataGridViewControlColumnHeaders(dataGridView);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Assert
            Assert.AreEqual(2, listBox.Items.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentSection_HydraulicBoundaryDatabaseWithLocations_SelectableHydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (ShowFullyConfiguredCalculationsView(
                assessmentSection))
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateCalculations_DikeProfilesPresent_ButtonEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            using (GrassCoverErosionInwardsCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(assessmentSection))
            {
                var button = (Button) calculationsView.Controls.Find("buttonGenerateCalculations", true)[0];

                // Call
                bool state = button.Enabled;

                // Assert
                Assert.IsTrue(state);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void FailureMechanism_FailureMechanismWithSections_SectionsListBoxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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

            failureMechanism.DikeProfiles.AddRange(new List<DikeProfile>
            {
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0), "profiel 1"),
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(5.0, 0.0), "profiel 2")
            }, string.Empty);

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);

            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (ShowFullyConfiguredCalculationsView(assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(10, cells.Count);
                Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
                Assert.AreEqual("name", cells[dikeProfileColumnIndex].FormattedValue);
                Assert.AreEqual(false, cells[useBreakWaterColumnIndex].FormattedValue);
                Assert.AreEqual("Havendam", cells[breakWaterTypeColumnIndex].FormattedValue);
                Assert.AreEqual(3.30.ToString("0.00", CultureInfo.CurrentCulture), cells[breakWaterHeightColumnIndex].FormattedValue);
                Assert.AreEqual(false, cells[useForeShoreGeometryColumnIndex].FormattedValue);
                Assert.AreEqual(1.10.ToString("0.00", CultureInfo.CurrentCulture), cells[dikeHeightColumnIndex].FormattedValue);
                Assert.AreEqual(4.4000.ToString("0.0000", CultureInfo.CurrentCulture), cells[meanCriticalFlowRateColumnIndex].FormattedValue);
                Assert.AreEqual(5.5000.ToString("0.0000", CultureInfo.CurrentCulture), cells[standardDeviationCriticalFlowRateColumnIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(10, cells.Count);
            }

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

            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            using (var calculationsView = new GrassCoverErosionInwardsCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection))
            {
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
            }

            WindowsFormsTestHelper.CloseAll();
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_ChangingListBoxSelection_DataGridViewCorrectlySyncedAndSelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (GrassCoverErosionInwardsCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(
                assessmentSection))
            {
                var selectionChangedCount = 0;
                calculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

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
        [TestCase("test", breakWaterHeightColumnIndex)]
        [TestCase("test", dikeHeightColumnIndex)]
        [TestCase("test", meanCriticalFlowRateColumnIndex)]
        [TestCase("test", standardDeviationCriticalFlowRateColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", breakWaterHeightColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", dikeHeightColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", meanCriticalFlowRateColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", standardDeviationCriticalFlowRateColumnIndex)]
        public void CalculationsView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (ShowFullyConfiguredCalculationsView(assessmentSection))
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
        [TestCase(1, breakWaterHeightColumnIndex)]
        [TestCase(1e-2, breakWaterHeightColumnIndex)]
        [TestCase(1e+6, breakWaterHeightColumnIndex)]
        [TestCase(14.3, breakWaterHeightColumnIndex)]
        [TestCase(1, dikeHeightColumnIndex)]
        [TestCase(1e-6, dikeHeightColumnIndex)]
        [TestCase(1e+6, dikeHeightColumnIndex)]
        [TestCase(14.3, dikeHeightColumnIndex)]
        [TestCase(1, meanCriticalFlowRateColumnIndex)]
        [TestCase(1e+6, meanCriticalFlowRateColumnIndex)]
        [TestCase(14.3, meanCriticalFlowRateColumnIndex)]
        [TestCase(1, standardDeviationCriticalFlowRateColumnIndex)]
        [TestCase(1e-6, standardDeviationCriticalFlowRateColumnIndex)]
        [TestCase(1e+6, standardDeviationCriticalFlowRateColumnIndex)]
        [TestCase(14.3, standardDeviationCriticalFlowRateColumnIndex)]
        public void FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            var newRoundedValue = (RoundedDouble) newValue;

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            mocks.ReplayAll();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newRoundedValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsViewWithFailureMechanism_WhenSectionsAddedAndFailureMechanismNotified_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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
                failureMechanismSection2
            });

            failureMechanism.DikeProfiles.AddRange(new List<DikeProfile>
            {
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0), "profiel 1"),
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(5.0, 0.0), "profiel 2"),
            }, string.Empty);

            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Precondition
            Assert.AreEqual(2, listBox.Items.Count);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });

            failureMechanism.DikeProfiles.AddRange(new List<DikeProfile>
            {
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(10.0, 0.0), "profiel 3")
            }, string.Empty);

            // When
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(-123.45)]
        [TestCase(1e-5)]
        public void CalculationsView_InvalidMeanCriticalFlowRate_ShowsErrorTooltip(double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var inputObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var newRoundedValue = (RoundedDouble) newValue;

            using (GrassCoverErosionInwardsCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) calculationsView.Data;
                var calculation = (GrassCoverErosionInwardsCalculationScenario) data.Children.First();

                calculation.Attach(calculationObserver);
                calculation.InputParameters.Attach(inputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[meanCriticalFlowRateColumnIndex].Value = newRoundedValue;

                // Assert
                Assert.AreEqual("Gemiddelde moet groter zijn dan 0.", dataGridView.Rows[0].ErrorText);
            }

            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void CalculationsView_InvalidStandardDeviationCriticalFlowRate_ShowsErrorTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var inputObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var newRoundedValue = (RoundedDouble) (-123.45);

            using (GrassCoverErosionInwardsCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) calculationsView.Data;
                var calculation = (GrassCoverErosionInwardsCalculationScenario) data.Children.First();

                calculation.Attach(calculationObserver);
                calculation.InputParameters.Attach(inputObserver);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[standardDeviationCriticalFlowRateColumnIndex].Value = newRoundedValue;

                // Assert
                Assert.AreEqual("Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.", dataGridView.Rows[0].ErrorText);
            }

            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [TestCase(breakWaterHeightColumnIndex, 8.0, true)]
        [TestCase(breakWaterHeightColumnIndex, 8.0, false)]
        [TestCase(dikeHeightColumnIndex, 8.0, true)]
        [TestCase(dikeHeightColumnIndex, 8.0, false)]
        [TestCase(meanCriticalFlowRateColumnIndex, 8.0, true)]
        [TestCase(meanCriticalFlowRateColumnIndex, 8.0, false)]
        [TestCase(standardDeviationCriticalFlowRateColumnIndex, 8.0, true)]
        [TestCase(standardDeviationCriticalFlowRateColumnIndex, 8.0, false)]
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
            assessmentSection.Replay();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            GrassCoverErosionInwardsCalculationsView calculationsView = ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            mocks.ReplayAll();

            var data = (CalculationGroup) calculationsView.Data;
            var calculationScenario = (GrassCoverErosionInwardsCalculationScenario) data.Children[1];

            if (useCalculationWithOutput)
            {
                calculationScenario.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(2.4),
                                                                                new TestDikeHeightOutput(4.2),
                                                                                new TestOvertoppingRateOutput(1.0));
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
            mocks.ReplayAll();

            using (ShowFullyConfiguredCalculationsView(
                assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[useBreakWaterColumnIndex].Value = newValue;

                // Assert
                Assert.AreEqual(expectedState, dataGridView.Rows[0].Cells[breakWaterTypeColumnIndex].ReadOnly);
                Assert.AreEqual(expectedState, dataGridView.Rows[0].Cells[breakWaterHeightColumnIndex].ReadOnly);
            }

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
            mocks.ReplayAll();

            using (GrassCoverErosionInwardsCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(
                assessmentSection))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[0];

                // Call
                object selection = calculationsView.Selection;

                // Assert
                Assert.IsInstanceOf<GrassCoverErosionInwardsInputContext>(selection);
                var dataRow = (GrassCoverErosionInwardsCalculationRow) dataGridView.Rows[selectedRow].DataBoundItem;
                Assert.AreSame(dataRow.CalculationScenario, ((GrassCoverErosionInwardsInputContext) selection).Calculation);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculationsView_EditingNameViaDataGridView_ObserversCorrectlyNotified(bool useCalculationWithOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();

            calculationObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (GrassCoverErosionInwardsCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(
                assessmentSection))
            {
                var data = (CalculationGroup) calculationsView.Data;
                var calculation = (GrassCoverErosionInwardsCalculationScenario) data.Children.First();

                if (useCalculationWithOutput)
                {
                    calculation.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(2.4),
                                                                            new TestDikeHeightOutput(4.2),
                                                                            new TestOvertoppingRateOutput(1.0));
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

        private static void AssertColumnMembers(IReadOnlyList<DataGridViewComboBoxColumn> dataGridViewColumns)
        {
            foreach (DataGridViewComboBoxColumn column in dataGridViewColumns)
            {
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }

            Assert.AreEqual("This", dataGridViewColumns[0].ValueMember);
            Assert.AreEqual("This", dataGridViewColumns[1].ValueMember);
            Assert.AreEqual("Value", dataGridViewColumns[2].ValueMember);
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

        private GrassCoverErosionInwardsCalculationsView ShowFullyConfiguredCalculationsView(
            IAssessmentSection assessmentSection)
        {
            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            return ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);
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

        private static CalculationGroup ConfigureCalculationGroup(GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var random = new Random(12);
            return new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculationScenario
                    {
                        Name = "Calculation 1",
                        Comments =
                        {
                            Body = "Comment for Calculation 1"
                        },
                        InputParameters =
                        {
                            DikeProfile = failureMechanism.DikeProfiles.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            DikeHeight = (RoundedDouble) 1.1,
                            Orientation = (RoundedDouble) 2.2,
                            BreakWater =
                            {
                                Height = (RoundedDouble) 3.3,
                                Type = BreakWaterType.Dam
                            },
                            DikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>(),
                            OvertoppingRateCalculationType = random.NextEnumValue<OvertoppingRateCalculationType>(),
                            CriticalFlowRate =
                            {
                                Mean = (RoundedDouble) 4.4,
                                StandardDeviation = (RoundedDouble) 5.5
                            },
                            UseBreakWater = false,
                            UseForeshore = false,
                            ShouldDikeHeightIllustrationPointsBeCalculated = random.NextBoolean(),
                            ShouldOvertoppingOutputIllustrationPointsBeCalculated = random.NextBoolean(),
                            ShouldOvertoppingRateIllustrationPointsBeCalculated = random.NextBoolean()
                        },
                        Output = null
                    },
                    new GrassCoverErosionInwardsCalculationScenario
                    {
                        Name = "Calculation 2",
                        Comments =
                        {
                            Body = "Comment for Calculation 2"
                        },
                        InputParameters =
                        {
                            DikeProfile = failureMechanism.DikeProfiles.Last(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Last(),
                            DikeHeight = (RoundedDouble) 1.1,
                            Orientation = (RoundedDouble) 2.2,
                            BreakWater =
                            {
                                Height = (RoundedDouble) 3.3,
                                Type = BreakWaterType.Dam
                            },
                            DikeHeightCalculationType = random.NextEnumValue<DikeHeightCalculationType>(),
                            OvertoppingRateCalculationType = random.NextEnumValue<OvertoppingRateCalculationType>(),
                            CriticalFlowRate =
                            {
                                Mean = (RoundedDouble) 4.4,
                                StandardDeviation = (RoundedDouble) 5.5
                            },
                            UseBreakWater = false,
                            UseForeshore = false,
                            ShouldDikeHeightIllustrationPointsBeCalculated = random.NextBoolean(),
                            ShouldOvertoppingOutputIllustrationPointsBeCalculated = random.NextBoolean(),
                            ShouldOvertoppingRateIllustrationPointsBeCalculated = random.NextBoolean()
                        }
                    }
                }
            };
        }

        private static GrassCoverErosionInwardsFailureMechanism ConfigureFailureMechanism()
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

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

            failureMechanism.DikeProfiles.AddRange(new List<DikeProfile>
            {
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0), "profiel 1"),
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(5.0, 0.0), "profiel 2")
            }, string.Empty);

            return failureMechanism;
        }

        private GrassCoverErosionInwardsCalculationsView ShowCalculationsView(CalculationGroup calculationGroup, GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var calculationsView = new GrassCoverErosionInwardsCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            testForm.Controls.Add(calculationsView);
            testForm.Show();

            return calculationsView;
        }

        private static void AssertDataGridViewControlColumnHeaders(DataGridView dataGridView)
        {
            Assert.AreEqual(10, dataGridView.ColumnCount);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Hydraulische belastingenlocatie", dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex].HeaderText);
            Assert.AreEqual("Dijkprofiel", dataGridView.Columns[dikeProfileColumnIndex].HeaderText);
            Assert.AreEqual("Gebruik dam", dataGridView.Columns[useBreakWaterColumnIndex].HeaderText);
            Assert.AreEqual("Damtype", dataGridView.Columns[breakWaterTypeColumnIndex].HeaderText);
            Assert.AreEqual("Damhoogte [m+NAP]", dataGridView.Columns[breakWaterHeightColumnIndex].HeaderText);
            Assert.AreEqual("Gebruik voorlandgeometrie", dataGridView.Columns[useForeShoreGeometryColumnIndex].HeaderText);
            Assert.AreEqual("Dijkhoogte [m+NAP]", dataGridView.Columns[dikeHeightColumnIndex].HeaderText);
            Assert.AreEqual("Verwachtingswaarde kritiek overslagdebiet [m3/m/s]", dataGridView.Columns[meanCriticalFlowRateColumnIndex].HeaderText);
            Assert.AreEqual("Standaardafwijking kritiek overslagdebiet [m3/m/s]", dataGridView.Columns[standardDeviationCriticalFlowRateColumnIndex].HeaderText);
        }
    }
}