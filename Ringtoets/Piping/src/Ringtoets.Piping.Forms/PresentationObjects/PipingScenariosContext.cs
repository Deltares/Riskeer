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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for configuration of scenarios for the Piping failure mechanism.
    /// </summary>
    public class PipingScenariosContext : WrappedObjectContextBase<CalculationGroup>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingScenariosContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="CalculationGroup"/>.</param>
        /// <param name="failureMechanism">A <see cref="PipingFailureMechanism"/> forming the context.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingScenariosContext(CalculationGroup wrappedData, PipingFailureMechanism failureMechanism)
            : base(wrappedData)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            ParentFailureMechanism = failureMechanism;
        }

        public PipingFailureMechanism ParentFailureMechanism { get; }
    }
}