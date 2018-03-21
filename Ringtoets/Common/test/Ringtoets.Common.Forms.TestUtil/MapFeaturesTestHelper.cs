// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapFeature"/>.
    /// </summary>
    public static class MapFeaturesTestHelper
    {
        /// <summary>
        /// Asserts whether the meta data for <paramref name="key"/> in <paramref name="feature"/>
        /// contains the correct output data for <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="HydraulicBoundaryLocationCalculation"/>
        /// to assert against.</param>
        /// <param name="feature">The <see cref="MapFeature"/> to be asserted.</param>
        /// <param name="key">The name of the meta data element to retrieve the data from.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the meta data of <paramref name="feature"/> does not 
        /// contain a <see cref="KeyValuePair{TKey,TValue}"/> with <paramref name="key"/> as key.</exception>
        /// <exception cref="AssertionException">Thrown when the wave height or the design water level of a 
        /// hydraulic boundary location and the  respective meta data value associated with <paramref name="key"/>
        ///  are not the same.
        /// </exception>
        public static void AssertHydraulicBoundaryLocationOutputMetaData(HydraulicBoundaryLocationCalculation calculation,
                                                                         MapFeature feature,
                                                                         string key)
        {
            RoundedDouble expectedValue = RoundedDouble.NaN;
            if (calculation.Output != null)
            {
                expectedValue = calculation.Output.Result;
            }

            Assert.AreEqual(expectedValue, feature.MetaData[key]);
        }

        public static void AssertHydraulicBoundaryFeaturesData(IAssessmentSection assessmentSection, IEnumerable<MapFeature> features)
        {
            HydraulicBoundaryLocation[] hydraulicBoundaryLocationsArray = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            int expectedNrOfFeatures = hydraulicBoundaryLocationsArray.Length;
            Assert.AreEqual(expectedNrOfFeatures, features.Count());

            for (var i = 0; i < expectedNrOfFeatures; i++)
            {
                HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryLocationsArray[i];
                MapFeature mapFeature = features.ElementAt(i);

                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(A+->A)");
                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(A->B)");
                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(B->C)");
                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(C->D)");

                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(A+->A)");
                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(A->B)");
                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(B->C)");
                AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs(C->D)");
            }
        }

        /// <summary>
        /// Asserts whether the meta data for <paramref name="key"/> in <paramref name="feature"/>
        /// contains the correct output data.
        /// </summary>
        /// <param name="expectedValue">The value  to assert against.</param>
        /// <param name="feature">The <see cref="MapFeature"/> to be asserted.</param>
        /// <param name="key">The name of the meta data element to retrieve the data from.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the meta data of <paramref name="feature"/> does not 
        /// contain a <see cref="KeyValuePair{TKey,TValue}"/> with <paramref name="key"/> as key.</exception>
        /// <exception cref="AssertionException">Thrown when the wave height or the design water level of a 
        /// hydraulic boundary location and the  respective meta data value associated with <paramref name="key"/>
        ///  are not the same.
        /// </exception>
        public static void AssertHydraulicBoundaryLocationOutputMetaData(RoundedDouble expectedValue,
                                                                         MapFeature feature,
                                                                         string key)
        {
            Assert.AreEqual(expectedValue, feature.MetaData[key]);
        }

        private static RoundedDouble GetExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculationList,
                                                       HydraulicBoundaryLocation hydraulicBoundaryLocation1)
        {
            return calculationList
                   .Where(calculation => calculation.HydraulicBoundaryLocation.Equals(hydraulicBoundaryLocation1))
                   .Select(calculation => calculation.Output?.Result ?? RoundedDouble.NaN)
                   .Single();
        }
    }
}