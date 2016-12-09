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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.MapFunctions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class MapControlTest
    {
        private const double padding = 0.05;

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
        [RequiresSTA]
        public void DefaultConstructor_MapFunctionsCorrectlyInitialized()
        {
            using (var form = new Form())
            {
                // Call
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;

                // Assert
                Assert.AreEqual(9, map.MapFunctions.Count);
                Assert.AreEqual(1, map.MapFunctions.OfType<MapFunctionPan>().Count());
                Assert.AreEqual(1, map.MapFunctions.OfType<MapFunctionSelectionZoom>().Count());
            }
        }

        [Test]
        public void GivenMapControlWithoutData_WhenDataSetToMapDataCollection_ThenMapControlUpdated()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var mapPointData = new MapPointData("Points");
                var mapLineData = new MapLineData("Lines");
                var mapPolygonData = new MapPolygonData("Polygons");
                var mapDataCollection = new MapDataCollection("Root collection");
                var nestedMapDataCollection1 = new MapDataCollection("Nested collection 1");
                var nestedMapDataCollection2 = new MapDataCollection("Nested collection 2");

                mapDataCollection.Add(mapPointData);
                mapDataCollection.Add(nestedMapDataCollection1);
                nestedMapDataCollection1.Add(mapLineData);
                nestedMapDataCollection1.Add(nestedMapDataCollection2);
                nestedMapDataCollection2.Add(mapPolygonData);

                // When
                map.Data = mapDataCollection;

                // Then
                Assert.AreEqual(3, mapView.Layers.Count);
                var featureLayers = mapView.Layers.Cast<FeatureLayer>().ToList();
                Assert.AreEqual("Points", featureLayers[0].Name);
                Assert.AreEqual("Lines", featureLayers[1].Name);
                Assert.AreEqual("Polygons", featureLayers[2].Name);
            }
        }

        [Test]
        public void GivenMapControlWithData_WhenDataSetToOtherMapDataCollection_ThenMapControlUpdated()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var mapPointData = new MapPointData("Points");
                var mapLineData = new MapLineData("Lines");
                var mapPolygonData = new MapPolygonData("Polygons");
                var mapDataCollection1 = new MapDataCollection("Collection 1");
                var mapDataCollection2 = new MapDataCollection("Collection 2");

                mapDataCollection1.Add(mapPointData);
                mapDataCollection2.Add(mapLineData);
                mapDataCollection2.Add(mapPolygonData);

                map.Data = mapDataCollection1;

                // Precondition
                Assert.AreEqual(1, mapView.Layers.Count);
                Assert.AreEqual("Points", ((FeatureLayer) mapView.Layers[0]).Name);

                // When
                map.Data = mapDataCollection2;

                // Then
                Assert.AreEqual(2, mapView.Layers.Count);
                var featureLayers = mapView.Layers.Cast<FeatureLayer>().ToList();
                Assert.AreEqual("Lines", featureLayers[0].Name);
                Assert.AreEqual("Polygons", featureLayers[1].Name);
            }
        }

        [Test]
        public void GivenMapControlWithData_WhenMapDataNotifiesChange_ThenAllLayersReused()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var mapPointData = new MapPointData("Points");
                var mapLineData = new MapLineData("Lines");
                var mapPolygonData = new MapPolygonData("Polygons");
                var mapDataCollection = new MapDataCollection("Root collection");
                var nestedMapDataCollection1 = new MapDataCollection("Nested collection 1");
                var nestedMapDataCollection2 = new MapDataCollection("Nested collection 2");

                mapDataCollection.Add(mapPointData);
                mapDataCollection.Add(nestedMapDataCollection1);
                nestedMapDataCollection1.Add(mapLineData);
                nestedMapDataCollection1.Add(nestedMapDataCollection2);
                nestedMapDataCollection2.Add(mapPolygonData);

                map.Data = mapDataCollection;

                var layersBeforeUpdate = mapView.Layers.ToList();

                // When
                mapLineData.Features = new MapFeature[0];
                mapLineData.NotifyObservers();

                // Then
                CollectionAssert.AreEqual(layersBeforeUpdate, mapView.Layers);
            }
        }

        [Test]
        public void GivenMapControlWithData_WhenMapDataRemoved_ThenCorrespondingLayerRemovedAndOtherLayersReused()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var mapPointData = new MapPointData("Points");
                var mapLineData = new MapLineData("Lines");
                var mapPolygonData = new MapPolygonData("Polygons");
                var mapDataCollection = new MapDataCollection("Root collection");
                var nestedMapDataCollection1 = new MapDataCollection("Nested collection 1");
                var nestedMapDataCollection2 = new MapDataCollection("Nested collection 2");

                mapDataCollection.Add(mapPointData);
                mapDataCollection.Add(nestedMapDataCollection1);
                nestedMapDataCollection1.Add(mapLineData);
                nestedMapDataCollection1.Add(nestedMapDataCollection2);
                nestedMapDataCollection2.Add(mapPolygonData);

                map.Data = mapDataCollection;

                var layersBeforeUpdate = mapView.Layers.ToList();

                // Precondition
                Assert.AreEqual(3, layersBeforeUpdate.Count);

                // When
                nestedMapDataCollection1.Remove(mapLineData);
                nestedMapDataCollection1.NotifyObservers();

                // Then
                Assert.AreEqual(2, mapView.Layers.Count);
                var featureLayers = mapView.Layers.Cast<FeatureLayer>().ToList();
                Assert.AreEqual("Points", featureLayers[0].Name);
                Assert.AreEqual("Polygons", featureLayers[1].Name);
                Assert.AreEqual(0, mapView.Layers.Except(layersBeforeUpdate).Count());
            }
        }

        [Test]
        public void GivenMapControlWithData_WhenMapDataAdded_ThenCorrespondingLayerAddedAndOtherLayersReused()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var mapPointData = new MapPointData("Points");
                var mapLineData = new MapLineData("Lines");
                var mapPolygonData = new MapPolygonData("Polygons");
                var mapDataCollection = new MapDataCollection("Root collection");
                var nestedMapDataCollection1 = new MapDataCollection("Nested collection 1");
                var nestedMapDataCollection2 = new MapDataCollection("Nested collection 2");

                mapDataCollection.Add(mapPointData);
                mapDataCollection.Add(nestedMapDataCollection1);
                nestedMapDataCollection1.Add(mapLineData);
                nestedMapDataCollection1.Add(nestedMapDataCollection2);
                nestedMapDataCollection2.Add(mapPolygonData);

                map.Data = mapDataCollection;

                var layersBeforeUpdate = mapView.Layers.ToList();

                // Precondition
                Assert.AreEqual(3, layersBeforeUpdate.Count);

                // When
                nestedMapDataCollection1.Insert(0, new MapPolygonData("Additional polygons"));
                nestedMapDataCollection1.NotifyObservers();

                // Then
                Assert.AreEqual(4, mapView.Layers.Count);
                var featureLayers = mapView.Layers.Cast<FeatureLayer>().ToList();
                Assert.AreEqual("Points", featureLayers[0].Name);
                Assert.AreEqual("Additional polygons", featureLayers[1].Name);
                Assert.AreEqual("Lines", featureLayers[2].Name);
                Assert.AreEqual("Polygons", featureLayers[3].Name);
                Assert.AreEqual(0, layersBeforeUpdate.Except(mapView.Layers).Count());
            }
        }

        [Test]
        public void GivenMapControlWithData_WhenMapDataMoved_ThenCorrespondingLayerMovedAndAllLayersReused()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var mapPointData = new MapPointData("Points");
                var mapLineData = new MapLineData("Lines");
                var mapPolygonData = new MapPolygonData("Polygons");
                var mapDataCollection = new MapDataCollection("Root collection");

                mapDataCollection.Add(mapPointData);
                mapDataCollection.Add(mapLineData);
                mapDataCollection.Add(mapPolygonData);

                map.Data = mapDataCollection;

                var layersBeforeUpdate = mapView.Layers.Cast<FeatureLayer>().ToList();

                // Precondition
                Assert.AreEqual(3, layersBeforeUpdate.Count);
                Assert.AreEqual("Points", layersBeforeUpdate[0].Name);
                Assert.AreEqual("Lines", layersBeforeUpdate[1].Name);
                Assert.AreEqual("Polygons", layersBeforeUpdate[2].Name);

                // When
                mapDataCollection.Remove(mapPointData);
                mapDataCollection.Add(mapPointData);
                mapDataCollection.NotifyObservers();

                // Then
                Assert.AreEqual(3, mapView.Layers.Count);
                var featureLayers = mapView.Layers.Cast<FeatureLayer>().ToList();
                Assert.AreEqual("Lines", featureLayers[0].Name);
                Assert.AreEqual("Polygons", featureLayers[1].Name);
                Assert.AreEqual("Points", featureLayers[2].Name);
                Assert.AreEqual(0, layersBeforeUpdate.Except(featureLayers).Count());
            }
        }

        [Test]
        [RequiresSTA]
        public void ZoomToAllVisibleLayers_MapInFormWithEmptyDataSet_ViewInvalidatedLayersSame()
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapDataCollection = new MapDataCollection("Collection");
                var mapData = new MapPointData("Test data");
                var invalidated = 0;
                var mapView = map.Controls.OfType<Map>().First();

                mapDataCollection.Add(mapData);

                map.Data = mapDataCollection;

                mapView.Invalidated += (sender, args) => { invalidated++; };

                Assert.AreEqual(0, invalidated, "Precondition failed: mapView.Invalidated > 0");

                // Call
                map.ZoomToAllVisibleLayers();

                // Assert
                Assert.AreEqual(0, invalidated);
                Extent expectedExtent = new Extent(0.0, 0.0, 0.0, 0.0);
                Assert.AreEqual(expectedExtent, mapView.ViewExtents);
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
                var mapFeatures = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(0.0, 0.0),
                                new Point2D(1.0, 1.0)
                            }
                        })
                    })
                };
                var mapDataCollection = new MapDataCollection("Collection");
                var mapData = new MapPointData("Test data")
                {
                    Features = mapFeatures
                };
                var mapView = map.Controls.OfType<Map>().First();
                var invalidated = 0;

                mapDataCollection.Add(mapData);

                map.Data = mapDataCollection;

                form.Controls.Add(map);

                mapView.Invalidated += (sender, args) => { invalidated++; };

                form.Show();
                Assert.AreEqual(0, invalidated, "Precondition failed: mapView.Invalidated > 0");

                // Call
                map.ZoomToAllVisibleLayers();

                // Assert
                Assert.AreEqual(2, invalidated);
                Extent expectedExtent = mapView.GetMaxExtent();

                var smallest = expectedExtent.Height < expectedExtent.Width ? expectedExtent.Height : expectedExtent.Width;
                expectedExtent.ExpandBy(smallest*padding);
                Assert.AreEqual(expectedExtent, mapView.ViewExtents);
            }
        }

        [Test]
        public void ZoomToAllVisibleLayers_NotAllLayersVisible_ZoomToVisibleLayersExtent()
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();

                map.Data = GetTestData();

                var expectedExtent = new Extent(0.0, 0.5, 1.6, 2.1);
                var smallest = expectedExtent.Height < expectedExtent.Width ? expectedExtent.Height : expectedExtent.Width;
                expectedExtent.ExpandBy(smallest*padding);

                // Precondition
                Assert.AreEqual(3, mapView.Layers.Count, "Precondition failed: mapView.Layers != 3");
                Assert.IsFalse(mapView.Layers.All(l => l.IsVisible), "Precondition failed: not all map layers should be visible.");

                // Call
                map.ZoomToAllVisibleLayers();

                // Assert
                Assert.AreNotEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
                Assert.AreEqual(expectedExtent, mapView.ViewExtents);
            }
        }

        [Test]
        [TestCase(5.0, 5.0)]
        [TestCase(5.0, 1.0)]
        [TestCase(1.0, 5.0)]
        [TestCase(double.MaxValue*0.96, double.MaxValue*0.96)]
        [TestCase(double.MaxValue, double.MaxValue)]
        public void ZoomToAllVisibleLayers_LayersOfVariousDimensions_ZoomToVisibleLayersExtent(double xMax, double yMax)
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var mapDataCollection = new MapDataCollection("Test data");

                mapDataCollection.Add(new MapPointData("Test data")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(0.0, 0.0),
                                    new Point2D(xMax, yMax)
                                }
                            })
                        })
                    }
                });

                map.Data = mapDataCollection;

                var expectedExtent = new Extent(0.0, 0.0, xMax, yMax);
                var smallest = expectedExtent.Height < expectedExtent.Width ? expectedExtent.Height : expectedExtent.Width;
                expectedExtent.ExpandBy(smallest*padding);

                // Call
                map.ZoomToAllVisibleLayers();

                // Assert
                if (double.IsInfinity(expectedExtent.Height) || double.IsInfinity(expectedExtent.Width))
                {
                    Assert.AreEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
                    Assert.AreNotEqual(expectedExtent, mapView.ViewExtents);
                }
                else
                {
                    Assert.AreNotEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
                    Assert.AreEqual(expectedExtent, mapView.ViewExtents);
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectionZoom_MouseUp_DefaultCursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionSelectionZoom = map.MapFunctions.OfType<MapFunctionSelectionZoom>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionSelectionZoom, "MouseUp", new GeoMouseArgs(new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.Default, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectionZoom_LeftMouseDown_SizeNWSECursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionSelectionZoom = map.MapFunctions.OfType<MapFunctionSelectionZoom>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionSelectionZoom, "MouseDown", new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.SizeNWSE, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(MouseButtons.Right)]
        [TestCase(MouseButtons.Middle)]
        public void SelectionZoom_OtherThanMouseLeftDownAndMapNotBusy_DefaultCursorSet(MouseButtons mouseButton)
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionSelectionZoom = map.MapFunctions.OfType<MapFunctionSelectionZoom>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionSelectionZoom, "MouseDown", new GeoMouseArgs(new MouseEventArgs(mouseButton, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.Default, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectionZoom_Activated_DefaultCursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionSelectionZoom = map.MapFunctions.OfType<MapFunctionSelectionZoom>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                mapFunctionSelectionZoom.Activate();

                // Assert
                Assert.AreEqual(Cursors.Default, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void Panning_MouseUp_DefaultCursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionPan, "MouseUp", new GeoMouseArgs(new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.Default, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void Panning_Activated_DefaultCursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                mapFunctionPan.Activate();

                // Assert
                Assert.AreEqual(Cursors.Default, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void Panning_LeftMouseDown_HandCursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionPan, "MouseDown", new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.Hand, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void Panning_MiddleMouseDown_HandCursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionPan, "MouseDown", new GeoMouseArgs(new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.Hand, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void Panning_RightMouseDown_DefaultCursorSet()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();

                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionPan, "MouseDown", new GeoMouseArgs(new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.Default, map.Cursor);
            }
        }

        [Test]
        [RequiresSTA]
        public void ToggleRectangleZooming_Always_CorrectlySetsMapFunctions()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();
                var mapFunctionSelectionZoom = map.MapFunctions.OfType<MapFunctionSelectionZoom>().First();

                // Call
                mapControl.ToggleRectangleZooming();

                // Assert
                Assert.IsTrue(mapFunctionSelectionZoom.Enabled);
                Assert.IsFalse(mapFunctionPan.Enabled);
                Assert.AreEqual(FunctionMode.None, map.FunctionMode);
            }
        }

        [Test]
        [RequiresSTA]
        public void TogglePanning_Always_CorrectlySetsMapFunctions()
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                mapControl.ToggleRectangleZooming();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();
                var mapFunctionSelectionZoom = map.MapFunctions.OfType<MapFunctionSelectionZoom>().First();

                // Call
                mapControl.TogglePanning();

                // Assert
                Assert.IsTrue(mapFunctionPan.Enabled);
                Assert.IsFalse(mapFunctionSelectionZoom.Enabled);
                Assert.AreEqual(FunctionMode.Pan, map.FunctionMode);
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(MouseButtons.Right)]
        [TestCase(MouseButtons.Middle)]
        public void SelectionZoom_OtherThanMouseLeftDownAndMapBusy_SizeNWSECursorSet(MouseButtons mouseButton)
        {
            using (var form = new Form())
            {
                // Setup
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) new ControlTester("Map").TheObject;
                var mapFunctionSelectionZoom = map.MapFunctions.OfType<MapFunctionSelectionZoom>().First();

                map.IsBusy = true;
                map.Cursor = Cursors.WaitCursor;

                // Call
                EventHelper.RaiseEvent(mapFunctionSelectionZoom, "MouseDown", new GeoMouseArgs(new MouseEventArgs(mouseButton, 1, 2, 3, 4), map));

                // Assert
                Assert.AreEqual(Cursors.SizeNWSE, map.Cursor);
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
                                string.Format("Precondition failed: IsRectangleZoomingEnabled is {0}", map.IsRectangleZoomingEnabled));
                Assert.AreEqual(!isRectangleZooming, map.IsPanningEnabled,
                                string.Format("Precondition failed: IsPanningEnabled is {0}", map.IsPanningEnabled));

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
        public void TogglePanning_Always_ChangesState(bool isPanning)
        {
            // Setup
            using (var map = new MapControl())
            {
                if (!isPanning)
                {
                    map.ToggleRectangleZooming();
                }

                // Precondition
                Assert.AreEqual(isPanning, map.IsPanningEnabled,
                                string.Format("Precondition failed: IsPanningEnabled is {0}", map.IsPanningEnabled));
                Assert.AreEqual(!isPanning, map.IsRectangleZoomingEnabled,
                                string.Format("Precondition failed: IsRectangleZoomingEnabled is {0}", map.IsRectangleZoomingEnabled));

                // Call
                map.TogglePanning();

                // Assert
                Assert.IsTrue(map.IsPanningEnabled);
                Assert.IsFalse(map.IsRectangleZoomingEnabled);
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

        private static MapDataCollection GetTestData()
        {
            var mapDataCollection = new MapDataCollection("Test data");

            mapDataCollection.Add(new MapPointData("Test data")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(1.5, 2)
                            }
                        }),
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(1.1, 1)
                            }
                        }),
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(0.8, 0.5)
                            }
                        })
                    })
                }
            });

            mapDataCollection.Add(new MapLineData("Test data")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(0.0, 1.1),
                                new Point2D(1.0, 2.1),
                                new Point2D(1.6, 1.6)
                            }
                        })
                    })
                }
            });

            mapDataCollection.Add(new MapPolygonData("Test data")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(1.0, 1.3),
                                new Point2D(3.0, 2.6),
                                new Point2D(5.6, 1.6)
                            }
                        })
                    })
                },
                IsVisible = false
            });

            return mapDataCollection;
        }
    }
}