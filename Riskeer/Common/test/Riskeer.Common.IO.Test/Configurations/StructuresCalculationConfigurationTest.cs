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

using System;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;

namespace Riskeer.Common.IO.Test.Configurations
{
    [TestFixture]
    public class StructuresCalculationConfigurationTest
    {
        [Test]
        public void Constructor_WithoutName_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestStructuresCalculationConfiguration(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("value", exception.ParamName);
            Assert.AreEqual("Name is required for a calculation configuration.\r\nParameter name: value", exception.Message);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "name";

            // Call
            var configuration = new TestStructuresCalculationConfiguration(name);

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
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new TestStructuresCalculationConfiguration("valid name");

            // Call
            void Call() => calculationConfiguration.Name = null;

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("value", exception.ParamName);
            Assert.AreEqual("Name is required for a calculation configuration.\r\nParameter name: value", exception.Message);
        }

        private class TestStructuresCalculationConfiguration : StructuresCalculationConfiguration
        {
            public TestStructuresCalculationConfiguration(string name) : base(name) {}
        }
    }
}