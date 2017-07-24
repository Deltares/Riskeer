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
using Core.Common.Utils.Extensions;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view to show the macro stability inwards input.
    /// </summary>
    public partial class MacroStabilityInwardsInputView : UserControl, IChartView
    {
        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;

        private readonly ChartDataCollection chartDataCollection;
        private readonly ChartDataCollection soilProfileChartData;
        private readonly ChartLineData surfaceLineChartData;
        private readonly ChartPointData surfaceLevelInsideChartData;
        private readonly ChartPointData ditchPolderSideChartData;
        private readonly ChartPointData bottomDitchPolderSideChartData;
        private readonly ChartPointData bottomDitchDikeSideChartData;
        private readonly ChartPointData ditchDikeSideChartData;
        private readonly ChartPointData dikeToeAtPolderChartData;
        private readonly ChartPointData shoulderTopInsideChartData;
        private readonly ChartPointData shoulderBaseInsideChartData;
        private readonly ChartPointData trafficLoadInsideChartData;
        private readonly ChartPointData dikeTopAtPolderChartData;
        private readonly ChartPointData trafficLoadOutsideChartData;
        private readonly ChartPointData dikeToeAtRiverChartData;
        private readonly ChartPointData surfaceLevelOutsideChartData;

        private readonly List<ChartMultipleAreaData> soilLayerChartDataLookup;

        private MacroStabilityInwardsCalculationScenario data;

        private MacroStabilityInwardsSoilProfile currentSoilProfile;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsInputView"/>.
        /// </summary>
        public MacroStabilityInwardsInputView()
        {
            InitializeComponent();

            calculationObserver = new Observer(UpdateChartTitle);
            calculationInputObserver = new Observer(UpdateViewData);

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.Calculation_Input);
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
            trafficLoadInsideChartData = MacroStabilityInwardsChartDataFactory.CreateTrafficLoadInsideChartData();
            dikeTopAtPolderChartData = MacroStabilityInwardsChartDataFactory.CreateDikeTopAtPolderChartData();
            trafficLoadOutsideChartData = MacroStabilityInwardsChartDataFactory.CreateTrafficLoadOutsideChartData();
            dikeToeAtRiverChartData = RingtoetsChartDataFactory.CreateDikeToeAtRiverChartData();
            surfaceLevelOutsideChartData = MacroStabilityInwardsChartDataFactory.CreateSurfaceLevelOutsideChartData();

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
            chartDataCollection.Add(trafficLoadInsideChartData);
            chartDataCollection.Add(dikeTopAtPolderChartData);
            chartDataCollection.Add(trafficLoadOutsideChartData);
            chartDataCollection.Add(dikeToeAtRiverChartData);
            chartDataCollection.Add(surfaceLevelOutsideChartData);

            soilLayerChartDataLookup = new List<ChartMultipleAreaData>(); // Use lookup because the ordering in the chart data collection might change
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
                UpdateTableData();
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

        private void UpdateChartTitle()
        {
            chartControl.ChartTitle = data.Name;
        }

        private void UpdateViewData()
        {
            UpdateChartData();
            UpdateTableData();
        }

        private void UpdateChartData()
        {
            SetChartData();

            chartDataCollection.Collection.ForEachElementDo(cd => cd.NotifyObservers());
            soilProfileChartData.Collection.ForEachElementDo(cd => cd.NotifyObservers());
        }

        private void UpdateTableData()
        {
            soilLayerTable.SetData(data?.InputParameters.StochasticSoilProfile?.SoilProfile?.Layers);
        }

        private void SetChartData()
        {
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = data.InputParameters.SurfaceLine;

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
            trafficLoadInsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadInsidePoint(surfaceLine);
            dikeTopAtPolderChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtPolderPoint(surfaceLine);
            trafficLoadOutsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadOutsidePoint(surfaceLine);
            dikeToeAtRiverChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);
            surfaceLevelOutsideChartData.Points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelOutsidePoint(surfaceLine);

            SetSoilProfileChartData();
        }

        private void SetSoilProfileChartData()
        {
            MacroStabilityInwardsSoilProfile soilProfile = data.InputParameters.StochasticSoilProfile?.SoilProfile;

            // If necessary, regenerate all soil layer chart data
            if (!ReferenceEquals(currentSoilProfile, soilProfile))
            {
                currentSoilProfile = soilProfile;

                soilProfileChartData.Clear();
                soilLayerChartDataLookup.Clear();
                GetSoilLayers().Select((layer, layerIndex) => MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(layerIndex, currentSoilProfile))
                               .ForEachElementDo(sl =>
                               {
                                   soilProfileChartData.Insert(0, sl);
                                   soilLayerChartDataLookup.Add(sl);
                               });

                MacroStabilityInwardsChartDataFactory.UpdateSoilProfileChartDataName(soilProfileChartData, currentSoilProfile);
            }

            // Update the areas of all soil layer chart data
            IList<MacroStabilityInwardsSoilLayer> soilLayers = GetSoilLayers();

            for (var i = 0; i < soilLayers.Count; i++)
            {
                ChartMultipleAreaData soilLayerData = soilLayerChartDataLookup[i];

                soilLayerData.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayers[i], currentSoilProfile, data.InputParameters.SurfaceLine);
            }
        }

        private IList<MacroStabilityInwardsSoilLayer> GetSoilLayers()
        {
            return data?.InputParameters.StochasticSoilProfile?.SoilProfile?.Layers.ToList() ?? new List<MacroStabilityInwardsSoilLayer>();
        }
    }
}