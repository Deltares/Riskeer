// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Integration.TestUtils.Test
{
    [TestFixture]
    public class PipingTestDataGeneratorTest
    {
        [Test]
        public void GetPipingFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Call
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();

            // Assert
            PipingTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);
        }

        [Test]
        public void ConfigureFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateFullyCalculated();

            // Call
            PipingTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);

            // Assert
            PipingTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);

            AssertCalculationsHasSameHydraulicBoundaryLocation(failureMechanism.CalculationsGroup, hydraulicBoundaryLocation);

            CalculationGroup nestedCalculationGroup = failureMechanism.CalculationsGroup.Children.OfType<CalculationGroup>().First();
            AssertCalculationsHasSameHydraulicBoundaryLocation(nestedCalculationGroup, hydraulicBoundaryLocation);
        }

        [Test]
        public void GetPipingCalculation_Always_ReturnCalculationWithDataSet()
        {
            // Call
            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculation();

            // Assert
            AssertCalculation(calculation);
        }

        [Test]
        public void GetPipingCalculationWithoutHydraulicLocationAndAssessmentLevel_Always_ReturnCalculationWithoutHydraulicLocationAndAssessmentLevel()
        {
            // Call
            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculationWithoutHydraulicLocationAndAssessmentLevel();

            // Assert
            AssertCalculation(calculation, false);
        }

        [Test]
        public void GetPipingCalculationWithAssessmentLevel_Always_ReturnCalculationWithAssessmentLevel()
        {
            // Call
            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculationWithAssessmentLevel();

            // Assert
            AssertCalculation(calculation, false, true);
        }

        [Test]
        public void GetPipingCalculationWithoutSurfaceLine_Always_ReturnCalculationWithoutSurfaceLine()
        {
            // Call
            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculationWithoutSurfaceLine();

            // Assert
            AssertCalculation(calculation, true, false, false);
        }

        [Test]
        public void GetPipingCalculationWithoutSoilModel_Always_ReturnCalculationWithoutSoilModel()
        {
            // Call
            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculationWithoutSoilModel();

            // Assert
            AssertCalculation(calculation, true, false, true, false);
        }

        [Test]
        public void GetPipingCalculationWithoutSoilProfile_Always_ReturnCalculationWithoutSoilProfile()
        {
            // Call
            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculationWithoutSoilProfile();

            // Assert
            AssertCalculation(calculation, true, false, true, true, false);
        }

        private static void AssertCalculation(PipingCalculation calculation,
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
                Assert.AreEqual("PK001_0001_Piping", calculation.InputParameters.StochasticSoilModel.Name);

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

            Assert.AreEqual(0, calculation.InputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(0.1, calculation.InputParameters.PhreaticLevelExit.StandardDeviation.Value);
            Assert.AreEqual(0.7, calculation.InputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(0.1, calculation.InputParameters.DampingFactorExit.StandardDeviation.Value);
        }

        private static void AssertCalculationsHasSameHydraulicBoundaryLocation(CalculationGroup calculations,
                                                                               TestHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            IEnumerable<PipingCalculation> calculationsWithHydraulicBoundaryLocation =
                calculations.Children
                            .OfType<PipingCalculation>()
                            .Where(calc => calc.InputParameters.HydraulicBoundaryLocation != null);

            foreach (PipingCalculation calculation in calculationsWithHydraulicBoundaryLocation)
            {
                Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
            }
        }
    }
}