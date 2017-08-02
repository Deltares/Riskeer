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

using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHydraulicBoundaryLocationOutputTest
    {
        [Test]
        public void TestHydraulicBoundaryLocationOutput_WithoutConvergence_ReturnsExpectedValues()
        {
            // Setup
            const double result = 9.0;

            // Call
            var output = new TestHydraulicBoundaryLocationOutput(result);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationOutput>(output);
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.NotCalculated)]
        [TestCase(CalculationConvergence.CalculatedNotConverged)]
        public void TestHydraulicBoundaryLocationOutput_WithConvergence_ReturnsExpectedValues(CalculationConvergence convergence)
        {
            // Setup
            const double result = 9.5;

            // Call
            var output = new TestHydraulicBoundaryLocationOutput(result, convergence);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationOutput>(output);
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void TestHydraulicBoundaryLocationOutput_WithResultAndWithoutGeneralResult_ReturnsExpectedValues()
        {
            // Setup
            const double result = 9.5;

            // Call
            var output = new TestHydraulicBoundaryLocationOutput(result, null);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationOutput>(output);
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.IsNull(output.GeneralResult);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged,
                            output.CalculationConvergence);
        }

        [Test]
        public void TestHydraulicBoundaryLocationOutput_WithGeneralResultAndResult_ReturnsExpectedValues()
        {
            // Setup
            const double result = 9.5;
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint();

            // Call
            var output = new TestHydraulicBoundaryLocationOutput(result, generalResult);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationOutput>(output);
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.AreSame(generalResult, output.GeneralResult);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged,
                            output.CalculationConvergence);
        }

        [Test]
        public void TestHydraulicBoundaryLocationOutput_WithGeneralResultNull_ReturnsExpectedValues()
        {
            // Call
            var output = new TestHydraulicBoundaryLocationOutput(null);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationOutput>(output);
            Assert.IsNull(output.GeneralResult);
            Assert.IsNaN(output.Result);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged,
                            output.CalculationConvergence);
        }

        [Test]
        public void TestHydraulicBoundaryLocationOutput_WithGeneralResult_ReturnsExpectedValues()
        {
            // Setup
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint();

            // Call
            var output = new TestHydraulicBoundaryLocationOutput(generalResult);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationOutput>(output);
            Assert.AreSame(output.GeneralResult, generalResult);
            Assert.IsNaN(output.Result);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged,
                            output.CalculationConvergence);
        }
    }
}