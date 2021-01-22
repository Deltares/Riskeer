// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using Core.Common.Util.Extensions;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Forms.ChangeHandlers
{
    /// <summary>
    /// Class for handling clearing the output of <see cref="ProbabilisticPipingCalculationScenario"/>.
    /// </summary>
    public class ClearProbabilisticPipingCalculationOutputChangeHandler : ClearCalculationOutputChangeHandlerBase<ProbabilisticPipingCalculationScenario>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClearProbabilisticPipingCalculationOutputChangeHandler"/>.
        /// </summary>
        /// <param name="calculations">The calculations to clear the output for.</param>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="viewCommands">The view commands used to close views for the calculation output.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearProbabilisticPipingCalculationOutputChangeHandler(
            IEnumerable<ProbabilisticPipingCalculationScenario> calculations,
            IInquiryHelper inquiryHelper, IViewCommands viewCommands)
            : base(calculations, inquiryHelper, viewCommands) {}

        protected override void DoPreUpdateActions()
        {
            Calculations.ForEachElementDo(calculation => ViewCommands.RemoveAllViewsForItem(calculation.Output));
        }
    }
}