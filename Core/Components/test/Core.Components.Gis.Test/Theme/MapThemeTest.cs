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
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Theme
{
    [TestFixture]
    public class MapThemeTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_InvalidAttribute_ThrowsArgumentException(string invalidAttributeName)
        {
            // Call
            TestDelegate call = () => new MapTheme(invalidAttributeName, new[]
            {
                CategoryThemeTestFactory.CreateCategoryTheme()
            });

            // Assert
            const string expectedMessage = "attributeName is null, empty or consists of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_CategoryThemesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapTheme("Arbitrary attribute", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categoryThemes", exception.ParamName);
        }

        [Test]
        public void Constructor_CategoryThemesEmpty_ThrowsArgumentException()
        {
            // Setup
            IEnumerable<CategoryTheme> mapCategories = Enumerable.Empty<CategoryTheme>();

            // Call
            TestDelegate call = () => new MapTheme("Arbitrary attribute", mapCategories);

            // Assert
            const string expectedMessage = "categoryThemes is empty.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ValidArguments_SetsExpectedValues()
        {
            // Setup
            const string attributeName = "Arbitrary attribute";
            CategoryTheme[] mapCategories =
            {
                CategoryThemeTestFactory.CreateCategoryTheme()
            };

            // Call
            var theme = new MapTheme(attributeName, mapCategories);

            // Assert
            Assert.AreEqual(attributeName, theme.AttributeName);
            Assert.AreSame(mapCategories, theme.CategoryThemes);
        }
    }
}