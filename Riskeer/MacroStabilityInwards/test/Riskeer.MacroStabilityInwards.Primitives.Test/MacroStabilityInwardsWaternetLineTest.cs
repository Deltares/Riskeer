﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Primitives.TestUtil;

namespace Riskeer.MacroStabilityInwards.Primitives.Test
{
    public class MacroStabilityInwardsWaternetLineTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsWaternetLine(null, new List<Point2D>(), MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsWaternetLine("name", null, MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_PhreaticLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsWaternetLine("name", new List<Point2D>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("phreaticLine", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string waternetLineName = "<some name>";
            const string phreaticLineName = "PL1";
            var geometry = new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(15.0, 15.0)
            };

            // Call
            var waternetLine = new MacroStabilityInwardsWaternetLine(waternetLineName,
                                                                     geometry,
                                                                     new MacroStabilityInwardsPhreaticLine(phreaticLineName, Enumerable.Empty<Point2D>()));

            // Assert
            Assert.AreEqual(waternetLineName, waternetLine.Name);
            CollectionAssert.AreEqual(new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(15.0, 15.0)
            }, waternetLine.Geometry);
            Assert.AreEqual(phreaticLineName, waternetLine.PhreaticLine.Name);
            CollectionAssert.IsEmpty(waternetLine.PhreaticLine.Geometry);
        }

        [TestFixture]
        private class MacroStabilityInwardsWaternetLineEqualsTest : EqualsTestFixture<MacroStabilityInwardsWaternetLine>
        {
            protected override MacroStabilityInwardsWaternetLine CreateObject()
            {
                return CreateWaternetLine();
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new MacroStabilityInwardsWaternetLine("Other name", new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine()))
                    .SetName("Other name");

                yield return new TestCaseData(new MacroStabilityInwardsWaternetLine("Test", new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1),
                        new Point2D(1, 1)
                    }, MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine()))
                    .SetName("Geometry Count");

                yield return new TestCaseData(new MacroStabilityInwardsWaternetLine("Test", new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(2, 2)
                    }, MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine()))
                    .SetName("Geometry content");

                yield return new TestCaseData(new MacroStabilityInwardsWaternetLine("Test", new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, new MacroStabilityInwardsPhreaticLine(
                        "Test",
                        new[]
                        {
                            new Point2D(0, 0)
                        })))
                    .SetName("Other phreatic line");
            }

            private static MacroStabilityInwardsWaternetLine CreateWaternetLine()
            {
                return new MacroStabilityInwardsWaternetLine(
                    "Test",
                    new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    },
                    MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine());
            }
        }
    }
}