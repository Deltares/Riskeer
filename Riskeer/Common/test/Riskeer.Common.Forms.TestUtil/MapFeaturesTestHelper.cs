// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Util.Helpers;
using Riskeer.Common.Util.TestUtil;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapFeature"/>.
    /// </summary>
    public static class MapFeaturesTestHelper
    {
        /// <summary>
        /// Asserts whether <paramref name="features"/> contains the data that is representative for the data of
        /// hydraulic boundary locations and calculations in <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section that contains the original data.</param>
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
        public static void AssertHydraulicBoundaryFeaturesData(IAssessmentSection assessmentSection, IEnumerable<MapFeature> features)
        {
            HydraulicBoundaryLocation[] hydraulicBoundaryLocationsArray = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            int expectedNrOfFeatures = hydraulicBoundaryLocationsArray.Length;
            Assert.AreEqual(expectedNrOfFeatures, features.Count());

            for (var i = 0; i < expectedNrOfFeatures; i++)
            {
                HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryLocationsArray[i];
                MapFeature mapFeature = features.ElementAt(i);

                Assert.AreEqual(hydraulicBoundaryLocation.Location, mapFeature.MapGeometries.First().PointCollections.First().First());

                MapFeaturesMetaDataTestHelper.AssertMetaData(hydraulicBoundaryLocation.Id, mapFeature, "ID");
                MapFeaturesMetaDataTestHelper.AssertMetaData(hydraulicBoundaryLocation.Name, mapFeature, "Naam");

                var presentedMetaDataItems = new List<string>();
                AssertMetaData(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, hydraulicBoundaryLocation, mapFeature,
                               assessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability, "h - {0}", presentedMetaDataItems);
                AssertMetaData(assessmentSection.WaterLevelCalculationsForSignalingNorm, hydraulicBoundaryLocation, mapFeature,
                               assessmentSection.FailureMechanismContribution.SignalFloodingProbability, "h - {0}", presentedMetaDataItems);

                foreach (HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
                {
                    AssertMetaData(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations, hydraulicBoundaryLocation, mapFeature,
                                   calculationsForTargetProbability.TargetProbability, "h - {0}", presentedMetaDataItems);
                }

                foreach (HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities)
                {
                    AssertMetaData(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations, hydraulicBoundaryLocation, mapFeature,
                                   calculationsForTargetProbability.TargetProbability, "Hs - {0}", presentedMetaDataItems);
                }
            }
        }

        /// <summary>
        /// Asserts whether the <paramref name="features"/> contains the meta data that is representative for the <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line that contains the original data.</param>
        /// <param name="assessmentSection">The assessment section that contains the original data.</param>
        /// <param name="features">The features that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the meta data does not have the correct values.</exception>
        public static void AssertReferenceLineMetaData(ReferenceLine referenceLine, IAssessmentSection assessmentSection, IEnumerable<MapFeature> features)
        {
            MapFeature feature = features.Single();
            Assert.AreEqual(assessmentSection.Id, feature.MetaData["ID"]);
            Assert.AreEqual(assessmentSection.Name, feature.MetaData["Naam"]);
            Assert.AreEqual(new RoundedDouble(2, referenceLine.Length), feature.MetaData["Lengte*"]);
        }

        private static void AssertMetaData(IEnumerable<HydraulicBoundaryLocationCalculation> calculations, HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                           MapFeature mapFeature, double targetProbability, string displayName, List<string> presentedMetaDataItems)
        {
            string uniqueName = NamingHelper.GetUniqueName(
                presentedMetaDataItems, string.Format(displayName, ProbabilityFormattingHelper.Format(targetProbability)),
                v => v);

            MapFeaturesMetaDataTestHelper.AssertMetaData(
                GetExpectedResult(calculations, hydraulicBoundaryLocation),
                mapFeature, uniqueName);

            presentedMetaDataItems.Add(uniqueName);
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