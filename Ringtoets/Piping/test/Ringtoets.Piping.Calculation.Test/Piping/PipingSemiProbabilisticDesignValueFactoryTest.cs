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
            var pipingData = new PipingCalculationData();

            // Call
            var thicknessCoverageLayer = PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(pipingData);

            // Assert
            Assert.AreSame(pipingData.ThicknessCoverageLayer, thicknessCoverageLayer.Distribution);
            Assert.AreEqual(0.05, thicknessCoverageLayer.Percentile);
        }

        [Test]
        public void GetPhreaticLevelExit_ValidPipingData_CreateDesignVariableForPhreaticLevelExit()
        {
            // Setup
            var pipingData = new PipingCalculationData();

            // Call
            var freaticLevelExit = PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(pipingData);

            // Assert
            Assert.AreSame(pipingData.PhreaticLevelExit, freaticLevelExit.Distribution);
            Assert.AreEqual(0.05, freaticLevelExit.Percentile);
        }

        [Test]
        public void GetDampingFactorExit_ValidPipingData_CreateDesignVariableForDampingFactorExit()
        {
            // Setup
            var pipingData = new PipingCalculationData();

            // Call
            var dampingFactorExit = PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(pipingData);

            // Assert
            Assert.AreSame(pipingData.DampingFactorExit, dampingFactorExit.Distribution);
            Assert.AreEqual(0.95, dampingFactorExit.Percentile);
        }

        #endregion

        #region Piping parameters

        [Test]
        public void GetSeepageLength_ValidPipingData_CreateDesignVariableForSeepageLength()
        {
            // Setup
            var pipingData = new PipingCalculationData();

            // Call
            var seepageLength = PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(pipingData);

            // Assert
            Assert.AreSame(pipingData.SeepageLength, seepageLength.Distribution);
            Assert.AreEqual(0.05, seepageLength.Percentile);
        }

        [Test]
        public void GetDiameter70_ValidPipingData_CreateDesignVariableForDiameter70()
        {
            // Setup
            var pipingData = new PipingCalculationData();

            // Call
            var d70 = PipingSemiProbabilisticDesignValueFactory.GetDiameter70(pipingData);

            // Assert
            Assert.AreSame(pipingData.Diameter70, d70.Distribution);
            Assert.AreEqual(0.05, d70.Percentile);
        }

        [Test]
        public void GetDarcyPermeability_ValidPipingData_CreateDesignVariableForDarcyPermeability()
        {
            // Setup
            var pipingData = new PipingCalculationData();

            // Call
            var darcyPermeability = PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(pipingData);

            // Assert
            Assert.AreSame(pipingData.DarcyPermeability, darcyPermeability.Distribution);
            Assert.AreEqual(0.95, darcyPermeability.Percentile);
        }

        [Test]
        public void GetThicknessAquiferLayer_ValidPipingData_CreateDesignVariableForThicknessAquiferLayer()
        {
            // Setup
            var pipingData = new PipingCalculationData();

            // Call
            var thicknessAquiferLayer = PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(pipingData);

            // Assert
            Assert.AreSame(pipingData.ThicknessAquiferLayer, thicknessAquiferLayer.Distribution);
            Assert.AreEqual(0.95, thicknessAquiferLayer.Percentile);
        }

        #endregion
    }
}