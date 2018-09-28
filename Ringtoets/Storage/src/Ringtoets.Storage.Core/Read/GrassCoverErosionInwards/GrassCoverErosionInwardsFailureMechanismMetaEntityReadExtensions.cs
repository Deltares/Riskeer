// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.GrassCoverErosionInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralGrassCoverErosionInwardsInput"/>
    /// based on the <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/> and use the information
        /// to update the <paramref name="input"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/> to use
        /// to update the <paramref name="input"/>.</param>
        /// <param name="input">The <see cref="GeneralGrassCoverErosionInwardsInput"/> to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this GrassCoverErosionInwardsFailureMechanismMetaEntity entity, GeneralGrassCoverErosionInwardsInput input)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            input.N = (RoundedDouble) entity.N;
        }
    }
}