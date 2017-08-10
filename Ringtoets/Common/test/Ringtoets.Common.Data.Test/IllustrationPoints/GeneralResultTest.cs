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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using CoreCloneAssert = Core.Common.Data.TestUtil.CloneAssert;
using CommonCloneAssert = Ringtoets.Common.Data.TestUtil.CloneAssert;

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
                                                                                new TestTopLevelIllustrationPoint(),
                                                                                new TestTopLevelIllustrationPoint()
                                                                            });

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}