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
        public void Create_Always_ReturnsBackgroundMapDataEntity(bool isConfigured)
        {
            // Setup
            const string name = "background";
            const string sourceCapabilitiesUrl = "//url";
            const string selectedCapabilityName = "selectedName";
            const string preferredFormat = "image/png";
            const bool isVisible = true;
            const BackgroundMapDataType backgroundDataType = BackgroundMapDataType.Wmts;
            RoundedDouble transparancy = (RoundedDouble) 0.3;

            var backgroundData = new BackgroundData
            {
                IsVisible = isVisible,
                Transparency = transparancy,
                Name = name,
                IsConfigured = isConfigured,
                Parameters =
                {
                    { BackgroundDataIdentifiers.SourceCapabilitiesUrl, sourceCapabilitiesUrl },
                    { BackgroundDataIdentifiers.SelectedCapabilityIdentifier, selectedCapabilityName },
                    { BackgroundDataIdentifiers.PreferredFormat, preferredFormat }
                },
                BackgroundMapDataType = backgroundDataType
            };

            // Call
            BackgroundDataEntity entity = backgroundData.Create();

            // Assert
            Assert.AreEqual(name, entity.Name);            
            Assert.AreEqual(Convert.ToByte(isVisible), entity.IsVisible);
            Assert.AreEqual(transparancy, entity.Transparency);
            Assert.AreEqual(Convert.ToByte(backgroundDataType), entity.BackgroundDataType);
            Assert.AreEqual(Convert.ToByte(isConfigured), entity.IsConfigured);

            if (isConfigured)
            {
                Assert.AreEqual(3, entity.BackgroundDataMetaEntities.Count);

                foreach (KeyValuePair<string, string> parameter in backgroundData.Parameters)
                {
                    BackgroundDataMetaEntity backgroundDataMetaEntity = entity.BackgroundDataMetaEntities.Single(e => e.Key.Equals(parameter.Key));
                    Assert.AreEqual(parameter.Value, backgroundDataMetaEntity.Value);
                }
            }
            else
            {
                CollectionAssert.IsEmpty(entity.BackgroundDataMetaEntities);
            }
        }
    }
}