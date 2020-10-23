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
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.KernelWrapper.SubCalculator;
using Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class DerivedSemiProbabilisticPipingInputTest
    {
        [Test]
        public void GetPiezometricHeadExit_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DerivedSemiProbabilisticPipingInput.GetPiezometricHeadExit(null, RoundedDouble.NaN);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("input", paramName);
        }

        [Test]
        public void GetPiezometricHeadExit_ValidInput_SetsParametersForCalculatorAndReturnsNotNaN()
        {
            // Setup
            var input = new TestPipingInput();

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Setup
                var assessmentLevel = (RoundedDouble) 1.1;

                // Call
                RoundedDouble piezometricHead = DerivedSemiProbabilisticPipingInput.GetPiezometricHeadExit(input, assessmentLevel);

                // Assert
                Assert.AreEqual(2, piezometricHead.NumberOfDecimalPlaces);
                Assert.IsFalse(double.IsNaN(piezometricHead));

                var factory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                PiezoHeadCalculatorStub piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(assessmentLevel, piezometricHeadAtExitCalculator.HRiver, assessmentLevel.GetAccuracy());
                Assert.AreEqual(PipingDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                input.PhreaticLevelExit.GetAccuracy());
                Assert.AreEqual(PipingDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                input.DampingFactorExit.GetAccuracy());
            }
        }

        [Test]
        public void GetPiezometricHeadExit_AssessmentLevelNaN_ReturnsNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);

            // Call
            RoundedDouble piezometricHead = DerivedSemiProbabilisticPipingInput.GetPiezometricHeadExit(input, RoundedDouble.NaN);

            // Assert
            Assert.IsNaN(piezometricHead);
        }
    }
}