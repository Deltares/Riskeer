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
using System.Linq;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Forms.ChangeHandlers
{
    /// <summary>
    /// Class which can, if required, inquire the user for a confirmation when a change to the
    /// <see cref="PipingFailureMechanism"/> requires calculation results to be altered.
    /// </summary>
    public class PipingFailureMechanismCalculationChangeHandler : FailureMechanismCalculationChangeHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismCalculationChangeHandler"/>.
        /// </summary>
        /// <param name="failureMechanism"><see cref="PipingFailureMechanism"/> for which to handle changes.</param>
        /// <param name="query">The query which should be displayed when inquiring for a confirmation.</param>
        /// <param name="inquiryHandler">Object responsible for inquiring required data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public PipingFailureMechanismCalculationChangeHandler(PipingFailureMechanism failureMechanism,
                                                              string query, IInquiryHelper inquiryHandler)
            : base(failureMechanism, query, inquiryHandler) {}

        public override bool RequireConfirmation()
        {
            return FailureMechanism.Calculations
                                   .OfType<ProbabilisticPipingCalculationScenario>()
                                   .Any(c => c.HasOutput);
        }
    }
}