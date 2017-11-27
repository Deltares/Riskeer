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
using Core.Common.Base.Geometry;
using Core.Common.Utils.Extensions;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
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
        private readonly ChartMultipleAreaData sliceEffectiveStressDailyChartData;
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

        private readonly List<ChartMultipleAreaData> soilLayerChartDataLookup;

        private readonly IDictionary<MacroStabilityInwardsPhreaticLine, ChartLineData> phreaticLineExtremeLookup;
        private readonly IDictionary<MacroStabilityInwardsPhreaticLine, ChartLineData> phreaticLineDailyLookup;
        private readonly IDictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> waternetLineExtremeLookup;
        private readonly IDictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData> waternetLineDailyLookup;
        private MacroStabilityInwardsCalculationScenario data;
        private IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> currentSoilProfile;
        private MacroStabilityInwardsSurfaceLine currentSurfaceLine;

        private MacroStabilityInwardsWaternet currentWaternetExtreme;
        private MacroStabilityInwardsWaternet currentWaternetDaily;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutputChartControl"/>.
        /// </summary>
        public MacroStabilityInwardsOutputChartControl()
        {
            InitializeComponent();

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.CalculationOutput_DisplayName);
            soilProfileChartData = RingtoetsChartDataFactory.CreateSoilProfileChartData();
            surfaceLineChartData = RingtoetsChartDataFactory.CreateSurfaceLineChartData();
            surfaceLevelInsideChartData = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelInsideChartData();
            ditchPolderSideChartData = RingtoetsChartDataFactory.CreateDitchPolderSideChartData();
            bottomDitchPolderSideChartData = RingtoetsChartDataFactory.CreateBottomDitchPolderSideChartData();
            bottomDitchDikeSideChartData = RingtoetsChartDataFactory.CreateBottomDitchDikeSideChartData();
            ditchDikeSideChartData = RingtoetsChartDataFactory.CreateDitchDikeSideChartData();
            dikeToeAtPolderChartData = RingtoetsChartDataFactory.CreateDikeToeAtPolderChartData();
            shoulderTopInsideChartData = MacroStabilityInwardsChartDataFactory.CreateShoulderTopInsideChartData();
            shoulderBaseInsideChartData = MacroStabilityInwardsChartDataFactory.CreateShoulderBaseInsideChartData();
            dikeTopAtPolderChartData = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtPolderChartData();
            dikeToeAtRiverChartData = RingtoetsChartDataFactory.CreateDikeToeAtRiverChartData();
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
            slicesChartData = MacroStabilityInwardsChartDataFactory.CreateSlicesChartData();

            sliceParametersChartDataCollection = MacroStabilityInwardsChartDataFactory.CreateSliceParametersChartDataCollection();
            sliceCohesionChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.Cohesion_DisplayName);
            sliceEffectiveStressChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_EffectiveStress_DisplayName);
            sliceEffectiveStressDailyChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_EffectiveStressDaily_DisplayName);
            sliceTotalPorePressureChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_TotalPorePressure_DisplayName);
            sliceWeightChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_Weight_DisplayName);
            slicePiezometricPorePressureChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_PiezometricPorePressure_DisplayName);
            slicePorePressureChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_PorePressure_DisplayName);
            sliceVerticalPorePressureChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_VerticalPorePressure_DisplayName);
            sliceHorizontalPorePressureChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_HorizontalPorePressure_DisplayName);
            sliceOverConsolidationRatioChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_OverConsolidationRatio_DisplayName);
            slicePopChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.Pop_DisplayName);
            sliceNormalStressChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_NormalStress_DisplayName);
            sliceShearStressChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_ShearStress_DisplayName);
            sliceLoadStressChartData = MacroStabilityInwardsChartDataFactory.CreateSliceParameterChartData(
                Resources.MacroStabilityInwardsSlice_LoadStress_DisplayName);

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
            sliceParametersChartDataCollection.Add(sliceCohesionChartData);
            sliceParametersChartDataCollection.Add(sliceEffectiveStressChartData);
            sliceParametersChartDataCollection.Add(sliceEffectiveStressDailyChartData);
            sliceParametersChartDataCollection.Add(sliceTotalPorePressureChartData);
            sliceParametersChartDataCollection.Add(sliceWeightChartData);
            sliceParametersChartDataCollection.Add(slicePiezometricPorePressureChartData);
            sliceParametersChartDataCollection.Add(slicePorePressureChartData);
            sliceParametersChartDataCollection.Add(sliceVerticalPorePressureChartData);
            sliceParametersChartDataCollection.Add(sliceHorizontalPorePressureChartData);
            sliceParametersChartDataCollection.Add(sliceOverConsolidationRatioChartData);
            sliceParametersChartDataCollection.Add(slicePopChartData);
            sliceParametersChartDataCollection.Add(sliceNormalStressChartData);
            sliceParametersChartDataCollection.Add(sliceShearStressChartData);
            sliceParametersChartDataCollection.Add(sliceLoadStressChartData);

            soilLayerChartDataLookup = new List<ChartMultipleAreaData>();

            phreaticLineExtremeLookup = new Dictionary<MacroStabilityInwardsPhreaticLine, ChartLineData>();
            phreaticLineDailyLookup = new Dictionary<MacroStabilityInwardsPhreaticLine, ChartLineData>();
            waternetLineExtremeLookup = new Dictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData>();
            waternetLineDailyLookup = new Dictionary<MacroStabilityInwardsWaternetLine, ChartMultipleAreaData>();
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

                chartControl.Data = data != null ? chartDataCollection : null;
                UpdateChartData();
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
            slicesChartData.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateSliceAreas(output?.SlidingCurve.Slices);
            tangentLinesChartData.Lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(output?.SlipPlane.TangentLines,
                                                                                                         data.InputParameters.SurfaceLine);
            SetSliceParameterChartData(output);
        }

        private void SetSliceParameterChartData(MacroStabilityInwardsOutput output)
        {
            sliceCohesionChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.Cohesion), 8);
            sliceEffectiveStressChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.EffectiveStress), 8);
            sliceEffectiveStressDailyChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.EffectiveStressDaily), 8);
            sliceTotalPorePressureChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.TotalPorePressure), 8);
            sliceWeightChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.Weight), 8);
            slicePiezometricPorePressureChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.PiezometricPorePressure), 8);
            slicePorePressureChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.PorePressure), 8);
            sliceVerticalPorePressureChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.VerticalPorePressure), 8);
            sliceHorizontalPorePressureChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.HorizontalPorePressure), 8);
            sliceOverConsolidationRatioChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.OverConsolidationRatio), 20);
            slicePopChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.Pop), 8);
            sliceNormalStressChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.NormalStress), 8);
            sliceShearStressChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.ShearStress), 8);
            sliceLoadStressChartData.Areas =
                MacroStabilityInwardsChartDataPointsFactory.CreateSliceParameterAreas(output?.SlidingCurve,
                                                                                      nameof(MacroStabilityInwardsSlice.LoadStress), 8);
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

            SetWaternetExtremeChartData(input.WaternetExtreme);
            SetWaternetDailyChartData(input.WaternetDaily);

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
    }
}