// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BruTile;
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.IO;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.DotSpatial.Projections;
using Core.Components.DotSpatial.Test.Properties;
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
        private static IEnumerable<TestCaseData> DrawRegionsTestCases
        {
            get
            {
                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                                                     7, 7, Resources.blue_256x256),
                                                  new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                                                     6, 7, Resources.green_256x256)
                                              }, 4),
                                              Resources.BackgroundLayerCanvas,
                                              Resources.BackgroundLayerCanvasAfterAddingTestTiles,
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              false,
                                              0f)
                    .SetName("DrawRegions for 2 consecutive tiles at level 4.");

                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                                                     7, 7, Resources.blue_256x256),
                                                  new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                                                     6, 7, Resources.green_256x256)
                                              }, 4),
                                              Resources.BackgroundLayerCanvas,
                                              Resources.BackgroundLayerCanvasAfterAddingTestTiles,
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              null,
                                              false,
                                              0f)
                    .SetName("DrawRegions for 2 consecutive tiles at level 4 without specifying region.");

                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(113712.32, 509448.72, 115432.64, 511169.04),
                                                                     232, 228, Resources.blue_256x256),
                                                  new TileInfoConfig(new Extent(111992, 511169.04, 113712.32, 512889.36),
                                                                     231, 227, Resources.green_256x256)
                                              }, 9),
                                              Resources.BackgroundLayerCanvas_smaller,
                                              Resources.BackgroundLayerCanvas_smallerAfterAddingTestTiles,
                                              new DotSpatialExtent(4.71640686348909, 52.5275200480914, 4.84542703038542, 52.622163604187),
                                              new DotSpatialExtent(4.71640686348909, 52.5275200480914, 4.84542703038542, 52.622163604187),
                                              true,
                                              0f)
                    .SetName("DrawRegions for 2 loose tiles at level 9 in WGS84.");

                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(113712.32, 509448.72, 115432.64, 511169.04),
                                                                     232, 228, Resources.blue_256x256),
                                                  new TileInfoConfig(new Extent(111992, 511169.04, 113712.32, 512889.36),
                                                                     231, 227, Resources.green_256x256)
                                              }, 9),
                                              Resources.BackgroundLayerCanvas_smaller,
                                              Resources.BackgroundLayerCanvas_smallerAfterAddingTestTiles,
                                              new DotSpatialExtent(4.71640686348909, 52.5275200480914, 4.84542703038542, 52.622163604187),
                                              null,
                                              true,
                                              0f)
                    .SetName("DrawRegions for 2 loose tiles at level 9 in WGS84 without specifying region.");

                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(113712.32, 509448.72, 115432.64, 511169.04),
                                                                     232, 228, Resources.blue_256x256),
                                                  new TileInfoConfig(new Extent(111992, 511169.04, 113712.32, 512889.36),
                                                                     231, 227, Resources.green_256x256)
                                              }, 9),
                                              Resources.BackgroundLayerCanvas_smaller,
                                              Resources.BackgroundLayerCanvas_smaller,
                                              new DotSpatialExtent(4.71640686348909, 52.5275200480914, 4.84542703038542, 52.622163604187),
                                              new DotSpatialExtent(4.58738669659276, 52.5275200480914, 4.45836652969643, 52.622163604187),
                                              false,
                                              0f)
                    .SetName("DrawRegions at level 9 for region outside viewport.");

                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                                                     7, 7, Resources.blue_256x256),
                                                  new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                                                     6, 7, Resources.green_256x256)
                                              }, 4),
                                              Resources.BackgroundLayerCanvas,
                                              Resources.BackgroundLayerCanvas,
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              false,
                                              1f)
                    .SetName("DrawRegions for 2 consecutive tiles at level 4 for fully transparent layer.");

                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                                                     7, 7, Resources.blue_256x256),
                                                  new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                                                     6, 7, Resources.green_256x256)
                                              }, 4),
                                              Resources.BackgroundLayerCanvas,
                                              Resources.BackgroundLayerAfterAddingHalfTransparentTestTiles,
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              false,
                                              0.5f)
                    .SetName("DrawRegions for 2 consecutive tiles at level 4 for 50% transparent layer.");

                yield return new TestCaseData(new TileInfosTestConfig(new[]
                                              {
                                                  new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                                                     7, 7, null),
                                                  new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                                                     6, 7, null)
                                              }, 4),
                                              Resources.BackgroundLayerCanvas,
                                              Resources.BackgroundLayerCanvas,
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                              false,
                                              0f)
                    .SetName("DrawRegions for 2 corrupted image tiles at level 4.");
            }
        }

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
            const string authorityCode = "EPSG:28992";
            var extent = new Extent(10000, 123456, 987654321, 321654);

            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks, authorityCode, extent);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                Assert.IsInstanceOf<DotSpatialLayer>(layer);
                Assert.IsInstanceOf<IMapLayer>(layer);

                Assert.AreEqual(AuthorityCodeHandler.Instance[authorityCode], layer.Projection);
                Assert.AreEqual(new DotSpatialExtent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY), layer.Extent);

                Assert.IsTrue(layer.IsVisible);
                Assert.IsTrue(layer.Checked);

                Assert.AreEqual(0, layer.Transparency);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ConfigurationNotInitialized_InitializeConfigurationBeforeUsingIt()
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateUninitializedStubConfiguration(mocks);
            mocks.ReplayAll();

            // Call
            using (new BruTileLayer(configuration))
            {
                // Only constructor call is relevant for unit test
            }

            // Assert
            mocks.VerifyAll(); // Asserts method call in proper order
        }

        [Test]
        public void Constructor_SrsOnlyNumber_ProjectionSet()
        {
            // Setup
            const string authorityNumber = "28991";

            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks, authorityNumber);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                Assert.AreEqual(AuthorityCodeHandler.Instance[$"EPSG:{authorityNumber}"], layer.Projection);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsEsriProjectionString_ProjectionSet()
        {
            // Setup
            string esriProjectionString = AuthorityCodeHandler.Instance["EPSG:2000"].ToEsriString();

            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks, esriProjectionString);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                ProjectionInfo expectedProjection = ProjectionInfo.FromEsriString(esriProjectionString);
                Assert.IsTrue(expectedProjection.Equals(layer.Projection));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsProj4String_ProjectionSet()
        {
            // Setup
            string proj4String = AuthorityCodeHandler.Instance["EPSG:2222"].ToProj4String();

            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks, proj4String);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                ProjectionInfo expectedProjection = ProjectionInfo.FromProj4String(proj4String);
                Assert.IsTrue(expectedProjection.Equals(layer.Projection));
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase("im not a projection spec")]
        [TestCase("A:B:C")]
        public void Constructor_UnknownProjectionSpecification_ProjectionSetToCorrectedWgs84(string authorityCode)
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks, authorityCode);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                ProjectionInfo wgs84 = GetCorrectedWgs84Projection();
                Assert.IsTrue(wgs84.Equals(layer.Projection));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsUrnFormattedString_ProjectionSet()
        {
            // Setup
            const string urnCode = "urn:ogc:def:crs:EPSG:6.18.3:3857";

            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks, urnCode);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                ProjectionInfo expectedProjection = AuthorityCodeHandler.Instance["EPSG:3857"];
                Assert.IsTrue(expectedProjection.Equals(layer.Projection));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SrsAsProj4StringWithCorruption_ProjectionSetToCorrectedWgs84()
        {
            // Setup
            const string proj4String = "+proj=sterea +lat_0=52.15616055555555 +lon_0=corruption +k=0.9999079 +x_0=155000 +y_0=463000 +ellps=bessel +towgs84=565.417,50.3319,465.552,-0.398957,0.343988,-1.8774,4.0725 +units=m +no_defs ";

            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks, proj4String);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                ProjectionInfo wgs84 = GetCorrectedWgs84Projection();
                Assert.IsTrue(wgs84.Equals(layer.Projection));
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
            IConfiguration configuration = CreateStubConfiguration(mocks, invalidSrs);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                // Assert
                ProjectionInfo wgs84 = GetCorrectedWgs84Projection();
                Assert.IsTrue(wgs84.Equals(layer.Projection));
            }

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-123.456f)]
        [TestCase(-1e-6f)]
        [TestCase(1.0f + 1e-6f)]
        [TestCase(456.123f)]
        public void Transparency_InvalidValue_ThrowArgumentOutOfRangeException(float invalidValue)
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            {
                // Call
                TestDelegate call = () => layer.Transparency = invalidValue;

                // Assert
                const string message = "Transparantie moet in het bereik [0,0, 1,0] liggen.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
                Assert.AreEqual("value", paramName);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Transparency_ValidValue_InvalidateMapFrame()
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks);

            var mapFrameExtents = new DotSpatialExtent();

            var mapFrame = mocks.StrictMock<IFrame>();
            mapFrame.Stub(f => f.ViewExtents).Return(mapFrameExtents);
            mapFrame.Expect(f => f.Invalidate(mapFrameExtents));
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration)
            {
                MapFrame = mapFrame
            })
            {
                object itemChangedSender = null;
                EventArgs itemChangedEventArgs = null;
                var itemChangedCount = 0;
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

            mocks.VerifyAll();
        }

        [Test]
        public void Clone_InitializedBruTileLayer_DeepCloneLayer()
        {
            // Setup
            const string authorityCode = "EPSG:28992";
            var extent = new Extent(10000, 123456, 987654321, 321654);

            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();

            var schema = mocks.Stub<ITileSchema>();
            schema.Stub(s => s.Srs).Return(authorityCode);
            schema.Stub(s => s.Extent).Return(extent);

            var clonedConfiguration = mocks.Stub<IConfiguration>();
            clonedConfiguration.Stub(c => c.Initialized).Return(true);
            clonedConfiguration.Stub(c => c.TileSchema).Return(schema);
            clonedConfiguration.Stub(c => c.TileFetcher).Return(tileFetcher);
            clonedConfiguration.Stub(c => c.Dispose());

            var configuration = mocks.Stub<IConfiguration>();
            configuration.Expect(c => c.Clone()).Return(clonedConfiguration);
            configuration.Stub(c => c.Initialized).Return(true);
            configuration.Stub(c => c.TileSchema).Return(schema);
            configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
            configuration.Stub(c => c.Dispose());

            var mapFrame = mocks.Stub<IFrame>();
            mocks.ReplayAll();

            const float transparency = 0.3f;
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

                    Assert.IsTrue(clonedLayer.IsVisible,
                                  "'clonedLayer' visibility should be the default for a newly constructed BruTileLayer.");
                    Assert.IsTrue(clonedLayer.Checked,
                                  "'clonedLayer' checked state should be the default for a newly constructed BruTileLayer.");

                    Assert.AreEqual(transparency, clonedLayer.Transparency);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Reproject_TargetProjectionNull_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks);
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

            mocks.VerifyAll();
        }

        [Test]
        public void Reproject_TargetProjectionDifferentFromOriginal_ChangeProjection()
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks);
            mocks.ReplayAll();

            // Call
            using (var layer = new BruTileLayer(configuration))
            {
                ProjectionInfo originalProjection = layer.Projection;
                DotSpatialExtent originalExtent = layer.Extent;

                ProjectionInfo targetProjection = KnownCoordinateSystems.Projected.NationalGrids.AmericanSamoa1962SamoaLambert;

                // Call
                layer.Reproject(targetProjection);

                // Assert
                Assert.AreSame(targetProjection, layer.Projection);
                DotSpatialExtent expectedExtent = originalExtent.Reproject(originalProjection, targetProjection);
                Assert.AreEqual(expectedExtent, layer.Extent);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Reproject_TargetProjectionSameAsOriginal_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks);
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

            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_WhenDisposeLocked_DoNothing()
        {
            // Given
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            var schema = mocks.Stub<ITileSchema>();
            schema.Stub(s => s.Srs).Return("EPSG:28992");
            schema.Stub(s => s.Extent).Return(new Extent());

            var disposedLocked = false;
            var configuration = mocks.Stub<IConfiguration>();
            configuration.Stub(c => c.Initialized).Return(true);
            configuration.Stub(c => c.TileSchema).Return(schema);
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

            // Then
            mocks.VerifyAll(); // Asserts method call in proper order
        }

        [Test]
        public void DrawRegions_LayerInvisible_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateStubConfiguration(mocks);
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

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(DrawRegionsTestCases))]
        public void DrawRegions_VariousTestCases_DrawMapTiles(TileInfosTestConfig testConfig,
                                                              Bitmap mapCanvasSource,
                                                              Bitmap expectedResult,
                                                              DotSpatialExtent mapExtent,
                                                              DotSpatialExtent regionExtent,
                                                              bool reprojectLayerToWgs84,
                                                              float transparency)
        {
            // Setup
            var mocks = new MockRepository();
            IConfiguration configuration = CreateConfigurationForDrawingImmediately(mocks, testConfig);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var mapCanvas = (Bitmap) mapCanvasSource.Clone())
            using (Graphics graphics = Graphics.FromImage(mapCanvas))
            {
                if (reprojectLayerToWgs84)
                {
                    layer.Reproject(KnownCoordinateSystems.Geographic.World.WGS1984);
                }

                layer.Transparency = transparency;

                var mapArgs = new MapArgs(new Rectangle(0, 0, mapCanvas.Width, mapCanvas.Height),
                                          mapExtent,
                                          graphics);

                var regions = new List<DotSpatialExtent>();
                if (regionExtent != null)
                {
                    regions.Add(regionExtent);
                }

                // Call
                layer.DrawRegions(mapArgs, regions);

                // Assert
                TestHelper.AssertImagesAreEqual(expectedResult, mapCanvas);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void DrawRegions_FirstTileRetrieveNotInCacheSecondTileInCache_DrawMapTiles()
        {
            // Setup
            var testConfig = new TileInfosTestConfig(new[]
            {
                new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                   7, 7, Resources.blue_256x256),
                new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                   6, 7, Resources.green_256x256)
            }, 4);

            var mocks = new MockRepository();
            IConfiguration configuration = CreateConfigurationForDrawingWhereFirstTileFetchNotInCacheButSecondFetchIs(mocks, testConfig);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var mapCanvas = (Bitmap) Resources.BackgroundLayerCanvas.Clone())
            using (Graphics graphics = Graphics.FromImage(mapCanvas))
            {
                var mapArgs = new MapArgs(new Rectangle(0, 0, mapCanvas.Width, mapCanvas.Height),
                                          new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                          graphics);

                var regions = new List<DotSpatialExtent>();

                // Call
                layer.DrawRegions(mapArgs, regions);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.BackgroundLayerCanvasAfterAddingTestTiles, mapCanvas);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DrawRegions_TileFetcherReturningNullOrEmpty_NoMapTilesDrawn(bool trueEmptyFalseNull)
        {
            // Setup
            var testConfig = new TileInfosTestConfig(new[]
            {
                new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                   7, 7, Resources.blue_256x256),
                new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                   6, 7, Resources.green_256x256)
            }, 4);

            var mocks = new MockRepository();
            byte[] tileData = trueEmptyFalseNull ? new byte[0] : null;
            IConfiguration configuration = CreateConfigurationForDrawing(mocks, testConfig,
                                                                         (tileFetcher, bitmaps) =>
                                                                         {
                                                                             foreach (KeyValuePair<TileInfo, Bitmap> tileInfoImagePair in bitmaps)
                                                                             {
                                                                                 tileFetcher.Stub(c => c.GetTile(tileInfoImagePair.Key))
                                                                                            .Return(tileData);
                                                                             }
                                                                         });
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var mapCanvas = (Bitmap) Resources.BackgroundLayerCanvas.Clone())
            using (Graphics graphics = Graphics.FromImage(mapCanvas))
            {
                var mapArgs = new MapArgs(new Rectangle(0, 0, mapCanvas.Width, mapCanvas.Height),
                                          new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503),
                                          graphics);

                var regions = new List<DotSpatialExtent>();

                // Call
                layer.DrawRegions(mapArgs, regions);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.BackgroundLayerCanvas, mapCanvas);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1.0, 1.0)]
        [TestCase(-1.0, -1.0)]
        [TestCase(-1.0, 0.0)]
        [TestCase(1.0, 0.0)]
        [TestCase(0.0, 1.0)]
        [TestCase(0.0, -1.0)]
        [TestCase(0.0, 0.0)]
        [TestCase(1.0 + 1e-6, 0.0)]
        [TestCase(-1.0 - 1e-6, 0.0)]
        [TestCase(0.0, 1.0 + 1e-6)]
        [TestCase(0.0, -1.0 - 1e-6)]
        public void LayerWithMapFrameAndDrawnAtLevel_TileReceivedEventAtCurrentLevel_InvalidateMapFrameIfRegionIntersects(double dxFactor, double dyFactor)
        {
            // Given
            const int zoomLevel = 4;
            var testConfig = new TileInfosTestConfig(new[]
            {
                new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                   7, 7, Resources.blue_256x256),
                new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                   6, 7, Resources.green_256x256)
            }, zoomLevel);

            var mapExtent = new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503);
            double dx = (mapExtent.MaxX - mapExtent.MinX) * dxFactor;
            double dy = (mapExtent.MaxY - mapExtent.MinY) * dyFactor;
            var tileExtent = new DotSpatialExtent(mapExtent.MinX + dx, mapExtent.MinY + dy,
                                                  mapExtent.MaxX + dx, mapExtent.MaxY + dy);

            var mocks = new MockRepository();
            var mapFrame = mocks.StrictMock<IFrame>();
            mapFrame.Stub(f => f.ViewExtents).Return(mapExtent);
            if (Math.Abs(dxFactor) <= 1.0 && Math.Abs(dyFactor) <= 1.0)
            {
                // Note: Only invalidate area's within the view port
                mapFrame.Expect(mf => mf.Invalidate(tileExtent));
            }

            IConfiguration configuration = CreateConfigurationForDrawingImmediately(mocks, testConfig);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration)
            {
                MapFrame = mapFrame
            })
            using (var mapCanvas = (Bitmap) Resources.BackgroundLayerCanvas.Clone())
            using (Graphics graphics = Graphics.FromImage(mapCanvas))
            {
                var mapArgs = new MapArgs(new Rectangle(0, 0, mapCanvas.Width, mapCanvas.Height),
                                          mapExtent,
                                          graphics);

                var regions = new List<DotSpatialExtent>();

                layer.DrawRegions(mapArgs, regions);

                // When
                configuration.TileFetcher.Raise(tf => tf.TileReceived += null,
                                                configuration.TileFetcher,
                                                new TileReceivedEventArgs(new TileInfo
                                                {
                                                    Extent = new Extent(tileExtent.MinX, tileExtent.MinY, tileExtent.MaxX, tileExtent.MaxY),
                                                    Index = new TileIndex(0, 0, $"EPSG:28992:{zoomLevel}")
                                                }, new byte[0]));
            }

            // Then
            mocks.VerifyAll(); // mapFrame.Invalidate should be called if extent overlaps
        }

        [Test]
        [TestCase(3)]
        [TestCase(5)]
        public void LayerWithMapFrameAndDrawnAtLevel_TileReceivedEventAtWrongLevel_DontInvalidate(int otherZoomLevel)
        {
            // Given
            const int currentZoomLevel = 4;
            var testConfig = new TileInfosTestConfig(new[]
            {
                new TileInfoConfig(new Extent(99949.76, 463000.08, 155000, 518050.32),
                                   7, 7, Resources.blue_256x256),
                new TileInfoConfig(new Extent(44899.52, 463000.08, 99949.76, 518050.32),
                                   6, 7, Resources.green_256x256)
            }, currentZoomLevel);

            var mapExtent = new DotSpatialExtent(-78529.9210634486, 403315.730436505, 306453.46038588, 581961.051306503);

            var mocks = new MockRepository();
            var mapFrame = mocks.StrictMock<IFrame>();
            mapFrame.Stub(f => f.ViewExtents).Return(mapExtent);
            mapFrame.Expect(mf => mf.Invalidate(Arg<DotSpatialExtent>.Is.Anything)).Repeat.Never();

            IConfiguration configuration = CreateConfigurationForDrawingImmediately(mocks, testConfig);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration)
            {
                MapFrame = mapFrame
            })
            using (var mapCanvas = (Bitmap) Resources.BackgroundLayerCanvas.Clone())
            using (Graphics graphics = Graphics.FromImage(mapCanvas))
            {
                var mapArgs = new MapArgs(new Rectangle(0, 0, mapCanvas.Width, mapCanvas.Height),
                                          mapExtent,
                                          graphics);

                var regions = new List<DotSpatialExtent>();

                layer.DrawRegions(mapArgs, regions);

                // When
                configuration.TileFetcher.Raise(tf => tf.TileReceived += null,
                                                configuration.TileFetcher,
                                                new TileReceivedEventArgs(new TileInfo
                                                {
                                                    Extent = new Extent(mapExtent.MinX, mapExtent.MinY, mapExtent.MaxX, mapExtent.MaxY),
                                                    Index = new TileIndex(0, 0, $"EPSG:28992:{otherZoomLevel}")
                                                }, new byte[0]));
            }

            // Then
            mocks.VerifyAll(); // mapFrame.Invalidate shouldn't be called!
        }

        [Test]
        public void LayerWithMapFrame_QueueEmptyEvent_InvalidateWholeMapFrame()
        {
            // Given
            var mocks = new MockRepository();
            var mapFrame = mocks.StrictMock<IFrame>();
            mapFrame.Expect(mf => mf.Invalidate());

            IConfiguration configuration = CreateStubConfiguration(mocks);
            mocks.ReplayAll();

            using (new BruTileLayer(configuration)
            {
                MapFrame = mapFrame
            })
            {
                // When
                configuration.TileFetcher.Raise(tf => tf.QueueEmpty += null,
                                                configuration.TileFetcher,
                                                EventArgs.Empty);
            }

            // Then
            mocks.VerifyAll(); // mapFrame.Invalidate should be called
        }

        private static Resolution CreateResolutionForLevel(int level, string epsgCode)
        {
            int numberOfImagesAtLevel = GetNumberOfImages(level);
            return new Resolution($"{epsgCode}:{level}", GetUnitsPerPixel(level),
                                  256, 256,
                                  -285401.92, 903402,
                                  numberOfImagesAtLevel, numberOfImagesAtLevel,
                                  GetScaleFactor(level));
        }

        private static int GetNumberOfImages(int level)
        {
            return Convert.ToInt32(Math.Pow(2, level));
        }

        private static double GetUnitsPerPixel(int level)
        {
            return 3440.64 / GetNumberOfImages(level);
        }

        private static int GetScaleFactor(int level)
        {
            return 12288000 / GetNumberOfImages(level);
        }

        private static IDictionary<string, Resolution> CreateResolutionDictionary(string epsgCode, int level)
        {
            int[] levelToCreateResolutionsFor =
            {
                level - 1,
                level,
                level + 1
            };

            return levelToCreateResolutionsFor.ToDictionary(resolutionLevel => $"{epsgCode}:{resolutionLevel}",
                                                            resolutionLevel => CreateResolutionForLevel(resolutionLevel, epsgCode));
        }

        public class TileInfosTestConfig
        {
            public TileInfosTestConfig(IEnumerable<TileInfoConfig> configs, int level)
            {
                TileInfoConfigurations = new List<TileInfoConfig>(configs);
                Level = level;
            }

            public IEnumerable<TileInfoConfig> TileInfoConfigurations { get; }
            public int Level { get; }
        }

        public class TileInfoConfig
        {
            public TileInfoConfig(Extent extent, int columnIndex, int rowIndex, Bitmap image)
            {
                Extent = extent;
                ColumnIndex = columnIndex;
                RowIndex = rowIndex;
                Image = image;
            }

            public Extent Extent { get; }
            public int ColumnIndex { get; }
            public int RowIndex { get; }
            public Bitmap Image { get; }
        }

        private static IConfiguration CreateConfigurationForDrawingImmediately(MockRepository mocks, TileInfosTestConfig config)
        {
            return CreateConfigurationForDrawing(mocks, config, TileFetcherGetsImageImmediately);
        }

        private static IConfiguration CreateConfigurationForDrawing(MockRepository mocks, TileInfosTestConfig config, Action<ITileFetcher, IDictionary<TileInfo, Bitmap>> configureTileFetcherGetTileStub)
        {
            const string epsgCode = "EPSG:28992";
            int level = config.Level;
            string levelId = $"{epsgCode}:{level}";

            Dictionary<TileInfo, Bitmap> tileInfoImageLookup = config.TileInfoConfigurations.ToDictionary(
                c => new TileInfo
                {
                    Extent = c.Extent,
                    Index = new TileIndex(c.ColumnIndex, c.RowIndex, levelId)
                },
                c => c.Image);

            var tileFetcher = mocks.Stub<ITileFetcher>();
            tileFetcher.Stub(tf => tf.TileReceived += null).IgnoreArguments();
            tileFetcher.Stub(tf => tf.TileReceived -= null).IgnoreArguments();
            tileFetcher.Stub(tf => tf.QueueEmpty += null).IgnoreArguments();
            tileFetcher.Stub(tf => tf.QueueEmpty -= null).IgnoreArguments();
            tileFetcher.Stub(tf => tf.DropAllPendingTileRequests());
            configureTileFetcherGetTileStub(tileFetcher, tileInfoImageLookup);

            var tileSchema = mocks.Stub<ITileSchema>();
            tileSchema.Stub(s => s.Srs).Return(epsgCode);
            tileSchema.Stub(s => s.Extent).Return(new Extent(-285401.92, 22598.16, 595401.92, 903402));
            tileSchema.Stub(s => s.Resolutions).Return(CreateResolutionDictionary(epsgCode, level));
            tileSchema.Stub(s => s.GetTileInfos(Arg<Extent>.Is.NotNull,
                                                Arg<string>.Is.Equal(levelId)))
                      .Return(tileInfoImageLookup.Keys);

            var configuration = mocks.Stub<IConfiguration>();
            configuration.Stub(c => c.Initialized).Return(true);
            configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
            configuration.Stub(c => c.TileSchema).Return(tileSchema);
            configuration.Stub(c => c.Dispose());
            return configuration;
        }

        private static IConfiguration CreateConfigurationForDrawingWhereFirstTileFetchNotInCacheButSecondFetchIs(MockRepository mocks, TileInfosTestConfig config)
        {
            return CreateConfigurationForDrawing(mocks, config, TileFetcherGetsImageOnSecondAttempt);
        }

        private static void TileFetcherGetsImageImmediately(ITileFetcher tileFetcher, IDictionary<TileInfo, Bitmap> tileInfoImageLookup)
        {
            foreach (KeyValuePair<TileInfo, Bitmap> tileInfoImagePair in tileInfoImageLookup)
            {
                tileFetcher.Stub(c => c.GetTile(tileInfoImagePair.Key))
                           .Return(ToByteArray(tileInfoImagePair.Value));
            }
        }

        private static void TileFetcherGetsImageOnSecondAttempt(ITileFetcher tileFetcher, IDictionary<TileInfo, Bitmap> tileInfoImageLookup)
        {
            Dictionary<TileInfo, bool> getTileCalledForTileInfoLookup = tileInfoImageLookup.ToDictionary(kvp => kvp.Key, kvp => false);
            foreach (KeyValuePair<TileInfo, Bitmap> tileInfoImagePair in tileInfoImageLookup)
            {
                // Configure persistent cache in such a way that first retrieve returns null for that tile
                // and then second retrieve does return the data (simulating tile having been
                // retrieved asynchronously):
                tileFetcher.Stub(c => c.GetTile(tileInfoImagePair.Key))
                           .WhenCalled(invocation =>
                           {
                               invocation.ReturnValue = getTileCalledForTileInfoLookup[(TileInfo) invocation.Arguments[0]] ? null : ToByteArray(tileInfoImagePair.Value);
                               getTileCalledForTileInfoLookup[(TileInfo) invocation.Arguments[0]] = true;
                           })
                           .Return(ToByteArray(tileInfoImagePair.Value));
            }
        }

        private static byte[] ToByteArray(Image image)
        {
            // image null: simulate corrupted data
            if (image == null)
            {
                return new[]
                {
                    (byte) 1,
                    (byte) 2
                };
            }

            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        private static IConfiguration CreateStubConfiguration(MockRepository mocks,
                                                              string tileSchemaSrsString = "EPSG:28992",
                                                              Extent tileSourceExtent = new Extent())
        {
            var schema = mocks.Stub<ITileSchema>();
            schema.Stub(s => s.Srs).Return(tileSchemaSrsString);
            schema.Stub(s => s.Extent).Return(tileSourceExtent);

            var tileFetcher = mocks.Stub<ITileFetcher>();

            var configuration = mocks.Stub<IConfiguration>();
            configuration.Stub(c => c.Initialized).Return(true);
            configuration.Stub(c => c.TileSchema).Return(schema);
            configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
            configuration.Stub(c => c.Dispose());
            return configuration;
        }

        private static IConfiguration CreateUninitializedStubConfiguration(MockRepository mocks)
        {
            var schema = mocks.Stub<ITileSchema>();
            schema.Stub(s => s.Srs).Return("EPSG:28992");
            schema.Stub(s => s.Extent).Return(new Extent());

            var tileFetcher = mocks.Stub<ITileFetcher>();

            var configuration = mocks.Stub<IConfiguration>();
            using (mocks.Ordered())
            {
                configuration.Stub(c => c.Initialized).Return(false);
                configuration.Expect(c => c.Initialize());
                configuration.Stub(c => c.TileSchema).Return(schema);
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