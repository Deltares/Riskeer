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

using System.Windows.Forms;
using Core.Common.Base;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view to show the piping input.
    /// </summary>
    public partial class PipingInputView : UserControl, IChartView, IObserver
    {
        private PipingInput data;
        private PipingCalculationScenario calculation;

        private ChartData surfaceLineData;
        private ChartData entryPointData;
        private ChartData exitPointData;
        private ChartData ditchPolderSideData;
        private ChartData bottomDitchPolderSidePointData;
        private ChartData bottomDitchDikeSidePointData;
        private ChartData ditchDikeSidePointData;
        private ChartData dikeToeAtRiverPointData;
        private ChartData dikeToeAtPolderPointData;

        /// <summary>
        /// Creates a new instance of <see cref="PipingInputView"/>.
        /// </summary>
        public PipingInputView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the calculation the input belongs to.
        /// </summary>
        public PipingCalculationScenario Calculation
        {
            get
            {
                return calculation;
            }
            set
            {
                DetachFromData();
                calculation = value;
                SetChartTitle();
                AttachToData();
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
                var newValue = value as PipingInput;

                DetachFromData();
                data = newValue;
                SetDataToChart();
                AttachToData();
            }
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }

        public void UpdateObserver()
        {
            SetChartTitle();
            SetDataToChart();
        }

        private void SetChartTitle()
        {
            if (calculation != null)
            {
                chartControl.ChartTitle = calculation.Name;
            }
        }
        
        private void SetDataToChart()
        {
            chartControl.Data.Name = Resources.PipingInputContext_NodeDisplayName;

            if (data != null)
            {
                // Bottom most layer
                surfaceLineData = AddOrUpdateChartData(surfaceLineData, GetSurfaceLineChartData());
                ditchPolderSideData = AddOrUpdateChartData(ditchPolderSideData, GetDitchPolderSideData());
                bottomDitchPolderSidePointData = AddOrUpdateChartData(bottomDitchPolderSidePointData, GetBottomDitchPolderSideData());
                bottomDitchDikeSidePointData = AddOrUpdateChartData(bottomDitchDikeSidePointData, GetBottomDitchDikeSideData());
                ditchDikeSidePointData = AddOrUpdateChartData(ditchDikeSidePointData, GetDitchDikeSideData());
                dikeToeAtRiverPointData = AddOrUpdateChartData(dikeToeAtRiverPointData, GetDikeToeAtRiverData());
                dikeToeAtPolderPointData = AddOrUpdateChartData(dikeToeAtPolderPointData, GetDikeToeAtPolderData());
                exitPointData = AddOrUpdateChartData(exitPointData, GetExitPointChartData());
                entryPointData = AddOrUpdateChartData(entryPointData, GetEntryPointChartData());
                // Top most layer
            }

            chartControl.Data.NotifyObservers();
        }

        private ChartData GetSurfaceLineChartData()
        {
            if (data == null || data.SurfaceLine == null)
            {
                return PipingChartDataFactory.CreateEmptyLineData(Resources.RingtoetsPipingSurfaceLine_DisplayName);
            }
            return PipingChartDataFactory.Create(data.SurfaceLine);
        }

        private ChartData GetEntryPointChartData()
        {
            if (data == null || data.SurfaceLine == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(Resources.PipingInput_EntryPointL_DisplayName);
            }
            return PipingChartDataFactory.CreateEntryPoint(data.EntryPointL, data.SurfaceLine);
        }

        private ChartData GetExitPointChartData()
        {
            if (data == null || data.SurfaceLine == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(Resources.PipingInput_ExitPointL_DisplayName);
            }
            return PipingChartDataFactory.CreateExitPoint(data.ExitPointL, data.SurfaceLine);
        }

        private ChartData GetDitchPolderSideData()
        {
            if (data == null || data.SurfaceLine == null || data.SurfaceLine.DitchPolderSide == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DitchPolderSide);
            }
            return PipingChartDataFactory.CreateDitchPolderSide(data.SurfaceLine);
        }

        private ChartData GetBottomDitchPolderSideData()
        {
            if (data == null || data.SurfaceLine == null || data.SurfaceLine.BottomDitchPolderSide == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_BottomDitchPolderSide);
            }
            return PipingChartDataFactory.CreateBottomDitchPolderSide(data.SurfaceLine);
        }

        private ChartData GetBottomDitchDikeSideData()
        {
            if (data == null || data.SurfaceLine == null || data.SurfaceLine.BottomDitchDikeSide == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_BottomDitchDikeSide);
            }
            return PipingChartDataFactory.CreateBottomDitchDikeSide(data.SurfaceLine);
        }

        private ChartData GetDitchDikeSideData()
        {
            if (data == null || data.SurfaceLine == null || data.SurfaceLine.DitchDikeSide == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DitchDikeSide);
            }
            return PipingChartDataFactory.CreateDitchDikeSide(data.SurfaceLine);
        }

        private ChartData GetDikeToeAtRiverData()
        {
            if (data == null || data.SurfaceLine == null || data.SurfaceLine.DikeToeAtRiver == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DikeToeAtRiver);
            }
            return PipingChartDataFactory.CreateDikeToeAtRiver(data.SurfaceLine);
        }

        private ChartData GetDikeToeAtPolderData()
        {
            if (data == null || data.SurfaceLine == null || data.SurfaceLine.DikeToeAtPolder == null)
            {
                return PipingChartDataFactory.CreateEmptyPointData(PipingDataResources.CharacteristicPoint_DikeToeAtPolder);
            }
            return PipingChartDataFactory.CreateDikeToeAtPolder(data.SurfaceLine);
        }

        private ChartData AddOrUpdateChartData(ChartData oldChartData, ChartData newChartData)
        {
            if (oldChartData != null)
            {
                chartControl.Data.Remove(oldChartData);
            }
            if (newChartData != null)
            {
                chartControl.Data.Add(newChartData);
            }

            return newChartData;
        }

        private void DetachFromData()
        {
            if (calculation != null)
            {
                calculation.Detach(this);
            }

            if (data != null)
            {
                data.Detach(this);
            }
        }

        private void AttachToData()
        {
            if (calculation != null)
            {
                calculation.Attach(this);
            }

            if (data != null)
            {
                data.Attach(this);
            }
        }
    }
}