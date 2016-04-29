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
using Application.Ringtoets.Storage.Read;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This partial class describes the read operation for a <see cref="StochasticSoilProfile"/> based on the
    /// <see cref="StochasticSoilProfileEntity"/>.
    /// </summary>
    public partial class StochasticSoilProfileEntity
    {
        /// <summary>
        /// Reads the <see cref="StochasticSoilProfileEntity"/> and use the information to construct a <see cref="StochasticSoilProfile"/>.
        /// </summary>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StochasticSoilProfile"/>.</returns>
        public StochasticSoilProfile Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var profile = new StochasticSoilProfile(Convert.ToDouble(Probability), SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = StochasticSoilProfileEntityId
            };
            ReadSoilProfile(profile, collector);

            return profile;
        }

        private void ReadSoilProfile(StochasticSoilProfile profile, ReadConversionCollector collector)
        {
            profile.SoilProfile = SoilProfileEntity.Read(collector);
        }
    }
}