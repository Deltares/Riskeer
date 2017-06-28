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
    /// Extension methods for <see cref="RealizedStochast"/> related to creating an instance of 
    /// <see cref="HydraulicLocationRealizedStochastEntity"/>.
    /// </summary>
    internal static class RealizedStochastCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicLocationRealizedStochastEntity"/> based on the information 
        /// of the <paramref name="realizedStochast"/>.
        /// </summary>
        /// <param name="realizedStochast">The stochast to create a database entity for.</param>
        /// <param name="order">The index at which <paramref name="realizedStochast"/> resides within its parent.</param>
        /// <returns>A new <see cref="HydraulicLocationRealizedStochastEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="realizedStochast"/> 
        /// is <c>null</c>.</exception>
        public static HydraulicLocationRealizedStochastEntity CreateHydraulicLocationRealizedStochastEntity(
            this RealizedStochast realizedStochast, int order)
        {
            if (realizedStochast == null)
            {
                throw new ArgumentNullException(nameof(realizedStochast));
            }

            var entity = new HydraulicLocationRealizedStochastEntity
            {
                Name = realizedStochast.Name.DeepClone(),
                Alpha = realizedStochast.Alpha,
                Duration = realizedStochast.Duration,
                Realization = realizedStochast.Realization,
                Order = order
            };

            return entity;
        }

        /// <summary>
        /// Creates a <see cref="GrassCoverErosionOutwardsHydraulicLocationRealizedStochastEntity"/> based 
        /// on the information of the <paramref name="realizedStochast"/>.
        /// </summary>
        /// <param name="realizedStochast">The stochast to create a database entity for.</param>
        /// <param name="order">The index at which <paramref name="realizedStochast"/> resides within its parent.</param>
        /// <returns>A new <see cref="GrassCoverErosionOutwardsHydraulicLocationRealizedStochastEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="realizedStochast"/> 
        /// is <c>null</c>.</exception>
        public static GrassCoverErosionOutwardsHydraulicLocationRealizedStochastEntity CreateGrassCoverErosionOutwardsHydraulicLocationRealizedStochastEntity(
            this RealizedStochast realizedStochast, int order)
        {
            if (realizedStochast == null)
            {
                throw new ArgumentNullException(nameof(realizedStochast));
            }

            var entity = new GrassCoverErosionOutwardsHydraulicLocationRealizedStochastEntity
            {
                Name = realizedStochast.Name.DeepClone(),
                Alpha = realizedStochast.Alpha,
                Duration = realizedStochast.Duration,
                Realization = realizedStochast.Realization,
                Order = order
            };

            return entity;
        }
    }
}