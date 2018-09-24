// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using BruTile.Web;
using Core.Components.BruTile.Configurations;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.BruTile.Test.Configurations
{
    [TestFixture]
    public class BruTileReflectionHelperTest
    {
        [Test]
        public void GetProviderFromTileSource_TileSourceNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => BruTileReflectionHelper.GetProviderFromTileSource(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("source", paramName);
        }

        [Test]
        public void GetProviderFromTileSource_HttpTileSourceInstance_ReturnTileProvider()
        {
            // Setup
            var schema = new TileSchema();
            var tileSource = new HttpTileSource(schema, (IRequest) null);

            // Call
            ITileProvider provider = BruTileReflectionHelper.GetProviderFromTileSource(tileSource);

            // Assert
            Assert.IsInstanceOf<HttpTileProvider>(provider);
        }

        [Test]
        public void GetProviderFromTileSource_HttpTileSourceInheritor_ReturnTileProvider()
        {
            // Setup
            var schema = new TileSchema();
            var tileSource = new HttpTileSourceInheritor(schema, null);

            // Call
            ITileProvider provider = BruTileReflectionHelper.GetProviderFromTileSource(tileSource);

            // Assert
            Assert.IsInstanceOf<HttpTileProvider>(provider);
        }

        [Test]
        public void GetProviderFromTileSource_TileSourceInstance_ReturnTileProvider()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            var schema = new TileSchema();
            var tileSource = new TileSource(tileProvider, schema);

            // Call
            ITileProvider provider = BruTileReflectionHelper.GetProviderFromTileSource(tileSource);

            // Assert
            Assert.AreSame(tileProvider, provider);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProviderFromTileSource_TileSourceInheritor_ReturnTileProvider()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            var schema = new TileSchema();
            var tileSource = new TileSourceInheritor(tileProvider, schema);

            // Call
            ITileProvider provider = BruTileReflectionHelper.GetProviderFromTileSource(tileSource);

            // Assert
            Assert.AreSame(tileProvider, provider);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProviderFromTileSource_UnsupportedTileSource_ThrowNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileSource = mocks.Stub<ITileSource>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => BruTileReflectionHelper.GetProviderFromTileSource(tileSource);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            StringAssert.StartsWith("Unable to find a ITileProvider field for type ITileSourceProxy", message);

            mocks.VerifyAll();
        }

        private class HttpTileSourceInheritor : HttpTileSource
        {
            public HttpTileSourceInheritor(ITileSchema tileSchema, IRequest request) : base(tileSchema, request) {}
        }

        private class TileSourceInheritor : TileSource
        {
            public TileSourceInheritor(ITileProvider tileProvider, ITileSchema tileSchema) : base(tileProvider, tileSchema) {}
        }
    }
}