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
using System.Linq;
using BruTile;
using BruTile.Wmts;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.DotSpatial.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.DotSpatial.Test.Layer.BruTile.Configurations
{
    [TestFixture]
    public class WmtsLayerConfigurationTest
    {
        private const string validPreferredFormat = "image/png";

        [Test]
        public void CreateInitializedConfiguration_CapabilitiesUrlNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration(null, "A", "images/png");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wmtsCapabilitiesUrl", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_CapabilityIdNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration("A", null, "images/png");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("capabilityIdentifier", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_PreferredFormatNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration("A", "B", null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_PreferredFormatNotMime_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration("A", "B", "png");

            // Assert
            string message = "Afbeelding formaat moet opgegeven worden als MIME-type.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_LayerIdNotInWmts_ThrowCannotFindTileSourceException()
        {
            // Setup
            const string url = "url";
            const string id = "id";
            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(url)).Return(Enumerable.Empty<ITileSource>());
            mocks.ReplayAll();

            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                // Call
                TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration(url, id, validPreferredFormat);
                
                // Assert
                string message= Assert.Throws<CannotFindTileSourceException>(call).Message;
                string expectedMessage = $"Niet in staat om de databron met naam '{id}' te kunnen vinden bij de WMTS url '{url}'.";
                Assert.AreEqual(expectedMessage, message);
            }
            mocks.VerifyAll();
        }
    }
}