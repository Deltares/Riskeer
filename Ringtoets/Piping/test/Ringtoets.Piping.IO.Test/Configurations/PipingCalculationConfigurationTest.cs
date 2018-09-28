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
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Piping.IO.Configurations;

namespace Ringtoets.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationTest
    {
        [Test]
        public void Constructor_WithoutName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithName_PropertiesAreDefault()
        {
            // Setup 
            const string name = "some name";

            // Call
            var readPipingCalculation = new PipingCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readPipingCalculation);
            Assert.AreEqual(name, readPipingCalculation.Name);
            Assert.IsNull(readPipingCalculation.AssessmentLevel);
            Assert.IsNull(readPipingCalculation.HydraulicBoundaryLocationName);
            Assert.IsNull(readPipingCalculation.SurfaceLineName);
            Assert.IsNull(readPipingCalculation.EntryPointL);
            Assert.IsNull(readPipingCalculation.ExitPointL);
            Assert.IsNull(readPipingCalculation.StochasticSoilModelName);
            Assert.IsNull(readPipingCalculation.StochasticSoilProfileName);
            Assert.IsNull(readPipingCalculation.PhreaticLevelExit);
            Assert.IsNull(readPipingCalculation.DampingFactorExit);
            Assert.IsNull(readPipingCalculation.Scenario);
        }

        [Test]
        public void Constructor_WithNameAndProperties_PropertiesAsExpected()
        {
            // Setup
            const string calculationName = "Name of the calculation";
            const double assessmentLevel = 1.1;
            const string hydraulicBoundaryLocation = "Name of the hydraulic boundary location";
            const string surfaceLine = "Name of the surface line";
            const double entryPointL = 2.2;
            const double exitPointL = 3.3;
            const string stochasticSoilModel = "Name of the stochastic soil model";
            const string stochasticSoilProfile = "Name of the stochastic soil profile";

            var phreaticLevelExit = new StochastConfiguration();
            var dampingFactorExit = new StochastConfiguration();
            var scenarioConfiguration = new ScenarioConfiguration();

            // Call
            var readPipingCalculation = new PipingCalculationConfiguration(calculationName)
            {
                AssessmentLevel = assessmentLevel,
                HydraulicBoundaryLocationName = hydraulicBoundaryLocation,
                SurfaceLineName = surfaceLine,
                EntryPointL = entryPointL,
                ExitPointL = exitPointL,
                StochasticSoilModelName = stochasticSoilModel,
                StochasticSoilProfileName = stochasticSoilProfile,
                PhreaticLevelExit = phreaticLevelExit,
                DampingFactorExit = dampingFactorExit,
                Scenario = scenarioConfiguration
            };

            // Assert
            Assert.AreEqual(calculationName, readPipingCalculation.Name);
            Assert.AreEqual(assessmentLevel, readPipingCalculation.AssessmentLevel);
            Assert.AreEqual(hydraulicBoundaryLocation, readPipingCalculation.HydraulicBoundaryLocationName);
            Assert.AreEqual(surfaceLine, readPipingCalculation.SurfaceLineName);
            Assert.AreEqual(entryPointL, readPipingCalculation.EntryPointL);
            Assert.AreEqual(exitPointL, readPipingCalculation.ExitPointL);
            Assert.AreEqual(stochasticSoilModel, readPipingCalculation.StochasticSoilModelName);
            Assert.AreEqual(stochasticSoilProfile, readPipingCalculation.StochasticSoilProfileName);
            Assert.AreSame(phreaticLevelExit, readPipingCalculation.PhreaticLevelExit);
            Assert.AreSame(dampingFactorExit, readPipingCalculation.DampingFactorExit);
            Assert.AreSame(scenarioConfiguration, readPipingCalculation.Scenario);
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new PipingCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}