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
using Ringtoets.ClosingStructures.Data;

namespace Application.Ringtoets.Storage.Read.ClosingStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralClosingStructuresInput"/>
    /// based on the <see cref="ClosingStructureFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class ClosingStructuresFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="ClosingStructureFailureMechanismMetaEntity"/> and use the
        /// information to construct a <see cref="GeneralClosingStructuresInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureFailureMechanismMetaEntity"/>
        /// to create <see cref="GeneralClosingStructuresInput"/> for.</param>
        /// <returns>A new <see cref="GeneralClosingStructuresInput"/>.</returns>
        internal static GeneralClosingStructuresInput Read(this ClosingStructureFailureMechanismMetaEntity entity)
        {
            return new GeneralClosingStructuresInput
            {
                N2A = entity.N2A
            };
        }
    }
}