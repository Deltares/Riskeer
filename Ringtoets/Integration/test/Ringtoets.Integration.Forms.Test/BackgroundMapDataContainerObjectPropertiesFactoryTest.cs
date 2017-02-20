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
using Core.Components.Gis;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class BackgroundMapDataContainerObjectPropertiesFactoryTest
    {
        [Test]
        public void CreateObjectProperties_BackgroundMapDataContainerNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => BackgroundMapDataContainerObjectPropertiesFactory.GetObjectProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("container", paramName);
        }

        [Test]
        public void CreateObjectProperties_BackgroundMapDataContainerWithoutMapData_ReturnBackgroundMapDataContainerProperties()
        {
            // Setup
            var container = new BackgroundMapDataContainer();

            // Call
            BackgroundMapDataContainerProperties properties = BackgroundMapDataContainerObjectPropertiesFactory.GetObjectProperties(container);

            // Assert
            Assert.AreEqual(typeof(BackgroundMapDataContainerProperties), properties.GetType());
            Assert.AreSame(container, properties.Data);
        }

        [Test]
        public void CreateObjectProperties_BackgroundMapDataContainerWithWellKnownTileSourceMapData_ReturnBackgroundMapDataContainerProperties()
        {
            // Setup
            var container = new BackgroundMapDataContainer
            {
                MapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)
            };

            // Call
            BackgroundMapDataContainerProperties properties = BackgroundMapDataContainerObjectPropertiesFactory.GetObjectProperties(container);

            // Assert
            Assert.AreEqual(typeof(BackgroundMapDataContainerProperties), properties.GetType());
            Assert.AreSame(container, properties.Data);
        }

        [Test]
        public void CreateObjectProperties_BackgroundMapDataContainerWithWmtsMapData_ReturnBackgroundWmtsMapDataContainerProperties()
        {
            // Setup
            var container = new BackgroundMapDataContainer
            {
                MapData = WmtsMapData.CreateDefaultPdokMapData()
            };

            // Call
            BackgroundMapDataContainerProperties properties = BackgroundMapDataContainerObjectPropertiesFactory.GetObjectProperties(container);

            // Assert
            Assert.AreEqual(typeof(BackgroundWmtsMapDataContainerProperties), properties.GetType());
            Assert.AreSame(container, properties.Data);
        }
    }
}