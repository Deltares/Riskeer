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
using Core.Common.Util;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Util.TestUtil;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapFeature"/>.
    /// </summary>
    public static class MapFeaturesTestHelper
    {
        private const string categoryMetaData = "Categorie";
        private const string probabilityMetaData = "Faalkans";

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

                Assert.AreEqual(hydraulicBoundaryLocation.Id, mapFeature.MetaData["ID"]);
                Assert.AreEqual(hydraulicBoundaryLocation.Name, mapFeature.MetaData["Naam"]);
                Assert.AreEqual(hydraulicBoundaryLocation.Location, mapFeature.MapGeometries.First().PointCollections.First().First());

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.A+");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.A");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.B");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h gr.C");

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.A+");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.A");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.B");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "Hs gr.C");
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="features"/> contains the data that is representative for the data
        /// in <paramref name="expectedAssembly"/> and <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="expectedAssembly">The expected <see cref="FailureMechanismSectionAssembly"/>.</param>
        /// <param name="failureMechanism">The failure mechanism that contains the sections.</param>
        /// <param name="features">The features that need to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>the number of sections and features are not the same;</item>
        /// <item>the properties of a section and the <paramref name="expectedAssembly"/> are not the same as
        /// the one in the features.</item>
        /// </list>
        /// </exception>
        public static void AssertAssemblyMapFeatures(FailureMechanismSectionAssembly expectedAssembly,
                                                     IFailureMechanism failureMechanism,
                                                     IEnumerable<MapFeature> features)
        {
            IEnumerable<FailureMechanismSection> sections = failureMechanism.Sections;
            Assert.AreEqual(sections.Count(), features.Count());

            for (var i = 0; i < features.Count(); i++)
            {
                FailureMechanismSection section = sections.ElementAt(i);
                Assert.AreEqual(1, features.ElementAt(i).MapGeometries.Count());
                CollectionAssert.AreEqual(section.Points, features.ElementAt(i).MapGeometries.Single().PointCollections.Single());
                Assert.AreEqual(2, features.ElementAt(i).MetaData.Keys.Count);
                Assert.AreEqual(new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                                    DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(expectedAssembly.Group)).DisplayName,
                                features.ElementAt(i).MetaData[categoryMetaData]);
                Assert.AreEqual(new NoProbabilityValueDoubleConverter().ConvertToString(expectedAssembly.Probability),
                                features.ElementAt(i).MetaData[probabilityMetaData]);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="features"/> contains the data that is representative for the data
        /// in <paramref name="expectedCategoryGroup"/> and <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="expectedCategoryGroup">The expected <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</param>
        /// <param name="failureMechanism">The failure mechanism that contains the sections.</param>
        /// <param name="features">The features that need to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>the number of sections and features are not the same;</item>
        /// <item>the properties of a section and the <paramref name="expectedCategoryGroup"/> are not the same as
        /// the one in the features.</item>
        /// </list>
        /// </exception>
        public static void AssertAssemblyCategoryGroupMapFeatures(FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup,
                                                                  IFailureMechanism failureMechanism,
                                                                  IEnumerable<MapFeature> features)
        {
            IEnumerable<FailureMechanismSection> sections = failureMechanism.Sections;
            Assert.AreEqual(sections.Count(), features.Count());

            for (var i = 0; i < features.Count(); i++)
            {
                FailureMechanismSection section = sections.ElementAt(i);
                Assert.AreEqual(1, features.ElementAt(i).MapGeometries.Count());
                CollectionAssert.AreEqual(section.Points, features.ElementAt(i).MapGeometries.Single().PointCollections.Single());
                Assert.AreEqual(1, features.ElementAt(i).MetaData.Keys.Count);
                Assert.AreEqual(new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                                    DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(expectedCategoryGroup)).DisplayName,
                                features.ElementAt(i).MetaData[categoryMetaData]);
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