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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationOutputTest
    {
        [Test]
        [SetCulture("nl-NL")]
        [Combinatorial]
        public void Constructor_InvalidTargetProbability_ThrowsArgumentOutOfRangeException([Values(-0.01, 1.01)] double targetProbability,
                                                                                           [Values(true, false)] bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(32);
            TestGeneralResultSubMechanismIllustrationPoint generalResult = withIllustrationPoints
                                                                               ? new TestGeneralResultSubMechanismIllustrationPoint()
                                                                               : null;

            // Call
            TestDelegate call = () =>
            {
                new HydraulicBoundaryLocationCalculationOutput(random.NextDouble(),
                                                               targetProbability,
                                                               random.NextDouble(),
                                                               random.NextDouble(),
                                                               random.NextDouble(),
                                                               random.NextEnumValue<CalculationConvergence>(),
                                                               generalResult);
            };

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("targetProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        [SetCulture("nl-NL")]
        [Combinatorial]
        public void Constructor_InvalidCalculatedProbability_ThrowsArgumentOutOfRangeException([Values(-0.01, 1.01)] double calculatedProbability,
                                                                                               [Values(true, false)] bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(32);
            TestGeneralResultSubMechanismIllustrationPoint generalResult = withIllustrationPoints
                                                                               ? new TestGeneralResultSubMechanismIllustrationPoint()
                                                                               : null;

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationCalculationOutput(random.NextDouble(),
                                                                                     random.NextDouble(),
                                                                                     random.NextDouble(),
                                                                                     calculatedProbability,
                                                                                     random.NextDouble(),
                                                                                     random.NextEnumValue<CalculationConvergence>(),
                                                                                     generalResult);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("calculatedProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        public void Constructor_ValidInputAndGeneralResultNull_ExpectedProperties()
        {
            // Setup
            var random = new Random(32);
            double result = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            var output = new HydraulicBoundaryLocationCalculationOutput(result,
                                                                        targetProbability,
                                                                        targetReliability,
                                                                        calculatedProbability,
                                                                        calculatedReliability,
                                                                        convergence,
                                                                        null);

            // Assert
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.IsNull(output.GeneralResult);
            Assert.IsFalse(output.HasGeneralResult);
        }

        [Test]
        public void Constructor_ValidInputAndGeneralResultNotNull_ExpectedProperties()
        {
            // Setup
            var random = new Random(32);
            double result = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint();

            // Call
            var output = new HydraulicBoundaryLocationCalculationOutput(result,
                                                                        targetProbability,
                                                                        targetReliability,
                                                                        calculatedProbability,
                                                                        calculatedReliability,
                                                                        convergence,
                                                                        generalResult);

            // Assert
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.AreSame(generalResult, output.GeneralResult);
            Assert.IsTrue(output.HasGeneralResult);
        }
    }
}