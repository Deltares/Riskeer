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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Class that holds all grass cover erosion inwards calculation specific input parameters.
    /// </summary>
    public class GrassCoverErosionInwardsInput : Observable, ICalculationInput
    {
        private IEnumerable<RoughnessProfileSection> geometry;
        private RoundedDouble orientation;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInput"/>.
        /// </summary>
        public GrassCoverErosionInwardsInput()
        {
            BreakWater = new List<BreakWater>();
            orientation = new RoundedDouble(2);
        }

        /// <summary>
        /// Gets the dike's geometry (without foreshore geometry).
        /// </summary>
        public IEnumerable<RoughnessProfileSection> DikeGeometry
        {
            get
            {
                return geometry == null ? Enumerable.Empty<RoughnessProfileSection>() : geometry.Skip(ForeshoreDikeGeometryPoints);
            }
        }

        /// <summary>
        /// Gets the dike's foreshore geometry.
        /// </summary>
        public IEnumerable<RoughnessProfileSection> ForeshoreGeometry
        {
            get
            {
                return geometry == null ? Enumerable.Empty<RoughnessProfileSection>() : geometry.Take(ForeshoreDikeGeometryPoints);
            }
        }

        /// <summary>
        /// Gets or sets the dike's orientation
        /// </summary>
        public RoundedDouble Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value.ToPrecision(orientation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the dike's critical flow rate.
        /// </summary>
        public LognormalDistribution CriticalFlowRate { get; set; }

        /// <summary>
        /// Gets or sets if a foreshore is present.
        /// </summary>
        /// <remarks>Value of <see cref="ForeshoreDikeGeometryPoints"/> must not be reset when <see cref="ForeshorePresent"/> is set to <c>false</c>.</remarks>
        public bool ForeshorePresent { get; set; }

        /// <summary>
        /// Gets or sets the dike height.
        /// </summary>
        public double DikeHeight { get; set; }

        /// <summary>
        /// Gets the number of profile points of the dike geometry that form the foreshore geometry.
        /// </summary>
        public int ForeshoreDikeGeometryPoints { get; private set; }

        /// <summary>
        /// Defines if <see cref="BreakWater"/> needs to be taken into account.
        /// </summary>
        public bool BreakWaterPresent { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BreakWater"/>.
        /// </summary>
        public IList<BreakWater> BreakWater { get; set; }

        /// <summary>
        /// Gets or set the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Sets the grass cover erosion inwards geometry.
        /// </summary>
        /// <param name="profileSections">The grass cover erosion inwards geometry points.</param>
        /// <param name="foreshoreGeometryPoints">Defines how many <see cref="RoughnessProfileSection"/> in 
        /// <paramref name="profileSections"/> are foreshore.</param>
        public void SetGeometry(IEnumerable<RoughnessProfileSection> profileSections, int foreshoreGeometryPoints)
        {
            if (profileSections == null)
            {
                throw new ArgumentNullException("profileSections");
            }
            geometry = profileSections;
            ForeshoreDikeGeometryPoints = foreshoreGeometryPoints;
        }
    }
}