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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.Hydraulics.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResult(null,
                                                        Enumerable.Empty<Stochast>(),
                                                        Enumerable.Empty<WindDirectionClosingSituationIllustrationPoint>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("governingWindDirection", paramName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Setup
            var windDirection = new TestWindDirection();

            // Call
            TestDelegate call = () => new GeneralResult(windDirection,
                                                        null,
                                                        Enumerable.Empty<WindDirectionClosingSituationIllustrationPoint>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("stochasts", paramName);
        }

        [Test]
        public void Constructor_WindDirectionClosingScenarioIllustrationPointsNull_ThrowsArgumentNullException()
        {
            // Setup
            var windDirection = new TestWindDirection();

            // Call
            TestDelegate call = () => new GeneralResult(windDirection,
                                                        Enumerable.Empty<Stochast>(),
                                                        null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("windDirectionClosingSituationIllustrationPoints", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedProperties()
        {
            // Setup
            var windDirection = new TestWindDirection();
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            IEnumerable<WindDirectionClosingSituationIllustrationPoint> combinations =
                Enumerable.Empty<WindDirectionClosingSituationIllustrationPoint>();

            // Call
            var generalResult = new GeneralResult(windDirection, stochasts, combinations);

            // Assert
            Assert.AreSame(windDirection, generalResult.GoverningWindDirection);
            Assert.AreSame(stochasts, generalResult.Stochasts);
            Assert.AreSame(combinations, generalResult.WindDirectionClosingSituationIllustrationPoints);
        }
    }
}