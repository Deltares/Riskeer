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
using Core.Common.Base.Data;
using Ringtoets.HeightStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.HeightStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralHeightStructuresInput"/>
    /// based on the <see cref="HeightStructuresFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class HeightStructuresFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="HeightStructuresFailureMechanismMetaEntity"/> and use the information to
        /// construct a <see cref="GeneralHeightStructuresInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructuresFailureMechanismMetaEntity"/> to create
        /// <see cref="GeneralHeightStructuresInput"/> for.</param>
        /// <returns>A new <see cref="GeneralHeightStructuresInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal static GeneralHeightStructuresInput Read(this HeightStructuresFailureMechanismMetaEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new GeneralHeightStructuresInput
            {
                N = (RoundedDouble) entity.N
            };
        }
    }
}