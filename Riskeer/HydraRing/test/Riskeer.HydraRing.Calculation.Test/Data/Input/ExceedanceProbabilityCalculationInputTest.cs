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
            const int expectedCalculationTypeId = 1;
            const int hydraulicBoundaryLocationId = 1000;

            // Call
            ExceedanceProbabilityCalculationInput exceedanceProbabilityCalculationInputImplementation =
                new ExceedanceProbabilityCalculationInputImplementation(hydraulicBoundaryLocationId);

            // Assert
            Assert.IsInstanceOf<HydraRingCalculationInput>(exceedanceProbabilityCalculationInputImplementation);
            Assert.AreEqual(expectedCalculationTypeId, exceedanceProbabilityCalculationInputImplementation.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, exceedanceProbabilityCalculationInputImplementation.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesOvertopping, exceedanceProbabilityCalculationInputImplementation.FailureMechanismType);
            Assert.AreEqual(5, exceedanceProbabilityCalculationInputImplementation.VariableId);
            Assert.AreEqual(1, exceedanceProbabilityCalculationInputImplementation.Section.SectionId);
            CollectionAssert.IsEmpty(exceedanceProbabilityCalculationInputImplementation.Variables);
            CollectionAssert.IsEmpty(exceedanceProbabilityCalculationInputImplementation.ProfilePoints);
            CollectionAssert.IsEmpty(exceedanceProbabilityCalculationInputImplementation.ForelandsPoints);
            Assert.IsNull(exceedanceProbabilityCalculationInputImplementation.BreakWater);
            Assert.IsNaN(exceedanceProbabilityCalculationInputImplementation.Beta);
        }

        private class ExceedanceProbabilityCalculationInputImplementation : ExceedanceProbabilityCalculationInput
        {
            public ExceedanceProbabilityCalculationInputImplementation(int hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return HydraRingFailureMechanismType.DikesOvertopping;
                }
            }

            public override int VariableId
            {
                get
                {
                    return 5;
                }
            }

            public override HydraRingSection Section
            {
                get
                {
                    return new HydraRingSection(1, 2.2, 3.3);
                }
            }
        }
    }
}