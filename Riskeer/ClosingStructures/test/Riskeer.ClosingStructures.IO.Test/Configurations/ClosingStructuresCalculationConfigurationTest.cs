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
using Riskeer.ClosingStructures.IO.Configurations;
using Riskeer.Common.IO.Configurations;

namespace Riskeer.ClosingStructures.IO.Test.Configurations
{
    [TestFixture]
    public class ClosingStructuresCalculationConfigurationTest
    {
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
            Assert.IsNull(configuration.ModelFactorSuperCriticalFlow);
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
    }
}