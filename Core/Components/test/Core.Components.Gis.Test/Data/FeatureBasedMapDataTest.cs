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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class FeatureBasedMapDataTest
    {
        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Call
            var data = new TestFeatureBasedMapData("test data");

            // Assert
            Assert.AreEqual("test data", data.Name);
            Assert.IsEmpty(data.Features);
            Assert.IsFalse(data.ShowLabels);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new TestFeatureBasedMapData(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the map data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Features_SetValidNewValue_GetsNewValue()
        {
            // Setup
            var data = new TestFeatureBasedMapData("test data");
            var features = new[]
            {
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>())
            };

            // Call
            data.Features = features;

            // Assert
            Assert.AreSame(features, data.Features);
        }

        [Test]
        public void Features_SetNullValue_ThrowsArgumentNullException()
        {
            // Setup
            var data = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => data.Features = null;

            // Assert
            const string expectedMessage = "The array of features cannot be null or contain null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Features_SetCollectionWithNullElement_ThrowsArgumentNullException()
        {
            // Setup
            var data = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => data.Features = new MapFeature[]
            {
                null
            };

            // Assert
            const string expectedMessage = "The array of features cannot be null or contain null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        private class TestFeatureBasedMapData : FeatureBasedMapData
        {
            public TestFeatureBasedMapData(string name) : base(name) {}
        }
    }
}