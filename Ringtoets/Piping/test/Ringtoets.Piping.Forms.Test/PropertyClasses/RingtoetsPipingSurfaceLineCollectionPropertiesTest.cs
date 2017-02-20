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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineCollectionPropertiesTest
    {
        [Test]
        public void Constructor_WithoutColleciton_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new RingtoetsPipingSurfaceLineCollectionProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collection", paramName);
        }

        [Test]
        public void Constructor_WithData_ReturnExpectedValues()
        {
            // Setup
            var someFilePath = "location/to/a/file";
            var collection = new RingtoetsPipingSurfaceLineCollection();
            collection.AddRange(Enumerable.Empty<RingtoetsPipingSurfaceLine>(), someFilePath);

            // Call
            var properties = new RingtoetsPipingSurfaceLineCollectionProperties(collection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<RingtoetsPipingSurfaceLineCollection>>(properties);
            Assert.AreSame(collection, properties.Data);
            Assert.AreEqual(someFilePath, properties.SourcePath);
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var collection = new RingtoetsPipingSurfaceLineCollection();

            // Call
            var properties = new RingtoetsPipingSurfaceLineCollectionProperties(collection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor surfaceLineCollectionProperty = dynamicProperties[0];
            Assert.IsNotNull(surfaceLineCollectionProperty);
            Assert.IsTrue(surfaceLineCollectionProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", surfaceLineCollectionProperty.Category);
            Assert.AreEqual("Bronlocatie", surfaceLineCollectionProperty.DisplayName);
            Assert.AreEqual(
                "De locatie van het bestand waaruit de profielschematisaties zijn geïmporteerd.",
                surfaceLineCollectionProperty.Description);
        }
    }
}