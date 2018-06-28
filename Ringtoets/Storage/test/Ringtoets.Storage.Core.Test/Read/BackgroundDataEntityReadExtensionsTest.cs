// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;

namespace Ringtoets.Storage.Core.Test.Read
{
    [TestFixture]
    public class BackgroundDataEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((BackgroundDataEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_EntityWithWmtsData_ReturnBackgroundData()
        {
            // Setup
            const string name = "map data";
            const string url = "//url";
            const string capabilityName = "capability name";
            const string preferredFormat = "image/jpeg";
            const bool isVisible = false;
            const double transparancy = 0.4;
            const bool isConfigured = true;

            const BackgroundDataType backgroundDataType = BackgroundDataType.Wmts;
            var backgroundDataMetaEntities = new[]
            {
                new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.IsConfigured,
                    Value = Convert.ToByte(isConfigured).ToString()
                },
                new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.SourceCapabilitiesUrl,
                    Value = url
                },
                new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.SelectedCapabilityIdentifier,
                    Value = capabilityName
                },
                new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.PreferredFormat,
                    Value = preferredFormat
                }
            };

            var entity = new BackgroundDataEntity
            {
                Name = name,
                BackgroundDataMetaEntities = backgroundDataMetaEntities,
                IsVisible = Convert.ToByte(isVisible),
                Transparency = transparancy,
                BackgroundDataType = Convert.ToByte(backgroundDataType)
            };

            // Call
            BackgroundData backgroundData = entity.Read();

            // Assert
            Assert.AreEqual(isVisible, backgroundData.IsVisible);
            Assert.AreEqual(transparancy, backgroundData.Transparency.Value);
            Assert.AreEqual(name, backgroundData.Name);

            Assert.IsNotNull(backgroundData.Configuration);
            var configuration = (WmtsBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(isConfigured, configuration.IsConfigured);
            Assert.AreEqual(url, configuration.SourceCapabilitiesUrl);
            Assert.AreEqual(capabilityName, configuration.SelectedCapabilityIdentifier);
            Assert.AreEqual(preferredFormat, configuration.PreferredFormat);
        }

        [Test]
        public void Read_EntityWithWellKnownData_ReturnBackgroundData()
        {
            // Setup
            const string name = "map data";
            const bool isVisible = false;
            const double transparancy = 0.4;

            const BackgroundDataType backgroundDataType = BackgroundDataType.WellKnown;

            var random = new Random(21);
            var wellKnownTileSource = random.NextEnumValue<RingtoetsWellKnownTileSource>();

            var entity = new BackgroundDataEntity
            {
                Name = name,
                BackgroundDataMetaEntities = new List<BackgroundDataMetaEntity>
                {
                    new BackgroundDataMetaEntity
                    {
                        Key = BackgroundDataIdentifiers.WellKnownTileSource,
                        Value = ((int) wellKnownTileSource).ToString()
                    }
                },
                IsVisible = Convert.ToByte(isVisible),
                Transparency = transparancy,
                BackgroundDataType = Convert.ToByte(backgroundDataType)
            };

            // Call
            BackgroundData backgroundData = entity.Read();

            // Assert
            Assert.AreEqual(isVisible, backgroundData.IsVisible);
            Assert.AreEqual(transparancy, backgroundData.Transparency.Value);

            Assert.AreEqual(name, backgroundData.Name);

            Assert.IsNotNull(backgroundData.Configuration);
            var configuration = (WellKnownBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(wellKnownTileSource, configuration.WellKnownTileSource);
        }

        [Test]
        public void Read_WmtsConfigurationIsConfiguredFalse_NoParametersAdded()
        {
            // Setup
            const bool isConfigured = false;

            var entity = new BackgroundDataEntity
            {
                BackgroundDataMetaEntities = new List<BackgroundDataMetaEntity>
                {
                    new BackgroundDataMetaEntity
                    {
                        Key = BackgroundDataIdentifiers.IsConfigured,
                        Value = Convert.ToByte(isConfigured).ToString()
                    },
                    new BackgroundDataMetaEntity
                    {
                        Key = BackgroundDataIdentifiers.SourceCapabilitiesUrl,
                        Value = "//url"
                    },
                    new BackgroundDataMetaEntity
                    {
                        Key = BackgroundDataIdentifiers.SelectedCapabilityIdentifier,
                        Value = "capability name"
                    },
                    new BackgroundDataMetaEntity
                    {
                        Key = BackgroundDataIdentifiers.PreferredFormat,
                        Value = "image/jpeg"
                    }
                },
                BackgroundDataType = Convert.ToByte(BackgroundDataType.Wmts)
            };

            // Call
            BackgroundData backgroundData = entity.Read();

            // Assert
            var configuration = (WmtsBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(false, configuration.IsConfigured);
            Assert.IsNull(configuration.SourceCapabilitiesUrl);
            Assert.IsNull(configuration.SelectedCapabilityIdentifier);
            Assert.IsNull(configuration.PreferredFormat);
        }
    }
}