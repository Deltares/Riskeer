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
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Configurations;

namespace Riskeer.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsGridConfigurationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var configuration = new MacroStabilityInwardsGridConfiguration();

            // Assert
            Assert.IsNull(configuration.XLeft);
            Assert.IsNull(configuration.XRight);
            Assert.IsNull(configuration.ZTop);
            Assert.IsNull(configuration.ZBottom);
            Assert.IsNull(configuration.NumberOfHorizontalPoints);
            Assert.IsNull(configuration.NumberOfVerticalPoints);
        }

        [Test]
        public void Constructor_SetProperties_ExpectedValues()
        {
            // Setup
            var random = new Random(31);
            double xLeft = random.NextDouble();
            double xRight = random.NextDouble();
            double zTop = random.NextDouble();
            double zBottom = random.NextDouble();
            int numberOfHorizontalPoints = random.Next();
            int numberOfVerticalPoints = random.Next();

            // Call
            var configuration = new MacroStabilityInwardsGridConfiguration
            {
                XLeft = xLeft,
                XRight = xRight,
                ZTop = zTop,
                ZBottom = zBottom,
                NumberOfHorizontalPoints = numberOfHorizontalPoints,
                NumberOfVerticalPoints = numberOfVerticalPoints
            };

            // Assert
            Assert.AreEqual(xLeft, configuration.XLeft);
            Assert.AreEqual(xRight, configuration.XRight);
            Assert.AreEqual(zTop, configuration.ZTop);
            Assert.AreEqual(zBottom, configuration.ZBottom);
            Assert.AreEqual(numberOfHorizontalPoints, configuration.NumberOfHorizontalPoints);
            Assert.AreEqual(numberOfVerticalPoints, configuration.NumberOfVerticalPoints);
        }
    }
}