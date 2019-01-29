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
using System.Linq;
using Core.Common.Gui;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO;

namespace Riskeer.Common.Forms.ChangeHandlers
{
    /// <summary>
    /// Class which can, if required, inquire the user for a confirmation when a change to the
    /// failure mechanism requires calculation results to be altered.
    /// </summary>
    public class FailureMechanismCalculationChangeHandler : IConfirmDataChangeHandler
    {
        private readonly string query;
        private readonly IFailureMechanism failureMechanism;
        private readonly IInquiryHelper inquiryHandler;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismCalculationChangeHandler"/>.
        /// </summary>
        /// <param name="failureMechanism">Failure mechanism for which to handle changes.</param>
        /// <param name="query">The query which should be displayed when inquiring for a confirmation.</param>
        /// <param name="inquiryHandler">Object responsible for inquiring required data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public FailureMechanismCalculationChangeHandler(IFailureMechanism failureMechanism,
                                                        string query,
                                                        IInquiryHelper inquiryHandler)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (inquiryHandler == null)
            {
                throw new ArgumentNullException(nameof(inquiryHandler));
            }

            this.failureMechanism = failureMechanism;
            this.query = query;
            this.inquiryHandler = inquiryHandler;
        }

        public bool RequireConfirmation()
        {
            return failureMechanism.Calculations.Any(calc => calc.HasOutput);
        }

        public bool InquireConfirmation()
        {
            return inquiryHandler.InquireContinuation(query);
        }
    }
}