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
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FaultTreeIllustrationPoint(null,
                                                                     12.3,
                                                                     Enumerable.Empty<Stochast>(),
                                                                     CombinationType.And);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FaultTreeIllustrationPoint("Test",
                                                                     12.3,
                                                                     null,
                                                                     CombinationType.And);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        [TestCase(CombinationType.And)]
        [TestCase(CombinationType.Or)]
        public void Constructor_ValidArguments_ReturnsExpectedValues(CombinationType combinationType)
        {
            // Setup
            const string name = "Fault tree illustration point name";

            var random = new Random(21);
            double beta = random.NextDouble();

            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();

            // Call
            var illustrationPoint = new FaultTreeIllustrationPoint(name, beta, stochasts, combinationType);

            // Assert
            Assert.IsInstanceOf<IllustrationPointBase>(illustrationPoint);
            Assert.AreEqual(name, illustrationPoint.Name);
            Assert.AreSame(stochasts, illustrationPoint.Stochasts);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(combinationType, illustrationPoint.CombinationType);
        }

        [Test]
        public void Constructor_StochastNamesNotUnique_ThrowArgumentException()
        {
            // Setup
            var random = new Random(21);
            var stochasts = new[]
            {
                new Stochast("unique", random.NextDouble(), random.NextDouble()),
                new Stochast("non-unique", random.NextDouble(), random.NextDouble()),
                new Stochast("non-unique", random.NextDouble(), random.NextDouble()),
                new Stochast("nonunique", random.NextDouble(), random.NextDouble()),
                new Stochast("nonunique", random.NextDouble(), random.NextDouble())
            };

            // Call
            TestDelegate test = () => new FaultTreeIllustrationPoint("Point A",
                                                                     random.NextDouble(),
                                                                     stochasts,
                                                                     CombinationType.And);

            // Assert
            const string expectedMessage = "Een of meerdere stochasten hebben dezelfde naam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new FaultTreeIllustrationPoint("Random name",
                                                          random.NextDouble(),
                                                          new[]
                                                          {
                                                              new Stochast("Random name 1",
                                                                           random.NextDouble(),
                                                                           random.NextDouble()),
                                                              new Stochast("Random name 2",
                                                                           random.NextDouble(),
                                                                           random.NextDouble())
                                                          },
                                                          random.NextEnumValue<CombinationType>());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}