﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Components.Stack.Data;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Forms.Factories;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Control to show illustration points in a chart view.
    /// </summary>
    public partial class IllustrationPointsChartControl : UserControl
    {
        private GeneralResult data;

        private readonly StackChartData chartData;

        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointsChartControl"/>.
        /// </summary>
        public IllustrationPointsChartControl()
        {
            InitializeComponent();

            chartData = RingtoetsStackChartDataFactory.Create();
            stackChartControl.Data = chartData;
        }

        /// <summary>
        /// Gets or sets the data of the control.
        /// </summary>
        public GeneralResult Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data != null)
                {
                    chartData.Clear();
                }

                data = value;

                if (data != null)
                {
                    SetChartData();
                }

                chartData.NotifyObservers();
            }
        }

        private void SetChartData()
        {
            RingtoetsStackChartDataFactory.CreateColumns(data, chartData);
            RingtoetsStackChartDataFactory.CreateRows(data, chartData);
        }
    }
}