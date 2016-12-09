﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    public class HydraRingVariableTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingVariable = new TestHydraRingVariable(1);

            // Assert
            Assert.AreEqual(1, hydraRingVariable.VariableId);
            Assert.AreEqual(0, hydraRingVariable.Value);
            Assert.AreEqual(0, hydraRingVariable.Parameter1);
            Assert.IsNull(hydraRingVariable.Parameter2);
            Assert.IsNull(hydraRingVariable.Parameter3);
            Assert.IsNull(hydraRingVariable.Parameter4);
            Assert.AreEqual(0, hydraRingVariable.CoefficientOfVariation);
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