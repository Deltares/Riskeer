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

using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Riskeer.HydraRing.IO.TestUtil.Test
{
    [TestFixture]
    public class ReadHydraulicLocationConfigurationDatabaseSettingsTestFactoryTest
    {
        [Test]
        public void Create_Always_ExpectedValues()
        {
            // Call
            ReadHydraulicLocationConfigurationDatabaseSettings settings = ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create();

            // Assert
            Assert.AreEqual("scenarioName", settings.ScenarioName);
            Assert.AreEqual(1337, settings.Year);
            Assert.AreEqual("scope", settings.Scope);
            Assert.AreEqual("seaLevel", settings.SeaLevel);
            Assert.AreEqual("riverDischarge", settings.RiverDischarge);
            Assert.AreEqual("lakeLevel", settings.LakeLevel);
            Assert.AreEqual("windDirection", settings.WindDirection);
            Assert.AreEqual("windSpeed", settings.WindSpeed);
            Assert.AreEqual("comment", settings.Comment);
        }
    }
}