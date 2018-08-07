// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.IO.Configurations;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.ClosingStructures.IO.Test.Configurations
{
    [TestFixture]
    public class ClosingStructuresCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ClosingStructuresCalculationConfiguration(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Call
            var configuration = new ClosingStructuresCalculationConfiguration("some name");

            // Assert
            Assert.IsInstanceOf<StructuresCalculationConfiguration>(configuration);
            Assert.AreEqual("some name", configuration.Name);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.InsideWaterLevel);
            Assert.IsNull(configuration.DrainCoefficient);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir);
            Assert.IsNull(configuration.AreaFlowApertures);
            Assert.IsNull(configuration.LevelCrestStructureNotClosing);
            Assert.IsNull(configuration.IdenticalApertures);
            Assert.IsNull(configuration.FactorStormDurationOpenStructure);
            Assert.IsNull(configuration.FailureProbabilityOpenStructure);
            Assert.IsNull(configuration.FailureProbabilityReparation);
            Assert.IsNull(configuration.ProbabilityOpenStructureBeforeFlooding);
        }

        [Test]
        public void SimpleProperties_SetNewValues_NewValuesSet()
        {
            // Setup
            var random = new Random(21);
            var inflowModelType = random.NextEnumValue<ConfigurationClosingStructureInflowModelType>();
            var insideWaterLevel = new StochastConfiguration();
            var drainCoefficient = new StochastConfiguration();
            var thresholdHeightOpenWeir = new StochastConfiguration();
            var areaFlowApertures = new StochastConfiguration();
            var levelCrestStructureNotClosing = new StochastConfiguration();
            int identicalApertures = random.Next(1, 5);
            double factorStormDurationOpenStructure = random.NextDouble();
            double failureProbabilityOpenStructure = random.NextDouble();
            double failureProbabilityReparation = random.NextDouble();
            double probabilityOpenStructureBeforeFlooding = random.NextDouble();

            var configuration = new ClosingStructuresCalculationConfiguration("some name");

            // Call
            configuration.InflowModelType = inflowModelType;
            configuration.InsideWaterLevel = insideWaterLevel;
            configuration.DrainCoefficient = drainCoefficient;
            configuration.ThresholdHeightOpenWeir = thresholdHeightOpenWeir;
            configuration.AreaFlowApertures = areaFlowApertures;
            configuration.LevelCrestStructureNotClosing = levelCrestStructureNotClosing;
            configuration.IdenticalApertures = identicalApertures;
            configuration.FactorStormDurationOpenStructure = factorStormDurationOpenStructure;
            configuration.FailureProbabilityOpenStructure = failureProbabilityOpenStructure;
            configuration.FailureProbabilityReparation = failureProbabilityReparation;
            configuration.ProbabilityOpenStructureBeforeFlooding = probabilityOpenStructureBeforeFlooding;

            // Assert
            Assert.AreEqual(inflowModelType, configuration.InflowModelType);
            Assert.AreSame(insideWaterLevel, configuration.InsideWaterLevel);
            Assert.AreSame(drainCoefficient, configuration.DrainCoefficient);
            Assert.AreSame(thresholdHeightOpenWeir, configuration.ThresholdHeightOpenWeir);
            Assert.AreSame(areaFlowApertures, configuration.AreaFlowApertures);
            Assert.AreSame(levelCrestStructureNotClosing, configuration.LevelCrestStructureNotClosing);
            Assert.AreEqual(identicalApertures, configuration.IdenticalApertures);
            Assert.AreEqual(factorStormDurationOpenStructure, configuration.FactorStormDurationOpenStructure);
            Assert.AreEqual(failureProbabilityOpenStructure, configuration.FailureProbabilityOpenStructure);
            Assert.AreEqual(failureProbabilityReparation, configuration.FailureProbabilityReparation);
            Assert.AreEqual(probabilityOpenStructureBeforeFlooding, configuration.ProbabilityOpenStructureBeforeFlooding);
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new ClosingStructuresCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}