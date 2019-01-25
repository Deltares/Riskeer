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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();

            // Call
            TestDelegate call = () => new WindDirection(null, windDirectionAngle);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ReturnsExpectedValues()
        {
            // Setup
            const string windDirectionName = "SSE";

            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();

            // Call
            var direction = new WindDirection(windDirectionName, windDirectionAngle);

            // Assert
            Assert.AreEqual(windDirectionName, direction.Name);
            Assert.AreEqual(windDirectionAngle, direction.Angle);
        }

        [TestFixture]
        private class WindDirectionEqualsTest : EqualsTestFixture<WindDirection, DerivedWindDirection>
        {
            protected override WindDirection CreateObject()
            {
                return CreateWindDirection();
            }

            protected override DerivedWindDirection CreateDerivedObject()
            {
                return new DerivedWindDirection(CreateWindDirection());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                WindDirection baseDirection = CreateWindDirection();

                var random = new Random(21);
                double offset = random.NextDouble();

                yield return new TestCaseData(new WindDirection("Different Name", baseDirection.Angle))
                    .SetName("Name");
                yield return new TestCaseData(new WindDirection(baseDirection.Name, baseDirection.Angle + offset))
                    .SetName("Angle");
            }

            private static WindDirection CreateWindDirection()
            {
                var random = new Random(21);
                return new WindDirection("Name", random.NextDouble());
            }
        }

        private class DerivedWindDirection : WindDirection
        {
            public DerivedWindDirection(WindDirection windDirection) : base(windDirection.Name, windDirection.Angle) {}
        }
    }
}