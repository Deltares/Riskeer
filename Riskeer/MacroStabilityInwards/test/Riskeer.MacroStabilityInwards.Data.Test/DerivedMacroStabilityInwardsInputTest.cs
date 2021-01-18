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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.CalculatedInput.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class DerivedMacroStabilityInwardsInputTest
    {
        [Test]
        public void GetWaternetExtreme_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DerivedMacroStabilityInwardsInput.GetWaternetExtreme(null, new GeneralMacroStabilityInwardsInput(), RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }
        
        [Test]
        public void GetWaternetExtreme_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            // Call
            void Call() => DerivedMacroStabilityInwardsInput.GetWaternetExtreme(calculation.InputParameters, null, RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void GetWaternetExtreme_SoilProfileNull_ReturnsMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.StochasticSoilProfile = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = DerivedMacroStabilityInwardsInput.GetWaternetExtreme(calculation.InputParameters, new GeneralMacroStabilityInwardsInput(), RoundedDouble.NaN);

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void GetWaternetExtreme_SurfaceLineNull_ReturnsMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.SurfaceLine = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = DerivedMacroStabilityInwardsInput.GetWaternetExtreme(calculation.InputParameters, new GeneralMacroStabilityInwardsInput(), RoundedDouble.NaN);

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void GetWaternetExtreme_ValidInput_SetsAssessmentLevelToCalculatorInputAndReturnsMacroStabilityInwardsWaternet()
        {
            // Setup
            RoundedDouble assessmentLevel = new Random(21).NextRoundedDouble();
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = DerivedMacroStabilityInwardsInput.GetWaternetExtreme(calculation.InputParameters, new GeneralMacroStabilityInwardsInput(), assessmentLevel);

                // Assert
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                WaternetCalculatorStub calculator = calculatorFactory.LastCreatedWaternetExtremeCalculator;
                Assert.AreEqual(assessmentLevel, calculator.Input.AssessmentLevel);
                CalculatorOutputAssert.AssertWaternet(calculator.Output, waternet);
            }
        }

        [Test]
        public void GetWaternetDaily_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DerivedMacroStabilityInwardsInput.GetWaternetDaily(null, new GeneralMacroStabilityInwardsInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }
        
        [Test]
        public void GetWaternetDaily_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            // Call
            void Call() => DerivedMacroStabilityInwardsInput.GetWaternetDaily(calculation.InputParameters, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void GetWaternetDaily_SoilProfileNull_ReturnsMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.StochasticSoilProfile = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = DerivedMacroStabilityInwardsInput.GetWaternetDaily(calculation.InputParameters, new GeneralMacroStabilityInwardsInput());

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void GetWaternetDaily_SurfaceLineNull_ReturnsMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.SurfaceLine = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = DerivedMacroStabilityInwardsInput.GetWaternetDaily(calculation.InputParameters, new GeneralMacroStabilityInwardsInput());

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void GetWaternetDaily_ValidInput_ReturnsMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = DerivedMacroStabilityInwardsInput.GetWaternetDaily(calculation.InputParameters, new GeneralMacroStabilityInwardsInput());

                // Assert
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetDailyCalculator.Output, waternet);
            }
        }
    }
}