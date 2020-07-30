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

using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.StabilityPointStructures.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationTest
    {
        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "some name";

            // Call
            var configuration = new StabilityPointStructuresCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<StructuresCalculationConfiguration>(configuration);
            Assert.AreEqual(name, configuration.Name);
            Assert.IsNull(configuration.AreaFlowApertures);
            Assert.IsNull(configuration.BankWidth);
            Assert.IsNull(configuration.ConstructiveStrengthLinearLoadModel);
            Assert.IsNull(configuration.ConstructiveStrengthQuadraticLoadModel);
            Assert.IsNull(configuration.DrainCoefficient);
            Assert.IsNull(configuration.EvaluationLevel);
            Assert.IsNull(configuration.FactorStormDurationOpenStructure);
            Assert.IsNull(configuration.FailureCollisionEnergy);
            Assert.IsNull(configuration.FailureProbabilityRepairClosure);
            Assert.IsNull(configuration.FlowVelocityStructureClosable);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.InsideWaterLevel);
            Assert.IsNull(configuration.InsideWaterLevelFailureConstruction);
            Assert.IsNull(configuration.LevelCrestStructure);
            Assert.IsNull(configuration.LevellingCount);
            Assert.IsNull(configuration.LoadSchematizationType);
            Assert.IsNull(configuration.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(configuration.ShipMass);
            Assert.IsNull(configuration.ShipVelocity);
            Assert.IsNull(configuration.StabilityLinearLoadModel);
            Assert.IsNull(configuration.StabilityQuadraticLoadModel);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir);
            Assert.IsNull(configuration.VerticalDistance);
            Assert.IsNull(configuration.VolumicWeightWater);
        }
    }
}