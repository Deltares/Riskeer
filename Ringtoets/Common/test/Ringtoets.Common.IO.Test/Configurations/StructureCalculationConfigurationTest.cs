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
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.Common.IO.Test.Configurations
{
    [TestFixture]
    public class StructureCalculationConfigurationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var configuration = new SimpleStructureCalculationConfiguration();

            // Assert
            Assert.IsNull(configuration.Name);
            Assert.IsNull(configuration.ModelFactorSuperCriticalFlow);
            Assert.IsNull(configuration.StructureName);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.StormDuration);
            Assert.IsNull(configuration.StructureNormalOrientation);
            Assert.IsNull(configuration.AllowedLevelIncreaseStorage);
            Assert.IsNull(configuration.StorageStructureArea);
            Assert.IsNull(configuration.FlowWidthAtBottomProtection);
            Assert.IsNull(configuration.CriticalOvertoppingDischarge);
            Assert.IsNull(configuration.FailureProbabilityStructureWithErosion);
            Assert.IsNull(configuration.WidthFlowApertures);
            Assert.IsNull(configuration.ForeshoreProfileName);
            Assert.IsNull(configuration.WaveReduction);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetsNewlySetValue()
        {
            // Setup
            var configuration = new SimpleStructureCalculationConfiguration();

            const string configurationName = "some name";
            const string structureName = "some structure";
            const string hydraulicBoundaryLocationName = "some hydraulic boundary location";
            const string foreshoreProfileName = "some foreshore profile";

            var random = new Random(22);
            double structureNormalOrientation = random.NextDouble();
            double failureProbabilityStructureWithErosion = random.NextDouble();

            var modelFactorSuperCriticalFlow = new MeanStandardDeviationStochastConfiguration();
            var stormDuration = new MeanVariationCoefficientStochastConfiguration();
            var allowedLevelIncreaseStorage = new MeanStandardDeviationStochastConfiguration();
            var storageStructureArea = new MeanVariationCoefficientStochastConfiguration();
            var flowWidthAtBottomProtection = new MeanStandardDeviationStochastConfiguration();
            var criticalOvertoppingDischarge = new MeanVariationCoefficientStochastConfiguration();
            var widthFlowApertures = new MeanStandardDeviationStochastConfiguration();
            var waveReduction = new WaveReductionConfiguration();

            // Call
            configuration.Name = configurationName;
            configuration.ModelFactorSuperCriticalFlow = modelFactorSuperCriticalFlow;
            configuration.StormDuration = stormDuration;
            configuration.StructureName = structureName;
            configuration.HydraulicBoundaryLocationName = hydraulicBoundaryLocationName;
            configuration.StructureNormalOrientation = structureNormalOrientation;
            configuration.AllowedLevelIncreaseStorage = allowedLevelIncreaseStorage;
            configuration.StorageStructureArea = storageStructureArea;
            configuration.FlowWidthAtBottomProtection = flowWidthAtBottomProtection;
            configuration.CriticalOvertoppingDischarge = criticalOvertoppingDischarge;
            configuration.FailureProbabilityStructureWithErosion = failureProbabilityStructureWithErosion;
            configuration.WidthFlowApertures = widthFlowApertures;
            configuration.ForeshoreProfileName = foreshoreProfileName;
            configuration.WaveReduction = waveReduction;

            // Assert
            Assert.AreEqual(configurationName, configuration.Name);
            Assert.AreSame(modelFactorSuperCriticalFlow, configuration.ModelFactorSuperCriticalFlow);
            Assert.AreSame(stormDuration, configuration.StormDuration);
            Assert.AreEqual(structureName, configuration.StructureName);
            Assert.AreEqual(hydraulicBoundaryLocationName, configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual(structureNormalOrientation, configuration.StructureNormalOrientation);
            Assert.AreSame(allowedLevelIncreaseStorage, configuration.AllowedLevelIncreaseStorage);
            Assert.AreSame(storageStructureArea, configuration.StorageStructureArea);
            Assert.AreSame(flowWidthAtBottomProtection, configuration.FlowWidthAtBottomProtection);
            Assert.AreSame(criticalOvertoppingDischarge, configuration.CriticalOvertoppingDischarge);
            Assert.AreEqual(failureProbabilityStructureWithErosion, configuration.FailureProbabilityStructureWithErosion);
            Assert.AreSame(widthFlowApertures, configuration.WidthFlowApertures);
            Assert.AreEqual(foreshoreProfileName, configuration.ForeshoreProfileName);
            Assert.AreSame(waveReduction, configuration.WaveReduction);
        }

        public class SimpleStructureCalculationConfiguration : StructureCalculationConfiguration
        {
        }
    }
}