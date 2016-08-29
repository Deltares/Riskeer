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
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Hydraulic boundary location for the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocation : IStorable
    {
        private RoundedDouble sectionSpecificWaterLevel;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation"/> 
        /// this specific water level applies to.</param>
        public GrassCoverErosionOutwardsHydraulicBoundaryLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocation");
            }
            HydraulicBoundaryLocation = hydraulicBoundaryLocation;
            sectionSpecificWaterLevel = new RoundedDouble(2, double.NaN);
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation"/>.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; private set; }

        /// <summary>
        /// Gets or sets the section specific water level of <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocation"/>.
        /// </summary>
        public RoundedDouble SectionSpecificWaterLevel
        {
            get
            {
                return sectionSpecificWaterLevel;
            }
            set
            {
                sectionSpecificWaterLevel = value.ToPrecision(sectionSpecificWaterLevel.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the convergence status of the section specific water level calculation.
        /// </summary>
        public CalculationConvergence SectionSpecificWaterLevelCalculationConvergence { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the storage of the class.
        /// </summary>
        public long StorageId { get; set; }
    }
}