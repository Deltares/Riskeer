// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Components.OxyPlot.Forms;
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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddChartControlWithEmptyCollectionData()
        {
            // Call
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                ChartControl chartControl = view.Controls[0] as ChartControl;
                Assert.IsNotNull(chartControl);
                Assert.AreEqual(DockStyle.Fill, chartControl.Dock);
                Assert.IsNotNull(chartControl.Data);
                CollectionAssert.IsEmpty(chartControl.Data.List);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Distance_DisplayName, chartControl.BottomAxisTitle);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Height_DisplayName, chartControl.LeftAxisTitle);
                Assert.IsNull(chartControl.ChartTitle);
            }
        }

        [Test]
        public void Data_GrassCoverErosionInwardsCalculation_DataSet()
        {
            // Setup
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();

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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                object calculation = new object();

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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();

                GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(3, view.Chart.Data.List.Count);

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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                var dikeProfile = GetDikeProfileWithGeometry();
                GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
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
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(3, chartData.List.Count);
                AssertDikeProfileChartData(dikeProfile, chartData.List[dikeProfileIndex]);
                AssertForeshoreChartData(dikeProfile, chartData.List[foreshoreIndex]);
                AssertDikeHeightChartData(dikeProfile, chartData.List[dikeHeightIndex]);
            }
        }

        [Test]
        public void Data_WithoutDikeProfile_CollectionOfEmptyDataSet()
        {
            // Setup
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(3, chartData.List.Count);
                var dikeGeometryData = (ChartLineData) chartData.List[dikeProfileIndex];
                var foreShoreData = (ChartLineData) chartData.List[foreshoreIndex];
                var dikeHeightData = (ChartLineData) chartData.List[dikeHeightIndex];

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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
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
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(3, chartData.List.Count);
                var dikeGeometryData = (ChartLineData) chartData.List[dikeProfileIndex];
                var foreShoreData = (ChartLineData) chartData.List[foreshoreIndex];
                var dikeHeightData = (ChartLineData) chartData.List[dikeHeightIndex];

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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                var initialName = "Initial name";
                var updatedName = "Updated name";

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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                var initialName = "Initial name";
                var updatedName = "Updated name";

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
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();

                GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                view.Data = calculation;

                ChartLineData oldDikeProfileChartData = (ChartLineData) view.Chart.Data.List[dikeProfileIndex];
                ChartLineData oldForeshoreChartData = (ChartLineData) view.Chart.Data.List[foreshoreIndex];
                ChartLineData oldDikeHeightChartData = (ChartLineData) view.Chart.Data.List[dikeHeightIndex];

                DikeProfile dikeProfile2 = GetSecondDikeProfileWithGeometry();

                calculation.InputParameters.DikeProfile = dikeProfile2;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                ChartLineData newDikeProfileChartData = (ChartLineData) view.Chart.Data.List[dikeProfileIndex];
                Assert.AreNotEqual(oldDikeProfileChartData, newDikeProfileChartData);
                AssertDikeProfileChartData(dikeProfile2, newDikeProfileChartData);

                ChartLineData newForeshoreChartData = (ChartLineData) view.Chart.Data.List[foreshoreIndex];
                Assert.AreNotEqual(oldForeshoreChartData, newForeshoreChartData);
                AssertForeshoreChartData(dikeProfile2, newForeshoreChartData);

                ChartLineData newDikeHeightChartData = (ChartLineData) view.Chart.Data.List[dikeHeightIndex];
                Assert.AreNotEqual(oldDikeHeightChartData, newDikeHeightChartData);
                AssertDikeHeightChartData(dikeProfile2, newDikeHeightChartData);
            }
        }

        [Test]
        public void UpdateObserver_OtherGrassCoverErosionInwardsCalculationUpdated_ChartDataNotUpdated()
        {
            // Setup
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                GrassCoverErosionInwardsCalculation calculation1 = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };
                
                GrassCoverErosionInwardsCalculation calculation2 = new GrassCoverErosionInwardsCalculation
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
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                DikeProfile dikeProfile = GetDikeProfileWithGeometry();
                GrassCoverErosionInwardsCalculation calculation1 = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfile
                    }
                };

                GrassCoverErosionInwardsCalculation calculation2 = new GrassCoverErosionInwardsCalculation();

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

        private DikeProfile GetDikeProfileWithGeometry()
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
                new Point2D(8.0, 9.0),
            };

            return GetDikeProfile(points, foreshore);
        }

        private DikeProfile GetSecondDikeProfileWithGeometry()
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
                new Point2D(2.0, 2.0),
            };

            return GetDikeProfile(points, foreshore);
        }

        private DikeProfile GetDikeProfile(RoughnessPoint[] dikeGeometry, Point2D[] foreshoreGeometry)
        {
            DikeProfile dikeProfile = new DikeProfile(new Point2D(0.0, 0.0), dikeGeometry, foreshoreGeometry)
            {
                Name = "Dike profile test",
                DikeHeight = (RoundedDouble) 10.0
            };

            return dikeProfile;
        }

        private void AssertDikeProfileChartData(DikeProfile dikeProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            ChartLineData dikeProfileChartData = (ChartLineData) chartData;

            RoughnessPoint[] dikeGeometry = dikeProfile.DikeGeometry;
            Assert.AreEqual(dikeGeometry.Length, dikeProfileChartData.Points.Count());
            CollectionAssert.AreEqual(dikeGeometry.Select(dg => dg.Point), dikeProfileChartData.Points);

            string expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DatatTypeDisplayName_0_DataIdentifier_1_, Resources.DikeProfile_DisplayName, dikeProfile.Name);
            Assert.AreEqual(expectedName, chartData.Name);
        }

        private void AssertForeshoreChartData(DikeProfile dikeProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            ChartLineData foreshoreChartData = (ChartLineData) chartData;

            Point2D[] foreshoreGeometry = dikeProfile.ForeshoreGeometry;
            Assert.AreEqual(foreshoreGeometry.Length, foreshoreChartData.Points.Count());
            CollectionAssert.AreEqual(foreshoreGeometry, foreshoreChartData.Points);

            string expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DatatTypeDisplayName_0_DataIdentifier_1_, Resources.Foreshore_DisplayName, dikeProfile.Name);
            Assert.AreEqual(expectedName, chartData.Name);
        }

        private void AssertDikeHeightChartData(DikeProfile dikeProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            ChartLineData dikeHeightChartData = (ChartLineData) chartData;

            Point2D[] dikeHeightGeometry =
            {
                new Point2D(dikeProfile.DikeGeometry.First().Point.X, dikeProfile.DikeHeight),
                new Point2D(dikeProfile.DikeGeometry.Last().Point.X, dikeProfile.DikeHeight)
            };

            Assert.AreEqual(dikeHeightGeometry.Length, dikeHeightChartData.Points.Count());
            CollectionAssert.AreEqual(dikeHeightGeometry, dikeHeightChartData.Points);

            string expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DatatTypeDisplayName_0_DataIdentifier_1_, Resources.DikeHeight_ChartName, dikeProfile.Name);
            Assert.AreEqual(expectedName, chartData.Name);
        }
    }
}