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

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This partial class describes the read operation for a <see cref="StochasticSoilModel"/> based on the
    /// <see cref="StochasticSoilModelEntity"/>.
    /// </summary>
    public partial class StochasticSoilModelEntity
    {
        /// <summary>
        /// Reads the <see cref="StochasticSoilModelEntity"/> and use the information to construct a <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StochasticSoilModel"/>.</returns>
        public StochasticSoilModel Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var model = new StochasticSoilModel(-1, Name, SegmentName)
            {
                StorageId = StochasticSoilModelEntityId
            };
            ReadStochasticSoilProfiles(model, collector);

            return model;
        }

        private void ReadStochasticSoilProfiles(StochasticSoilModel model, ReadConversionCollector collector)
        {
            foreach (var stochasticSoilProfileEntity in StochasticSoilProfileEntities)
            {
                model.StochasticSoilProfiles.Add(stochasticSoilProfileEntity.Read(collector));
            }
        }
    }
}