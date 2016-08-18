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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Extensions;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view to show the piping input.
    /// </summary>
    public partial class PipingInputView : UserControl, IChartView
    {
        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;

        private readonly ChartDataCollection soilProfileChartData;
        private readonly ChartLineData surfaceLineChartData;
        private readonly ChartPointData ditchPolderSideChartData;
        private readonly ChartPointData bottomDitchPolderSideChartData;
        private readonly ChartPointData bottomDitchDikeSideChartData;
        private readonly ChartPointData ditchDikeSideChartData;
        private readonly ChartPointData dikeToeAtPolderChartData;
        private readonly ChartPointData dikeToeAtRiverChartData;
        private readonly ChartPointData exitPointChartData;
        private readonly ChartPointData entryPointChartData;

        private readonly List<ChartMultipleAreaData> soilLayerChartDataLookup;

        private PipingCalculationScenario data;

        private StochasticSoilProfile currentStochasticSoilProfile;

        /// <summary>
        /// Creates a new instance of <see cref="PipingInputView"/>.
        /// </summary>
        public PipingInputView()
        {
            InitializeComponent();
            InitializeChartControl();

            calculationObserver = new Observer(UpdateChartTitle);
            calculationInputObserver = new Observer(UpdateChartData);

            soilProfileChartData = PipingChartDataFactory.CreateSoilProfileChartData();
            surfaceLineChartData = PipingChartDataFactory.CreateSurfaceLineChartData();
            ditchPolderSideChartData = PipingChartDataFactory.CreateDitchPolderSideChartData();
            bottomDitchPolderSideChartData = PipingChartDataFactory.CreateBottomDitchPolderSideChartData();
            bottomDitchDikeSideChartData = PipingChartDataFactory.CreateBottomDitchDikeSideChartData();
            ditchDikeSideChartData = PipingChartDataFactory.CreateDitchDikeSideChartData();
            dikeToeAtPolderChartData = PipingChartDataFactory.CreateDikeToeAtPolderChartData();
            dikeToeAtRiverChartData = PipingChartDataFactory.CreateDikeToeAtRiverChartData();
            exitPointChartData = PipingChartDataFactory.CreateExitPointChartData();
            entryPointChartData = PipingChartDataFactory.CreateEntryPointChartData();

            chartControl.Data.Add(soilProfileChartData);
            chartControl.Data.Add(surfaceLineChartData);
            chartControl.Data.Add(ditchPolderSideChartData);
            chartControl.Data.Add(bottomDitchPolderSideChartData);
            chartControl.Data.Add(bottomDitchDikeSideChartData);
            chartControl.Data.Add(ditchDikeSideChartData);
            chartControl.Data.Add(dikeToeAtPolderChartData);
            chartControl.Data.Add(dikeToeAtRiverChartData);
            chartControl.Data.Add(exitPointChartData);
            chartControl.Data.Add(entryPointChartData);

            chartControl.Data.Name = Resources.PipingInputContext_NodeDisplayName;

            soilLayerChartDataLookup = new List<ChartMultipleAreaData>(); // Use lookup because the ordering in the chart data collection might change
        }

        private void InitializeChartControl()
        {
            chartControl.LeftAxisTitle = RingtoetsCommonFormsResources.InputView_Height_DisplayName;
            chartControl.BottomAxisTitle = RingtoetsCommonFormsResources.InputView_Distance_DisplayName;
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as PipingCalculationScenario;

                calculationObserver.Observable = data;
                calculationInputObserver.Observable = GetPipingInput();

                if (data == null)
                {
                    Chart.ResetChartData();
                    return;
                }

                UpdateChartTitle();
                UpdateChartData();
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

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateChartTitle()
        {
            chartControl.ChartTitle = data != null ? data.Name : string.Empty;
        }

        private void UpdateChartData()
        {
            var pipingInput = GetPipingInput();
            var surfaceLine = GetSurfaceLine();

            PipingChartDataFactory.UpdateSurfaceLineChartDataName(surfaceLineChartData, surfaceLine);

            UpdatePointBasedChartData(surfaceLineChartData,
                                      PipingChartDataPointsFactory.CreateSurfaceLinePoints(surfaceLine));
            UpdatePointBasedChartData(ditchPolderSideChartData,
                                      PipingChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine));
            UpdatePointBasedChartData(bottomDitchPolderSideChartData,
                                      PipingChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine));
            UpdatePointBasedChartData(bottomDitchDikeSideChartData,
                                      PipingChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine));
            UpdatePointBasedChartData(ditchDikeSideChartData,
                                      PipingChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine));
            UpdatePointBasedChartData(dikeToeAtPolderChartData,
                                      PipingChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine));
            UpdatePointBasedChartData(dikeToeAtRiverChartData,
                                      PipingChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine));
            UpdatePointBasedChartData(exitPointChartData,
                                      PipingChartDataPointsFactory.CreateExitPointPoint(pipingInput));
            UpdatePointBasedChartData(entryPointChartData,
                                      PipingChartDataPointsFactory.CreateEntryPointPoint(pipingInput));

            UpdateSoilProfileChartData();

            chartControl.Data.NotifyObservers();
        }

        private static void UpdatePointBasedChartData(PointBasedChartData chartData, Point2D[] points)
        {
            chartData.Points = points;
            chartData.NotifyObservers();
        }

        private void UpdateSoilProfileChartData()
        {
            var stochasticSoilProfile = GetStochasticSoilProfile();

            // If necessary, regenerate all soil layer chart data
            if (!ReferenceEquals(currentStochasticSoilProfile, stochasticSoilProfile))
            {
                currentStochasticSoilProfile = stochasticSoilProfile;

                soilProfileChartData.Clear();
                soilLayerChartDataLookup.Clear();
                GetSoilLayers().Select((layer, layerIndex) => PipingChartDataFactory.CreateSoilLayerChartData(layerIndex, stochasticSoilProfile.SoilProfile))
                               .ForEachElementDo(sl =>
                               {
                                   soilProfileChartData.Insert(0, sl);
                                   soilLayerChartDataLookup.Add(sl);
                               });

                PipingChartDataFactory.UpdateSoilProfileChartDataName(soilProfileChartData, stochasticSoilProfile);

                soilProfileChartData.NotifyObservers();
            }

            // Update the areas of all soil layer chart data
            var soilLayers = GetSoilLayers();

            for (var i = 0; i < soilLayers.Count; i++)
            {
                var soilLayerData = soilLayerChartDataLookup[i];

                soilLayerData.Areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayers[i], stochasticSoilProfile.SoilProfile, GetSurfaceLine());
                soilLayerData.NotifyObservers();
            }
        }

        private RingtoetsPipingSurfaceLine GetSurfaceLine()
        {
            return data != null
                       ? data.InputParameters.SurfaceLine
                       : null;
        }

        private PipingInput GetPipingInput()
        {
            return data != null
                       ? data.InputParameters
                       : null;
        }

        private StochasticSoilProfile GetStochasticSoilProfile()
        {
            return data != null
                       ? data.InputParameters.StochasticSoilProfile
                       : null;
        }

        private IList<PipingSoilLayer> GetSoilLayers()
        {
            return data != null && data.InputParameters.StochasticSoilProfile != null && data.InputParameters.StochasticSoilProfile.SoilProfile != null
                       ? data.InputParameters.StochasticSoilProfile.SoilProfile.Layers.ToList()
                       : new List<PipingSoilLayer>();
        }
    }
}