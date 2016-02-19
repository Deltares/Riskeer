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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="PipingCalculation"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class PipingCalculationContext : PipingContext<PipingCalculation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationContext"/> class.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="soilProfiles">The soil profiles available within the piping context.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism which the piping context belongs to.</param>
        public PipingCalculationContext(PipingCalculation calculation,
                                        IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines,
                                        IEnumerable<PipingSoilProfile> soilProfiles,
                                        PipingFailureMechanism pipingFailureMechanism)
            : base(calculation, surfaceLines, soilProfiles)
        {
            if (pipingFailureMechanism == null)
            {
                var message = String.Format(Resources.PipingContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.PipingContext_DataDescription_PipingFailureMechanism);
                throw new ArgumentNullException("pipingFailureMechanism", message);
            }

            PipingFailureMechanism = pipingFailureMechanism;
        }

        /// <summary>
        /// Gets the piping failure mechanism which the piping context belongs to.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism { get; private set; }
    }
}