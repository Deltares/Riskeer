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
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Importers;
using Ringtoets.GrassCoverErosionInwards.IO.Readers;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.Importers
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationImporterTest
    {
        private readonly string path = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.GrassCoverErosionInwards.IO, 
            nameof(GrassCoverErosionInwardsCalculationConfigurationImporter));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                "",
                new CalculationGroup(),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Assert
            Assert.IsInstanceOf<
                CalculationConfigurationImporter<
                    GrassCoverErosionInwardsCalculationConfigurationReader, 
                    ReadGrassCoverErosionInwardsCalculation>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                "",
                new CalculationGroup(),
                null,
                Enumerable.Empty<DikeProfile>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_DikeProfilesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                "",
                new CalculationGroup(),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dikeProfiles", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_ValidConfigurationInvalidOrientation_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationInvalidOrientation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = "Een waarde van '380' als oriëntatie is ongeldig. De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen. " +
                                     "Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_ValidConfigurationInvalidCriticalWaveReductionMean_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationInvalidCriticalWaveReductionMean.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = "Een waarde van '-1' als gemiddelde voor het kritisch overslagdebiet is ongeldig. Gemiddelde moet groter zijn dan 0. " +
                                     "Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_ValidConfigurationInvalidCriticalWaveReductionStandardDeviation_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationInvalidCriticalWaveReductionStandardDeviation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = "Een waarde van '-2,1' als standaardafwijking voor het kritisch overslagdebiet is ongeldig. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0. " +
                                     "Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationUnknownHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De locatie met hydraulische randvoorwaarden 'HRlocatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_DikeProfileUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationUnknownDikeProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het dijkprofiel 'Dijkprofiel' bestaat niet. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_WaveReductionWithoutDikeProfile_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationWaveReductionWithoutDikeProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen dijkprofiel opgegeven om golfreductie parameters aan toe te voegen. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_DikeHeightWithoutDikeProfile_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationDikeHeightWithoutDikeProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen dijkprofiel opgegeven om de dijkhoogte aan toe te voegen. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreButProfileWithoutGeometry_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationUseForeshoreWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var dikeProfile = new TestDikeProfile("Dijkprofiel");
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    dikeProfile
                });

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het opgegeven dijkprofiel 'Dijkprofiel' heeft geen geometrie en kan daarom niet gebruikt worden. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ValidConfigurationWithoutForeshoreProfileNotUsed_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationNotUseForeshoreWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
            var dikeProfile = new TestDikeProfile("Dijkprofiel");

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new[]
                {
                    hydraulicBoundaryLocation
                },
                new[]
                {
                    dikeProfile
                });

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (GrassCoverErosionInwardsCalculation)calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ValidConfigurationWithValidData_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculation.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new[]
            {
                new RoughnessPoint(new Point2D(0, 0), 2.1),
                new RoughnessPoint(new Point2D(1, 1), 3.9),
                new RoughnessPoint(new Point2D(2, 2), 5.2)
            }, new[]
            {
                new Point2D(1, 0),
                new Point2D(3, 4),
                new Point2D(6, 5)
            }, new BreakWater(BreakWaterType.Caisson, 0), new DikeProfile.ConstructionProperties
            {
                Id = "id",
                Name = "Dijkprofiel",
                DikeHeight = 3.45
            });

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new[]
                {
                    hydraulicBoundaryLocation
                },
                new[]
                {
                    dikeProfile
                });

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    Orientation = (RoundedDouble) 5.5,
                    UseForeshore = true,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 6.6,
                        Type = BreakWaterType.Caisson
                    },
                    CriticalFlowRate =
                    {
                        Mean = (RoundedDouble) 2.0,
                        StandardDeviation = (RoundedDouble) 1.1
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (GrassCoverErosionInwardsCalculation)calculationGroup.Children[0]);
        }

        private void AssertCalculation(GrassCoverErosionInwardsCalculation expectedCalculation, GrassCoverErosionInwardsCalculation actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreSame(expectedCalculation.InputParameters.HydraulicBoundaryLocation, actualCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedCalculation.InputParameters.Orientation, actualCalculation.InputParameters.Orientation);
            Assert.AreEqual(expectedCalculation.InputParameters.DikeProfile, actualCalculation.InputParameters.DikeProfile);
            Assert.AreEqual(expectedCalculation.InputParameters.DikeHeightCalculationType, actualCalculation.InputParameters.DikeHeightCalculationType);
            Assert.AreEqual(expectedCalculation.InputParameters.DikeHeight, actualCalculation.InputParameters.DikeHeight);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.CriticalFlowRate, actualCalculation.InputParameters.CriticalFlowRate);
        }
    }
}