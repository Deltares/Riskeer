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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GeneralGrassCoverErosionInwardsInput"/> related
    /// to creating a <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class GeneralGrassCoverErosionInwardsInputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/> based
        /// on the information of the <see cref="GeneralGrassCoverErosionInwardsInput"/>.
        /// </summary>
        /// <param name="input">The general calculation input for Grass Cover Erosion Inwards
        /// to create a database entity for.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsFailureMechanismMetaEntity Create(this GeneralGrassCoverErosionInwardsInput input)
        {
            var entity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                N = input.N
            };
            return entity;
        }
    }
}