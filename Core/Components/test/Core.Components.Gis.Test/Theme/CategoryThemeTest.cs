// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using Core.Common.TestUtil;
using Core.Components.Gis.Theme;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Theme
{
    [TestFixture]
    public class CategoryThemeTest
    {
        [Test]
        public void Constructor_CriterionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            Color themeColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());

            // Call
            TestDelegate call = () => new CategoryTheme(themeColor, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("criterion", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var criterion = new ValueCriterion(random.NextEnumValue<ValueCriterionOperator>(),
                                               "test value");

            Color themeColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());

            // Call
            var category = new CategoryTheme(themeColor, criterion);

            // Assert
            Assert.AreEqual(themeColor, category.Color);
            Assert.AreSame(criterion, category.Criterion);
        }
    }
}