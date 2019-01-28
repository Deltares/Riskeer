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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.TestUtil.Test
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
            PipingTestDataGeneratorHelper.AssertHasFailureMechanismSections(failureMechanism);
        }

        [Test]
        public void ConfigureFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            PipingTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);

            // Assert
            PipingTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasFailureMechanismSections(failureMechanism);

            AssertCalculationsHasSameHydraulicBoundaryLocation(failureMechanism.CalculationsGroup, hydraulicBoundaryLocation);

            CalculationGroup nestedCalculationGroup = failureMechanism.CalculationsGroup.Children.OfType<CalculationGroup>().First();
            AssertCalculationsHasSameHydraulicBoundaryLocation(nestedCalculationGroup, hydraulicBoundaryLocation);
        }

        [Test]
        public void GetPipingCalculationScenario_Always_ReturnCalculationWithDataSet()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenario();

            // Assert
            AssertCalculationScenario(calculation);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel_Always_ReturnCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel();

            // Assert
            AssertCalculationScenario(calculation, false);
        }

        [Test]
        public void GetPipingCalculationScenarioWithAssessmentLevel_Always_ReturnCalculationScenarioWithAssessmentLevel()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithAssessmentLevel();

            // Assert
            AssertCalculationScenario(calculation, false, true);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutSurfaceLine_Always_ReturnCalculationScenarioWithoutSurfaceLine()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSurfaceLine();

            // Assert
            AssertCalculationScenario(calculation, true, false, false);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutSoilModel_Always_ReturnCalculationScenarioWithoutSoilModel()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilModel();

            // Assert
            AssertCalculationScenario(calculation, true, false, true, false);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutSoilProfile_Always_ReturnCalculationScenarioWithoutSoilProfile()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilProfile();

            // Assert
            AssertCalculationScenario(calculation, true, false, true, true, false);
        }

        [Test]
        public void GetIrrelevantPipingCalculationScenario_Always_ReturnCalculationScenario()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetIrrelevantPipingCalculationScenario();

            // Assert
            Assert.IsFalse(calculation.IsRelevant);
            Assert.AreEqual(0.5432, calculation.Contribution, calculation.Contribution.GetAccuracy());
        }

        [Test]
        public void GetPipingCalculationScenarioWithNaNs_Always_ReturnCalculationScenarioWithNaNs()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithNaNs();

            // Assert
            Assert.IsTrue(calculation.IsRelevant);
            Assert.AreEqual(double.NaN, calculation.Contribution);
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);
            Assert.AreEqual(double.NaN, calculation.InputParameters.AssessmentLevel.Value);
            Assert.AreEqual("PK001_0001", calculation.InputParameters.SurfaceLine.Name);
            Assert.AreEqual(double.NaN, calculation.InputParameters.EntryPointL.Value);
            Assert.AreEqual(double.NaN, calculation.InputParameters.ExitPointL.Value);
            Assert.AreEqual("PK001_0001_Piping", calculation.InputParameters.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", calculation.InputParameters.StochasticSoilProfile.SoilProfile.Name);
            Assert.AreEqual(double.NaN, calculation.InputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(double.NaN, calculation.InputParameters.PhreaticLevelExit.StandardDeviation.Value);
            Assert.AreEqual(double.NaN, calculation.InputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(double.NaN, calculation.InputParameters.DampingFactorExit.StandardDeviation.Value);
        }

        [Test]
        public void GetPipingCalculationScenarioWithInfinities_Always_ReturnCalculationScenarioWithInfinities()
        {
            // Call
            PipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithInfinities();

            // Assert
            Assert.IsTrue(calculation.IsRelevant);
            Assert.AreEqual(double.PositiveInfinity, calculation.Contribution);
            Assert.AreEqual("PK001_0001 W1-6_0_1D1", calculation.Name);
            Assert.AreEqual(double.NegativeInfinity, calculation.InputParameters.AssessmentLevel.Value);
            Assert.AreEqual("PK001_0001", calculation.InputParameters.SurfaceLine.Name);
            Assert.AreEqual(double.NegativeInfinity, calculation.InputParameters.EntryPointL.Value);
            Assert.AreEqual(double.PositiveInfinity, calculation.InputParameters.ExitPointL.Value);
            Assert.AreEqual("PK001_0001_Piping", calculation.InputParameters.StochasticSoilModel.Name);
            Assert.AreEqual("W1-6_0_1D1", calculation.InputParameters.StochasticSoilProfile.SoilProfile.Name);
            Assert.AreEqual(double.NegativeInfinity, calculation.InputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(double.PositiveInfinity, calculation.InputParameters.PhreaticLevelExit.StandardDeviation.Value);
            Assert.AreEqual(double.PositiveInfinity, calculation.InputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(double.PositiveInfinity, calculation.InputParameters.DampingFactorExit.StandardDeviation.Value);
        }

        private static void AssertCalculationScenario(PipingCalculationScenario calculation,
                                                      bool hasHydraulicLocation = true,
                                                      bool hasAssessmentLevel = false,
                                                      bool hasSurfaceLine = true,
                                                      bool hasSoilModel = true,
                                                      bool hasSoilProfile = true)
        {
            Assert.IsTrue(calculation.IsRelevant);
            Assert.AreEqual(1.0, calculation.Contribution, calculation.Contribution.GetAccuracy());
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
                Assert.AreEqual(3, calculation.InputParameters.AssessmentLevel,
                                calculation.InputParameters.AssessmentLevel.GetAccuracy());
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

            DistributionAssert.AreEqual(new NormalDistribution(3)
            {
                Mean = (RoundedDouble) 0,
                StandardDeviation = (RoundedDouble) 0.1
            }, calculation.InputParameters.PhreaticLevelExit);

            DistributionAssert.AreEqual(new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.7,
                StandardDeviation = (RoundedDouble) 0.1
            }, calculation.InputParameters.DampingFactorExit);
        }

        private static void AssertCalculationsHasSameHydraulicBoundaryLocation(CalculationGroup calculationGroup,
                                                                               HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            IEnumerable<PipingCalculation> calculationsWithHydraulicBoundaryLocation =
                calculationGroup.Children
                                .OfType<PipingCalculation>()
                                .Where(calc => calc.InputParameters.HydraulicBoundaryLocation != null);

            foreach (PipingCalculation calculation in calculationsWithHydraulicBoundaryLocation)
            {
                Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
            }
        }
    }
}