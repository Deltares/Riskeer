﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingStochasticSoilProfile"/> 
    /// based on the <see cref="PipingStochasticSoilProfileEntity"/>.
    /// </summary>
    internal static class PipingStochasticSoilProfileEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="PipingStochasticSoilProfileEntity"/> and use the information to 
        /// construct a <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingStochasticSoilProfileEntity"/> to create 
        /// <see cref="PipingStochasticSoilProfile"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingStochasticSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static PipingStochasticSoilProfile Read(this PipingStochasticSoilProfileEntity entity,
                                                         ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            PipingSoilProfile soilProfile = entity.ReadSoilProfile(collector);
            var stochasticSoilProfile = new PipingStochasticSoilProfile(entity.Probability, soilProfile);

            collector.Read(entity, stochasticSoilProfile);

            return stochasticSoilProfile;
        }

        private static PipingSoilProfile ReadSoilProfile(this PipingStochasticSoilProfileEntity entity,
                                                         ReadConversionCollector collector)
        {
            return entity.PipingSoilProfileEntity.Read(collector);
        }
    }
}