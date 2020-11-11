// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class ExceedanceProbabilityCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;

            // Call
            ExceedanceProbabilityCalculationInput exceedanceProbabilityCalculationInputImplementation =
                new ExceedanceProbabilityCalculationInputImplementation(hydraulicBoundaryLocationId);

            // Assert
            Assert.IsInstanceOf<HydraRingCalculationInput>(exceedanceProbabilityCalculationInputImplementation);
            Assert.AreEqual(1, exceedanceProbabilityCalculationInputImplementation.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, exceedanceProbabilityCalculationInputImplementation.HydraulicBoundaryLocationId);
            CollectionAssert.IsEmpty(exceedanceProbabilityCalculationInputImplementation.Variables);
            CollectionAssert.IsEmpty(exceedanceProbabilityCalculationInputImplementation.ProfilePoints);
            CollectionAssert.IsEmpty(exceedanceProbabilityCalculationInputImplementation.ForelandPoints);
            Assert.IsNull(exceedanceProbabilityCalculationInputImplementation.BreakWater);
            Assert.IsNaN(exceedanceProbabilityCalculationInputImplementation.Beta);
        }

        private class ExceedanceProbabilityCalculationInputImplementation : ExceedanceProbabilityCalculationInput
        {
            public ExceedanceProbabilityCalculationInputImplementation(int hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId) {}

            public override HydraRingFailureMechanismType FailureMechanismType => throw new NotImplementedException();

            public override int VariableId => throw new NotImplementedException();

            public override int FaultTreeModelId => throw new NotImplementedException();

            public override HydraRingSection Section => throw new NotImplementedException();
        }
    }
}