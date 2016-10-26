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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.8)]
        [TestCase(1.0)]
        public void CreatePipingCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(double probability)
        {
            // Setup
            FailureMechanismSection section = CreateSection();

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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateFailedPipingCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSetToNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateIrreleveantPipingCalculationScenario_WithSection_CreatesIrrelevantCalculation()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateNotCalculatedPipingCalculationScenario_WithSection_CreatesRelevantCalculationWithoutOutput()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

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