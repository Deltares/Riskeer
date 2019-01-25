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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(123);

            // Call
            TestDelegate call = () => new FaultTreeIllustrationPoint(null,
                                                                     random.NextDouble(),
                                                                     Enumerable.Empty<Stochast>(),
                                                                     random.NextEnumValue<CombinationType>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(123);

            // Call
            TestDelegate call = () => new FaultTreeIllustrationPoint("name",
                                                                     random.NextDouble(),
                                                                     null,
                                                                     random.NextEnumValue<CombinationType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidParameter_ReturnsNewInstance()
        {
            // Setup
            var random = new Random(123);
            const string name = "name";
            double beta = random.NextDouble();
            var combinationType = random.NextEnumValue<CombinationType>();
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();

            // Call
            var illustrationPoint = new FaultTreeIllustrationPoint(name, beta, stochasts, combinationType);

            // Assert
            Assert.IsInstanceOf<IIllustrationPoint>(illustrationPoint);
            Assert.AreEqual(name, illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            Assert.AreEqual(beta, illustrationPoint.Beta);
            Assert.AreEqual(combinationType, illustrationPoint.CombinationType);
            Assert.AreSame(stochasts, illustrationPoint.Stochasts);
        }
    }
}