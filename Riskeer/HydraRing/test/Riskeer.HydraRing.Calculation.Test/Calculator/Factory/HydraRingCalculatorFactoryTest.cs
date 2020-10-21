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

using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil;

namespace Riskeer.HydraRing.Calculation.Test.Calculator.Factory
{
    [TestFixture]
    public class HydraRingCalculatorFactoryTest
    {
        [Test]
        public void CreateDesignWaterLevelCalculator_Always_ReturnsDesignWaterLevelCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IDesignWaterLevelCalculator calculator = HydraRingCalculatorFactory.Instance.CreateDesignWaterLevelCalculator(settings);

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculator>(calculator);
        }

        [Test]
        public void CreateOvertoppingCalculator_Always_ReturnsOvertoppingCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IOvertoppingCalculator calculator = HydraRingCalculatorFactory.Instance.CreateOvertoppingCalculator(settings);

            // Assert
            Assert.IsInstanceOf<OvertoppingCalculator>(calculator);
        }

        [Test]
        public void CreateDikeHeightCalculator_Always_ReturnsHydraulicLoadsCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IHydraulicLoadsCalculator calculator = HydraRingCalculatorFactory.Instance.CreateDikeHeightCalculator(settings);

            // Assert
            Assert.IsInstanceOf<HydraulicLoadsCalculator>(calculator);
        }

        [Test]
        public void CreateOvertoppingRateCalculator_Always_ReturnsHydraulicLoadsCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IHydraulicLoadsCalculator calculator = HydraRingCalculatorFactory.Instance.CreateOvertoppingRateCalculator(settings);

            // Assert
            Assert.IsInstanceOf<HydraulicLoadsCalculator>(calculator);
        }

        [Test]
        public void CreateWaveConditionsCosineCalculator_Always_ReturnsWaveConditionsCosineCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IWaveConditionsCosineCalculator calculator = HydraRingCalculatorFactory.Instance.CreateWaveConditionsCosineCalculator(settings);

            // Assert
            Assert.IsInstanceOf<WaveConditionsCosineCalculator>(calculator);
        }

        [Test]
        public void CreateWaveHeightCalculator_Always_ReturnsWaveHeightCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IWaveHeightCalculator calculator = HydraRingCalculatorFactory.Instance.CreateWaveHeightCalculator(settings);

            // Assert
            Assert.IsInstanceOf<WaveHeightCalculator>(calculator);
        }

        [Test]
        public void CreateDunesBoundaryConditionsCalculator_Always_ReturnsDunesBoundaryConditionsCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IDunesBoundaryConditionsCalculator calculator = HydraRingCalculatorFactory.Instance.CreateDunesBoundaryConditionsCalculator(settings);

            // Assert
            Assert.IsInstanceOf<DunesBoundaryConditionsCalculator>(calculator);
        }

        [Test]
        public void CreateStructuresCalculator_Always_ReturnsStructuresCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IStructuresCalculator<ExceedanceProbabilityCalculationInput> calculator = HydraRingCalculatorFactory.Instance.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(settings);

            // Assert
            Assert.IsInstanceOf<StructuresCalculator<ExceedanceProbabilityCalculationInput>>(calculator);
        }

        [Test]
        public void CreatePipingCalculator_Always_ReturnsPipingCalculator()
        {
            // Setup
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Call
            IPipingCalculator calculator = HydraRingCalculatorFactory.Instance.CreatePipingCalculator(settings);

            // Assert
            Assert.IsInstanceOf<PipingCalculator>(calculator);
        }
    }
}