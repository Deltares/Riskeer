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
using System.Reflection;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Services
{
    [TestFixture]
    public class HydraRingConfigurationServiceTest
    {
        private static readonly string hydraRingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"HydraRing");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.Model);

            // Assert
            Assert.AreEqual("34-1", hydraRingConfigurationService.RingId);
            Assert.AreEqual(HydraRingUncertaintiesType.Model, hydraRingConfigurationService.UncertaintiesType);
        }

        [Test]
        public void AddHydraRingCalculationInput_DuplicateSectionId_ThrowsArgumentException()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.Model);
            var calculationInput1 = new HydraRingCalculationInputImplementation(1, 2);
            var calculationInput2 = new HydraRingCalculationInputImplementation(1, 3);

            hydraRingConfigurationService.AddHydraRingCalculationInput(calculationInput1);

            // Call
            TestDelegate test = () => hydraRingConfigurationService.AddHydraRingCalculationInput(calculationInput2);

            // Assert
            const string expectedMessage = "Section id is not unique";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void AddHydraRingCalculationInput_MultipleFailureMechanismTypes_ThrowsNotSupportedException()
        {
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.Model);
            var calculationInput1 = new HydraRingCalculationInputImplementation(1, 2);
            var calculationInput2 = new HydraRingCalculationInputImplementation(2, 3);
            calculationInput2.SetFailureMechanismType(HydraRingFailureMechanismType.DikesHeight);

            hydraRingConfigurationService.AddHydraRingCalculationInput(calculationInput1);

            // Call
            TestDelegate test = () => hydraRingConfigurationService.AddHydraRingCalculationInput(calculationInput2);

            // Assert
            const string expectedMessage = "Running calculations for multiple failure mechanism types is not supported.";
            var exception = Assert.Throws<NotSupportedException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GenerateDataBaseCreationScript_SingleHydraRingCalculationInputAddedToConfiguration_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.Model);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(1, 700004)
            {
                DesignTablesSetting = new DesignTablesSetting(6.6, 7.7)
            });

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (1, 2, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 2.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (1, 1, 1, 1, 4, 5, 0, 0, 0, 0, 6.6, 7.7, 1.1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Numerics];" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 1, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 22.2, 0, 0, NULL, NULL, NULL, 0, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 222.2, 2, 333.3, 444.4, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 2222.2, 2, 3333.3, NULL, NULL, NULL, 0, 4444.4, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 22222.2, 4, 33333.3, 44444.4, 55555.5, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 222222.2, 4, 333333.3, NULL, 555555.5, NULL, 0, 444444.4, 300);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (1, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (1, 2, 11.1, 22.2, 33.3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionFaultTreeModels] VALUES (1, 1, 1, 1, 1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 1, 1234);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Fetches];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Profiles];" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (1, 2, 11.1, 22.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                         "INSERT INTO [ForelandModels] VALUES (1, 1, 3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Forelands];" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (1, 2, 2.2, 3.3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ProbabilityAlternatives];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SetUpHeights];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalcWindDirections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Swells];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Areas];" + Environment.NewLine +
                                         "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Projects];" + Environment.NewLine +
                                         "INSERT INTO [Projects] VALUES (1, 'WTI 2017', 'Ringtoets calculation');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                         "INSERT INTO [Breakwaters] VALUES (1, 1, 99.9);" + Environment.NewLine;

            var databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                var creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void GenerateDataBaseCreationScript_MultipleHydraRingCalculationInputsAddedToConfiguration_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.Model);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(1, 700004)
            {
                DesignTablesSetting = new DesignTablesSetting(6.6, 7.7)
            });
            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(2, 700005)
            {
                DesignTablesSetting = new DesignTablesSetting(8.8, 9.9)
            });
            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(3, 700006)
            {
                DesignTablesSetting = new DesignTablesSetting(10.10, 11.11)
            });

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (1, 2, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (2, 1, 1, 2, 2, 0, 0, 0, 0, 700005, 700005, 100, 3.3, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (3, 1, 1, 3, 3, 0, 0, 0, 0, 700006, 700006, 100, 3.3, 2.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (1, 1, 1, 1, 4, 5, 0, 0, 0, 0, 6.6, 7.7, 1.1);" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (2, 1, 1, 1, 4, 5, 0, 0, 0, 0, 8.8, 9.9, 1.1);" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (3, 1, 1, 1, 4, 5, 0, 0, 0, 0, 10.1, 11.11, 1.1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Numerics];" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 1, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (2, 1, 1, 1, 1, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (3, 1, 1, 1, 1, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 22.2, 0, 0, NULL, NULL, NULL, 0, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 222.2, 2, 333.3, 444.4, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 2222.2, 2, 3333.3, NULL, NULL, NULL, 0, 4444.4, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 22222.2, 4, 33333.3, 44444.4, 55555.5, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 222222.2, 4, 333333.3, NULL, 555555.5, NULL, 0, 444444.4, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 22.2, 0, 0, NULL, NULL, NULL, 0, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 222.2, 2, 333.3, 444.4, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 2222.2, 2, 3333.3, NULL, NULL, NULL, 0, 4444.4, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 22222.2, 4, 33333.3, 44444.4, 55555.5, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 222222.2, 4, 333333.3, NULL, 555555.5, NULL, 0, 444444.4, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 22.2, 0, 0, NULL, NULL, NULL, 0, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 222.2, 2, 333.3, 444.4, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 2222.2, 2, 3333.3, NULL, NULL, NULL, 0, 4444.4, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 22222.2, 4, 33333.3, 44444.4, 55555.5, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 222222.2, 4, 333333.3, NULL, 555555.5, NULL, 0, 444444.4, 300);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (1, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (1, 2, 11.1, 22.2, 33.3);" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (2, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (2, 2, 11.1, 22.2, 33.3);" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (3, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (3, 2, 11.1, 22.2, 33.3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionFaultTreeModels] VALUES (1, 1, 1, 1, 1);" + Environment.NewLine +
                                         "INSERT INTO [SectionFaultTreeModels] VALUES (2, 1, 1, 1, 1);" + Environment.NewLine +
                                         "INSERT INTO [SectionFaultTreeModels] VALUES (3, 1, 1, 1, 1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 1, 1234);" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (2, 1, 1, 1, 1234);" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (3, 1, 1, 1, 1234);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Fetches];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Profiles];" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (1, 2, 11.1, 22.2);" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (2, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (2, 2, 11.1, 22.2);" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (3, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (3, 2, 11.1, 22.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                         "INSERT INTO [ForelandModels] VALUES (1, 1, 3);" + Environment.NewLine +
                                         "INSERT INTO [ForelandModels] VALUES (2, 1, 3);" + Environment.NewLine +
                                         "INSERT INTO [ForelandModels] VALUES (3, 1, 3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Forelands];" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (1, 2, 2.2, 3.3);" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (2, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (2, 2, 2.2, 3.3);" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (3, 1, 1.1, 2.2);" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (3, 2, 2.2, 3.3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ProbabilityAlternatives];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SetUpHeights];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalcWindDirections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Swells];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Areas];" + Environment.NewLine +
                                         "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Projects];" + Environment.NewLine +
                                         "INSERT INTO [Projects] VALUES (1, 'WTI 2017', 'Ringtoets calculation');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                         "INSERT INTO [Breakwaters] VALUES (1, 1, 99.9);" + Environment.NewLine +
                                         "INSERT INTO [Breakwaters] VALUES (2, 1, 99.9);" + Environment.NewLine +
                                         "INSERT INTO [Breakwaters] VALUES (3, 1, 99.9);" + Environment.NewLine;

            var databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                var creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        private class HydraRingCalculationInputImplementation : HydraRingCalculationInput
        {
            private readonly int sectionId;
            private HydraRingFailureMechanismType failureMechanismType;

            public HydraRingCalculationInputImplementation(int sectionId, int hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId)
            {
                this.sectionId = sectionId;

                failureMechanismType = HydraRingFailureMechanismType.AssessmentLevel;
            }

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return failureMechanismType;
                }
            }

            public override int CalculationTypeId
            {
                get
                {
                    return 4;
                }
            }

            public override int VariableId
            {
                get
                {
                    return 5;
                }
            }

            public override HydraRingSection Section
            {
                get
                {
                    return new HydraRingSection(sectionId, 2.2, 3.3);
                }
            }

            public override IEnumerable<HydraRingVariable> Variables
            {
                get
                {
                    yield return new HydraRingVariable(26, HydraRingDistributionType.Deterministic, 2.2, HydraRingDeviationType.Standard, 3.3, 4.4, 5.5);
                    yield return new HydraRingVariable(26, HydraRingDistributionType.Deterministic, 22.2, HydraRingDeviationType.Variation, 33.3, 44.4, 55.5);
                    yield return new HydraRingVariable(26, HydraRingDistributionType.Normal, 222.2, HydraRingDeviationType.Standard, 333.3, 444.4, 555.5);
                    yield return new HydraRingVariable(26, HydraRingDistributionType.Normal, 2222.2, HydraRingDeviationType.Variation, 3333.3, 4444.4, 5555.5);
                    yield return new HydraRingVariable(26, HydraRingDistributionType.LogNormal, 22222.2, HydraRingDeviationType.Standard, 33333.3, 44444.4, 55555.5);
                    yield return new HydraRingVariable(26, HydraRingDistributionType.LogNormal, 222222.2, HydraRingDeviationType.Variation, 333333.3, 444444.4, 555555.5);
                }
            }

            public override IEnumerable<HydraRingProfilePoint> ProfilePoints
            {
                get
                {
                    yield return new HydraRingProfilePointDerivative(1.1, 2.2, 3.3);
                    yield return new HydraRingProfilePointDerivative(11.1, 22.2, 33.3);
                }
            }

            public override IEnumerable<HydraRingForelandPoint> ForelandsPoints
            {
                get
                {
                    yield return new HydraRingForelandPoint(1.1, 2.2);
                    yield return new HydraRingForelandPoint(2.2, 3.3);
                }
            }

            public override HydraRingBreakWater BreakWater
            {
                get
                {
                    return new HydraRingBreakWater(1, 99.9);
                }
            }

            public override double Beta
            {
                get
                {
                    return 1.1;
                }
            }

            public void SetFailureMechanismType(HydraRingFailureMechanismType type)
            {
                failureMechanismType = type;
            }

            public override int? GetSubMechanismModelId(int subMechanismId)
            {
                return 1234;
            }
        }

        private class HydraRingProfilePointDerivative : HydraRingProfilePoint
        {
            private readonly double roughness;

            public HydraRingProfilePointDerivative(double x, double z, double roughness)
                : base(x, z)
            {
                this.roughness = roughness;
            }

            public override double Roughness
            {
                get
                {
                    return roughness;
                }
            }
        }
    }
}