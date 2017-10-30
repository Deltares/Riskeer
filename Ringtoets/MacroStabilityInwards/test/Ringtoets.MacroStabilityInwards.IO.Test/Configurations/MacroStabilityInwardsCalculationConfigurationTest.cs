﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "some name";

            // Call
            var configuration = new MacroStabilityInwardsCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(configuration);
            Assert.AreEqual(name, configuration.Name);
            Assert.IsNull(configuration.AssessmentLevel);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.SurfaceLineName);
            Assert.IsNull(configuration.StochasticSoilModelName);
            Assert.IsNull(configuration.StochasticSoilProfileName);
            Assert.IsNull(configuration.Scenario);
            Assert.IsNull(configuration.DikeSoilScenario);
        }

        [Test]
        public void SimpleProperties_SetNewValues_NewValuesSet()
        {
            // Setup
            const string calculationName = "Name of the calculation";
            const double assessmentLevel = 1.1;
            const string hydraulicBoundaryLocation = "Name of the hydraulic boundary location";
            const string surfaceLine = "Name of the surface line";
            const string stochasticSoilModel = "Name of the stochastic soil model";
            const string stochasticSoilProfile = "Name of the stochastic soil profile";
            const ConfigurationDikeSoilScenario dikeSoilScenario = ConfigurationDikeSoilScenario.SandDikeOnSand;

            var scenarioConfiguration = new ScenarioConfiguration();

            // Call
            var configuration = new MacroStabilityInwardsCalculationConfiguration(calculationName)
            {
                AssessmentLevel = assessmentLevel,
                HydraulicBoundaryLocationName = hydraulicBoundaryLocation,
                SurfaceLineName = surfaceLine,
                StochasticSoilModelName = stochasticSoilModel,
                StochasticSoilProfileName = stochasticSoilProfile,
                Scenario = scenarioConfiguration,
                DikeSoilScenario = dikeSoilScenario
            };

            // Assert
            Assert.AreEqual(calculationName, configuration.Name);
            Assert.AreEqual(assessmentLevel, configuration.AssessmentLevel);
            Assert.AreEqual(hydraulicBoundaryLocation, configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual(surfaceLine, configuration.SurfaceLineName);
            Assert.AreEqual(stochasticSoilModel, configuration.StochasticSoilModelName);
            Assert.AreEqual(stochasticSoilProfile, configuration.StochasticSoilProfileName);
            Assert.AreSame(scenarioConfiguration, configuration.Scenario);
            Assert.AreEqual(dikeSoilScenario, configuration.DikeSoilScenario);
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new MacroStabilityInwardsCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}