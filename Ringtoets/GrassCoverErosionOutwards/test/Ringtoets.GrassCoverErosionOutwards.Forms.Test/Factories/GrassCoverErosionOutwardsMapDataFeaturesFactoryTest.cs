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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Util.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Factories;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Factories
{
    [TestFixture]
    public class GrassCoverErosionOutwardsMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateCalculationFeatures_CalculationsNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_NoCalculations_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(
                Enumerable.Empty<GrassCoverErosionOutwardsWaveConditionsCalculation>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_GivenCalculations_ReturnsCalculationFeaturesCollection()
        {
            // Setup
            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var calculationB = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            calculationA.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.0, 3.0));
            calculationB.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.0, 4.0));

            calculationA.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 5.0, 4.0);
            calculationB.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 2.2, 3.8);

            // Call
            IEnumerable<MapFeature> features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(new[]
            {
                calculationA,
                calculationB
            });

            // Assert
            Assert.AreEqual(2, features.Count());
            Assert.AreEqual(1, features.ElementAt(0).MapGeometries.Count());
            Assert.AreEqual(1, features.ElementAt(1).MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                new Point2D(1.0, 3.0),
                new Point2D(5.0, 4.0)
            }, features.ElementAt(0).MapGeometries.ElementAt(0));
            AssertEqualPointCollections(new[]
            {
                new Point2D(1.0, 4.0),
                new Point2D(2.2, 3.8)
            }, features.ElementAt(1).MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateHydraulicBoundaryLocationsFeatures_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationsFeatures(
                null, new GrassCoverErosionOutwardsFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationsFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationsFeatures(
                assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateHydraulicBoundaryLocationsFeatures_GivenLocations_ReturnsLocationFeaturesCollection(bool withOutput)
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "test", 1, 1),
                new HydraulicBoundaryLocation(2, "test", 2, 2)
            };

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AddHydraulicBoundaryLocations(
                failureMechanism, assessmentSection, locations, withOutput);

            // Call
            MapFeature[] features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationsFeatures(assessmentSection, failureMechanism).ToArray();

            // Assert
            AssertHydraulicBoundaryFeaturesData(assessmentSection, failureMechanism, features);

            Point2D[] expectedPoints = assessmentSection.HydraulicBoundaryDatabase.Locations
                                                        .Select(location => location.Location)
                                                        .ToArray();
            AssertEqualFeatureCollections(
                expectedPoints, features);
        }

        private static void AssertHydraulicBoundaryFeaturesData(IAssessmentSection assessmentSection, GrassCoverErosionOutwardsFailureMechanism failureMechanism, IEnumerable<MapFeature> features)
        {
            HydraulicBoundaryLocation[] hydraulicBoundaryLocationsArray = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            int expectedNrOfFeatures = hydraulicBoundaryLocationsArray.Length;
            Assert.AreEqual(expectedNrOfFeatures, features.Count());

            for (var i = 0; i < expectedNrOfFeatures; i++)
            {
                HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryLocationsArray[i];
                MapFeature mapFeature = features.ElementAt(i);

                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(Iv->IIv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(IIv->IIIv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(IIIv->IVv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(IVv->Vv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "h(Vv->VIv)");

                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "hs(Iv->IIv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, hydraulicBoundaryLocation),
                    mapFeature, "hs(IIv->IIIv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "hs(IIIv->IVv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "hs(IVv->Vv)");
                MapFeaturesMetaDataTestHelper.AssertHydraulicBoundaryLocationOutputMetaData(
                    GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, hydraulicBoundaryLocation),
                    mapFeature, "hs(Vv->VIv)");
            }
        }

        private static RoundedDouble GetExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculationList,
                                                       HydraulicBoundaryLocation hydraulicBoundaryLocation1)
        {
            return calculationList
                   .Where(calculation => calculation.HydraulicBoundaryLocation.Equals(hydraulicBoundaryLocation1))
                   .Select(calculation => calculation.Output?.Result ?? RoundedDouble.NaN)
                   .Single();
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p)), geometry.PointCollections.First());
        }

        private static void AssertEqualFeatureCollections(IEnumerable<Point2D> points, IEnumerable<MapFeature> features)
        {
            Assert.AreEqual(points.Count(), features.Count());
            for (var i = 0; i < points.Count(); i++)
            {
                CollectionAssert.AreEqual(new[]
                {
                    points.ElementAt(i)
                }, features.ElementAt(i).MapGeometries.First().PointCollections.First());
            }
        }
    }
}