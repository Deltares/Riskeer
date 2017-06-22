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

namespace Ringtoets.Common.Data.Test.Hydraulics.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResult(0, null, Enumerable.Empty<Stochast>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("governingWindirection", paramName);
        }

        [Test]
        public void Constructor_StochastNull_ThrowsArgumentNullException()
        {
            // Setup
            var windDirection = new WindDirection("", 0);

            // Call
            TestDelegate call = () => new GeneralResult(0, windDirection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("stochasts", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(12);
            double beta = random.NextDouble();
            var windDirection = new WindDirection("", 0);
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();

            // Call
            var generalResult = new GeneralResult(beta, windDirection, stochasts);

            // Assert
            Assert.AreEqual(beta, generalResult.Beta);
            Assert.AreSame(windDirection, generalResult.GoverningWindirection);
            Assert.AreSame(stochasts, generalResult.Stochasts);
        }
    }
}