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
using System.Reflection;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Integration
{
    [TestFixture]
    public class HydraRingConfigurationServiceIntegrationTest
    {
        private static readonly string hydraRingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"HydraRing");

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithAssessmentLevelCalculationInput_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.All);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new AssessmentLevelCalculationInput(1, 700004, 10000));

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 1, 1, 1, 2, 26, 0, 0, 0, 0, 2.18, 4.18, 3.71901648545568);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 1, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 1, 1, 1, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Fetches];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Profiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Forelands];" + Environment.NewLine +
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
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithWaveHeightCalculationInput_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.All);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new WaveHeightCalculationInput(1, 700004, 10000));

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (3, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 11, 1, 1, 2, 28, 0, 0, 0, 0, 0.11, 2.11, 3.71901648545568);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 11, 1, 1, 11, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 11, 1, 1, 28, 0, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 11, 1, 1, 11);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Fetches];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Profiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Forelands];" + Environment.NewLine +
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
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithOvertoppingCalculationInput_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            var hydraRingSection = new HydraRingSection(1, 2.2, 3.3);
            const double dikeHeight = 11.11;
            const double modelFactorCriticalOvertopping = 1;
            const double factorFnMean = 4.75;
            const double factorFnStandardDeviation = 0.5;
            const double hydraRingFactorFnMean = 2.6;
            const double hydraRingFactorFnStandardDeviation = 0.35;
            const double hydraRingmodelFactorOvertopping = 1;
            const double criticalOvertoppingMean = 22.22;
            const double criticalOvertoppingStandardDeviation = 33.33;
            const double hydraRingModelFactorFrunupMean = 1;
            const double hydraRingModelFactorFrunupStandardDeviation = 0.07;
            const double hydraRingExponentModelFactorShallowMean = 0.92;
            const double hydraRingExponentModelFactorShallowStandardDeviation = 0.24;
            var profilePoints = new List<HydraRingRoughnessProfilePoint>
            {
                new HydraRingRoughnessProfilePoint(1.1, 2.2, 3.3)
            };
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new OvertoppingCalculationInput(
                    hydraulicBoundaryLocationId, hydraRingSection,
                    dikeHeight,
                    modelFactorCriticalOvertopping, factorFnMean, factorFnStandardDeviation, hydraRingFactorFnMean, hydraRingFactorFnStandardDeviation,
                    hydraRingmodelFactorOvertopping, criticalOvertoppingMean, criticalOvertoppingStandardDeviation, hydraRingModelFactorFrunupMean,
                    hydraRingModelFactorFrunupStandardDeviation, hydraRingExponentModelFactorShallowMean, hydraRingExponentModelFactorShallowStandardDeviation,
                    profilePoints, forelandPoints, breakWater));

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (3, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 2.2);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 101, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 102, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 103, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 1, 11.11, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 8, 1, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 10, 0, 2, 4.75, 0.5, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 11, 0, 2, 2.6, 0.35, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 12, 1, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 17, 0, 4, 22.22, 33.33, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 120, 0, 2, 1, 0.07, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 123, 0, 2, 0.92, 0.24, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            "INSERT INTO [CalculationProfiles] VALUES (1, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 101, 1, 1, 1017);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 102, 94);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 103, 95);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Fetches];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Profiles];" + Environment.NewLine +
                                            "INSERT INTO [Profiles] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                            "INSERT INTO [ForelandModels] VALUES (1, 101, 3);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Forelands];" + Environment.NewLine +
                                            "INSERT INTO [Forelands] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
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
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithDikeHeightCalculationInput_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            var hydraRingSection = new HydraRingSection(1, 2.2, 3.3);
            const double norm = 1000;
            const double modelFactorCriticalOvertopping = 1;
            const double factorFnMean = 4.75;
            const double factorFnStandardDeviation = 0.5;
            const double hydraRingFactorFnMean = 2.6;
            const double hydraRingFactorFnStandardDeviation = 0.35;
            const double hydraRingmodelFactorOvertopping = 1;
            const double criticalOvertoppingMean = 22.22;
            const double criticalOvertoppingStandardDeviation = 33.33;
            const double hydraRingModelFactorFrunupMean = 1;
            const double hydraRingModelFactorFrunupStandardDeviation = 0.07;
            const double hydraRingExponentModelFactorShallowMean = 0.92;
            const double hydraRingExponentModelFactorShallowStandardDeviation = 0.24;
            var profilePoints = new List<HydraRingRoughnessProfilePoint>
            {
                new HydraRingRoughnessProfilePoint(1.1, 2.2, 3.3)
            };
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new DikeHeightCalculationInput(hydraulicBoundaryLocationId, norm, hydraRingSection,
                                               modelFactorCriticalOvertopping, factorFnMean, factorFnStandardDeviation, hydraRingFactorFnMean, hydraRingFactorFnStandardDeviation,
                                               hydraRingmodelFactorOvertopping, criticalOvertoppingMean, criticalOvertoppingStandardDeviation, hydraRingModelFactorFrunupMean,
                                               hydraRingModelFactorFrunupStandardDeviation, hydraRingExponentModelFactorShallowMean, hydraRingExponentModelFactorShallowStandardDeviation,
                                               profilePoints, forelandPoints, breakWater));

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (3, 1, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 2.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (1, 101, 1, 1, 2, 1, 0, 0, 0, 0, 3.48, 5.48, 3.09023230616781);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Numerics];" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 102, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 103, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 1, 0, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 8, 1, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 10, 0, 2, 4.75, 0.5, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 11, 0, 2, 2.6, 0.35, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 12, 1, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 17, 0, 4, 22.22, 33.33, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 120, 0, 2, 1, 0.07, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 123, 0, 2, 0.92, 0.24, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (1, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionFaultTreeModels] VALUES (1, 101, 1, 1, 1017);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 102, 94);" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 103, 95);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Fetches];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Profiles];" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                         "INSERT INTO [ForelandModels] VALUES (1, 101, 3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Forelands];" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
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
                                         "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

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
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithStructuresOvertoppingCalculationInput_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            var hydraRingSection = new HydraRingSection(1, 2.2, 3.3);
            const double gravitationalAcceleration = 9.81;
            const double modelFactorOvertoppingMean = 0.09;
            const double modelFactorOvertoppingStandardDeviation = 0.06;
            const double levelOfCrestOfStructureStandardDeviation = 0.05;
            const double modelFactorOvertoppingSupercriticalFlowMean = 1.1;
            const double modelFactorOvertoppingSupercriticalFlowStandardDeviation = 0.03;
            const double allowableIncreaseOfLevelForStorageStandardDeviation = 0.1;
            const double modelFactorForStorageVolumeMean = 1.0;
            const double modelFactorForStorageVolumeStandardDeviation = 0.2;
            const double storageStructureAreaStandardDeviation = 0.1;
            const double modelFactorForIncomingFlowVolume = 1;
            const double flowWidthAtBottomProtectionStandardDeviation = 0.05;
            const double criticalOvertoppingDischargeMeanStandardDeviation = 0.15;
            const double widthOfFlowAperturesStandardDeviation = 0.05;
            const double stormDurationMean = 7.5;
            const double stormDurationStandardDeviation = 0.25;

            const double levelOfCrestOfStructureMean = 1.1;
            const double orientationOfTheNormalOfTheStructure = 2.2;
            const double allowableIncreaseOfLevelForStorageMean = 3.3;
            const double storageStructureAreaMean = 4.4;
            const double flowWidthAtBottomProtectionMean = 5.5;
            const double criticalOvertoppingDischargeMean = 6.6;
            const double failureProbabilityOfStructureGivenErosion = 7.7;
            const double widthOfFlowAperturesMean = 8.8;
            const double deviationOfTheWaveDirection = 9.9;

            hydraRingConfigurationService.AddHydraRingCalculationInput(new StructuresOvertoppingCalculationInput(hydraulicBoundaryLocationId, hydraRingSection, gravitationalAcceleration,
                                                                                                                 modelFactorOvertoppingMean, modelFactorOvertoppingStandardDeviation,
                                                                                                                 levelOfCrestOfStructureMean, levelOfCrestOfStructureStandardDeviation,
                                                                                                                 orientationOfTheNormalOfTheStructure,
                                                                                                                 modelFactorOvertoppingSupercriticalFlowMean, modelFactorOvertoppingSupercriticalFlowStandardDeviation,
                                                                                                                 allowableIncreaseOfLevelForStorageMean, allowableIncreaseOfLevelForStorageStandardDeviation,
                                                                                                                 modelFactorForStorageVolumeMean, modelFactorForStorageVolumeStandardDeviation,
                                                                                                                 storageStructureAreaMean, storageStructureAreaStandardDeviation,
                                                                                                                 modelFactorForIncomingFlowVolume,
                                                                                                                 flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                                                                                 criticalOvertoppingDischargeMean, criticalOvertoppingDischargeMeanStandardDeviation,
                                                                                                                 failureProbabilityOfStructureGivenErosion,
                                                                                                                 widthOfFlowAperturesMean, widthOfFlowAperturesStandardDeviation,
                                                                                                                 deviationOfTheWaveDirection,
                                                                                                                 stormDurationMean, stormDurationStandardDeviation));
            string expectedCreationScript =
                "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                "INSERT INTO [HydraulicModels] VALUES (3, 1, 'WTI 2017');" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Sections];" + Environment.NewLine +
                "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 2.2);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [DesignTables];" + Environment.NewLine +
                "INSERT INTO [DesignTables] VALUES (1, 110, 1, 1, 1, 60, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Numerics];" + Environment.NewLine +
                "INSERT INTO [Numerics] VALUES (1, 110, 1, 1, 421, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 10000, 20000, 0.1, -6, 6, 25);" + Environment.NewLine +
                "INSERT INTO [Numerics] VALUES (1, 110, 1, 1, 422, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 10000, 20000, 0.1, -6, 6, 25);" + Environment.NewLine +
                "INSERT INTO [Numerics] VALUES (1, 110, 1, 1, 423, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 10000, 20000, 0.1, -6, 6, 25);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [VariableDatas];" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 58, 9.81, 0, 0, NULL, NULL, NULL, 0, 0, 99000);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 59, 0, 4, 0.09, 0.06, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 60, 0, 2, 1.1, 0.05, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 61, 2.2, 0, 0, NULL, NULL, NULL, 0, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 62, 0, 2, 1.1, 0.03, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 94, 0, 4, 3.3, 0.1, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 95, 0, 4, 1, 0.2, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 96, 0, 4, 4.4, NULL, NULL, NULL, 0, 0.1, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 97, 1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 103, 0, 2, 5.5, 0.05, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 104, 0, 4, 6.6, NULL, NULL, NULL, 0, 0.15, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 105, 0, 2, 7.7, 0, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 106, 0, 2, 8.8, NULL, NULL, NULL, 0, 0.05, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 107, 9.9, 0, 0, NULL, NULL, NULL, 0, 0, 99000);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 108, 0, 4, 7.5, NULL, NULL, NULL, 0, 0.25, 999999);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                "INSERT INTO [SectionFaultTreeModels] VALUES (1, 110, 1, 1, 4404);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Fetches];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [AreaPoints];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [PresentationSections];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Profiles];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [ForelandModels];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Forelands];" + Environment.NewLine +
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
                "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithWaveConditionsCosineCalculationInput_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new WaveConditionsCosineCalculationInput(1,
                                                         56.23,
                                                         hydraulicBoundaryLocationId,
                                                         10000,
                                                         forelandPoints,
                                                         breakWater,
                                                         3.3,
                                                         4.4,
                                                         5.5,
                                                         6.6));

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (3, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 56.23, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 3, 1, 1, 7, 114, 0, 0, 0, 0, 1, 5, 3.71901648545568);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 3, 1, 1, 5, 4, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 113, 3.3, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 114, 1, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 115, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 116, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 119, 6.6, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 3, 1, 1, 6);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 5, 71);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Fetches];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Profiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                            "INSERT INTO [ForelandModels] VALUES (1, 3, 3);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Forelands];" + Environment.NewLine +
                                            "INSERT INTO [Forelands] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
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
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithWaveConditionsTrapezoidCalculationInput_ReturnsExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new WaveConditionsTrapezoidCalculationInput(1,
                                                            86.48,
                                                            hydraulicBoundaryLocationId,
                                                            10000,
                                                            forelandPoints,
                                                            breakWater,
                                                            3.3,
                                                            4.4,
                                                            5.5,
                                                            6.6,
                                                            7.7));

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (3, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 86.48, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 3, 1, 1, 7, 114, 0, 0, 0, 0, 1, 5, 3.71901648545568);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 3, 1, 1, 5, 4, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 113, 3.3, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 114, 1, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 115, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 116, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 117, 6.6, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 3, 1, 1, 118, 7.7, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 3, 1, 1, 6);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 5, 70);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Fetches];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Profiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                            "INSERT INTO [ForelandModels] VALUES (1, 3, 3);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Forelands];" + Environment.NewLine +
                                            "INSERT INTO [Forelands] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
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
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDataBaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }
    }
}