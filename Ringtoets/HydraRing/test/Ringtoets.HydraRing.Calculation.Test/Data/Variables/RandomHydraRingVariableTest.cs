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
using Ringtoets.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Variables
{
    [TestFixture]
    public class RandomHydraRingVariableTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var variableId = 1;
            var deviationType = HydraRingDeviationType.Variation;
            var distributionType = HydraRingDistributionType.LogNormal;

            // Call
            var variable = new TestRandomHydraRingVariable(variableId, deviationType, distributionType);

            // Assert
            Assert.IsInstanceOf<HydraRingVariable>(variable);
            Assert.AreEqual(distributionType, variable.DistributionType);
        }

        private class TestRandomHydraRingVariable : RandomHydraRingVariable
        {
            public TestRandomHydraRingVariable(int variableId, HydraRingDeviationType deviationType, HydraRingDistributionType distributionType) 
                : base(variableId, deviationType, distributionType) {}
        }
    }
}