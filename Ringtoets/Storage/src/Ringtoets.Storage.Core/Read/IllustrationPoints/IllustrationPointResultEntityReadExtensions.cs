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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="IllustrationPointResultEntity"/> related to creating an 
    /// <see cref="IllustrationPointResult"/>.
    /// </summary>
    internal static class IllustrationPointResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="IllustrationPointResultEntity"/> and uses the information to 
        /// construct an <see cref="IllustrationPointResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IllustrationPointResultEntity"/> to create 
        /// a <see cref="IllustrationPointResult"/> for.</param>
        /// <returns>A new <see cref="IllustrationPointResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal static IllustrationPointResult Read(this IllustrationPointResultEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new IllustrationPointResult(entity.Description,
                                               entity.Value);
        }
    }
}