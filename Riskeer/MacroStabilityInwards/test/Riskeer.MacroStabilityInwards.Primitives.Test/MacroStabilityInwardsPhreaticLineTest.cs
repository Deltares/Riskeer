﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.Primitives.Test
{
    public class MacroStabilityInwardsPhreaticLineTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsPhreaticLine(null, new List<Point2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsPhreaticLine("name", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "<some name>";
            var geometry = new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(15.0, 15.0)
            };

            // Call
            var phreaticLine = new MacroStabilityInwardsPhreaticLine(name, geometry);

            // Assert
            Assert.AreEqual(name, phreaticLine.Name);
            CollectionAssert.AreEqual(new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(15.0, 15.0)
            }, phreaticLine.Geometry);
        }

        [TestFixture]
        private class MacroStabilityInwardsPhreaticLineEqualsTest : EqualsTestFixture<MacroStabilityInwardsPhreaticLine>
        {
            protected override MacroStabilityInwardsPhreaticLine CreateObject()
            {
                return CreatePhreaticLine();
            }

            public static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                MacroStabilityInwardsPhreaticLine baseLine = CreatePhreaticLine();

                yield return new TestCaseData(new MacroStabilityInwardsPhreaticLine("Other name", baseLine.Geometry))
                    .SetName("Other name");

                yield return new TestCaseData(new MacroStabilityInwardsPhreaticLine(baseLine.Name, new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1),
                        new Point2D(1, 1)
                    }))
                    .SetName("Geometry point count");

                yield return new TestCaseData(new MacroStabilityInwardsPhreaticLine(baseLine.Name, new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(2, 2)
                    }))
                    .SetName("Geometry coordinates");
            }

            private static MacroStabilityInwardsPhreaticLine CreatePhreaticLine()
            {
                return new MacroStabilityInwardsPhreaticLine(
                    "Test",
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    });
            }
        }
    }
}