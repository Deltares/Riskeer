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
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Riskeer.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SubMechanismIllustrationPoint(null,
                                                                        Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                        Enumerable.Empty<IllustrationPointResult>(),
                                                                        123);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_StochastNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SubMechanismIllustrationPoint("Name", null, Enumerable.Empty<IllustrationPointResult>(), 123);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        public void Constructor_IllustrationPointResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SubMechanismIllustrationPoint("Name", Enumerable.Empty<SubMechanismIllustrationPointStochast>(), null, 123);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("illustrationPointResults", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ReturnsNewInstance()
        {
            // Setup
            const string name = "Name";

            var random = new Random(21);
            double beta = random.NextDouble();

            IEnumerable<SubMechanismIllustrationPointStochast> stochasts = Enumerable.Empty<SubMechanismIllustrationPointStochast>();
            IEnumerable<IllustrationPointResult> illustrationPointResults = Enumerable.Empty<IllustrationPointResult>();

            // Call
            var illustrationPoint = new SubMechanismIllustrationPoint(name, stochasts, illustrationPointResults, beta);

            // Assert
            Assert.IsInstanceOf<IIllustrationPoint>(illustrationPoint);
            Assert.AreSame(stochasts, illustrationPoint.Stochasts);
            Assert.AreSame(illustrationPointResults, illustrationPoint.Results);
            Assert.AreEqual(beta, illustrationPoint.Beta);
        }
    }
}