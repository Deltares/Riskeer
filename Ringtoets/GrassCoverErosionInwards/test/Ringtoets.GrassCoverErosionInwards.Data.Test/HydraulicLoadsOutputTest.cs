﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
                                                                   convergence);

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
                                                                   convergence);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("calculatedProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        [TestCase(double.NaN, 0.8457)]
        [TestCase(0.654, double.NaN)]
        public void Constructor_ValidInput_ExpectedProperties(double targetProbability, double calculatedProbability)
        {
            // Setup
            var random = new Random(32);
            double targetReliability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            var output = new TestHydraulicLoadsOutput(targetProbability,
                                                      targetReliability,
                                                      calculatedProbability,
                                                      calculatedReliability,
                                                      convergence);

            // Assert
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.IsFalse(output.HasGeneralResult);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void SetGeneralResult_GeneralResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(32);
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var output = new TestHydraulicLoadsOutput(double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      convergence);
            // Call
            TestDelegate call = () => output.SetGeneralResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("generalResult", exception.ParamName);
        }

        [Test]
        public void SetGeneralResult_ValidGeneralResult_SetExpectedProperties()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();
            var random = new Random(32);
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var output = new TestHydraulicLoadsOutput(double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      convergence);

            // Call
            output.SetGeneralResult(generalResult);

            // Assert
            Assert.AreSame(generalResult, output.GeneralResult);
            Assert.IsTrue(output.HasGeneralResult);
        }

        [Test]
        public void ClearGeneralResult_ValidGeneralResultPresent_ClearsGeneralResult()
        {
            // Setup
            var random = new Random(32);
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var output = new TestHydraulicLoadsOutput(double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      convergence);
            output.SetGeneralResult(generalResult);

            // Call
            output.ClearGeneralResult();

            // Assert
            Assert.IsNull(output.GeneralResult);
            Assert.IsFalse(output.HasGeneralResult);
        }

        [Test]
        public void ClearGeneralResult_NoGeneralResultPresent_ClearsGeneralResult()
        {
            // Setup
            var random = new Random(32);
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var output = new TestHydraulicLoadsOutput(double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      double.NaN,
                                                      convergence);

            // Call
            output.ClearGeneralResult();

            // Assert
            Assert.IsNull(output.GeneralResult);
            Assert.IsFalse(output.HasGeneralResult);
        }

        private class TestHydraulicLoadsOutput : HydraulicLoadsOutput
        {
            public TestHydraulicLoadsOutput(double targetProbability, double targetReliability,
                                            double calculatedProbability, double calculatedReliability, CalculationConvergence calculationConvergence)
                : base(targetProbability, targetReliability, calculatedProbability,
                       calculatedReliability, calculationConvergence) {}
        }
    }
}