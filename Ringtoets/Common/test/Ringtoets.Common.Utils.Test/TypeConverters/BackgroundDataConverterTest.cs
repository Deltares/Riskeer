﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Utils.TypeConverters;

namespace Ringtoets.Common.Utils.Test.TypeConverters
{
    [TestFixture]
    public class BackgroundDataConverterTest
    {
        private static IEnumerable<TestCaseData> WmtsMapDatas
        {
            get
            {
                yield return new TestCaseData(WmtsMapData.CreateDefaultPdokMapData())
                    .SetName("Configured WMTS map data.");
                yield return new TestCaseData(WmtsMapData.CreateUnconnectedMapData())
                    .SetName("Not configured WMTS map data.");
            }
        }

        [Test]
        public void ConvertTo_MapDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => BackgroundDataConverter.ConvertTo(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("mapData", exception.ParamName);
        }

        [Test]
        public void ConvertTo_MapDataNotSupportedForConversion_ThrowNotSupportedException()
        {
            // Setup
            var unsupportedImageBasedMapData = new TestImageBasedMapData("What's in a name?", false);

            // Call 
            TestDelegate call = () => BackgroundDataConverter.ConvertTo(unsupportedImageBasedMapData);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(call);
            string expectedMessage = $"Can't create a background data configuration for {unsupportedImageBasedMapData.GetType()}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConvertTo_WmtsMapData_ReturnBackgroundData(bool configured)
        {
            // Setup
            WmtsMapData mapData = configured
                                      ? WmtsMapData.CreateDefaultPdokMapData()
                                      : WmtsMapData.CreateUnconnectedMapData();

            // Call
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);

            // Assert
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.AreEqual(mapData.IsVisible, backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);

            var configuration = (WmtsBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(configured, configuration.IsConfigured);
            Assert.AreEqual(mapData.SourceCapabilitiesUrl, configuration.SourceCapabilitiesUrl);
            Assert.AreEqual(mapData.SelectedCapabilityIdentifier, configuration.SelectedCapabilityIdentifier);
            Assert.AreEqual(mapData.PreferredFormat, configuration.PreferredFormat);
        }

        [Test]
        public void ConvertTo_WellKnownMapData_ReturnBackgroundData()
        {
            // Setup
            var random = new Random(21);
            WellKnownTileSource wellKnownTileSource = random.NextEnumValue<WellKnownTileSource>();
            var mapData = new WellKnownTileSourceMapData(wellKnownTileSource);

            // Call
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);

            // Assert
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.AreEqual(mapData.IsVisible, backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);

            var configuration = (WellKnownBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(mapData.TileSource, configuration.WellKnownTileSource);
        }

        [Test]
        public void ConvertFrom_BackgroundDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => BackgroundDataConverter.ConvertFrom(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("backgroundData", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(WmtsMapDatas))]
        public void ConvertFrom_BackgroundData_ReturnWmtsMapData(WmtsMapData mapData)
        {
            // Setup
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            // Call
            ImageBasedMapData convertedMapData = BackgroundDataConverter.ConvertFrom(backgroundData);

            // Assert
            MapDataTestHelper.AssertImageBasedMapData(mapData, convertedMapData);
        }

        [Test]
        public void ConvertFrom_BackgroundData_ReturnWellKnownMapData()
        {
            // Setup            
            var random = new Random(21);
            WellKnownTileSource wellKnownTileSource = random.NextEnumValue<WellKnownTileSource>();
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData(wellKnownTileSource);

            // Call
            ImageBasedMapData convertedMapData = BackgroundDataConverter.ConvertFrom(backgroundData);

            // Assert
            var expectedMapData = new WellKnownTileSourceMapData(wellKnownTileSource);
            MapDataTestHelper.AssertImageBasedMapData(expectedMapData, convertedMapData);
        }

        [Test]
        public void ConvertFrom_BackgroundDataWithInvalidWellKnownTileSourceValue_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var backgroundData = new BackgroundData(new WellKnownBackgroundDataConfiguration((WellKnownTileSource) 999));

            // Call
            TestDelegate call = () => BackgroundDataConverter.ConvertFrom(backgroundData);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(call);
        }
    }
}