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
using Core.Common.Base.Geometry;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Hydraulic boundary location for which values are calculated during the assessment of the 
    /// <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocation : IHydraulicBoundaryLocation
    {
        private readonly IHydraulicBoundaryLocation hydraulicBoundaryLocation;
        private RoundedDouble designWaterLevel;
        private RoundedDouble waveHeight;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="IHydraulicBoundaryLocation"/> 
        /// which is used in the calculation of grass cover erosion outwards specific properties of the
        /// location.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsHydraulicBoundaryLocation(IHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocation");
            }
            this.hydraulicBoundaryLocation = hydraulicBoundaryLocation;
            designWaterLevel = new RoundedDouble(2, double.NaN);
            waveHeight = new RoundedDouble(2, double.NaN);
        }

        public RoundedDouble DesignWaterLevel
        {
            get
            {
                return designWaterLevel;
            }
            set
            {
                designWaterLevel = value.ToPrecision(designWaterLevel.NumberOfDecimalPlaces);
            }
        }

        public CalculationConvergence DesignWaterLevelCalculationConvergence { get; set; }

        public RoundedDouble WaveHeight
        {
            get
            {
                return waveHeight;
            }
            set
            {
                waveHeight = value.ToPrecision(waveHeight.NumberOfDecimalPlaces);
            }
        }

        public CalculationConvergence WaveHeightCalculationConvergence { get; set; }

        public long Id
        {
            get
            {
                return hydraulicBoundaryLocation.Id;
            }
        }

        public string Name
        {
            get
            {
                return hydraulicBoundaryLocation.Name;
            }
        }

        public Point2D Location
        {
            get
            {
                return hydraulicBoundaryLocation.Location;
            }
        }

        public long StorageId { get; set; }
    }
}