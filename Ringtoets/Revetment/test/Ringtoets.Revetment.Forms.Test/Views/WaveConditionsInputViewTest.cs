// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Forms.Views;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.Revetment.Forms.Test.Views
{
    [TestFixture]
    public class WaveConditionsInputViewTest
    {
        private const int foreShoreChartDataIndex = 0;
        private const int lowerBoundaryRevetmentChartDataIndex = 1;
        private const int upperBoundaryRevetmentChartDataIndex = 2;
        private const int revetmentChartDataIndex = 3;
        private const int revetmentBaseChartDataIndex = 4;
        private const int lowerBoundaryWaterLevelsChartDataIndex = 5;
        private const int upperBoundaryWaterLevelsChartDataIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new WaveConditionsInputView())
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
            using (var view = new WaveConditionsInputView())
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
            using (var view = new WaveConditionsInputView())
            {
                var calculation = new TestWaveConditionsCalculation();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanIWwaveCondittionsCalculation_DataNull()
        {
            // Setup
            using (var view = new WaveConditionsInputView())
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
            using (var view = new WaveConditionsInputView
            {
                Data = new TestWaveConditionsCalculation()
            })
            {
                // Precondition
                Assert.AreEqual(7, view.Chart.Data.Collection.Count());
                Assert.AreEqual("Nieuwe berekening", view.Chart.ChartTitle);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Chart.Data);
                Assert.AreEqual(string.Empty, view.Chart.ChartTitle);
            }
        }

        [Test]
        public void Data_EmptyCalculation_NoChartDataSet()
        {
            // Setup
            using (var view = new WaveConditionsInputView())
            {
                var calculation = new TestWaveConditionsCalculation();

                // Call
                view.Data = calculation;

                // Assert
                AssertEmptyChartData(view.Chart.Data);
            }
        }

        [Test]
        public void Data_SetChartData_ChartDataSet()
        {
            // Setup
            const string calculationName = "Calculation name";

            using (var view = new WaveConditionsInputView())
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
                Assert.AreEqual(7, chartData.Collection.Count());

                AssertForeshoreChartData(calculation.InputParameters.ForeshoreProfile, chartData.Collection.ElementAt(foreShoreChartDataIndex));
                AssertRevetmentChartData(calculation.InputParameters.ForeshoreGeometry.Last(),
                                         calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment,
                                         chartData.Collection.ElementAt(revetmentChartDataIndex));
                AssertRevetmentBaseChartData(calculation.InputParameters.ForeshoreGeometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             chartData.Collection.ElementAt(revetmentBaseChartDataIndex));
                AssertLowerBoundaryRevetmentChartData(calculation.InputParameters.ForeshoreGeometry,
                                                      calculation.InputParameters.LowerBoundaryRevetment,
                                                      chartData.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                AssertUpperBoundaryRevetmentChartData(calculation.InputParameters.ForeshoreGeometry,
                                                      calculation.InputParameters.UpperBoundaryRevetment,
                                                      chartData.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));

                AssertLowerBoundaryWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                                        calculation.InputParameters.LowerBoundaryWaterLevels,
                                                        chartData.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                AssertUpperBoundaryWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                                        calculation.InputParameters.UpperBoundaryWaterLevels,
                                                        chartData.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));
            }
        }

        [Test]
        public void UpdateObserver_CalculationNameUpdated_ChartTitleUpdated()
        {
            // Setup
            using (var view = new WaveConditionsInputView())
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
        public void UpdateObserver_OtherCalculationNameUpdated_ChartTitleNotUpdated()
        {
            // Setup
            using (var view = new WaveConditionsInputView())
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation1 = new TestWaveConditionsCalculation
                {
                    Name = initialName
                };
                var calculation2 = new TestWaveConditionsCalculation
                {
                    Name = initialName
                };

                view.Data = calculation1;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                calculation2.Name = updatedName;

                // Call
                calculation1.NotifyObservers();

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
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(7);
            mocks.ReplayAll();

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

            using (var view = new WaveConditionsInputView
            {
                Data = calculation
            })
            {
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex);
                var revetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex);
                var revetmentBaseChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex);
                var lowerBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
                var upperBoundaryRevetmentChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
                var lowerBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex);
                var upperBoundaryWaterLevelsChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex);

                foreshoreChartData.Attach(observer);
                revetmentChartData.Attach(observer);
                revetmentBaseChartData.Attach(observer);
                lowerBoundaryRevetmentChartData.Attach(observer);
                upperBoundaryRevetmentChartData.Attach(observer);
                lowerBoundaryWaterLevelsChartData.Attach(observer);
                upperBoundaryWaterLevelsChartData.Attach(observer);

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
                Assert.AreSame(revetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentChartDataIndex));
                Assert.AreSame(revetmentBaseChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(revetmentBaseChartDataIndex));
                Assert.AreSame(lowerBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex));
                Assert.AreSame(upperBoundaryRevetmentChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex));
                Assert.AreSame(lowerBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(lowerBoundaryWaterLevelsChartDataIndex));
                Assert.AreSame(upperBoundaryWaterLevelsChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(upperBoundaryWaterLevelsChartDataIndex));

                AssertForeshoreChartData(profile2, foreshoreChartData);
                AssertRevetmentChartData(profile2.Geometry.Last(), calculation.InputParameters.LowerBoundaryRevetment,
                                         calculation.InputParameters.UpperBoundaryRevetment, revetmentChartData);
                AssertRevetmentBaseChartData(profile2.Geometry.Last(),
                                             calculation.InputParameters.LowerBoundaryRevetment,
                                             revetmentBaseChartData);
                AssertLowerBoundaryRevetmentChartData(calculation.InputParameters.ForeshoreGeometry,
                                                      calculation.InputParameters.LowerBoundaryRevetment,
                                                      lowerBoundaryRevetmentChartData);
                AssertUpperBoundaryRevetmentChartData(calculation.InputParameters.ForeshoreGeometry,
                                                      calculation.InputParameters.UpperBoundaryRevetment,
                                                      upperBoundaryRevetmentChartData);

                AssertLowerBoundaryWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                                        calculation.InputParameters.LowerBoundaryWaterLevels,
                                                        lowerBoundaryWaterLevelsChartData);
                AssertUpperBoundaryWaterLevelsChartData(calculation.InputParameters.ForeshoreGeometry,
                                                        calculation.InputParameters.UpperBoundaryWaterLevels,
                                                        upperBoundaryWaterLevelsChartData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void UpdateObserver_OtherCalculationUpdated_ChartDataNotUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var calculation1 = new TestWaveConditionsCalculation();
            using (var view = new WaveConditionsInputView
            {
                Data = calculation1
            })
            {
                ((ChartLineData) view.Chart.Data.Collection.ElementAt(foreShoreChartDataIndex)).Attach(observer);

                var calculation2 = new TestWaveConditionsCalculation();
                ForeshoreProfile profile2 = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 3),
                    new Point2D(8, 8)
                });

                calculation2.InputParameters.ForeshoreProfile = profile2;

                // Call
                calculation2.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(calculation1, view.Data);
                mocks.VerifyAll(); // no update observer expected
            }
        }

        private static void AssertEmptyChartData(ChartDataCollection chartDataCollection)
        {
            Assert.AreEqual("Invoer", chartDataCollection.Name);

            List<ChartData> chartDatasList = chartDataCollection.Collection.ToList();

            Assert.AreEqual(7, chartDatasList.Count);

            var foreshoreData = (ChartLineData) chartDatasList[foreShoreChartDataIndex];
            var lowerBoundaryRevetmentData = (ChartLineData) chartDatasList[lowerBoundaryRevetmentChartDataIndex];
            var upperBoundaryRevetmentData = (ChartLineData) chartDatasList[upperBoundaryRevetmentChartDataIndex];
            var revetmentData = (ChartLineData) chartDatasList[revetmentChartDataIndex];
            var revetmentBaseData = (ChartLineData) chartDatasList[revetmentBaseChartDataIndex];
            var lowerBoundaryWaterLevelsData = (ChartLineData) chartDatasList[lowerBoundaryWaterLevelsChartDataIndex];
            var upperBoundaryWaterLevelsData = (ChartLineData) chartDatasList[upperBoundaryWaterLevelsChartDataIndex];

            CollectionAssert.IsEmpty(foreshoreData.Points);
            CollectionAssert.IsEmpty(lowerBoundaryRevetmentData.Points);
            CollectionAssert.IsEmpty(upperBoundaryRevetmentData.Points);
            CollectionAssert.IsEmpty(revetmentData.Points);
            CollectionAssert.IsEmpty(revetmentBaseData.Points);
            CollectionAssert.IsEmpty(lowerBoundaryWaterLevelsData.Points);
            CollectionAssert.IsEmpty(upperBoundaryWaterLevelsData.Points);

            Assert.AreEqual("Voorlandprofiel", foreshoreData.Name);
            Assert.AreEqual("Ondergrens bekleding", lowerBoundaryRevetmentData.Name);
            Assert.AreEqual("Bovengrens bekleding", upperBoundaryRevetmentData.Name);
            Assert.AreEqual("Bekleding", revetmentData.Name);
            Assert.AreEqual("Bekleding", revetmentBaseData.Name);
            Assert.AreEqual("Ondergrens waterstanden", lowerBoundaryWaterLevelsData.Name);
            Assert.AreEqual("Bovengrens waterstanden", upperBoundaryWaterLevelsData.Name);
        }

        private static void AssertForeshoreChartData(ForeshoreProfile foreshoreProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var foreshoreChartData = (ChartLineData) chartData;

            RoundedPoint2DCollection foreshoreGeometry = foreshoreProfile.Geometry;
            Assert.AreEqual(foreshoreGeometry.Count(), foreshoreChartData.Points.Length);
            CollectionAssert.AreEqual(foreshoreGeometry, foreshoreChartData.Points);

            string expectedName = $"{foreshoreProfile.Name} - Voorlandprofiel";
            Assert.AreEqual(expectedName, chartData.Name);
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

        private static void AssertRevetmentBaseChartData(Point2D lastForeshorePoint,
                                                         double lowerBoundaryRevetment,
                                                         ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var revetmentChartData = (ChartLineData) chartData;

            var expectedGeometry = new[]
            {
                new Point2D(lastForeshorePoint.X, lastForeshorePoint.Y),
                new Point2D(GetPointX(lowerBoundaryRevetment, lastForeshorePoint), lowerBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, revetmentChartData.Points);

            Assert.AreEqual("Bekleding", revetmentChartData.Name);
        }

        private static void AssertLowerBoundaryRevetmentChartData(RoundedPoint2DCollection foreshorePoints,
                                                                  double lowerBoundaryRevetment,
                                                                  ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var revetmentChartData = (ChartLineData) chartData;

            var expectedGeometry = new[]
            {
                new Point2D(foreshorePoints.First().X, lowerBoundaryRevetment),
                new Point2D(GetPointX(lowerBoundaryRevetment, foreshorePoints.Last()), lowerBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, revetmentChartData.Points);

            Assert.AreEqual("Ondergrens bekleding", revetmentChartData.Name);
        }

        private static void AssertUpperBoundaryRevetmentChartData(RoundedPoint2DCollection foreshorePoints,
                                                                  double upperBoundaryRevetment,
                                                                  ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var revetmentChartData = (ChartLineData) chartData;

            var expectedGeometry = new[]
            {
                new Point2D(foreshorePoints.First().X, upperBoundaryRevetment),
                new Point2D(GetPointX(upperBoundaryRevetment, foreshorePoints.Last()), upperBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, revetmentChartData.Points);

            Assert.AreEqual("Bovengrens bekleding", revetmentChartData.Name);
        }

        private static void AssertLowerBoundaryWaterLevelsChartData(RoundedPoint2DCollection foreshorePoints,
                                                                    double lowerBoundaryWaterLevels,
                                                                    ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var waterLevelsChartData = (ChartLineData) chartData;

            var expectedGeometry = new[]
            {
                new Point2D(foreshorePoints.First().X, lowerBoundaryWaterLevels),
                new Point2D(GetPointX(lowerBoundaryWaterLevels, foreshorePoints.Last()), lowerBoundaryWaterLevels)
            };

            CollectionAssert.AreEqual(expectedGeometry, waterLevelsChartData.Points);

            Assert.AreEqual("Ondergrens waterstanden", waterLevelsChartData.Name);
        }

        private static void AssertUpperBoundaryWaterLevelsChartData(RoundedPoint2DCollection foreshorePoints,
                                                                    double upperBoundaryWaterLevels,
                                                                    ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var waterLevelsChartData = (ChartLineData) chartData;

            var expectedGeometry = new[]
            {
                new Point2D(foreshorePoints.First().X, upperBoundaryWaterLevels),
                new Point2D(GetPointX(upperBoundaryWaterLevels, foreshorePoints.Last()), upperBoundaryWaterLevels)
            };

            CollectionAssert.AreEqual(expectedGeometry, waterLevelsChartData.Points);

            Assert.AreEqual("Bovengrens waterstanden", waterLevelsChartData.Name);
        }

        private static double GetPointX(double pointY, Point2D lastForeshorePoint)
        {
            return ((pointY - lastForeshorePoint.Y) / 3) + lastForeshorePoint.X;
        }
    }
}