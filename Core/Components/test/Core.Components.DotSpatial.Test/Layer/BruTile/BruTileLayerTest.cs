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
using System.Drawing;
using BruTile;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.DotSpatial.Layer.BruTile.Projections;
using Core.Components.DotSpatial.Layer.BruTile.TileFetching;
using DotSpatial.Controls;
using DotSpatial.Projections;
using DotSpatial.Projections.AuthorityCodes;
using DotSpatial.Symbology;
using NUnit.Framework;
using Rhino.Mocks;
using DotSpatialLayer = DotSpatial.Symbology.Layer;
using DotSpatialExtent = DotSpatial.Data.Extent;

namespace Core.Components.DotSpatial.Test.Layer.BruTile
{
    [TestFixture]
    public class BruTileLayerTest
    {
        [Test]
        public void Constructor_ConfigurationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new BruTileLayer(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("configuration", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                const string authorityCode = "EPSG:28992";
                var extent = new Extent(10000, 123456, 987654321, 321654);
                const string legendText = "A";
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, authorityCode, extent, legendText);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    Assert.IsInstanceOf<DotSpatialLayer>(layer);
                    Assert.IsInstanceOf<IMapLayer>(layer);

                    Assert.AreEqual(AuthorityCodeHandler.Instance[authorityCode], layer.Projection);
                    Assert.AreEqual(new DotSpatialExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY), layer.Extent);

                    Assert.AreEqual(legendText, layer.LegendText);
                    Assert.IsTrue(layer.LegendItemVisible);
                    Assert.AreEqual(SymbolMode.Symbol, layer.LegendSymbolMode);
                    Assert.AreEqual(LegendType.Custom, layer.LegendType);

                    Assert.IsTrue(layer.IsVisible);
                    Assert.IsTrue(layer.Checked);

