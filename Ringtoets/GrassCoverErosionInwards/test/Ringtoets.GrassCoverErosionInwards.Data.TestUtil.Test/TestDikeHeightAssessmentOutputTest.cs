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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestDikeHeightAssessmentOutputTest
    {
         [Test]
        public void TestHydraulicBoundaryLocationOutput_WithoutConvergence_ReturnsExpectedValues()
        {
            // Setup
            const double result = 9.0;

            // Call
            DikeHeightAssessmentOutput output = new TestDikeHeightAssessmentOutput(result);

            // Assert
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
        }

        [Test]
        public void TestHydraulicBoundaryLocationOutput_WithConvergence_ReturnsExpectedValues(
            [Values(CalculationConvergence.CalculatedConverged, CalculationConvergence.NotCalculated,
                CalculationConvergence.CalculatedNotConverged)] CalculationConvergence convergence)
        {
            // Setup
            const double result = 9.5;

            // Call
            DikeHeightAssessmentOutput output = new TestDikeHeightAssessmentOutput(result, convergence);

            // Assert
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}