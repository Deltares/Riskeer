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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Integration.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsTestDataGeneratorTest
    {
        [Test]
        public void GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Call
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();

            // Assert
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);
        }

        [Test]
        public void ConfigureFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateFullyCalculated();

            // Call
            MacroStabilityInwardsTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);

            // Assert
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);

            AssertCalculationsHasSameHydraulicBoundaryLocation(failureMechanism.CalculationsGroup, hydraulicBoundaryLocation);

            CalculationGroup nestedCalculationGroup = failureMechanism.CalculationsGroup.Children.OfType<CalculationGroup>().First();
            AssertCalculationsHasSameHydraulicBoundaryLocation(nestedCalculationGroup, hydraulicBoundaryLocation);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculation_Always_ReturnCalculationWithDataSet()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculation();

            // Assert
            AssertCalculation(calculation);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationWithoutHydraulicLocationAndAssessmentLevel_Always_ReturnCalculationWithoutHydraulicLocationAndAssessmentLevel()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutHydraulicLocationAndAssessmentLevel();

            // Assert
            AssertCalculation(calculation, false);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationWithAssessmentLevel_Always_ReturnCalculationWithAssessmentLevel()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithAssessmentLevel();

            // Assert
            AssertCalculation(calculation, false, true);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationWithoutSurfaceLine_Always_ReturnCalculationWithoutSurfaceLine()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutSurfaceLine();

            // Assert
            AssertCalculation(calculation, true, false, false);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationWithoutSoilModel_Always_ReturnCalculationWithoutSoilModel()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutSoilModel();

            // Assert
            AssertCalculation(calculation, true, false, true, false);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationWithoutSoilProfile_Always_ReturnCalculationWithoutSoilProfile()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutSoilProfile();

            // Assert
            AssertCalculation(calculation, true, false, true, true, false);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationWithNaNs_Always_ReturnCalculationWithNaNs()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithNaNs();

            // Assert
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);
            Assert.AreEqual(double.NaN, calculation.InputParameters.AssessmentLevel.Value);
            Assert.AreEqual("PK001_0001", calculation.InputParameters.SurfaceLine.Name);
            Assert.AreEqual("PK001_0001_Macrostabiliteit", calculation.InputParameters.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", calculation.InputParameters.StochasticSoilProfile.SoilProfile.Name);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationWithInfinities_Always_ReturnCalculationWithInfinities()
        {
            // Call
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithInfinities();

            // Assert
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);
            Assert.AreEqual(double.NegativeInfinity, calculation.InputParameters.AssessmentLevel.Value);
            Assert.AreEqual("PK001_0001", calculation.InputParameters.SurfaceLine.Name);
            Assert.AreEqual("PK001_0001_Macrostabiliteit", calculation.InputParameters.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", calculation.InputParameters.StochasticSoilProfile.SoilProfile.Name);
        }

        private static void AssertCalculation(MacroStabilityInwardsCalculation calculation,
                                              bool hasHydraulicLocation = true,
                                              bool hasAssessmentLevel = false,
                                              bool hasSurfaceLine = true,
                                              bool hasSoilModel = true,
                                              bool hasSoilProfile = true)
        {
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);

            if (hasHydraulicLocation)
            {
                Assert.AreEqual(1, calculation.InputParameters.HydraulicBoundaryLocation.Id);
                Assert.AreEqual("PUNT_KAT_18", calculation.InputParameters.HydraulicBoundaryLocation.Name);
            }
            else
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            }

            if (hasAssessmentLevel)
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
                Assert.IsTrue(calculation.InputParameters.UseAssessmentLevelManualInput);
                Assert.AreEqual(3, calculation.InputParameters.AssessmentLevel.Value);
            }

            if (hasSurfaceLine)
            {
                Assert.AreEqual("PK001_0001", calculation.InputParameters.SurfaceLine.Name);
            }
            else
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            if (hasSoilModel)
            {
                Assert.AreEqual(1, calculation.InputParameters.StochasticSoilModel.Id);
                Assert.AreEqual("PK001_0001_Macrostabiliteit", calculation.InputParameters.StochasticSoilModel.Name);

                if (hasSoilProfile)
                {
                    Assert.IsNotNull(calculation.InputParameters.StochasticSoilProfile);
                    Assert.AreEqual("W1-6_0_1D1", calculation.InputParameters.StochasticSoilProfile.SoilProfile.Name);
                    Assert.AreEqual(1, calculation.InputParameters.StochasticSoilProfile.SoilProfile.Layers.Count());
                }
                else
                {
                    Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
                }
            }
            else
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
                Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            }
        }

        private static void AssertCalculationsHasSameHydraulicBoundaryLocation(CalculationGroup calculationGroup,
                                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            IEnumerable<MacroStabilityInwardsCalculation> calculationsWithHydraulicBoundaryLocation =
                calculationGroup.Children
                                .OfType<MacroStabilityInwardsCalculation>()
                                .Where(calc => calc.InputParameters.HydraulicBoundaryLocation != null);

            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithHydraulicBoundaryLocation)
            {
                Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
            }
        }
    }
}