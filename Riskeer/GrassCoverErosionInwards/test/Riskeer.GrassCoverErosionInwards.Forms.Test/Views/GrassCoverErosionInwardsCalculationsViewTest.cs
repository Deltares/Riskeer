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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
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
        public void Constructor_ExpectedValues()
        {
            // Call
            GrassCoverErosionInwardsCalculationsView view = ShowCalculationsView(new CalculationGroup(), new GrassCoverErosionInwardsFailureMechanism(), new AssessmentSectionStub());

            // Assert
            Assert.IsInstanceOf<CalculationsView<GrassCoverErosionInwardsCalculationScenario, GrassCoverErosionInwardsInput, GrassCoverErosionInwardsCalculationRow, GrassCoverErosionInwardsFailureMechanism>>(view);

            var button = (Button) new ControlTester("generateButton").TheObject;
            Assert.AreEqual("Genereer &berekeningen...", button.Text);

            var label = (Label) new ControlTester("warningText").TheObject;
            Assert.AreEqual("Als u het dijkprofiel van een berekening wijzigt kan de berekening in een ander vak komen te liggen.", label.Text);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);

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

            Assert.AreEqual(10, dataGridView.ColumnCount);

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
        public void CalculationsView_FailureMechanismWithDikeProfiles_DikeProfilesComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var dikeProfileComboBox = (DataGridViewComboBoxColumn) dataGridView.Columns[dikeProfileColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection dikeProfileComboBoxItems = dikeProfileComboBox.Items;
            Assert.AreEqual(3, dikeProfileComboBoxItems.Count);
            Assert.AreEqual("<selecteer>", dikeProfileComboBoxItems[0].ToString());
            Assert.AreEqual("Profiel 1", dikeProfileComboBoxItems[1].ToString());
            Assert.AreEqual("Profiel 2", dikeProfileComboBoxItems[2].ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_Always_BreakWaterTypeComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var breakWaterTypeComboBox = (DataGridViewComboBoxColumn) dataGridView.Columns[breakWaterTypeColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection breakWaterTypeComboBoxItems = breakWaterTypeComboBox.Items;
            Assert.AreEqual(3, breakWaterTypeComboBoxItems.Count);
            Assert.AreEqual("Muur", breakWaterTypeComboBoxItems[0].ToString());
            Assert.AreEqual("Caisson", breakWaterTypeComboBoxItems[1].ToString());
            Assert.AreEqual("Havendam", breakWaterTypeComboBoxItems[2].ToString());
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

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(10, cells.Count);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("Profiel 1", cells[dikeProfileColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useBreakWaterColumnIndex].FormattedValue);
            Assert.AreEqual("Havendam", cells[breakWaterTypeColumnIndex].FormattedValue);
            Assert.AreEqual(3.30.ToString("0.00", CultureInfo.CurrentCulture), cells[breakWaterHeightColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useForeShoreGeometryColumnIndex].FormattedValue);
            Assert.AreEqual(1.10.ToString("0.00", CultureInfo.CurrentCulture), cells[dikeHeightColumnIndex].FormattedValue);
            Assert.AreEqual(4.4000.ToString("0.0000", CultureInfo.CurrentCulture), cells[meanCriticalFlowRateColumnIndex].FormattedValue);
            Assert.AreEqual(5.5000.ToString("0.0000", CultureInfo.CurrentCulture), cells[standardDeviationCriticalFlowRateColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(10, cells.Count);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 2 (5 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("Profiel 2", cells[dikeProfileColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useBreakWaterColumnIndex].FormattedValue);
            Assert.AreEqual("Havendam", cells[breakWaterTypeColumnIndex].FormattedValue);
            Assert.AreEqual(3.30.ToString("0.00", CultureInfo.CurrentCulture), cells[breakWaterHeightColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[useForeShoreGeometryColumnIndex].FormattedValue);
            Assert.AreEqual(1.10.ToString("0.00", CultureInfo.CurrentCulture), cells[dikeHeightColumnIndex].FormattedValue);
            Assert.AreEqual(4.4000.ToString("0.0000", CultureInfo.CurrentCulture), cells[meanCriticalFlowRateColumnIndex].FormattedValue);
            Assert.AreEqual(5.5000.ToString("0.0000", CultureInfo.CurrentCulture), cells[standardDeviationCriticalFlowRateColumnIndex].FormattedValue);
            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateCalculations_DikeProfilesPresent_ButtonEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var button = (Button) new ControlTester("generateButton").TheObject;

            // Call
            bool state = button.Enabled;

            // Assert
            Assert.IsTrue(state);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_ChangingDikeProfiles_ButtonCorrectState()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            // Precondition
            var button = (Button) new ControlTester("generateButton").TheObject;
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

            failureMechanism.DikeProfiles.AddRange(new List<DikeProfile>
            {
                DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0), "profiel 1")
            }, string.Empty);

            // Call
            failureMechanism.DikeProfiles.NotifyObservers();

            // Assert
            Assert.IsTrue(button.Enabled);
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
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

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
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            mocks.ReplayAll();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
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
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            // Call
            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children.First();

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(inputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[meanCriticalFlowRateColumnIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual("Gemiddelde moet groter zijn dan 0.", dataGridView.Rows[0].ErrorText);
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
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children.First();

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(inputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[standardDeviationCriticalFlowRateColumnIndex].Value = (RoundedDouble) (-123.45);

            // Assert
            Assert.AreEqual("Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.", dataGridView.Rows[0].ErrorText);
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
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            mocks.ReplayAll();

            var calculationScenario = (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[1];

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
        public void GivenCalculationsView_WhenDikeProfilesUpdatedAndNotified_ThenDikeProfilesComboboxCorrectlyUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            ShowCalculationsView(ConfigureCalculationGroup(failureMechanism, assessmentSection), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var dikeProfilesComboBox = (DataGridViewComboBoxColumn) dataGridView.Columns[dikeProfileColumnIndex];

            // Precondition
            Assert.AreEqual(3, dikeProfilesComboBox.Items.Count);

            // When
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile("3", "Profiel 3", new Point2D(0.0, 0.0)),
                DikeProfileTestFactory.CreateDikeProfile("4", "Profiel 4", new Point2D(5.0, 0.0))
            }, string.Empty);
            failureMechanism.DikeProfiles.NotifyObservers();

            // Then
            DataGridViewComboBoxCell.ObjectCollection dikeProfileItems = dikeProfilesComboBox.Items;
            Assert.AreEqual(5, dikeProfileItems.Count);
            Assert.AreEqual("<selecteer>", dikeProfileItems[0].ToString());
            Assert.AreEqual("Profiel 1", dikeProfileItems[1].ToString());
            Assert.AreEqual("Profiel 2", dikeProfileItems[2].ToString());
            Assert.AreEqual("Profiel 3", dikeProfileItems[3].ToString());
            Assert.AreEqual("Profiel 4", dikeProfileItems[4].ToString());

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

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            // This step is necessary because setting the same value would not change the view state.
            var calculation = (GrassCoverErosionInwardsCalculationScenario) calculationGroup.GetCalculations().First();
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

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(failureMechanism, assessmentSection);

            GrassCoverErosionInwardsCalculationsView view = ShowCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[0];

            // Call
            object selection = view.Selection;

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsInputContext>(selection);
            var dataRow = (GrassCoverErosionInwardsCalculationRow) dataGridView.Rows[selectedRow].DataBoundItem;
            Assert.AreSame(dataRow.Calculation, ((GrassCoverErosionInwardsInputContext) selection).Calculation);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsView_WhenGenerateScenariosButtonClicked_ThenShowViewWithDikeProfiles()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();
        
            const string arbitraryFilePath = "path";
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile("1", "Profiel 1", new Point2D(0.0, 0.0)),
                DikeProfileTestFactory.CreateDikeProfile("2", "Profiel 2", new Point2D(5.0, 0.0))
            }, arbitraryFilePath);

            ShowCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);
        
            var button = new ButtonTester("generateButton", testForm);

            GrassCoverErosionInwardsDikeProfileSelectionDialog selectionDialog = null;
            DataGridViewControl grid = null;
            var rows = 0;
            DialogBoxHandler = (name, wnd) =>
            {
                selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
                grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                rows = grid.Rows.Count;

                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };
        
            // When
            button.Click();
        
            // Then
            Assert.NotNull(selectionDialog);
            Assert.NotNull(grid);
            Assert.AreEqual(failureMechanism.DikeProfiles.Count, rows);
            mocks.VerifyAll();
        }
        
        [Test]
        public void GivenCalculationsViewGenerateCalculationsButtonClicked_WhenDialogClosed_ThenNotifyCalculationGroup()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();
        
            const string arbitraryFilePath = "path";
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile("1", "Profiel 1", new Point2D(0.0, 0.0)),
                DikeProfileTestFactory.CreateDikeProfile("2", "Profiel 2", new Point2D(5.0, 0.0))
            }, arbitraryFilePath);
            failureMechanism.CalculationsGroup.Attach(observer);
        
            ShowCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);
        
            var button = new ButtonTester("generateButton", testForm);
        
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog)new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl)new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;
        
                // When
                new ButtonTester("DoForSelectedButton", selectionDialog).Click();
            };
        
            button.Click();
        
            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsViewGenerateCalculationsButtonClicked_WhenDikeProfileSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            ShowCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl)new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                new ButtonTester("DoForSelectedButton", selectionDialog).Click();
            };

            button.Click();

            // Then
            GrassCoverErosionInwardsCalculationScenario[] calculationScenarios = failureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculationScenario>().ToArray();
            GrassCoverErosionInwardsFailureMechanismSectionResult failureMechanismSectionResult1 = failureMechanism.SectionResults.First();
            GrassCoverErosionInwardsFailureMechanismSectionResult failureMechanismSectionResult2 = failureMechanism.SectionResults.ElementAt(1);

            Assert.AreEqual(1, failureMechanismSectionResult1.GetCalculationScenarios(calculationScenarios).Count());
            CollectionAssert.IsEmpty(failureMechanismSectionResult2.GetCalculationScenarios(calculationScenarios));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsViewGenerateCalculationsCancelButtonClicked_WhenDialogClosed_CalculationsNotUpdatedAndCalculationGroupNotNotified()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            ShowCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            failureMechanism.CalculationsGroup.Attach(observer);

            var button = new ButtonTester("generateButton", testForm);
        
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog)new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl)new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;
        
                // When
                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };
        
            button.Click();
        
            // Then
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            mocks.VerifyAll(); // No observer notified
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
                            DikeProfile = failureMechanism.DikeProfiles.FirstOrDefault(),
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
                            DikeProfile = failureMechanism.DikeProfiles.LastOrDefault(),
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
                DikeProfileTestFactory.CreateDikeProfile("1", "Profiel 1", new Point2D(0.0, 0.0)),
                DikeProfileTestFactory.CreateDikeProfile("2", "Profiel 2", new Point2D(5.0, 0.0))
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
    }
}