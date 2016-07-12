﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralGrassCoverErosionInwardsInput"/>
    /// based on the <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/>
        /// and use the information to construct a <see cref="GeneralGrassCoverErosionInwardsInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/>
        /// to create <see cref="GeneralGrassCoverErosionInwardsInput"/> for.</param>
        /// <returns>A new <see cref="GeneralGrassCoverErosionInwardsInput"/>.</returns>
        internal static GeneralGrassCoverErosionInwardsInput Read(this GrassCoverErosionInwardsFailureMechanismMetaEntity entity)
        {
            return new GeneralGrassCoverErosionInwardsInput
            {
                N = entity.N
            };
        }
    }
}