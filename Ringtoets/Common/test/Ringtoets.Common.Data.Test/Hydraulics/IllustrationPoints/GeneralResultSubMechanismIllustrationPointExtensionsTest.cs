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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.Hydraulics.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultSubMechanismIllustrationPointExtensionsTest
    {
        [Test]
        public void AllClosingSituationsSame_IllustrationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            GeneralResultSubMechanismIllustrationPoint illustrationPoint = null;

            // Call
            TestDelegate test = () => illustrationPoint.AllClosingSituationsSame();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPoint", exception.ParamName);
        }

        [Test]
        public void AllClosingSituationsSame_SituationsSame_ReturnTrue()
        {
            // Setup
            var illustrationPoint = new GeneralResultSubMechanismIllustrationPoint(
                WindDirectionTestFactory.CreateTestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(), "Open",
                        new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                        new TestSubMechanismIllustrationPoint())
                });

            // Call
            bool situationsSame = illustrationPoint.AllClosingSituationsSame();

            // Assert
            Assert.IsTrue(situationsSame);
        }

        [Test]
        public void AllClosingSituationsSame_SituationsNotSame_ReturnFalse()
        {
            // Setup
            var illustrationPoint = new GeneralResultSubMechanismIllustrationPoint(
                WindDirectionTestFactory.CreateTestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                        new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                        new TestSubMechanismIllustrationPoint())
                });

            // Call
            bool situationsSame = illustrationPoint.AllClosingSituationsSame();

            // Assert
            Assert.IsFalse(situationsSame);
        }
    }
}