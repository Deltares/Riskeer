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

using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Factories;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms.Views
{
    /// <summary>
    /// This class is a view to show the wave conditions input.
    /// </summary>
    public partial class WaveConditionsInputView : UserControl, IChartView
    {
        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;

        private readonly ChartDataCollection chartDataCollection;
        private readonly ChartLineData foreshoreChartData;
        private readonly ChartLineData lowerBoundaryRevetmentChartData;
        private readonly ChartLineData upperBoundaryRevetmentChartData;
        private readonly ChartLineData revetmentChartData;
        private readonly ChartLineData revetmentBaseChartData;

        private readonly ChartLineData lowerBoundaryWaterLevelsChartData;
        private readonly ChartLineData upperBoundaryWaterLevelsChartData;
        private readonly ChartLineData designWaterLevelChartData;
        private readonly ChartMultipleLineData waterLevelsChartData;

        private IWaveConditionsCalculation data;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInputView"/>.        
        /// </summary>
        /// <param name="inputViewStyle">The style which should be applied to the <see cref="ChartLineData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputViewStyle"/>
        /// is <c>null</c>.</exception>
        public WaveConditionsInputView(IWaveConditionsInputViewStyle inputViewStyle)
        {
            if (inputViewStyle == null)
            {
                throw new ArgumentNullException(nameof(inputViewStyle));
            }

            InitializeComponent();

            calculationObserver = new Observer(UpdateChartTitle);
            calculationInputObserver = new Observer(UpdateChartData);

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.Calculation_Input);
            foreshoreChartData = RingtoetsChartDataFactory.CreateForeshoreGeometryChartData();
            lowerBoundaryRevetmentChartData = WaveConditionsChartDataFactory.CreateLowerRevetmentBoundaryChartData(inputViewStyle.RevetmentLineColor);
            upperBoundaryRevetmentChartData = WaveConditionsChartDataFactory.CreateUpperRevetmentBoundaryChartData(inputViewStyle.RevetmentLineColor);
            revetmentChartData = WaveConditionsChartDataFactory.CreateRevetmentChartData(inputViewStyle.RevetmentLineColor);
            revetmentBaseChartData = WaveConditionsChartDataFactory.CreateRevetmentBaseChartData(inputViewStyle.RevetmentLineColor);
            lowerBoundaryWaterLevelsChartData = WaveConditionsChartDataFactory.CreateLowerWaterLevelsBoundaryChartdata();
            upperBoundaryWaterLevelsChartData = WaveConditionsChartDataFactory.CreateUpperWaterLevelsBoundaryChartdata();
            designWaterLevelChartData = WaveConditionsChartDataFactory.CreateDesignWaterLevelChartdata(inputViewStyle.DesignWaterLevelName);
            waterLevelsChartData = WaveConditionsChartDataFactory.CreateWaterLevelsChartData();

            chartDataCollection.Add(foreshoreChartData);
            chartDataCollection.Add(revetmentBaseChartData);
            chartDataCollection.Add(revetmentChartData);
            chartDataCollection.Add(lowerBoundaryRevetmentChartData);
            chartDataCollection.Add(upperBoundaryRevetmentChartData);
            chartDataCollection.Add(waterLevelsChartData);
            chartDataCollection.Add(lowerBoundaryWaterLevelsChartData);
            chartDataCollection.Add(upperBoundaryWaterLevelsChartData);
            chartDataCollection.Add(designWaterLevelChartData);
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as IWaveConditionsCalculation;

                calculationObserver.Observable = data;
                calculationInputObserver.Observable = data?.InputParameters;

                if (data == null)
                {
                    chartControl.Data = null;
                    chartControl.ChartTitle = string.Empty;
                }
                else
                {
                    SetChartData();

                    chartControl.Data = chartDataCollection;
                    UpdateChartTitle();
                }
            }
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }

        protected override void Dispose(bool disposing)
        {
            calculationObserver.Dispose();
            calculationInputObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateChartData()
        {
            SetChartData();

            foreshoreChartData.NotifyObservers();
            revetmentBaseChartData.NotifyObservers();
            revetmentChartData.NotifyObservers();
            lowerBoundaryRevetmentChartData.NotifyObservers();
            upperBoundaryRevetmentChartData.NotifyObservers();
            lowerBoundaryWaterLevelsChartData.NotifyObservers();
            upperBoundaryWaterLevelsChartData.NotifyObservers();
            designWaterLevelChartData.NotifyObservers();
            waterLevelsChartData.NotifyObservers();
        }

        private void UpdateChartTitle()
        {
            chartControl.ChartTitle = data.Name;
        }

        private void SetChartData()
        {
            WaveConditionsInput input = data.InputParameters;

            WaveConditionsChartDataFactory.UpdateForeshoreGeometryChartDataName(foreshoreChartData, input);

            foreshoreChartData.Points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);
            revetmentBaseChartData.Points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);
            revetmentChartData.Points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);
            lowerBoundaryRevetmentChartData.Points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryRevetmentGeometryPoints(input);
            upperBoundaryRevetmentChartData.Points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryRevetmentGeometryPoints(input);
            lowerBoundaryWaterLevelsChartData.Points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryWaterLevelsGeometryPoints(input);
            upperBoundaryWaterLevelsChartData.Points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryWaterLevelsGeometryPoints(input);
            designWaterLevelChartData.Points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(input);
            waterLevelsChartData.Lines = WaveConditionsChartDataPointsFactory.CreateWaterLevelsGeometryPoints(input);
        }
    }
}