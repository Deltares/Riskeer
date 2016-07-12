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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view to show the grass cover erosion inwards input.
    /// </summary>
    public partial class GrassCoverErosionInwardsInputView : UserControl, IChartView
    {
        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;
        private GrassCoverErosionInwardsCalculation data;
        private ChartData dikeProfileData;
        private ChartData foreshoreData;
        private ChartData dikeHeightData;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInputView"/>.
        /// </summary>
        public GrassCoverErosionInwardsInputView()
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
                data = value as GrassCoverErosionInwardsCalculation;

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
            chartControl.Data.Name = Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName;

            if (data != null)
            {
                // Bottom most layer
                foreshoreData = AddOrUpdateChartData(foreshoreData, GetForeshoreData());
                dikeProfileData = AddOrUpdateChartData(dikeProfileData, GetDikeProfileData());
                dikeHeightData = AddOrUpdateChartData(dikeHeightData, GetDikeHeightData());
                // Top most layer
            }

            chartControl.Data.NotifyObservers();
        }

        private ChartData GetForeshoreData()
        {
            if (!HasForeshorePoints())
            {
                return ChartDataFactory.CreateEmptyLineData(Resources.Foreshore_DisplayName);
            }

            return GrassCoverErosionInwardsChartDataFactory.Create(data.InputParameters.ForeshoreGeometry, data.InputParameters.DikeProfile.Name);
        }

        private ChartData GetDikeProfileData()
        {
            if (!HasDikeProfilePoints())
            {
                return ChartDataFactory.CreateEmptyLineData(Resources.DikeProfile_DisplayName);
            }

            return GrassCoverErosionInwardsChartDataFactory.Create(data.InputParameters.DikeGeometry, data.InputParameters.DikeProfile.Name);
        }

        private ChartData GetDikeHeightData()
        {
            if (!HasDikeProfilePoints())
            {
                return ChartDataFactory.CreateEmptyLineData(Resources.DikeHeight_ChartName);
            }

            return GrassCoverErosionInwardsChartDataFactory.Create(data.InputParameters.DikeHeight, data.InputParameters.DikeGeometry, data.InputParameters.DikeProfile.Name);
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

        private bool HasForeshorePoints()
        {
            return data != null && data.InputParameters.DikeProfile != null && data.InputParameters.ForeshoreGeometry.Any() && data.InputParameters.UseForeshore;
        }

        private bool HasDikeProfilePoints()
        {
            return data != null && data.InputParameters.DikeProfile != null && data.InputParameters.DikeGeometry.Any();
        }
    }
}