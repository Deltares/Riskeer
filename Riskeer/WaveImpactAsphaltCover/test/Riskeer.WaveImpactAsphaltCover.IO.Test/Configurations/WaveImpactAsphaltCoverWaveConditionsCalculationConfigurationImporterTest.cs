﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Configurations;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.IO.Configurations;

namespace Riskeer.WaveImpactAsphaltCover.IO.Test.Configurations
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationImporterTest
    {
        private readonly string path = TestHelper.GetTestDataPath(TestDataPath.Riskeer.WaveImpactAsphaltCover.IO,
                                                                  nameof(WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationImporter));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationImporter(
                "",
                new CalculationGroup(),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationConfigurationImporter<WaveImpactAsphaltCoverWaveConditionsCalculation,
                WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader, WaveConditionsCalculationConfiguration>>(importer);
        }

        [Test]
        public void Import_ValidConfigurationWithValidData_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculation.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20);
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, new BreakWater(BreakWaterType.Caisson, 0), new ForeshoreProfile.ConstructionProperties
            {
                Id = "Voorlandprofiel",
                Name = "VoorlandProfielName"
            });

            var importer = new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new[]
                {
                    hydraulicBoundaryLocation
                },
                new[]
                {
                    foreshoreProfile
                },
                new FailureMechanismContribution(0.1, 0.05),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    WaterLevelType = WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    LowerBoundaryRevetment = (RoundedDouble) 2,
                    UpperBoundaryWaterLevels = (RoundedDouble) 9,
                    LowerBoundaryWaterLevels = (RoundedDouble) 4,
                    StepSize = WaveConditionsInputStepSize.Half,
                    ForeshoreProfile = foreshoreProfile,
                    Orientation = (RoundedDouble) 5.5,
                    UseForeshore = false,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 6.6,
                        Type = BreakWaterType.Caisson
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertWaveConditionsCalculation(expectedCalculation, (WaveImpactAsphaltCoverWaveConditionsCalculation) calculationGroup.Children[0]);
        }

        private static void AssertWaveConditionsCalculation(WaveImpactAsphaltCoverWaveConditionsCalculation expectedCalculation,
                                                            WaveImpactAsphaltCoverWaveConditionsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreSame(expectedCalculation.InputParameters.HydraulicBoundaryLocation, actualCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.AreSame(expectedCalculation.InputParameters.CalculationsTargetProbability, actualCalculation.InputParameters.CalculationsTargetProbability);
            Assert.AreEqual(expectedCalculation.InputParameters.WaterLevelType, actualCalculation.InputParameters.WaterLevelType);
            Assert.AreEqual(expectedCalculation.InputParameters.UpperBoundaryRevetment, actualCalculation.InputParameters.UpperBoundaryRevetment);
            Assert.AreEqual(expectedCalculation.InputParameters.LowerBoundaryRevetment, actualCalculation.InputParameters.LowerBoundaryRevetment);
            Assert.AreEqual(expectedCalculation.InputParameters.UpperBoundaryWaterLevels, actualCalculation.InputParameters.UpperBoundaryWaterLevels);
            Assert.AreEqual(expectedCalculation.InputParameters.LowerBoundaryWaterLevels, actualCalculation.InputParameters.LowerBoundaryWaterLevels);
            Assert.AreEqual(expectedCalculation.InputParameters.StepSize, actualCalculation.InputParameters.StepSize);
            Assert.AreEqual(expectedCalculation.InputParameters.Orientation, actualCalculation.InputParameters.Orientation);
            Assert.AreSame(expectedCalculation.InputParameters.ForeshoreProfile, actualCalculation.InputParameters.ForeshoreProfile);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
        }
    }
}