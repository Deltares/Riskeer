// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Primitives;

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
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasFailureMechanismSections(failureMechanism);
        }

        [Test]
        public void ConfigureFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            MacroStabilityInwardsTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);

            // Assert
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);
            MacroStabilityInwardsTestDataGeneratorHelper.AssertHasFailureMechanismSections(failureMechanism);

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
            Assert.AreEqual(0.5432, calculation.Contribution, calculation.Contribution.GetAccuracy());
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
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Inwards.Value);
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Outwards.Value);

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

            IMacroStabilityInwardsLocationInputDaily inputDaily = input.LocationInputDaily;
            Assert.IsNaN(inputDaily.WaterLevelPolder);
            Assert.IsNaN(inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNaN(inputDaily.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder);

            IMacroStabilityInwardsLocationInputExtreme inputExtreme = input.LocationInputExtreme;
            Assert.IsNaN(inputExtreme.PenetrationLength);
            Assert.IsNaN(inputExtreme.WaterLevelPolder);
            Assert.IsNaN(inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNaN(inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder);

            Assert.IsNaN(input.ZoneBoundaryLeft);
            Assert.IsNaN(input.ZoneBoundaryRight);
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
            Assert.AreEqual(double.PositiveInfinity, input.PiezometricHeadPhreaticLine2Inwards.Value);
            Assert.AreEqual(double.NegativeInfinity, input.PiezometricHeadPhreaticLine2Outwards.Value);
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

            IMacroStabilityInwardsLocationInputDaily inputDaily = input.LocationInputDaily;
            Assert.AreEqual(double.PositiveInfinity, inputDaily.WaterLevelPolder);
            Assert.AreEqual(double.PositiveInfinity, inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(double.PositiveInfinity, inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(double.NegativeInfinity, inputDaily.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(double.NegativeInfinity, inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder);

            IMacroStabilityInwardsLocationInputExtreme inputExtreme = input.LocationInputExtreme;
            Assert.AreEqual(double.PositiveInfinity, inputExtreme.PenetrationLength);
            Assert.AreEqual(double.NegativeInfinity, inputExtreme.WaterLevelPolder);
            Assert.AreEqual(double.NegativeInfinity, inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(double.NegativeInfinity, inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(double.PositiveInfinity, inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(double.PositiveInfinity, inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder);

            Assert.AreEqual(double.NegativeInfinity, input.ZoneBoundaryLeft);
            Assert.AreEqual(double.PositiveInfinity, input.ZoneBoundaryRight);
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
            Assert.AreEqual(20.1, input.PiezometricHeadPhreaticLine2Inwards, input.PiezometricHeadPhreaticLine2Inwards.GetAccuracy());
            Assert.AreEqual(20.2, input.PiezometricHeadPhreaticLine2Outwards, input.PiezometricHeadPhreaticLine2Outwards.GetAccuracy());

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
                Assert.AreEqual(3, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
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

            IMacroStabilityInwardsLocationInputDaily inputDaily = input.LocationInputDaily;
            Assert.IsTrue(inputDaily.UseDefaultOffsets);
            Assert.AreEqual(2.2, inputDaily.WaterLevelPolder, inputDaily.WaterLevelPolder.GetAccuracy());
            Assert.AreEqual(2.21, inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver, inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());
            Assert.AreEqual(2.22, inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder, inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());
            Assert.AreEqual(2.23, inputDaily.PhreaticLineOffsetBelowShoulderBaseInside, inputDaily.PhreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
            Assert.AreEqual(2.24, inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder, inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());

            IMacroStabilityInwardsLocationInputExtreme inputExtreme = input.LocationInputExtreme;
            Assert.IsFalse(inputExtreme.UseDefaultOffsets);
            Assert.AreEqual(16.2, inputExtreme.PenetrationLength, inputExtreme.PenetrationLength.GetAccuracy());
            Assert.AreEqual(15.2, inputExtreme.WaterLevelPolder, inputExtreme.WaterLevelPolder.GetAccuracy());
            Assert.AreEqual(15.21, inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver, inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());
            Assert.AreEqual(15.22, inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder, inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());
            Assert.AreEqual(15.23, inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside, inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
            Assert.AreEqual(15.24, inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder, inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());

            Assert.IsTrue(input.CreateZones);
            Assert.AreEqual(MacroStabilityInwardsZoningBoundariesDeterminationType.Manual, input.ZoningBoundariesDeterminationType);
            Assert.AreEqual(0.0, input.ZoneBoundaryLeft, input.ZoneBoundaryLeft.GetAccuracy());
            Assert.AreEqual(100.0, input.ZoneBoundaryRight, input.ZoneBoundaryRight.GetAccuracy());
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