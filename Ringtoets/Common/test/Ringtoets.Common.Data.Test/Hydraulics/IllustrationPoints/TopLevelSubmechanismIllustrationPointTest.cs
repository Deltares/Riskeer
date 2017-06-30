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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.Hydraulics.IllustrationPoints
{
    [TestFixture]
    public class TopLevelSubmechanismIllustrationPointTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var submechanismIllustrationPoint = new TestSubmechanismIllustrationPoint();

            // Call
            TestDelegate call = () =>
                new TopLevelSubmechanismIllustrationPoint(null, "closing situation", submechanismIllustrationPoint);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsArgumentNullException()
        {
            // Setup
            var windDirection = new TestWindDirection();
            var submechanismIllustrationPoint = new TestSubmechanismIllustrationPoint();

            // Call
            TestDelegate call = () =>
                new TopLevelSubmechanismIllustrationPoint(windDirection, null, submechanismIllustrationPoint);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_SubmechanismIllustationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            var windDirection = new TestWindDirection();

            // Call
            TestDelegate call = () =>
                new TopLevelSubmechanismIllustrationPoint(windDirection, "closing situation", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("submechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            const string closingScenario = "closing scenario";
            var windDirection = new TestWindDirection();
            var submechanismIllustrationPoint = new TestSubmechanismIllustrationPoint();

            // Call
            var windDirectionClosingScenarioIllustrationPoint =
                new TopLevelSubmechanismIllustrationPoint(windDirection, closingScenario, submechanismIllustrationPoint);

            // Assert
            Assert.AreEqual(closingScenario, windDirectionClosingScenarioIllustrationPoint.ClosingSituation);
            Assert.AreSame(windDirection, windDirectionClosingScenarioIllustrationPoint.WindDirection);
            Assert.AreSame(submechanismIllustrationPoint, windDirectionClosingScenarioIllustrationPoint.SubmechanismIllustrationPoint);
        }
    }
}