                    Assert.AreEqual(0, layer.Transparency);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ConfigurationNotInitialized_InitializeConfigurationBeforeUsingIt()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateUninitializedStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                // Call
                using (new BruTileLayer(configuration))
                {
                    // Only constructor call is relevant for unit test
                }
            }
            // Assert
            mocks.VerifyAll(); // Asserts method call in proper order
        }

        [Test]
        public void Constructor_SrsOnlyNumber_ProjectionSet()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                const string authorityNumber = "28991";
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, authorityNumber);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    Assert.AreEqual(AuthorityCodeHandler.Instance[$"EPSG:{authorityNumber}"], layer.Projection);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsEsriProjectionString_ProjectionSet()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                string esriProjectionString = AuthorityCodeHandler.Instance["EPSG:2000"].ToEsriString();
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, esriProjectionString);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    ProjectionInfo expectedProjection = ProjectionInfo.FromEsriString(esriProjectionString);
                    Assert.IsTrue(expectedProjection.Equals(layer.Projection));
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsProj4String_ProjectionSet()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                string proj4String = AuthorityCodeHandler.Instance["EPSG:2222"].ToProj4String();
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, proj4String);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    ProjectionInfo expectedProjection = ProjectionInfo.FromProj4String(proj4String);
                    Assert.IsTrue(expectedProjection.Equals(layer.Projection));
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_UnknownProjectionSpecification_ProjectionSetToCorrectedWgs84()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                const string authorityCode = "im not a projection spec";
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, authorityCode);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    ProjectionInfo wgs84 = GetCorrectedWgs84Projection();
                    Assert.IsTrue(wgs84.Equals(layer.Projection));
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsUrnFormattedString_ProjectionSet()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                string urnCode = "urn:ogc:def:crs:EPSG:6.18.3:3857";
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, urnCode);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    ProjectionInfo expectedProjection = AuthorityCodeHandler.Instance["EPSG:3857"];
                    Assert.IsTrue(expectedProjection.Equals(layer.Projection));
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsProj4StringWithCorruption_ProjectionSetToCorrectedWgs84()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                string proj4String = "+proj=sterea +lat_0=52.15616055555555 +lon_0=corruption +k=0.9999079 +x_0=155000 +y_0=463000 +ellps=bessel +towgs84=565.417,50.3319,465.552,-0.398957,0.343988,-1.8774,4.0725 +units=m +no_defs ";
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, proj4String);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    ProjectionInfo wgs84 = GetCorrectedWgs84Projection();
                    Assert.IsTrue(wgs84.Equals(layer.Projection));
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Constructor_SrsAsNullOrWhitespaceString_ProjectionSetToCorrectedWgs84(string invalidSrs)
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher, invalidSrs);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    ProjectionInfo wgs84 = GetCorrectedWgs84Projection();
                    Assert.IsTrue(wgs84.Equals(layer.Projection));
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(-123.456f)]
        [TestCase(-1e-6f)]
        [TestCase(1.0f + 1e-6f)]
        [TestCase(456.123f)]
        public void Transparency_InvalidValue_ThrowArgumentOutOfRangeException(float invalidValue)
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                using (var layer = new BruTileLayer(configuration))
                {
                    // Call
                    TestDelegate call = () => layer.Transparency = invalidValue;

                    // Assert
                    string message = "Transparantie moet in het bereik [0.0, 1.0] liggen.";
                    string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
                    Assert.AreEqual("value", paramName);
                }
            }
        }

        [Test]
        public void Transparency_ValidValue_InvalidateMapFrame()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);

                var mapFrameExtents = new DotSpatialExtent();

                var mapFrame = mocks.StrictMock<IFrame>();
                mapFrame.ViewExtents = mapFrameExtents;
                mapFrame.Expect(f => f.Invalidate(mapFrameExtents));
                mocks.ReplayAll();

                using (var layer = new BruTileLayer(configuration))
                {
                    object itemChangedSender = null;
                    EventArgs itemChangedEventArgs = null;
                    int itemChangedCount = 0;
                    layer.ItemChanged += (sender, args) =>
                    {
                        itemChangedSender = sender;
                        itemChangedEventArgs = args;
                        itemChangedCount++;
                    };

                    // Call
                    layer.Transparency = 0.5f;

                    // Assert
                    Assert.AreEqual(1, itemChangedCount);
                    Assert.AreSame(layer, itemChangedSender);
                    Assert.AreEqual(EventArgs.Empty, itemChangedEventArgs);
                }
            }
        }

        [Test]
        public void Clone_InitializedBruTileLayer_DeepCloneLayer()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                var schema = mocks.Stub<ITileSchema>();
                const string authorityCode = "EPSG:28992";
                schema.Stub(s => s.Srs).Return(authorityCode);
                var extent = new Extent(10000, 123456, 987654321, 321654);
                schema.Stub(s => s.Extent).Return(extent);

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                string legendText = "<Legend Text>";
                var clonedConfiguration = mocks.Stub<IConfiguration>();
                clonedConfiguration.Stub(c => c.Initialized).Return(true);
                clonedConfiguration.Stub(c => c.TileSource).Return(tileSource);
                clonedConfiguration.Stub(c => c.LegendText).Return(legendText);
                clonedConfiguration.Stub(c => c.TileFetcher).Return(tileFetcher);
                clonedConfiguration.Stub(c => c.Dispose());

                var configuration = mocks.Stub<IConfiguration>();
                configuration.Expect(c => c.Clone()).Return(clonedConfiguration);
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return(legendText);
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
                configuration.Stub(c => c.Dispose());

                var mapFrame = mocks.Stub<IFrame>();
                mocks.ReplayAll();

                var transparency = 0.3f;
                using (var layer = new BruTileLayer(configuration)
                {
                    Transparency = transparency,
                    IsVisible = false,
                    Checked = false,
                    LegendText = "A",
                    LegendItemVisible = false,
                    MapFrame = mapFrame
                })
                {
                    layer.Reproject(KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984ComplexUTMZone20N);

                    // Call
                    using (var clonedLayer = (BruTileLayer) layer.Clone())
                    {
                        // Assert
                        Assert.AreEqual(AuthorityCodeHandler.Instance[authorityCode], clonedLayer.Projection,
                                        "Even if 'layer' has been reprojected, the cloned layer should be in the original coordinate system.");
                        Assert.AreEqual(new DotSpatialExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY), clonedLayer.Extent);

                        Assert.AreEqual(legendText, clonedLayer.LegendText,
                                        "'clonedLayer' should have it's name initialized based on the configuration.");
                        Assert.IsTrue(clonedLayer.LegendItemVisible,
                                      "'clonedLayer' visibility of the legend item should be the default for a newly constructed BruTileLayer.");
                        Assert.AreEqual(SymbolMode.Symbol, clonedLayer.LegendSymbolMode,
                                        "'clonedLayer' value of the legend symbol mode should be the default for a newly constructed BruTileLayer.");
                        Assert.AreEqual(LegendType.Custom, clonedLayer.LegendType,
                                        "'clonedLayer' value of the legend type should be the default for a newly constructed BruTileLayer.");

                        Assert.IsTrue(clonedLayer.IsVisible,
                                      "'clonedLayer' visibility should be the default for a newly constructed BruTileLayer.");
                        Assert.IsTrue(clonedLayer.Checked,
                                      "'clonedLayer' checked state should be the default for a newly constructed BruTileLayer.");

                        Assert.AreEqual(transparency, clonedLayer.Transparency);
                    }
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Reproject_TargetProjectionNull_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    ProjectionInfo originalProjection = layer.Projection;
                    DotSpatialExtent originalExtent = layer.Extent;

                    // Call
                    layer.Reproject(null);

                    // Assert
                    Assert.AreSame(originalProjection, layer.Projection);
                    Assert.AreSame(originalExtent, layer.Extent);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Reproject_TargetProjectionDifferentFromOriginal_ChangeProjection()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    var originalProjection = layer.Projection;
                    var originalExtent = layer.Extent;

                    ProjectionInfo targetProjection = KnownCoordinateSystems.Projected.NationalGrids.AmericanSamoa1962SamoaLambert;

                    // Call
                    layer.Reproject(targetProjection);

                    // Assert
                    Assert.AreSame(targetProjection, layer.Projection);
                    var expectedExtent = originalExtent.Reproject(originalProjection, targetProjection);
                    Assert.AreEqual(expectedExtent, layer.Extent);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Reproject_TargetProjectionSameAsOriginal_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    ProjectionInfo originalProjection = layer.Projection;
                    DotSpatialExtent originalExtent = layer.Extent;

                    // Call
                    layer.Reproject(originalProjection);

                    // Assert
                    Assert.AreSame(originalProjection, layer.Projection);
                    Assert.AreSame(originalExtent, layer.Extent);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_WhenDisposeLocked_DoNothing()
        {
            // Given
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                var schema = mocks.Stub<ITileSchema>();
                schema.Stub(s => s.Srs).Return("EPSG:28992");
                schema.Stub(s => s.Extent).Return(new Extent());

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                bool disposedLocked = false;
                var configuration = mocks.Stub<IConfiguration>();
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return("<Legend Text>");
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
                configuration.Expect(c => c.Dispose())
                             .WhenCalled(invocation =>
                             {
                                 if (disposedLocked)
                                 {
                                     Assert.Fail("configuration shouldn't be disposed if layer.Dispose is locked.");
                                 }
                             });
                mocks.ReplayAll();

                using (var layer = new BruTileLayer(configuration))
                {
                    // When
                    layer.LockDispose();
                    disposedLocked = true;
                    layer.Dispose();
                    layer.UnlockDispose();
                    disposedLocked = false;
                }
            }
            // Then
            mocks.VerifyAll(); // Asserts method call in proper order
        }

        [Test]
        public void DrawRegions_LayerInvisible_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<ITileProvider>();
            using (var tileFetcher = new AsyncTileFetcher(provider, 50, 200))
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                using (var layer = new BruTileLayer(configuration)
                {
                    IsVisible = false
                })
                {
                    var mapArgs = new MapArgs(new Rectangle(), null, null);
                    var regions = new List<DotSpatialExtent>
                    {
                        layer.Extent
                    };

                    // Call
                    TestDelegate call = () => layer.DrawRegions(mapArgs, regions);

                    // Assert
                    Assert.DoesNotThrow(call,
                                        "No exception should be thrown for null Graphics objects, as nothing should be drawn.");
                }
            }
            mocks.VerifyAll();
        }

        private static IConfiguration CreateStubConfiguration(MockRepository mocks, AsyncTileFetcher tileFetcher,
                                                              string tileSchemaSrsString = "EPSG:28992",
                                                              Extent tileSourceExtent = new Extent(),
                                                              string legendText = "<Legend Text>")
        {
            var schema = mocks.Stub<ITileSchema>();
            schema.Stub(s => s.Srs).Return(tileSchemaSrsString);
            schema.Stub(s => s.Extent).Return(tileSourceExtent);

            var tileSource = mocks.Stub<ITileSource>();
            tileSource.Stub(ts => ts.Schema).Return(schema);

            var configuration = mocks.Stub<IConfiguration>();
            configuration.Stub(c => c.Initialized).Return(true);
            configuration.Stub(c => c.TileSource).Return(tileSource);
            configuration.Stub(c => c.LegendText).Return(legendText);
            configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
            configuration.Stub(c => c.Dispose());
            return configuration;
        }

        private static IConfiguration CreateUninitializedStubConfiguration(MockRepository mocks, AsyncTileFetcher tileFetcher)
        {
            var schema = mocks.Stub<ITileSchema>();
            schema.Stub(s => s.Srs).Return("EPSG:28992");
            schema.Stub(s => s.Extent).Return(new Extent());

            var tileSource = mocks.Stub<ITileSource>();
            tileSource.Stub(ts => ts.Schema).Return(schema);

            var configuration = mocks.Stub<IConfiguration>();
            using (mocks.Ordered())
            {
                configuration.Stub(c => c.Initialized).Return(false);
                configuration.Expect(c => c.Initialize());
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return("<Legend Text>");
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
                configuration.Stub(c => c.Dispose());
            }
            return configuration;
        }

        private static ProjectionInfo GetCorrectedWgs84Projection()
        {
            ProjectionInfo wgs84 = AuthorityCodeHandler.Instance["EPSG:3857"];
            wgs84.GeographicInfo.Datum = KnownCoordinateSystems.Geographic.World.WGS1984.GeographicInfo.Datum;
            return wgs84;
        }
    }
}