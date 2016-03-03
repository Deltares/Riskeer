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
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;

using DotSpatial.Controls;
using DotSpatial.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class MapControlTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var map = new MapControl())
            {
                // Assert
                Assert.IsInstanceOf<Control>(map);
                Assert.IsInstanceOf<IMapControl>(map);
                Assert.IsNull(map.Data);
                Assert.IsTrue(map.IsPanningEnabled);
                Assert.IsFalse(map.IsRectangleZoomingEnabled);
                Assert.IsTrue(map.IsMouseCoordinatesVisible);
            }
        }

        [Test]
        public void Data_UnknownMapData_ThrowsNotSupportedException()
        {
            // Setup
            using (var map = new MapControl())
            {
                var testData = new TestMapData("test data");

                // Call
                TestDelegate test = () => map.Data = testData;

                // Assert
                Assert.Throws<NotSupportedException>(test);
            }
        }

        [Test]
        public void Data_Null_ReturnsNull()
        {
            // Setup
            using (var map = new MapControl())
            {
                // Call
                map.Data = null;

                // Assert
                Assert.IsNull(map.Data);
            }
        }

        [Test]
        public void Data_NotNull_ReturnsData()
        {
            // Setup
            using (var map = new MapControl())
            {
                var testData = new MapPointData(Enumerable.Empty<Point2D>(), "test data");

                // Call
                map.Data = testData;

                // Assert
                Assert.AreSame(testData, map.Data);
            }
        }

        [Test]
        public void Data_SetPointData_MapPointLayerAdded()
        {
            // Setup
            using (var map = new MapControl())
            {
                var testData = new MapPointData(Enumerable.Empty<Point2D>(), "test data");
                var mapView = map.Controls.OfType<Map>().First();

                // Call
                map.Data = testData;

                // Assert
                Assert.IsInstanceOf<MapPointData>(map.Data);
                Assert.AreEqual(1, mapView.Layers.Count);
                Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);
            }
        }

        [Test]
        public void UpdateObserver_UpdateData_UpdateMap()
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var testData = new MapDataCollection(new List<MapData>
                {

                    new MapPointData(Enumerable.Empty<Point2D>(), "test data")
                }, "test data");

                map.Data = testData;

                // Precondition
                Assert.AreEqual(1, mapView.Layers.Count);
                Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);

                testData.List.Add(new MapLineData(Enumerable.Empty<Point2D>(), "test data"));

                // Call
                map.UpdateObserver();

                // Assert
                Assert.AreEqual(2, mapView.Layers.Count);
                Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);
                Assert.IsInstanceOf<MapLineLayer>(mapView.Layers[1]);
            }
        }

        [Test]
        public void Data_SetToNull_DetachObserver()
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var testData = new MapDataCollection(new List<MapData>
                {
                    new MapPointData(Enumerable.Empty<Point2D>(), "test data")
                }, "test data");

                map.Data = testData;

                // Precondition
                Assert.AreEqual(1, mapView.Layers.Count);
                Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);

                map.Data = null;

                testData.List.Add(new MapPointData(Enumerable.Empty<Point2D>(), "test data"));

                // Call
                map.UpdateObserver();

                // Assert
                Assert.IsNull(map.Data);
                Assert.AreEqual(0, mapView.Layers.Count);
            }
        }

        [Test]
        [RequiresSTA]
        public void ZoomToAllVisibleLayers_MapInForm_ViewInvalidatedLayersSame()
        {
            // Setup
            using (var form = new Form())
            {
                var map = new MapControl();
                var testData = new MapPointData(Enumerable.Empty<Point2D>(), "test data");
                var mapView = map.Controls.OfType<Map>().First();
                var invalidated = 0;

                map.Data = testData;
                form.Controls.Add(map);

                mapView.Invalidated += (sender, args) => { invalidated++; };

                form.Show();
                Assert.AreEqual(0, invalidated, "Precondition failed: mapView.Invalidated > 0");

                // Call
                map.ZoomToAllVisibleLayers();

                // Assert
                Assert.AreEqual(2, invalidated);
                Assert.AreEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
            }
        }

        [Test]
        public void ZoomToAllVisibleLayers_NotAllLayersVisible_ZoomToVisibleLayersExtent()
        {
            // Setup
            var map = new MapControl();
            var mapView = map.Controls.OfType<Map>().First();
            var testData = GetTestData();
            map.Data = testData;

            var expectedExtent = new Extent(0.0, 0.5, 1.6, 2.1);

            // Precondition
            Assert.AreEqual(3, mapView.Layers.Count, "Precondition failed: mapView.Layers != 3");

            // Call
            map.ZoomToAllVisibleLayers();

            // Assert
            Assert.AreNotEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
            Assert.AreEqual(expectedExtent, mapView.ViewExtents);
        }

        [Test]
        public void ToggleRectangleZooming_PanningIsEnabled_RectangleZoomingIsEnabled()
        {
            // Setup
            using (var map = new MapControl())
            {
                Assert.IsFalse(map.IsRectangleZoomingEnabled, "Precondition failed: IsRectangleZoomingEnabled is true");

                // Call
                map.ToggleRectangleZooming();

                // Assert
                Assert.IsTrue(map.IsRectangleZoomingEnabled);
                Assert.IsFalse(map.IsPanningEnabled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleRectangleZooming_Always_ChangesState(bool isRectangleZooming)
        {
            // Setup
            using (var map = new MapControl())
            {
                if (isRectangleZooming)
                {
                    map.ToggleRectangleZooming();
                }

                // Precondition
                Assert.AreEqual(isRectangleZooming, map.IsRectangleZoomingEnabled,
                                String.Format("Precondition failed: IsRectangleZoomingEnabled is {0}", map.IsRectangleZoomingEnabled));
                Assert.AreEqual(!isRectangleZooming, map.IsPanningEnabled,
                                String.Format("Precondition failed: IsPanningEnabled is {0}", map.IsPanningEnabled));

                // Call
                map.ToggleRectangleZooming();

                // Assert
                Assert.IsTrue(map.IsRectangleZoomingEnabled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleMouseCoordinatesVisibility_Always_ChangesState(bool isShowingCoordinates)
        {
            // Setup
            using (var map = new MapControl())
            {
                if (!isShowingCoordinates)
                {
                    // Make sure the state is correct
                    map.ToggleMouseCoordinatesVisibility();

                    // Precondition
                    Assert.IsFalse(map.IsMouseCoordinatesVisible);
                }

                // Call
                map.ToggleMouseCoordinatesVisibility();

                // Assert
                Assert.AreNotEqual(isShowingCoordinates, map.IsMouseCoordinatesVisible);
            }
        }

        [Test]
        [RequiresSTA]
        public void UpdateObserver_MapInForm_MapLayersRenewed()
        {
            // Setup
            using (var form = new Form())
            {
                var map = new MapControl();
                var testData = new MapPointData(Enumerable.Empty<Point2D>(), "test data");
                var view = map.Controls.OfType<Map>().First();

                map.Data = testData;
                var layers = view.Layers.ToList();

                form.Controls.Add(map);

                form.Show();

                // Call
                map.UpdateObserver();

                // Assert
                Assert.AreEqual(1, view.Layers.Count);
                Assert.AreNotSame(layers[0], view.Layers[0]);
            }
        }

        private static MapDataCollection GetTestData()
        {
            var points = new MapPointData(new Collection<Point2D>
            {
                new Point2D(1.5, 2),
                new Point2D(1.1, 1),
                new Point2D(0.8, 0.5)
            }, "test data");
            var lines = new MapLineData(new Collection<Point2D>
            {
                new Point2D(0.0, 1.1),
                new Point2D(1.0, 2.1),
                new Point2D(1.6, 1.6)
            }, "test data");
            var polygons = new MapPolygonData(new Collection<Point2D>
            {
                new Point2D(1.0, 1.3),
                new Point2D(3.0, 2.6),
                new Point2D(5.6, 1.6)
            }, "test data")
            {
                IsVisible = false
            };

            return new MapDataCollection(new List<MapData>
            {
                points, lines, polygons
            }, "test data");
        }
    }
}