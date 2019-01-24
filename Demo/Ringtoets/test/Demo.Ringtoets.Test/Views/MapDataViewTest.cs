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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Demo.Riskeer.Views;
using NUnit.Framework;

namespace Demo.Riskeer.Test.Views
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
        public void DefaultConstructor_Always_AddEmptyMapControl()
        {
            // Call
            using (var view = new MapDataView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Map, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_SetToObject_DoesNotThrow()
        {
            // Setup
            using (var mapView = new MapDataView())
            {
                // Call
                TestDelegate testDelegate = () => mapView.Data = new object();

                // Assert
                Assert.DoesNotThrow(testDelegate);
                Assert.IsNull(mapView.Data);
            }
        }

        [Test]
        public void Data_SetToMapCollectionData_MapDataSet()
        {
            // Setup
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            using (var mapView = new MapDataView())
            {
                var map = (MapControl) mapView.Controls[0];
                var collection = new MapDataCollection("test");
                var pointData = new MapPointData("test data")
                {
                    Features = features
                };

                collection.Add(pointData);

                // Call
                mapView.Data = collection;

                // Assert
                Assert.AreSame(pointData, map.Data.Collection.First());
                Assert.AreSame(collection, mapView.Data);
            }
        }
    }
}