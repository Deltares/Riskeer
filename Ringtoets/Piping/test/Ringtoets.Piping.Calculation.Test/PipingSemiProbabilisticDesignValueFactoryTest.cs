using NUnit.Framework;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Test
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
            Assert.AreSame(inputParameters.ThicknessCoverageLayer, thicknessCoverageLayer.Distribution);
            Assert.AreEqual(0.05, thicknessCoverageLayer.Percentile);
        }

        [Test]
        public void GetSaturatedVolumicWeightOfCoverageLayer_ValidPipingCalculation_CreateDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var saturatedVolumicWeightOfCoverageLayer = PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.SaturatedVolumicWeightOfCoverageLayer, saturatedVolumicWeightOfCoverageLayer.Distribution);
            Assert.AreEqual(0.05, saturatedVolumicWeightOfCoverageLayer.Percentile);
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
            Assert.AreSame(inputParameters.SeepageLength, seepageLength.Distribution);
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
            Assert.AreSame(inputParameters.Diameter70, d70.Distribution);
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
            Assert.AreSame(inputParameters.DarcyPermeability, darcyPermeability.Distribution);
            Assert.AreEqual(0.95, darcyPermeability.Percentile);
        }

        [Test]
        public void GetThicknessAquiferLayer_ValidPipingCalculation_CreateDesignVariableForThicknessAquiferLayer()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());

            // Call
            var thicknessAquiferLayer = PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.ThicknessAquiferLayer, thicknessAquiferLayer.Distribution);
            Assert.AreEqual(0.95, thicknessAquiferLayer.Percentile);
        }

        #endregion
    }
}