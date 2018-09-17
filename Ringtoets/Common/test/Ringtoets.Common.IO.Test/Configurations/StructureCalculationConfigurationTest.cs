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
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.Common.IO.Test.Configurations
{
    [TestFixture]
    public class StructureCalculationConfigurationTest
    {
        [Test]
        public void Constructor_WithoutName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleStructuresCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "name";

            // Call
            var configuration = new SimpleStructuresCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(configuration);
            Assert.AreEqual(name, configuration.Name);
            Assert.IsNull(configuration.StructureId);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.StormDuration);
            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.AllowedLevelIncreaseStorage);
            Assert.IsNull(configuration.StorageStructureArea);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);
            Assert.IsNull(configuration.WidthFlowApertures);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.WaveReduction);
            Assert.IsNull(configuration.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetsNewlySetValue()
        {
            // Setup
            const string configurationName = "some name";
            var configuration = new SimpleStructuresCalculationConfiguration(configurationName);

            const string structureId = "some structure";
            const string hydraulicBoundaryLocationName = "some hydraulic boundary location";
            const string foreshoreProfileName = "some foreshore profile";

            var random = new Random(22);
            double structureNormalOrientation = random.NextDouble();
            double failureProbabilityStructureWithErosion = random.NextDouble();
            bool shouldIllustrationPointsBeCalculated = random.NextBoolean();

            var stormDuration = new StochastConfiguration();
            var allowedLevelIncreaseStorage = new StochastConfiguration();
            var storageStructureArea = new StochastConfiguration();
            var flowWidthAtBottomProtection = new StochastConfiguration();
            var criticalOvertoppingDischarge = new StochastConfiguration();
            var widthFlowApertures = new StochastConfiguration();
            var waveReduction = new WaveReductionConfiguration();

            // Call
            configuration.Name = configurationName;
            configuration.StormDuration = stormDuration;
            configuration.StructureId = structureId;
            configuration.HydraulicBoundaryLocationName = hydraulicBoundaryLocationName;
            configuration.StructureNormalOrientation = structureNormalOrientation;
            configuration.AllowedLevelIncreaseStorage = allowedLevelIncreaseStorage;
            configuration.StorageStructureArea = storageStructureArea;
            configuration.FlowWidthAtBottomProtection = flowWidthAtBottomProtection;
            configuration.CriticalOvertoppingDischarge = criticalOvertoppingDischarge;
            configuration.FailureProbabilityStructureWithErosion = failureProbabilityStructureWithErosion;
            configuration.WidthFlowApertures = widthFlowApertures;
            configuration.ForeshoreProfileId = foreshoreProfileName;
            configuration.WaveReduction = waveReduction;
            configuration.ShouldIllustrationPointsBeCalculated = shouldIllustrationPointsBeCalculated;

            // Assert
            Assert.AreEqual(configurationName, configuration.Name);
            Assert.AreSame(stormDuration, configuration.StormDuration);
            Assert.AreEqual(structureId, configuration.StructureId);
            Assert.AreEqual(hydraulicBoundaryLocationName, configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual(structureNormalOrientation, configuration.StructureNormalOrientation);
            Assert.AreSame(allowedLevelIncreaseStorage, configuration.AllowedLevelIncreaseStorage);
            Assert.AreSame(storageStructureArea, configuration.StorageStructureArea);
            Assert.AreSame(flowWidthAtBottomProtection, configuration.FlowWidthAtBottomProtection);
            Assert.AreSame(criticalOvertoppingDischarge, configuration.CriticalOvertoppingDischarge);
            Assert.AreEqual(failureProbabilityStructureWithErosion, configuration.FailureProbabilityStructureWithErosion);
            Assert.AreSame(widthFlowApertures, configuration.WidthFlowApertures);
            Assert.AreEqual(foreshoreProfileName, configuration.ForeshoreProfileId);
            Assert.AreSame(waveReduction, configuration.WaveReduction);
            Assert.AreEqual(shouldIllustrationPointsBeCalculated, configuration.ShouldIllustrationPointsBeCalculated);
        }

        private class SimpleStructuresCalculationConfiguration : StructuresCalculationConfiguration
        {
            public SimpleStructuresCalculationConfiguration(string name) : base(name) {}
        }
    }
}