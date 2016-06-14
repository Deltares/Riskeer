using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class PipingCalculationScenarioFactoryTest
    {
        [Test]
        public void CreatePipingCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationScenarioFactory.CreatePipingCalculationScenario(double.NaN, null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(3.0)]
        [TestCase(115.2)]
        [TestCase(0.2)]
        public void CreatePipingCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(double probability)
        {
            // Setup
            var section = CreateSection();

            // Call
            var scenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(probability, section);

            // Assert
            Assert.NotNull(scenario.SemiProbabilisticOutput);
            Assert.NotNull(scenario.Output);
            Assert.AreEqual(probability, scenario.SemiProbabilisticOutput.PipingProbability, 1e-6);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateFailedPipingCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateFailedPipingCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSetToNaN()
        {
            // Setup
            var section = CreateSection();

            // Call
            var scenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);

            // Assert
            Assert.NotNull(scenario.SemiProbabilisticOutput);
            Assert.NotNull(scenario.Output);
            Assert.IsNaN(scenario.SemiProbabilisticOutput.PipingProbability);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateIrreleveantPipingCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationScenarioFactory.CreateIrreleveantPipingCalculationScenario(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateIrreleveantPipingCalculationScenario_WithSection_CreatesIrrelevantCalculation()
        {
            // Setup
            var section = CreateSection();

            // Call
            var scenario = PipingCalculationScenarioFactory.CreateIrreleveantPipingCalculationScenario(section);

            // Assert
            Assert.IsFalse(scenario.IsRelevant);
        }

        [Test]
        public void CreateNotCalculatedPipingCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateNotCalculatedPipingCalculationScenario_WithSection_CreatesRelevantCalculationWithoutOutput()
        {
            // Setup
            var section = CreateSection();

            // Call
            var scenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            // Assert
            Assert.IsNull(scenario.SemiProbabilisticOutput);
            Assert.IsNull(scenario.Output);
            Assert.IsTrue(scenario.IsRelevant);
        }

        private FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}