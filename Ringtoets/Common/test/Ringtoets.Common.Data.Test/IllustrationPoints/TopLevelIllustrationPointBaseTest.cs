// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class TopLevelIllustrationPointBaseTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestTopLevelIllustrationPointBase(null, "closing situation");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            const string windDirectionName = "Name of the wind";
            double windDirectionAngle = random.NextDouble();
            var windDirection = new WindDirection(windDirectionName, windDirectionAngle);

            // Call
            TestDelegate call = () => new TestTopLevelIllustrationPointBase(windDirection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(21);

            const string windDirectionName = "Name of the wind";
            double windDirectionAngle = random.NextDouble();
            var windDirection = new WindDirection(windDirectionName, windDirectionAngle);

            const string closingSituation = "closing situation";

            // Call
            var topLevelIllustrationPoint = new TestTopLevelIllustrationPointBase(windDirection, closingSituation);

            // Assert
            Assert.AreSame(windDirection, topLevelIllustrationPoint.WindDirection);
            Assert.AreEqual(closingSituation, topLevelIllustrationPoint.ClosingSituation);
        }

        private class TestTopLevelIllustrationPointBase : TopLevelIllustrationPointBase
        {
            public TestTopLevelIllustrationPointBase(WindDirection windDirection, string closingSituation)
                : base(windDirection, closingSituation) {}
        }
    }
}