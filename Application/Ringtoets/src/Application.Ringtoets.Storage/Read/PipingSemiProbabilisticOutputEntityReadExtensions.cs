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

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingSemiProbabilisticOutput"/>
    /// based on the <see cref="PipingSemiProbabilisticOutputEntity"/>.
    /// </summary>
    internal static class PipingSemiProbabilisticOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="PipingSemiProbabilisticOutputEntity"/> and use the information
        /// to construct a <see cref="PipingSemiProbabilisticOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSemiProbabilisticOutputEntity"/> to
        /// create <see cref="PipingSemiProbabilisticOutput"/> for.</param>
        /// <returns>A new <see cref="PipingSemiProbabilisticOutput"/>.</returns>
        internal static PipingSemiProbabilisticOutput Read(this PipingSemiProbabilisticOutputEntity entity)
        {
            return new PipingSemiProbabilisticOutput(
                GetNanDouble(entity.UpliftFactorOfSafety), GetNanDouble(entity.UpliftReliability), GetNanDouble(entity.UpliftProbability),
                GetNanDouble(entity.HeaveFactorOfSafety), GetNanDouble(entity.HeaveReliability), GetNanDouble(entity.HeaveProbability),
                GetNanDouble(entity.SellmeijerFactorOfSafety), GetNanDouble(entity.SellmeijerReliability), GetNanDouble(entity.SellmeijerProbability),
                GetNanDouble(entity.RequiredProbability), GetNanDouble(entity.RequiredReliability),
                GetNanDouble(entity.PipingProbability), GetNanDouble(entity.PipingReliability), GetNanDouble(entity.PipingFactorOfSafety))
            {
                StorageId = entity.PipingSemiProbabilisticOutputEntityId
            };
        }

        private static double GetNanDouble(decimal? parameterValue)
        {
            if (parameterValue == null)
            {
                return double.NaN;
            }
            return Convert.ToDouble(parameterValue);
        }
    }
}