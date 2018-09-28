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
using System.Drawing;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Storage.Core.Test
{
    [TestFixture]
    public class ColorConversionExtensionsTest
    {
        [Test]
        public void ToInt64_WithColor_ReturnsArgbValue()
        {
            // Setup
            const int argb = 123;
            Color color = Color.FromArgb(argb);

            // Call
            long? result = color.ToInt64();

            // Assert
            Assert.AreEqual(argb, result);
        }

        [Test]
        public void ToInt64_WithEmptyColor_ReturnsNull()
        {
            // Call
            long? result = Color.Empty.ToInt64();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ToColor_WithValue_ReturnsColor()
        {
            // Setup
            long? argb = 123;

            // Call
            Color color = argb.ToColor();

            // Assert
            Assert.AreEqual(Color.FromArgb(Convert.ToInt32(argb)), color);
        }

        [Test]
        public void ToColor_ArgbNull_ReturnsEmptyColor()
        {
            // Call
            Color color = ColorConversionExtensions.ToColor(null);

            // Assert
            Assert.AreEqual(Color.Empty, color);
        }

        [Test]
        [TestCaseSource(nameof(ColorsToTest))]
        public void GivenColor_WhenConversionRoundtrip_ThenSameColor(Color color)
        {
            // When
            long? intValue = color.ToInt64();
            Color colorValue = intValue.ToColor();

            // Then
            Assert.AreEqual(color.ToArgb(), colorValue.ToArgb());
        }

        private static IEnumerable<Color> ColorsToTest()
        {
            var random = new Random(31);
            yield return Color.Black;
            yield return Color.White;
            yield return Color.Empty;
            yield return Color.Transparent;
            yield return Color.FromArgb(random.Next());
            yield return Color.FromKnownColor(random.NextEnumValue<KnownColor>());
        }
    }
}