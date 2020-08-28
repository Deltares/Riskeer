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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util.Extensions;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Riskeer.Common.Forms.Factories;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.Factories;
using Riskeer.MacroStabilityInwards.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// Control to show macro stability inwards output data in a chart view.
    /// </summary>
    public partial class MacroStabilityInwardsOutputChartControl : UserControl, IChartView
    {
        private readonly ChartDataCollection chartDataCollection;
        private readonly ChartDataCollection soilProfileChartData;
        private readonly ChartDataCollection waternetZonesExtremeChartData;
        private readonly ChartDataCollection waternetZonesDailyChartData;
        private readonly ChartMultipleAreaData slicesChartData;
        private readonly ChartLineData surfaceLineChartData;
        private readonly ChartPointData surfaceLevelInsideChartData;
        private readonly ChartPointData ditchPolderSideChartData;
        private readonly ChartPointData bottomDitchPolderSideChartData;
        private readonly ChartPointData bottomDitchDikeSideChartData;
        private readonly ChartPointData ditchDikeSideChartData;
        private readonly ChartPointData dikeToeAtPolderChartData;
        private readonly ChartPointData shoulderTopInsideChartData;
        private readonly ChartPointData shoulderBaseInsideChartData;
        private readonly ChartPointData dikeTopAtPolderChartData;
        private readonly ChartPointData dikeToeAtRiverChartData;
        private readonly ChartPointData dikeTopAtRiverChartData;
        private readonly ChartPointData surfaceLevelOutsideChartData;
        private readonly ChartPointData leftGridChartData;
        private readonly ChartPointData rightGridChartData;
        private readonly ChartLineData slipPlaneChartData;
        private readonly ChartLineData leftCircleRadiusChartData;
        private readonly ChartLineData rightCircleRadiusChartData;
        private readonly ChartMultipleLineData tangentLinesChartData;

        private readonly ChartDataCollection sliceParametersChartDataCollection;
        private readonly ChartMultipleAreaData sliceCohesionChartData;
        private readonly ChartMultipleAreaData sliceEffectiveStressChartData;
        private readonly ChartMultipleAreaData sliceTotalPorePressureChartData;
        private readonly ChartMultipleAreaData sliceWeightChartData;
        private readonly ChartMultipleAreaData slicePiezometricPorePressureChartData;
        private readonly ChartMultipleAreaData slicePorePressureChartData;
        private readonly ChartMultipleAreaData sliceVerticalPorePressureChartData;
        private readonly ChartMultipleAreaData sliceHorizontalPorePressureChartData;
        private readonly ChartMultipleAreaData sliceOverConsolidationRatioChartData;
        private readonly ChartMultipleAreaData slicePopChartData;
        private readonly ChartMultipleAreaData sliceNormalStressChartData;
        private readonly ChartMultipleAreaData sliceShearStressChartData;
        private readonly ChartMultipleAreaData sliceLoadStressChartData;

        private readonly Func<RoundedDouble> getNormativeAssessmentLevelFunc;

        private readonly List<ChartMultipleAreaData> soilLayerChartDataLookup;

        private readonly IDictionary<MacroStabilityInwardsPhreaticLine, ChartLineData> phreaticLineExtremeLookup;
        private readonly IDictionary<MacroStabilityInwardsPhreaticLine, ChartLineData> phreaticLineDailyLookup;
        private readonly IDictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> waternetLineExtremeLookup;
        private readonly IDictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> waternetLineDailyLookup;

        private MacroStabilityInwardsCalculationScenario data;
        private MacroStabilityInwardsSurfaceLine currentSurfaceLine;
        private IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> currentSoilProfile;

        private MacroStabilityInwardsWaternet currentWaternetExtreme;
        private MacroStabilityInwardsWaternet currentWaternetDaily;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutputChartControl"/>.
        /// </summary>
        /// <param name="data">The calculation to show the output for.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{TResult}"/> for obtaining the normative assessment level.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsOutputChartControl(MacroStabilityInwardsCalculationScenario data,
                                                       Func<RoundedDouble> getNormativeAssessmentLevelFunc)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (getNormativeAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormativeAssessmentLevelFunc));
            }

            this.data = data;
            this.getNormativeAssessmentLevelFunc = getNormativeAssessmentLevelFunc;

            InitializeComponent();

            chartDataCollection = new ChartDataCollection(RiskeerCommonFormsResources.CalculationOutput_DisplayName);
            soilProfileChartData = RiskeerChartDataFactory.CreateSoilProfileChartData();
            surfaceLineChartData = RiskeerChartDataFactory.CreateSurfaceLineChartData();
            surfaceLevelInsideChartData = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelInsideChartData();
            ditchPolderSideChartData = RiskeerChartDataFactory.CreateDitchPolderSideChartData();
            bottomDitchPolderSideChartData = RiskeerChartDataFactory.CreateBottomDitchPolderSideChartData();
            bottomDitchDikeSideChartData = RiskeerChartDataFactory.CreateBottomDitchDikeSideChartData();
            ditchDikeSideChartData = RiskeerChartDataFactory.CreateDitchDikeSideChartData();
            dikeToeAtPolderChartData = RiskeerChartDataFactory.CreateDikeToeAtPolderChartData();
            shoulderTopInsideChartData = MacroStabilityInwardsChartDataFactory.CreateShoulderTopInsideChartData();
            shoulderBaseInsideChartData = MacroStabilityInwardsChartDataFactory.CreateShoulderBaseInsideChartData();
            dikeTopAtPolderChartData = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtPolderChartData();
            dikeToeAtRiverChartData = RiskeerChartDataFactory.CreateDikeToeAtRiverChartData();
            dikeTopAtRiverChartData = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtRiverChartData();
            surfaceLevelOutsideChartData = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelOutsideChartData();
            waternetZonesExtremeChartData = MacroStabilityInwardsChartDataFactory.CreateWaternetZonesExtremeChartDataCollection();
            waternetZonesDailyChartData = MacroStabilityInwardsChartDataFactory.CreateWaternetZonesDailyChartDataCollection();
            tangentLinesChartData = MacroStabilityInwardsChartDataFactory.CreateTangentLinesChartData();
            leftGridChartData = MacroStabilityInwardsChartDataFactory.CreateLeftGridChartData();
            rightGridChartData = MacroStabilityInwardsChartDataFactory.CreateRightGridChartData();
            slipPlaneChartData = MacroStabilityInwardsChartDataFactory.CreateSlipPlaneChartData();
            leftCircleRadiusChartData = MacroStabilityInwardsChartDataFactory.CreateActiveCircleRadiusChartData();
            rightCircleRadiusChartData = MacroStabilityInwardsChartDataFactory.CreatePassiveCircleRadiusChartData();
            slicesChartData = MacroStabilityInwardsSliceChartDataFactory.CreateSlicesChartData();

            sliceParametersChartDataCollection = MacroStabilityInwardsSliceChartDataFactory.CreateSliceParametersChartDataCollection();
            sliceCohesionChartData = MacroStabilityInwardsSliceChartDataFactory.CreateCohesionChartData();
            sliceEffectiveStressChartData = MacroStabilityInwardsSliceChartDataFactory.CreateEffectiveStressChartData();
            sliceTotalPorePressureChartData = MacroStabilityInwardsSliceChartDataFactory.CreateTotalPorePressureChartData();
            sliceWeightChartData = MacroStabilityInwardsSliceChartDataFactory.CreateWeightChartData();
            slicePiezometricPorePressureChartData = MacroStabilityInwardsSliceChartDataFactory.CreatePiezometricPorePressureChartData();
            slicePorePressureChartData = MacroStabilityInwardsSliceChartDataFactory.CreatePorePressureChartData();
            sliceVerticalPorePressureChartData = MacroStabilityInwardsSliceChartDataFactory.CreateVerticalPorePressureChartData();
            sliceHorizontalPorePressureChartData = MacroStabilityInwardsSliceChartDataFactory.CreateHorizontalPorePressureChartData();
            sliceOverConsolidationRatioChartData = MacroStabilityInwardsSliceChartDataFactory.CreateOverConsolidationRatioChartData();
            slicePopChartData = MacroStabilityInwardsSliceChartDataFactory.CreatePopChartData();
            sliceNormalStressChartData = MacroStabilityInwardsSliceChartDataFactory.CreateNormalStressChartData();
            sliceShearStressChartData = MacroStabilityInwardsSliceChartDataFactory.CreateShearStressChartData();
            sliceLoadStressChartData = MacroStabilityInwardsSliceChartDataFactory.CreateLoadStressChartData();

            chartDataCollection.Add(soilProfileChartData);
            chartDataCollection.Add(surfaceLineChartData);
            chartDataCollection.Add(surfaceLevelInsideChartData);
            chartDataCollection.Add(ditchPolderSideChartData);
            chartDataCollection.Add(bottomDitchPolderSideChartData);
            chartDataCollection.Add(bottomDitchDikeSideChartData);
            chartDataCollection.Add(ditchDikeSideChartData);
            chartDataCollection.Add(dikeToeAtPolderChartData);
            chartDataCollection.Add(shoulderTopInsideChartData);
            chartDataCollection.Add(shoulderBaseInsideChartData);
            chartDataCollection.Add(dikeTopAtPolderChartData);
            chartDataCollection.Add(dikeToeAtRiverChartData);
            chartDataCollection.Add(dikeTopAtRiverChartData);
            chartDataCollection.Add(surfaceLevelOutsideChartData);
            chartDataCollection.Add(waternetZonesExtremeChartData);
            chartDataCollection.Add(waternetZonesDailyChartData);
            chartDataCollection.Add(tangentLinesChartData);
            chartDataCollection.Add(leftGridChartData);
            chartDataCollection.Add(rightGridChartData);
            chartDataCollection.Add(slicesChartData);
            chartDataCollection.Add(slipPlaneChartData);
            chartDataCollection.Add(leftCircleRadiusChartData);
            chartDataCollection.Add(rightCircleRadiusChartData);

            chartDataCollection.Add(sliceParametersChartDataCollection);
            sliceParametersChartDataCollection.Add(sliceLoadStressChartData);
            sliceParametersChartDataCollection.Add(sliceShearStressChartData);
            sliceParametersChartDataCollection.Add(sliceNormalStressChartData);
            sliceParametersChartDataCollection.Add(slicePopChartData);
            sliceParametersChartDataCollection.Add(sliceOverConsolidationRatioChartData);
            sliceParametersChartDataCollection.Add(sliceHorizontalPorePressureChartData);
            sliceParametersChartDataCollection.Add(sliceVerticalPorePressureChartData);
            sliceParametersChartDataCollection.Add(slicePorePressureChartData);
            sliceParametersChartDataCollection.Add(slicePiezometricPorePressureChartData);
            sliceParametersChartDataCollection.Add(sliceWeightChartData);
            sliceParametersChartDataCollection.Add(sliceTotalPorePressureChartData);
            sliceParametersChartDataCollection.Add(sliceEffectiveStressChartData);
            sliceParametersChartDataCollection.Add(sliceCohesionChartData);

            soilLayerChartDataLookup = new List<ChartMultipleAreaData>();

            phreaticLineExtremeLookup = new Dictionary<MacroStabilityInwardsPhreaticLine, ChartLineData>();
            phreaticLineDailyLookup = new Dictionary<MacroStabilityInwardsPhreaticLine, ChartLineData>();
            waternetLineExtremeLookup = new Dictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData>();
            waternetLineDailyLookup = new Dictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData>();

            UpdateChartData();

            chartControl.Data = chartDataCollection;
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as MacroStabilityInwardsCalculationScenario;
            }
        }

        /// <summary>
        /// Updates the chart data.
        /// </summary>
        public void UpdateChartData()
        {
            Chart.ChartTitle = data != null ? data.Name : string.Empty;

            if (data != null)
            {
                UpdateInputChartData();
                UpdateOutputChartData();

                chartDataCollection.Collection.ForEachElementDo(cd => cd.NotifyObservers());
                sliceParametersChartDataCollection.Collection.ForEachElementDo(cd => cd.NotifyObservers());
            }
        }

        private void UpdateOutputChartData()
        {
            MacroStabilityInwardsOutput output = data.Output;

            leftGridChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(output?.SlipPlane.LeftGrid);
            rightGridChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(output?.SlipPlane.RightGrid);

            slipPlaneChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSlipPlanePoints(output?.SlidingCurve);
            leftCircleRadiusChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateLeftCircleRadiusPoints(output?.SlidingCurve);
            rightCircleRadiusChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateRightCircleRadiusPoints(output?.SlidingCurve);
            slicesChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateSliceAreas(output?.SlidingCurve);
            tangentLinesChartData.Lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(output?.SlipPlane.TangentLines,
                                                                                                         data.InputParameters.SurfaceLine);
            SetSliceParameterChartData(output?.SlidingCurve);
        }

        private void SetSliceParameterChartData(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            sliceCohesionChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateCohesionAreas(slidingCurve);
            sliceEffectiveStressChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressAreas(slidingCurve);
            sliceTotalPorePressureChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateTotalPorePressureAreas(slidingCurve);
            sliceWeightChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateWeightAreas(slidingCurve);
            slicePiezometricPorePressureChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePiezometricPorePressureAreas(slidingCurve);
            slicePorePressureChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePorePressureAreas(slidingCurve);
            sliceVerticalPorePressureChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateVerticalPorePressureAreas(slidingCurve);
            sliceHorizontalPorePressureChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateHorizontalPorePressureAreas(slidingCurve);
            sliceOverConsolidationRatioChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateOverConsolidationRatioAreas(slidingCurve);
            slicePopChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas(slidingCurve);
            sliceNormalStressChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateNormalStressAreas(slidingCurve);
            sliceShearStressChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateShearStressAreas(slidingCurve);
            sliceLoadStressChartData.Areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateLoadStressAreas(slidingCurve);
        }

        private void UpdateInputChartData()
        {
            MacroStabilityInwardsInput input = data.InputParameters;
            MacroStabilityInwardsSurfaceLine surfaceLine = input.SurfaceLine;
            IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile = input.StochasticSoilProfile?.SoilProfile;

            if (!ReferenceEquals(currentSoilProfile, soilProfile) || !ReferenceEquals(currentSurfaceLine, surfaceLine))
            {
                currentSoilProfile = soilProfile;
                currentSurfaceLine = surfaceLine;

                SetSoilProfileChartData();
            }

            SetWaternetExtremeChartData(DerivedMacroStabilityInwardsInput.GetWaternetExtreme(input, GetEffectiveAssessmentLevel()));
            SetWaternetDailyChartData(DerivedMacroStabilityInwardsInput.GetWaternetDaily(input));

            if (data.Output != null)
            {
                SetSurfaceLineChartData(surfaceLine);
                SetSoilLayerAreas();
                SetWaternetDatas(surfaceLine);
            }
            else
            {
                SetSurfaceLineChartData(null);
                SetEmptySoilLayerAreas();
                SetEmptyWaternets();
            }

            soilProfileChartData.Collection.ForEachElementDo(sp => sp.NotifyObservers());
            waternetZonesDailyChartData.Collection.ForEachElementDo(cd => cd.NotifyObservers());
            waternetZonesExtremeChartData.Collection.ForEachElementDo(cd => cd.NotifyObservers());
        }

        private void SetSoilProfileChartData()
        {
            soilProfileChartData.Clear();
            soilLayerChartDataLookup.Clear();

            IEnumerable<MacroStabilityInwardsSoilLayer2D> soilLayers = GetSoilLayers().Reverse();

            soilLayers.Select(MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData)
                      .ForEachElementDo(sl =>
                      {
                          soilProfileChartData.Insert(0, sl);
                          soilLayerChartDataLookup.Add(sl);
                      });

            MacroStabilityInwardsChartDataFactory.UpdateSoilProfileChartDataName(soilProfileChartData, data?.InputParameters.StochasticSoilProfile?.SoilProfile);
        }

        private void SetSurfaceLineChartData(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            MacroStabilityInwardsChartDataFactory.UpdateSurfaceLineChartDataName(surfaceLineChartData, surfaceLine);

            surfaceLineChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLinePoints(surfaceLine);
            surfaceLevelInsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelInsidePoint(surfaceLine);
            ditchPolderSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine);
            bottomDitchPolderSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine);
            bottomDitchDikeSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine);
            ditchDikeSideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine);
            dikeToeAtPolderChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);
            shoulderTopInsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderTopInsidePoint(surfaceLine);
            shoulderBaseInsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderBaseInsidePoint(surfaceLine);
            dikeTopAtPolderChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtPolderPoint(surfaceLine);
            dikeToeAtRiverChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);
            dikeTopAtRiverChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtRiverPoint(surfaceLine);
            surfaceLevelOutsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelOutsidePoint(surfaceLine);
        }

        private void SetSoilLayerAreas()
        {
            var i = 0;
            foreach (MacroStabilityInwardsSoilLayer2D soilLayer in GetSoilLayers().Reverse())
            {
                ChartMultipleAreaData soilLayerData = soilLayerChartDataLookup[i++];
                soilLayerData.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateOuterRingArea(soilLayer);
            }
        }

        private void SetEmptySoilLayerAreas()
        {
            foreach (ChartMultipleAreaData soilLayerData in soilLayerChartDataLookup)
            {
                soilLayerData.Areas = Enumerable.Empty<Point2D[]>();
            }
        }

        private IEnumerable<MacroStabilityInwardsSoilLayer2D> GetSoilLayers()
        {
            return MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(data.InputParameters.SoilProfileUnderSurfaceLine?.Layers);
        }

        private void SetWaternetDatas(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            foreach (KeyValuePair<MacroStabilityInwardsPhreaticLine, ChartLineData> dailyPhreaticLineData in phreaticLineDailyLookup)
            {
                dailyPhreaticLineData.Value.Points = MacroStabilityInwardsChartDataPointsFactory.CreatePhreaticLinePoints(dailyPhreaticLineData.Key);
            }

            foreach (KeyValuePair<MacroStabilityInwardsPhreaticLine, ChartLineData> extremePhreaticLineData in phreaticLineExtremeLookup)
            {
                extremePhreaticLineData.Value.Points = MacroStabilityInwardsChartDataPointsFactory.CreatePhreaticLinePoints(extremePhreaticLineData.Key);
            }

            foreach (KeyValuePair<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> dailyWaternetLineData in waternetLineDailyLookup)
            {
                dailyWaternetLineData.Value.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(dailyWaternetLineData.Key, surfaceLine);
            }

            foreach (KeyValuePair<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> extremeWaternetLineData in waternetLineExtremeLookup)
            {
                extremeWaternetLineData.Value.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(extremeWaternetLineData.Key, surfaceLine);
            }
        }

        private void SetEmptyWaternets()
        {
            foreach (KeyValuePair<MacroStabilityInwardsPhreaticLine, ChartLineData> dailyPhreaticLineData in phreaticLineDailyLookup)
            {
                dailyPhreaticLineData.Value.Points = new Point2D[0];
            }

            foreach (KeyValuePair<MacroStabilityInwardsPhreaticLine, ChartLineData> extremePhreaticLineData in phreaticLineExtremeLookup)
            {
                extremePhreaticLineData.Value.Points = new Point2D[0];
            }

            foreach (KeyValuePair<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> dailyWaternetLineData in waternetLineDailyLookup)
            {
                dailyWaternetLineData.Value.Areas = Enumerable.Empty<Point2D[]>();
            }

            foreach (KeyValuePair<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> extremeWaternetLineData in waternetLineExtremeLookup)
            {
                extremeWaternetLineData.Value.Areas = Enumerable.Empty<Point2D[]>();
            }
        }

        private void SetWaternetExtremeChartData(MacroStabilityInwardsWaternet waternet)
        {
            if (!waternet.Equals(currentWaternetExtreme))
            {
                currentWaternetExtreme = waternet;
                SetWaternetZonesChartData(waternet, waternetZonesExtremeChartData,
                                          phreaticLineExtremeLookup, waternetLineExtremeLookup);
            }
        }

        private void SetWaternetDailyChartData(MacroStabilityInwardsWaternet waternet)
        {
            if (!waternet.Equals(currentWaternetDaily))
            {
                currentWaternetDaily = waternet;
                SetWaternetZonesChartData(waternet, waternetZonesDailyChartData,
                                          phreaticLineDailyLookup, waternetLineDailyLookup);
            }
        }

        private static void SetWaternetZonesChartData(MacroStabilityInwardsWaternet waternet, ChartDataCollection chartData,
                                                      IDictionary<MacroStabilityInwardsPhreaticLine, ChartLineData> phreaticLineLookup,
                                                      IDictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> waternetLineLookup)
        {
            chartData.Clear();
            phreaticLineLookup.Clear();
            waternetLineLookup.Clear();

            foreach (MacroStabilityInwardsPhreaticLine phreaticLine in waternet.PhreaticLines)
            {
                ChartLineData phreaticLineChartData = MacroStabilityInwardsChartDataFactory.CreatePhreaticLineChartData(phreaticLine.Name, false);
                chartData.Add(phreaticLineChartData);
                phreaticLineLookup.Add(phreaticLine, phreaticLineChartData);
            }

            foreach (MacroStabilityInwardsWaternetLine waternetLine in waternet.WaternetLines)
            {
                ChartMultipleAreaData waternetLineChartData = MacroStabilityInwardsChartDataFactory.CreateWaternetZoneChartData(waternetLine.Name, false);
                chartData.Add(waternetLineChartData);
                waternetLineLookup.Add(waternetLine, waternetLineChartData);
            }
        }

        private RoundedDouble GetEffectiveAssessmentLevel()
        {
            return data.InputParameters.UseAssessmentLevelManualInput
                       ? data.InputParameters.AssessmentLevel
                       : getNormativeAssessmentLevelFunc();
        }
    }
}