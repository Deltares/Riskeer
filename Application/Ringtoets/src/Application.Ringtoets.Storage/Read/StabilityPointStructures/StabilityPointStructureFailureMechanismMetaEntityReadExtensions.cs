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

using Application.Ringtoets.Storage.DbContext;
using Ringtoets.StabilityPointStructures.Data;

namespace Application.Ringtoets.Storage.Read.StabilityPointStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralStabilityPointStructuresInput"/>
    /// based on the <see cref="StabilityPointStructuresFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class StabilityPointStructureFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="StabilityPointStructuresFailureMechanismMetaEntity"/> and use the
        /// information to construct a <see cref="GeneralStabilityPointStructuresInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructuresFailureMechanismMetaEntity"/>
        /// to create <see cref="GeneralStabilityPointStructuresInput"/> for.</param>
        /// <returns>A new <see cref="GeneralStabilityPointStructuresInput"/>.</returns>
        internal static GeneralStabilityPointStructuresInput Read(this StabilityPointStructuresFailureMechanismMetaEntity entity)
        {
            return new GeneralStabilityPointStructuresInput
            {
                N = entity.N
            };
        }
    }
}