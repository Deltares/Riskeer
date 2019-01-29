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

using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingSemiProbabilisticDesignVariableFactoryTest
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
        public void GetThicknessCoverageLayer_PipingInputWithCoverLayer_CreatePercentileBasedDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            PipingInput inputParameters = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            DesignVariable<LogNormalDistribution> thicknessCoverageLayer = PipingSemiProbabilisticDesignVariableFactory.GetThicknessCoverageLayer(inputParameters);

            // Assert
            Assert.IsInstanceOf<PercentileBasedDesignVariable<LogNormalDistribution>>(thicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessCoverageLayer(inputParameters), thicknessCoverageLayer.Distribution);
            AssertPercentile(0.05, thicknessCoverageLayer);
        }

        [Test]
        public void GetThicknessCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            DesignVariable<LogNormalDistribution> thicknessCoverageLayer = PipingSemiProbabilisticDesignVariableFactory.GetThicknessCoverageLayer(inputParameters);

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

            // Call
            DesignVariable<LogNormalDistribution> effectiveThicknessCoverageLayer =
                PipingSemiProbabilisticDesignVariableFactory.GetEffectiveThicknessCoverageLayer(inputParameters);

            // Assert
            Assert.IsInstanceOf<PercentileBasedDesignVariable<LogNormalDistribution>>(effectiveThicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetEffectiveThicknessCoverageLayer(inputParameters), effectiveThicknessCoverageLayer.Distribution);
            AssertPercentile(0.05, effectiveThicknessCoverageLayer);
        }

        [Test]
        public void GetEffectiveThicknessCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForEffectiveThicknessCoverageLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            DesignVariable<LogNormalDistribution> effectiveThicknessCoverageLayer =
                PipingSemiProbabilisticDesignVariableFactory.GetEffectiveThicknessCoverageLayer(inputParameters);

            // Assert
            Assert.IsInstanceOf<DeterministicDesignVariable<LogNormalDistribution>>(effectiveThicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetEffectiveThicknessCoverageLayer(inputParameters), effectiveThicknessCoverageLayer.Distribution);
            Assert.AreEqual(new RoundedDouble(2), effectiveThicknessCoverageLayer.GetDesignValue());
        }

        [Test]
        public void GetPhreaticLevelExit_ValidPipingCalculation_CreateDesignVariableForPhreaticLevelExit()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            DesignVariable<NormalDistribution> freaticLevelExit =
                PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, freaticLevelExit.Distribution);
            AssertPercentile(0.05, freaticLevelExit);
        }

        [Test]
        public void GetDampingFactorExit_ValidPipingCalculation_CreateDesignVariableForDampingFactorExit()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            DesignVariable<LogNormalDistribution> dampingFactorExit =
                PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(inputParameters);

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
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> seepageLength =
                PipingSemiProbabilisticDesignVariableFactory.GetSeepageLength(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetSeepageLength(inputParameters), seepageLength.Distribution);
            AssertPercentile(0.05, seepageLength);
        }

        [Test]
        public void GetDiameter70_ValidPipingCalculation_CreateDesignVariableForDiameter70()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> d70 =
                PipingSemiProbabilisticDesignVariableFactory.GetDiameter70(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetDiameterD70(inputParameters), d70.Distribution);
            AssertPercentile(0.05, d70);
        }

        [Test]
        public void GetDarcyPermeability_ValidPipingCalculation_CreateDesignVariableForDarcyPermeability()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> darcyPermeability =
                PipingSemiProbabilisticDesignVariableFactory.GetDarcyPermeability(inputParameters);

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
                PipingSemiProbabilisticDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters),
                                        saturatedVolumicWeightOfCoverageLayer.Distribution);
            AssertPercentile(0.05, saturatedVolumicWeightOfCoverageLayer);
        }

        [Test]
        public void GetSaturatedVolumicWeightOfCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForSaturatedVolumicWeightOfCoverageLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            DesignVariable<LogNormalDistribution> saturatedVolumicWeightOfCoverageLayer =
                PipingSemiProbabilisticDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters);

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
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            DesignVariable<LogNormalDistribution> thicknessAquiferLayer =
                PipingSemiProbabilisticDesignVariableFactory.GetThicknessAquiferLayer(inputParameters);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessAquiferLayer(inputParameters), thicknessAquiferLayer.Distribution);
            AssertPercentile(0.95, thicknessAquiferLayer);
        }

        #endregion
    }
}