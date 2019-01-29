// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Riskeer.GrassCoverErosionOutwards.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapFeature"/>.
    /// </summary>
    public static class GrassCoverErosionOutwardsMapFeaturesTestHelper
    {
        /// <summary>
        /// Asserts whether <paramref name="features"/> contains the data that is representative for the data of
        /// hydraulic boundary locations and calculations in <paramref name="failureMechanism"/> and
        /// <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism that contains the first part of the original data.</param>
        /// <param name="assessmentSection">The assessment section that contains the second part of the original data.</param>
        /// <param name="features">The features that need to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>the number of hydraulic boundary locations and features are not the same;</item>
        /// <item>the general properties (such as id, name and location) of hydraulic boundary locations and features
        /// are not the same;</item>
        /// <item>the wave height or the design water level calculation results of a hydraulic boundary location and the
        /// respective outputs of a corresponding feature are not the same.</item>
        /// </list>
        /// </exception>
        public static void AssertHydraulicBoundaryFeaturesData(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                               IAssessmentSection assessmentSection,
                                                               IEnumerable<MapFeature> features)
        {
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            int expectedNrOfFeatures = hydraulicBoundaryLocations.Count();
            Assert.AreEqual(expectedNrOfFeatures, features.Count());

            for (var i = 0; i < expectedNrOfFeatures; i++)
            {
                HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryLocations.ElementAt(i);
                MapFeature mapFeature = features.ElementAt(i);

                Assert.AreEqual(hydraulicBoundaryLocation.Id, mapFeature.MetaData["ID"]);
                Assert.AreEqual(hydraulicBoundaryLocation.Name, mapFeature.MetaData["Naam"]);
                Assert.AreEqual(hydraulicBoundaryLocation.Location, mapFeature.MapGeometries.First().PointCollections.First().First());

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.Iv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.IIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.IIIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.IVv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.Vv");

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.Iv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.IIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.IIIv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.IVv");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.Vv");

                Assert.AreEqual(12, mapFeature.MetaData.Keys.Count);
            }
        }

        private static string GetExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            RoundedDouble result = calculations
                                   .Single(calculation => calculation.HydraulicBoundaryLocation.Equals(hydraulicBoundaryLocation))
                                   .Output?.Result ?? RoundedDouble.NaN;

            return result.ToString();
        }
    }
}