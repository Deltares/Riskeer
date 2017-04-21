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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.Views;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneErosionMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateDuneLocationFeatures_DuneLocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneErosionMapDataFeaturesFactory.CreateDuneLocationFeatures(null);

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
                Output = new TestDuneLocationOutput()
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
                Assert.AreEqual(locations[i].Output.WaterLevel, (double) features[i].MetaData["Rekenwaarde waterstand"],
                                locations[i].Output.WaterLevel.GetAccuracy());
                Assert.AreEqual(locations[i].Output.WaveHeight, (double) features[i].MetaData["Rekenwaarde Hs"],
                                locations[i].Output.WaveHeight.GetAccuracy());
                Assert.AreEqual(locations[i].Output.WavePeriod, (double) features[i].MetaData["Rekenwaarde Tp"],
                                locations[i].Output.WavePeriod.GetAccuracy());
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