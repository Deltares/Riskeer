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

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class TargetProbabilityCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var norm = 10000;
            var expectedBeta = -Normal.InvCDF(0.0, 1.0, 1.0/norm);
            var targetProbabilityCalculationInputImplementation = new TargetProbabilityCalculationInputImplementation(1, norm);

            // Assert
            Assert.AreEqual(1, targetProbabilityCalculationInputImplementation.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesPiping, targetProbabilityCalculationInputImplementation.FailureMechanismType);
            Assert.AreEqual(2, targetProbabilityCalculationInputImplementation.CalculationTypeId);
            Assert.AreEqual(5, targetProbabilityCalculationInputImplementation.VariableId);
            Assert.AreEqual(1, targetProbabilityCalculationInputImplementation.DikeSection.SectionId);
            CollectionAssert.IsEmpty(targetProbabilityCalculationInputImplementation.Variables);
            CollectionAssert.IsEmpty(targetProbabilityCalculationInputImplementation.ProfilePoints);
            Assert.AreEqual(expectedBeta, targetProbabilityCalculationInputImplementation.Beta);
        }

        private class TargetProbabilityCalculationInputImplementation : TargetProbabilityCalculationInput
        {
            public TargetProbabilityCalculationInputImplementation(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId, beta) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return HydraRingFailureMechanismType.DikesPiping;
                }
            }

            public override int VariableId
            {
                get
                {
                    return 5;
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