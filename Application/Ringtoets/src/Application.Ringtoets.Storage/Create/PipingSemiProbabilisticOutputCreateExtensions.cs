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
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="PipingSemiProbabilisticOutput"/> related to creating 
    /// a <see cref="PipingSemiProbabilisticOutputEntity"/>.
    /// </summary>
    internal static class PipingSemiProbabilisticOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingSemiProbabilisticOutputEntity"/> based on the information
        /// of the <see cref="PipingSemiProbabilisticOutput"/>.
        /// </summary>
        /// <param name="semiProbabilisticOutput">The semi-probabilistic calculation output
        /// for piping to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="PipingSemiProbabilisticOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static PipingSemiProbabilisticOutputEntity Create(this PipingSemiProbabilisticOutput semiProbabilisticOutput, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var entity = new PipingSemiProbabilisticOutputEntity
            {
                UpliftFactorOfSafety = semiProbabilisticOutput.UpliftFactorOfSafety.Value.ToNullableDecimal(),
                UpliftReliability = semiProbabilisticOutput.UpliftReliability.Value.ToNullableDecimal(),
                UpliftProbability = semiProbabilisticOutput.UpliftProbability.ToNullableDecimal(),
                HeaveFactorOfSafety = semiProbabilisticOutput.HeaveFactorOfSafety.Value.ToNullableDecimal(),
                HeaveReliability = semiProbabilisticOutput.HeaveReliability.Value.ToNullableDecimal(),
                HeaveProbability = semiProbabilisticOutput.HeaveProbability.ToNullableDecimal(),
                SellmeijerFactorOfSafety = semiProbabilisticOutput.SellmeijerFactorOfSafety.Value.ToNullableDecimal(),
                SellmeijerReliability = semiProbabilisticOutput.SellmeijerReliability.Value.ToNullableDecimal(),
                SellmeijerProbability = semiProbabilisticOutput.SellmeijerProbability.ToNullableDecimal(),
                RequiredProbability = semiProbabilisticOutput.RequiredProbability.ToNullableDecimal(),
                RequiredReliability = semiProbabilisticOutput.RequiredReliability.Value.ToNullableDecimal(),
                PipingProbability = semiProbabilisticOutput.PipingProbability.ToNullableDecimal(),
                PipingReliability = semiProbabilisticOutput.PipingReliability.Value.ToNullableDecimal(),
                PipingFactorOfSafety = semiProbabilisticOutput.PipingFactorOfSafety.Value.ToNullableDecimal(),
            };
            registry.Register(entity, semiProbabilisticOutput);
            return entity;
        }
    }
}