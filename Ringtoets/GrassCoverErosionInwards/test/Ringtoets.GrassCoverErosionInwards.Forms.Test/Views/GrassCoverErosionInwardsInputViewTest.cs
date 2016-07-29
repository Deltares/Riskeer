﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddChartControlWithCollectionOfEmptyChartData()
        {
            // Call
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Chart, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control)view.Chart).Dock);
                Assert.AreEqual(Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName, view.Chart.Data.Name);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Distance_DisplayName, view.Chart.BottomAxisTitle);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Height_DisplayName, view.Chart.LeftAxisTitle);
                AssertEmptyChartData(view.Chart.Data);
            }
        }

        [Test]
        public void Data_GrassCoverErosionInwardsCalculation_DataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                var calculation = new GrassCoverErosionInwardsCalculation();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanGrassCoverErosionInwardsCalculations_DataNull()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
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
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();

                var calculation = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(3, view.Chart.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Chart.Data);
            }
        }

        [Test]
        public void Data_SetChartData_ChartDataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                var calculation = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);

                var chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(3, chartData.Collection.Count());
                AssertDikeProfileChartData(dikeProfile, chartData.Collection.ElementAt(dikeProfileIndex));
                AssertForeshoreChartData(dikeProfile, chartData.Collection.ElementAt(foreshoreIndex));
                AssertDikeHeightChartData(dikeProfile, chartData.Collection.ElementAt(dikeHeightIndex));
            }
        }

        [Test]
        public void Data_WithoutDikeProfile_CollectionOfEmptyDataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                var calculation = new GrassCoverErosionInwardsCalculation();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);

                var chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(3, chartData.Collection.Count());

                var dikeGeometryData = (ChartLineData) chartData.Collection.ElementAt(dikeProfileIndex);
                var foreShoreData = (ChartLineData) chartData.Collection.ElementAt(foreshoreIndex);
                var dikeHeightData = (ChartLineData) chartData.Collection.ElementAt(dikeHeightIndex);

                CollectionAssert.IsEmpty(dikeGeometryData.Points);
                CollectionAssert.IsEmpty(foreShoreData.Points);
                CollectionAssert.IsEmpty(dikeHeightData.Points);
                Assert.AreEqual(Resources.DikeProfile_DisplayName, dikeGeometryData.Name);
                Assert.AreEqual(Resources.Foreshore_DisplayName, foreShoreData.Name);
                Assert.AreEqual(Resources.DikeHeight_ChartName, dikeHeightData.Name);
            }
        }

        [Test]
        public void Data_UseForeshoreFalse_SetEmptyForeshoreDataOnChart()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                var calculation = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile,
                        UseForeshore = false
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);

                var chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(3, chartData.Collection.Count());

                var dikeGeometryData = (ChartLineData) chartData.Collection.ElementAt(dikeProfileIndex);
                var foreShoreData = (ChartLineData) chartData.Collection.ElementAt(foreshoreIndex);
                var dikeHeightData = (ChartLineData) chartData.Collection.ElementAt(dikeHeightIndex);

                CollectionAssert.IsEmpty(foreShoreData.Points);
                Assert.AreEqual(Resources.Foreshore_DisplayName, foreShoreData.Name);
                AssertDikeProfileChartData(dikeProfile, dikeGeometryData);
                AssertDikeHeightChartData(dikeProfile, dikeHeightData);
            }
        }

        [Test]
        public void UpdateObservers_CalculationNameUpdated_ChartTitleUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation = new GrassCoverErosionInwardsCalculation
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
        public void UpdateObservers_OtherCalculationNameUpdated_ChartTitleNotUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation1 = new GrassCoverErosionInwardsCalculation
                {
                    Name = initialName
                };
                var calculation2 = new GrassCoverErosionInwardsCalculation
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
        public void UpdateObservers_CalculationDikeProfileUpdated_SetNewChartData()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();

                var calculation = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                view.Data = calculation;

                var dikeProfileChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(dikeProfileIndex);
                var foreshoreChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(foreshoreIndex);
                var dikeHeightChartData = (ChartLineData) view.Chart.Data.Collection.ElementAt(dikeHeightIndex);

                DikeProfile dikeProfile2 = GetSecondDikeProfileWithGeometry();

                calculation.InputParameters.DikeProfile = dikeProfile2;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                Assert.AreSame(dikeProfileChartData, (ChartLineData) view.Chart.Data.Collection.ElementAt(dikeProfileIndex));
                AssertDikeProfileChartData(dikeProfile2, dikeProfileChartData);
                Assert.AreSame(foreshoreChartData, (ChartLineData)view.Chart.Data.Collection.ElementAt(foreshoreIndex));
                AssertForeshoreChartData(dikeProfile2, foreshoreChartData);
                Assert.AreSame(dikeHeightChartData, (ChartLineData)view.Chart.Data.Collection.ElementAt(dikeHeightIndex));
                AssertDikeHeightChartData(dikeProfile2, dikeHeightChartData);
            }
        }

        [Test]
        public void UpdateObserver_OtherGrassCoverErosionInwardsCalculationUpdated_ChartDataNotUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                var calculation1 = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };
                
                var calculation2 = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                view.Data = calculation1;

                DikeProfile dikeProfile2 = GetSecondDikeProfileWithGeometry();

                calculation2.InputParameters.DikeProfile = dikeProfile2;

                // Call
                calculation2.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(calculation1, view.Data);
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_ChartSeriesOrderSame()
        {
            // Setup
            const int updatedDikeProfileIndex = dikeProfileIndex + 1;
            const int updatedForeshoreIndex = foreshoreIndex;
            const int updatedDikeHeightIndex = dikeHeightIndex - 1;

            var calculation = new GrassCoverErosionInwardsCalculation();

            using (var view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation
            })
            {
                ChartLineData dataToMove = (ChartLineData) view.Chart.Data.Collection.ElementAt(dikeProfileIndex);
                view.Chart.Data.Remove(dataToMove);
                view.Chart.Data.Add(dataToMove);

                // Precondition
                var dikeProfileChartData = (ChartLineData)view.Chart.Data.Collection.ElementAt(updatedDikeProfileIndex);
                Assert.AreEqual(Resources.DikeProfile_DisplayName, dikeProfileChartData.Name);

                var foreshoreChartData = (ChartLineData)view.Chart.Data.Collection.ElementAt(updatedForeshoreIndex);
                Assert.AreEqual(Resources.Foreshore_DisplayName, foreshoreChartData.Name);

                var dikeHeightChartData = (ChartLineData)view.Chart.Data.Collection.ElementAt(updatedDikeHeightIndex);
                Assert.AreEqual(Resources.DikeHeight_ChartName, dikeHeightChartData.Name);

                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                calculation.InputParameters.DikeProfile = dikeProfile;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                var actualDikeProfileChartData = (ChartLineData)view.Chart.Data.Collection.ElementAt(updatedDikeProfileIndex);
                string expectedDikeProfileName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                               dikeProfile.Name, Resources.DikeProfile_DisplayName);
                Assert.AreEqual(expectedDikeProfileName, actualDikeProfileChartData.Name);

                var actualForeshoreChartData = (ChartLineData)view.Chart.Data.Collection.ElementAt(updatedForeshoreIndex);
                string expectedForeshoreName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                dikeProfile.Name,
                                                Resources.Foreshore_DisplayName);
                Assert.AreEqual(expectedForeshoreName, actualForeshoreChartData.Name);

                var actualDikeHeightChartData = (ChartLineData)view.Chart.Data.Collection.ElementAt(updatedDikeHeightIndex);
                Assert.AreEqual(Resources.DikeHeight_ChartName, actualDikeHeightChartData.Name);
            }
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                var calculation1 = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                var calculation2 = new GrassCoverErosionInwardsCalculation();

                view.Data = calculation1;
                ChartData dataBeforeUpdate = view.Chart.Data;

                view.Data = calculation2;

                DikeProfile dikeProfile2 = GetSecondDikeProfileWithGeometry();

                calculation1.InputParameters.DikeProfile = dikeProfile2;

                // Call
                calculation1.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, view.Chart.Data);
            }
        }

        private const int foreshoreIndex = 0;
        private const int dikeProfileIndex = 1;
        private const int dikeHeightIndex = 2;

        private static DikeProfile GetDikeProfileWithGeometry()
        {
            var points = new[]
            {
                new RoughnessPoint(new Point2D(2.0, 3.0), 4.0),
                new RoughnessPoint(new Point2D(3.0, 4.0), 4.0),
                new RoughnessPoint(new Point2D(4.0, 5.0), 4.0)
            };

            var foreshore = new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(5.0, 6.0),
                new Point2D(8.0, 9.0)
            };

            return GetDikeProfile(points, foreshore);
        }

        private static DikeProfile GetSecondDikeProfileWithGeometry()
        {
            var points = new[]
            {
                new RoughnessPoint(new Point2D(8.0, 3.0), 1.0),
                new RoughnessPoint(new Point2D(10.0, 4.0), 1.0),
                new RoughnessPoint(new Point2D(12.0, 5.0), 1.0)
            };

            var foreshore = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(2.0, 2.0)
            };

            return GetDikeProfile(points, foreshore);
        }

        private static DikeProfile GetDikeProfile(RoughnessPoint[] dikeGeometry, Point2D[] foreshoreGeometry)
        {
            return new DikeProfile(new Point2D(0.0, 0.0), dikeGeometry, foreshoreGeometry,
                                   null, new DikeProfile.ConstructionProperties
                                   {
                                       Name = "Dike profile test",
                                       DikeHeight = 10.0
                                   });
        }

        private static void AssertEmptyChartData(ChartDataCollection chartData)
        {
            Assert.IsInstanceOf<ChartDataCollection>(chartData);

            var chartDatasList = chartData.Collection.ToList();

            Assert.AreEqual(3, chartDatasList.Count);

            var foreshoreData = (ChartLineData) chartDatasList[foreshoreIndex];
            var dikeProfileData = (ChartLineData) chartDatasList[dikeProfileIndex];
            var dikeHeightData = (ChartLineData) chartDatasList[dikeHeightIndex];

            CollectionAssert.IsEmpty(foreshoreData.Points);
            CollectionAssert.IsEmpty(dikeProfileData.Points);
            CollectionAssert.IsEmpty(dikeHeightData.Points);

            Assert.AreEqual(Resources.Foreshore_DisplayName, foreshoreData.Name);
            Assert.AreEqual(Resources.DikeProfile_DisplayName, dikeProfileData.Name);
            Assert.AreEqual(Resources.DikeHeight_ChartName, dikeHeightData.Name);
        }

        private static void AssertDikeProfileChartData(DikeProfile dikeProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            ChartLineData dikeProfileChartData = (ChartLineData) chartData;

            RoughnessPoint[] dikeGeometry = dikeProfile.DikeGeometry;
            Assert.AreEqual(dikeGeometry.Length, dikeProfileChartData.Points.Length);
            CollectionAssert.AreEqual(dikeGeometry.Select(dg => dg.Point), dikeProfileChartData.Points);

            string expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                dikeProfile.Name,
                                                Resources.DikeProfile_DisplayName);
            Assert.AreEqual(expectedName, chartData.Name);
        }

        private static void AssertForeshoreChartData(DikeProfile dikeProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            ChartLineData foreshoreChartData = (ChartLineData) chartData;

            RoundedPoint2DCollection foreshoreGeometry = dikeProfile.ForeshoreGeometry;
            Assert.AreEqual(foreshoreGeometry.Count(), foreshoreChartData.Points.Length);
            CollectionAssert.AreEqual(foreshoreGeometry, foreshoreChartData.Points);

            string expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                dikeProfile.Name,
                                                Resources.Foreshore_DisplayName);
            Assert.AreEqual(expectedName, chartData.Name);
        }

        private static void AssertDikeHeightChartData(DikeProfile dikeProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            ChartLineData dikeHeightChartData = (ChartLineData) chartData;

            Point2D[] dikeHeightGeometry =
            {
                new Point2D(dikeProfile.DikeGeometry.First().Point.X, dikeProfile.DikeHeight),
                new Point2D(dikeProfile.DikeGeometry.Last().Point.X, dikeProfile.DikeHeight)
            };

            Assert.AreEqual(dikeHeightGeometry.Length, dikeHeightChartData.Points.Length);
            CollectionAssert.AreEqual(dikeHeightGeometry, dikeHeightChartData.Points);

            Assert.AreEqual(Resources.DikeHeight_ChartName, chartData.Name);
        }
    }
}