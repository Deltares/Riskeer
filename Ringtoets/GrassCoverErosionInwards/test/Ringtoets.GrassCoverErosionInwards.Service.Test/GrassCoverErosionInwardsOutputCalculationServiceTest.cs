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

using Core.Common.Base.Data;
using NUnit.Framework;
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
            var norm = 30000;
            var probability = 0.24;
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new NormProbabilityGrassCoverErosionInwardsInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, norm, probability);

            // Assert
            RoundedDouble result = calculation.Output.FactorOfSafety;
            Assert.AreEqual(0.919890363, result, 1e-2);
        }
    }
}