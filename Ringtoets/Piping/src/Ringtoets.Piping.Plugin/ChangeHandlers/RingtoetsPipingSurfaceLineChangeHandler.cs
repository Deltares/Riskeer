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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui;
using Ringtoets.Common.IO;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.Properties;

namespace Ringtoets.Piping.Plugin.ChangeHandlers
{
    /// <summary>
    /// Class which can, if required, inquire the user for a confirmation when a change to the
    /// surface line collection requires calculation results to be altered.
    /// </summary>
    public class RingtoetsPipingSurfaceLineChangeHandler : IConfirmDataChangeHandler
    {
        private readonly IInquiryHelper inquiryHandler;
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsPipingSurfaceLineChangeHandler"/>.
        /// </summary>
        /// <param name="failureMechanism">Failure mechanism for which to handle changes in stochastic soil models.</param>
        /// <param name="inquiryHandler">Object responsible for inquiring required data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public RingtoetsPipingSurfaceLineChangeHandler(PipingFailureMechanism failureMechanism, IInquiryHelper inquiryHandler)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (inquiryHandler == null)
            {
                throw new ArgumentNullException(nameof(inquiryHandler));
            }

            this.failureMechanism = failureMechanism;
            this.inquiryHandler = inquiryHandler;
        }

        public bool RequireConfirmation()
        {
            IEnumerable<PipingCalculation> calculations = failureMechanism.Calculations.Cast<PipingCalculation>();

            return calculations.Any(HasOutput);
        }

        public bool InquireConfirmation()
        {
            return inquiryHandler.InquireContinuation(
                Resources.RingtoetsPipingSurfaceLineChangeHandler_InquireConfirmation_When_updating_RingtoetsSurfaceLines_definitions_assigned_to_calculations_output_will_be_cleared_confirm);
        }

        private static bool HasOutput(PipingCalculation calculation)
        {
            return calculation.HasOutput;
        }
    }
}