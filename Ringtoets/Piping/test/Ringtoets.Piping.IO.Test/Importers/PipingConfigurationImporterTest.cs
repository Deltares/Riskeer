﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
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

        private static IEnumerable<TestCaseData> NestedOrNaNData
        {
            get
            {
                yield return new TestCaseData("validConfigurationNesting.xml", GetNestedData());
                yield return new TestCaseData("validConfigurationCalculationContainingNaNs.xml", GetNaNData());                
            }
        }

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
        public void Import_InvalidFile_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = Path.Combine(path, "invalidConfigurationCalculationContainingEmptyStrings.xml");
            var importer = new PipingConfigurationImporter(filePath,
                                                           new CalculationGroup(),
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           new PipingFailureMechanism());

            // Call
            bool importSuccesful = true;
            Action call = () => importSuccesful = importer.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith($"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie voor de berekeningen beschrijft is niet geldig.", msgs[0]);
            });
            
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

            var expectedCalculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = "Calculation",
                InputParameters =
                {
                    UseAssessmentLevelManualInput = manualAssessmentLevel,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 2.2,
                    ExitPointL = (RoundedDouble) 3.3,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble) 4.4,
                        StandardDeviation = (RoundedDouble) 5.5
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble) 6.6,
                        StandardDeviation = (RoundedDouble) 7.7
                    }
                }
            };
            if (manualAssessmentLevel)
            {
                expectedCalculation.InputParameters.AssessmentLevel = (RoundedDouble) 1.1;
            }

            AssertCalculationGroup(new CalculationGroup
            {
                Children =
                {
                    expectedCalculation
                }
            }, calculationGroup);
        }

        [Test]
        [TestCaseSource(nameof(NestedOrNaNData))]
        public void Import_NestedOrEmptyCalculations_DataAddedToModel(string file, CalculationGroup expectedCalculationGroup)
        {
            // Setup
            string filePath = Path.Combine(path, file);

            var calculationGroup = new CalculationGroup();
            var pipingFailureMechanism = new PipingFailureMechanism();

            var importer = new PipingConfigurationImporter(filePath,
                                                           calculationGroup,
                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                           pipingFailureMechanism);

            // Call
            bool succesful = importer.Import();

            // Assert
            Assert.IsTrue(succesful);
            AssertCalculationGroup(expectedCalculationGroup, calculationGroup);
        }

        private static CalculationGroup GetNestedData()
        {
            return new CalculationGroup("Root", false)
            {
                Children =
                {
                    new CalculationGroup("Group 1", false)
                    {
                        Children =
                        {
                            new PipingCalculationScenario(new GeneralPipingInput())
                            {
                                Name = "Calculation 3"
                            }
                        }
                    },
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 1"
                    },
                    new CalculationGroup("Group 2", false)
                    {
                        Children =
                        {
                            new CalculationGroup("Group 4", false)
                            {
                                Children =
                                {
                                    new PipingCalculationScenario(new GeneralPipingInput())
                                    {
                                        Name = "Calculation 5"
                                    }
                                }
                            },
                            new PipingCalculationScenario(new GeneralPipingInput())
                            {
                                Name = "Calculation 4"
                            }
                        }
                    },
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 2"
                    },
                    new CalculationGroup("Group 3", false)
                }
            };
        }

        private static CalculationGroup GetNaNData()
        {
            return new CalculationGroup("Root", false)
            {
                Children =
                {
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation",
                        InputParameters =
                        {
                            UseAssessmentLevelManualInput = true,
                            AssessmentLevel = RoundedDouble.NaN,
                            EntryPointL = RoundedDouble.NaN,
                            ExitPointL = RoundedDouble.NaN,
                            PhreaticLevelExit =
                            {
                                Mean = RoundedDouble.NaN,
                                StandardDeviation = RoundedDouble.NaN
                            },
                            DampingFactorExit =
                            {
                                Mean = RoundedDouble.NaN,
                                StandardDeviation = RoundedDouble.NaN
                            }
                        }
                    }
                }
            };
        }

        private static void AssertCalculationGroup(CalculationGroup expectedCalculationGroup, CalculationGroup actualCalculationGroup)
        {
            Assert.AreEqual(expectedCalculationGroup.Children.Count, actualCalculationGroup.Children.Count);

            for (var i = 0; i < expectedCalculationGroup.Children.Count; i++)
            {
                Assert.AreEqual(expectedCalculationGroup.Children[i].Name, actualCalculationGroup.Children[i].Name);
                var innerCalculationgroup = expectedCalculationGroup.Children[i] as CalculationGroup;
                var innerCalculation = expectedCalculationGroup.Children[i] as PipingCalculationScenario;

                if (innerCalculationgroup != null)
                {
                    AssertCalculationGroup(innerCalculationgroup, (CalculationGroup) actualCalculationGroup.Children[i]);
                }

                if (innerCalculation != null)
                {
                    AssertCalculation(innerCalculation, (PipingCalculationScenario) actualCalculationGroup.Children[i]);
                }
            }
        }

        private static void AssertCalculation(PipingCalculationScenario expectedCalculation, PipingCalculationScenario actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.InputParameters.UseAssessmentLevelManualInput, actualCalculation.InputParameters.UseAssessmentLevelManualInput);
            if (expectedCalculation.InputParameters.UseAssessmentLevelManualInput)
            {
                Assert.AreEqual(expectedCalculation.InputParameters.AssessmentLevel.Value, actualCalculation.InputParameters.AssessmentLevel.Value);
            }
            else
            {
                Assert.AreSame(expectedCalculation.InputParameters.HydraulicBoundaryLocation, actualCalculation.InputParameters.HydraulicBoundaryLocation);
            }
            Assert.AreSame(expectedCalculation.InputParameters.SurfaceLine, actualCalculation.InputParameters.SurfaceLine);
            Assert.AreEqual(expectedCalculation.InputParameters.EntryPointL.Value, actualCalculation.InputParameters.EntryPointL.Value);
            Assert.AreEqual(expectedCalculation.InputParameters.ExitPointL.Value, actualCalculation.InputParameters.ExitPointL.Value);
            Assert.AreSame(expectedCalculation.InputParameters.StochasticSoilModel, actualCalculation.InputParameters.StochasticSoilModel);
            Assert.AreSame(expectedCalculation.InputParameters.StochasticSoilProfile, actualCalculation.InputParameters.StochasticSoilProfile);
            Assert.AreEqual(expectedCalculation.InputParameters.PhreaticLevelExit.Mean.Value, actualCalculation.InputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(expectedCalculation.InputParameters.PhreaticLevelExit.StandardDeviation.Value, actualCalculation.InputParameters.PhreaticLevelExit.StandardDeviation.Value);
            Assert.AreEqual(expectedCalculation.InputParameters.DampingFactorExit.Mean.Value, actualCalculation.InputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(expectedCalculation.InputParameters.DampingFactorExit.StandardDeviation.Value, actualCalculation.InputParameters.DampingFactorExit.StandardDeviation.Value);
        }

        private class ExpectedProgressNotification
        {
            public string Text { get; set; }
            public int CurrentStep { get; set; }
            public int MaxNrOfSteps { get; set; }
        }
    }
}