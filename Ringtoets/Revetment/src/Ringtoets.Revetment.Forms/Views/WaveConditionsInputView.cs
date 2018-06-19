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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Util.Extensions;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
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
        private readonly Func<HydraulicBoundaryLocationCalculation> getHydraulicBoundaryLocationCalculationFunc;

        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;
        private readonly Observer hydraulicBoundaryLocationCalculationObserver;

        private readonly ChartDataCollection chartDataCollection;
        private readonly ChartLineData foreshoreChartData;
        private readonly ChartLineData lowerBoundaryRevetmentChartData;
        private readonly ChartLineData upperBoundaryRevetmentChartData;
        private readonly ChartLineData lowerBoundaryWaterLevelsChartData;
        private readonly ChartLineData upperBoundaryWaterLevelsChartData;
        private readonly ChartLineData designWaterLevelChartData;
        private readonly ChartMultipleLineData waterLevelsChartData;
        private readonly ChartLineData revetmentBaseChartData;
        private readonly ChartLineData revetmentChartData;

        private readonly ICalculation<WaveConditionsInput> calculation;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInputView"/>.
        /// </summary>
        /// <param name="calculation">The calculation to show in the view.</param>
        /// <param name="getHydraulicBoundaryLocationCalculationFunc">The <see cref="Func{TResult}"/> for obtaining the <see cref="HydraulicBoundaryLocationCalculation"/>.</param>
        /// <param name="inputViewStyle">The style which should be applied to the <see cref="ChartLineData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public WaveConditionsInputView(ICalculation<WaveConditionsInput> calculation,
                                       Func<HydraulicBoundaryLocationCalculation> getHydraulicBoundaryLocationCalculationFunc,
                                       IWaveConditionsInputViewStyle inputViewStyle)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (getHydraulicBoundaryLocationCalculationFunc == null)
            {
                throw new ArgumentNullException(nameof(getHydraulicBoundaryLocationCalculationFunc));
            }

            if (inputViewStyle == null)
            {
                throw new ArgumentNullException(nameof(inputViewStyle));
            }

            InitializeComponent();

            this.getHydraulicBoundaryLocationCalculationFunc = getHydraulicBoundaryLocationCalculationFunc;

            calculationObserver = new Observer(UpdateChartTitle);
            calculationInputObserver = new Observer(UpdateCalculationInput);
            hydraulicBoundaryLocationCalculationObserver = new Observer(UpdateChartData);

            this.calculation = calculation;

            calculationObserver.Observable = calculation;
            calculationInputObserver.Observable = calculation.InputParameters;
            hydraulicBoundaryLocationCalculationObserver.Observable = getHydraulicBoundaryLocationCalculationFunc();

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.Calculation_Input);
            foreshoreChartData = RingtoetsChartDataFactory.CreateForeshoreGeometryChartData();
            lowerBoundaryRevetmentChartData = WaveConditionsChartDataFactory.CreateLowerRevetmentBoundaryChartData(inputViewStyle.RevetmentLineColor);
            upperBoundaryRevetmentChartData = WaveConditionsChartDataFactory.CreateUpperRevetmentBoundaryChartData(inputViewStyle.RevetmentLineColor);
            lowerBoundaryWaterLevelsChartData = WaveConditionsChartDataFactory.CreateLowerWaterLevelsBoundaryChartData();
            upperBoundaryWaterLevelsChartData = WaveConditionsChartDataFactory.CreateUpperWaterLevelsBoundaryChartData();
            designWaterLevelChartData = WaveConditionsChartDataFactory.CreateDesignWaterLevelChartData(inputViewStyle.DesignWaterLevelName);
            waterLevelsChartData = WaveConditionsChartDataFactory.CreateWaterLevelsChartData();
            revetmentBaseChartData = WaveConditionsChartDataFactory.CreateRevetmentBaseChartData(inputViewStyle.RevetmentLineColor);
            revetmentChartData = WaveConditionsChartDataFactory.CreateRevetmentChartData(inputViewStyle.RevetmentLineColor);

            chartDataCollection.Add(foreshoreChartData);
            chartDataCollection.Add(lowerBoundaryRevetmentChartData);
            chartDataCollection.Add(upperBoundaryRevetmentChartData);
            chartDataCollection.Add(lowerBoundaryWaterLevelsChartData);
            chartDataCollection.Add(upperBoundaryWaterLevelsChartData);
            chartDataCollection.Add(designWaterLevelChartData);
            chartDataCollection.Add(waterLevelsChartData);
            chartDataCollection.Add(revetmentBaseChartData);
            chartDataCollection.Add(revetmentChartData);

            SetChartData();

            chartControl.Data = chartDataCollection;
            UpdateChartTitle();
        }

        public object Data
        {
            get
            {
                return calculation;
            }
            set {}
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

        private void UpdateCalculationInput()
        {
            hydraulicBoundaryLocationCalculationObserver.Observable = getHydraulicBoundaryLocationCalculationFunc();
            UpdateChartData();
        }

        private void UpdateChartData()
        {
            SetChartData();
            chartDataCollection.Collection.ForEachElementDo(cd => cd.NotifyObservers());
        }

        private void UpdateChartTitle()
        {
            chartControl.ChartTitle = calculation.Name;
        }

        private void SetChartData()
        {
            WaveConditionsInput input = calculation.InputParameters;

            WaveConditionsChartDataFactory.UpdateForeshoreGeometryChartDataName(foreshoreChartData, input);

            foreshoreChartData.Points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);
            lowerBoundaryRevetmentChartData.Points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryRevetmentGeometryPoints(input);
            upperBoundaryRevetmentChartData.Points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryRevetmentGeometryPoints(input);
            lowerBoundaryWaterLevelsChartData.Points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryWaterLevelsGeometryPoints(input);
            upperBoundaryWaterLevelsChartData.Points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryWaterLevelsGeometryPoints(input);

            RoundedDouble assessmentLevel = getHydraulicBoundaryLocationCalculationFunc()?.Output?.Result ?? RoundedDouble.NaN;
            designWaterLevelChartData.Points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(input, assessmentLevel);
            waterLevelsChartData.Lines = WaveConditionsChartDataPointsFactory.CreateWaterLevelsGeometryPoints(input, assessmentLevel);

            revetmentBaseChartData.Points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);
            revetmentChartData.Points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);
        }
    }
}