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
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Riskeer.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    public class GeneralResultTest
    {
        [Test]
        public void Constructor_GoverningWindNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResult(0,
                                                        null,
                                                        Enumerable.Empty<Stochast>(),
                                                        new Dictionary<
                                                            WindDirectionClosingSituation,
                                                            IllustrationPointTreeNode>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("governingWindDirection", paramName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResult(0,
                                                        new TestWindDirection(),
                                                        null,
                                                        new Dictionary<
                                                            WindDirectionClosingSituation,
                                                            IllustrationPointTreeNode>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("stochasts", paramName);
        }

        [Test]
        public void Constructor_IllustrationPointsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResult(0,
                                                        new TestWindDirection(),
                                                        Enumerable.Empty<Stochast>(),
                                                        null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("illustrationPoints", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsNewInstance()
        {
            // Setup
            var random = new Random(123);
            double beta = random.NextDouble();
            var governingWind = new TestWindDirection();
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();

            // Call
            var result = new GeneralResult(beta, governingWind, stochasts, illustrationPoints);

            // Assert
            Assert.AreEqual(beta, result.Beta);
            Assert.AreSame(governingWind, result.GoverningWindDirection);
            Assert.AreSame(stochasts, result.Stochasts);
            Assert.AreSame(illustrationPoints, result.IllustrationPoints);
        }
    }
}