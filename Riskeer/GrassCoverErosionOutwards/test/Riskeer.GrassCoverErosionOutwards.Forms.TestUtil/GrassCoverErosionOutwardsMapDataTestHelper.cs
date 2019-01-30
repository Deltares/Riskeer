// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Components.Gis.Data;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.GrassCoverErosionOutwards.Data;

namespace Riskeer.GrassCoverErosionOutwards.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapData"/>.
    /// </summary>
    public static class GrassCoverErosionOutwardsMapDataTestHelper
    {
        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the data of
        /// hydraulic boundary locations and calculations in <paramref name="failureMechanism"/> and
        /// <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism that contains the first part of the original data.</param>
        /// <param name="assessmentSection">The assessment section that contains the second part of the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>;</item>
        /// <item>the number of hydraulic boundary locations and features in <see cref="MapData"/> are not the same;</item>
        /// <item>the general properties (such as id, name and location) of hydraulic boundary locations and features in
        /// <see cref="MapData"/> are not the same;</item>
        /// <item>the wave height or the design water level calculation results of a hydraulic boundary location and the
        /// respective outputs of a corresponding feature are not the same.</item>
        /// </list>
        /// </exception>
        public static void AssertHydraulicBoundaryLocationsMapData(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                   IAssessmentSection assessmentSection,
                                                                   MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var hydraulicLocationsMapData = (MapPointData) mapData;

            GrassCoverErosionOutwardsMapFeaturesTestHelper.AssertHydraulicBoundaryFeaturesData(failureMechanism,
                                                                                               assessmentSection,
                                                                                               hydraulicLocationsMapData.Features);
        }
    }
}