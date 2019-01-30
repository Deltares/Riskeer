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

using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class HydraRingCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingCalculationInput = new HydraRingCalculationInputImplementation(1);

            // Assert
            Assert.AreEqual(HydraRingFailureMechanismType.QVariant, hydraRingCalculationInput.FailureMechanismType);
            Assert.AreEqual(4, hydraRingCalculationInput.CalculationTypeId);
            Assert.AreEqual(5, hydraRingCalculationInput.VariableId);
            Assert.AreEqual(1, hydraRingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, hydraRingCalculationInput.Section.SectionId);
            CollectionAssert.IsEmpty(hydraRingCalculationInput.Variables);
            CollectionAssert.IsEmpty(hydraRingCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(hydraRingCalculationInput.ForelandsPoints);
            Assert.IsNull(hydraRingCalculationInput.BreakWater);
            Assert.IsNaN(hydraRingCalculationInput.Beta);
            Assert.AreEqual(3, hydraRingCalculationInput.IterationMethodId);
        }

        [Test]
        public void GetSubMechanismModelId_ReturnsExpectedValues()
        {
            // Call
            var hydraRingCalculationInput = new HydraRingCalculationInputImplementation(1);

            // Assert
            Assert.AreEqual(10, hydraRingCalculationInput.GetSubMechanismModelId(1));
            Assert.AreEqual(20, hydraRingCalculationInput.GetSubMechanismModelId(2));
            Assert.IsNull(hydraRingCalculationInput.GetSubMechanismModelId(3));
        }

        private class HydraRingCalculationInputImplementation : HydraRingCalculationInput
        {
            public HydraRingCalculationInputImplementation(int hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return HydraRingFailureMechanismType.QVariant;
                }
            }

            public override int CalculationTypeId
            {
                get
                {
                    return 4;
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

            public override int? GetSubMechanismModelId(int subMechanismId)
            {
                switch (subMechanismId)
                {
                    case 1:
                        return 10;
                    case 2:
                        return 20;
                    default:
                        return null;
                }
            }
        }
    }
}