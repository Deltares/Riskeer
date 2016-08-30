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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an enumeration of <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocation"/> 
    /// for water level at intersection probability calculations.
    /// </summary>
    public class GrassCoverErosionOutwardsWaterLevelLocationsContext : ObservableWrappedObjectContextBase<IAssessmentSection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaterLevelLocationsContext"/>.
        /// </summary>
        /// <param name="wrappedAssessmentSection">The <see cref="IAssessmentSection"/> which the 
        /// <see cref="GrassCoverErosionOutwardsWaterLevelLocationsContext"/> belongs to.</param>
        /// <param name="grassCoverErosionOutwardsHydraulicBoundaryLocations">The grass cover erosion outwards hydraulic boundary locations for this context.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaterLevelLocationsContext(IAssessmentSection wrappedAssessmentSection,
                                                                   ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation> grassCoverErosionOutwardsHydraulicBoundaryLocations)
            : base(wrappedAssessmentSection)
        {
            if (grassCoverErosionOutwardsHydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException("grassCoverErosionOutwardsHydraulicBoundaryLocations");
            }
            Locations = grassCoverErosionOutwardsHydraulicBoundaryLocations;
        }

        /// <summary>
        /// Gets the grass cover erosion outwards hydraulic boundary locations.
        /// </summary>
        public ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation> Locations { get; private set; }
    }
}