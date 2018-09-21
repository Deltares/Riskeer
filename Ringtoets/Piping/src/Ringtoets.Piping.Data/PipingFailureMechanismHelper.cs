// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class containing helper methods for <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public static class PipingFailureMechanismHelper
    {
        /// <summary>
        /// Determines if the failure mechanism has section assembly results that are manually overwritten.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/>.</param>
        /// <returns><c>true</c> if the failure mechanism contains section assembly results that are manually overwritten,
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static bool HasManualAssemblyResults(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.SectionResults.Any(sr => sr.UseManualAssembly);
        }
    }
}