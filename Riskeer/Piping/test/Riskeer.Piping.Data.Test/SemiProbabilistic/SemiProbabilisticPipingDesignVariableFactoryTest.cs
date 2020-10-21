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

using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingDesignVariableFactoryTest
    {
        private static void AssertPercentile<T>(double percentile, DesignVariable<T> designVariable) where T : IDistribution
        {
            Assert.IsInstanceOf<PercentileBasedDesignVariable<T>>(designVariable);
            var percentileBasedDesignVariable = (PercentileBasedDesignVariable<T>) designVariable;
            Assert.AreEqual(percentile, percentileBasedDesignVariable.Percentile);
        }

        private static void AssertPercentile(double percentile, VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
        {
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariable>(designVariable);
            var percentileBasedDesignVariable = (VariationCoefficientLogNormalDistributionDesignVariable) designVariable;
            Assert.AreEqual(percentile, percentileBasedDesignVariable.Percentile);
        }

        #region General parameters

        [Test]
        public void GetUpliftModelFactorDesignVariable_GeneralPipingInput_CreateDeterministicDesignVariableForUpliftModelFactor()
        {
            // Setup
            var inputParameters = new GeneralPipingInput();

            // Call
            DeterministicDesignVariable<LogNormalDistribution> upliftModelFactor = SemiProbabilisticPipingDesignVariableFactory.GetUpliftModelFactorDesignVariable(inputParameters);

            // Assert
            DistributionAssert.AreEqual(inputParameters.UpliftModelFactor, upliftModelFactor.Distribution);
            Assert.AreEqual(inputParameters.UpliftModelFactor.Mean, upliftModelFactor.GetDesignValue());
        }

        [Test]
        public void GetSellmeijerModelFactorDesignVariable_GeneralPipingInput_CreateDeterministicDesignVariableForSellmeijerModelFactor()
        {
            // Setup
            var inputParameters = new GeneralPipingInput();

            // Call
            DeterministicDesignVariable<LogNormalDistribution> sellmeijerModelFactor = SemiProbabilisticPipingDesignVariableFactory.GetSellmeijerModelFactorDesignVariable(inputParameters);

            // Assert
            DistributionAssert.AreEqual(inputParameters.SellmeijerModelFactor, sellmeijerModelFactor.Distribution);
            Assert.AreEqual(inputParameters.SellmeijerModelFactor.Mean, sellmeijerModelFactor.GetDesignValue());
        }

        [Test]
        public void GetThicknessCoverageLayer_PipingInputWithCoverLayer_CreatePercentileBasedDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            PipingInput inputParameters = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            DesignVariable<LogNormalDistribution> thicknessCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetThicknessCoverageLayer(inputParameters);

            // Assert
            Assert.IsInstanceOf<PercentileBasedDesignVariable<LogNormalDistribution>>(thicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessCoverageLayer(inputParameters), thicknessCoverageLayer.Distribution);
            AssertPercentile(0.05, thicknessCoverageLayer);
        }

        [Test]
        public void GetThicknessCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> thicknessCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetThicknessCoverageLayer(inputParameters);

            // Assert
            Assert.IsInstanceOf<DeterministicDesignVariable<LogNormalDistribution>>(thicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessCoverageLayer(inputParameters), thicknessCoverageLayer.Distribution);
            Assert.AreEqual(new RoundedDouble(2), thicknessCoverageLayer.GetDesignValue());
        }

        [Test]
        public void GetEffectiveThicknessCoverageLayer_PipingInputWithCoverLayer_CreatePercentileBasedDesignVariableForEffectiveThicknessCoverageLayer()
        {
            // Setup
            PipingInput inputParameters = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> effectiveThicknessCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetEffectiveThicknessCoverageLayer(inputParameters, generalPipingInput);

            // Assert
            Assert.IsInstanceOf<PercentileBasedDesignVariable<LogNormalDistribution>>(effectiveThicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetEffectiveThicknessCoverageLayer(inputParameters, generalPipingInput), effectiveThicknessCoverageLayer.Distribution);
            AssertPercentile(0.05, effectiveThicknessCoverageLayer);
        }

        [Test]
        public void GetEffectiveThicknessCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForEffectiveThicknessCoverageLayer()
        {
            // Setup
            var inputParameters = new TestPipingInput();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> effectiveThicknessCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetEffectiveThicknessCoverageLayer(inputParameters, generalPipingInput);

            // Assert
            Assert.IsInstanceOf<DeterministicDesignVariable<LogNormalDistribution>>(effectiveThicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetEffectiveThicknessCoverageLayer(inputParameters, generalPipingInput), effectiveThicknessCoverageLayer.Distribution);
            Assert.AreEqual(new RoundedDouble(2), effectiveThicknessCoverageLayer.GetDesignValue());
        }

        [Test]
        public void GetPhreaticLevelExit_ValidPipingCalculation_CreateDesignVariableForPhreaticLevelExit()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            DesignVariable<NormalDistribution> phreaticLevelExit =
                SemiProbabilisticPipingDesignVariableFactory.GetPhreaticLevelExit(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, phreaticLevelExit.Distribution);
            AssertPercentile(0.05, phreaticLevelExit);
        }

        [Test]
        public void GetDampingFactorExit_ValidPipingCalculation_CreateDesignVariableForDampingFactorExit()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> dampingFactorExit =
                SemiProbabilisticPipingDesignVariableFactory.GetDampingFactorExit(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.DampingFactorExit, dampingFactorExit.Distribution);
            AssertPercentile(0.95, dampingFactorExit);
        }

        #endregion

        #region Piping parameters

        [Test]
        public void GetSeepageLength_ValidPipingCalculation_CreateDesignVariableForSeepageLength()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> seepageLength =
                SemiProbabilisticPipingDesignVariableFactory.GetSeepageLength(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetSeepageLength(inputParameters), seepageLength.Distribution);
            AssertPercentile(0.05, seepageLength);
        }

        [Test]
        public void GetDiameter70_ValidPipingCalculation_CreateDesignVariableForDiameter70()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> d70 =
                SemiProbabilisticPipingDesignVariableFactory.GetDiameter70(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetDiameterD70(inputParameters), d70.Distribution);
            AssertPercentile(0.05, d70);
        }

        [Test]
        public void GetDarcyPermeability_ValidPipingCalculation_CreateDesignVariableForDarcyPermeability()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> darcyPermeability =
                SemiProbabilisticPipingDesignVariableFactory.GetDarcyPermeability(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetDarcyPermeability(inputParameters), darcyPermeability.Distribution);
            AssertPercentile(0.95, darcyPermeability);
        }

        [Test]
        public void GetSaturatedVolumicWeightOfCoverageLayer_PipingInputWithCoverLayerWithSaturatedDefinition_CreateDesignVariableForSaturatedVolumicWeightOfCoverageLayer()
        {
            // Setup
            PipingInput inputParameters = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            inputParameters.StochasticSoilProfile.SoilProfile.Layers.First().BelowPhreaticLevel = new LogNormalDistribution
            {
                Mean = (RoundedDouble) 3.2
            };

            // Call
            DesignVariable<LogNormalDistribution> saturatedVolumicWeightOfCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters),
                                        saturatedVolumicWeightOfCoverageLayer.Distribution);
            AssertPercentile(0.05, saturatedVolumicWeightOfCoverageLayer);
        }

        [Test]
        public void GetSaturatedVolumicWeightOfCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForSaturatedVolumicWeightOfCoverageLayer()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> saturatedVolumicWeightOfCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters);

            // Assert
            Assert.IsInstanceOf<DeterministicDesignVariable<LogNormalDistribution>>(saturatedVolumicWeightOfCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters),
                                        saturatedVolumicWeightOfCoverageLayer.Distribution);
            Assert.AreEqual(new RoundedDouble(2), saturatedVolumicWeightOfCoverageLayer.GetDesignValue());
        }

        [Test]
        public void GetThicknessAquiferLayer_ValidPipingCalculation_CreateDesignVariableForThicknessAquiferLayer()
        {
            // Setup
            var inputParameters = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> thicknessAquiferLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetThicknessAquiferLayer(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessAquiferLayer(inputParameters), thicknessAquiferLayer.Distribution);
            AssertPercentile(0.95, thicknessAquiferLayer);
        }

        #endregion
    }
}