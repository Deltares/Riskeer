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
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingDesignVariableFactoryTest
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
        public void GetUpliftModelFactorDesignVariable_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingDesignVariableFactory.GetUpliftModelFactorDesignVariable(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalPipingInput", exception.ParamName);
        }

        [Test]
        public void GetUpliftModelFactorDesignVariable_GeneralPipingInput_CreateDeterministicDesignVariableForUpliftModelFactor()
        {
            // Setup
            var generalPipingInput = new GeneralPipingInput();

            // Call
            DeterministicDesignVariable<LogNormalDistribution> upliftModelFactor = SemiProbabilisticPipingDesignVariableFactory.GetUpliftModelFactorDesignVariable(generalPipingInput);

            // Assert
            DistributionAssert.AreEqual(generalPipingInput.UpliftModelFactor, upliftModelFactor.Distribution);
            Assert.AreEqual(generalPipingInput.UpliftModelFactor.Mean, upliftModelFactor.GetDesignValue());
        }

        [Test]
        public void GetCriticalHeaveGradientDesignVariable_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingDesignVariableFactory.GetCriticalHeaveGradientDesignVariable(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalPipingInput", exception.ParamName);
        }

        [Test]
        public void GetCriticalHeaveGradientDesignVariable_GeneralPipingInput_CreateDeterministicDesignVariableForCriticalHeaveGradient()
        {
            // Setup
            var generalPipingInput = new GeneralPipingInput();

            // Call
            DeterministicDesignVariable<LogNormalDistribution> criticalHeaveGradient = SemiProbabilisticPipingDesignVariableFactory.GetCriticalHeaveGradientDesignVariable(generalPipingInput);

            // Assert
            DistributionAssert.AreEqual(generalPipingInput.CriticalHeaveGradient, criticalHeaveGradient.Distribution);
            Assert.AreEqual(0.3, criticalHeaveGradient.GetDesignValue());
        }

        [Test]
        public void GetSellmeijerModelFactorDesignVariable_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingDesignVariableFactory.GetSellmeijerModelFactorDesignVariable(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalPipingInput", exception.ParamName);
        }

        [Test]
        public void GetSellmeijerModelFactorDesignVariable_GeneralPipingInput_CreateDeterministicDesignVariableForSellmeijerModelFactor()
        {
            // Setup
            var generalPipingInput = new GeneralPipingInput();

            // Call
            DeterministicDesignVariable<LogNormalDistribution> sellmeijerModelFactor = SemiProbabilisticPipingDesignVariableFactory.GetSellmeijerModelFactorDesignVariable(generalPipingInput);

            // Assert
            DistributionAssert.AreEqual(generalPipingInput.SellmeijerModelFactor, sellmeijerModelFactor.Distribution);
            Assert.AreEqual(generalPipingInput.SellmeijerModelFactor.Mean, sellmeijerModelFactor.GetDesignValue());
        }

        [Test]
        public void GetThicknessCoverageLayer_PipingInputWithCoverLayer_CreatePercentileBasedDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            PipingInput pipingInput = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            DesignVariable<LogNormalDistribution> thicknessCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetThicknessCoverageLayer(pipingInput);

            // Assert
            Assert.IsInstanceOf<PercentileBasedDesignVariable<LogNormalDistribution>>(thicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessCoverageLayer(pipingInput), thicknessCoverageLayer.Distribution);
            AssertPercentile(0.05, thicknessCoverageLayer);
        }

        [Test]
        public void GetThicknessCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> thicknessCoverageLayer = SemiProbabilisticPipingDesignVariableFactory.GetThicknessCoverageLayer(pipingInput);

            // Assert
            Assert.IsInstanceOf<DeterministicDesignVariable<LogNormalDistribution>>(thicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessCoverageLayer(pipingInput), thicknessCoverageLayer.Distribution);
            Assert.AreEqual(new RoundedDouble(2), thicknessCoverageLayer.GetDesignValue());
        }

        [Test]
        public void GetEffectiveThicknessCoverageLayer_PipingInputWithCoverLayer_CreatePercentileBasedDesignVariableForEffectiveThicknessCoverageLayer()
        {
            // Setup
            PipingInput pipingInput = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> effectiveThicknessCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetEffectiveThicknessCoverageLayer(pipingInput, generalPipingInput);

            // Assert
            Assert.IsInstanceOf<PercentileBasedDesignVariable<LogNormalDistribution>>(effectiveThicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetEffectiveThicknessCoverageLayer(pipingInput, generalPipingInput), effectiveThicknessCoverageLayer.Distribution);
            AssertPercentile(0.05, effectiveThicknessCoverageLayer);
        }

        [Test]
        public void GetEffectiveThicknessCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForEffectiveThicknessCoverageLayer()
        {
            // Setup
            var pipingInput = new TestPipingInput();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> effectiveThicknessCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetEffectiveThicknessCoverageLayer(pipingInput, generalPipingInput);

            // Assert
            Assert.IsInstanceOf<DeterministicDesignVariable<LogNormalDistribution>>(effectiveThicknessCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetEffectiveThicknessCoverageLayer(pipingInput, generalPipingInput), effectiveThicknessCoverageLayer.Distribution);
            Assert.AreEqual(new RoundedDouble(2), effectiveThicknessCoverageLayer.GetDesignValue());
        }

        [Test]
        public void GetPhreaticLevelExit_PipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingDesignVariableFactory.GetPhreaticLevelExit(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("pipingInput", exception.ParamName);
        }

        [Test]
        public void GetPhreaticLevelExit_ValidPipingInput_CreateDesignVariableForPhreaticLevelExit()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            DesignVariable<NormalDistribution> phreaticLevelExit =
                SemiProbabilisticPipingDesignVariableFactory.GetPhreaticLevelExit(pipingInput);

            // Assert
            Assert.AreSame(pipingInput.PhreaticLevelExit, phreaticLevelExit.Distribution);
            AssertPercentile(0.05, phreaticLevelExit);
        }

        [Test]
        public void GetDampingFactorExit_PipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SemiProbabilisticPipingDesignVariableFactory.GetDampingFactorExit(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("pipingInput", exception.ParamName);
        }

        [Test]
        public void GetDampingFactorExit_ValidPipingInput_CreateDesignVariableForDampingFactorExit()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> dampingFactorExit =
                SemiProbabilisticPipingDesignVariableFactory.GetDampingFactorExit(pipingInput);

            // Assert
            Assert.AreSame(pipingInput.DampingFactorExit, dampingFactorExit.Distribution);
            AssertPercentile(0.95, dampingFactorExit);
        }

        #endregion

        #region Piping parameters

        [Test]
        public void GetSeepageLength_ValidPipingInput_CreateDesignVariableForSeepageLength()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> seepageLength =
                SemiProbabilisticPipingDesignVariableFactory.GetSeepageLength(pipingInput);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetSeepageLength(pipingInput), seepageLength.Distribution);
            AssertPercentile(0.05, seepageLength);
        }

        [Test]
        public void GetDiameter70_ValidPipingInput_CreateDesignVariableForDiameter70()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> d70 =
                SemiProbabilisticPipingDesignVariableFactory.GetDiameter70(pipingInput);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetDiameterD70(pipingInput), d70.Distribution);
            AssertPercentile(0.05, d70);
        }

        [Test]
        public void GetDarcyPermeability_ValidPipingInput_CreateDesignVariableForDarcyPermeability()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> darcyPermeability =
                SemiProbabilisticPipingDesignVariableFactory.GetDarcyPermeability(pipingInput);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetDarcyPermeability(pipingInput), darcyPermeability.Distribution);
            AssertPercentile(0.95, darcyPermeability);
        }

        [Test]
        public void GetSaturatedVolumicWeightOfCoverageLayer_PipingInputWithCoverLayerWithSaturatedDefinition_CreateDesignVariableForSaturatedVolumicWeightOfCoverageLayer()
        {
            // Setup
            PipingInput pipingInput = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            pipingInput.StochasticSoilProfile.SoilProfile.Layers.First().BelowPhreaticLevel = new LogNormalDistribution
            {
                Mean = (RoundedDouble) 3.2
            };

            // Call
            DesignVariable<LogNormalDistribution> saturatedVolumicWeightOfCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(pipingInput);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(pipingInput),
                                        saturatedVolumicWeightOfCoverageLayer.Distribution);
            AssertPercentile(0.05, saturatedVolumicWeightOfCoverageLayer);
        }

        [Test]
        public void GetSaturatedVolumicWeightOfCoverageLayer_PipingInputWithoutCoverLayer_CreateDeterministicDesignVariableForSaturatedVolumicWeightOfCoverageLayer()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> saturatedVolumicWeightOfCoverageLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetSaturatedVolumicWeightOfCoverageLayer(pipingInput);

            // Assert
            Assert.IsInstanceOf<DeterministicDesignVariable<LogNormalDistribution>>(saturatedVolumicWeightOfCoverageLayer);
            DistributionAssert.AreEqual(DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(pipingInput),
                                        saturatedVolumicWeightOfCoverageLayer.Distribution);
            Assert.AreEqual(new RoundedDouble(2), saturatedVolumicWeightOfCoverageLayer.GetDesignValue());
        }

        [Test]
        public void GetThicknessAquiferLayer_ValidPipingInput_CreateDesignVariableForThicknessAquiferLayer()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            DesignVariable<LogNormalDistribution> thicknessAquiferLayer =
                SemiProbabilisticPipingDesignVariableFactory.GetThicknessAquiferLayer(pipingInput);

            // Assert
            DistributionAssert.AreEqual(DerivedPipingInput.GetThicknessAquiferLayer(pipingInput), thicknessAquiferLayer.Distribution);
            AssertPercentile(0.95, thicknessAquiferLayer);
        }

        #endregion
    }
}