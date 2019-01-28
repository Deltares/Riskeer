// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Extensions;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Factories;
using Ringtoets.Piping.Primitives;
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
        private readonly ChartDataCollection chartDataCollection;

        private readonly List<ChartMultipleAreaData> soilLayerChartDataLookup;

        private PipingCalculationScenario data;

        private PipingSoilProfile currentSoilProfile;

        /// <summary>
        /// Creates a new instance of <see cref="PipingInputView"/>.
        /// </summary>
        public PipingInputView()
        {
            InitializeComponent();

            calculationObserver = new Observer(UpdateChartTitle);
            calculationInputObserver = new Observer(UpdateViewData);

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.Calculation_Input);
            soilProfileChartData = RingtoetsChartDataFactory.CreateSoilProfileChartData();
            surfaceLineChartData = RingtoetsChartDataFactory.CreateSurfaceLineChartData();
            ditchPolderSideChartData = RingtoetsChartDataFactory.CreateDitchPolderSideChartData();
            bottomDitchPolderSideChartData = RingtoetsChartDataFactory.CreateBottomDitchPolderSideChartData();
            bottomDitchDikeSideChartData = RingtoetsChartDataFactory.CreateBottomDitchDikeSideChartData();
            ditchDikeSideChartData = RingtoetsChartDataFactory.CreateDitchDikeSideChartData();
            dikeToeAtPolderChartData = RingtoetsChartDataFactory.CreateDikeToeAtPolderChartData();
            dikeToeAtRiverChartData = RingtoetsChartDataFactory.CreateDikeToeAtRiverChartData();
            exitPointChartData = PipingChartDataFactory.CreateExitPointChartData();
            entryPointChartData = PipingChartDataFactory.CreateEntryPointChartData();

            chartDataCollection.Add(soilProfileChartData);
            chartDataCollection.Add(surfaceLineChartData);
            chartDataCollection.Add(ditchPolderSideChartData);
            chartDataCollection.Add(bottomDitchPolderSideChartData);
            chartDataCollection.Add(bottomDitchDikeSideChartData);
            chartDataCollection.Add(ditchDikeSideChartData);
            chartDataCollection.Add(dikeToeAtPolderChartData);
            chartDataCollection.Add(dikeToeAtRiverChartData);
            chartDataCollection.Add(exitPointChartData);
            chartDataCollection.Add(entryPointChartData);

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
                data = value as PipingCalculationScenario;

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
            soilProfileChartData.Collection.ForEachElementDo(md => md.NotifyObservers());
        }

        private void UpdateTableData()
        {
            pipingSoilLayerTable.SetData(data?.InputParameters.StochasticSoilProfile?.SoilProfile.Layers);
        }

        private void SetChartData()
        {
            PipingInput pipingInput = data.InputParameters;
            PipingSurfaceLine surfaceLine = data.InputParameters.SurfaceLine;

            PipingChartDataFactory.UpdateSurfaceLineChartDataName(surfaceLineChartData, surfaceLine);

            surfaceLineChartData.Points = PipingChartDataPointsFactory.CreateSurfaceLinePoints(surfaceLine);
            ditchPolderSideChartData.Points = PipingChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine);
            bottomDitchPolderSideChartData.Points = PipingChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine);
            bottomDitchDikeSideChartData.Points = PipingChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine);
            ditchDikeSideChartData.Points = PipingChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine);
            dikeToeAtPolderChartData.Points = PipingChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);
            dikeToeAtRiverChartData.Points = PipingChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);
            exitPointChartData.Points = PipingChartDataPointsFactory.CreateExitPointPoint(pipingInput);
            entryPointChartData.Points = PipingChartDataPointsFactory.CreateEntryPointPoint(pipingInput);

            SetSoilProfileChartData();
        }

        private void SetSoilProfileChartData()
        {
            PipingSoilProfile soilProfile = data.InputParameters.StochasticSoilProfile?.SoilProfile;

            // If necessary, regenerate all soil layer chart data
            if (!ReferenceEquals(currentSoilProfile, soilProfile))
            {
                currentSoilProfile = soilProfile;

                soilProfileChartData.Clear();
                soilLayerChartDataLookup.Clear();
                GetSoilLayers().Select(PipingChartDataFactory.CreateSoilLayerChartData)
                               .ForEachElementDo(sl =>
                               {
                                   soilProfileChartData.Insert(0, sl);
                                   soilLayerChartDataLookup.Add(sl);
                               });

                PipingChartDataFactory.UpdateSoilProfileChartDataName(soilProfileChartData, currentSoilProfile);
            }

            // Update the areas of all soil layer chart data
            IEnumerable<PipingSoilLayer> soilLayers = GetSoilLayers();

            for (var i = 0; i < soilLayers.Count(); i++)
            {
                ChartMultipleAreaData soilLayerData = soilLayerChartDataLookup[i];

                soilLayerData.Areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayers.ElementAt(i), currentSoilProfile, data.InputParameters.SurfaceLine);
            }
        }

        private IEnumerable<PipingSoilLayer> GetSoilLayers()
        {
            return data?.InputParameters.StochasticSoilProfile?.SoilProfile.Layers.ToArray() ?? new PipingSoilLayer[0];
        }
    }
}