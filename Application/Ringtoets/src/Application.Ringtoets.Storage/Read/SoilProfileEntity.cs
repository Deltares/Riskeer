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
using System.Linq;
using Application.Ringtoets.Storage.Read;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This partial class describes the read operation for a <see cref="PipingSoilProfile"/> based on the
    /// <see cref="SoilProfileEntity"/>.
    /// </summary>
    public partial class SoilProfileEntity
    {
        /// <summary>
        /// Reads the <see cref="SoilProfileEntity"/> and use the information to construct a <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingSoilProfile"/> or one from the <paramref name="collector"/> if the 
        /// <see cref="SoilProfileEntity"/> has been read before.</returns>
        public PipingSoilProfile Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            if (collector.Contains(this))
            {
                return collector.Get(this);
            }
            var layers = SoilLayerEntities.Select(sl => sl.Read());
            var pipingSoilProfile = new PipingSoilProfile(Name, Convert.ToDouble(Bottom), layers, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = SoilProfileEntityId
            };
            collector.Read(this, pipingSoilProfile);
            return pipingSoilProfile;
        }
    }
}