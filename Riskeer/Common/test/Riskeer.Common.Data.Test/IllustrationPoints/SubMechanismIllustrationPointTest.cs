﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double beta = random.NextDouble();

            // Call
            void Call() => new SubMechanismIllustrationPoint(null, beta, Enumerable.Empty<SubMechanismIllustrationPointStochast>(), Enumerable.Empty<IllustrationPointResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_StochastsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double beta = random.NextDouble();

            // Call
            void Call() => new SubMechanismIllustrationPoint("Illustration Point", beta, null, Enumerable.Empty<IllustrationPointResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        public void Constructor_IlustrationPointResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double beta = random.NextDouble();

            // Call
            void Call() => new SubMechanismIllustrationPoint("Illustration Point", beta, Enumerable.Empty<SubMechanismIllustrationPointStochast>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("illustrationPointResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            const string name = "Illustration Point";
            var random = new Random(21);
            double beta = random.NextDouble();

            var stochasts = new List<SubMechanismIllustrationPointStochast>();
            var illustrationPointResults = new List<IllustrationPointResult>();

            // Call
            var illustrationPoint = new SubMechanismIllustrationPoint(name,
                                                                      beta,
                                                                      stochasts,
                                                                      illustrationPointResults);

            // Assert
            Assert.IsInstanceOf<IllustrationPointBase>(illustrationPoint);
            Assert.AreEqual(name, illustrationPoint.Name);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreSame(stochasts, illustrationPoint.Stochasts);
            Assert.AreSame(illustrationPointResults, illustrationPoint.IllustrationPointResults);
        }

        [Test]
        public void Constructor_StochastNamesNotUnique_ThrowArgumentException()
        {
            // Setup
            var stochasts = new[]
            {
                new SubMechanismIllustrationPointStochast("unique", "-", 0, 0, 0),
                new SubMechanismIllustrationPointStochast("non-unique", "-", 0, 0, 0),
                new SubMechanismIllustrationPointStochast("non-unique", "-", 0, 0, 0),
                new SubMechanismIllustrationPointStochast("nonunique", "-", 0, 0, 0),
                new SubMechanismIllustrationPointStochast("nonunique", "-", 0, 0, 0)
            };

            // Call
            void Call() => new TestSubMechanismIllustrationPoint(stochasts);

            // Assert
            const string expectedMessage = "Een of meerdere stochasten hebben dezelfde naam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_IllustrationPointResultDescriptionsNotUnique_ThrowArgumentException()
        {
            // Setup
            var random = new Random(21);
            var results = new[]
            {
                new IllustrationPointResult("non-unique", "-", 0),
                new IllustrationPointResult("non-unique", "-", 0),
                new IllustrationPointResult("unique", "-", 0)
            };

            // Call
            void Call() => new SubMechanismIllustrationPoint("Point A", random.NextDouble(), Enumerable.Empty<SubMechanismIllustrationPointStochast>(), results);

            // Assert
            const string expectedMessage = "Een of meerdere uitvoer variabelen hebben dezelfde naam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new SubMechanismIllustrationPoint("Random name",
                                                             random.NextDouble(),
                                                             new[]
                                                             {
                                                                 new SubMechanismIllustrationPointStochast("Random name 1",
                                                                                                           "-",
                                                                                                           random.NextDouble(),
                                                                                                           random.NextDouble(),
                                                                                                           random.NextDouble()),
                                                                 new SubMechanismIllustrationPointStochast("Random name 2",
                                                                                                           "-",
                                                                                                           random.NextDouble(),
                                                                                                           random.NextDouble(),
                                                                                                           random.NextDouble())
                                                             },
                                                             new[]
                                                             {
                                                                 new IllustrationPointResult("Random description 1",
                                                                                             "-",
                                                                                             random.NextDouble()),
                                                                 new IllustrationPointResult("Random description 2",
                                                                                             "-",
                                                                                             random.NextDouble())
                                                             });

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}