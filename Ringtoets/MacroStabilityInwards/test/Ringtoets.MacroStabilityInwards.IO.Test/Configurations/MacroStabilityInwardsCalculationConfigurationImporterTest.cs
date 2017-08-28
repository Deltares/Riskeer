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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationImporterTest
    {
        private readonly string readerPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO,
                                                                        nameof(MacroStabilityInwardsCalculationConfigurationReader));

        private readonly string importerPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO,
                                                                          nameof(MacroStabilityInwardsCalculationConfigurationImporter));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter("",
                                                                                     new CalculationGroup(),
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<MacroStabilityInwardsCalculationConfigurationReader, MacroStabilityInwardsCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationConfigurationImporter("",
                                                                                                new CalculationGroup(),
                                                                                                null,
                                                                                                new MacroStabilityInwardsFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("availableHydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationConfigurationImporter("",
                                                                                                new CalculationGroup(),
                                                                                                Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     new MacroStabilityInwardsFailureMechanism());

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
        public void Import_SurfaceLineUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSurfaceLine.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De profielschematisatie 'Profielschematisatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelNotIntersectingWithSurfaceLine_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingNonIntersectingSurfaceLineAndSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel("Ondergrondmodel");
            stochasticSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel'doorkruist de profielschematisatie 'Profielschematisatie' niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilProfileUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSoilProfile.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel("Ondergrondmodel");
            stochasticSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De ondergrondschematisatie 'Ondergrondschematisatie' bestaat niet binnen het stochastische ondergrondmodel 'Ondergrondmodel'. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilProfileSpecifiedWithoutSoilModel_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingSoilProfileWithoutSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen stochastisch ondergrondmodel opgegeven bij ondergrondschematisatie 'Ondergrondschematisatie'. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ScenarioEmpty_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingEmptyScenario.xml");

            var calculationGroup = new CalculationGroup();

            var macroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     macroStabilityInwardsFailureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "In een berekening moet voor het scenario tenminste de relevantie of contributie worden opgegeven. " +
                                           "Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ScenarioWithContributionSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioContributionOnly.xml");

            var calculationGroup = new CalculationGroup();

            var macroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     macroStabilityInwardsFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "Calculation",
                Contribution = (RoundedDouble) 0.088
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertMacroStabilityInwardsCalculationScenario(expectedCalculation, (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ScenarioWithRevelantSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioRevelantOnly.xml");

            var calculationGroup = new CalculationGroup();

            var macroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     macroStabilityInwardsFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "Calculation",
                IsRelevant = false
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertMacroStabilityInwardsCalculationScenario(expectedCalculation, (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        [TestCase(false, "validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml")]
        [TestCase(true, "validConfigurationFullCalculationContainingAssessmentLevel.xml")]
        public void Import_ValidConfigurationWithValidHydraulicBoundaryData_DataAddedToModel(bool manualAssessmentLevel, string file)
        {
            // Setup
            string filePath = Path.Combine(readerPath, file);

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, new MacroStabilityInwardsSoilProfile1D("Ondergrondschematisatie", 0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(0)
            }));

            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel("Ondergrondmodel");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);
            stochasticSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HRlocatie", 10, 20);
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new[]
                                                                                     {
                                                                                         hydraulicBoundaryLocation
                                                                                     },
                                                                                     failureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "Calculation",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    UseAssessmentLevelManualInput = manualAssessmentLevel,
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile
                },
                IsRelevant = false,
                Contribution = (RoundedDouble) 0.088
            };
            if (manualAssessmentLevel)
            {
                expectedCalculation.InputParameters.AssessmentLevel = (RoundedDouble) 1.1;
            }

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertMacroStabilityInwardsCalculationScenario(expectedCalculation, (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        private static void AssertMacroStabilityInwardsCalculationScenario(MacroStabilityInwardsCalculationScenario expectedCalculation,
                                                                           MacroStabilityInwardsCalculationScenario actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
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
            Assert.AreSame(expectedCalculation.InputParameters.StochasticSoilModel, actualCalculation.InputParameters.StochasticSoilModel);
            Assert.AreSame(expectedCalculation.InputParameters.StochasticSoilProfile, actualCalculation.InputParameters.StochasticSoilProfile);

            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution, actualCalculation.Contribution.GetAccuracy());
        }
    }
}