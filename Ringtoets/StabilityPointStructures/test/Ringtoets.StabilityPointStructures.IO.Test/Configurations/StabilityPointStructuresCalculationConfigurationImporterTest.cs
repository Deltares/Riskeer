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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.IO.Configurations;

namespace Ringtoets.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationImporterTest
    {
        private readonly string importerPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityPointStructures.IO,
                                                                          nameof(StabilityPointStructuresCalculationConfigurationImporter));

        private static IEnumerable<TestCaseData> ValidConfigurationInvalidData
        {
            get
            {
                yield return new TestCaseData(
                    "validConfigurationAreaFlowAperturesVariationCoefficient.xml",
                    "Indien voor parameter 'doorstroomoppervlak' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.");

                yield return new TestCaseData(
                    "validConfigurationDrainCoefficientStandardDeviation.xml",
                    "Er kan geen spreiding voor stochast 'afvoercoefficient' opgegeven worden.");
                yield return new TestCaseData(
                    "validConfigurationDrainCoefficientVariationCoefficient.xml",
                    "Er kan geen spreiding voor stochast 'afvoercoefficient' opgegeven worden.");

                yield return new TestCaseData(
                    "validConfigurationInvalidFailureProbabilityStructureErosion.xml",
                    "Een waarde van '1,1' als faalkans gegeven erosie bodem is ongeldig. De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.");

                yield return new TestCaseData(
                    "validConfigurationFailureProbabilityRepairClosureWithoutStructure.xml",
                    "Er is geen kunstwerk opgegeven om faalkans herstel van gefaalde situatie aan toe te voegen.");
                yield return new TestCaseData(
                    "validConfigurationInvalidFailureProbabilityRepairClosureElement.xml",
                    "Een waarde van '1,1' als faalkans herstel van gefaalde situatie is ongeldig. De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.");

                yield return new TestCaseData(
                    "validConfigurationInflowModelTypeWithoutStructure.xml",
                    "Er is geen kunstwerk opgegeven om instroommodel aan toe te voegen.");

                yield return new TestCaseData(
                    "validConfigurationStructureNormalOrientationWithoutStructure.xml",
                    "Er is geen kunstwerk opgegeven om oriëntatie aan toe te voegen.");
                yield return new TestCaseData(
                    "validConfigurationInvalidStructureNormalOrientation.xml",
                    "Een waarde van '-12' als oriëntatie is ongeldig. De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen.");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new StabilityPointStructuresCalculationConfigurationImporter("",
                                                                                        new CalculationGroup(),
                                                                                        Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                        Enumerable.Empty<StabilityPointStructure>());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<StabilityPointStructuresCalculationConfigurationReader,
                StabilityPointStructuresCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityPointStructuresCalculationConfigurationImporter("",
                                                                                                   new CalculationGroup(),
                                                                                                   null,
                                                                                                   Enumerable.Empty<ForeshoreProfile>(),
                                                                                                   Enumerable.Empty<StabilityPointStructure>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityPointStructuresCalculationConfigurationImporter("",
                                                                                                   new CalculationGroup(),
                                                                                                   Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                                   null,
                                                                                                   Enumerable.Empty<StabilityPointStructure>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        public void Constructor_StructuresNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityPointStructuresCalculationConfigurationImporter("",
                                                                                                   new CalculationGroup(),
                                                                                                   Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                                   Enumerable.Empty<ForeshoreProfile>(),
                                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structures", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(ValidConfigurationInvalidData))]
        public void Import_ValidConfigurationInvalidData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var structure = new TestStabilityPointStructure("kunstwerk1");
            var foreshoreProfile = new TestForeshoreProfile("profiel 1");

            var importer = new StabilityPointStructuresCalculationConfigurationImporter(filePath,
                                                                                        calculationGroup,
                                                                                        Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                        new ForeshoreProfile[]
                                                                                        {
                                                                                            foreshoreProfile
                                                                                        },
                                                                                        new StabilityPointStructure[]
                                                                                        {
                                                                                            structure
                                                                                        });
            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [TestCase("validConfigurationEmptyCalculation.xml")]
        [TestCase("validConfigurationEmptyStochasts.xml")]
        [TestCase("validConfigurationEmptyStochastElements.xml")]
        [TestCase("validConfigurationEmptyWaveReduction.xml")]
        public void Import_EmptyConfigurations_DataAddedToModel(string file)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var structure = new TestStabilityPointStructure("kunstwerk1");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new[]
                {
                    structure
                });

            var expectedCalculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "Berekening 1"
            };

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculation<StabilityPointStructuresInput>) calculationGroup.Children[0]);
        }

        [TestCase("validConfigurationUnknownForeshoreProfile.xml",
            "Het voorlandprofiel 'unknown' bestaat niet.")]
        [TestCase("validConfigurationUnknownHydraulicBoundaryLocation.xml",
            "De locatie met hydraulische randvoorwaarden 'unknown' bestaat niet.")]
        [TestCase("validConfigurationUnknownStructure.xml",
            "Het kunstwerk 'unknown' bestaat niet.")]
        public void Import_ValidConfigurationUnknownData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();

            var importer = new StabilityPointStructuresCalculationConfigurationImporter(filePath,
                                                                                        calculationGroup,
                                                                                        Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                        Enumerable.Empty<StabilityPointStructure>());
            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_FullCalculationConfiguration_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validFullConfiguration.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1");
            var foreshoreProfile = new TestForeshoreProfile("profiel1", new List<Point2D>
            {
                new Point2D(0, 3)
            });
            var structure = new TestStabilityPointStructure("kunstwerk1");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
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
                new[]
                {
                    structure
                });

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            var expectedCalculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    Structure = structure,
                    ForeshoreProfile = foreshoreProfile,

                    BankWidth =
                    {
                        Mean = (RoundedDouble) 1.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    BreakWater =
                    {
                        Height = (RoundedDouble) 1.23,
                        Type = BreakWaterType.Dam
                    },

                    FactorStormDurationOpenStructure = (RoundedDouble) 0.002,
                    FailureProbabilityRepairClosure = 0.001,
                    FailureProbabilityStructureWithErosion = 0.0001,




                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    











                    StructureNormalOrientation = (RoundedDouble) 7,


                    
                    UseBreakWater = true,
                    UseForeshore = false,








                    StormDuration =
                    {
                        Mean = (RoundedDouble) 6.0
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) 1.10
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 15000,
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },

                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 80.5,
                        StandardDeviation = (RoundedDouble) 1
                    },
                    DrainCoefficient =
                    {
                        Mean = (RoundedDouble) 0.1
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 1.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculation<StabilityPointStructuresInput>)calculationGroup.Children[0]);
        }

        private static void AssertCalculation(StructuresCalculation<StabilityPointStructuresInput> expectedCalculation,
                                              StructuresCalculation<StabilityPointStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
            Assert.AreEqual(expectedCalculation.InputParameters.EvaluationLevel, actualCalculation.InputParameters.EvaluationLevel);

            Assert.AreEqual(expectedCalculation.InputParameters.FactorStormDurationOpenStructure, actualCalculation.InputParameters.FactorStormDurationOpenStructure);
            Assert.AreEqual(expectedCalculation.InputParameters.FailureProbabilityRepairClosure, actualCalculation.InputParameters.FailureProbabilityRepairClosure);
            Assert.AreEqual(expectedCalculation.InputParameters.FailureProbabilityStructureWithErosion, actualCalculation.InputParameters.FailureProbabilityStructureWithErosion);
            Assert.AreSame(expectedCalculation.InputParameters.ForeshoreProfile, actualCalculation.InputParameters.ForeshoreProfile);

            Assert.AreEqual(expectedCalculation.InputParameters.InflowModelType, actualCalculation.InputParameters.InflowModelType);
            Assert.AreEqual(expectedCalculation.InputParameters.LevellingCount, actualCalculation.InputParameters.LevellingCount);
            Assert.AreEqual(expectedCalculation.InputParameters.LoadSchematizationType, actualCalculation.InputParameters.LoadSchematizationType);
            Assert.AreEqual(expectedCalculation.InputParameters.ProbabilityCollisionSecondaryStructure, actualCalculation.InputParameters.ProbabilityCollisionSecondaryStructure);

            Assert.AreSame(expectedCalculation.InputParameters.Structure, actualCalculation.InputParameters.Structure);
            Assert.AreEqual(expectedCalculation.InputParameters.StructureNormalOrientation, actualCalculation.InputParameters.StructureNormalOrientation);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);

            Assert.AreEqual(expectedCalculation.InputParameters.VerticalDistance, actualCalculation.InputParameters.VerticalDistance);
            Assert.AreEqual(expectedCalculation.InputParameters.VolumicWeightWater, actualCalculation.InputParameters.VolumicWeightWater);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.AllowedLevelIncreaseStorage, actualCalculation.InputParameters.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.AreaFlowApertures, actualCalculation.InputParameters.AreaFlowApertures);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.BankWidth, actualCalculation.InputParameters.BankWidth);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ConstructiveStrengthLinearLoadModel, actualCalculation.InputParameters.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ConstructiveStrengthQuadraticLoadModel, actualCalculation.InputParameters.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.CriticalOvertoppingDischarge, actualCalculation.InputParameters.CriticalOvertoppingDischarge);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.DrainCoefficient, actualCalculation.InputParameters.DrainCoefficient);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.FailureCollisionEnergy, actualCalculation.InputParameters.FailureCollisionEnergy);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.FlowVelocityStructureClosable, actualCalculation.InputParameters.FlowVelocityStructureClosable);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.FlowWidthAtBottomProtection, actualCalculation.InputParameters.FlowWidthAtBottomProtection);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.InsideWaterLevel, actualCalculation.InputParameters.InsideWaterLevel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.InsideWaterLevelFailureConstruction, actualCalculation.InputParameters.InsideWaterLevelFailureConstruction);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.LevelCrestStructure, actualCalculation.InputParameters.LevelCrestStructure);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ModelFactorSuperCriticalFlow, actualCalculation.InputParameters.ModelFactorSuperCriticalFlow);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ShipMass, actualCalculation.InputParameters.ShipMass);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ShipVelocity, actualCalculation.InputParameters.ShipVelocity);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StabilityLinearLoadModel, actualCalculation.InputParameters.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StabilityQuadraticLoadModel, actualCalculation.InputParameters.StabilityQuadraticLoadModel);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StorageStructureArea, actualCalculation.InputParameters.StorageStructureArea);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StormDuration, actualCalculation.InputParameters.StormDuration);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.WidthFlowApertures, actualCalculation.InputParameters.WidthFlowApertures);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ThresholdHeightOpenWeir, actualCalculation.InputParameters.ThresholdHeightOpenWeir);
        }
    }
}