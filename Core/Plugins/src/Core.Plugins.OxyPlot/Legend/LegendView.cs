// Copyright (C) Stichting Deltares 2016. All rights preserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class defines a view which shows the data that have been added to a <see cref="BaseChart"/>.
    /// </summary>
    public sealed partial class LegendView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="LegendView"/>.
        /// </summary>
        public LegendView()
        {
            InitializeComponent();
            Text = Resources.General_Chart;
        }

        public object Data
        {
            get
            {
                return seriesTree.ChartData;
            }
            set
            {
                UpdateTree((ChartData) value);
            }
        }

        /// <summary>
        /// Updates the tree with the current state of the chart.
        /// </summary>
        private void UpdateTree(ChartData data)
        {
            if (IsDisposed)
            {
                return;
            }
            seriesTree.ChartData = data;
        }
    }
}