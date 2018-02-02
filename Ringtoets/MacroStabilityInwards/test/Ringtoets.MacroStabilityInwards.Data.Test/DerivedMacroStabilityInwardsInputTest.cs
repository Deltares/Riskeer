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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.CalculatedInput.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class DerivedMacroStabilityInwardsInputTest
    {
        [Test]
        public void Constructor_WithoutMacroStabilityInwardsInput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DerivedMacroStabilityInwardsInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void WaternetExtreme_SoilProfileNull_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = new DerivedMacroStabilityInwardsInput(calculation.InputParameters).WaternetExtreme;

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void WaternetExtreme_SurfaceLineNull_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            calculation.InputParameters.SurfaceLine = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = new DerivedMacroStabilityInwardsInput(calculation.InputParameters).WaternetExtreme;

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void WaternetExtreme_ValidInput_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                // Call
                MacroStabilityInwardsWaternet waternet = new DerivedMacroStabilityInwardsInput(calculation.InputParameters).WaternetExtreme;

                // Assert
                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetCalculator.Output, waternet);
            }
        }

        [Test]
        public void WaternetDaily_SoilProfileNull_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = new DerivedMacroStabilityInwardsInput(calculation.InputParameters).WaternetDaily;

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void WaternetDaily_SurfaceLineNull_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            calculation.InputParameters.SurfaceLine = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternet waternet = new DerivedMacroStabilityInwardsInput(calculation.InputParameters).WaternetDaily;

                // Assert
                Assert.IsNotNull(waternet);
                CollectionAssert.IsEmpty(waternet.PhreaticLines);
                CollectionAssert.IsEmpty(waternet.WaternetLines);
            }
        }

        [Test]
        public void WaternetDaily_ValidInput_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                // Call
                MacroStabilityInwardsWaternet waternet = new DerivedMacroStabilityInwardsInput(calculation.InputParameters).WaternetDaily;

                // Assert
                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetCalculator.Output, waternet);
            }
        }
    }
}