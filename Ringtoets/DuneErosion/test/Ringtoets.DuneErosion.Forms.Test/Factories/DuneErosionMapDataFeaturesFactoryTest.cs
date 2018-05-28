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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Util.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.Factories;

namespace Ringtoets.DuneErosion.Forms.Test.Factories
{
    [TestFixture]
    public class DuneErosionMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateDuneLocationFeatures_DuneLocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures((DuneLocation[]) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("duneLocations", paramName);
        }

        [Test]
        public void CreateDuneLocationFeatures_DuneLocationsArrayEmpty_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(new DuneLocation[0]);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateDuneLocationFeatures_DuneLocationsWithOutput_ReturnsLocationFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            DuneLocation[] locations = points.Select(p => new ValidDuneLocation(p)
            {
                Calculation =
                {
                    Output = new TestDuneLocationCalculationOutput()
                }
            }).Cast<DuneLocation>().ToArray();

            // Call
            MapFeature[] features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(locations);

            // Assert
            Assert.AreEqual(locations.Length, features.Length);
            for (var i = 0; i < locations.Length; i++)
            {
                Assert.AreEqual(locations[i].Id, features[i].MetaData["ID"]);
                Assert.AreEqual(locations[i].Name, features[i].MetaData["Naam"]);
                Assert.AreEqual(locations[i].CoastalAreaId, features[i].MetaData["Kustvaknummer"]);
                Assert.AreEqual(locations[i].Offset.ToString("0.#", CultureInfo.InvariantCulture), features[i].MetaData["Metrering"]);
                Assert.AreEqual(locations[i].Calculation.Output.WaterLevel, (double) features[i].MetaData["Rekenwaarde waterstand"],
                                locations[i].Calculation.Output.WaterLevel.GetAccuracy());
                Assert.AreEqual(locations[i].Calculation.Output.WaveHeight, (double) features[i].MetaData["Rekenwaarde Hs"],
                                locations[i].Calculation.Output.WaveHeight.GetAccuracy());
                Assert.AreEqual(locations[i].Calculation.Output.WavePeriod, (double) features[i].MetaData["Rekenwaarde Tp"],
                                locations[i].Calculation.Output.WavePeriod.GetAccuracy());
                Assert.AreEqual(locations[i].D50, (RoundedDouble) features[i].MetaData["Rekenwaarde d50"],
                                locations[i].D50.GetAccuracy());
                Assert.AreEqual(8, features[i].MetaData.Keys.Count);
            }

