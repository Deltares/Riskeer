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
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Test.Data.Variables
{
    [TestFixture]
    public class HydraRingVariableTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingVariable = new TestHydraRingVariable(1);

            // Assert
            Assert.AreEqual(1, hydraRingVariable.VariableId);
            Assert.IsNaN(hydraRingVariable.Value);
            Assert.IsNaN(hydraRingVariable.Parameter1);
            Assert.IsNaN(hydraRingVariable.Parameter2);
            Assert.IsNaN(hydraRingVariable.Parameter3);
            Assert.IsNaN(hydraRingVariable.Parameter4);
            Assert.IsNaN(hydraRingVariable.CoefficientOfVariation);
        }

        private class TestHydraRingVariable : HydraRingVariable
        {
            public TestHydraRingVariable(int variableId)
                : base(variableId) {}

            public override HydraRingDistributionType DistributionType
            {
                get
                {
                    return HydraRingDistributionType.Normal;
                }
            }

            public override HydraRingDeviationType DeviationType
            {
                get
                {
                    return HydraRingDeviationType.Standard;
                }
            }
        }
    }
}