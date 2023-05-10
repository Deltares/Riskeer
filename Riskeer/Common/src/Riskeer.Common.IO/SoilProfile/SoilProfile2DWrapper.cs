// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// Wrapper class to link the <see cref="SoilProfile2D"/> with the <see cref="Schema.FailureMechanismType"/>.
    /// </summary>
    public class SoilProfile2DWrapper
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile2DWrapper"/>.
        /// </summary>
        /// <param name="soilProfile">The <see cref="SoilProfile2D"/> to wrap.</param>
        /// <param name="failureMechanismType">The <see cref="Schema.FailureMechanismType"/> the soil profile is associated with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public SoilProfile2DWrapper(SoilProfile2D soilProfile, FailureMechanismType failureMechanismType)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            SoilProfile = soilProfile;
            FailureMechanismType = failureMechanismType;
        }

        /// <summary>
        /// Gets the wrapped soil profile.
        /// </summary>
        public SoilProfile2D SoilProfile { get; }

        /// <summary>
        /// Gets the <see cref="Schema.FailureMechanismType"/>.
        /// </summary>
        public FailureMechanismType FailureMechanismType { get; }
    }
}