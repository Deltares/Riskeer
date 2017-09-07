// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressTest
    {
        [Test]
        public void Constructor_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);

            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double stressCalculationValue = random.NextDouble();
            double stressShift = random.NextDouble();
            double stressMean = random.NextDouble();
            double stressCoefficientOfVariation = random.NextDouble();

            // Call
            var stress = new MacroStabilityInwardsPreconsolidationStress(xCoordinate,
                                                                         zCoordinate,
                                                                         stressCalculationValue,
                                                                         stressMean,
                                                                         stressCoefficientOfVariation,
                                                                         stressShift);
            // Assert
            Assert.AreEqual(xCoordinate, stress.XCoordinate);
            Assert.AreEqual(zCoordinate, stress.ZCoordinate);

            Assert.AreEqual(stressCalculationValue, stress.PreconsolidationStressCalculationValue);
            Assert.AreEqual(stressMean, stress.PreconsolidationStressMean);
            Assert.AreEqual(stressCoefficientOfVariation, stress.PreconsolidationStressCoefficientOfVariation);
            Assert.AreEqual(stressShift, stress.PreconsolidationStressShift);
        }
    }
}