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

        private readonly List<ChartMultipleAreaData> soilLayerChartDataLookup;
        private MacroStabilityInwardsCalculationScenario data;
        private IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> currentSoilProfile;

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

            soilLayerChartDataLookup = new List<ChartMultipleAreaData>();

            Chart.Data = chartDataCollection;
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

                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates the chart data.
        /// </summary>
        public void UpdateChartData()
        {
            if (data?.Output != null)
            {
                SetChartData();
            }
            else
            {
                SetChartDataEmpty();
            }

            chartDataCollection.Collection.ForEachElementDo(cd => cd.NotifyObservers());
            soilProfileChartData.Collection.ForEachElementDo(sp => sp.NotifyObservers());
        }

        private void SetChartDataEmpty()
        {
            IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile = data?.InputParameters.StochasticSoilProfile?.SoilProfile;

            SetSurfaceLineChartData(null);
            SetSoilProfileChartData(null, soilProfile);
        }

        private void SetChartData()
        {
            MacroStabilityInwardsInput macroStabilityInwardsInput = data.InputParameters;
            MacroStabilityInwardsSurfaceLine surfaceLine = macroStabilityInwardsInput.SurfaceLine;
            IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile = macroStabilityInwardsInput.StochasticSoilProfile?.SoilProfile;

            SetSurfaceLineChartData(surfaceLine);
            SetSoilProfileChartData(surfaceLine, soilProfile);
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

        private void SetSoilProfileChartData(MacroStabilityInwardsSurfaceLine surfaceLine,
                                             IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile)
        {
            if (!ReferenceEquals(currentSoilProfile, soilProfile))
            {
                currentSoilProfile = soilProfile;

                SetSoilProfileChartData(soilProfile);
            }

            if (soilProfile != null)
            {
                if (surfaceLine != null)
                {
                    SetSoilLayerAreas();
                }
                else
                {
                    SetEmptySoilLayerAreas();
                }
            }
        }

        private void SetSoilProfileChartData(IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile)
        {
            soilProfileChartData.Clear();
            soilLayerChartDataLookup.Clear();
            GetSoilLayers().Select((layer, layerIndex) => MacroStabilityInwardsChartDataFactory.CreateSoilLayerChartData(layerIndex, soilProfile))
                           .ForEachElementDo(sl =>
                           {
                               soilProfileChartData.Insert(0, sl);
                               soilLayerChartDataLookup.Add(sl);
                           });

            ChartMultipleAreaData holesChartData = MacroStabilityInwardsChartDataFactory.CreateHolesChartData();
            soilProfileChartData.Insert(soilLayerChartDataLookup.Count, holesChartData);
            soilLayerChartDataLookup.Add(holesChartData);

            MacroStabilityInwardsChartDataFactory.UpdateSoilProfileChartDataName(soilProfileChartData, soilProfile);
        }

        private void SetSoilLayerAreas()
        {
            IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfileUnderSurfaceLine = data.InputParameters.SoilProfileUnderSurfaceLine;
            var i = 0;
            foreach (IMacroStabilityInwardsSoilLayerUnderSurfaceLine soilLayer in soilProfileUnderSurfaceLine.Layers)
            {
                ChartMultipleAreaData soilLayerData = soilLayerChartDataLookup[i++];
                soilLayerData.Areas = MacroStabilityInwardsChartDataPointsFactory.CreateOuterRingArea(soilLayer);
            }

            soilLayerChartDataLookup.Last().Areas = MacroStabilityInwardsChartDataPointsFactory.CreateHolesAreas(soilProfileUnderSurfaceLine);
        }

        private void SetEmptySoilLayerAreas()
        {
            foreach (ChartMultipleAreaData soilLayerData in soilLayerChartDataLookup)
            {
                soilLayerData.Areas = Enumerable.Empty<Point2D[]>();
            }
        }

        private IEnumerable<IMacroStabilityInwardsSoilLayer> GetSoilLayers()
        {
            return data?.InputParameters.StochasticSoilProfile?.SoilProfile.Layers.ToList() ?? new List<IMacroStabilityInwardsSoilLayer>();
        }
    }
}