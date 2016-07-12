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
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view to show the piping input.
    /// </summary>
    public partial class PipingInputView : UserControl, IChartView
    {
        private PipingCalculationScenario data;

        private ChartData surfaceLineData;
        private ChartData entryPointData;
        private ChartData exitPointData;
        private ChartData ditchPolderSideData;
        private ChartData bottomDitchPolderSidePointData;
        private ChartData bottomDitchDikeSidePointData;
        private ChartData ditchDikeSidePointData;
        private ChartData dikeToeAtRiverPointData;
        private ChartData dikeToeAtPolderPointData;
        private ChartData soilProfile;

        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingInputView"/>.
        /// </summary>
        public PipingInputView()
        {
            InitializeComponent();

            calculationObserver = new Observer(SetChartTitle);
            calculationInputObserver = new Observer(SetDataToChart);
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
                calculationInputObserver.Observable = data != null ? data.InputParameters : null;

                if (data == null)
                {
                    Chart.ResetChartData();
                    return;
                }

                SetChartTitle();
                SetDataToChart();
            }
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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

        private void SetChartTitle()
        {
            chartControl.ChartTitle = data != null ? data.Name : string.Empty;
        }

        private void SetDataToChart()
        {
            chartControl.Data.Name = Resources.PipingInputContext_NodeDisplayName;

            if (data != null)
            {
                // Bottom most layer
                soilProfile = AddOrUpdateChartData(soilProfile, GetStochasticSoilProfileData());
                surfaceLineData = AddOrUpdateChartData(surfaceLineData, GetSurfaceLineChartData());
                ditchPolderSideData = AddOrUpdateChartData(ditchPolderSideData, GetDitchPolderSideData());
                bottomDitchPolderSidePointData = AddOrUpdateChartData(bottomDitchPolderSidePointData, GetBottomDitchPolderSideData());
                bottomDitchDikeSidePointData = AddOrUpdateChartData(bottomDitchDikeSidePointData, GetBottomDitchDikeSideData());
                ditchDikeSidePointData = AddOrUpdateChartData(ditchDikeSidePointData, GetDitchDikeSideData());
                dikeToeAtPolderPointData = AddOrUpdateChartData(dikeToeAtPolderPointData, GetDikeToeAtPolderData());
                dikeToeAtRiverPointData = AddOrUpdateChartData(dikeToeAtRiverPointData, GetDikeToeAtRiverData());
                exitPointData = AddOrUpdateChartData(exitPointData, GetExitPointChartData());
                entryPointData = AddOrUpdateChartData(entryPointData, GetEntryPointChartData());
                // Top most layer
            }

            chartControl.Data.NotifyObservers();
        }

        private ChartData GetSurfaceLineChartData()
        {
            if (HasSurfaceLine())
            {
                return ChartDataFactory.CreateEmptyLineData(Resources.RingtoetsPipingSurfaceLine_DisplayName);
            }
            return PipingChartDataFactory.Create(data.InputParameters.SurfaceLine);
        }

        private ChartData GetEntryPointChartData()
        {
            if (HasSurfaceLine())
            {
                return ChartDataFactory.CreateEmptyPointData(Resources.PipingInput_EntryPointL_DisplayName);
            }
            return PipingChartDataFactory.CreateEntryPoint(data.InputParameters.EntryPointL, data.InputParameters.SurfaceLine);
        }

        private ChartData GetExitPointChartData()
        {
            if (HasSurfaceLine())
            {
                return ChartDataFactory.CreateEmptyPointData(Resources.PipingInput_ExitPointL_DisplayName);
            }
            return PipingChartDataFactory.CreateExitPoint(data.InputParameters.ExitPointL, data.InputParameters.SurfaceLine);
        }

        private ChartData GetDitchPolderSideData()
        {
            if (HasSurfaceLine() || data.InputParameters.SurfaceLine.DitchPolderSide == null)
            {
                return ChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DitchPolderSide);
            }
            return PipingChartDataFactory.CreateDitchPolderSide(data.InputParameters.SurfaceLine);
        }

        private ChartData GetBottomDitchPolderSideData()
        {
            if (HasSurfaceLine() || data.InputParameters.SurfaceLine.BottomDitchPolderSide == null)
            {
                return ChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_BottomDitchPolderSide);
            }
            return PipingChartDataFactory.CreateBottomDitchPolderSide(data.InputParameters.SurfaceLine);
        }

        private ChartData GetBottomDitchDikeSideData()
        {
            if (HasSurfaceLine() || data.InputParameters.SurfaceLine.BottomDitchDikeSide == null)
            {
                return ChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_BottomDitchDikeSide);
            }
            return PipingChartDataFactory.CreateBottomDitchDikeSide(data.InputParameters.SurfaceLine);
        }

        private ChartData GetDitchDikeSideData()
        {
            if (HasSurfaceLine() || data.InputParameters.SurfaceLine.DitchDikeSide == null)
            {
                return ChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DitchDikeSide);
            }
            return PipingChartDataFactory.CreateDitchDikeSide(data.InputParameters.SurfaceLine);
        }

        private ChartData GetDikeToeAtRiverData()
        {
            if (HasSurfaceLine() || data.InputParameters.SurfaceLine.DikeToeAtRiver == null)
            {
                return ChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DikeToeAtRiver);
            }
            return PipingChartDataFactory.CreateDikeToeAtRiver(data.InputParameters.SurfaceLine);
        }

        private ChartData GetDikeToeAtPolderData()
        {
            if (HasSurfaceLine() || data.InputParameters.SurfaceLine.DikeToeAtPolder == null)
            {
                return ChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DikeToeAtPolder);
            }
            return PipingChartDataFactory.CreateDikeToeAtPolder(data.InputParameters.SurfaceLine);
        }

        private bool HasSurfaceLine()
        {
            return data == null || data.InputParameters.SurfaceLine == null;
        }

        private ChartData GetStochasticSoilProfileData()
        {
            if (data == null || data.InputParameters.StochasticSoilProfile == null || data.InputParameters.StochasticSoilProfile.SoilProfile == null)
            {
                return ChartDataFactory.CreateEmptyChartDataCollection(Resources.StochasticSoilProfileProperties_DisplayName);
            }
            var pipingSoilProfile = data.InputParameters.StochasticSoilProfile.SoilProfile;

            return new ChartDataCollection(pipingSoilProfile.Layers.Select((layer, layerIndex) => 
                PipingChartDataFactory.CreatePipingSoilLayer(
                    layerIndex, 
                    pipingSoilProfile,
                    data.InputParameters.SurfaceLine)).Reverse().ToList(), pipingSoilProfile.Name);
        }

        private ChartData AddOrUpdateChartData(ChartData oldChartData, ChartData newChartData)
        {
            if (oldChartData != null)
            {
                chartControl.Data.Replace(oldChartData, newChartData);
            }
            else
            {
                chartControl.Data.Add(newChartData);
            }

            return newChartData;
        }
    }
}