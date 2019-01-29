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

using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Util.Extensions;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.Factories;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Factories;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// This class is a view to show the grass cover erosion inwards input.
    /// </summary>
    public partial class GrassCoverErosionInwardsInputView : UserControl, IChartView
    {
        private readonly Observer calculationObserver;
        private readonly Observer calculationInputObserver;

        private readonly ChartLineData foreshoreChartData;
        private readonly ChartLineData dikeGeometryChartData;
        private readonly ChartLineData dikeHeightChartData;
        private readonly ChartDataCollection chartDataCollection;

        private GrassCoverErosionInwardsCalculation data;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInputView"/>.
        /// </summary>
        public GrassCoverErosionInwardsInputView()
        {
            InitializeComponent();

            calculationObserver = new Observer(UpdateChartTitle);
            calculationInputObserver = new Observer(UpdateChartData);

            chartDataCollection = new ChartDataCollection(RingtoetsCommonFormsResources.Calculation_Input);
            foreshoreChartData = RingtoetsChartDataFactory.CreateForeshoreGeometryChartData();
            dikeGeometryChartData = GrassCoverErosionInwardsChartDataFactory.CreateDikeGeometryChartData();
            dikeHeightChartData = GrassCoverErosionInwardsChartDataFactory.CreateDikeHeightChartData();

            chartDataCollection.Add(foreshoreChartData);
            chartDataCollection.Add(dikeGeometryChartData);
            chartDataCollection.Add(dikeHeightChartData);
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

        private void UpdateChartData()
        {
            SetChartData();

            chartDataCollection.Collection.ForEachElementDo(cd => cd.NotifyObservers());
        }

        private void SetChartData()
        {
            GrassCoverErosionInwardsInput input = data.InputParameters;
            DikeProfile dikeProfile = input.DikeProfile;

            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(foreshoreChartData, input);
            GrassCoverErosionInwardsChartDataFactory.UpdateDikeGeometryChartDataName(dikeGeometryChartData, dikeProfile);

            foreshoreChartData.Points = GrassCoverErosionInwardsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);
            dikeGeometryChartData.Points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeGeometryPoints(dikeProfile);
            dikeHeightChartData.Points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeHeightPoints(input);
        }
    }
}