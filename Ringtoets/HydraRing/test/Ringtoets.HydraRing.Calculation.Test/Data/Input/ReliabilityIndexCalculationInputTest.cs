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

using System;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class ReliabilityIndexCalculationInputTest
    {
        [Test]
        [TestCase(2, 1.0 / 10000)]
        [TestCase(-50, 1.0 / 1)]
        [TestCase(0, 1.0 / -90)]
        [TestCase(200000, double.NaN)]
        public void Constructed_UsingDifferentReturnPeriodAndLocationId_ReturnDifferentBetaAndDefaultValues(int locationId, double norm)
        {
            // Call
            var reliabilityIndexCalculationInput = new SimpleReliabilityIndexCalculationInput(locationId, norm);

            // Assert
            Assert.IsInstanceOf<HydraRingCalculationInput>(reliabilityIndexCalculationInput);
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.AreEqual(locationId, reliabilityIndexCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(9, reliabilityIndexCalculationInput.CalculationTypeId);
            CollectionAssert.IsEmpty(reliabilityIndexCalculationInput.Variables);
            CollectionAssert.IsEmpty(reliabilityIndexCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(reliabilityIndexCalculationInput.ForelandsPoints);
            Assert.IsNull(reliabilityIndexCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, reliabilityIndexCalculationInput.Beta);
        }

        private class SimpleReliabilityIndexCalculationInput : ReliabilityIndexCalculationInput
        {
            public SimpleReliabilityIndexCalculationInput(int i, double norm)
                : base(i, norm) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override int VariableId
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override HydraRingSection Section
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}