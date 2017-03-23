// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class BackgroundDataCreateExtensionsTest
    {
        [Test]
        public void Create_BackgroundDataNull_ThrowArgumentNullException()
        {
            // Setup
            BackgroundData backgroundData = null;

            // Call
            TestDelegate test = () => backgroundData.Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("backgroundData", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_BackgroundDataContainsWMTSConfiguration_ReturnsBackgroundMapDataEntity(bool isConfigured)
        {
            // Setup
            const string name = "background";
            const string sourceCapabilitiesUrl = "//url";
            const string selectedCapabilityName = "selectedName";
            const string preferredFormat = "image/png";
            const bool isVisible = true;
            RoundedDouble transparancy = (RoundedDouble) 0.3;

            var configuration = new WmtsBackgroundDataConfiguration(isConfigured,
                                                                    sourceCapabilitiesUrl,
                                                                    selectedCapabilityName,
                                                                    preferredFormat);

            var backgroundData = new BackgroundData(configuration)
            {
                IsVisible = isVisible,
                Transparency = transparancy,
                Name = name
            };

            // Call
            BackgroundDataEntity entity = backgroundData.Create();

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(Convert.ToByte(isVisible), entity.IsVisible);
            Assert.AreEqual(transparancy, entity.Transparency);

            var expectedKeyValuePairs = new Dictionary<string, string>
            {
                {
                    BackgroundDataIdentifiers.IsConfigured, Convert.ToByte(isConfigured).ToString()
                }
            };

            if (isConfigured)
            {
                expectedKeyValuePairs.Add(BackgroundDataIdentifiers.SourceCapabilitiesUrl, sourceCapabilitiesUrl);
                expectedKeyValuePairs.Add(BackgroundDataIdentifiers.SelectedCapabilityIdentifier, selectedCapabilityName);
                expectedKeyValuePairs.Add(BackgroundDataIdentifiers.PreferredFormat, preferredFormat);
            }

            var actualKeyValuePairs = entity.BackgroundDataMetaEntities.Select(
                metaEntity => new KeyValuePair<string, string>(metaEntity.Key, metaEntity.Value));
            CollectionAssert.AreEquivalent(expectedKeyValuePairs, actualKeyValuePairs);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_BackgroundDataContainsWellKnownConfiguration_ReturnsBackgroundMapDataEntity(bool isConfigured)
        {
            // Setup
            var random = new Random(21);
            var wellKnownTileSource = random.NextEnumValue<WellKnownTileSource>();

            const string name = "background";
            const bool isVisible = true;
            const BackgroundMapDataType backgroundDataType = BackgroundMapDataType.WellKnown;
            RoundedDouble transparancy = (RoundedDouble)0.3;

            var configuration = new WellKnownBackgroundDataConfiguration(wellKnownTileSource);
            var backgroundData = new BackgroundData(configuration)
            {
                IsVisible = isVisible,
                Transparency = transparancy,
                Name = name
            };

            // Call
            BackgroundDataEntity entity = backgroundData.Create();

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(Convert.ToByte(isVisible), entity.IsVisible);
            Assert.AreEqual(transparancy, entity.Transparency);
            Assert.AreEqual(Convert.ToByte(backgroundDataType), entity.BackgroundDataType);

            var expectedKeyValuePairs = new Dictionary<string, string>
            {
                {
                    BackgroundDataIdentifiers.WellKnownTileSource, ((int)wellKnownTileSource).ToString()
                }
            };
            var actualKeyValuePairs = entity.BackgroundDataMetaEntities.Select(
                metaEntity => new KeyValuePair<string, string>(metaEntity.Key, metaEntity.Value));
            CollectionAssert.AreEquivalent(expectedKeyValuePairs, actualKeyValuePairs);
        }

    }
}