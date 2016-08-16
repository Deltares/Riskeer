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

using NUnit.Framework;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.KernelWrapper.Test
{
    [TestFixture]
    public class PipingSemiProbabilisticDesignValueFactoryTest
    {
        #region General parameters

        [Test]
        public void GetThicknessCoverageLayer_ValidPipingCalculation_CreateDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var thicknessCoverageLayer = PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(inputParameters);

            // Assert
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.Mean, thicknessCoverageLayer.Distribution.Mean);
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.StandardDeviation, thicknessCoverageLayer.Distribution.StandardDeviation);
            Assert.AreEqual(0.05, thicknessCoverageLayer.Percentile);
        }

        [Test]
        public void GetPhreaticLevelExit_ValidPipingCalculation_CreateDesignVariableForPhreaticLevelExit()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var freaticLevelExit = PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, freaticLevelExit.Distribution);
            Assert.AreEqual(0.05, freaticLevelExit.Percentile);
        }

        [Test]
        public void GetDampingFactorExit_ValidPipingCalculation_CreateDesignVariableForDampingFactorExit()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var dampingFactorExit = PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.DampingFactorExit, dampingFactorExit.Distribution);
            Assert.AreEqual(0.95, dampingFactorExit.Percentile);
        }

        #endregion

        #region Piping parameters

        [Test]
        public void GetSeepageLength_ValidPipingCalculation_CreateDesignVariableForSeepageLength()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var seepageLength = PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(inputParameters);

            // Assert
            Assert.AreEqual(inputParameters.SeepageLength.Mean, seepageLength.Distribution.Mean);
            Assert.AreEqual(inputParameters.SeepageLength.StandardDeviation, seepageLength.Distribution.StandardDeviation);
            Assert.AreEqual(0.05, seepageLength.Percentile);
        }

        [Test]
        public void GetDiameter70_ValidPipingCalculation_CreateDesignVariableForDiameter70()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var d70 = PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters);

            // Assert
            Assert.AreEqual(inputParameters.Diameter70.Mean, d70.Distribution.Mean);
            Assert.AreEqual(inputParameters.Diameter70.StandardDeviation, d70.Distribution.StandardDeviation);
            Assert.AreEqual(0.05, d70.Percentile);
        }

        [Test]
        public void GetDarcyPermeability_ValidPipingCalculation_CreateDesignVariableForDarcyPermeability()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var darcyPermeability = PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters);

            // Assert
            Assert.AreEqual(inputParameters.DarcyPermeability.Mean, darcyPermeability.Distribution.Mean);
            Assert.AreEqual(inputParameters.DarcyPermeability.StandardDeviation, darcyPermeability.Distribution.StandardDeviation);
            Assert.AreEqual(0.95, darcyPermeability.Percentile);
        }

        [Test]
        public void GetSaturatedVolumicWeightOfCoverageLayer_ValidPipingCalculation_CreateDesignVariableForSaturatedVolumicWeightOfCoverageLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var saturatedVolumicWeightOfCoverageLayer = PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters);

            // Assert
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean, 
                saturatedVolumicWeightOfCoverageLayer.Distribution.Mean);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation, 
                saturatedVolumicWeightOfCoverageLayer.Distribution.StandardDeviation);
            Assert.AreEqual(
                inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift, 
                saturatedVolumicWeightOfCoverageLayer.Distribution.Shift);
            Assert.AreEqual(0.05, saturatedVolumicWeightOfCoverageLayer.Percentile);
        }

        [Test]
        public void GetThicknessAquiferLayer_ValidPipingCalculation_CreateDesignVariableForThicknessAquiferLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var thicknessAquiferLayer = PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters);

            // Assert
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.Mean, thicknessAquiferLayer.Distribution.Mean);
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.StandardDeviation, thicknessAquiferLayer.Distribution.StandardDeviation);
            Assert.AreEqual(0.95, thicknessAquiferLayer.Percentile);
        }

        #endregion
    }
}