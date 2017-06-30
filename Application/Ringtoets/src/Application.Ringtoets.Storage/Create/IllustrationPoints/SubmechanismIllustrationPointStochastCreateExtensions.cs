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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;

namespace Application.Ringtoets.Storage.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="SubmechanismIllustrationPointStochast"/> related to creating an instance of 
    /// <see cref="SubmechanismIllustrationPointStochastEntity"/>.
    /// </summary>
    internal static class SubmechanismIllustrationPointStochastCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SubmechanismIllustrationPointStochastEntity"/> based on the information 
        /// of the <paramref name="submechanismIllustrationPointStochast"/>.
        /// </summary>
        /// <param name="submechanismIllustrationPointStochast">The stochast to create a database entity for.</param>
        /// <param name="order">The index at which <paramref name="submechanismIllustrationPointStochast"/> resides within its parent.</param>
        /// <returns>A new <see cref="SubmechanismIllustrationPointStochastEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="submechanismIllustrationPointStochast"/> 
        /// is <c>null</c>.</exception>
        public static SubmechanismIllustrationPointStochastEntity CreateHydraulicLocationRealizedStochastEntity(
            this SubmechanismIllustrationPointStochast submechanismIllustrationPointStochast, int order)
        {
            if (submechanismIllustrationPointStochast == null)
            {
                throw new ArgumentNullException(nameof(submechanismIllustrationPointStochast));
            }

            var entity = new SubmechanismIllustrationPointStochastEntity
            {
                Name = submechanismIllustrationPointStochast.Name.DeepClone(),
                Alpha = submechanismIllustrationPointStochast.Alpha,
                Duration = submechanismIllustrationPointStochast.Duration,
                Realization = submechanismIllustrationPointStochast.Realization,
                Order = order
            };

            return entity;
        }
    }
}