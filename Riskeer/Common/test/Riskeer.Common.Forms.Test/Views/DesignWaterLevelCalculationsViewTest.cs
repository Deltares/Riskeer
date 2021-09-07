// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    public class DesignWaterLevelCalculationsViewTest
    {
        private const int calculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int designWaterLevelColumnIndex = 5;

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
        public void Constructor_CalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            void Call() => new DesignWaterLevelCalculationsView(null,
                                                                assessmentSection,
                                                                () => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_GetCalculationIdentifierFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            void Call() => new DesignWaterLevelCalculationsView(new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01),
                                                                assessmentSection,
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
            using (var view = new DesignWaterLevelCalculationsView(new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01),
                                                                   assessmentSection,
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
            ShowDesignWaterLevelCalculationsView(new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01),
                                                 assessmentSection,
                                                 "1/100",
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

            var designWaterLevelColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[designWaterLevelColumnIndex];
            Assert.AreEqual("Waterstand [m+NAP]", designWaterLevelColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Constructor_WithCalculations_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredDesignWaterLevelCalculationsView(GetTestHydraulicBoundaryLocationCalculations(), testForm);

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
            Assert.AreEqual("-", cells[designWaterLevelColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[designWaterLevelColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[designWaterLevelColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[designWaterLevelColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelCalculationsView_CalculationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability = GetTestHydraulicBoundaryLocationCalculations();

            ShowFullyConfiguredDesignWaterLevelCalculationsView(targetProbability, testForm);

            const double designWaterLevel = 10.23;
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(10, "10", 10.0, 10.0))
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = new TestHydraulicBoundaryLocationCalculationOutput(designWaterLevel)
            };

            // Precondition
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            Assert.AreEqual(4, rows.Count);

            targetProbability.HydraulicBoundaryLocationCalculations.Clear();
            targetProbability.HydraulicBoundaryLocationCalculations.Add(hydraulicBoundaryLocationCalculation);

            // Call
            targetProbability.HydraulicBoundaryLocationCalculations.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(designWaterLevel, cells[designWaterLevelColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelCalculationsView_CalculationUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            ShowFullyConfiguredDesignWaterLevelCalculationsView(hydraulicBoundaryLocationCalculations, testForm);

            // Precondition
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);

            HydraulicBoundaryLocationCalculation calculation = hydraulicBoundaryLocationCalculations.HydraulicBoundaryLocationCalculations.First();

            // Call
            calculation.InputParameters.ShouldIllustrationPointsBeCalculated = true;
            calculation.NotifyObservers();

            // Assert
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelCalculationsView_CalculationUpdated_IllustrationPointsControlCorrectlyUpdated()
        {
            // Setup
            HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            ShowFullyConfiguredDesignWaterLevelCalculationsView(hydraulicBoundaryLocationCalculations, testForm);

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
            HydraulicBoundaryLocationCalculation boundaryLocationCalculation = hydraulicBoundaryLocationCalculations.HydraulicBoundaryLocationCalculations.ElementAt(2);
            boundaryLocationCalculation.Output = output;
            boundaryLocationCalculation.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevels()
        {
            // Setup
            HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            DesignWaterLevelCalculationsView view = ShowFullyConfiguredDesignWaterLevelCalculationsView(hydraulicBoundaryLocationCalculations, testForm);

            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation => { performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[0]).ToArray(); });
            mockRepository.ReplayAll();

            view.CalculationGuiService = guiService;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(hydraulicBoundaryLocationCalculations.HydraulicBoundaryLocationCalculations.First(), performedCalculations.First());
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredDesignWaterLevelCalculationsView(GetTestHydraulicBoundaryLocationCalculations(), testForm);

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
        public void CalculateForSelectedButton_Always_CalculateDesignWaterLevelsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";
            const string calculationIdentifier = "1/100";

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
            string calculationIdentifierValue = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[0]).ToArray();
                    assessmentSectionValue = (IAssessmentSection) invocation.Arguments[1];
                    normValue = (double) invocation.Arguments[2];
                    calculationIdentifierValue = (string) invocation.Arguments[3];
                });

            mockRepository.ReplayAll();

            HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculations = GetTestHydraulicBoundaryLocationCalculations();

            DesignWaterLevelCalculationsView view = ShowDesignWaterLevelCalculationsView(hydraulicBoundaryLocationCalculations,
                                                                                         assessmentSection,
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
            Assert.AreEqual(hydraulicBoundaryLocationCalculations.TargetProbability, normValue);
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(hydraulicBoundaryLocationCalculations.HydraulicBoundaryLocationCalculations.First(), performedCalculations.First());
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

        private static DesignWaterLevelCalculationsView ShowDesignWaterLevelCalculationsView(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                                                             IAssessmentSection assessmentSection,
                                                                                             string calculationIdentifier,
                                                                                             Form form)
        {
            var view = new DesignWaterLevelCalculationsView(calculationsForTargetProbability,
                                                            assessmentSection,
                                                            () => calculationIdentifier);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static DesignWaterLevelCalculationsView ShowFullyConfiguredDesignWaterLevelCalculationsView(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                                                                            Form form)
        {
            var assessmentSection = new AssessmentSectionStub();

            return ShowDesignWaterLevelCalculationsView(calculationsForTargetProbability, assessmentSection, "1/100", form);
        }

        private static HydraulicBoundaryLocationCalculationsForTargetProbability GetTestHydraulicBoundaryLocationCalculations()
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

            var targetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            {
                HydraulicBoundaryLocationCalculations =
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
                }
            };
            return targetProbability;
        }

        [TestFixture]
        private class ViewSynchronizationTest : CalculationsViewSynchronizationTester<HydraulicBoundaryLocationCalculation>
        {
            private HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability;

            protected override int OutputColumnIndex
            {
                get
                {
                    return designWaterLevelColumnIndex;
                }
            }

            public override void Setup()
            {
                targetProbability = GetTestHydraulicBoundaryLocationCalculations();

                base.Setup();
            }

            protected override object GetCalculationSelection(LocationCalculationsView<HydraulicBoundaryLocationCalculation> view, object selectedRowObject)
            {
                return new DesignWaterLevelCalculationContext(((HydraulicBoundaryLocationCalculationRow) selectedRowObject).CalculatableObject);
            }

            protected override LocationCalculationsView<HydraulicBoundaryLocationCalculation> ShowFullyConfiguredCalculationsView(Form form)
            {
                return ShowFullyConfiguredDesignWaterLevelCalculationsView(targetProbability, form);
            }

            protected override ObservableList<HydraulicBoundaryLocationCalculation> GetCalculationsInView(LocationCalculationsView<HydraulicBoundaryLocationCalculation> view)
            {
                return targetProbability.HydraulicBoundaryLocationCalculations;
            }
        }
    }
}