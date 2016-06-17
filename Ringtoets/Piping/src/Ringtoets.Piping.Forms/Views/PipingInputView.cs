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
using Core.Components.Charting.Forms;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class is a view to show the piping input.
    /// </summary>
    public partial class PipingInputView : UserControl, IChartView
    {
        private PipingInput data;
        private PipingCalculationScenario calculation;

        /// <summary>
        /// Creates a new instance of <see cref="PipingInputView"/>.
        /// </summary>
        public PipingInputView()
        {
            InitializeComponent();

            SetChartAxisTitles();
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
                calculation = value;

                SetChartTitle();
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
                data = value as PipingInput;
            }
        }

        public IChartControl Chart
        {
            get
            {
                return chartControl;
            }
        }

        private void SetChartTitle()
        {
            chartControl.SetChartTitle(calculation.Name);
        }

        private void SetChartAxisTitles()
        {
            chartControl.SetBottomAxisTitle(Resources.PipingInputView_Distance_DisplayName);
            chartControl.SetLeftAxisTitle(Resources.PipingInputView_Height_DisplayName);
        }
    }
}