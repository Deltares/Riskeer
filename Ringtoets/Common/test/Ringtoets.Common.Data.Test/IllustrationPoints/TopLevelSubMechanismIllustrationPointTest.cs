﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    public class TopLevelSubMechanismIllustrationPointTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var submechanismIllustrationPoint = new TestSubMechanismIllustrationPoint();

            // Call
            TestDelegate call = () =>
                new TopLevelSubMechanismIllustrationPoint(null, "closing situation", submechanismIllustrationPoint);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var submechanismIllustrationPoint = new TestSubMechanismIllustrationPoint();

            // Call
            TestDelegate call = () =>
                new TopLevelSubMechanismIllustrationPoint(windDirection, null, submechanismIllustrationPoint);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();

            // Call
            TestDelegate call = () =>
                new TopLevelSubMechanismIllustrationPoint(windDirection, "closing situation", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("subMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const string closingScenario = "closing scenario";
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var submechanismIllustrationPoint = new TestSubMechanismIllustrationPoint();

            // Call
            var windDirectionClosingScenarioIllustrationPoint =
                new TopLevelSubMechanismIllustrationPoint(windDirection, closingScenario, submechanismIllustrationPoint);

            // Assert
            Assert.IsInstanceOf<TopLevelIllustrationPointBase>(windDirectionClosingScenarioIllustrationPoint);
            Assert.AreEqual(closingScenario, windDirectionClosingScenarioIllustrationPoint.ClosingSituation);
            Assert.AreSame(windDirection, windDirectionClosingScenarioIllustrationPoint.WindDirection);
            Assert.AreSame(submechanismIllustrationPoint, windDirectionClosingScenarioIllustrationPoint.SubMechanismIllustrationPoint);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                     "Random closing situation",
                                                                     new TestSubMechanismIllustrationPoint());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}