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
    public class GeneralResultSubMechanismIllustrationPointTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResultSubMechanismIllustrationPoint(null,
                                                                                     Enumerable.Empty<Stochast>(),
                                                                                     Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("governingWindDirection", paramName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();

            // Call
            TestDelegate call = () => new GeneralResultSubMechanismIllustrationPoint(windDirection,
                                                                                     null,
                                                                                     Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("stochasts", paramName);
        }

        [Test]
        public void Constructor_TopLevelSubMechanismIllustrationPointsNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();

            // Call
            TestDelegate call = () => new GeneralResultSubMechanismIllustrationPoint(windDirection,
                                                                                     Enumerable.Empty<Stochast>(),
                                                                                     null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("topLevelSubMechanismIllustrationPoints", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedProperties()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            IEnumerable<TopLevelSubMechanismIllustrationPoint> combinations =
                Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>();

            // Call
            var generalResult = new GeneralResultSubMechanismIllustrationPoint(windDirection, stochasts, combinations);

            // Assert
            Assert.AreSame(windDirection, generalResult.GoverningWindDirection);
            Assert.AreSame(stochasts, generalResult.Stochasts);
            Assert.AreSame(combinations, generalResult.TopLevelSubMechanismIllustrationPoints);
        }
    }
}