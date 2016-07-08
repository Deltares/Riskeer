// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Plugins.DotSpatial.Forms;
using NUnit.Framework;

namespace Core.Plugins.DotSpatial.Test.Forms
{
    [TestFixture]
    public class MapDataViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var mapView = new MapDataView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(mapView);
                Assert.IsInstanceOf<IView>(mapView);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddsMapControl()
        {
            // Call
            using (var mapView = new MapDataView())
            {
                // Assert
                Assert.AreEqual(1, mapView.Controls.Count);
                object mapObject = mapView.Controls[0];
                Assert.IsInstanceOf<MapControl>(mapObject);

                var map = (MapControl)mapObject;
                Assert.AreEqual(DockStyle.Fill, map.Dock);
                Assert.NotNull(mapView.Map);
            }
        }

        [Test]
        public void Data_SetToNull_MapControlNotUpdated()
        {
            // Setup
            using (var mapView = new MapDataView())
            {
                var map = (MapControl)mapView.Controls[0];
                var mapData = map.Data;

                // Call
                TestDelegate testDelegate = () => mapView.Data = null;

                // Assert
                Assert.DoesNotThrow(testDelegate);
                Assert.AreSame(mapData, map.Data);
            }
        }

        [Test]
        public void Data_SetToObject_ThrowsInvalidCastException()
        {
            // Setup
            using (var mapView = new MapDataView())
            {
                // Call
                TestDelegate testDelegate = () => mapView.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(testDelegate);
            }
        }

        [Test]
        public void Data_SetToMapCollectionData_MapDataSet()
        {
            // Setup
            var features = new Collection<MapFeature>
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            using (var mapView = new MapDataView())
            {
                var map = (MapControl)mapView.Controls[0];
                var pointData = new MapPointData(features, "test data");
                var collection = new MapDataCollection(new List<MapData>
                {
                    pointData
                }, "test");

                // Call
                mapView.Data = collection;

                // Assert
                Assert.AreSame(pointData, map.Data.List.First());
                Assert.AreSame(collection, mapView.Data);
            }
        }
    }
}