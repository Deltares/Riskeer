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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Categories;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapThemeTest
    {
        [Test]
        [TestCase("   ")]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_InvalidAttribute_ThrowsArgumentException(string invalidAttributeName)
        {
            // Setup
            var mocks = new MockRepository();
            var mapCategory = mocks.Stub<IMapCategory>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new MapTheme(invalidAttributeName, new []
            {
                mapCategory
            });

            // Assert
            const string expectedMessage = "AttributeName is null, empty or consists of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_MapCategoriesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapTheme("Arbitrary attribute", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mapCategories", exception.ParamName);
        }

        [Test]
        public void Constructor_MapCategoriesEmpty_ThrowsArgumentException()
        {
            // Setup
            IEnumerable<IMapCategory> mapCategories = Enumerable.Empty<IMapCategory>();

            // Call
            TestDelegate call = () => new MapTheme("Arbitrary attribute", mapCategories);

            // Assert
            const string expectedMessage = "MapCategories is empty.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ValidArguments_SetsExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var mapCategory = mocks.Stub<IMapCategory>();
            mocks.ReplayAll();

            const string attributeName = "Arbitrary attribute";
            var mapCategories = new[]
            {
                mapCategory
            };

            // Call
            var theme = new MapTheme(attributeName, mapCategories);

            // Assert
            Assert.AreEqual(attributeName, theme.AttributeName);
            Assert.AreSame(mapCategories, theme.MapCategories);
            mocks.VerifyAll();
        }
    }
}