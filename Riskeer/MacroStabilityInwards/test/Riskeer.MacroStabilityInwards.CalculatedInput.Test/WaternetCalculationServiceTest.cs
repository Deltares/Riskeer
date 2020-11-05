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
using System.Collections;
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.CalculatedInput.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Test
{
    [TestFixture]
    public class WaternetCalculationServiceTest
    {
        private MacroStabilityInwardsCalculationScenario testCalculation;

        [SetUp]
        public void Setup()
        {
            testCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
        }

        [Test]
        public void ValidateExtreme_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaternetCalculationService.ValidateExtreme(null, RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void ValidateExtreme_WithInput_SetsInputOnCalculator()
        {
            // Setup
            RoundedDouble assessmentLevel = new Random(21).NextRoundedDouble();
            MacroStabilityInwardsInput input = testCalculation.InputParameters;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                WaternetCalculationService.ValidateExtreme(input, assessmentLevel);

                // Assert
                var factory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                WaternetCalculatorInput actualInput = factory.LastCreatedWaternetExtremeCalculator.Input;

                CalculatorInputAssert.AssertExtremeInput(input, actualInput, assessmentLevel);
            }
        }

        [Test]
        public void ValidateExtreme_MessagesInValidation_ReturnValidationMessages()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                WaternetCalculatorStub calculator = calculatorFactory.LastCreatedWaternetExtremeCalculator;
                calculator.ReturnValidationError = true;
                calculator.ReturnValidationWarning = true;

                // Call
                IEnumerable<MacroStabilityInwardsKernelMessage> messages = WaternetCalculationService.ValidateExtreme(testCalculation.InputParameters, RoundedDouble.NaN);

                // Assert
                CollectionAssert.AreEqual(calculator.Validate(), messages, new MacroStabilityInwardsKernelMessageComparer());
            }
        }

        [Test]
        public void ValidateExtreme_NoMessagesInValidation_ReturnEmptyCollection()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<MacroStabilityInwardsKernelMessage> messages = WaternetCalculationService.ValidateExtreme(testCalculation.InputParameters, RoundedDouble.NaN);

                // Assert
                CollectionAssert.IsEmpty(messages);
            }
        }

        [Test]
        public void ValidateExtreme_ErrorInValidation_ThrowsWaternetCalculationException()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculatorFactory.LastCreatedWaternetExtremeCalculator.ThrowExceptionOnValidate = true;

                // Call
                void Call() => WaternetCalculationService.ValidateExtreme(testCalculation.InputParameters, RoundedDouble.NaN);

                // Assert
                var exception = Assert.Throws<WaternetCalculationException>(Call);
                Assert.IsInstanceOf<WaternetCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void ValidateDaily_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaternetCalculationService.ValidateDaily(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void ValidateDaily_WithInput_SetsInputOnCalculator()
        {
            // Setup
            MacroStabilityInwardsInput input = testCalculation.InputParameters;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                WaternetCalculationService.ValidateDaily(input);

                // Assert
                var factory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                WaternetCalculatorInput actualInput = factory.LastCreatedWaternetDailyCalculator.Input;

                CalculatorInputAssert.AssertDailyInput(input, actualInput);
            }
        }

        [Test]
        public void ValidateDaily_MessagesInValidation_ReturnValidationMessages()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                WaternetCalculatorStub calculator = calculatorFactory.LastCreatedWaternetDailyCalculator;
                calculator.ReturnValidationError = true;
                calculator.ReturnValidationWarning = true;

                // Call
                IEnumerable<MacroStabilityInwardsKernelMessage> messages = WaternetCalculationService.ValidateDaily(testCalculation.InputParameters);

                // Assert
                CollectionAssert.AreEqual(calculator.Validate(), messages, new MacroStabilityInwardsKernelMessageComparer());
            }
        }

        [Test]
        public void ValidateDaily_NoMessagesInValidation_ReturnEmptyCollection()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<MacroStabilityInwardsKernelMessage> messages = WaternetCalculationService.ValidateDaily(testCalculation.InputParameters);

                // Assert
                CollectionAssert.IsEmpty(messages);
            }
        }

        [Test]
        public void ValidateDaily_ErrorInValidation_ThrowsWaternetCalculationException()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculatorFactory.LastCreatedWaternetDailyCalculator.ThrowExceptionOnValidate = true;

                // Call
                void Call() => WaternetCalculationService.ValidateDaily(testCalculation.InputParameters);

                // Assert
                var exception = Assert.Throws<WaternetCalculationException>(Call);
                Assert.IsInstanceOf<WaternetCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CalculateExtreme_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaternetCalculationService.CalculateExtreme(null, RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void CalculateExtreme_WithInput_SetsInputOnCalculator()
        {
            // Setup
            RoundedDouble assessmentLevel = new Random(21).NextRoundedDouble();
            MacroStabilityInwardsInput input = testCalculation.InputParameters;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                WaternetCalculationService.CalculateExtreme(input, assessmentLevel);

                // Assert
                var factory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                WaternetCalculatorInput actualInput = factory.LastCreatedWaternetExtremeCalculator.Input;

                CalculatorInputAssert.AssertExtremeInput(input, actualInput, assessmentLevel);
            }
        }

        [Test]
        public void CalculateExtreme_CalculationRan_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                // Call
                MacroStabilityInwardsWaternet output = WaternetCalculationService.CalculateExtreme(testCalculation.InputParameters, RoundedDouble.NaN);

                // Assert
                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetExtremeCalculator.Output, output);
            }
        }

        [Test]
        public void CalculateExtreme_ErrorInCalculation_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculatorFactory.LastCreatedWaternetExtremeCalculator.ThrowExceptionOnCalculate = true;

                // Call
                MacroStabilityInwardsWaternet output = WaternetCalculationService.CalculateExtreme(testCalculation.InputParameters, RoundedDouble.NaN);

                // Assert
                Assert.IsNotNull(output);
                CollectionAssert.IsEmpty(output.PhreaticLines);
                CollectionAssert.IsEmpty(output.WaternetLines);
            }
        }

        [Test]
        public void CalculateDaily_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaternetCalculationService.CalculateDaily(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void CalculateDaily_WithInput_SetsInputOnCalculator()
        {
            // Setup
            MacroStabilityInwardsInput input = testCalculation.InputParameters;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                WaternetCalculationService.CalculateDaily(input);

                // Assert
                var factory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                WaternetCalculatorInput actualInput = factory.LastCreatedWaternetDailyCalculator.Input;

                CalculatorInputAssert.AssertDailyInput(input, actualInput);
            }
        }

        [Test]
        public void CalculateDaily_CalculationRan_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                // Call
                MacroStabilityInwardsWaternet output = WaternetCalculationService.CalculateDaily(testCalculation.InputParameters);

                // Assert
                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetDailyCalculator.Output, output);
            }
        }

        [Test]
        public void CalculateDaily_ErrorInCalculation_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;
                calculatorFactory.LastCreatedWaternetDailyCalculator.ThrowExceptionOnCalculate = true;

                // Call
                MacroStabilityInwardsWaternet output = WaternetCalculationService.CalculateDaily(testCalculation.InputParameters);

                // Assert
                Assert.IsNotNull(output);
                CollectionAssert.IsEmpty(output.PhreaticLines);
                CollectionAssert.IsEmpty(output.WaternetLines);
            }
        }

        private class MacroStabilityInwardsKernelMessageComparer : IComparer<MacroStabilityInwardsKernelMessage>, IComparer
        {
            public int Compare(object x, object y)
            {
                return Compare((MacroStabilityInwardsKernelMessage) x, (MacroStabilityInwardsKernelMessage) y);
            }

            public int Compare(MacroStabilityInwardsKernelMessage x, MacroStabilityInwardsKernelMessage y)
            {
                return x.Message == y.Message && x.Type == y.Type ? 0 : 1;
            }
        }
    }
}