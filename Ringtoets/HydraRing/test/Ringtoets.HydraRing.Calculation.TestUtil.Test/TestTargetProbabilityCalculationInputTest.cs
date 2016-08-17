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

using MathNet.Numerics.Distributions;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Test
{
    [TestFixture]
    public class TestTargetProbabilityCalculationInputTest
    {
        [Test]
        [TestCase(2, 10000)]
        [TestCase(-50, 1)]
        [TestCase(0, -90)]
        [TestCase(200000, double.NaN)]
        public void Constructed_UsingDifferentNormAndLocationId_ReturnDifferentBetaAndDefaultValues(int locationId, double norm)
        {
            // Setup
            double expectedBeta = -Normal.InvCDF(0.0, 1.0, 1.0/norm);

            // Call
            var testInput = new TestTargetProbabilityCalculationInput(locationId, norm);

            // Assert
            Assert.IsInstanceOf<TargetProbabilityCalculationInput>(testInput);
            Assert.AreEqual(locationId, testInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesPiping, testInput.FailureMechanismType);
            Assert.AreEqual(2, testInput.CalculationTypeId);
            Assert.AreEqual(5, testInput.VariableId);
            Assert.AreEqual(1, testInput.Section.SectionId);
            CollectionAssert.IsEmpty(testInput.Variables);
            CollectionAssert.IsEmpty(testInput.ProfilePoints);
            CollectionAssert.IsEmpty(testInput.ForelandsPoints);
            Assert.IsNull(testInput.BreakWater);
            Assert.AreEqual(expectedBeta, testInput.Beta);
        }
    }
}