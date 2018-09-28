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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class TopLevelIllustrationPointBaseTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestTopLevelIllustrationPointBase(null, "closing situation");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestTopLevelIllustrationPointBase(
                WindDirectionTestFactory.CreateTestWindDirection(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnExpectedValues()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            const string closingSituation = "closing situation";

            // Call
            var topLevelIllustrationPoint = new TestTopLevelIllustrationPointBase(windDirection, closingSituation);

            // Assert
            Assert.IsInstanceOf<ICloneable>(topLevelIllustrationPoint);
            Assert.AreSame(windDirection, topLevelIllustrationPoint.WindDirection);
            Assert.AreEqual(closingSituation, topLevelIllustrationPoint.ClosingSituation);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new TestTopLevelIllustrationPointBase(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                 "Random closing situation");

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }

        private class TestTopLevelIllustrationPointBase : TopLevelIllustrationPointBase
        {
            public TestTopLevelIllustrationPointBase(WindDirection windDirection, string closingSituation)
                : base(windDirection, closingSituation) {}
        }
    }
}