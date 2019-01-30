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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.Forms.TestUtil;
using Riskeer.Revetment.Forms.Views;

namespace Riskeer.Revetment.Forms.Test.Views
{
    [TestFixture]
    public class WaveConditionsInputViewTest
    {
        private const int numberOfChartDataLayers = 9;

        private const int foreShoreChartDataIndex = 0;
        private const int lowerBoundaryRevetmentChartDataIndex = 1;
        private const int upperBoundaryRevetmentChartDataIndex = 2;
        private const int lowerBoundaryWaterLevelsChartDataIndex = 3;
        private const int upperBoundaryWaterLevelsChartDataIndex = 4;
        private const int assessmentLevelChartDataIndex = 5;
        private const int waterLevelsChartDataIndex = 6;
        private const int revetmentBaseChartDataIndex = 7;
        private const int revetmentChartDataIndex = 8;

        [Test]
        public void Constructor_InputViewStyleNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsInputView(CreateTestCalculation(),
                                                                  GetHydraulicBoundaryLocationCalculation,
                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("inputViewStyle", exception.ParamName);
        }

        [Test]
        public void Constructor_GetHydraulicBoundaryLocationCalculationFuncNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsInputView(CreateTestCalculation(),
                                                                  null,
                                                                  new TestWaveConditionsInputViewStyle());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getHydraulicBoundaryLocationCalculationFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsInputView(null,
                                                                  GetHydraulicBoundaryLocationCalculation,
                                                                  new TestWaveConditionsInputViewStyle());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            TestWaveConditionsCalculation<TestWaveConditionsInput> calculation = CreateTestCalculation();
            using (var view = new WaveConditionsInputView(calculation,
                                                          GetHydraulicBoundaryLocationCalculation,
                                                          new TestWaveConditionsInputViewStyle()))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.AreSame(calculation, view.Data);
                Assert.AreEqual(1, view.Controls.Count);
            }
        }

        [Test]
        public void Constructor_WithValidParameters_ChartDataSet()
        {
            // Setup
            const string calculationName = "Calculation name";
            var assessmentLevel = (RoundedDouble) 6;
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput())
            {
                Name = calculationName,
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new[]
                    {
                        new Point2D(0.0, 0.0),
                        new Point2D(1.0, 1.0),
                        new Point2D(2.0, 2.0)
                    }),
                    LowerBoundaryRevetment = (RoundedDouble) 5,
                    UpperBoundaryRevetment = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 3,
                    UpperBoundaryWaterLevels = (RoundedDouble) 9
                }
            };

            // Call
            using (var view = new WaveConditionsInputView(calculation,
                                                          () => GetHydraulicBoundaryLocationCalculation(assessmentLevel),
                                                          new TestWaveConditionsInputViewStyle()))
            {
                // Assert
                IChartControl chartControl = view.Chart;
                Assert.IsInstanceOf<Control>(chartControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) chartControl).Dock);
                Assert.AreEqual("Afstand [m]", chartControl.BottomAxisTitle);
                Assert.AreEqual("Hoogte [m+NAP]", chartControl.LeftAxisTitle);

                Assert.AreEqual(calculationName, chartControl.ChartTitle);

                ChartDataCollection chartData = chartControl.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(numberOfChartDataLayers, chartData.Collection.Count());

                AssertForeshoreChartData(calculation.InputParameters.ForeshoreProfile, chartData.Collection.ElementAt(foreShoreChartDataIndex));

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryRevetment,
                                chartData.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex), "Ondergrens bekleding");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryRevetment,
                                chartData.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex), "Bovengrens bekleding");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryWaterLevels,
                                chartData.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex), "Ondergrens waterstanden");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryWaterLevels,
                                chartData.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex), "Bovengrens waterstanden");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, assessmentLevel,
                                chartData.Collection.ElementAt(assessmentLevelChartDataIndex), "Waterstand bij categoriegrens");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(assessmentLevel),
                                           chartData.Collection.ElementAt(waterLevelsChartDataIndex));

                AssertRevetmentBaseChartData(calculation.InputParameters.ForeshoreGeometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             calculation.InputParameters.LowerBoundaryWaterLevels,
                                             chartData.Collection.ElementAt(revetmentBaseChartDataIndex));
                AssertRevetmentChartData(calculation.InputParameters.ForeshoreGeometry.Last(),
                                         calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment,
                                         chartData.Collection.ElementAt(revetmentChartDataIndex));
            }
        }

        [Test]
        public void UpdateObserver_CalculationNameUpdated_ChartTitleUpdated()
        {
            // Setup
            const string initialName = "Initial name";
            const string updatedName = "Updated name";

            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput())
            {
                Name = initialName
            };

            using (var view = new WaveConditionsInputView(calculation,
                                                          GetHydraulicBoundaryLocationCalculation,
                                                          new TestWaveConditionsInputViewStyle()))
            {
                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                calculation.Name = updatedName;

                // Call
                calculation.NotifyObservers();

                // Assert
                Assert.AreEqual(updatedName, view.Chart.ChartTitle);
            }
        }

        [Test]
        public void UpdateObserver_ForeshoreProfileUpdated_ChartDataUpdatedAndObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChartDataLayers);
            mocks.ReplayAll();

            var assessmentLevel = (RoundedDouble) 6;
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput())
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new[]
                    {
                        new Point2D(0.0, 0.0),
                        new Point2D(1.0, 1.0),
                        new Point2D(2.0, 2.0)
                    }),
                    LowerBoundaryRevetment = (RoundedDouble) 5,
                    UpperBoundaryRevetment = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 3,
                    UpperBoundaryWaterLevels = (RoundedDouble) 7
                }
            };

            using (var view = new WaveConditionsInputView(calculation,
                                                          () => GetHydraulicBoundaryLocationCalculation(assessmentLevel),
                                                          new TestWaveConditionsInputViewStyle()))
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);
                var assessmentLevelChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex);
                var waterLevelsChartData = (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);

                foreshoreChartData.Attach(observer);
                lowerBoundaryRevetmentChartData.Attach(observer);
                upperBoundaryRevetmentChartData.Attach(observer);
                lowerBoundaryWaterLevelsChartData.Attach(observer);
                upperBoundaryWaterLevelsChartData.Attach(observer);
                assessmentLevelChartData.Attach(observer);
                waterLevelsChartData.Attach(observer);
                revetmentBaseChartData.Attach(observer);
                revetmentChartData.Attach(observer);

                ForeshoreProfile profile2 = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 3),
                    new Point2D(8, 8)
                });

                calculation.InputParameters.ForeshoreProfile = profile2;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                Assert.AreSame(foreshoreChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex));
                Assert.AreSame(lowerBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                Assert.AreSame(upperBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));
                Assert.AreSame(lowerBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(upperBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(assessmentLevelChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex));
                Assert.AreSame(waterLevelsChartData, (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex));
                Assert.AreSame(revetmentBaseChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex));
                Assert.AreSame(revetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex));

                AssertForeshoreChartData(profile2, foreshoreChartData);

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryRevetment,
                                lowerBoundaryRevetmentChartData, "Ondergrens bekleding");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryRevetment,
                                upperBoundaryRevetmentChartData, "Bovengrens bekleding");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryWaterLevels,
                                lowerBoundaryWaterLevelsChartData, "Ondergrens waterstanden");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryWaterLevels,
                                upperBoundaryWaterLevelsChartData, "Bovengrens waterstanden");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, assessmentLevel,
                                assessmentLevelChartData, "Waterstand bij categoriegrens");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(assessmentLevel),
                                           waterLevelsChartData);

                AssertRevetmentBaseChartData(profile2.Geometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             calculation.InputParameters.LowerBoundaryWaterLevels,
                                             revetmentBaseChartData);
                AssertRevetmentChartData(profile2.Geometry.Last(), calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment, revetmentChartData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithInputData_WhenHydraulicBoundaryLocationCalculationOutputSetAndNotified_ThenUpdatedDataIsShownInChart()
        {
            // Given
            var profile = new TestForeshoreProfile(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(2.0, 2.0)
            });
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    ForeshoreProfile = profile,
                    LowerBoundaryRevetment = (RoundedDouble) 5,
                    UpperBoundaryRevetment = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 3,
                    UpperBoundaryWaterLevels = (RoundedDouble) 7
                }
            };
            var assessmentLevel = (RoundedDouble) 6;
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = GetHydraulicBoundaryLocationCalculation(assessmentLevel);

            using (var view = new WaveConditionsInputView(calculation,
                                                          () => hydraulicBoundaryLocationCalculation,
                                                          new TestWaveConditionsInputViewStyle()))
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);
                var assessmentLevelChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex);
                var waterLevelsChartData = (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);

                // When
                assessmentLevel = (RoundedDouble) 8;
                hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(assessmentLevel);
                hydraulicBoundaryLocationCalculation.NotifyObservers();

                // Then
                Assert.AreSame(foreshoreChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex));
                Assert.AreSame(lowerBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                Assert.AreSame(upperBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));
                Assert.AreSame(lowerBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(upperBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(assessmentLevelChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex));
                Assert.AreSame(waterLevelsChartData, (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex));
                Assert.AreSame(revetmentBaseChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex));
                Assert.AreSame(revetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex));

                AssertForeshoreChartData(profile, foreshoreChartData);

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryRevetment,
                                lowerBoundaryRevetmentChartData, "Ondergrens bekleding");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryRevetment,
                                upperBoundaryRevetmentChartData, "Bovengrens bekleding");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryWaterLevels,
                                lowerBoundaryWaterLevelsChartData, "Ondergrens waterstanden");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryWaterLevels,
                                upperBoundaryWaterLevelsChartData, "Bovengrens waterstanden");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, assessmentLevel,
                                assessmentLevelChartData, "Waterstand bij categoriegrens");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(assessmentLevel),
                                           waterLevelsChartData);

                AssertRevetmentBaseChartData(profile.Geometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             calculation.InputParameters.LowerBoundaryWaterLevels,
                                             revetmentBaseChartData);
                AssertRevetmentChartData(profile.Geometry.Last(), calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment, revetmentChartData);
            }
        }

        [Test]
        public void GivenViewWithInputData_WhenHydraulicBoundaryLocationCalculationOutputClearedAndNotified_ThenUpdatedDataIsShownInChart()
        {
            // Given
            var profile = new TestForeshoreProfile(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(2.0, 2.0)
            });
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    ForeshoreProfile = profile,
                    LowerBoundaryRevetment = (RoundedDouble) 5,
                    UpperBoundaryRevetment = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 3,
                    UpperBoundaryWaterLevels = (RoundedDouble) 7
                }
            };
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = GetHydraulicBoundaryLocationCalculation(new Random(39).NextRoundedDouble());

            using (var view = new WaveConditionsInputView(calculation,
                                                          () => hydraulicBoundaryLocationCalculation,
                                                          new TestWaveConditionsInputViewStyle()))
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);
                var assessmentLevelChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex);
                var waterLevelsChartData = (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);

                // When
                hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput();
                hydraulicBoundaryLocationCalculation.NotifyObservers();

                // Then
                Assert.AreSame(foreshoreChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex));
                Assert.AreSame(lowerBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                Assert.AreSame(upperBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));
                Assert.AreSame(lowerBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(upperBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(assessmentLevelChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex));
                Assert.AreSame(waterLevelsChartData, (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex));
                Assert.AreSame(revetmentBaseChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex));
                Assert.AreSame(revetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex));

                AssertForeshoreChartData(profile, foreshoreChartData);

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryRevetment,
                                lowerBoundaryRevetmentChartData, "Ondergrens bekleding");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryRevetment,
                                upperBoundaryRevetmentChartData, "Bovengrens bekleding");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryWaterLevels,
                                lowerBoundaryWaterLevelsChartData, "Ondergrens waterstanden");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryWaterLevels,
                                upperBoundaryWaterLevelsChartData, "Bovengrens waterstanden");

                CollectionAssert.IsEmpty(assessmentLevelChartData.Points);
                Assert.AreEqual("Waterstand bij categoriegrens", assessmentLevelChartData.Name);
                CollectionAssert.IsEmpty(waterLevelsChartData.Lines);
                Assert.AreEqual("Waterstanden in berekening", waterLevelsChartData.Name);

                AssertRevetmentBaseChartData(profile.Geometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             calculation.InputParameters.LowerBoundaryWaterLevels,
                                             revetmentBaseChartData);
                AssertRevetmentChartData(profile.Geometry.Last(), calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment, revetmentChartData);
            }
        }

        [Test]
        public void GivenViewWithInputData_WhenCalculationInputNotified_ThenUpdatedDataIsShownInChart()
        {
            // Given
            var profile = new TestForeshoreProfile(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(2.0, 2.0)
            });
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    ForeshoreProfile = profile,
                    LowerBoundaryRevetment = (RoundedDouble) 5,
                    UpperBoundaryRevetment = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 3,
                    UpperBoundaryWaterLevels = (RoundedDouble) 7
                }
            };

            var assessmentLevel = (RoundedDouble) 6;
            using (var view = new WaveConditionsInputView(calculation,
                                                          () => GetHydraulicBoundaryLocationCalculation(assessmentLevel),
                                                          new TestWaveConditionsInputViewStyle()))
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);
                var assessmentLevelChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex);
                var waterLevelsChartData = (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);

                // When
                assessmentLevel = (RoundedDouble) 8;
                calculation.InputParameters.NotifyObservers();

                // Then
                Assert.AreSame(foreshoreChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex));
                Assert.AreSame(lowerBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                Assert.AreSame(upperBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));
                Assert.AreSame(lowerBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(upperBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(assessmentLevelChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex));
                Assert.AreSame(waterLevelsChartData, (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex));
                Assert.AreSame(revetmentBaseChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex));
                Assert.AreSame(revetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex));

                AssertForeshoreChartData(profile, foreshoreChartData);

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryRevetment,
                                lowerBoundaryRevetmentChartData, "Ondergrens bekleding");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryRevetment,
                                upperBoundaryRevetmentChartData, "Bovengrens bekleding");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryWaterLevels,
                                lowerBoundaryWaterLevelsChartData, "Ondergrens waterstanden");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryWaterLevels,
                                upperBoundaryWaterLevelsChartData, "Bovengrens waterstanden");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, assessmentLevel,
                                assessmentLevelChartData, "Waterstand bij categoriegrens");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(assessmentLevel),
                                           waterLevelsChartData);

                AssertRevetmentBaseChartData(profile.Geometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             calculation.InputParameters.LowerBoundaryWaterLevels,
                                             revetmentBaseChartData);
                AssertRevetmentChartData(profile.Geometry.Last(), calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment, revetmentChartData);
            }
        }

        [Test]
        public void GivenViewWithInputData_WhenHydraulicBoundaryLocationCalculationChangedAndCalculationNotified_ThenUpdatedDataIsShownInChart()
        {
            // Given
            var random = new Random(39);
            var profile = new TestForeshoreProfile(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(2.0, 2.0)
            });
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    ForeshoreProfile = profile,
                    LowerBoundaryRevetment = (RoundedDouble) 5,
                    UpperBoundaryRevetment = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 3,
                    UpperBoundaryWaterLevels = (RoundedDouble) 7
                }
            };

            HydraulicBoundaryLocationCalculation newHydraulicBoundaryLocationCalculation = GetHydraulicBoundaryLocationCalculation(random.NextRoundedDouble());
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = GetHydraulicBoundaryLocationCalculation(random.NextRoundedDouble());

            using (var view = new WaveConditionsInputView(calculation,
                                                          () => hydraulicBoundaryLocationCalculation,
                                                          new TestWaveConditionsInputViewStyle()))
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);
                var assessmentLevelChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex);
                var waterLevelsChartData = (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);

                // When
                hydraulicBoundaryLocationCalculation = newHydraulicBoundaryLocationCalculation;
                calculation.InputParameters.NotifyObservers();
                newHydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextRoundedDouble());
                newHydraulicBoundaryLocationCalculation.NotifyObservers();

                // Then
                Assert.AreSame(foreshoreChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex));
                Assert.AreSame(lowerBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                Assert.AreSame(upperBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));
                Assert.AreSame(lowerBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(upperBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(assessmentLevelChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(assessmentLevelChartDataIndex));
                Assert.AreSame(waterLevelsChartData, (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex));
                Assert.AreSame(revetmentBaseChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex));
                Assert.AreSame(revetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex));

                AssertForeshoreChartData(profile, foreshoreChartData);

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryRevetment,
                                lowerBoundaryRevetmentChartData, "Ondergrens bekleding");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryRevetment,
                                upperBoundaryRevetmentChartData, "Bovengrens bekleding");

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.LowerBoundaryWaterLevels,
                                lowerBoundaryWaterLevelsChartData, "Ondergrens waterstanden");
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, calculation.InputParameters.UpperBoundaryWaterLevels,
                                upperBoundaryWaterLevelsChartData, "Bovengrens waterstanden");

                RoundedDouble expectedAssessmentLevel = newHydraulicBoundaryLocationCalculation.Output.Result;
                AssertChartData(calculation.InputParameters.ForeshoreGeometry, expectedAssessmentLevel,
                                assessmentLevelChartData, "Waterstand bij categoriegrens");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(expectedAssessmentLevel),
                                           waterLevelsChartData);

                AssertRevetmentBaseChartData(profile.Geometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             calculation.InputParameters.LowerBoundaryWaterLevels,
                                             revetmentBaseChartData);
                AssertRevetmentChartData(profile.Geometry.Last(), calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment, revetmentChartData);
            }
        }

        private static HydraulicBoundaryLocationCalculation GetHydraulicBoundaryLocationCalculation()
        {
            return new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
        }

        private static HydraulicBoundaryLocationCalculation GetHydraulicBoundaryLocationCalculation(RoundedDouble assessmentLevel)
        {
            return new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(assessmentLevel)
            };
        }

        private static TestWaveConditionsCalculation<TestWaveConditionsInput> CreateTestCalculation()
        {
            return new TestWaveConditionsCalculation<TestWaveConditionsInput>(new TestWaveConditionsInput());
        }

        private static void AssertForeshoreChartData(ForeshoreProfile foreshoreProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var foreshoreChartData = (ChartLineData) chartData;

            RoundedPoint2DCollection foreshoreGeometry = foreshoreProfile.Geometry;
            string expectedName = $"{foreshoreProfile.Name} - Voorlandprofiel";
            Assert.AreEqual(expectedName, chartData.Name);
            CollectionAssert.AreEqual(foreshoreGeometry, foreshoreChartData.Points);
        }

        private static void AssertChartData(RoundedPoint2DCollection foreshorePoints,
                                            double value,
                                            ChartData chartData,
                                            string chartDataName)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var chartLineData = (ChartLineData) chartData;

            var expectedGeometry = new[]
            {
                new Point2D(foreshorePoints.First().X, value),
                new Point2D(GetPointX(value, foreshorePoints.Last()), value)
            };

            CollectionAssert.AreEqual(expectedGeometry, chartLineData.Points);

            Assert.AreEqual(chartDataName, chartLineData.Name);
        }

        private static void AssertWaterLevelsChartData(RoundedPoint2DCollection foreshorePoints,
                                                       IEnumerable<RoundedDouble> waterLevels,
                                                       ChartData chartData)
        {
            Assert.IsInstanceOf<ChartMultipleLineData>(chartData);
            var chartLineData = (ChartMultipleLineData) chartData;

            List<Point2D[]> expectedGeometry = waterLevels.Select(waterLevel => new[]
            {
                new Point2D(foreshorePoints.First().X, waterLevel),
                new Point2D(GetPointX(waterLevel, foreshorePoints.Last()), waterLevel)
            }).ToList();

            CollectionAssert.AreEqual(expectedGeometry, chartLineData.Lines);

            Assert.AreEqual("Waterstanden in berekening", chartLineData.Name);
        }

        private static void AssertRevetmentBaseChartData(Point2D lastForeshorePoint,
                                                         double lowerBoundaryRevetment,
                                                         double lowerBoundaryWaterLevels,
                                                         ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var revetmentChartData = (ChartLineData) chartData;

            var expectedGeometry = new List<Point2D>();

            if (lowerBoundaryWaterLevels < lastForeshorePoint.Y)
            {
                expectedGeometry.Add(new Point2D(GetPointX(lowerBoundaryWaterLevels, lastForeshorePoint), lowerBoundaryWaterLevels));
            }

            expectedGeometry.AddRange(new[]
            {
                new Point2D(lastForeshorePoint),
                new Point2D(GetPointX(lowerBoundaryRevetment, lastForeshorePoint), lowerBoundaryRevetment)
            });

            CollectionAssert.AreEqual(expectedGeometry, revetmentChartData.Points);

            Assert.AreEqual("Hulplijn bekleding", revetmentChartData.Name);
        }

        private static void AssertRevetmentChartData(Point2D lastForeshorePoint,
                                                     double lowerBoundaryRevetment,
                                                     double upperBoundaryRevetment,
                                                     ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var revetmentChartData = (ChartLineData) chartData;

            var expectedGeometry = new[]
            {
                new Point2D(GetPointX(lowerBoundaryRevetment, lastForeshorePoint), lowerBoundaryRevetment),
                new Point2D(GetPointX(upperBoundaryRevetment, lastForeshorePoint), upperBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, revetmentChartData.Points);

            Assert.AreEqual("Bekleding", revetmentChartData.Name);
        }

        private static double GetPointX(double pointY, Point2D lastForeshorePoint)
        {
            return ((pointY - lastForeshorePoint.Y) / 3) + lastForeshorePoint.X;
        }
    }
}