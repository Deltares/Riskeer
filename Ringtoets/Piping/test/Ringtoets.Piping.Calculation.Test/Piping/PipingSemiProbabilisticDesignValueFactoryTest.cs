using NUnit.Framework;

using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Test.Piping
{
    [TestFixture]
    public class PipingSemiProbabilisticDesignValueFactoryTest
    {
        #region General parameters

        [Test]
        public void GetThicknessCoverageLayer_ValidPipingData_CreateDesignVariableForThicknessCoverageLayer()
        {
            // Setup
            var inputParameters = new PipingInputParameters();

            // Call
            var thicknessCoverageLayer = PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.ThicknessCoverageLayer, thicknessCoverageLayer.Distribution);
            Assert.AreEqual(0.05, thicknessCoverageLayer.Percentile);
        }

        [Test]
        public void GetPhreaticLevelExit_ValidPipingData_CreateDesignVariableForPhreaticLevelExit()
        {
            // Setup
            var inputParameters = new PipingInputParameters();

            // Call
            var freaticLevelExit = PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, freaticLevelExit.Distribution);
            Assert.AreEqual(0.05, freaticLevelExit.Percentile);
        }

        [Test]
        public void GetDampingFactorExit_ValidPipingData_CreateDesignVariableForDampingFactorExit()
        {
            // Setup
            var inputParameters = new PipingInputParameters();

            // Call
            var dampingFactorExit = PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.DampingFactorExit, dampingFactorExit.Distribution);
            Assert.AreEqual(0.95, dampingFactorExit.Percentile);
        }

        #endregion

        #region Piping parameters

        [Test]
        public void GetSeepageLength_ValidPipingData_CreateDesignVariableForSeepageLength()
        {
            // Setup
            var inputParameters = new PipingInputParameters();

            // Call
            var seepageLength = PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.SeepageLength, seepageLength.Distribution);
            Assert.AreEqual(0.05, seepageLength.Percentile);
        }

        [Test]
        public void GetDiameter70_ValidPipingData_CreateDesignVariableForDiameter70()
        {
            // Setup
            var inputParameters = new PipingInputParameters();

            // Call
            var d70 = PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.Diameter70, d70.Distribution);
            Assert.AreEqual(0.05, d70.Percentile);
        }

        [Test]
        public void GetDarcyPermeability_ValidPipingData_CreateDesignVariableForDarcyPermeability()
        {
            // Setup
            var inputParameters = new PipingInputParameters();

            // Call
            var darcyPermeability = PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.DarcyPermeability, darcyPermeability.Distribution);
            Assert.AreEqual(0.95, darcyPermeability.Percentile);
        }

        [Test]
        public void GetThicknessAquiferLayer_ValidPipingData_CreateDesignVariableForThicknessAquiferLayer()
        {
            // Setup
            var inputParameters = new PipingInputParameters();

            // Call
            var thicknessAquiferLayer = PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters);

            // Assert
            Assert.AreSame(inputParameters.ThicknessAquiferLayer, thicknessAquiferLayer.Distribution);
            Assert.AreEqual(0.95, thicknessAquiferLayer.Percentile);
        }

        #endregion
    }
}