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

using System;
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocationCalculation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<ICalculatable>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreSame(hydraulicBoundaryLocation, calculation.HydraulicBoundaryLocation);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void HasOutput_WithOrWithoutOutput_ReturnsExpectedValue(bool setOutput)
        {
            // Setup
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            if (setOutput)
            {
                calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(5);
            }

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.AreEqual(setOutput, hasOutput);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculations))]
        public void ShouldCalculate_Always_ReturnsExpectedValue(HydraulicBoundaryLocationCalculation calculation,
                                                                bool expectedShouldCalculate)
        {
            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.AreEqual(expectedShouldCalculate, shouldCalculate);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ClearIllustrationPoints_CalculationWithOutput_ClearsIllustrationPointResult(bool hasIllustrationPoints)
        {
            // Setup
            var random = new Random(21);
            var originalOutput = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(),
                                                                                    hasIllustrationPoints
                                                                                        ? new TestGeneralResultSubMechanismIllustrationPoint()
                                                                                        : null);
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = originalOutput
            };

            // Call
            calculation.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(originalOutput.Result, calculation.Output.Result);
            Assert.AreEqual(originalOutput.CalculatedProbability, calculation.Output.CalculatedProbability);
            Assert.AreEqual(originalOutput.CalculatedReliability, calculation.Output.CalculatedReliability);
            Assert.AreEqual(originalOutput.TargetProbability, calculation.Output.TargetProbability);
            Assert.AreEqual(originalOutput.TargetReliability, calculation.Output.TargetReliability);
            Assert.AreEqual(originalOutput.CalculationConvergence, calculation.Output.CalculationConvergence);

            Assert.IsNull(calculation.Output.GeneralResult);
        }

        [Test]
        public void ClearIllustrationPoints_CalculationWithoutOutput_NothingHappens()
        {
            // Setup
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            TestDelegate call = () => calculation.ClearIllustrationPoints();

            // Assert
            Assert.DoesNotThrow(call);
        }

        private static IEnumerable<TestCaseData> GetCalculations()
        {
            var outputWithoutGeneralResult = new TestHydraulicBoundaryLocationCalculationOutput(1.0, CalculationConvergence.CalculatedConverged);
            var outputWithGeneralResult = new TestHydraulicBoundaryLocationCalculationOutput(1.0, new TestGeneralResultSubMechanismIllustrationPoint());

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = outputWithGeneralResult
                }, false)
                .SetName("OutputSufficientScenario1");

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    Output = outputWithoutGeneralResult
                }, false)
                .SetName("OutputSufficientScenario2");

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()), true)
                .SetName("NoOutputScenario1");

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    }
                }, true)
                .SetName("NoOutputScenario2");

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    Output = outputWithGeneralResult
                }, true)
                .SetName("OutputWithRedundantGeneralResult");

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = outputWithoutGeneralResult
                }, true)
                .SetName("OutputWithMissingGeneralResult");
        }
    }
}