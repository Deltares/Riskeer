// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.GuiServices;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class WaveHeightCalculationsViewTest
    {
        private const int calculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int hydraulicBoundaryDatabaseFileNameColumnIndex = 4;
        private const int waveHeightColumnIndex = 5;

        private Form testForm;
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_GetTargetProbabilityFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            void Call() => new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                          assessmentSection,
                                                          null,
                                                          () => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getTargetProbabilityFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_GetCalculationIdentifierFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            void Call() => new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                          assessmentSection,
                                                          () => 0.01,
                                                          null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getCalculationIdentifierFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            using (var view = new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                             assessmentSection,
                                                             () => 0.01,
                                                             () => "1/100"))
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryCalculationsView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            ShowWaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                           assessmentSection,
                                           0.01,
                                           "1/100",
                                           testForm);

            // Assert
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            Assert.AreEqual(6, calculationsDataGridView.ColumnCount);

            var calculateColumn = (DataGridViewCheckBoxColumn) calculationsDataGridView.Columns[calculateColumnIndex];
            Assert.AreEqual("Berekenen", calculateColumn.HeaderText);
            Assert.IsTrue(calculateColumn.Visible);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) calculationsDataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);
            Assert.IsTrue(includeIllustrationPointsColumn.Visible);

            var locationNameColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);
            Assert.IsTrue(locationNameColumn.Visible);

            var locationIdColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);
            Assert.IsTrue(locationIdColumn.Visible);

            var hydraulicBoundaryDatabaseFileNameColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[hydraulicBoundaryDatabaseFileNameColumnIndex];
            Assert.AreEqual("HRD bestand", hydraulicBoundaryDatabaseFileNameColumn.HeaderText);
            Assert.IsFalse(hydraulicBoundaryDatabaseFileNameColumn.Visible);

            var waveHeightColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[waveHeightColumnIndex];
            Assert.AreEqual("Golfhoogte [m]", waveHeightColumn.HeaderText);
            Assert.IsTrue(waveHeightColumn.Visible);
        }

        [Test]
        public void Constructor_WithCalculations_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredWaveHeightCalculationsView(GetTestHydraulicBoundaryLocationCalculations(), testForm);

            // Assert
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            Assert.AreEqual(4, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database1", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waveHeightColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database1", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[waveHeightColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database2", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waveHeightColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database2", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[waveHeightColumnIndex].FormattedValue);
        }

        [Test]
        public void WaveHeightCalculationsView_CalculationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            ObservableList<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();
            AssessmentSectionStub assessmentSection = GetConfiguredAssessmentSectionStub(hydraulicBoundaryLocationCalculations);
            ShowWaveHeightCalculationsView(hydraulicBoundaryLocationCalculations, assessmentSection, testForm);

            var location = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0);
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases[0].Locations.Add(location);

            const double waveHeight = 10.23;
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(location)
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = new TestHydraulicBoundaryLocationCalculationOutput(waveHeight)
            };

            // Precondition
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            Assert.AreEqual(4, rows.Count);

            hydraulicBoundaryLocationCalculations.Clear();
            hydraulicBoundaryLocationCalculations.Add(hydraulicBoundaryLocationCalculation);

            // Call
            hydraulicBoundaryLocationCalculations.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database1", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);
            Assert.AreEqual(waveHeight, cells[waveHeightColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateWaveHeights()
        {
            // Setup
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            WaveHeightCalculationsView view = ShowFullyConfiguredWaveHeightCalculationsView(hydraulicBoundaryLocationCalculations, testForm);

            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[0]).ToArray();
                });
            mockRepository.ReplayAll();

            view.CalculationGuiService = guiService;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(hydraulicBoundaryLocationCalculations.First(), performedCalculations.First());
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredWaveHeightCalculationsView(GetTestHydraulicBoundaryLocationCalculations(), testForm);

            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            void Call() => button.Click();

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        public void CalculateForSelectedButton_Always_CalculateWaveHeightsCalledAsExpected()
        {
            // Setup
            const double targetProbability = 0.01;
            const string calculationIdentifier = "1/100";

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();
            AssessmentSectionStub assessmentSection = GetConfiguredAssessmentSectionStub(hydraulicBoundaryLocationCalculations);

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            IAssessmentSection assessmentSectionValue = null;
            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            double targetProbabilityValue = double.NaN;
            string calculationIdentifierValue = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[0]).ToArray();
                    assessmentSectionValue = (IAssessmentSection) invocation.Arguments[1];
                    targetProbabilityValue = (double) invocation.Arguments[2];
                    calculationIdentifierValue = (string) invocation.Arguments[3];
                });

            mockRepository.ReplayAll();

            WaveHeightCalculationsView view = ShowWaveHeightCalculationsView(hydraulicBoundaryLocationCalculations,
                                                                             assessmentSection,
                                                                             targetProbability,
                                                                             calculationIdentifier,
                                                                             testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            DataGridViewRowCollection rows = calculationsDataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.AreEqual(calculationIdentifier, calculationIdentifierValue);
            Assert.AreSame(assessmentSection, assessmentSectionValue);
            Assert.AreEqual(targetProbability, targetProbabilityValue);
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(hydraulicBoundaryLocationCalculations.First(), performedCalculations.First());
        }

        private DataGridView GetCalculationsDataGridView()
        {
            return ControlTestHelper.GetDataGridView(testForm, "DataGridView");
        }

        private DataGridViewControl GetCalculationsDataGridViewControl()
        {
            return ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
        }

        private IllustrationPointsControl GetIllustrationPointsControl()
        {
            return ControlTestHelper.GetControls<IllustrationPointsControl>(testForm, "IllustrationPointsControl").Single();
        }

        private static IEnumerable<IllustrationPointControlItem> CreateControlItems(
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            return generalResult.TopLevelIllustrationPoints
                                .Select(topLevelIllustrationPoint =>
                                {
                                    SubMechanismIllustrationPoint illustrationPoint = topLevelIllustrationPoint.SubMechanismIllustrationPoint;
                                    return new IllustrationPointControlItem(topLevelIllustrationPoint,
                                                                            topLevelIllustrationPoint.WindDirection.Name,
                                                                            topLevelIllustrationPoint.ClosingSituation,
                                                                            illustrationPoint.Stochasts,
                                                                            illustrationPoint.Beta);
                                });
        }

        private static WaveHeightCalculationsView ShowWaveHeightCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                                 IAssessmentSection assessmentSection,
                                                                                 double targetProbability,
                                                                                 string calculationIdentifier,
                                                                                 Form form)
        {
            var view = new WaveHeightCalculationsView(calculations,
                                                      assessmentSection,
                                                      () => targetProbability,
                                                      () => calculationIdentifier);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static WaveHeightCalculationsView ShowWaveHeightCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                                 IAssessmentSection assessmentSection,
                                                                                 Form form)
        {
            return ShowWaveHeightCalculationsView(calculations, assessmentSection, 0.01, "1/100", form);
        }

        private static WaveHeightCalculationsView ShowFullyConfiguredWaveHeightCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                                                Form form)
        {
            AssessmentSectionStub assessmentSection = GetConfiguredAssessmentSectionStub(calculations);
            return ShowWaveHeightCalculationsView(calculations, assessmentSection, form);
        }

        private static ObservableList<HydraulicBoundaryLocationCalculation> GetTestHydraulicBoundaryLocationCalculations()
        {
            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint()),
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Test",
                                                          new TestSubMechanismIllustrationPoint())
            };

            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);

            return new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0)),
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0))
                {
                    Output = new TestHydraulicBoundaryLocationCalculationOutput(1.23)
                },
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0))
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    }
                },
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(4, "4", 4.0, 4.0))
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationCalculationOutput(1.01, generalResult)
                }
            };
        }

        private static AssessmentSectionStub GetConfiguredAssessmentSectionStub(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase
            {
                FilePath = @"path\to\database1.sqlite"
            };
            hydraulicBoundaryDatabase1.Locations.AddRange(calculations.Take(2).Select(c => c.HydraulicBoundaryLocation));

            var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase
            {
                FilePath = @"path\to\database2.sqlite"
            };
            hydraulicBoundaryDatabase2.Locations.AddRange(calculations.Skip(2).Select(c => c.HydraulicBoundaryLocation));

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.AddRange(new[]
            {
                hydraulicBoundaryDatabase1,
                hydraulicBoundaryDatabase2
            });
            return assessmentSection;
        }

        [TestFixture]
        private class ViewSynchronizationTest : HydraulicBoundaryCalculationsViewSynchronizationTester
        {
            private ObservableList<HydraulicBoundaryLocationCalculation> calculations;

            protected override int OutputColumnIndex
            {
                get
                {
                    return waveHeightColumnIndex;
                }
            }

            public override void Setup()
            {
                calculations = GetTestHydraulicBoundaryLocationCalculations();

                base.Setup();
            }

            protected override object GetCalculationSelection(HydraulicBoundaryCalculationsView view, object selectedRowObject)
            {
                return new WaveHeightCalculationContext(((HydraulicBoundaryLocationCalculationRow) selectedRowObject).CalculatableObject);
            }

            protected override HydraulicBoundaryCalculationsView ShowFullyConfiguredCalculationsView(Form form)
            {
                return ShowFullyConfiguredWaveHeightCalculationsView(calculations, form);
            }

            protected override ObservableList<HydraulicBoundaryLocationCalculation> GetCalculationsInView(HydraulicBoundaryCalculationsView view)
            {
                return calculations;
            }
        }
    }
}