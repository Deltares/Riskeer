﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Riskeer.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointResultTest
    {
        [Test]
        public void Constructor_DescriptionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new IllustrationPointResult(null, "-", 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("description", paramName);
        }

        [Test]
        public void Constructor_UnitNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new IllustrationPointResult("IllustrationPointResult", null, new Random(21).NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("unit", exception.ParamName);
        }

        [Test]
        public void Constructor_Always_ReturnsNewInstance()
        {
            // Setup
            const string description = "some description";
            const string unit = "-";
            const double value = 123;

            // Call
            var result = new IllustrationPointResult(description, unit, value);

            // Assert
            Assert.AreEqual(description, result.Description);
            Assert.AreEqual(unit, result.Unit);
            Assert.AreEqual(value, result.Value);
        }
    }
}