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
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This partial class describes the read operation for a <see cref="HydraulicBoundaryLocation"/> based on the
    /// <see cref="HydraulicLocationEntity"/>.
    /// </summary>
    public partial class HydraulicLocationEntity
    {
        /// <summary>
        /// Read the <see cref="HydraulicLocationEntity"/> and use the information to construct a <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <returns>A new <see cref="HydraulicBoundaryLocation"/>.</returns>
        public HydraulicBoundaryLocation Read()
        {
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(
                LocationId, 
                Name, 
                Convert.ToDouble(LocationX), 
                Convert.ToDouble(LocationY))
            {
                StorageId = HydraulicLocationEntityId
            };

            if (DesignWaterLevel.HasValue)
            {
                hydraulicBoundaryLocation.DesignWaterLevel = Convert.ToDouble(DesignWaterLevel);
            }

            return hydraulicBoundaryLocation;
        }
    }
}