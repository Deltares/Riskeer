// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.Test.Data
{
    [TestFixture]
    public class NormalHydraRingVariableTest
    {
        [Test]
        [TestCase(HydraRingDeviationType.Standard, 3.3, 0)]
        [TestCase(HydraRingDeviationType.Variation, null, 3.3)]
        public void Constructor_ExpectedValues(HydraRingDeviationType deviationType, double? expectedParameter2, double expectedCoefficientOfVariation)
        {
            // Call
            var hydraRingVariable = new NormalHydraRingVariable(1, deviationType, 2.2, 3.3);

            // Assert
            Assert.IsInstanceOf<HydraRingVariable>(hydraRingVariable);
            Assert.AreEqual(1, hydraRingVariable.VariableId);
            Assert.AreEqual(HydraRingDistributionType.Normal, hydraRingVariable.DistributionType);
            Assert.AreEqual(deviationType, hydraRingVariable.DeviationType);
            Assert.AreEqual(2.2, hydraRingVariable.Parameter1);
            Assert.AreEqual(expectedParameter2, hydraRingVariable.Parameter2);
            Assert.IsNull(hydraRingVariable.Parameter3);
            Assert.IsNull(hydraRingVariable.Parameter4);
            Assert.AreEqual(expectedCoefficientOfVariation, hydraRingVariable.CoefficientOfVariation);
        }
    }
}