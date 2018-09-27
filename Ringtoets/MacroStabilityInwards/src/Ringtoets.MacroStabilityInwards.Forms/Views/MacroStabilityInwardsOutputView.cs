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

using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Components.Chart.Forms;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class is a view to show the macro stability inwards output.
    /// </summary>
    public partial class MacroStabilityInwardsOutputView : UserControl, IChartView
    {
        private readonly Observer calculationObserver;
        private readonly Observer inputObserver;

        private readonly Func<RoundedDouble> getNormativeAssessmentLevelFunc;

        private MacroStabilityInwardsCalculationScenario data;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutputView"/>.
        /// </summary>
        /// <param name="data">The calculation to show the output for.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{TResult}"/> for obtaining the normative assessment level.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsOutputView(MacroStabilityInwardsCalculationScenario data,
                                               Func<RoundedDouble> getNormativeAssessmentLevelFunc)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (getNormativeAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormativeAssessmentLevelFunc));
            }

            this.data = data;
            this.getNormativeAssessmentLevelFunc = getNormativeAssessmentLevelFunc;

            InitializeComponent();

            calculationObserver = new Observer(UpdateViewData)
            {
                Observable = data
            };

            inputObserver = new Observer(UpdateViewData)
            {
                Observable = data.InputParameters
            };

            UpdateTableData();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as MacroStabilityInwardsCalculationScenario;
            }
        }

        public IChartControl Chart
        {
            get
            {
                return macroStabilityInwardsOutputChartControl.Chart;
            }
        }

        protected override void Dispose(bool disposing)
        {
            calculationObserver.Dispose();
            inputObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateViewData()
        {
            macroStabilityInwardsOutputChartControl.UpdateChartData();
            UpdateTableData();
        }

        private void UpdateTableData()
        {
            slicesTable.SetData(data?.Output?.SlidingCurve.Slices);
        }
    }
}