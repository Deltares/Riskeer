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
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;
using BruTile;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.DotSpatial.MapFunctions;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Extent = DotSpatial.Data.Extent;

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
                Assert.IsNull(map.BackgroundMapData);
                Assert.IsTrue(map.IsPanningEnabled);
                Assert.IsFalse(map.IsRectangleZoomingEnabled);
                Assert.IsTrue(map.IsMouseCoordinatesVisible);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void DefaultConstructor_MapCorrectlyInitialized()
        {
            using (var form = new Form())
            {
                // Call
                var mapControl = new MapControl();
                form.Controls.Add(mapControl);
                form.Show();

                var map = (Map) form.Controls.Find("Map", true)[0];

                // Assert
                Assert.AreEqual(MapDataConstants.FeatureBasedMapDataCoordinateSystem, map.Projection);
                Assert.AreEqual(ActionMode.Never, map.ProjectionModeDefine);

                Assert.AreEqual(9, map.MapFunctions.Count);
                Assert.AreEqual(1, map.MapFunctions.OfType<MapFunctionPan>().Count());
                Assert.AreEqual(1, map.MapFunctions.OfType<MapFunctionSelectionZoom>().Count());

                Assert.AreEqual(KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel, map.Projection);
                Assert.AreEqual(ActionMode.Never, map.ProjectionModeDefine);
                Assert.IsTrue(map.ZoomOutFartherThanMaxExtent);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenMapControlWithoutBackgroundMapData_WhenBackgroundMapDataSet_ThenMapControlUpdated(bool isVisible)
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();

            var mapDataContainer = new BackgroundMapDataContainer
            {
                IsVisible = isVisible,
                Transparency = (RoundedDouble) 0.25,
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl())
            {
                // When
                map.BackgroundMapData = mapDataContainer;

                // Then
                var mapView = map.Controls.OfType<Map>().First();
                Assert.AreEqual(1, mapView.Layers.Count);
                BruTileLayer bruTileLayer = (BruTileLayer) mapView.Layers[0];
                Assert.AreEqual(isVisible, bruTileLayer.IsVisible);
                Assert.AreEqual(backgroundMapData.Transparency.Value, bruTileLayer.Transparency);

                Assert.IsTrue(bruTileLayer.Projection.Equals(mapView.Projection),
                              "The background layer's Project should define the Projection of 'mapView'.");
            }
        }

        [Test]
        [TestCaseSource(nameof(GetProblematicTileSourceFactoryTestCaseData), new object[]
        {
            "MapControlWithoutBackgroundMapData"
        })]
        public void GivenMapControlWithoutBackgroundMapData_WhenTileSourceFactoryProblematic_ThenLogErrorAndDoNotAddBackgroundLayer(ITileSourceFactory problematicFactory)
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var container = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(problematicFactory))
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var originalProjection = mapView.Projection;

                // When
                Action call = () => map.BackgroundMapData = container;

                // Then
                const string expectedMessage = "Verbinden met WMTS is mislukt waardoor geen kaartgegevens ingeladen kunnen worden. De achtergrondkaart kan nu niet getoond worden.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                Assert.AreEqual(0, mapView.Layers.Count);

                Assert.AreSame(originalProjection, mapView.Projection);
            }
        }

        [Test]
        public void GivenMapControlWithoutBackgroundMapData_WhenSettingBackgroundAndFailingToCreateCache_ThenLogErrorAndDoNotAddBackgroundLayer()
        {
            DoWhileTileCacheRootLocked(() =>
            {
                // Given
                WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
                var mapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = backgroundMapData
                };

                using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
                using (var map = new MapControl())
                {
                    var mapView = map.Controls.OfType<Map>().First();
                    var originalProjection = mapView.Projection;

                    // When
                    Action call = () => map.BackgroundMapData = mapDataContainer;

                    // Then
                    const string expectedMessage = "Configuratie van kaartgegevens hulpbestanden is mislukt. De achtergrondkaart kan nu niet getoond worden.";
                    TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                    Assert.AreEqual(0, mapView.Layers.Count);

                    Assert.AreSame(originalProjection, mapView.Projection);
                }
            });
        }

        [Test]
        [TestCaseSource(nameof(GetProblematicTileSourceFactoryTestCaseData), new object[]
        {
            "MapWithFailedBackgroundMapDataThenSuccessfulAtNotify"
        })]
        public void GivenMapControlWithFailingBackgroundMapData_WhenBackgroundNotifiesAndInitializationSuccessful_ThenBackgroundLayerAdded(ITileSourceFactory problematicFactory)
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(problematicFactory))
            using (var map = new MapControl())
            {
                // Precondition
                Action setAndCauseFailingInitialization = () => map.BackgroundMapData = mapDataContainer;
                const string expectedMessage = "Verbinden met WMTS is mislukt waardoor geen kaartgegevens ingeladen kunnen worden. De achtergrondkaart kan nu niet getoond worden.";
                TestHelper.AssertLogMessageIsGenerated(setAndCauseFailingInitialization, expectedMessage, 1);

                using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
                {
                    // When
                    backgroundMapData.NotifyObservers();

                    // Then
                    var mapView = map.Controls.OfType<Map>().First();
                    Assert.AreEqual(1, mapView.Layers.Count);
                    IMapLayer backgroundLayer = mapView.Layers[0];

                    Assert.IsInstanceOf<BruTileLayer>(backgroundLayer);
                    Assert.AreSame(backgroundLayer.Projection, mapView.Projection);
                }
            }
        }

        [Test]
        public void GivenMapControlWithFailedCacheForBackgroundMapData_WhenBackgroundNotifiesObserversAndLayerInitializationSuccessful_ThenBackgroundLayerAdded()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl())
            {
                DoWhileTileCacheRootLocked(() =>
                {
                    // Precondition
                    Action setAndCauseCacheInitializationFailure = () => map.BackgroundMapData = mapDataContainer;
                    const string expectedMessage = "Configuratie van kaartgegevens hulpbestanden is mislukt. De achtergrondkaart kan nu niet getoond worden.";
                    TestHelper.AssertLogMessageIsGenerated(setAndCauseCacheInitializationFailure, expectedMessage, 1);
                });

                // When
                backgroundMapData.NotifyObservers();

                // Then
                var mapView = map.Controls.OfType<Map>().First();
                Assert.AreEqual(1, mapView.Layers.Count);
                IMapLayer backgroundLayer = mapView.Layers[0];

                Assert.IsInstanceOf<BruTileLayer>(backgroundLayer);
                Assert.AreSame(backgroundLayer.Projection, mapView.Projection);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetProblematicTileSourceFactoryTestCaseData), new object[]
        {
            "MapWithFailedBackgroundMapDataThenFailedAgainAtNotify"
        })]
        public void GivenMapControlWithFailedBackgroundMapData_WhenBackgroundNotifiesObservers_ThenFailedInitializationShouldNotGenerateLogMessage(ITileSourceFactory problematicFactory)
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(problematicFactory))
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var originalProjection = mapView.Projection;

                // Precondition
                Action setAndCauseFailToInitializeLayer = () => map.BackgroundMapData = mapDataContainer;
                const string expectedMessage = "Verbinden met WMTS is mislukt waardoor geen kaartgegevens ingeladen kunnen worden. De achtergrondkaart kan nu niet getoond worden.";
                TestHelper.AssertLogMessageIsGenerated(setAndCauseFailToInitializeLayer, expectedMessage, 1);

                // When
                Action call = () => backgroundMapData.NotifyObservers();

                // Then
                TestHelper.AssertLogMessagesCount(call, 0);

                Assert.AreEqual(0, mapView.Layers.Count);

                Assert.AreSame(originalProjection, mapView.Projection);
            }
        }

        [Test]
        public void GivenMapControlWithFailedCacheforBackgroundMapData_WhenBackgroundNotifiesObservers_ThenFailedInitializationShouldNotGenerateLogMessage()
        {
            DoWhileTileCacheRootLocked(() =>
            {
                // Given
                WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
                var mapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = backgroundMapData
                };

                using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
                using (var map = new MapControl())
                {
                    var mapView = map.Controls.OfType<Map>().First();
                    var originalProjection = mapView.Projection;

                    // Precondition
                    Action setAndCauseCacheInitializationFailure = () => map.BackgroundMapData = mapDataContainer;
                    const string expectedMessage = "Configuratie van kaartgegevens hulpbestanden is mislukt. De achtergrondkaart kan nu niet getoond worden.";
                    TestHelper.AssertLogMessageIsGenerated(setAndCauseCacheInitializationFailure, expectedMessage, 1);

                    // When
                    Action call = () => backgroundMapData.NotifyObservers();

                    // Then
                    TestHelper.AssertLogMessagesCount(call, 0);

                    Assert.AreEqual(0, mapView.Layers.Count);

                    Assert.AreSame(originalProjection, mapView.Projection);
                }
            });
        }

        [Test]
        public void GivenMapControlWithoutBackgroundMapData_WhenUnconfiguredBackgroundMapDataSet_NoChangedToMap()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                ProjectionInfo originalProjection = mapView.Projection;

                WmtsMapData backgroundMapData = WmtsMapData.CreateUnconnectedMapData();
                var mapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = backgroundMapData
                };

                // When
                map.BackgroundMapData = mapDataContainer;

                // Then
                Assert.AreEqual(0, mapView.Layers.Count);

                Assert.AreEqual(originalProjection, mapView.Projection);
            }
        }

        [Test]
        public void GivenMapControlWithBackgroundMapData_WhenBackgroundMapDataSetToUnconfigured_ThenMapControlUpdated()
        {
            // Given
            WmtsMapData originalBackGroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                IsVisible = true,
                Transparency = (RoundedDouble)0.25,
                MapData = originalBackGroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(originalBackGroundMapData))
            using (var map = new MapControl())

            {
                var mapView = map.Controls.OfType<Map>().First();
                ProjectionInfo originalProjection = mapView.Projection;

                map.BackgroundMapData = mapDataContainer;

                // Precondition
                Assert.AreEqual(1, mapView.Layers.Count);
                var layer = (BruTileLayer) mapView.Layers[0];
                Assert.AreEqual(true, layer.IsVisible);
                Assert.AreEqual(0.25, layer.Transparency);
                Assert.AreEqual(layer.Projection, mapView.Projection);

                // When
                var newBackGroundMapData = WmtsMapData.CreateUnconnectedMapData();
                var newMapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = newBackGroundMapData
                };
                map.BackgroundMapData = newMapDataContainer;

                // Then
                Assert.AreEqual(0, mapView.Layers.Count);
                Assert.IsTrue(originalProjection.Equals(mapView.Projection));
            }
        }

        [Test]
        public void GivenMapControlWithUnconfiguredBackgroundMapData_WhenBackgroundMapDataSet_ThenMapControlUpdated()
        {
            // Given
            var newBackGroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var newMapDataContainer = new BackgroundMapDataContainer
            {
                IsVisible = true,
                Transparency = (RoundedDouble) 0.75,
                MapData = newBackGroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(newBackGroundMapData))
            using (var map = new MapControl())
            {
                WmtsMapData originalBackGroundMapData = WmtsMapData.CreateUnconnectedMapData();
                var originalMapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = originalBackGroundMapData
                };

                var mapView = map.Controls.OfType<Map>().First();
                ProjectionInfo originalProjection = mapView.Projection;

                map.BackgroundMapData = originalMapDataContainer;

                // Precondition
                Assert.AreEqual(0, mapView.Layers.Count);
                Assert.IsTrue(originalProjection.Equals(mapView.Projection));

                // When
                map.BackgroundMapData = newMapDataContainer;

                // Then
                Assert.AreEqual(1, mapView.Layers.Count);
                var layer = (BruTileLayer) mapView.Layers[0];
                Assert.AreEqual(true, layer.IsVisible);
                Assert.AreEqual(0.75, layer.Transparency);
                Assert.IsTrue(layer.Projection.Equals(mapView.Projection));
            }
        }

        [Test]
        public void GivenMapControlWithBackgroundData_WhenVisibilityPropertiesChangeAndNotified_ThenBackgroundLayerReused()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                IsVisible = true,
                Transparency = (RoundedDouble) 0.25,
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl
            {
                BackgroundMapData = mapDataContainer
            })
            {
                var mapView = map.Controls.OfType<Map>().First();
                IMapLayer[] layersBeforeUpdate = mapView.Layers.ToArray();

                // When
                const bool newVisibilityValue = false;
                const double newTransparencyValue = 0.75;
                backgroundMapData.IsVisible = newVisibilityValue;
                backgroundMapData.Transparency = (RoundedDouble) newTransparencyValue;
                backgroundMapData.NotifyObservers();

                // Then
                CollectionAssert.AreEqual(layersBeforeUpdate, mapView.Layers);
                var backgroundLayer = (BruTileLayer) layersBeforeUpdate[0];
                Assert.AreEqual(newVisibilityValue, backgroundLayer.IsVisible);
                Assert.AreEqual(newTransparencyValue, backgroundLayer.Transparency);
            }
        }

        [Test]
        public void GivenMapWithUnconfiguredBackgroundAndMapDataCollection_WhenBackgroundMapDataConfigured_ThenLayerIsReusedAndUpdatedFeaturesReprojected()
        {
            // Given
            var newBackgroundMapData = WmtsMapData.CreateAlternativePdokMapData();
            var startingBackgroundMapData = WmtsMapData.CreateUnconnectedMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = startingBackgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(newBackgroundMapData))
            using (var map = new MapControl
            {
                BackgroundMapData = mapDataContainer
            })
            {
                var mapView = map.Controls.OfType<Map>().First();

                var mapPointData = new MapPointData("Points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                };
                var mapDataCollection = new MapDataCollection("Root collection");
                mapDataCollection.Add(mapPointData);

                map.Data = mapDataCollection;

                // Precondition
                IMapLayer[] layersBeforeUpdate = mapView.Layers.ToArray();
                var pointFeatureLayer = (FeatureLayer) layersBeforeUpdate[0];
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(1.1, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
                Assert.AreEqual(2.2, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);

                // When
                startingBackgroundMapData.Configure(newBackgroundMapData.SourceCapabilitiesUrl,
                                                    newBackgroundMapData.SelectedCapabilityIdentifier,
                                                    newBackgroundMapData.PreferredFormat);
                startingBackgroundMapData.NotifyObservers();

                // Then
                Assert.AreEqual(2, mapView.Layers.Count);
                CollectionAssert.AreEqual(layersBeforeUpdate, mapView.Layers.Skip(1));
                Assert.IsInstanceOf<BruTileLayer>(mapView.Layers[0]);
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(523414.9114786592, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                Assert.AreEqual(5313600.4932731427, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
            }
        }

        [Test]
        public void GivenMapWithBackgroundAndMapDataCollection_WhenBackgroundMapDataConfigurationRemoved_ThenLayerIsReusedAndUpdatedFeaturesReprojected()
        {
            // Given
            var startingBackgroundMapData = WmtsMapData.CreateAlternativePdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = startingBackgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(startingBackgroundMapData))
            using (var map = new MapControl
            {
                BackgroundMapData = mapDataContainer
            })
            {
                var mapView = map.Controls.OfType<Map>().First();

                var mapPointData = new MapPointData("Points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                };
                var mapDataCollection = new MapDataCollection("Root collection");
                mapDataCollection.Add(mapPointData);

                map.Data = mapDataCollection;

                // Precondition
                Assert.AreEqual(2, mapView.Layers.Count);
                IMapLayer[] layersBeforeUpdate = mapView.Layers.ToArray();
                Assert.IsInstanceOf<BruTileLayer>(mapView.Layers[0]);
                var pointFeatureLayer = (FeatureLayer) layersBeforeUpdate[1];
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(523414.9114786592, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                Assert.AreEqual(5313600.4932731427, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");

                // When
                startingBackgroundMapData.RemoveConfiguration();
                startingBackgroundMapData.NotifyObservers();

                // Then
                Assert.IsTrue(MapDataConstants.FeatureBasedMapDataCoordinateSystem.Equals(mapView.Projection));
                Assert.AreEqual(1, mapView.Layers.Count);
                CollectionAssert.DoesNotContain(mapView.Layers, layersBeforeUpdate[0]);
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(1.1, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X, 1e-6,
                                "Minimal drift is acceptable (if it becomes a problem, we need to keep original coordinates in the layer).");
                Assert.AreEqual(2.2, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y, 1e-6,
                                "Minimal drift is acceptable (if it becomes a problem, we need to keep original coordinates in the layer).");
            }
        }

        [Test]
        public void GivenMapControlWithData_WhenBackgroundMapDataSet_ThenDataLayersReprojected()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateAlternativePdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl())
            {
                var mapDataCollection = new MapDataCollection("A");
                mapDataCollection.Add(new MapPointData("points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                });
                map.Data = mapDataCollection;

                // Precondition
                var mapView = map.Controls.OfType<Map>().First();
                var pointFeatureLayer = (FeatureLayer) mapView.Layers[0];
                Assert.AreEqual(1, mapView.Layers.Count);
                Assert.AreEqual(1.1, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
                Assert.AreEqual(2.2, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);

                // When
                map.BackgroundMapData = mapDataContainer;

                // Then
                Assert.AreEqual(2, mapView.Layers.Count);
                Assert.IsInstanceOf<BruTileLayer>(mapView.Layers[0]);
                pointFeatureLayer = (FeatureLayer) mapView.Layers[1];
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(523414.9114786592, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                Assert.AreEqual(5313600.4932731427, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
            }
        }

        [Test]
        public void GivenMapControlWithData_WhenUnconfiguredBackgroundMapDataSet_NoChangesToLayerConfiguration()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateUnconnectedMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl())
            {
                var mapDataCollection = new MapDataCollection("A");
                mapDataCollection.Add(new MapPointData("points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                });
                map.Data = mapDataCollection;

                // Precondition
                var mapView = map.Controls.OfType<Map>().First();
                var pointFeatureLayer = (FeatureLayer) mapView.Layers[0];
                Assert.AreEqual(1, mapView.Layers.Count);
                Assert.AreEqual(1.1, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
                Assert.AreEqual(2.2, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);

                // When
                map.BackgroundMapData = mapDataContainer;

                // Then
                Assert.AreEqual(1, mapView.Layers.Count);
                pointFeatureLayer = (FeatureLayer) mapView.Layers[0];
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(1.1, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
                Assert.AreEqual(2.2, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);
            }
        }

        [Test]
        public void GivenMapControlWithDataAndBackground_WhenDifferentBackgroundMapDataSet_LayersReprojected()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl())
            {
                var mapDataCollection = new MapDataCollection("A");
                mapDataCollection.Add(new MapPointData("points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                });
                map.Data = mapDataCollection;
                map.BackgroundMapData = mapDataContainer;

                // Precondition
                var mapView = map.Controls.OfType<Map>().First();
                Assert.AreEqual(2, mapView.Layers.Count);
                Assert.IsInstanceOf<BruTileLayer>(mapView.Layers[0]);
                var pointFeatureLayer = (FeatureLayer) mapView.Layers[1];
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(1.1, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
                Assert.AreEqual(2.2, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);

                // When
                var differentBackgroundMapData = WmtsMapData.CreateAlternativePdokMapData();
                var differentMapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = differentBackgroundMapData
                };
                using (new UseCustomTileSourceFactoryConfig(differentBackgroundMapData))
                {
                    map.BackgroundMapData = differentMapDataContainer;

                    // Then
                    Assert.AreEqual(2, mapView.Layers.Count);
                    Assert.IsInstanceOf<BruTileLayer>(mapView.Layers[0]);
                    pointFeatureLayer = (FeatureLayer) mapView.Layers[1];
                    Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                    Assert.AreEqual(523414.9114786592, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                    "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                    Assert.AreEqual(5313600.4932731427, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                    "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                }
            }
        }

        [Test]
        public void GivenMapControlWithDataAndBackground_WhenUnconfiguredBackgroundMapDataSet_LayersReprojected()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateAlternativePdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl())
            {
                var mapDataCollection = new MapDataCollection("A");
                mapDataCollection.Add(new MapPointData("points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                });
                map.Data = mapDataCollection;
                map.BackgroundMapData = mapDataContainer;

                // Precondition
                var mapView = map.Controls.OfType<Map>().First();
                Assert.AreEqual(2, mapView.Layers.Count);
                Assert.IsInstanceOf<BruTileLayer>(mapView.Layers[0]);
                var pointFeatureLayer = (FeatureLayer) mapView.Layers[1];
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(523414.9114786592, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                Assert.AreEqual(5313600.4932731427, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");

                // When
                var differentBackgroundMapData = WmtsMapData.CreateUnconnectedMapData();
                var differentMapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = differentBackgroundMapData
                };
                using (new UseCustomTileSourceFactoryConfig(differentBackgroundMapData))
                {
                    map.BackgroundMapData = differentMapDataContainer;

                    // Then
                    Assert.AreEqual(1, mapView.Layers.Count);
                    pointFeatureLayer = (FeatureLayer) mapView.Layers[0];
                    Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                    Assert.AreEqual(1.1, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
                    Assert.AreEqual(2.2, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);
                }
            }
        }

        [Test]
        public void GivenMapControlWithoutData_WhenDataSetToMapDataCollection_ThenMapControlUpdated()
        {
            // Given
            using (var map = new MapControl())
            {
                var mapPointData = new MapPointData("Points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                };
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
                var mapView = map.Controls.OfType<Map>().First();
                Assert.AreEqual(3, mapView.Layers.Count);
                var featureLayers = mapView.Layers.Cast<FeatureLayer>().ToList();
                Assert.AreEqual("Points", featureLayers[0].Name);
                Assert.IsTrue(mapView.Projection.Equals(featureLayers[0].Projection));
                Assert.AreEqual(1.1, featureLayers[0].FeatureSet.Features[0].BasicGeometry.Coordinates[0].X);
                Assert.AreEqual(2.2, featureLayers[0].FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y);
                Assert.AreEqual("Lines", featureLayers[1].Name);
                Assert.IsTrue(mapView.Projection.Equals(featureLayers[1].Projection));
                Assert.AreEqual("Polygons", featureLayers[2].Name);
                Assert.IsTrue(mapView.Projection.Equals(featureLayers[2].Projection));
            }
        }

        [Test]
        public void GivenMapControlWithBackground_WhenDataSetToMapDataCollection_FeatureLayersAreReprojected()
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateAlternativePdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl
            {
                BackgroundMapData = mapDataContainer
            })
            {
                var mapPointData = new MapPointData("Points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                };
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
                var mapView = map.Controls.OfType<Map>().First();
                Assert.AreEqual(4, mapView.Layers.Count);
                var featureLayers = mapView.Layers.Skip(1).Cast<FeatureLayer>().ToList();
                Assert.AreEqual("Points", featureLayers[0].Name);
                Assert.IsTrue(mapView.Projection.Equals(featureLayers[0].Projection));
                Assert.AreEqual(523414.9114786592, featureLayers[0].FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                Assert.AreEqual(5313600.4932731427, featureLayers[0].FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                Assert.AreEqual("Lines", featureLayers[1].Name);
                Assert.IsTrue(mapView.Projection.Equals(featureLayers[1].Projection));
                Assert.AreEqual("Polygons", featureLayers[2].Name);
                Assert.IsTrue(mapView.Projection.Equals(featureLayers[2].Projection));
            }
        }

        [Test]
        [TestCaseSource(nameof(GetProblematicTileSourceFactoryTestCaseData), new object[]
        {
            "SettingBothBackgroundAndRegularMapDataWhileBackgroundFailed"
        })]
        public void GivenMapControl_WhenBackgroundAndThenMapDataSetWhileTileSourceFactoryProblematic_MapControlUpdated(ITileSourceFactory problematicFactory)
        {
            // Given
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(problematicFactory))
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();
                var originalProjection = mapView.Projection;

                var mapPointData = new MapPointData("Points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                };
                var mapDataCollection = new MapDataCollection("Root collection");
                mapDataCollection.Add(mapPointData);

                // When
                Action call = () =>
                {
                    map.BackgroundMapData = mapDataContainer;
                    map.Data = mapDataCollection;
                };

                // Then
                const string expectedMessage = "Verbinden met WMTS is mislukt waardoor geen kaartgegevens ingeladen kunnen worden. De achtergrondkaart kan nu niet getoond worden.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1); // Message should only the generated once!

                Assert.AreEqual(1, mapView.Layers.Count);
                var pointsLayer = (FeatureLayer) mapView.Layers[0];
                Assert.AreEqual(mapPointData.Name, pointsLayer.Name);
                Assert.AreSame(originalProjection, pointsLayer.Projection);

                Assert.AreSame(originalProjection, mapView.Projection);
            }
        }

        [Test]
        public void GivenMapControl_WhenBackgroundAndThenMapDataSetAndFailingToCreateCache_MapControlUpdated()
        {
            DoWhileTileCacheRootLocked(() =>
            {
                // Given
                WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();
                var mapDataContainer = new BackgroundMapDataContainer
                {
                    MapData = backgroundMapData
                };

                using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
                using (var map = new MapControl())
                {
                    var mapView = map.Controls.OfType<Map>().First();
                    var originalProjection = mapView.Projection;

                    var mapPointData = new MapPointData("Points")
                    {
                        Features = new[]
                        {
                            new MapFeature(new[]
                            {
                                new MapGeometry(new[]
                                {
                                    new[]
                                    {
                                        new Point2D(1.1, 2.2)
                                    }
                                })
                            })
                        }
                    };
                    var mapDataCollection = new MapDataCollection("Root collection");
                    mapDataCollection.Add(mapPointData);

                    // When
                    Action call = () =>
                    {
                        map.BackgroundMapData = mapDataContainer;
                        map.Data = mapDataCollection;
                    };

                    // Then
                    const string expectedMessage = "Configuratie van kaartgegevens hulpbestanden is mislukt. De achtergrondkaart kan nu niet getoond worden.";
                    TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                    Assert.AreEqual(1, mapView.Layers.Count);
                    var pointsLayer = (FeatureLayer) mapView.Layers[0];
                    Assert.AreEqual(mapPointData.Name, pointsLayer.Name);
                    Assert.AreSame(originalProjection, pointsLayer.Projection);

                    Assert.AreSame(originalProjection, mapView.Projection);
                }
            });
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

                IMapLayer[] layersBeforeUpdate = mapView.Layers.ToArray();

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
        public void GivenMapWithBackgroundAndMapDataCollection_WhenMapDataCollectionFeaturesUpdates_ThenLayerIsReusedAndUpdatedFeaturesReprojected()
        {
            // Given
            var backgroundMapData = WmtsMapData.CreateAlternativePdokMapData();
            var mapDataContainer = new BackgroundMapDataContainer
            {
                MapData = backgroundMapData
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new MapControl
            {
                BackgroundMapData = mapDataContainer
            })
            {
                var mapView = map.Controls.OfType<Map>().First();

                var mapPointData = new MapPointData("Points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                };
                var mapDataCollection = new MapDataCollection("Root collection");
                mapDataCollection.Add(mapPointData);

                map.Data = mapDataCollection;

                // Precondition
                IMapLayer[] layersBeforeUpdate = mapView.Layers.ToArray();
                var pointFeatureLayer = (FeatureLayer) layersBeforeUpdate[1];
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(523414.9114786592, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");
                Assert.AreEqual(5313600.4932731427, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=1.1000000&y=2.2000000).");

                // When
                mapPointData.Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(12345.6789, 9876.54321)
                            }
                        })
                    })
                };
                mapPointData.NotifyObservers();

                // Then
                CollectionAssert.AreEqual(layersBeforeUpdate, mapView.Layers);
                Assert.IsTrue(mapView.Projection.Equals(pointFeatureLayer.Projection));
                Assert.AreEqual(535419.87415209203, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].X,
                                "Coordinate does not match. (Ball park expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=12345.6789000&y=9876.5432100).");
                Assert.AreEqual(5323846.0863087801, pointFeatureLayer.FeatureSet.Features[0].BasicGeometry.Coordinates[0].Y,
                                "Coordinate does not match (Estimate of expected value can be calculated from https://epsg.io/transform#s_srs=28992&t_srs=25831&x=12345.6789000&y=9876.5432100).");
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
                ExtendWithExpectedMargin(expectedExtent);

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
                ExtendWithExpectedMargin(expectedExtent);

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
        [TestCase(double.MaxValue * 0.96, double.MaxValue * 0.96)]
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
                ExtendWithExpectedMargin(expectedExtent);

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
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleLayers_WithNonChildMapData_ThrowArgumentException()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Collection");
            using (var map = new MapControl
            {
                Data = mapDataCollection
            })
            {
                var mapData = new MapPointData("Test data");

                // Call
                TestDelegate call = () => map.ZoomToAllVisibleLayers(mapData);

                // Assert
                string message = "Can only zoom to MapData that is part of this MapControls drawn mapData.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
                Assert.AreEqual("mapData", paramName);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleLayers_MapInFormWithEmptyDataSetAndForChildMapData_ViewInvalidatedLayersSame()
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
                map.ZoomToAllVisibleLayers(mapData);

                // Assert
                Assert.AreEqual(0, invalidated);
                Extent expectedExtent = new Extent(0.0, 0.0, 0.0, 0.0);
                Assert.AreEqual(expectedExtent, mapView.ViewExtents);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleLayers_MapInFormForChildMapData_ViewInvalidatedLayersSame()
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
                map.ZoomToAllVisibleLayers(mapData);

                // Assert
                Assert.AreEqual(2, invalidated);

                Extent expectedExtent = mapView.GetMaxExtent();
                ExtendWithExpectedMargin(expectedExtent);

                Assert.AreEqual(expectedExtent, mapView.ViewExtents);
            }
        }

        [Test]
        public void ZoomToAllVisibleLayers_ForVisibleChildMapData_ZoomToVisibleLayerExtent()
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();

                MapDataCollection dataCollection = GetTestData();
                map.Data = dataCollection;

                MapData mapData = dataCollection.Collection.ElementAt(0);
                var expectedExtent = GetExpectedExtent((FeatureBasedMapData) mapData);
                ExtendWithExpectedMargin(expectedExtent);

                // Precondition
                Assert.IsTrue(mapData.IsVisible);

                // Call
                map.ZoomToAllVisibleLayers(mapData);

                // Assert
                Assert.AreNotEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
                Assert.AreEqual(expectedExtent, mapView.ViewExtents);
            }
        }

        [Test]
        public void ZoomToAllVisibleLayers_ForInvisibleChildMapData_DoNotChangeViewExtentsOfMapView()
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();

                MapDataCollection dataCollection = GetTestData();
                map.Data = dataCollection;

                MapData mapData = dataCollection.Collection.ElementAt(2);
                Extent expectedExtent = GetExpectedExtent((FeatureBasedMapData) mapData);
                ExtendWithExpectedMargin(expectedExtent);

                // Precondition
                Assert.IsFalse(mapData.IsVisible);

                var originalViewExtents = (Extent) mapView.ViewExtents.Clone();

                // Call
                map.ZoomToAllVisibleLayers(mapData);

                // Assert
                Assert.AreNotEqual(expectedExtent, mapView.ViewExtents,
                                   "Do not set extent based on the invisible layer.");
                Assert.AreEqual(originalViewExtents, mapView.ViewExtents);
            }
        }

        [Test]
        [TestCase(5.0, 5.0)]
        [TestCase(5.0, 1.0)]
        [TestCase(1.0, 5.0)]
        [TestCase(double.MaxValue * 0.96, double.MaxValue * 0.96)]
        [TestCase(double.MaxValue, double.MaxValue)]
        public void ZoomToAllVisibleLayers_ForMapDataOfVariousDimensions_ZoomToVisibleLayerExtent(double xMax, double yMax)
        {
            // Setup
            using (var map = new MapControl())
            {
                var mapView = map.Controls.OfType<Map>().First();

                MapData mapData = new MapPointData("Test data")
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
                };
                var mapDataCollection = new MapDataCollection("Test data collection");
                mapDataCollection.Add(mapData);

                map.Data = mapDataCollection;

                var expectedExtent = new Extent(0.0, 0.0, xMax, yMax);
                ExtendWithExpectedMargin(expectedExtent);

                // Call
                map.ZoomToAllVisibleLayers(mapData);

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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
        [Apartment(ApartmentState.STA)]
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
                                $"Precondition failed: IsPanningEnabled is {map.IsPanningEnabled}");
                Assert.AreEqual(!isPanning, map.IsRectangleZoomingEnabled,
                                $"Precondition failed: IsRectangleZoomingEnabled is {map.IsRectangleZoomingEnabled}");

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

        /// <summary>
        /// Generates <see cref="TestCaseData"/> containing problematic <see cref="ITileSourceFactory"/>.
        /// </summary>
        /// <param name="prefix">The test-name prefix.</param>
        /// <returns>The data for the test cases.</returns>
        /// <remarks>Some test runners, like TeamCity, cannot properly deal with reuse of
        /// <see cref="TestCaseData"/> sources where the source defines a name of the test,
        /// as these testrunners to not display tests in hierarchical form.</remarks>
        private static IEnumerable<TestCaseData> GetProblematicTileSourceFactoryTestCaseData(string prefix)
        {
            var factoryWithoutRequiredTileSource = MockRepository.GenerateStub<ITileSourceFactory>();
            factoryWithoutRequiredTileSource.Stub(f => f.GetWmtsTileSources(Arg<string>.Is.NotNull))
                                            .Return(Enumerable.Empty<ITileSource>());

            var factoryThrowingCannotFindTileSourceException = MockRepository.GenerateStub<ITileSourceFactory>();
            factoryThrowingCannotFindTileSourceException.Stub(f => f.GetWmtsTileSources(Arg<string>.Is.NotNull))
                                                        .Throw(new CannotFindTileSourceException());

            yield return new TestCaseData(factoryWithoutRequiredTileSource)
                .SetName($"{prefix}: Required tile source not returned by factory.");

            yield return new TestCaseData(factoryThrowingCannotFindTileSourceException)
                .SetName($"{prefix}: Tile source factory throws CannotFindTileSourceException.");
        }

        private static void DoWhileTileCacheRootLocked(Action test)
        {
            string expectedTileCachePath = Path.Combine(BruTileSettings.PersistentCacheDirectoryRoot, "wmts");

            string backupTileCachePath = expectedTileCachePath + "_bak";
            if (Directory.Exists(backupTileCachePath))
            {
                Directory.Delete(backupTileCachePath, true);
            }

            if (Directory.Exists(expectedTileCachePath))
            {
                Directory.Move(expectedTileCachePath, backupTileCachePath);
            }

            try
            {
                using (new DirectoryPermissionsRevoker(BruTileSettings.PersistentCacheDirectoryRoot, FileSystemRights.Write))
                {
                    test();
                }
            }
            finally
            {
                if (Directory.Exists(backupTileCachePath))
                {
                    Directory.Move(backupTileCachePath, expectedTileCachePath);
                }
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

        private static void ExtendWithExpectedMargin(Extent expectedExtent)
        {
            double smallestDimension = Math.Min(expectedExtent.Height, expectedExtent.Width);
            expectedExtent.ExpandBy(smallestDimension * padding);
        }

        private Extent GetExpectedExtent(FeatureBasedMapData visibleMapData)
        {
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach (MapFeature feature in visibleMapData.Features)
            {
                foreach (MapGeometry geometry in feature.MapGeometries)
                {
                    foreach (IEnumerable<Point2D> pointCollection in geometry.PointCollections)
                    {
                        foreach (Point2D point in pointCollection)
                        {
                            minX = Math.Min(minX, point.X);
                            maxX = Math.Max(maxX, point.X);

                            minY = Math.Min(minY, point.Y);
                            maxY = Math.Max(maxY, point.Y);
                        }
                    }
                }
            }

            return new Extent(minX, minY, maxX, maxY);
        }
    }
}