            AssertEqualFeatureCollections(points, features);
        }

        [Test]
        public void CreateDuneLocationFeatures_DuneLocationsWithoutOutput_ReturnsLocationFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            DuneLocation[] locations = points.Select(p => new ValidDuneLocation(p)).Cast<DuneLocation>().ToArray();

            // Call
            MapFeature[] features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(locations);

            // Assert
            Assert.AreEqual(locations.Length, features.Length);
            for (var i = 0; i < locations.Length; i++)
            {
                Assert.AreEqual(locations[i].Id, features[i].MetaData["ID"]);
                Assert.AreEqual(locations[i].Name, features[i].MetaData["Naam"]);
                Assert.AreEqual(locations[i].CoastalAreaId, features[i].MetaData["Kustvaknummer"]);
                Assert.AreEqual(locations[i].Offset.ToString("0.#", CultureInfo.InvariantCulture), features[i].MetaData["Metrering"]);
                Assert.AreEqual(double.NaN, features[i].MetaData["Rekenwaarde waterstand"]);
                Assert.AreEqual(double.NaN, features[i].MetaData["Rekenwaarde Hs"]);
                Assert.AreEqual(double.NaN, features[i].MetaData["Rekenwaarde Tp"]);
                Assert.AreEqual(locations[i].D50, (RoundedDouble) features[i].MetaData["Rekenwaarde d50"],
                                locations[i].D50.GetAccuracy());
                Assert.AreEqual(8, features[i].MetaData.Keys.Count);
            }

            AssertEqualFeatureCollections(points, features);
        }

        [Test]
        public void CreateDuneLocationsFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures((DuneErosionFailureMechanism) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateDuneLocationFeatures_FailureMechanismWithoutDuneLocations_ReturnsEmptyFeaturesArray()
        {
            // Call
            IEnumerable<MapFeature> features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(new DuneErosionFailureMechanism());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDuneLocationsFeatures_WithFailureMechanism_ReturnFeature(bool withOutput)
        {
            // Setup
            var duneLocations = new[]
            {
                CreateDuneLocation(1),
                CreateDuneLocation(2)
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(duneLocations);

            if (withOutput)
            {
                DuneLocationsTestHelper.SetDuneLocationCalculationOutput(failureMechanism);
            }

            // Call
            IEnumerable<MapFeature> features = DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(failureMechanism);

            // Assert
            Assert.AreEqual(duneLocations.Length, features.Count());

            for (var i = 0; i < duneLocations.Length; i++)
            {
                DuneLocation expectedDuneLocation = duneLocations[i];
                MapFeature mapFeature = features.ElementAt(i);

                Assert.AreEqual(expectedDuneLocation.Id, mapFeature.MetaData["ID"]);
                Assert.AreEqual(expectedDuneLocation.Name, mapFeature.MetaData["Naam"]);
                Assert.AreEqual(expectedDuneLocation.CoastalAreaId, mapFeature.MetaData["Kustvaknummer"]);
                Assert.AreEqual(expectedDuneLocation.Offset.ToString("0.#", CultureInfo.InvariantCulture), mapFeature.MetaData["Metrering"]);
                Assert.AreEqual(expectedDuneLocation.Location, mapFeature.MapGeometries.First().PointCollections.First().Single());

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h(Iv->IIv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h(IIv->IIIv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h(IIIv->IVv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h(IVv->Vv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaterLevel(failureMechanism.CalculationsForFactorizedLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde h(Vv->VIv)");

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs(Iv->IIv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs(IIv->IIIv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs(IIIv->IVv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs(IVv->Vv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWaveHeight(failureMechanism.CalculationsForFactorizedLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Hs(Vv->VIv)");

                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp(Iv->IIv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp(IIv->IIIv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp(IIIv->IVv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp(IVv->Vv)");
                MapFeaturesMetaDataTestHelper.AssertMetaData(
                    GetExpectedWavePeriod(failureMechanism.CalculationsForFactorizedLowerLimitNorm, expectedDuneLocation),
                    mapFeature, "Rekenwaarde Tp(Vv->VIv)");

                Assert.AreEqual(19, mapFeature.MetaData.Keys.Count);
            }
        }

        private static DuneLocation CreateDuneLocation(int seed)
        {
            var random = new Random(seed);

            int id = random.Next();
            return new DuneLocation(id, $"Location_{id}", new Point2D(random.NextDouble(), random.NextDouble()), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = random.Next(),
                D50 = random.NextDouble(),
                Offset = random.NextDouble()
            });
        }

        private static string GetExpectedWaterLevel(IEnumerable<DuneLocationCalculation> calculations,
                                                    DuneLocation duneLocation)
        {
            RoundedDouble result = calculations
                                   .Single(calculation => calculation.DuneLocation.Equals(duneLocation))
                                   .Output?.WaterLevel ?? RoundedDouble.NaN;

            return result.ToString();
        }

        private static string GetExpectedWaveHeight(IEnumerable<DuneLocationCalculation> calculations,
                                                    DuneLocation duneLocation)
        {
            RoundedDouble result = calculations
                                   .Single(calculation => calculation.DuneLocation.Equals(duneLocation))
                                   .Output?.WaveHeight ?? RoundedDouble.NaN;

            return result.ToString();
        }

        private static string GetExpectedWavePeriod(IEnumerable<DuneLocationCalculation> calculations,
                                                    DuneLocation duneLocation)
        {
            RoundedDouble result = calculations
                                   .Single(calculation => calculation.DuneLocation.Equals(duneLocation))
                                   .Output?.WavePeriod ?? RoundedDouble.NaN;

            return result.ToString();
        }

        private class ValidDuneLocation : DuneLocation
        {
            public ValidDuneLocation(Point2D location) : base(0, "", location, new ConstructionProperties()) {}
        }

        private static void AssertEqualFeatureCollections(Point2D[] points, MapFeature[] features)
        {
            Assert.AreEqual(points.Length, features.Length);
            for (var i = 0; i < points.Length; i++)
            {
                CollectionAssert.AreEqual(new[]
                {
                    points[i]
                }, features[i].MapGeometries.First().PointCollections.First());
            }
        }
    }
}