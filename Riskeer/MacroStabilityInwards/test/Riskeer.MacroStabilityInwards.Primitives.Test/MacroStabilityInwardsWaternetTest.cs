// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
    public class MacroStabilityInwardsWaternetTest
    {
        [Test]
        public void Constructor_PhreaticLinesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsWaternet(null, Enumerable.Empty<MacroStabilityInwardsWaternetLine>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("phreaticLines", exception.ParamName);
        }

        [Test]
        public void Constructor_WaternetLinesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsWaternet(Enumerable.Empty<MacroStabilityInwardsPhreaticLine>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waternetLines", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var phreaticLine = new MacroStabilityInwardsPhreaticLine("Phreatic Line", Enumerable.Empty<Point2D>());
            var waternetLine = new MacroStabilityInwardsWaternetLine("Waternet Line", Enumerable.Empty<Point2D>(), phreaticLine);

            // Call
            var waternet = new MacroStabilityInwardsWaternet(new[]
            {
                phreaticLine
            }, new[]
            {
                waternetLine
            });

            // Assert
            Assert.AreSame(phreaticLine, waternet.PhreaticLines.Single());
            Assert.AreSame(waternetLine, waternet.WaternetLines.Single());
        }

        private class DerivedMacroStabilityInwardsWaternet : MacroStabilityInwardsWaternet
        {
            public DerivedMacroStabilityInwardsWaternet(MacroStabilityInwardsWaternet waternet)
                : base(waternet.PhreaticLines, waternet.WaternetLines) {}
        }

        [TestFixture]
        private class MacroStabilityInwardsWaternetEqualsTest
            : EqualsTestFixture<MacroStabilityInwardsWaternet, DerivedMacroStabilityInwardsWaternet>
        {
            protected override MacroStabilityInwardsWaternet CreateObject()
            {
                return CreateWaternet();
            }

            protected override DerivedMacroStabilityInwardsWaternet CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsWaternet(CreateWaternet());
            }

            private static MacroStabilityInwardsWaternet CreateWaternet()
            {
                return new MacroStabilityInwardsWaternet(
                    new MacroStabilityInwardsPhreaticLine[]
                    {
                        new TestMacroStabilityInwardsPhreaticLine()
                    },
                    new MacroStabilityInwardsWaternetLine[]
                    {
                        new TestMacroStabilityInwardsWaternetLine()
                    });
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new MacroStabilityInwardsWaternet(
                                                  new[]
                                                  {
                                                      new MacroStabilityInwardsPhreaticLine("Test", new Point2D[0])
                                                  },
                                                  new MacroStabilityInwardsWaternetLine[]
                                                  {
                                                      new TestMacroStabilityInwardsWaternetLine()
                                                  }))
                    .SetName("Other phreatic line");

                yield return new TestCaseData(new MacroStabilityInwardsWaternet(
                                                  new[]
                                                  {
                                                      new TestMacroStabilityInwardsPhreaticLine(),
                                                      new TestMacroStabilityInwardsPhreaticLine()
                                                  },
                                                  new MacroStabilityInwardsWaternetLine[]
                                                  {
                                                      new TestMacroStabilityInwardsWaternetLine()
                                                  }))
                    .SetName("Other phreatic line count");

                yield return new TestCaseData(new MacroStabilityInwardsWaternet(
                                                  new[]
                                                  {
                                                      new TestMacroStabilityInwardsPhreaticLine()
                                                  },
                                                  new[]
                                                  {
                                                      new MacroStabilityInwardsWaternetLine("Test", new Point2D[0], new TestMacroStabilityInwardsPhreaticLine())
                                                  }))
                    .SetName("Other waternet line");

                yield return new TestCaseData(new MacroStabilityInwardsWaternet(
                                                  new[]
                                                  {
                                                      new TestMacroStabilityInwardsPhreaticLine()
                                                  },
                                                  new MacroStabilityInwardsWaternetLine[]
                                                  {
                                                      new TestMacroStabilityInwardsWaternetLine(),
                                                      new TestMacroStabilityInwardsWaternetLine()
                                                  }))
                    .SetName("Other waternet line count");
            }
        }
    }
}