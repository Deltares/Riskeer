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
using Core.Common.Base.Geometry;
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
        private const int locationColumnIndex = 4;
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
        public void Constructor_GetNormFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                     assessmentSection,
                                                                     null,
                                                                     "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getNormFunc", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_CategoryBoundaryNameInvalid_ThrowsArgumentException(string categoryBoundaryName)
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                     assessmentSection,
                                                                     () => 0.01,
                                                                     categoryBoundaryName);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("'categoryBoundaryName' must have a value.", exception.Message);
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
                                                             "A"))
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
                                           "A",
                                           testForm);

            // Assert
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            Assert.AreEqual(6, calculationsDataGridView.ColumnCount);

            var calculateColumn = (DataGridViewCheckBoxColumn) calculationsDataGridView.Columns[calculateColumnIndex];
            Assert.AreEqual("Berekenen", calculateColumn.HeaderText);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) calculationsDataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var waveHeightColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[waveHeightColumnIndex];
            Assert.AreEqual("Golfhoogte [m]", waveHeightColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
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
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waveHeightColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[waveHeightColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waveHeightColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[waveHeightColumnIndex].FormattedValue);
        }

        [Test]
        public void WaveHeightCalculationsView_CalculationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            ObservableList<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            ShowFullyConfiguredWaveHeightCalculationsView(hydraulicBoundaryLocationCalculations, testForm);

            const double waveHeight = 10.23;
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(10, "10", 10.0, 10.0))
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
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(waveHeight, cells[waveHeightColumnIndex].Value);
        }

        [Test]
        public void WaveHeightCalculationsView_CalculationUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            ShowFullyConfiguredWaveHeightCalculationsView(hydraulicBoundaryLocationCalculations, testForm);

            // Precondition
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);

            HydraulicBoundaryLocationCalculation calculation = hydraulicBoundaryLocationCalculations.First();

            // Call
            calculation.InputParameters.ShouldIllustrationPointsBeCalculated = true;
            calculation.NotifyObservers();

            // Assert
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
        }

        [Test]
        public void WaveHeightCalculationsView_CalculationUpdated_IllustrationPointsControlCorrectlyUpdated()
        {
            // Setup
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            ShowFullyConfiguredWaveHeightCalculationsView(hydraulicBoundaryLocationCalculations, testForm);

            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();

            calculationsDataGridViewControl.SetCurrentCell(calculationsDataGridViewControl.GetCell(2, 0));

            // Precondition
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationCalculationOutput(generalResult);

            // Call
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = hydraulicBoundaryLocationCalculations.ElementAt(2);
            hydraulicBoundaryLocationCalculation.Output = output;
            hydraulicBoundaryLocationCalculation.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
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
                invocation => { performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[0]).ToArray(); });
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
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void CalculateForSelectedButton_Always_CalculateWaveHeightsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";
            const double norm = 0.01;
            const string categoryBoundaryName = "A";

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = databaseFilePath
            };
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            IAssessmentSection assessmentSectionValue = null;
            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            double normValue = double.NaN;
            string categoryBoundaryNameValue = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[0]).ToArray();
                    assessmentSectionValue = (IAssessmentSection) invocation.Arguments[1];
                    normValue = (double) invocation.Arguments[2];
                    categoryBoundaryNameValue = (string) invocation.Arguments[3];
                });

            mockRepository.ReplayAll();

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            WaveHeightCalculationsView view = ShowWaveHeightCalculationsView(hydraulicBoundaryLocationCalculations,
                                                                             assessmentSection,
                                                                             norm,
                                                                             categoryBoundaryName,
                                                                             testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            DataGridViewRowCollection rows = calculationsDataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.AreEqual(categoryBoundaryName, categoryBoundaryNameValue);
            Assert.AreSame(assessmentSection, assessmentSectionValue);
            Assert.AreEqual(norm, normValue);
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
                                                                                 double norm,
                                                                                 string categoryBoundaryName,
                                                                                 Form form)
        {
            var view = new WaveHeightCalculationsView(calculations,
                                                      assessmentSection,
                                                      () => norm,
                                                      categoryBoundaryName);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static WaveHeightCalculationsView ShowFullyConfiguredWaveHeightCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                                                Form form)
        {
            var assessmentSection = new AssessmentSectionStub();

            return ShowWaveHeightCalculationsView(calculations, assessmentSection, 0.01, "A", form);
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

        [TestFixture]
        private class ViewSynchronizationTest : CalculationsViewSynchronizationTester<HydraulicBoundaryLocationCalculation>
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

            protected override object GetCalculationSelection(CalculationsView<HydraulicBoundaryLocationCalculation> view, object selectedRowObject)
            {
                return new WaveHeightCalculationContext(((HydraulicBoundaryLocationCalculationRow) selectedRowObject).CalculatableObject);
            }

            protected override CalculationsView<HydraulicBoundaryLocationCalculation> ShowFullyConfiguredCalculationsView(Form form)
            {
                return ShowFullyConfiguredWaveHeightCalculationsView(calculations, form);
            }

            protected override ObservableList<HydraulicBoundaryLocationCalculation> GetCalculationsInView(CalculationsView<HydraulicBoundaryLocationCalculation> view)
            {
                return calculations;
            }
        }
    }
}