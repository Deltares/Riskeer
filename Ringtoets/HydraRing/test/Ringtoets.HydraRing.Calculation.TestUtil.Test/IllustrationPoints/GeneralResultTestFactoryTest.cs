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

using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultTestFactoryTest
    {
        [Test]
        public void CreateGeneralResultWithDuplicateStochasts_ExpectedProperties()
        {
            // Call
            GeneralResult generalResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0.5, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);
            Assert.AreEqual(2, generalResult.Stochasts.Count());
            foreach (Stochast stochast in generalResult.Stochasts)
            {
                Assert.AreEqual("Stochast A", stochast.Name);
            }

            CollectionAssert.IsEmpty(generalResult.IllustrationPoints);
        }

        private static void AssertWindDirection(WindDirection expected, WindDirection actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Angle, actual.Angle);
        }
    }
}