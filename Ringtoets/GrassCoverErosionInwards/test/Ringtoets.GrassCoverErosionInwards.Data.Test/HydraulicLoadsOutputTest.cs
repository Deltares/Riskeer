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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class HydraulicLoadsOutputTest
    {
        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-0.01)]
        [TestCase(1.01)]
        public void Constructor_InvalidTargetProbability_ThrowsArgumentOutOfRangeException(double targetProbability)
        {
            // Setup
            var random = new Random(32);
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            TestDelegate call = () => new TestHydraulicLoadsOutput(targetProbability,
                                                                   targetReliability,
                                                                   calculatedProbability,
                                                                   calculatedReliability,
                                                                   convergence,
                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("targetProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-0.01)]
        [TestCase(1.01)]
        public void Constructor_InvalidCalculatedProbability_ThrowsArgumentOutOfRangeException(double calculatedProbability)
        {
            // Setup
            var random = new Random(32);
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            TestDelegate call = () => new TestHydraulicLoadsOutput(targetProbability,
                                                                   targetReliability,
                                                                   calculatedProbability,
                                                                   calculatedReliability,
                                                                   convergence,
                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("calculatedProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        [TestCase(double.NaN, 0.8457, true)]
        [TestCase(0.654, double.NaN, false)]
        public void Constructor_ValidInputAndGeneralResult_ReturnsExpectedProperties(
            double targetProbability,
            double calculatedProbability,
            bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(32);
            double targetReliability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = withIllustrationPoints
                                                                                  ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                                  : null;

            // Call
            var output = new TestHydraulicLoadsOutput(targetProbability,
                                                      targetReliability,
                                                      calculatedProbability,
                                                      calculatedReliability,
                                                      convergence,
                                                      generalResult);

            // Assert
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.AreEqual(withIllustrationPoints, output.HasGeneralResult);
            Assert.AreSame(generalResult, output.GeneralResult);
        }

        private class TestHydraulicLoadsOutput : HydraulicLoadsOutput
        {
            public TestHydraulicLoadsOutput(double targetProbability, double targetReliability,
                                            double calculatedProbability, double calculatedReliability, CalculationConvergence calculationConvergence,
                                            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
                : base(targetProbability, targetReliability, calculatedProbability,
                       calculatedReliability, calculationConvergence, generalResult) {}
        }
    }
}