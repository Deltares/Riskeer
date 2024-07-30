// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
    /// Wrapper class to link an <see cref="ISoilProfile"/> with a <see cref="Schema.FailureMechanismType"/>.
    /// </summary>
    /// <typeparam name="TSoilProfile">The type of soil profile to wrap.</typeparam>
    public class SoilProfileWrapper<TSoilProfile>
        where TSoilProfile : class, ISoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilProfileWrapper{T}"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to wrap.</param>
        /// <param name="failureMechanismType">The <see cref="Schema.FailureMechanismType"/> the soil profile is associated with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public SoilProfileWrapper(TSoilProfile soilProfile, FailureMechanismType failureMechanismType)
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
        public TSoilProfile SoilProfile { get; }

        /// <summary>
        /// Gets the <see cref="Schema.FailureMechanismType"/>.
        /// </summary>
        public FailureMechanismType FailureMechanismType { get; }
    }
}