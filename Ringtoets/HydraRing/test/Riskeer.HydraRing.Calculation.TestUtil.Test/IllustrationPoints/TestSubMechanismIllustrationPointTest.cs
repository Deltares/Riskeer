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

using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Riskeer.HydraRing.Calculation.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class TestSubMechanismIllustrationPointTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var illustrationPoint = new TestSubMechanismIllustrationPoint();

            // Assert
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(illustrationPoint);
            Assert.AreEqual("Illustration point", illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(illustrationPoint.Results);
            Assert.AreEqual(1, illustrationPoint.Beta);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Call
            var illustrationPoint = new TestSubMechanismIllustrationPoint("Point name");

            // Assert
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(illustrationPoint);
            Assert.AreEqual("Point name", illustrationPoint.Name);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(illustrationPoint.Results);
            Assert.AreEqual(1, illustrationPoint.Beta);
        }
    }
}