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
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Test
{
    [TestFixture]
    public class TestHydraRingVariableTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int id = 1;
            const HydraRingDistributionType distributionType = HydraRingDistributionType.Normal;
            const HydraRingDeviationType deviationType = HydraRingDeviationType.Standard;
            const double value = 2.2;
            const double param1 = 3.3;
            const double param2 = 4.4;
            const double param3 = 5.5;
            const double param4 = 6.6;
            const double variation = 7.7;

            // Call
            var variable = new TestHydraRingVariable(id, distributionType, deviationType, value, param1, param2, param3, param4, variation);

            // Assert
            Assert.AreEqual(id, variable.VariableId);
            Assert.AreEqual(distributionType, variable.DistributionType);
            Assert.AreEqual(deviationType, variable.DeviationType);
            Assert.AreEqual(value, variable.Value);
            Assert.AreEqual(param1, variable.Parameter1);
            Assert.AreEqual(param2, variable.Parameter2);
            Assert.AreEqual(param3, variable.Parameter3);
            Assert.AreEqual(param4, variable.Parameter4);
            Assert.AreEqual(variation, variable.CoefficientOfVariation);
        }
    }
}