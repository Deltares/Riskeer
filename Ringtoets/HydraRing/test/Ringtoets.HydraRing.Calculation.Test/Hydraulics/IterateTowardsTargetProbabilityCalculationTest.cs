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
using Ringtoets.HydraRing.Calculation.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Test.Hydraulics
{
    [TestFixture]
    public class IterateTowardsTargetProbabilityCalculationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var iterateTowardsTargetProbabilityCalculationImplementation = new IterateTowardsTargetProbabilityCalculationImplementation(1, 2.2);

            // Assert
            Assert.AreEqual(1, iterateTowardsTargetProbabilityCalculationImplementation.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesPiping, iterateTowardsTargetProbabilityCalculationImplementation.FailureMechanismType);
            Assert.AreEqual(2, iterateTowardsTargetProbabilityCalculationImplementation.CalculationTypeId);
            CollectionAssert.IsEmpty(iterateTowardsTargetProbabilityCalculationImplementation.Variables);
            CollectionAssert.IsEmpty(iterateTowardsTargetProbabilityCalculationImplementation.ProfilePoints);
            Assert.AreEqual(2.2, iterateTowardsTargetProbabilityCalculationImplementation.Beta);
        }

        private class IterateTowardsTargetProbabilityCalculationImplementation : IterateTowardsTargetProbabilityCalculation
        {
            public IterateTowardsTargetProbabilityCalculationImplementation(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId, beta) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return HydraRingFailureMechanismType.DikesPiping;
                }
            }

            public override HydraRingDikeSection DikeSection
            {
                get
                {
                    return new HydraRingDikeSection(1, "Name", 2.2, 3.3, 4.4, 5.5, 6.6, 7.7);
                }
            }
        }
    }
}