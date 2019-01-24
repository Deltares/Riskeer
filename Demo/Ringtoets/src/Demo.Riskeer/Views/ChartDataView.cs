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
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;

namespace Demo.Riskeer.Views
{
    /// <summary>
    /// This class represents a simple view with a chart, to which data can be added.
    /// </summary>
    public partial class ChartDataView : UserControl, IChartView
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartDataView"/>.
        /// </summary>
        public ChartDataView()
        {
            InitializeComponent();
        }

        public object Data
        {
            get
            {
                return Chart.Data;
            }
            set
            {
                Chart.Data = value as ChartDataCollection;
            }
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