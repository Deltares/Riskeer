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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double beta = random.NextDouble();

            // Call
            TestDelegate call = () => new IllustrationPoint(null,
                                                            Enumerable.Empty<SubmechanismIllustrationPointStochast>(),
                                                            Enumerable.Empty<IllustrationPointResult>(),
                                                            beta);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double beta = random.NextDouble();

            // Call
            TestDelegate call = () => new IllustrationPoint("Illustration Point",
                                                            null,
                                                            Enumerable.Empty<IllustrationPointResult>(),
                                                            beta);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        public void Constructor_IlustrationPointResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double beta = random.NextDouble();

            // Call
            TestDelegate call = () => new IllustrationPoint("Illustration Point",
                                                            Enumerable.Empty<SubmechanismIllustrationPointStochast>(),
                                                            null,
                                                            beta);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("illustrationPointResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            const string name = "Illustration Point";
            var random = new Random(21);
            double beta = random.NextDouble();

            var stochasts = new List<SubmechanismIllustrationPointStochast>();
            var illustrationPointResults = new List<IllustrationPointResult>();

            // Call
            var illustrationPoint = new IllustrationPoint(name,
                                                          stochasts,
                                                          illustrationPointResults,
                                                          beta);

            // Assert
            Assert.AreEqual(name, illustrationPoint.Name);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(5, illustrationPoint.Beta.NumberOfDecimalPlaces);
            Assert.AreSame(stochasts, illustrationPoint.Stochasts);
            Assert.AreSame(illustrationPointResults, illustrationPoint.IllustrationPointResults);
        }
    }
}