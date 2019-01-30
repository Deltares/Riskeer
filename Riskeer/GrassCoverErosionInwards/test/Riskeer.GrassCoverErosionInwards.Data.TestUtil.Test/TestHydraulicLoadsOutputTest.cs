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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHydraulicLoadsOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double targetProbability = 0.3;
            const double targetReliability = 0.85;
            const double calculatedProbability = 0.2;
            const double calculatedReliability = 0.6;
            const CalculationConvergence calculationConvergence = CalculationConvergence.CalculatedConverged;
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            // Call
            var output = new TestHydraulicLoadsOutput(targetProbability, targetReliability, calculatedProbability, calculatedReliability, calculationConvergence, generalResult);

            // Assert
            Assert.IsInstanceOf<HydraulicLoadsOutput>(output);

            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability);
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability);
            Assert.AreEqual(calculationConvergence, output.CalculationConvergence);
            Assert.IsTrue(output.HasGeneralResult);
            Assert.AreEqual(generalResult, output.GeneralResult);
        }
    }
}