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
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
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

        private readonly ChartLineData foreshoreChartData;
        private readonly ChartLineData dikeGeometryChartData;
        private readonly ChartLineData dikeHeightChartData;

        private GrassCoverErosionInwardsCalculation data;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInputView"/>.
        /// </summary>
        public GrassCoverErosionInwardsInputView()
        {
            InitializeComponent();

            calculationObserver = new Observer(UpdateChartTitle);
            calculationInputObserver = new Observer(UpdateChartData);

            foreshoreChartData = GrassCoverErosionInwardsChartDataFactory.CreateForeshoreGeometryChartData();
            dikeGeometryChartData = GrassCoverErosionInwardsChartDataFactory.CreateDikeGeometryChartData();
            dikeHeightChartData = GrassCoverErosionInwardsChartDataFactory.CreateDikeHeightChartData();

            chartControl.Data.Add(foreshoreChartData);
            chartControl.Data.Add(dikeGeometryChartData);
            chartControl.Data.Add(dikeHeightChartData);

            chartControl.Data.Name = Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName;
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
            var input = data != null ? data.InputParameters : null;
            var dikeProfile = input != null ? input.DikeProfile : null;

            GrassCoverErosionInwardsChartDataFactory.UpdateForeshoreGeometryChartDataName(foreshoreChartData, input);
            GrassCoverErosionInwardsChartDataFactory.UpdateDikeGeometryChartDataName(dikeGeometryChartData, dikeProfile);

            UpdatePointBasedChartData(foreshoreChartData, GrassCoverErosionInwardsChartDataPointsFactory.CreateForeshoreGeometryPoints(input));
            UpdatePointBasedChartData(dikeGeometryChartData, GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeGeometryPoints(dikeProfile));
            UpdatePointBasedChartData(dikeHeightChartData, GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeHeightPoints(input));

            chartControl.Data.NotifyObservers();
        }

        private static void UpdatePointBasedChartData(PointBasedChartData chartData, Point2D[] points)
        {
            chartData.Points = points;
            chartData.NotifyObservers();
        }
    }
}