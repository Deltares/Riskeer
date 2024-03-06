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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryCalculationsViewTest
    {
        private const int calculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
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
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "dataGridView");
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GetIllustrationPointControlItems_ViewWithData_ReturnsExpectedControlItems()
        {
            // Setup
            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };

            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationCalculationOutput(generalResult);

            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = output
            };
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                calculation
            };

            TestHydraulicBoundaryCalculationsView view = ShowTestHydraulicBoundaryCalculationsView(calculations);

            // Call
            IEnumerable<IllustrationPointControlItem> actualControlItems =
                view.PublicGetIllustrationPointControlItems();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems =
                CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, actualControlItems,
                                      new IllustrationPointControlItemComparer());
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

        private void ShowTestHydraulicBoundaryCalculationsView()
        {
            ShowTestHydraulicBoundaryCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>());
        }

        private TestHydraulicBoundaryCalculationsView ShowTestHydraulicBoundaryCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            var view = new TestHydraulicBoundaryCalculationsView(calculations, new AssessmentSectionStub());

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private TestHydraulicBoundaryCalculationsView ShowFullyConfiguredTestHydraulicBoundaryCalculationsView()
        {
            return ShowTestHydraulicBoundaryCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>
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
                }
            });
        }

        private sealed class TestHydraulicBoundaryCalculationsView : HydraulicBoundaryCalculationsView
        {
            public TestHydraulicBoundaryCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                         IAssessmentSection assessmentSection)
                : base(calculations, assessmentSection) {}

            public IEnumerable<IllustrationPointControlItem> PublicGetIllustrationPointControlItems()
            {
                // return GetIllustrationPointControlItems(); 
                return null;
            }

            protected override void PerformSelectedCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
            {
                throw new NotImplementedException();
            }

            protected override object CreateSelectedItemFromCurrentRow()
            {
                return null;
            }
        }
    }
}