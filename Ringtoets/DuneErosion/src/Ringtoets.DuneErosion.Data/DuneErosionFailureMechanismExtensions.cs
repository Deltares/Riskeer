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

namespace Ringtoets.DuneErosion.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="DuneErosionFailureMechanism"/> instances.
    /// </summary>
    public static class DuneErosionFailureMechanismExtensions
    {
        /// <summary>
        /// Gets the norm which is needed in the calculations within <see cref="DuneErosionFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> to get the failure mechanism norm for.</param>
        /// <param name="norm">The assessment section norm.</param>
        /// <returns>The value of the failure mechanism norm.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetMechanismSpecificNorm(this DuneErosionFailureMechanism failureMechanism, double norm)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return 2.15 * (failureMechanism.Contribution / 100)
                   * norm / failureMechanism.GeneralInput.N;
        }
    }
}