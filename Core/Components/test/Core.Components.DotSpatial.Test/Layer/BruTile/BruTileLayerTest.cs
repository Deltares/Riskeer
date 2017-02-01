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
using BruTile;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
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
                var schema = mocks.Stub<ITileSchema>();
                const string authorityCode = "EPSG:28992";
                schema.Stub(s => s.Srs).Return(authorityCode);
                var extent = new Extent(10000, 123456, 987654321, 321654);
                schema.Stub(s => s.Extent).Return(extent);

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                string legendText = "<Legend Text>";
                var configuration = mocks.Stub<IConfiguration>();
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return(legendText);
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
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
                var schema = mocks.Stub<ITileSchema>();
                const string authorityCode = "EPSG:28992";
                schema.Stub(s => s.Srs).Return(authorityCode);
                var extent = new Extent(10000, 123456, 987654321, 321654);
                schema.Stub(s => s.Extent).Return(extent);

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                string legendText = "<Legend Text>";
                var configuration = mocks.StrictMock<IConfiguration>();
                using (mocks.Ordered())
                {
                    configuration.Stub(c => c.Initialized).Return(false);
                    configuration.Expect(c => c.Initialize());
                    configuration.Stub(c => c.TileSource).Return(tileSource);
                    configuration.Stub(c => c.LegendText).Return(legendText);
                    configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
                }
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
                var schema = mocks.Stub<ITileSchema>();
                schema.Stub(s => s.Srs).Return(authorityNumber);
                schema.Stub(s => s.Extent).Return(new Extent());

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                var configuration = mocks.StrictMock<IConfiguration>();
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return("A");
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
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
                var schema = mocks.Stub<ITileSchema>();
                schema.Stub(s => s.Srs).Return(esriProjectionString);
                schema.Stub(s => s.Extent).Return(new Extent());

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                var configuration = mocks.StrictMock<IConfiguration>();
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return("A");
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    var expectedProjection = ProjectionInfo.FromEsriString(esriProjectionString);
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
                var schema = mocks.Stub<ITileSchema>();
                schema.Stub(s => s.Srs).Return(proj4String);
                schema.Stub(s => s.Extent).Return(new Extent());

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                var configuration = mocks.StrictMock<IConfiguration>();
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return("A");
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    var expectedProjection = ProjectionInfo.FromProj4String(proj4String);
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
                var schema = mocks.Stub<ITileSchema>();
                schema.Stub(s => s.Srs).Return(authorityCode);
                schema.Stub(s => s.Extent).Return(new Extent());

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                var configuration = mocks.StrictMock<IConfiguration>();
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return("A");
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
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
                var schema = mocks.Stub<ITileSchema>();
                schema.Stub(s => s.Srs).Return(urnCode);
                schema.Stub(s => s.Extent).Return(new Extent());

                var tileSource = mocks.Stub<ITileSource>();
                tileSource.Stub(ts => ts.Schema).Return(schema);

                var configuration = mocks.StrictMock<IConfiguration>();
                configuration.Stub(c => c.Initialized).Return(true);
                configuration.Stub(c => c.TileSource).Return(tileSource);
                configuration.Stub(c => c.LegendText).Return("A");
                configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
                mocks.ReplayAll();

                // Call
                using (var layer = new BruTileLayer(configuration))
                {
                    // Assert
                    var expectedProjection = AuthorityCodeHandler.Instance["EPSG:3857"];
                    Assert.IsTrue(expectedProjection.Equals(layer.Projection));
                }
            }
            mocks.VerifyAll();
        }

        private static ProjectionInfo GetCorrectedWgs84Projection()
        {
            ProjectionInfo wgs84 = AuthorityCodeHandler.Instance["EPSG:3857"];
            wgs84.GeographicInfo.Datum = KnownCoordinateSystems.Geographic.World.WGS1984.GeographicInfo.Datum;
            return wgs84;
        }
    }
}