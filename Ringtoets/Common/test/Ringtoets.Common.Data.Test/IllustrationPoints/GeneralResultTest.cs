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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultTest
    {
        [Test]
        public void Constructor_GoverningWindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                new GeneralResult<TopLevelIllustrationPointBase>(null,
                                                                 Enumerable.Empty<Stochast>(),
                                                                 Enumerable.Empty<TopLevelIllustrationPointBase>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("governingWindDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_StochastNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();

            // Call
            TestDelegate call = () =>
                new GeneralResult<TopLevelIllustrationPointBase>(windDirection,
                                                                 null,
                                                                 Enumerable.Empty<TopLevelIllustrationPointBase>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasts", exception.ParamName);
        }

        [Test]
        public void Constructor_TopLevelIllustrationPointsNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();

            // Call
            TestDelegate call = () =>
                new GeneralResult<TopLevelIllustrationPointBase>(windDirection,
                                                                 Enumerable.Empty<Stochast>(),
                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLevelIllustrationPoints", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            IEnumerable<TopLevelIllustrationPointBase> topLevelIllustrationPoints =
                Enumerable.Empty<TopLevelIllustrationPointBase>();

            // Call
            var generalResult = new GeneralResult<TopLevelIllustrationPointBase>(windDirection,
                                                                                 stochasts,
                                                                                 topLevelIllustrationPoints);

            // Assert
            Assert.IsInstanceOf<ICloneable>(generalResult);
            Assert.AreSame(windDirection, generalResult.GoverningWindDirection);
            Assert.AreSame(topLevelIllustrationPoints, generalResult.TopLevelIllustrationPoints);
            Assert.AreSame(stochasts, generalResult.Stochasts);
        }

        [Test]
        public void Constructor_StochastNotUnique_ThrowArgumentException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var stochasts = new[]
            {
                new Stochast("unique", 0, 0),
                new Stochast("non-unique", 0, 0),
                new Stochast("non-unique", 0, 0),
                new Stochast("nonunique", 0, 0),
                new Stochast("nonunique", 0, 0)
            };
            IEnumerable<TopLevelIllustrationPointBase> topLevelIllustrationPoints =
                Enumerable.Empty<TopLevelIllustrationPointBase>();

            // Call
            TestDelegate test = () => new GeneralResult<TopLevelIllustrationPointBase>(windDirection,
                                                                                       stochasts,
                                                                                       topLevelIllustrationPoints);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("Een of meerdere stochasten hebben dezelfde naam. " +
                            "Het uitlezen van illustratiepunten wordt overgeslagen.",
                            exception.Message);
        }

        [Test]
        public void Constructor_TopLevelIllustrationPointsNotUnique_ThrowArgumentException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            IEnumerable<Stochast> stochasts = Enumerable.Empty<Stochast>();
            IEnumerable<TopLevelIllustrationPointBase> topLevelIllustrationPoints = new[]
            {
                new TopLevelIllustrationPoint(new WindDirection("N", 0.5), "not unique"),
                new TopLevelIllustrationPoint(new WindDirection("N", 0.5), "not unique")
            };

            // Call
            TestDelegate test = () => new GeneralResult<TopLevelIllustrationPointBase>(windDirection,
                                                                                       stochasts,
                                                                                       topLevelIllustrationPoints);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("Een of meerdere illustratiepunten hebben dezelfde combinatie van sluitscenario en windrichting. " +
                            "Het uitlezen van illustratiepunten wordt overgeslagen.",
                            exception.Message);
        }

        [Test]
        public void Constructor_StochastsNotUnique_ThrowArgumentException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            IEnumerable<Stochast> stochasts = new[]
            {
                new Stochast("Stochast 1", 0, 0),
                new Stochast("Stochast 1", 0, 0)
            };
            IEnumerable<TopLevelIllustrationPointBase> topLevelIllustrationPoints = new List<TopLevelIllustrationPointBase>();

            // Call
            TestDelegate test = () => new GeneralResult<TopLevelIllustrationPointBase>(windDirection,
                                                                                       stochasts,
                                                                                       topLevelIllustrationPoints);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("Een of meerdere stochasten hebben dezelfde naam. " +
                            "Het uitlezen van illustratiepunten wordt overgeslagen.",
                            exception.Message);
        }

        [Test]
        public void Constructor_ChildStochastsNotInStochasts_ThrowArgumentException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            IEnumerable<Stochast> stochasts = new[]
            {
                new Stochast("Stochast 1", 0, 0)
            };
            var illustrationPointNode = new IllustrationPointNode(new FaultTreeIllustrationPoint("A", 0.0, new []
            {
                new Stochast("Stochast 2", 0, 0)
            }, CombinationType.And));

            IEnumerable<TopLevelIllustrationPointBase> topLevelIllustrationPoints = new List<TopLevelIllustrationPointBase>
            {
                new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(), "closing", illustrationPointNode)
            };

            // Call
            TestDelegate test = () => new GeneralResult<TopLevelIllustrationPointBase>(windDirection,
                                                                                       stochasts,
                                                                                       topLevelIllustrationPoints);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De stochasten van een illustratiepunt bevatten niet dezelfde stochasten als in de onderliggende illustratiepunten. " +
                            "Het uitlezen van illustratiepunten wordt overgeslagen.",
                            exception.Message);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new GeneralResult<TopLevelIllustrationPointBase>(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                            new[]
                                                                            {
                                                                                new Stochast("Random name 1",
                                                                                             random.NextDouble(),
                                                                                             random.NextDouble()),
                                                                                new Stochast("Random name 2",
                                                                                             random.NextDouble(),
                                                                                             random.NextDouble())
                                                                            },
                                                                            new[]
                                                                            {
                                                                                new TestTopLevelIllustrationPoint("situation 1"),
                                                                                new TestTopLevelIllustrationPoint("situation 2")
                                                                            });

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }

        private class TopLevelIllustrationPoint : TopLevelIllustrationPointBase
        {
            public TopLevelIllustrationPoint(WindDirection windDirection, string closingSituation)
                : base(windDirection, closingSituation) {}
        }
    }
}