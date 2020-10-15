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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data.TestUtil.Test
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
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenario();

            // Assert
            AssertCalculationScenario(calculation);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel_Always_ReturnCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel()
        {
            // Call
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel();

            // Assert
            AssertCalculationScenario(calculation, false);
        }

        [Test]
        public void GetPipingCalculationScenarioWithAssessmentLevel_Always_ReturnCalculationScenarioWithAssessmentLevel()
        {
            // Call
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithAssessmentLevel();

            // Assert
            AssertCalculationScenario(calculation, false, true);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutSurfaceLine_Always_ReturnCalculationScenarioWithoutSurfaceLine()
        {
            // Call
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSurfaceLine();

            // Assert
            AssertCalculationScenario(calculation, true, false, false);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutSoilModel_Always_ReturnCalculationScenarioWithoutSoilModel()
        {
            // Call
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilModel();

            // Assert
            AssertCalculationScenario(calculation, true, false, true, false);
        }

        [Test]
        public void GetPipingCalculationScenarioWithoutSoilProfile_Always_ReturnCalculationScenarioWithoutSoilProfile()
        {
            // Call
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilProfile();

            // Assert
            AssertCalculationScenario(calculation, true, false, true, true, false);
        }

        [Test]
        public void GetIrrelevantPipingCalculationScenario_Always_ReturnCalculationScenario()
        {
            // Call
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetIrrelevantPipingCalculationScenario();

            // Assert
            Assert.IsFalse(calculation.IsRelevant);
            Assert.AreEqual(0.5432, calculation.Contribution, calculation.Contribution.GetAccuracy());
        }

        [Test]
        public void GetPipingCalculationScenarioWithNaNs_Always_ReturnCalculationScenarioWithNaNs()
        {
            // Call
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithNaNs();

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
            SemiProbabilisticPipingCalculationScenario calculation = PipingTestDataGenerator.GetPipingCalculationScenarioWithInfinities();

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

        [Test]
        public void GetRandomSemiProbabilisticPipingOutput_Always_ReturnOutput()
        {
            // Call
            SemiProbabilisticPipingOutput output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();

            // Assert
            Assert.IsTrue(IsValidDouble(output.HeaveFactorOfSafety));
            Assert.IsTrue(IsValidDouble(output.UpliftEffectiveStress));
            Assert.IsTrue(IsValidDouble(output.UpliftFactorOfSafety));
            Assert.IsTrue(IsValidDouble(output.SellmeijerFactorOfSafety));
            Assert.IsTrue(IsValidDouble(output.HeaveGradient));
            Assert.IsTrue(IsValidDouble(output.SellmeijerCreepCoefficient));
            Assert.IsTrue(IsValidDouble(output.SellmeijerCriticalFall));
            Assert.IsTrue(IsValidDouble(output.SellmeijerReducedFall));
        }

        [Test]
        public void GetSemiProbabilisticPipingOutput_Always_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            double heaveFactorOfSafety = random.NextDouble();
            double upliftFactorOfSafety = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();

            // Call
            SemiProbabilisticPipingOutput output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(heaveFactorOfSafety, upliftFactorOfSafety, sellmeijerFactorOfSafety);

            // Assert
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety);
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety);
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety);
        }

        [Test]
        public void GetRandomPartialProbabilisticPipingOutput_WithGeneralResult_ReturnOutput()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            // Call
            PartialProbabilisticPipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(generalResult);

            // Assert
            Assert.IsTrue(IsValidDouble(output.Reliability));
            Assert.AreSame(generalResult, output.GeneralResult);
        }

        [Test]
        public void GetRandomPartialProbabilisticPipingOutput_WithoutGeneralResult_ReturnOutput()
        {
            // Call
            PartialProbabilisticPipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null);

            // Assert
            Assert.IsTrue(IsValidDouble(output.Reliability));
            Assert.IsNull(output.GeneralResult);
        }

        private static void AssertCalculationScenario(SemiProbabilisticPipingCalculationScenario calculation,
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
            IEnumerable<IPipingCalculation<PipingInput>> calculationsWithHydraulicBoundaryLocation =
                calculationGroup.Children
                                .OfType<IPipingCalculation<PipingInput>>()
                                .Where(calc => calc.InputParameters.HydraulicBoundaryLocation != null);

            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithHydraulicBoundaryLocation)
            {
                Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
            }
        }

        private static bool IsValidDouble(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}