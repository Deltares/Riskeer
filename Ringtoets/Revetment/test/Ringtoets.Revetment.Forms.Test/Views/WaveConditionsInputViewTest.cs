﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.TestUtil;
using Ringtoets.Revetment.Forms.Views;

namespace Ringtoets.Revetment.Forms.Test.Views
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
        private const int designWaterLevelChartDataIndex = 5;
        private const int waterLevelsChartDataIndex = 6;
        private const int revetmentBaseChartDataIndex = 7;
        private const int revetmentChartDataIndex = 8;

        private static IEnumerable<TestCaseData> NotifyChange
        {
            get
            {
                yield return new TestCaseData(new Action<WaveConditionsInput>(wci => { wci.HydraulicBoundaryLocation.NotifyObservers(); })).SetName("NotifyWaterLevelChange");

                yield return new TestCaseData(new Action<WaveConditionsInput>(wci =>
                {
                    var newLocation = new TestHydraulicBoundaryLocation();
                    wci.HydraulicBoundaryLocation = newLocation;
                    wci.NotifyObservers();
                    newLocation.NotifyObservers();
                })).SetName("NotifyLocationAndWaterLevelChange");

                yield return new TestCaseData(new Action<WaveConditionsInput>(wci =>
                {
                    var newLocation = new TestHydraulicBoundaryLocation();
                    wci.HydraulicBoundaryLocation = newLocation;
                    wci.NotifyObservers();
                })).SetName("NotifyLocationChange");
            }
        }

        [Test]
        public void Constructor_InputViewStyleNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsInputView(null, GetTestNormativeAssessmentLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("inputViewStyle", exception.ParamName);
        }

        [Test]
        public void Constructor_GetNormativeAssessmentLevelFuncNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
            }
        }

        [Test]
        public void Constructor_Always_AddEmptyChartControl()
        {
            // Call
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel))
            {
                // Assert
                var chartControl = (IChartControl) view.Controls.Find("chartControl", true).First();
                Assert.IsInstanceOf<Control>(chartControl);
                Assert.AreSame(chartControl, chartControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) chartControl).Dock);
                Assert.AreEqual("Afstand [m]", chartControl.BottomAxisTitle);
                Assert.AreEqual("Hoogte [m+NAP]", chartControl.LeftAxisTitle);
                Assert.IsNull(chartControl.Data);
            }
        }

        [Test]
        public void Data_IWaveConditionsCalculation_DataSet()
        {
            // Setup
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel))
            {
                var calculation = new TestWaveConditionsCalculation();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanWaveConditionsCalculation_DataNull()
        {
            // Setup
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel))
            {
                var calculation = new object();

                // Call
                view.Data = calculation;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_SetToNull_ChartDataCleared()
        {
            // Setup
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel)
            {
                Data = new TestWaveConditionsCalculation()
            })
            {
                // Precondition
                Assert.AreEqual(numberOfChartDataLayers, view.Chart.Data.Collection.Count());
                Assert.AreEqual("Nieuwe berekening", view.Chart.ChartTitle);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Chart.Data);
                Assert.IsEmpty(view.Chart.ChartTitle);
            }
        }

        [Test]
        public void Data_SetChartData_ChartDataSet()
        {
            // Setup
            const string calculationName = "Calculation name";
            var normativeAssessmentLevel = (RoundedDouble) 6;

            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), () => normativeAssessmentLevel))
            {
                var calculation = new TestWaveConditionsCalculation
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
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.AreEqual(calculationName, view.Chart.ChartTitle);

                ChartDataCollection chartData = view.Chart.Data;
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

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, normativeAssessmentLevel,
                                chartData.Collection.ElementAt(designWaterLevelChartDataIndex), "Toetspeil");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(normativeAssessmentLevel),
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
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel))
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation = new TestWaveConditionsCalculation
                {
                    Name = initialName
                };

                view.Data = calculation;

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
        public void UpdateObserver_PreviousCalculationNameUpdated_ChartTitleNotUpdated()
        {
            // Setup
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel))
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation = new TestWaveConditionsCalculation
                {
                    Name = initialName
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                view.Data = new TestWaveConditionsCalculation
                {
                    Name = initialName
                };

                calculation.Name = updatedName;

                // Call
                calculation.NotifyObservers();

                // Assert
                Assert.AreEqual(initialName, view.Chart.ChartTitle);
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

            var normativeAssessmentLevel = (RoundedDouble) 6;
            var calculation = new TestWaveConditionsCalculation
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

            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), () => normativeAssessmentLevel)
            {
                Data = calculation
            })
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);
                var designWaterLevelChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(designWaterLevelChartDataIndex);
                var waterLevelsChartData = (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);

                foreshoreChartData.Attach(observer);
                lowerBoundaryRevetmentChartData.Attach(observer);
                upperBoundaryRevetmentChartData.Attach(observer);
                lowerBoundaryWaterLevelsChartData.Attach(observer);
                upperBoundaryWaterLevelsChartData.Attach(observer);
                designWaterLevelChartData.Attach(observer);
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
                Assert.AreSame(designWaterLevelChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(designWaterLevelChartDataIndex));
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

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, normativeAssessmentLevel,
                                designWaterLevelChartData, "Toetspeil");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(normativeAssessmentLevel),
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
        public void UpdateObserver_PreviousCalculationUpdated_ChartDataNotUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var calculation = new TestWaveConditionsCalculation();

            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), GetTestNormativeAssessmentLevel)
            {
                Data = calculation
            })
            {
                ((ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex)).Attach(observer);

                view.Data = new TestWaveConditionsCalculation();

                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 3),
                    new Point2D(8, 8)
                });

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                mocks.VerifyAll(); // No update observer expected
            }
        }

        [Test]
        [TestCaseSource(nameof(NotifyChange))]
        public void GivenViewWithInputData_WhenChangeNotified_ThenUpdatedDataIsShownInChart(Action<WaveConditionsInput> notifyChange)
        {
            // Given
            var profile = new TestForeshoreProfile(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(2.0, 2.0)
            });
            var calculation = new TestWaveConditionsCalculation
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

            var normativeAssessmentLevel = (RoundedDouble) 6;
            Func<RoundedDouble> getNormativeAssessmentLevel = () => normativeAssessmentLevel;
            using (var view = new WaveConditionsInputView(new TestWaveConditionsInputViewStyle(), getNormativeAssessmentLevel)
            {
                Data = calculation
            })
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);
                var designWaterLevelChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(designWaterLevelChartDataIndex);
                var waterLevelsChartData = (ChartMultipleLineData) view.Chart.Data.Collection.ElementAt(waterLevelsChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);

                // When
                normativeAssessmentLevel = (RoundedDouble) 8;
                notifyChange(calculation.InputParameters);

                // Then
                Assert.AreSame(foreshoreChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex));
                Assert.AreSame(lowerBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                Assert.AreSame(upperBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));
                Assert.AreSame(lowerBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(upperBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(designWaterLevelChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(designWaterLevelChartDataIndex));
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

                AssertChartData(calculation.InputParameters.ForeshoreGeometry, normativeAssessmentLevel,
                                designWaterLevelChartData, "Toetspeil");

                AssertWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                           calculation.InputParameters.GetWaterLevels(normativeAssessmentLevel),
                                           waterLevelsChartData);

                AssertRevetmentBaseChartData(profile.Geometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             calculation.InputParameters.LowerBoundaryWaterLevels,
                                             revetmentBaseChartData);
                AssertRevetmentChartData(profile.Geometry.Last(), calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment, revetmentChartData);
            }
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

        private static RoundedDouble GetTestNormativeAssessmentLevel()
        {
            return (RoundedDouble) 1.1;
        }
    }
}