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
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class PipingConfigurationImporterTest
    {
        private readonly string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingConfigurationReader");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new PipingConfigurationImporter("",
                                                           new CalculationGroup(),
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<CalculationGroup>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingConfigurationImporter("",
                                                                      new CalculationGroup(),
                                                                      null,
                                                                      new PipingFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingConfigurationImporter("",
                                                                      new CalculationGroup(),
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new PipingConfigurationImporter(filePath,
                                                           new CalculationGroup(),
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           new PipingFailureMechanism());

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam. " + Environment.NewLine +
                                  "Er is geen berekening configuratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "I_dont_exist");
            var importer = new PipingConfigurationImporter(filePath,
                                                           new CalculationGroup(),
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           new PipingFailureMechanism());

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand bestaat niet. " + Environment.NewLine +
                                  "Er is geen berekening configuratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccesful);
        }

        [Test]
        [TestCase("Inlezen")]
        [TestCase("Valideren")]
        public void Import_CancelingImport_CancelImportAndLog(string expectedProgressMessage)
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            string filePath = Path.Combine(path, "validConfigurationNesting.xml");
            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           new PipingFailureMechanism());

            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(expectedProgressMessage))
                {
                    importer.Cancel();
                }
            });

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Berekening configuratie importeren afgebroken. Geen data ingelezen.", 1);
            CollectionAssert.IsEmpty(calculationGroup.Children);
            Assert.IsFalse(importSuccesful);
        }

        [Test]
        public void GivenImport_WhenImporting_ThenExpectedProgressMessagesGenerated()
        {
            // Given
            string filePath = Path.Combine(path, "validConfigurationNesting.xml");
            var importer = new PipingConfigurationImporter(filePath,
                                                           new CalculationGroup(),
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           new PipingFailureMechanism());

            var expectedProgressMessages = new[]
            {
                new ExpectedProgressNotification
                {
                    Text = "Inlezen berekening configuratie.", CurrentStep = 1, MaxNrOfSteps = 3
                },
                new ExpectedProgressNotification
                {
                    Text = "Valideren berekening configuratie.", CurrentStep = 2, MaxNrOfSteps = 3
                },
                new ExpectedProgressNotification
                {
                    Text = "Geïmporteerde data toevoegen aan het toetsspoor.", CurrentStep = 3, MaxNrOfSteps = 3
                }
            };

            var progressChangedCallCount = 0;
            importer.SetProgressChanged((description, step, steps) =>
            {
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].MaxNrOfSteps, steps);
                progressChangedCallCount++;
            });

            // When
            importer.Import();

            // Then
            Assert.AreEqual(expectedProgressMessages.Length, progressChangedCallCount);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationInvalid_LogMessageAndContinueImport()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            string filePath = Path.Combine(path, "validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml");
            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           new PipingFailureMechanism());

            // Call
            bool succesful = false;
            Action call = () => succesful = importer.Import();

            // Assert
            const string expectedMessage = "De locatie met hydraulische randvoorwaarden 'HRlocatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(succesful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_SurfaceLineInvalid_LogMessageAndContinueImport()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            string filePath = Path.Combine(path, "validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml");
            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           new[]
                                                           {
                                                               new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20),
                                                           },
                                                           new PipingFailureMechanism());

            // Call
            bool succesful = false;
            Action call = () => succesful = importer.Import();

            // Assert
            const string expectedMessage = "De profielschematisatie 'Profielschematisatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(succesful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelInvalid_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Profielschematisatie"
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.5, 2.3, 8.0),
                new Point3D(6.9, 2.0, 2.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           new[]
                                                           {
                                                               hydraulicBoundaryLocation
                                                           },
                                                           pipingFailureMechanism);

            // Call
            bool succesful = false;
            Action call = () => succesful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(succesful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelNotIntersectingWithSurfaceLine_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Profielschematisatie"
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });
            var stochasticSoilModel = new StochasticSoilModel(1, "Ondergrondmodel", "Segment");
            stochasticSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "path");            

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           new[]
                                                           {
                                                               hydraulicBoundaryLocation
                                                           },
                                                           pipingFailureMechanism);

            // Call
            bool succesful = false;
            Action call = () => succesful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel'doorkruist de profielschematisatie 'Profielschematisatie' niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(succesful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilProfileInvalid_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Profielschematisatie"
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var stochasticSoilModel = new StochasticSoilModel(1, "Ondergrondmodel", "Segment");
            stochasticSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "path");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           new[]
                                                           {
                                                               hydraulicBoundaryLocation
                                                           },
                                                           pipingFailureMechanism);

            // Call
            bool succesful = false;
            Action call = () => succesful = importer.Import();

            // Assert
            const string expectedMessage = "De ondergrondschematisatie 'Ondergrondschematisatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(succesful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml", false)]
        [TestCase("validConfigurationFullCalculationContainingAssessmentLevel.xml", true)]
        public void Import_ValidData_DataAddedToModel(string file, bool manualAssessmentLevel)
        {
            // Setup
            string filePath = Path.Combine(path, file);

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Profielschematisatie"
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var stochasticSoilProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("Ondergrondschematisatie", 0, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var stochasticSoilModel = new StochasticSoilModel(1, "Ondergrondmodel", "Segment");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);
            stochasticSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "path");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           new[]
                                                           {
                                                               hydraulicBoundaryLocation
                                                           },
                                                           pipingFailureMechanism);

            // Call
            bool succesful = importer.Import();

            // Assert
            Assert.IsTrue(succesful);
            Assert.AreEqual(1, calculationGroup.Children.Count);
            PipingCalculation calculation = calculationGroup.Children[0] as PipingCalculation;

            Assert.AreEqual("Calculation", calculation.Name);
            Assert.AreEqual(manualAssessmentLevel, calculation.InputParameters.UseAssessmentLevelManualInput);
            if (manualAssessmentLevel)
            {
                Assert.AreEqual(1.1, calculation.InputParameters.AssessmentLevel.Value);
            }
            else
            {
                Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
            }
            Assert.AreSame(surfaceLine, calculation.InputParameters.SurfaceLine);
            Assert.AreEqual(2.2, calculation.InputParameters.EntryPointL.Value);
            Assert.AreEqual(3.3, calculation.InputParameters.ExitPointL.Value);
            Assert.AreSame(stochasticSoilModel, calculation.InputParameters.StochasticSoilModel);
            Assert.AreSame(stochasticSoilProfile, calculation.InputParameters.StochasticSoilProfile);
            Assert.AreEqual(4.4, calculation.InputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(5.5, calculation.InputParameters.PhreaticLevelExit.StandardDeviation.Value);
            Assert.AreEqual(6.6, calculation.InputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(7.7, calculation.InputParameters.DampingFactorExit.StandardDeviation.Value);
        }

        private class ExpectedProgressNotification
        {
            public string Text { get; set; }
            public int CurrentStep { get; set; }
            public int MaxNrOfSteps { get; set; }
        }
    }
}