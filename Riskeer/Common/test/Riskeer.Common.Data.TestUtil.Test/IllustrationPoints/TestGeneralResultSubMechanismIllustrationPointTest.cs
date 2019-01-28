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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class TestGeneralResultSubMechanismIllustrationPointTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint();

            // Assert
            Assert.IsInstanceOf<GeneralResult<TopLevelSubMechanismIllustrationPoint>>(generalResult);
            AssertWindDirection(WindDirectionTestFactory.CreateTestWindDirection(), generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
            CollectionAssert.IsEmpty(generalResult.TopLevelIllustrationPoints);
        }

        [Test]
        public void Constructor_TopLevelSubMechanismIllustrationPoint_ExpectedProperties()
        {
            // Setup
            IEnumerable<TopLevelSubMechanismIllustrationPoint> topLevelIllustrationPoints =
                Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>();

            // Call
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);

            // Assert
            Assert.IsInstanceOf<GeneralResult<TopLevelSubMechanismIllustrationPoint>>(generalResult);
            AssertWindDirection(WindDirectionTestFactory.CreateTestWindDirection(), generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
            Assert.AreSame(topLevelIllustrationPoints, generalResult.TopLevelIllustrationPoints);
        }

        private static void AssertWindDirection(WindDirection expected, WindDirection actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Angle, actual.Angle);
        }
    }
}