﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class SoilLayerVisualizationHelperTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetDisplayName_InvalidName_ReturnValidName(string invalidName)
        {
            // Call
            string validName = SoilLayerVisualizationHelper.GetDisplayName(invalidName);

            // Assert
            Assert.AreEqual("Onbekend", validName);
        }

        [Test]
        public void GetDisplayName_ValidName_ReturnName()
        {
            // Setup
            const string name = "Test";

            // Call
            string validName = SoilLayerVisualizationHelper.GetDisplayName(name);

            // Assert
            Assert.AreEqual(name, validName);
        }

        [Test]
        public void GetDisplayColor_ColorEmpty_ReturnColorWhite()
        {
            // Call
            Color validColor = SoilLayerVisualizationHelper.GetDisplayColor(Color.Empty);

            // Assert
            Assert.AreEqual(Color.White, validColor);
        }

        [Test]
        public void GetDisplayColor_ValidColor_ReturnColor()
        {
            // Setup
            var random = new Random(21);
            Color color = Color.FromKnownColor(random.NextEnumValue<KnownColor>());

            // Call
            Color validColor = SoilLayerVisualizationHelper.GetDisplayColor(color);

            // Assert
            Assert.AreEqual(color, validColor);
        }
    }
}