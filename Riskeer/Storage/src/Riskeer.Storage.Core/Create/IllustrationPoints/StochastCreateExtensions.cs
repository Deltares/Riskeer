﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="Stochast"/> related to creating an instance of <see cref="StochastEntity"/>.
    /// </summary>
    internal static class StochastCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StochastEntity"/> based on the information 
        /// of the <paramref name="stochast"/>.
        /// </summary>
        /// <param name="stochast">The stochast to create a database entity for.</param>
        /// <param name="order">The index at which <paramref name="stochast"/> resides within its parent.</param>
        /// <returns>A new <see cref="StochastEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stochast"/> 
        /// is <c>null</c>.</exception>
        internal static StochastEntity Create(
            this Stochast stochast, int order)
        {
            if (stochast == null)
            {
                throw new ArgumentNullException(nameof(stochast));
            }

            var entity = new StochastEntity
            {
                Name = stochast.Name.DeepClone(),
                Alpha = stochast.Alpha,
                Duration = stochast.Duration,
                Order = order
            };

            return entity;
        }
    }
}