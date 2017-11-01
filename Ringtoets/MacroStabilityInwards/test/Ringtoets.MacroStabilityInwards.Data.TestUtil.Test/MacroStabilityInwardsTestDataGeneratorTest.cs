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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test
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
        public void GetMacroStabilityInwardsCalculationScenario_Always_ReturnCalculationScenarioWithDataSet()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenario();

            // Assert
            AssertCalculation(calculation);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel_Always_ReturnCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel();

            // Assert
            AssertCalculation(calculation, false);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationScenarioWithAssessmentLevel_Always_ReturnCalculationScenarioWithAssessmentLevel()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithAssessmentLevel();

            // Assert
            AssertCalculation(calculation, false, true);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationScenarioWithoutSurfaceLine_Always_ReturnCalculationScenarioWithoutSurfaceLine()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutSurfaceLine();

            // Assert
            AssertCalculation(calculation, true, false, false);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationScenarioWithoutSoilModel_Always_ReturnCalculationScenarioWithoutSoilModel()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutSoilModel();

            // Assert
            AssertCalculation(calculation, true, false, true, false);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationScenarioWithoutSoilProfile_Always_ReturnCalculationScenarioWithoutSoilProfile()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutSoilProfile();

            // Assert
            AssertCalculation(calculation, true, false, true, true, false);
        }

        [Test]
        public void GetIrrelevantMacroStabilityInwardsCalculationScenario_Always_ReturnCalculationScenario()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsTestDataGenerator.GetIrrelevantMacroStabilityInwardsCalculationScenario();

            // Assert
            Assert.IsFalse(calculation.IsRelevant);
            Assert.AreEqual(0.5, calculation.Contribution, calculation.Contribution.GetAccuracy());
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationScenarioWithNaNs_Always_ReturnCalculationScenarioWithNaNs()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithNaNs();

            // Assert
            Assert.IsTrue(calculation.IsRelevant);
            Assert.IsNaN(calculation.Contribution);
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);

            MacroStabilityInwardsInput input = calculation.InputParameters;
            Assert.IsNaN(input.AssessmentLevel.Value);
            Assert.AreEqual("PK001_0001", input.SurfaceLine.Name);
            Assert.AreEqual("PK001_0001_Macrostabiliteit", input.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", input.StochasticSoilProfile.SoilProfile.Name);
            Assert.IsNaN(input.XCoordinateDrainageConstruction);
            Assert.IsNaN(input.ZCoordinateDrainageConstruction);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopRiver);

            Assert.IsNaN(input.SlipPlaneMinimumDepth);
            Assert.IsNaN(input.SlipPlaneMinimumLength);
            Assert.IsNaN(input.MaximumSliceWidth);

            Assert.IsNaN(input.TangentLineZTop);
            Assert.IsNaN(input.TangentLineZBottom);

            Assert.IsNaN(input.LeftGrid.XLeft);
            Assert.IsNaN(input.LeftGrid.XRight);
            Assert.IsNaN(input.LeftGrid.ZTop);
            Assert.IsNaN(input.LeftGrid.ZBottom);

            Assert.IsNaN(input.RightGrid.XLeft);
            Assert.IsNaN(input.RightGrid.XRight);
            Assert.IsNaN(input.RightGrid.ZTop);
            Assert.IsNaN(input.RightGrid.ZBottom);
        }

        [Test]
        public void GetMacroStabilityInwardsCalculationScenarioWithInfinities_Always_ReturnCalculationScenarioWithInfinities()
        {
            // Call
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithInfinities();

            // Assert
            Assert.IsTrue(calculation.IsRelevant);
            Assert.AreEqual(double.PositiveInfinity, calculation.Contribution);
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);

            MacroStabilityInwardsInput input = calculation.InputParameters;
            Assert.AreEqual(double.PositiveInfinity, input.WaterLevelRiverAverage.Value);
            Assert.AreEqual(double.PositiveInfinity, input.XCoordinateDrainageConstruction.Value);
            Assert.AreEqual(double.NegativeInfinity, input.ZCoordinateDrainageConstruction.Value);

            Assert.AreEqual(double.PositiveInfinity, input.MinimumLevelPhreaticLineAtDikeTopPolder.Value);
            Assert.AreEqual(double.NegativeInfinity, input.MinimumLevelPhreaticLineAtDikeTopRiver.Value);
            Assert.AreEqual(double.NegativeInfinity, input.AssessmentLevel.Value);
            Assert.AreEqual("PK001_0001", input.SurfaceLine.Name);
            Assert.AreEqual("PK001_0001_Macrostabiliteit", input.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", input.StochasticSoilProfile.SoilProfile.Name);

            Assert.AreEqual(double.NegativeInfinity, input.SlipPlaneMinimumDepth);
            Assert.AreEqual(double.PositiveInfinity, input.SlipPlaneMinimumLength);
            Assert.AreEqual(double.NegativeInfinity, input.MaximumSliceWidth);

            Assert.AreEqual(double.PositiveInfinity, input.TangentLineZTop);
            Assert.AreEqual(double.NegativeInfinity, input.TangentLineZBottom);

            Assert.AreEqual(double.NegativeInfinity, input.LeftGrid.XLeft);
            Assert.AreEqual(double.PositiveInfinity, input.LeftGrid.XRight);
            Assert.AreEqual(double.PositiveInfinity, input.LeftGrid.ZTop);
            Assert.AreEqual(double.NegativeInfinity, input.LeftGrid.ZBottom);

            Assert.AreEqual(double.NegativeInfinity, input.RightGrid.XLeft);
            Assert.AreEqual(double.PositiveInfinity, input.RightGrid.XRight);
            Assert.AreEqual(double.PositiveInfinity, input.RightGrid.ZTop);
            Assert.AreEqual(double.NegativeInfinity, input.RightGrid.ZBottom);
        }

        private static void AssertCalculation(MacroStabilityInwardsCalculationScenario calculation,
                                              bool hasHydraulicLocation = true,
                                              bool hasAssessmentLevel = false,
                                              bool hasSurfaceLine = true,
                                              bool hasSoilModel = true,
                                              bool hasSoilProfile = true)
        {
            Assert.IsTrue(calculation.IsRelevant);
            Assert.AreEqual(1.0, calculation.Contribution, calculation.Contribution.GetAccuracy());
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);

            MacroStabilityInwardsInput input = calculation.InputParameters;
            Assert.IsTrue(input.DrainageConstructionPresent);
            Assert.AreEqual(10.5, input.WaterLevelRiverAverage, input.WaterLevelRiverAverage.GetAccuracy());
            Assert.AreEqual(10.6, input.XCoordinateDrainageConstruction, input.XCoordinateDrainageConstruction.GetAccuracy());
            Assert.AreEqual(10.7, input.ZCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction.GetAccuracy());
            Assert.AreEqual(10.8, input.MinimumLevelPhreaticLineAtDikeTopPolder, input.MinimumLevelPhreaticLineAtDikeTopPolder.GetAccuracy());
            Assert.AreEqual(10.9, input.MinimumLevelPhreaticLineAtDikeTopRiver, input.MinimumLevelPhreaticLineAtDikeTopRiver.GetAccuracy());

            Assert.AreEqual(0.4, input.SlipPlaneMinimumDepth, input.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(0.5, input.SlipPlaneMinimumLength, input.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(0.6, input.MaximumSliceWidth, input.MaximumSliceWidth.GetAccuracy());
            Assert.AreEqual(10, input.TangentLineZTop, input.TangentLineZTop.GetAccuracy());
            Assert.AreEqual(1, input.TangentLineZBottom, input.TangentLineZBottom.GetAccuracy());
            Assert.AreEqual(5, input.TangentLineNumber);

            if (hasHydraulicLocation)
            {
                Assert.AreEqual(1, input.HydraulicBoundaryLocation.Id);
                Assert.AreEqual("PUNT_KAT_18", input.HydraulicBoundaryLocation.Name);
            }
            else
            {
                Assert.IsNull(input.HydraulicBoundaryLocation);
            }

            if (hasAssessmentLevel)
            {
                Assert.IsNull(input.HydraulicBoundaryLocation);
                Assert.IsTrue(input.UseAssessmentLevelManualInput);
                Assert.AreEqual(3, input.AssessmentLevel,
                                input.AssessmentLevel.GetAccuracy());
            }

            if (hasSurfaceLine)
            {
                Assert.AreEqual("PK001_0001", input.SurfaceLine.Name);
            }
            else
            {
                Assert.IsNull(input.SurfaceLine);
            }

            if (hasSoilModel)
            {
                Assert.AreEqual("PK001_0001_Macrostabiliteit", input.StochasticSoilModel.Name);

                if (hasSoilProfile)
                {
                    Assert.IsNotNull(input.StochasticSoilProfile);
                    var soilProfile = input.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
                    Assert.NotNull(soilProfile);
                    Assert.AreEqual("W1-6_0_1D1", soilProfile.Name);
                    Assert.AreEqual(1, soilProfile.Layers.Count());
                }
                else
                {
                    Assert.IsNull(input.StochasticSoilProfile);
                }
            }
            else
            {
                Assert.IsNull(input.StochasticSoilModel);
                Assert.IsNull(input.StochasticSoilProfile);
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