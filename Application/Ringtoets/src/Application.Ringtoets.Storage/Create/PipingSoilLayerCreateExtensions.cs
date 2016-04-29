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
using Application.Ringtoets.Storage.Create;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="PipingSoilLayer"/> related to creating a <see cref="SoilLayerEntity"/>.
    /// </summary>
    public static class SoilLayerCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SoilLayerEntity"/> based on the information of the <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="layer">The layer to create a database entity for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="SoilLayerEntity"/>.</returns>
        public static SoilLayerEntity Create(this PipingSoilLayer layer, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new SoilLayerEntity
            {
                IsAquifer = Convert.ToByte(layer.IsAquifer),
                Top = Convert.ToDecimal(layer.Top)
            };

            collector.Create(entity, layer);
            return entity;
        }
    }
}