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
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms.Views
{
    /// <summary>
    /// This class is a view to show the wave conditions input.
    /// </summary>
    public partial class WaveConditionsInputView : UserControl, IChartView
    {
        private readonly ChartDataCollection chartDataCollection;
        private readonly ChartLineData foreshoreChartData;

        private IWaveConditionsCalculation data;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInputView"/>.
        /// </summary>
        public WaveConditionsInputView()
        {
            InitializeComponent();

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.Calculation_Input);
            foreshoreChartData = RingtoetsChartDataFactory.CreateForeshoreGeometryChartData();

            chartDataCollection.Add(foreshoreChartData);
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

                if (data != null)
                {
                    SetChartData();

                    chartControl.Data = chartDataCollection;
                    chartControl.ChartTitle = data.Name;
                }
            }
        }

        private void SetChartData()
        {
            WaveConditionsInput input = data.InputParameters;

            WaveConditionsChartDataFactory.UpdateForeshoreGeometryChartDataName(foreshoreChartData, input);

            foreshoreChartData.Points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }
    }
}