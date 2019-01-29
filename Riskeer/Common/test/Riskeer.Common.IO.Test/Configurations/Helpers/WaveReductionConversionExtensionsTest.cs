// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;

namespace Riskeer.Common.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class WaveReductionConversionExtensionsTest
    {
        [Test]
        public void SetConfigurationForeshoreProfileDependentProperties_ConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var structureInput = new SimpleStructuresInput();

            // Call
            TestDelegate call = () => WaveReductionConversionExtensions.SetConfigurationForeshoreProfileDependentProperties(null, structureInput);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("configuration", paramName);
        }

        [Test]
        public void SetConfigurationForeshoreProfileDependentProperties_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var configuration = new SimpleStructuresCalculationConfiguration();

            // Call
            TestDelegate call = () => configuration.SetConfigurationForeshoreProfileDependentProperties<StructureBase>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("input", paramName);
        }

        [Test]
        public void SetConfigurationForeshoreProfileDependentProperties_WithoutForeshoreProfile_DoesNotUpdate()
        {
            // Setup
            var configuration = new SimpleStructuresCalculationConfiguration();
            var structureInput = new SimpleStructuresInput();

            // Call
            configuration.SetConfigurationForeshoreProfileDependentProperties(structureInput);

            // Assert
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.WaveReduction);
        }

        [Test]
        public void SetConfigurationForeshoreProfileDependentProperties_WithForeshoreProfile_UpdatesConfiguration()
        {
            // Setup
            var random = new Random(6543);
            var configuration = new SimpleStructuresCalculationConfiguration();
            var structureInput = new SimpleStructuresInput
            {
                ForeshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        Enumerable.Empty<Point2D>(),
                                                        new BreakWater(
                                                            BreakWaterType.Dam,
                                                            random.NextDouble()),
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = "id",
                                                            Name = "profile"
                                                        }),
                UseBreakWater = random.NextBoolean(),
                UseForeshore = random.NextBoolean()
            };

            // Call
            configuration.SetConfigurationForeshoreProfileDependentProperties(structureInput);

            // Assert
            Assert.AreEqual("id", configuration.ForeshoreProfileId);
            WaveReductionConfiguration waveReduction = configuration.WaveReduction;
            Assert.AreEqual(structureInput.UseBreakWater, waveReduction.UseBreakWater);
            Assert.AreEqual(structureInput.UseForeshore, waveReduction.UseForeshoreProfile);
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, waveReduction.BreakWaterType);
            Assert.AreEqual(structureInput.BreakWater.Height, waveReduction.BreakWaterHeight);
        }

        [Test]
        public void SetConfigurationForeshoreProfileDependentProperties_WithForeshoreProfileInvalidBreakwaterType_UpdatesConfiguration()
        {
            // Setup
            var random = new Random(6543);
            var configuration = new SimpleStructuresCalculationConfiguration();
            var structureInput = new SimpleStructuresInput
            {
                ForeshoreProfile = new TestForeshoreProfile(new BreakWater(
                                                                (BreakWaterType) 999,
                                                                random.NextDouble())),
                UseBreakWater = random.NextBoolean(),
                UseForeshore = random.NextBoolean()
            };

            // Call
            configuration.SetConfigurationForeshoreProfileDependentProperties(structureInput);

            // Assert
            Assert.AreEqual(structureInput.ForeshoreProfile.Id, configuration.ForeshoreProfileId);
            WaveReductionConfiguration waveReduction = configuration.WaveReduction;
            Assert.AreEqual(structureInput.UseBreakWater, waveReduction.UseBreakWater);
            Assert.AreEqual(structureInput.UseForeshore, waveReduction.UseForeshoreProfile);
            Assert.IsNull(waveReduction.BreakWaterType);
            Assert.AreEqual(structureInput.BreakWater.Height, waveReduction.BreakWaterHeight);
        }

        private class SimpleStructuresInput : StructuresInputBase<StructureBase>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }

        private class SimpleStructuresCalculationConfiguration : StructuresCalculationConfiguration
        {
            public SimpleStructuresCalculationConfiguration() : base(string.Empty) {}
        }
    }
}