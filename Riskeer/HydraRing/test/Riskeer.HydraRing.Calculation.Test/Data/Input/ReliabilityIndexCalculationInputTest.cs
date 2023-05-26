﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class ReliabilityIndexCalculationInputTest
    {
        [Test]
        [TestCase(2, 1.0 / 10000)]
        [TestCase(-50, 1.0 / 1)]
        [TestCase(0, 1.0 / -90)]
        [TestCase(200000, double.NaN)]
        public void Constructed_UsingDifferentReturnPeriodAndLocationId_ReturnDifferentBetaAndDefaultValues(int locationId, double targetProbability)
        {
            // Call
            var reliabilityIndexCalculationInput = new ReliabilityIndexCalculationInputImplementation(locationId, targetProbability);

            // Assert
            Assert.IsInstanceOf<HydraRingCalculationInput>(reliabilityIndexCalculationInput);
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(targetProbability);
            Assert.AreEqual(locationId, reliabilityIndexCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(9, reliabilityIndexCalculationInput.CalculationTypeId);
            CollectionAssert.IsEmpty(reliabilityIndexCalculationInput.Variables);
            CollectionAssert.IsEmpty(reliabilityIndexCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(reliabilityIndexCalculationInput.ForelandPoints);
            Assert.IsNull(reliabilityIndexCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, reliabilityIndexCalculationInput.Beta);
        }

        private class ReliabilityIndexCalculationInputImplementation : ReliabilityIndexCalculationInput
        {
            public ReliabilityIndexCalculationInputImplementation(int i, double targetProbability)
                : base(i, targetProbability) {}

            public override HydraRingFailureMechanismType FailureMechanismType => throw new NotImplementedException();

            public override int VariableId => throw new NotImplementedException();

            public override int FaultTreeModelId => throw new NotImplementedException();

            public override HydraRingSection Section => throw new NotImplementedException();
        }
    }
}