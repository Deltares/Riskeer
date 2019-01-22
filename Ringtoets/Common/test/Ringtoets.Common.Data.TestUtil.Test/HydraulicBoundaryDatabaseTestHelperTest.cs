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
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseTestHelperTest
    {
        [Test]
        public void SetHydraulicBoundaryLocationConfigurationSettings_Always_SetsExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            // Assert
            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual("some\\Path\\ToHlcd", settings.FilePath);
            Assert.AreEqual("ScenarioName", settings.ScenarioName);
            Assert.AreEqual(1337, settings.Year);
            Assert.AreEqual("Scope", settings.Scope);
            Assert.AreEqual("SeaLevel", settings.SeaLevel);
            Assert.AreEqual("RiverDischarge", settings.RiverDischarge);
            Assert.AreEqual("LakeLevel", settings.LakeLevel);
            Assert.AreEqual("WindDirection", settings.WindDirection);
            Assert.AreEqual("WindSpeed", settings.WindSpeed);
            Assert.AreEqual("Comment", settings.Comment);
        }
    }
}