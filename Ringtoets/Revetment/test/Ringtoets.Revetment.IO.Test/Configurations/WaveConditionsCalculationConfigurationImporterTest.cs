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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.IO.Configurations;

namespace Ringtoets.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationImporterTest
    {
        private readonly string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO, "WaveConditionsCalculationConfigurationImporter");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                "",
                new CalculationGroup(),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<WaveConditionsCalculationConfigurationReader, WaveConditionsCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                "",
                new CalculationGroup(),
                null,
                Enumerable.Empty<ForeshoreProfile>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                "",
                new CalculationGroup(),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("validConfigurationUpperRevetmentBoundBelowLowerRevetmentBound.xml",
            "Een waarde van '2,2' als ondergrens bekledingen is ongeldig. De bovengrens van de bekleding moet boven de ondergrens liggen.")]
        [TestCase("validConfigurationUpperWaterLevelBoundBelowLowerWaterLevelBound.xml",
            "Een waarde van '2,2' als ondergrens van de waterstanden is ongeldig. De bovengrens van de waterstanden moet boven de ondergrens liggen.")]
        [TestCase("validConfigurationOrientationOutOfRange.xml",
            "Een waarde van '380' als oriëntatie is ongeldig. De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen.")]
        public void Import_ValidConfigurationInvalidData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(path, file);

            var calculationGroup = new CalculationGroup();
            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationContainingUnknownHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De locatie met hydraulische randvoorwaarden 'HRlocatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ForeshoreProfileUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationContainingUnknownForeshoreProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het voorlandprofiel met ID 'Voorlandprofiel' bestaat niet. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_WaveReductionWithoutForeshoreProfile_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationContainingWaveReductionWithoutForeshoreProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen voorlandprofiel opgegeven om golfreductie parameters aan toe te voegen. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreTrueButProfileWithoutForeshoreGeometry_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationUseForeshoreTrueForeshoreProfileWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var foreshoreProfile = new TestForeshoreProfile("Voorlandprofiel");
            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    foreshoreProfile
                });

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het opgegeven voorlandprofiel 'Voorlandprofiel' heeft geen voorlandgeometrie en kan daarom niet gebruikt worden. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreFalseButProfileWithoutForeshoreGeometry_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationUseForeshoreFalseForeshoreProfileWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var foreshoreProfile = new TestForeshoreProfile("Voorlandprofiel");
            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    foreshoreProfile
                });

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new TestWaveConditionsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    UseForeshore = false,
                    Orientation = (RoundedDouble) 0,
                    ForeshoreProfile = foreshoreProfile
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertWaveConditionsCalculation(expectedCalculation, (ICalculation<WaveConditionsInput>) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ValidConfigurationWithValidData_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculation.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
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

            var importer = new WaveConditionsCalculationConfigurationImporter<TestWaveConditionsCalculation>(
                filePath,
                calculationGroup,
                new[]
                {
                    hydraulicBoundaryLocation
                },
                new[]
                {
                    foreshoreProfile
                });

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new TestWaveConditionsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
            AssertWaveConditionsCalculation(expectedCalculation, (ICalculation<WaveConditionsInput>) calculationGroup.Children[0]);
        }

        private static void AssertWaveConditionsCalculation(ICalculation<WaveConditionsInput> expectedCalculation, ICalculation<WaveConditionsInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreSame(expectedCalculation.InputParameters.HydraulicBoundaryLocation, actualCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedCalculation.InputParameters.UpperBoundaryRevetment, actualCalculation.InputParameters.UpperBoundaryRevetment);
            Assert.AreEqual(expectedCalculation.InputParameters.LowerBoundaryRevetment, actualCalculation.InputParameters.LowerBoundaryRevetment);
            Assert.AreEqual(expectedCalculation.InputParameters.UpperBoundaryWaterLevels, actualCalculation.InputParameters.UpperBoundaryWaterLevels);
            Assert.AreEqual(expectedCalculation.InputParameters.LowerBoundaryWaterLevels, actualCalculation.InputParameters.LowerBoundaryWaterLevels);
            Assert.AreEqual(expectedCalculation.InputParameters.StepSize, actualCalculation.InputParameters.StepSize);
            Assert.AreEqual(expectedCalculation.InputParameters.Orientation, actualCalculation.InputParameters.Orientation);
            Assert.AreEqual(expectedCalculation.InputParameters.ForeshoreProfile, actualCalculation.InputParameters.ForeshoreProfile);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
        }
    }
}