// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="IllustrationPointResult"/> related to creating an instance 
    /// of <see cref="IllustrationPointResultEntity"/>.
    /// </summary>
    internal static class IllustrationPointResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="IllustrationPointResult"/> based on the 
        /// information of the <paramref name="illustrationPointResult"/>.
        /// </summary>
        /// <param name="illustrationPointResult">The illustration point result to create a database 
        /// entity for.</param>
        /// <param name="order">The index at which <paramref name="illustrationPointResult"/> 
        /// resides within its parent.</param>
        /// <returns>A new <see cref="IllustrationPointResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="illustrationPointResult"/> 
        /// is <c>null</c>.</exception>
        internal static IllustrationPointResultEntity Create(
            this IllustrationPointResult illustrationPointResult, int order)
        {
            if (illustrationPointResult == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointResult));
            }

            var entity = new IllustrationPointResultEntity
            {
                Description = illustrationPointResult.Description.DeepClone(),
                Value = illustrationPointResult.Value,
                Order = order
            };

            return entity;
        }
    }
}