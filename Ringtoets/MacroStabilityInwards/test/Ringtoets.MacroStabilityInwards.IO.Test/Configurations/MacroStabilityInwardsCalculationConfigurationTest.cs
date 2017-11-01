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
            Assert.IsNull(configuration.WaterLevelRiverAverage);
            Assert.IsNull(configuration.DrainageConstructionPresent);
            Assert.IsNull(configuration.XCoordinateDrainageConstruction);
            Assert.IsNull(configuration.ZCoordinateDrainageConstruction);
            Assert.IsNull(configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNull(configuration.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNull(configuration.AdjustPhreaticLine3And4ForUplift);
            Assert.IsNull(configuration.PhreaticLine2);
            Assert.IsNull(configuration.PhreaticLine3);
            Assert.IsNull(configuration.PhreaticLine4);
            Assert.IsNull(configuration.LocationInputDaily);
            Assert.IsNull(configuration.SlipPlaneMinimumDepth);
            Assert.IsNull(configuration.SlipPlaneMinimumLength);
            Assert.IsNull(configuration.MaximumSliceWidth);
            Assert.IsNull(configuration.CreateZones);
            Assert.IsNull(configuration.GridDeterminationType);
            Assert.IsNull(configuration.MoveGrid);
            Assert.IsNull(configuration.TangentLineDeterminationType);
            Assert.IsNull(configuration.TangentLineZTop);
            Assert.IsNull(configuration.TangentLineZBottom);
            Assert.IsNull(configuration.TangentLineNumber);
            Assert.IsNull(configuration.LeftGrid);
            Assert.IsNull(configuration.RightGrid);
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

            const double waterLevelRiverAverage = 1.5;
            const bool drainageConstructionPresent = true;
            const double xCoordinateDrainageConstruction = 0.6;
            const double zCoordinateDrainageConstruction = 0.7;

            const double minimumLevelPhreaticLineAtDikeTopRiver = 0.8;
            const double minimumLevelPhreaticLineAtDikeTopPolder = 0.9;

            const bool adjustPhreaticLine3And4ForUplift = true;
            var phreaticLine2 = new PhreaticLineConfiguration();
            var phreaticLine3 = new PhreaticLineConfiguration();
            var phreaticLine4 = new PhreaticLineConfiguration();
            var locationInputDaily = new MacroStabilityInwardsLocationInputConfiguration();

            const double slipPlaneMinimumDepth = 2.2;
            const double slipPlaneMinimumLength = 3.3;
            const double maximumSliceWidth = 4.4;
            const bool createZones = true;

            var scenarioConfiguration = new ScenarioConfiguration();
            const bool movegrid = true;
            const ConfigurationGridDeterminationType gridDeterminationType = ConfigurationGridDeterminationType.Manual;
            const ConfigurationTangentLineDeterminationType tangentLineDeterminationType = ConfigurationTangentLineDeterminationType.Specified;
            const double tangentLineZTop = 5.5;
            const double tangentLineZBottom = 6.6;
            const int tangentLineNumber = 7;
            var leftGrid = new MacroStabilityInwardsGridConfiguration();
            var rightGrid = new MacroStabilityInwardsGridConfiguration();

            // Call
            var configuration = new MacroStabilityInwardsCalculationConfiguration(calculationName)
            {
                AssessmentLevel = assessmentLevel,
                HydraulicBoundaryLocationName = hydraulicBoundaryLocation,
                SurfaceLineName = surfaceLine,
                StochasticSoilModelName = stochasticSoilModel,
                StochasticSoilProfileName = stochasticSoilProfile,
                Scenario = scenarioConfiguration,
                DikeSoilScenario = dikeSoilScenario,
                WaterLevelRiverAverage = waterLevelRiverAverage,
                DrainageConstructionPresent = drainageConstructionPresent,
                XCoordinateDrainageConstruction = xCoordinateDrainageConstruction,
                ZCoordinateDrainageConstruction = zCoordinateDrainageConstruction,
                MinimumLevelPhreaticLineAtDikeTopRiver = minimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = minimumLevelPhreaticLineAtDikeTopPolder,
                AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift,
                PhreaticLine2 = phreaticLine2,
                PhreaticLine3 = phreaticLine3,
                PhreaticLine4 = phreaticLine4,
                LocationInputDaily = locationInputDaily,
                SlipPlaneMinimumDepth = slipPlaneMinimumDepth,
                SlipPlaneMinimumLength = slipPlaneMinimumLength,
                MaximumSliceWidth = maximumSliceWidth,
                CreateZones = createZones,
                MoveGrid = movegrid,
                GridDeterminationType = gridDeterminationType,
                TangentLineDeterminationType = tangentLineDeterminationType,
                TangentLineZTop = tangentLineZTop,
                TangentLineZBottom = tangentLineZBottom,
                TangentLineNumber = tangentLineNumber,
                LeftGrid = leftGrid,
                RightGrid = rightGrid
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
            Assert.AreEqual(waterLevelRiverAverage, configuration.WaterLevelRiverAverage);
            Assert.AreEqual(drainageConstructionPresent, configuration.DrainageConstructionPresent);
            Assert.AreEqual(xCoordinateDrainageConstruction, configuration.XCoordinateDrainageConstruction);
            Assert.AreEqual(zCoordinateDrainageConstruction, configuration.ZCoordinateDrainageConstruction);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, configuration.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, configuration.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, configuration.AdjustPhreaticLine3And4ForUplift);
            Assert.AreSame(phreaticLine2, configuration.PhreaticLine2);
            Assert.AreSame(phreaticLine3, configuration.PhreaticLine3);
            Assert.AreSame(phreaticLine4, configuration.PhreaticLine4);
            Assert.AreSame(locationInputDaily, configuration.LocationInputDaily);
            Assert.AreEqual(slipPlaneMinimumDepth, configuration.SlipPlaneMinimumDepth);
            Assert.AreEqual(slipPlaneMinimumLength, configuration.SlipPlaneMinimumLength);
            Assert.AreEqual(maximumSliceWidth, configuration.MaximumSliceWidth);
            Assert.AreEqual(createZones, configuration.CreateZones);
            Assert.AreEqual(movegrid, configuration.MoveGrid);
            Assert.AreEqual(gridDeterminationType, configuration.GridDeterminationType);
            Assert.AreEqual(tangentLineDeterminationType, configuration.TangentLineDeterminationType);
            Assert.AreEqual(tangentLineZTop, configuration.TangentLineZTop);
            Assert.AreEqual(tangentLineZBottom, configuration.TangentLineZBottom);
            Assert.AreEqual(tangentLineNumber, configuration.TangentLineNumber);
            Assert.AreSame(leftGrid, configuration.LeftGrid);
            Assert.AreSame(rightGrid, configuration.RightGrid);
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