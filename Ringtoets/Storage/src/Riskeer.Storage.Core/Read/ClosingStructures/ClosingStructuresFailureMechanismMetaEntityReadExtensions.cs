// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.ClosingStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralClosingStructuresInput"/>
    /// based on the <see cref="ClosingStructuresFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class ClosingStructuresFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="ClosingStructuresFailureMechanismMetaEntity"/> and use the information to
        /// construct a <see cref="GeneralClosingStructuresInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructuresFailureMechanismMetaEntity"/> to create
        /// <see cref="GeneralClosingStructuresInput"/> for.</param>
        /// <returns>A new <see cref="GeneralClosingStructuresInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal static GeneralClosingStructuresInput Read(this ClosingStructuresFailureMechanismMetaEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new GeneralClosingStructuresInput
            {
                N2A = entity.N2A
            };
        }
    }
}