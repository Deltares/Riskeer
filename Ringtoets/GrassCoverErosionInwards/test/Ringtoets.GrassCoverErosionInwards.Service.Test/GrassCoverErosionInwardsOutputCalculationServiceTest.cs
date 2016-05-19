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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputCalculationServiceTest
    {
        [Test]
        public void Calculate_CompleteInput_ReturnsGrassCoverErosionInwardsOutputWithValues()
        {
            // Setup
            const int norm = 30000;
            const double probability = 0.24;
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new GeneralNormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, norm, probability);

            // Assert
            GrassCoverErosionInwardsOutput output = calculation.Output;
            Assert.AreEqual(0.9199, output.FactorOfSafety, output.FactorOfSafety.GetAccuracy());
            Assert.AreEqual(probability, output.Probability, output.Probability.GetAccuracy());
            Assert.AreEqual(4.107, output.Reliability, output.Reliability.GetAccuracy());
            Assert.AreEqual(3.99, output.RequiredProbability, output.RequiredProbability.GetAccuracy());
            Assert.AreEqual(4.465, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
        }
    }
}