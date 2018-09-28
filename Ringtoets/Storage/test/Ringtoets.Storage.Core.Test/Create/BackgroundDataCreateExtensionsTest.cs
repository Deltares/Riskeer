// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create
{
    [TestFixture]
    public class BackgroundDataCreateExtensionsTest
    {
        [Test]
        public void Create_BackgroundDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((BackgroundData) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("backgroundData", exception.ParamName);
        }

        [Test]
        public void Create_BackgroundDataContainsConfiguredWMTSConfiguration_ReturnsBackgroundDataEntity()
        {
            // Setup
            const string name = "background";
            const string sourceCapabilitiesUrl = "//url";
            const string selectedCapabilityName = "selectedName";
            const string preferredFormat = "image/png";
            const bool isVisible = true;
            const bool isConfigured = true;
            var transparancy = (RoundedDouble) 0.3;

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
                },
                {
                    BackgroundDataIdentifiers.SourceCapabilitiesUrl, sourceCapabilitiesUrl
                },
                {
                    BackgroundDataIdentifiers.SelectedCapabilityIdentifier, selectedCapabilityName
                },
                {
                    BackgroundDataIdentifiers.PreferredFormat, preferredFormat
                }
            };

            IEnumerable<KeyValuePair<string, string>> actualKeyValuePairs = entity.BackgroundDataMetaEntities.Select(
                metaEntity => new KeyValuePair<string, string>(metaEntity.Key, metaEntity.Value));
            CollectionAssert.AreEquivalent(expectedKeyValuePairs, actualKeyValuePairs);
        }

        [Test]
        public void Create_BackgroundDataContainsUnconfiguredWMTSConfiguration_ReturnsBackgroundDataEntity()
        {
            // Setup
            const string name = "background";
            const bool isVisible = true;
            const bool isConfigured = false;
            var transparancy = (RoundedDouble) 0.3;

            var configuration = new WmtsBackgroundDataConfiguration();

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

            IEnumerable<KeyValuePair<string, string>> actualKeyValuePairs = entity.BackgroundDataMetaEntities.Select(
                metaEntity => new KeyValuePair<string, string>(metaEntity.Key, metaEntity.Value));
            CollectionAssert.AreEquivalent(expectedKeyValuePairs, actualKeyValuePairs);
        }

        [Test]
        public void Create_BackgroundDataContainsWellKnownConfiguration_ReturnsBackgroundDataEntity()
        {
            // Setup
            var random = new Random(21);
            var wellKnownTileSource = random.NextEnumValue<RingtoetsWellKnownTileSource>();

            const string name = "background";
            const bool isVisible = true;
            const BackgroundDataType backgroundDataType = BackgroundDataType.WellKnown;
            var transparancy = (RoundedDouble) 0.3;

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
                    BackgroundDataIdentifiers.WellKnownTileSource, ((int) wellKnownTileSource).ToString()
                }
            };
            IEnumerable<KeyValuePair<string, string>> actualKeyValuePairs = entity.BackgroundDataMetaEntities.Select(
                metaEntity => new KeyValuePair<string, string>(metaEntity.Key, metaEntity.Value));
            CollectionAssert.AreEquivalent(expectedKeyValuePairs, actualKeyValuePairs);
        }
    }
}