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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateReferenceLineFeatures_ReferenceLineNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateReferenceLineFeatures_GivenReferenceLine_ReturnsReferenceLineFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine);

            // Assert
            AssertEqualPointCollections(points, features.ElementAt(0).MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateHydraulicBoundaryDatabaseFeatures_HydraulicBoundaryDatabaseNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateHydraulicBoundaryDatabaseFeatures_GivenHydraulicBoundaryDatabase_ReturnsLocationFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.AddRange(points.Select(p => new HydraulicBoundaryLocation(0, "", p.X, p.Y)));

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeatures(hydraulicBoundaryDatabase);

            // Assert
            AssertEqualPointCollections(points, features.ElementAt(0).MapGeometries.ElementAt(0));
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points, geometry.PointCollections.First());
        } 
    }
}
