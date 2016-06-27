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
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view to show the grass cover erosion inwards input.
    /// </summary>
    public partial class GrassCoverErosionInwardsInputView : UserControl, IChartView, IObserver
    {
        private GrassCoverErosionInwardsInput data;
        private GrassCoverErosionInwardsCalculation calculation;
        private ChartData dikeProfileData;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInputView"/>.
        /// </summary>
        public GrassCoverErosionInwardsInputView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the calculation the input belongs to.
        /// </summary>
        public GrassCoverErosionInwardsCalculation Calculation
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
                var newValue = value as GrassCoverErosionInwardsInput;

                DetachFromData();
                data = newValue;

                if (data == null)
                {
                    Chart.ResetChartData();
                    return;
                }

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

        private void SetDataToChart()
        {
            chartControl.Data.Name = Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName;

            if (data != null)
            {
                // Bottom most layer
                dikeProfileData = AddOrUpdateChartData(dikeProfileData, GetDikeProfileData());
                // Top most layer
            }

            chartControl.Data.NotifyObservers();
        }

        private ChartData GetDikeProfileData()
        {
            if (data == null || data.DikeProfile == null)
            {
                return ChartDataFactory.CreateEmptyLineData(Resources.DikeProfile_DisplayName);
            }
            return GrassCoverErosionInwardsChartDataFactory.Create(data.DikeProfile);
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

        private void SetChartTitle()
        {
            chartControl.ChartTitle = calculation != null ? calculation.Name : string.Empty;
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