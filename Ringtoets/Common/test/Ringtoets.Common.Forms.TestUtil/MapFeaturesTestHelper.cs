﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Util.TestUtil;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapFeature"/>.
    /// </summary>
    public static class MapFeaturesTestHelper
    {
        public static void AssertHydraulicBoundaryFeaturesData(IAssessmentSection assessmentSection, IEnumerable<MapFeature> features)
        {
            HydraulicBoundaryLocation[] hydraulicBoundaryLocationsArray = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            int expectedNrOfFeatures = hydraulicBoundaryLocationsArray.Length;
            Assert.AreEqual(expectedNrOfFeatures, features.Count());

            for (var i = 0; i < expectedNrOfFeatures; i++)
            {
                HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryLocationsArray[i];
                MapFeature mapFeature = features.ElementAt(i);

                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(A+->A)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(A->B)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(B->C)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(C->D)");

                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(A+->A)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(A->B)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(B->C)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(C->D)");
            }
        }

        /// <summary>
        /// Gets the expected result of a hydraulic boundary location calculation.
        /// </summary>
        /// <param name="calculationList">The list to get the calculation from.</param>
        /// <param name="hydraulicBoundaryLocation">The location to get the calculation for.</param>
        /// <returns>The result when there is output; <see cref="RoundedDouble.NaN"/> otherwise.</returns>
        public static RoundedDouble GetExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculationList,
                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return calculationList
                   .Where(calculation => calculation.HydraulicBoundaryLocation.Equals(hydraulicBoundaryLocation))
                   .Select(calculation => calculation.Output?.Result ?? RoundedDouble.NaN)
                   .Single();
        }
    }
}