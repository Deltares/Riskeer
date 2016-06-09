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

using Core.Common.Base.Data;

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
                UpliftFactorOfSafety = GetNullableDecimal(semiProbabilisticOutput.UpliftFactorOfSafety),
                UpliftReliability = GetNullableDecimal(semiProbabilisticOutput.UpliftReliability),
                UpliftProbability = GetNullableDecimal(semiProbabilisticOutput.UpliftProbability),
                HeaveFactorOfSafety = GetNullableDecimal(semiProbabilisticOutput.HeaveFactorOfSafety),
                HeaveReliability = GetNullableDecimal(semiProbabilisticOutput.HeaveReliability),
                HeaveProbability = GetNullableDecimal(semiProbabilisticOutput.HeaveProbability),
                SellmeijerFactorOfSafety = GetNullableDecimal(semiProbabilisticOutput.SellmeijerFactorOfSafety),
                SellmeijerReliability = GetNullableDecimal(semiProbabilisticOutput.SellmeijerReliability),
                SellmeijerProbability = GetNullableDecimal(semiProbabilisticOutput.SellmeijerProbability),
                RequiredProbability = GetNullableDecimal(semiProbabilisticOutput.RequiredProbability),
                RequiredReliability = GetNullableDecimal(semiProbabilisticOutput.RequiredReliability),
                PipingProbability = GetNullableDecimal(semiProbabilisticOutput.PipingProbability),
                PipingReliability = GetNullableDecimal(semiProbabilisticOutput.PipingReliability),
                PipingFactorOfSafety = GetNullableDecimal(semiProbabilisticOutput.PipingFactorOfSafety),
            };
            registry.Register(entity, semiProbabilisticOutput);
            return entity;
        }

        private static decimal? GetNullableDecimal(RoundedDouble parameterValue)
        {
            if (double.IsNaN(parameterValue))
            {
                return null;
            }
            return Convert.ToDecimal(parameterValue);
        }
    }
}