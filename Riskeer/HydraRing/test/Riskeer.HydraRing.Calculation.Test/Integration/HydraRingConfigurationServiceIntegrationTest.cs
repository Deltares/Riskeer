﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Util;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.Data.Input.Piping;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.Data.Settings;
using Riskeer.HydraRing.Calculation.Services;

namespace Riskeer.HydraRing.Calculation.Test.Integration
{
    [TestFixture]
    public class HydraRingConfigurationServiceIntegrationTest
    {
        private static readonly string hydraRingDirectory = Path.Combine(
            AssemblyHelper.GetApplicationDirectory(), "Standalone",
            "Deltares", $"HydraRing-{HydraRingFileConstants.HydraRingVersionNumber}");

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForAssessmentLevelCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new AssessmentLevelCalculationInput(1, 700004, 1.0 / 10000)
            {
                DesignTablesSetting = new DesignTablesSetting(1.1, 2.2),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        1, new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25)
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(1)
            });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 1, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 1, 1, 1, 9, 26, 0, 0, 0, 0, 1.1, 2.2, 3.71901648545571);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 1, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForWaveHeightCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new WaveHeightCalculationInput(1, 700004, 1.0 / 10000)
            {
                DesignTablesSetting = new DesignTablesSetting(1.1, 2.2),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        11, new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25)
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(1)
            });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 11, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 11, 1, 1, 9, 28, 0, 0, 0, 0, 1.1, 2.2, 3.71901648545571);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 11, 1, 1, 11, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForOvertoppingCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            const double dikeHeight = 4.4;
            const double modelFactorCriticalOvertopping = 5.5;
            const double factorFbMean = 6.6;
            const double factorFbStandardDeviation = 7.7;
            const double factorFbLowerBoundary = 17.7;
            const double factorFbUpperBoundary = 18.8;
            const double factorFnMean = 8.8;
            const double factorFnStandardDeviation = 9.9;
            const double factorFnLowerBoundary = 19.9;
            const double factorFnUpperBoundary = 20.0;
            const double modelFactorOvertopping = 10.10;
            const double criticalOvertoppingMean = 11.11;
            const double criticalOvertoppingStandardDeviation = 12.12;
            const double modelFactorFrunupMean = 13.13;
            const double modelFactorFrunupStandardDeviation = 14.14;
            const double modelFactorFrunupLowerBoundary = 21.1;
            const double modelFactorFrunupUpperBoundary = 22.2;
            const double exponentModelFactorShallowMean = 15.15;
            const double exponentModelFactorShallowStandardDeviation = 16.16;
            const double exponentModelFactorShallowLowerBoundary = 23.3;
            const double exponentModelFactorShallowUpperBoundary = 24.4;
            var profilePoints = new List<HydraRingRoughnessProfilePoint>
            {
                new HydraRingRoughnessProfilePoint(1.1, 2.2, 3.3)
            };
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);
            var numericsSetting = new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new OvertoppingCalculationInput(
                    hydraulicBoundaryLocationId, sectionNormal,
                    profilePoints, forelandPoints, breakWater,
                    dikeHeight,
                    modelFactorCriticalOvertopping,
                    factorFbMean, factorFbStandardDeviation,
                    factorFbLowerBoundary, factorFbUpperBoundary,
                    factorFnMean, factorFnStandardDeviation,
                    factorFnLowerBoundary, factorFnUpperBoundary,
                    modelFactorOvertopping,
                    criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                    modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                    modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary,
                    exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation,
                    exponentModelFactorShallowLowerBoundary, exponentModelFactorShallowUpperBoundary)
                {
                    DesignTablesSetting = new DesignTablesSetting(17.17, 18.18),
                    NumericsSettings = new Dictionary<int, NumericsSetting>
                    {
                        {
                            102, numericsSetting
                        },
                        {
                            103, numericsSetting
                        }
                    },
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 101, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 101, 1, 1, 1, 1, 0, 0, 0, 0, 17.17, 18.18, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 102, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 103, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 1, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 8, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 10, 0, 19, 6.6, 7.7, 17.7, 18.8, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 11, 0, 19, 8.8, 9.9, 19.9, 20, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 12, 10.1, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 17, 0, 4, 11.11, 12.12, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 120, 0, 19, 13.13, 14.14, 21.1, 22.2, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 123, 0, 19, 15.15, 16.16, 23.3, 24.4, 1, 0, 300);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForDikeHeightCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            const double targetProbability = 1.0 / 1000;
            const double modelFactorCriticalOvertopping = 4.4;
            const double factorFbMean = 5.5;
            const double factorFbStandardDeviation = 6.6;
            const double factorFbLowerBoundary = 7.7;
            const double factorFbUpperBoundary = 8.8;
            const double factorFnMean = 9.9;
            const double factorFnStandardDeviation = 10.0;
            const double factorFnLowerBoundary = 11.1;
            const double factorFnUpperBoundary = 12.2;
            const double modelFactorOvertopping = 13.3;
            const double criticalOvertoppingMean = 14.4;
            const double criticalOvertoppingStandardDeviation = 15.5;
            const double modelFactorFrunupMean = 16.6;
            const double modelFactorFrunupStandardDeviation = 17.7;
            const double modelFactorFrunupLowerBoundary = 18.8;
            const double modelFactorFrunupUpperBoundary = 19.9;
            const double exponentModelFactorShallowMean = 20.0;
            const double exponentModelFactorShallowStandardDeviation = 21.1;
            const double exponentModelFactorShallowLowerBoundary = 22.2;
            const double exponentModelFactorShallowUpperBoundary = 23.3;

            var profilePoints = new List<HydraRingRoughnessProfilePoint>
            {
                new HydraRingRoughnessProfilePoint(1.1, 2.2, 3.3)
            };
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);
            var numericsSetting = new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new DikeHeightCalculationInput(hydraulicBoundaryLocationId, targetProbability, sectionNormal,
                                               profilePoints, forelandPoints, breakWater,
                                               modelFactorCriticalOvertopping,
                                               factorFbMean, factorFbStandardDeviation,
                                               factorFbLowerBoundary, factorFbUpperBoundary,
                                               factorFnMean, factorFnStandardDeviation,
                                               factorFnLowerBoundary, factorFnUpperBoundary,
                                               modelFactorOvertopping,
                                               criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                                               modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                               modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary,
                                               exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation,
                                               exponentModelFactorShallowLowerBoundary, exponentModelFactorShallowUpperBoundary)
                {
                    DesignTablesSetting = new DesignTablesSetting(16.16, 17.17),
                    NumericsSettings = new Dictionary<int, NumericsSetting>
                    {
                        {
                            102, numericsSetting
                        },
                        {
                            103, numericsSetting
                        }
                    },
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 101, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 101, 1, 1, 9, 1, 0, 0, 0, 0, 16.16, 17.17, 3.09023230616781);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 102, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 103, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 1, 0, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 8, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 10, 0, 19, 5.5, 6.6, 7.7, 8.8, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 11, 0, 19, 9.9, 10, 11.1, 12.2, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 12, 13.3, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 17, 0, 4, 14.4, 15.5, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 120, 0, 19, 16.6, 17.7, 18.8, 19.9, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 123, 0, 19, 20, 21.1, 22.2, 23.3, 1, 0, 300);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForOvertoppingRateCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            const double targetProbability = 1.0 / 1000;
            const double dikeHeight = 4.4;
            const double modelFactorCriticalOvertopping = 5.5;
            const double factorFbMean = 6.6;
            const double factorFbStandardDeviation = 7.7;
            const double factorFbLowerBoundary = 8.8;
            const double factorFbUpperBoundary = 9.9;
            const double factorFnMean = 10.0;
            const double factorFnStandardDeviation = 11.1;
            const double factorFnLowerBoundary = 12.2;
            const double factorFnUpperBoundary = 13.3;
            const double modelFactorOvertopping = 14.4;
            const double modelFactorFrunupMean = 15.5;
            const double modelFactorFrunupStandardDeviation = 16.6;
            const double modelFactorFrunupLowerBoundary = 17.7;
            const double modelFactorFrunupUpperBoundary = 18.8;
            const double exponentModelFactorShallowMean = 19.9;
            const double exponentModelFactorShallowStandardDeviation = 20.0;
            const double exponentModelFactorShallowLowerBoundary = 21.1;
            const double exponentModelFactorShallowUpperBoundary = 22.2;

            var profilePoints = new List<HydraRingRoughnessProfilePoint>
            {
                new HydraRingRoughnessProfilePoint(1.1, 2.2, 3.3)
            };
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);
            var numericsSetting = new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new OvertoppingRateCalculationInput(hydraulicBoundaryLocationId, targetProbability, sectionNormal,
                                                    profilePoints, forelandPoints, breakWater,
                                                    dikeHeight,
                                                    modelFactorCriticalOvertopping,
                                                    factorFbMean, factorFbStandardDeviation,
                                                    factorFbLowerBoundary, factorFbUpperBoundary,
                                                    factorFnMean, factorFnStandardDeviation,
                                                    factorFnLowerBoundary, factorFnUpperBoundary,
                                                    modelFactorOvertopping,
                                                    modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                    modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary,
                                                    exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation,
                                                    exponentModelFactorShallowLowerBoundary, exponentModelFactorShallowUpperBoundary)
                {
                    DesignTablesSetting = new DesignTablesSetting(15.15, 16.16),
                    NumericsSettings = new Dictionary<int, NumericsSetting>
                    {
                        {
                            102, numericsSetting
                        },
                        {
                            103, numericsSetting
                        }
                    },
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 101, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 101, 1, 1, 2, 17, 0, 0, 0, 0, 15.15, 16.16, 3.09023230616781);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 102, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 101, 1, 1, 103, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 1, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 8, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 10, 0, 19, 6.6, 7.7, 8.8, 9.9, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 11, 0, 19, 10, 11.1, 12.2, 13.3, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 12, 14.4, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 17, 0, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 120, 0, 19, 15.5, 16.6, 17.7, 18.8, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 101, 1, 1, 123, 0, 19, 19.9, 20, 21.1, 22.2, 1, 0, 300);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresOvertoppingCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);
            var numericsSetting = new NumericsSetting(11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6, 6, 25);

            const double gravitationalAcceleration = 4.4;
            const double modelFactorOvertoppingMean = 5.5;
            const double modelFactorOvertoppingStandardDeviation = 6.6;
            const double levelCrestStructureMean = 7.7;
            const double levelCrestStructureStandardDeviation = 8.8;
            const double structureNormalOrientation = 9.9;
            const double modelFactorSuperCriticalFlowMean = 10.10;
            const double modelFactorSuperCriticalFlowStandardDeviation = 11.11;
            const double allowedLevelIncreaseStorageMean = 12.12;
            const double allowedLevelIncreaseStorageStandardDeviation = 13.13;
            const double modelFactorStorageVolumeMean = 14.14;
            const double modelFactorStorageVolumeStandardDeviation = 15.15;
            const double storageStructureAreaMean = 16.16;
            const double storageStructureAreaVariation = 17.17;
            const double modelFactorInflowVolume = 18.18;
            const double flowWidthAtBottomProtectionMean = 19.19;
            const double flowWidthAtBottomProtectionStandardDeviation = 20.20;
            const double criticalOvertoppingDischargeMean = 21.21;
            const double criticalOvertoppingDischargeVariation = 22.22;
            const double failureProbabilityStructureWithErosion = 23.23;
            const double widthFlowAperturesMean = 24.24;
            const double widthFlowAperturesStandardDeviation = 25.25;
            const double deviationWaveDirection = 26.26;
            const double stormDurationMean = 27.27;
            const double stormDurationVariation = 28.28;

            hydraRingConfigurationService.AddHydraRingCalculationInput(new StructuresOvertoppingCalculationInput(hydraulicBoundaryLocationId,
                                                                                                                 sectionNormal,
                                                                                                                 forelandPoints, breakWater,
                                                                                                                 gravitationalAcceleration,
                                                                                                                 modelFactorOvertoppingMean, modelFactorOvertoppingStandardDeviation,
                                                                                                                 levelCrestStructureMean, levelCrestStructureStandardDeviation,
                                                                                                                 structureNormalOrientation,
                                                                                                                 modelFactorSuperCriticalFlowMean, modelFactorSuperCriticalFlowStandardDeviation,
                                                                                                                 allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                                                                                                                 modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                                                                                                                 storageStructureAreaMean, storageStructureAreaVariation,
                                                                                                                 modelFactorInflowVolume,
                                                                                                                 flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                                                                                 criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                                                                                                                 failureProbabilityStructureWithErosion,
                                                                                                                 widthFlowAperturesMean, widthFlowAperturesStandardDeviation,
                                                                                                                 deviationWaveDirection,
                                                                                                                 stormDurationMean, stormDurationVariation)
            {
                DesignTablesSetting = new DesignTablesSetting(0, 0),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        421, numericsSetting
                    },
                    {
                        422, numericsSetting
                    },
                    {
                        423, numericsSetting
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(1)
            });
            string expectedCreationScript =
                "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Sections];" + Environment.NewLine +
                "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                "INSERT INTO [SectionCalculationSchemes] VALUES (1, 110, 1);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [DesignTables];" + Environment.NewLine +
                "INSERT INTO [DesignTables] VALUES (1, 110, 1, 1, 1, 60, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Numerics];" + Environment.NewLine +
                "INSERT INTO [Numerics] VALUES (1, 110, 1, 1, 421, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                "INSERT INTO [Numerics] VALUES (1, 110, 1, 1, 422, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                "INSERT INTO [Numerics] VALUES (1, 110, 1, 1, 423, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [VariableDatas];" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 58, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 59, 0, 4, 5.5, 6.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 60, 0, 2, 7.7, 8.8, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 61, 9.9, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 62, 0, 2, 10.1, 11.11, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 94, 0, 4, 12.12, 13.13, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 95, 0, 4, 14.14, 15.15, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 96, 0, 4, 16.16, NULL, NULL, NULL, 0, 17.17, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 97, 18.18, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 103, 0, 4, 19.19, 20.2, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 104, 0, 4, 21.21, NULL, NULL, NULL, 0, 22.22, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 105, 23.23, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 106, 0, 2, 24.24, 25.25, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 107, 26.26, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                "INSERT INTO [VariableDatas] VALUES (1, 110, 1, 1, 108, 0, 4, 27.27, NULL, NULL, NULL, 0, 28.28, 999999);" + Environment.NewLine +
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
                "INSERT INTO [ForelandModels] VALUES (1, 110, 3);" + Environment.NewLine +
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
                "DELETE FROM [WaveReductions];" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Areas];" + Environment.NewLine +
                "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Projects];" + Environment.NewLine +
                "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                Environment.NewLine +
                "DELETE FROM [Breakwaters];" + Environment.NewLine +
                "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForWaveConditionsCosineCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
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
                                                         1.0 / 10000,
                                                         forelandPoints,
                                                         breakWater,
                                                         3.3,
                                                         4.4,
                                                         5.5,
                                                         6.6)
                {
                    DesignTablesSetting = new DesignTablesSetting(7.7, 8.8),
                    NumericsSettings = new Dictionary<int, NumericsSetting>
                    {
                        {
                            5, new NumericsSetting(4, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25)
                        }
                    },
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 56.23, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 3, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 3, 1, 1, 8, 114, 0, 0, 0, 0, 7.7, 8.8, 3.71901648545571);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 3, 1, 1, 5, 4, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForWaveConditionsTrapezoidCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
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
                                                            1.0 / 10000,
                                                            forelandPoints,
                                                            breakWater,
                                                            3.3,
                                                            4.4,
                                                            5.5,
                                                            6.6,
                                                            7.7)
                {
                    DesignTablesSetting = new DesignTablesSetting(8.8, 9.9),
                    NumericsSettings = new Dictionary<int, NumericsSetting>
                    {
                        {
                            5, new NumericsSetting(4, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25)
                        }
                    },
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 86.48, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 3, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 3, 1, 1, 8, 114, 0, 0, 0, 0, 8.8, 9.9, 3.71901648545571);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 3, 1, 1, 5, 4, 1, 50, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresClosureFloodedCulvertCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new StructuresClosureFloodedCulvertCalculationInput(hydraulicBoundaryLocationId,
                                                                    sectionNormal,
                                                                    forelandPoints, breakWater,
                                                                    1.1, 2.2, 3.3, 4.4, 5,
                                                                    6.6, 7.7, 8.8, 9.9, 10.0,
                                                                    11.1, 12.2, 13.3, 14.4,
                                                                    15.5, 16.6, 17.7, 18.8,
                                                                    19.9, 20.0, 21.1, 22.2,
                                                                    23.3, 24.4, 25.5, 26.6)
                {
                    DesignTablesSetting = new DesignTablesSetting(0, 0),
                    NumericsSettings = CreateStructuresClosureNumericsSettings(),
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 111, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 111, 1, 1, 1, 58, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 422, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 424, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 425, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 426, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 427, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 58, 1.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 63, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 66, 0, 4, 21.1, 22.2, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 67, 0, 4, 23.3, 24.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 68, 3.3, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 69, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 71, 5, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 93, 0, 2, 25.5, 26.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 94, 0, 4, 6.6, 7.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 95, 0, 4, 8.8, 9.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 96, 0, 4, 10, NULL, NULL, NULL, 0, 11.1, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 97, 12.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 103, 0, 4, 13.3, 14.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 104, 0, 4, 15.5, NULL, NULL, NULL, 0, 16.6, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 105, 17.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 108, 0, 4, 18.8, NULL, NULL, NULL, 0, 19.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 129, 20, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 111, 1, 1, 4505);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 424, 107);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 425, 113);" + Environment.NewLine +
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
                                            "INSERT INTO [ForelandModels] VALUES (1, 111, 3);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresClosureLowSillCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new StructuresClosureLowSillCalculationInput(hydraulicBoundaryLocationId,
                                                             sectionNormal,
                                                             forelandPoints, breakWater,
                                                             1.1, 2.2, 3.3, 4.4, 5,
                                                             6.6, 7.7, 8.8, 9.9, 10.0,
                                                             11.1, 12.2, 13.3, 14.4,
                                                             15.5, 16.6, 17.7, 18.8,
                                                             19.9, 20.0, 21.1, 22.2,
                                                             23.3, 24.4, 25.5, 26.6,
                                                             27.7, 28.8)
                {
                    DesignTablesSetting = new DesignTablesSetting(0, 0),
                    NumericsSettings = CreateStructuresClosureNumericsSettings(),
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 111, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 111, 1, 1, 1, 58, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 422, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 424, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 425, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 426, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 427, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 58, 1.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 63, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 65, 0, 2, 21.1, 22.2, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 68, 3.3, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 69, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 71, 5, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 93, 0, 2, 23.3, 24.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 94, 0, 4, 6.6, 7.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 95, 0, 4, 8.8, 9.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 96, 0, 4, 10, NULL, NULL, NULL, 0, 11.1, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 97, 12.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 103, 0, 4, 13.3, 14.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 104, 0, 4, 15.5, NULL, NULL, NULL, 0, 16.6, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 105, 17.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 106, 0, 2, 25.5, 26.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 108, 0, 4, 18.8, NULL, NULL, NULL, 0, 19.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 125, 0, 2, 27.7, 28.8, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 129, 20, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 111, 1, 1, 4505);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 424, 106);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 425, 111);" + Environment.NewLine +
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
                                            "INSERT INTO [ForelandModels] VALUES (1, 111, 3);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresClosureVerticalWallCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new StructuresClosureVerticalWallCalculationInput(hydraulicBoundaryLocationId,
                                                                  sectionNormal,
                                                                  forelandPoints, breakWater,
                                                                  1.1, 2.2, 3.3, 4.4, 5,
                                                                  6.6, 7.7, 8.8, 9.9, 10.0,
                                                                  11.1, 12.2, 13.3, 14.4,
                                                                  15.5, 16.6, 17.7, 18.8,
                                                                  19.9, 20.0, 21.1, 22.2,
                                                                  23.3, 24.4, 25.5, 26.6,
                                                                  27.7, 28.8, 29.9, 30.0)
                {
                    DesignTablesSetting = new DesignTablesSetting(0, 0),
                    NumericsSettings = CreateStructuresClosureNumericsSettings(),
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 111, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 111, 1, 1, 1, 58, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 422, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 424, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 425, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 426, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 111, 1, 1, 427, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 58, 1.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 59, 0, 4, 21.1, 22.2, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 61, 23.3, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 62, 0, 2, 24.4, 25.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 63, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 68, 3.3, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 69, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 71, 5, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 72, 0, 2, 26.6, 27.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 94, 0, 4, 6.6, 7.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 95, 0, 4, 8.8, 9.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 96, 0, 4, 10, NULL, NULL, NULL, 0, 11.1, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 97, 12.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 103, 0, 4, 13.3, 14.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 104, 0, 4, 15.5, NULL, NULL, NULL, 0, 16.6, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 105, 17.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 106, 0, 2, 28.8, 29.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 107, 30, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 108, 0, 4, 18.8, NULL, NULL, NULL, 0, 19.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 111, 1, 1, 129, 20, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 111, 1, 1, 4505);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 424, 105);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 425, 109);" + Environment.NewLine +
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
                                            "INSERT INTO [ForelandModels] VALUES (1, 111, 3);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresStabilityPointFloodedCulvertLinearCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new StructuresStabilityPointFloodedCulvertLinearCalculationInput(hydraulicBoundaryLocationId,
                                                                                 sectionNormal,
                                                                                 forelandPoints, breakWater,
                                                                                 1.1, 2.2, 3.3, 4.4, 5.5,
                                                                                 6.6, 7.7, 8.8, 9.9, 10.0,
                                                                                 11.1, 12.2, 13.3, 14.4,
                                                                                 15.5, 16.6, 17.7, 18.8,
                                                                                 19.9, 20, 21.1, 22.2,
                                                                                 23.3, 24.4, 25.5, 26.6,
                                                                                 27.7, 28.8, 29.9, 30.0,
                                                                                 31.1, 32.2, 33.3, 34.4,
                                                                                 35.5, 36.6, 37.7, 38.8,
                                                                                 39.9, 40.0, 41.1, 42.2,
                                                                                 43.3, 44.4, 45.5, 46.6,
                                                                                 47.7, 48.8, 49.9, 50.0,
                                                                                 51.1, 52.2, 53.3, 54.4,
                                                                                 55.5, 56.6, 57.7, 58.8,
                                                                                 59.9)
                {
                    DesignTablesSetting = new DesignTablesSetting(0, 0),
                    NumericsSettings = CreateStructuresStabilityPointNumericsSettings(),
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 112, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 112, 1, 1, 1, 58, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 422, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 424, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 425, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 430, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 431, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 432, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 433, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 434, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 435, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 43, 1.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 58, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 60, 0, 2, 3.3, 4.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 61, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 63, 6.6, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 65, 0, 2, 7.7, 8.8, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 66, 0, 4, 52.2, 53.3, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 67, 0, 4, 54.4, 55.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 80, 0, 4, 56.6, NULL, NULL, NULL, 0, 57.7, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 82, 0, 2, 9.9, 10, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 83, 0, 4, 58.8, NULL, NULL, NULL, 0, 59.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 85, 11.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 86, 0, 4, 12.2, NULL, NULL, NULL, 0, 13.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 87, 0, 2, 14.4, NULL, NULL, NULL, 0, 15.5, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 88, 0, 2, 16.6, NULL, NULL, NULL, 0, 17.7, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 89, 0, 2, 18.8, NULL, NULL, NULL, 0, 19.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 90, 20, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 91, 21.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 92, 0, 2, 22.2, NULL, NULL, NULL, 0, 23.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 93, 0, 2, 24.4, 25.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 94, 0, 4, 26.6, 27.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 95, 0, 4, 28.8, 29.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 96, 0, 4, 30, NULL, NULL, NULL, 0, 31.1, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 97, 32.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 103, 0, 4, 33.3, 34.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 104, 0, 4, 35.5, NULL, NULL, NULL, 0, 36.6, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 105, 37.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 108, 0, 4, 38.8, NULL, NULL, NULL, 0, 39.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 125, 0, 2, 40, 41.1, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 130, 0, 2, 42.2, 43.3, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 131, 44.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 132, 0, 2, 45.5, 46.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 133, 0, 18, 48.8, 47.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 134, 49.9, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 135, 50, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 136, 51.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 112, 1, 1, 4607);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 424, 107);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 425, 113);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 430, 114);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 435, 116);" + Environment.NewLine +
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
                                            "INSERT INTO [ForelandModels] VALUES (1, 112, 3);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresStabilityPointFloodedCulvertQuadraticCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(hydraulicBoundaryLocationId,
                                                                                    sectionNormal,
                                                                                    forelandPoints, breakWater,
                                                                                    1.1, 2.2, 3.3, 4.4, 5.5,
                                                                                    6.6, 7.7, 8.8, 9.9, 10.0,
                                                                                    11.1, 12.2, 13.3, 14.4,
                                                                                    15.5, 16.6, 17.7, 18.8,
                                                                                    19.9, 20, 21.1, 22.2,
                                                                                    23.3, 24.4, 25.5, 26.6,
                                                                                    27.7, 28.8, 29.9, 30.0,
                                                                                    31.1, 32.2, 33.3, 34.4,
                                                                                    35.5, 36.6, 37.7, 38.8,
                                                                                    39.9, 40.0, 41.1, 42.2,
                                                                                    43.3, 44.4, 45.5, 46.6,
                                                                                    47.7, 48.8, 49.9, 50.0,
                                                                                    51.1, 52.2, 53.3, 54.4,
                                                                                    55.5, 56.6, 57.7, 58.8,
                                                                                    59.9)
                {
                    DesignTablesSetting = new DesignTablesSetting(0, 0),
                    NumericsSettings = CreateStructuresStabilityPointNumericsSettings(),
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 112, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 112, 1, 1, 1, 58, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 422, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 424, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 425, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 430, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 431, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 432, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 433, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 434, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 435, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 43, 1.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 58, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 60, 0, 2, 3.3, 4.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 61, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 63, 6.6, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 65, 0, 2, 7.7, 8.8, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 66, 0, 4, 52.2, 53.3, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 67, 0, 4, 54.4, 55.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 81, 0, 4, 56.6, NULL, NULL, NULL, 0, 57.7, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 82, 0, 2, 9.9, 10, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 84, 0, 4, 58.8, NULL, NULL, NULL, 0, 59.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 85, 11.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 86, 0, 4, 12.2, NULL, NULL, NULL, 0, 13.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 87, 0, 2, 14.4, NULL, NULL, NULL, 0, 15.5, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 88, 0, 2, 16.6, NULL, NULL, NULL, 0, 17.7, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 89, 0, 2, 18.8, NULL, NULL, NULL, 0, 19.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 90, 20, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 91, 21.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 92, 0, 2, 22.2, NULL, NULL, NULL, 0, 23.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 93, 0, 2, 24.4, 25.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 94, 0, 4, 26.6, 27.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 95, 0, 4, 28.8, 29.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 96, 0, 4, 30, NULL, NULL, NULL, 0, 31.1, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 97, 32.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 103, 0, 4, 33.3, 34.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 104, 0, 4, 35.5, NULL, NULL, NULL, 0, 36.6, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 105, 37.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 108, 0, 4, 38.8, NULL, NULL, NULL, 0, 39.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 125, 0, 2, 40, 41.1, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 130, 0, 2, 42.2, 43.3, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 131, 44.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 132, 0, 2, 45.5, 46.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 133, 0, 18, 48.8, 47.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 134, 49.9, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 135, 50, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 136, 51.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 112, 1, 1, 4607);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 424, 107);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 425, 113);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 430, 115);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 435, 117);" + Environment.NewLine +
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
                                            "INSERT INTO [ForelandModels] VALUES (1, 112, 3);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresStabilityPointLowSillLinearCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new StructuresStabilityPointLowSillLinearCalculationInput(hydraulicBoundaryLocationId,
                                                                          sectionNormal,
                                                                          forelandPoints, breakWater,
                                                                          1.1, 2.2, 3.3, 4.4, 5.5,
                                                                          6.6, 7.7, 8.8, 9.9, 10.0,
                                                                          11.1, 12.2, 13.3, 14.4,
                                                                          15.5, 16.6, 17.7, 18.8,
                                                                          19.9, 20, 21.1, 22.2,
                                                                          23.3, 24.4, 25.5, 26.6,
                                                                          27.7, 28.8, 29.9, 30.0,
                                                                          31.1, 32.2, 33.3, 34.4,
                                                                          35.5, 36.6, 37.7, 38.8,
                                                                          39.9, 40.0, 41.1, 42.2,
                                                                          43.3, 44.4, 45.5, 46.6,
                                                                          47.7, 48.8, 49.9, 50.0,
                                                                          51.1, 52.2, 53.3, 54.4,
                                                                          55.5, 56.6, 57.7)
                {
                    DesignTablesSetting = new DesignTablesSetting(0, 0),
                    NumericsSettings = CreateStructuresStabilityPointNumericsSettings(),
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 112, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 112, 1, 1, 1, 58, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 422, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 424, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 425, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 430, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 431, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 432, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 433, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 434, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 435, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 43, 1.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 58, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 60, 0, 2, 3.3, 4.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 61, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 63, 6.6, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 65, 0, 2, 7.7, 8.8, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 80, 0, 4, 52.2, NULL, NULL, NULL, 0, 53.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 82, 0, 2, 9.9, 10, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 83, 0, 4, 54.4, NULL, NULL, NULL, 0, 55.5, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 85, 11.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 86, 0, 4, 12.2, NULL, NULL, NULL, 0, 13.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 87, 0, 2, 14.4, NULL, NULL, NULL, 0, 15.5, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 88, 0, 2, 16.6, NULL, NULL, NULL, 0, 17.7, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 89, 0, 2, 18.8, NULL, NULL, NULL, 0, 19.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 90, 20, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 91, 21.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 92, 0, 2, 22.2, NULL, NULL, NULL, 0, 23.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 93, 0, 2, 24.4, 25.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 94, 0, 4, 26.6, 27.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 95, 0, 4, 28.8, 29.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 96, 0, 4, 30, NULL, NULL, NULL, 0, 31.1, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 97, 32.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 103, 0, 4, 33.3, 34.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 104, 0, 4, 35.5, NULL, NULL, NULL, 0, 36.6, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 105, 37.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 106, 0, 2, 56.6, 57.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 108, 0, 4, 38.8, NULL, NULL, NULL, 0, 39.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 125, 0, 2, 40, 41.1, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 130, 0, 2, 42.2, 43.3, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 131, 44.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 132, 0, 2, 45.5, 46.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 133, 0, 18, 48.8, 47.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 134, 49.9, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 135, 50, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 136, 51.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 112, 1, 1, 4607);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 424, 106);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 425, 111);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 430, 114);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 435, 116);" + Environment.NewLine +
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
                                            "INSERT INTO [ForelandModels] VALUES (1, 112, 3);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForStructuresStabilityPointLowSillQuadraticCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionNormal = 3.3;
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };
            var breakWater = new HydraRingBreakWater(1, 2.2);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new StructuresStabilityPointLowSillQuadraticCalculationInput(hydraulicBoundaryLocationId,
                                                                             sectionNormal,
                                                                             forelandPoints, breakWater,
                                                                             1.1, 2.2, 3.3, 4.4, 5.5,
                                                                             6.6, 7.7, 8.8, 9.9, 10.0,
                                                                             11.1, 12.2, 13.3, 14.4,
                                                                             15.5, 16.6, 17.7, 18.8,
                                                                             19.9, 20, 21.1, 22.2,
                                                                             23.3, 24.4, 25.5, 26.6,
                                                                             27.7, 28.8, 29.9, 30.0,
                                                                             31.1, 32.2, 33.3, 34.4,
                                                                             35.5, 36.6, 37.7, 38.8,
                                                                             39.9, 40.0, 41.1, 42.2,
                                                                             43.3, 44.4, 45.5, 46.6,
                                                                             47.7, 48.8, 49.9, 50.0,
                                                                             51.1, 52.2, 53.3, 54.4,
                                                                             55.5, 56.6, 57.7)
                {
                    DesignTablesSetting = new DesignTablesSetting(0, 0),
                    NumericsSettings = CreateStructuresStabilityPointNumericsSettings(),
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 112, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 112, 1, 1, 1, 58, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 422, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 424, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 425, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 430, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 431, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 432, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 433, 1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 434, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 112, 1, 1, 435, 11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 6, 10000, 40000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 43, 1.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 58, 2.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 60, 0, 2, 3.3, 4.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 61, 5.5, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 63, 6.6, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 65, 0, 2, 7.7, 8.8, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 81, 0, 4, 52.2, NULL, NULL, NULL, 0, 53.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 82, 0, 2, 9.9, 10, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 84, 0, 4, 54.4, NULL, NULL, NULL, 0, 55.5, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 85, 11.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 86, 0, 4, 12.2, NULL, NULL, NULL, 0, 13.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 87, 0, 2, 14.4, NULL, NULL, NULL, 0, 15.5, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 88, 0, 2, 16.6, NULL, NULL, NULL, 0, 17.7, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 89, 0, 2, 18.8, NULL, NULL, NULL, 0, 19.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 90, 20, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 91, 21.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 92, 0, 2, 22.2, NULL, NULL, NULL, 0, 23.3, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 93, 0, 2, 24.4, 25.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 94, 0, 4, 26.6, 27.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 95, 0, 4, 28.8, 29.9, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 96, 0, 4, 30, NULL, NULL, NULL, 0, 31.1, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 97, 32.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 103, 0, 4, 33.3, 34.4, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 104, 0, 4, 35.5, NULL, NULL, NULL, 0, 36.6, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 105, 37.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 106, 0, 2, 56.6, 57.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 108, 0, 4, 38.8, NULL, NULL, NULL, 0, 39.9, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 125, 0, 2, 40, 41.1, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 130, 0, 2, 42.2, 43.3, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 131, 44.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 132, 0, 2, 45.5, 46.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 133, 0, 18, 48.8, 47.7, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 134, 49.9, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 135, 50, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 112, 1, 1, 136, 51.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 112, 1, 1, 4607);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 424, 106);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 425, 111);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 430, 115);" + Environment.NewLine +
                                            "INSERT INTO [SectionSubMechanismModels] VALUES (1, 1, 1, 435, 117);" + Environment.NewLine +
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
                                            "INSERT INTO [ForelandModels] VALUES (1, 112, 3);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine +
                                            "INSERT INTO [Breakwaters] VALUES (1, 1, 2.2);" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForDunesBoundaryConditionsCalculationInput_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new DunesBoundaryConditionsCalculationInput(1, 700004, 1.0 / 10000)
            {
                DesignTablesSetting = new DesignTablesSetting(1.1, 2.2),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        6, new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25)
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(1)
            });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 0, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 1, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 1, 1, 1, 2, 26, 0, 0, 0, 0, 1.1, 2.2, 3.71901648545571);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 6, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 1, 1, 1, 8);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForPipingCalculationInputWithCoverageLayer_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionLength = 1.1;
            const double phreaticLevelExitMean = 2.2;
            const double phreaticLevelExitStandardDeviation = 3.3;
            const double waterVolumetricWeight = 4.4;
            const double effectiveThicknessCoverageLayerMean = 5.5;
            const double effectiveThicknessCoverageLayerStandardDeviation = 6.6;
            const double saturatedVolumicWeightOfCoverageLayerMean = 7.7;
            const double saturatedVolumicWeightOfCoverageLayerStandardDeviation = 8.8;
            const double saturatedVolumicWeightOfCoverageLayerShift = 9.9;
            const double upliftModelFactorMean = 10.0;
            const double upliftModelFactorStandardDeviation = 11.1;
            const double dampingFactorExitMean = 12.2;
            const double dampingFactorExitStandardDeviation = 13.3;
            const double seepageLengthMean = 14.4;
            const double seepageLengthCoefficientOfVariation = 15.5;
            const double thicknessAquiferLayerMean = 16.6;
            const double thicknessAquiferLayerStandardDeviation = 17.7;
            const double sandParticlesVolumicWeight = 18.8;
            const double sellmeijerModelFactorMean = 19.9;
            const double sellmeijerModelFactorStandardDeviation = 20.0;
            const double beddingAngle = 21.1;
            const double whitesDragCoefficient = 22.2;
            const double waterKinematicViscosity = 23.3;
            const double darcyPermeabilityMean = 24.4;
            const double darcyPermeabilityCoefficientOfVariation = 25.5;
            const double diameter70Mean = 26.6;
            const double diameter70CoefficientOfVariation = 27.7;
            const double gravity = 28.8;
            const double criticalHeaveGradientMean = 29.9;
            const double criticalHeaveGradientStandardDeviation = 30.0;

            var numericsSetting = new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new PipingCalculationInput(
                    hydraulicBoundaryLocationId, sectionLength, phreaticLevelExitMean, phreaticLevelExitStandardDeviation,
                    waterVolumetricWeight, effectiveThicknessCoverageLayerMean, effectiveThicknessCoverageLayerStandardDeviation,
                    saturatedVolumicWeightOfCoverageLayerMean, saturatedVolumicWeightOfCoverageLayerStandardDeviation,
                    saturatedVolumicWeightOfCoverageLayerShift, upliftModelFactorMean, upliftModelFactorStandardDeviation,
                    dampingFactorExitMean, dampingFactorExitStandardDeviation, seepageLengthMean, seepageLengthCoefficientOfVariation,
                    thicknessAquiferLayerMean, thicknessAquiferLayerStandardDeviation, sandParticlesVolumicWeight,
                    sellmeijerModelFactorMean, sellmeijerModelFactorStandardDeviation, beddingAngle, whitesDragCoefficient,
                    waterKinematicViscosity, darcyPermeabilityMean, darcyPermeabilityCoefficientOfVariation, diameter70Mean,
                    diameter70CoefficientOfVariation, gravity, criticalHeaveGradientMean, criticalHeaveGradientStandardDeviation)
                {
                    DesignTablesSetting = new DesignTablesSetting(17.17, 18.18),
                    NumericsSettings = new Dictionary<int, NumericsSetting>
                    {
                        {
                            311, numericsSetting
                        },
                        {
                            313, numericsSetting
                        },
                        {
                            314, numericsSetting
                        }
                    },
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 0, 1.1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 103, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 103, 1, 1, 1, 58, 0, 0, 0, 0, 17.17, 18.18, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 103, 1, 1, 311, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 103, 1, 1, 313, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 103, 1, 1, 314, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 42, 0, 2, 2.2, 3.3, NULL, NULL, 1, 0, 1.1);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 43, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 44, 0, 4, 5.5, 6.6, NULL, NULL, 1, 0, 200);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 45, 0, 4, 7.7, 8.8, 9.9, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 46, 0, 4, 10, 11.1, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 47, 0, 4, 12.2, 13.3, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 48, 0, 4, 14.4, NULL, NULL, NULL, 0, 15.5, 3000);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 49, 0, 4, 16.6, 17.7, NULL, NULL, 1, 0, 1500);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 50, 23.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 51, 0, 4, 19.9, 20, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 52, 21.1, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 53, 22.2, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 54, 23.3, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 55, 0, 4, 24.4, NULL, NULL, NULL, 0, 25.5, 600);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 56, 0, 4, 26.6, NULL, NULL, NULL, 0, 27.7, 180);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 58, 28.8, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 124, 0, 4, 29.9, 30, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 103, 1, 1, 3015);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        [Test]
        public void WriteDatabaseCreationScript_HydraRingConfigurationForPipingCalculationInputWithoutCoverageLayer_WritesExpectedCreationScript()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.All);
            const int hydraulicBoundaryLocationId = 700004;

            const double sectionLength = 1.1;
            const double phreaticLevelExitMean = 2.2;
            const double phreaticLevelExitStandardDeviation = 3.3;
            const double waterVolumetricWeight = 4.4;
            const double upliftModelFactorMean = 5.5;
            const double upliftModelFactorStandardDeviation = 6.6;
            const double dampingFactorExitMean = 7.7;
            const double dampingFactorExitStandardDeviation = 8.8;
            const double seepageLengthMean = 9.9;
            const double seepageLengthCoefficientOfVariation = 10.0;
            const double thicknessAquiferLayerMean = 11.1;
            const double thicknessAquiferLayerStandardDeviation = 12.2;
            const double sandParticlesVolumicWeight = 13.3;
            const double sellmeijerModelFactorMean = 14.4;
            const double sellmeijerModelFactorStandardDeviation = 15.5;
            const double beddingAngle = 16.6;
            const double whitesDragCoefficient = 17.7;
            const double waterKinematicViscosity = 18.8;
            const double darcyPermeabilityMean = 19.9;
            const double darcyPermeabilityCoefficientOfVariation = 20.0;
            const double diameter70Mean = 21.1;
            const double diameter70CoefficientOfVariation = 22.2;
            const double gravity = 23.3;
            const double criticalHeaveGradientMean = 24.4;
            const double criticalHeaveGradientStandardDeviation = 25.5;

            var numericsSetting = new NumericsSetting(1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3000, 10000, 0.1, -6, 6, 25);

            hydraRingConfigurationService.AddHydraRingCalculationInput(
                new PipingCalculationInput(
                    hydraulicBoundaryLocationId, sectionLength, phreaticLevelExitMean, phreaticLevelExitStandardDeviation,
                    waterVolumetricWeight, upliftModelFactorMean, upliftModelFactorStandardDeviation, dampingFactorExitMean,
                    dampingFactorExitStandardDeviation, seepageLengthMean, seepageLengthCoefficientOfVariation,
                    thicknessAquiferLayerMean, thicknessAquiferLayerStandardDeviation, sandParticlesVolumicWeight,
                    sellmeijerModelFactorMean, sellmeijerModelFactorStandardDeviation, beddingAngle, whitesDragCoefficient,
                    waterKinematicViscosity, darcyPermeabilityMean, darcyPermeabilityCoefficientOfVariation, diameter70Mean,
                    diameter70CoefficientOfVariation, gravity, criticalHeaveGradientMean, criticalHeaveGradientStandardDeviation)
                {
                    DesignTablesSetting = new DesignTablesSetting(17.17, 18.18),
                    NumericsSettings = new Dictionary<int, NumericsSetting>
                    {
                        {
                            311, numericsSetting
                        },
                        {
                            313, numericsSetting
                        },
                        {
                            314, numericsSetting
                        }
                    },
                    TimeIntegrationSetting = new TimeIntegrationSetting(1)
                });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 0, 1.1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 103, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 103, 1, 1, 1, 58, 0, 0, 0, 0, 17.17, 18.18, 0);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 103, 1, 1, 311, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 103, 1, 1, 313, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 103, 1, 1, 314, 1, 9, 150, 0.15, 0.01, 0.01, 0.01, 2, 3, 3000, 10000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 42, 0, 2, 2.2, 3.3, NULL, NULL, 1, 0, 1.1);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 43, 4.4, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 44, 0, 0, 0, NULL, NULL, NULL, 1, 0, 200);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 45, 0, 0, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 46, 0, 4, 5.5, 6.6, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 47, 0, 4, 7.7, 8.8, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 48, 0, 4, 9.9, NULL, NULL, NULL, 0, 10, 3000);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 49, 0, 4, 11.1, 12.2, NULL, NULL, 1, 0, 1500);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 50, 17.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 51, 0, 4, 14.4, 15.5, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 52, 16.6, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 53, 17.7, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 54, 18.8, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 55, 0, 4, 19.9, NULL, NULL, NULL, 0, 20, 600);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 56, 0, 4, 21.1, NULL, NULL, NULL, 0, 22.2, 180);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 58, 23.3, 0, 0, NULL, NULL, NULL, 1, 0, 999999);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 103, 1, 1, 124, 0, 4, 24.4, 25.5, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 103, 1, 1, 3014);" + Environment.NewLine +
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
                                            "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Areas];" + Environment.NewLine +
                                            "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Projects];" + Environment.NewLine +
                                            "INSERT INTO [Projects] VALUES (1, 'BOI', 'Riskeer calculation');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Breakwaters];" + Environment.NewLine;

            string databaseFilePath = Path.Combine(hydraRingDirectory, "temp.db");
            using (new FileDisposeHelper(databaseFilePath))
            {
                // Call
                hydraRingConfigurationService.WriteDatabaseCreationScript(databaseFilePath);

                // Assert
                string creationScript = File.ReadAllText(databaseFilePath);
                Assert.AreEqual(expectedCreationScript, creationScript);
            }
        }

        private static Dictionary<int, NumericsSetting> CreateStructuresClosureNumericsSettings()
        {
            var numericsSettingForm = new NumericsSetting(1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6, 6, 25);
            var numericsSettingDirs = new NumericsSetting(11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6, 6, 25);

            return new Dictionary<int, NumericsSetting>
            {
                {
                    422, numericsSettingForm
                },
                {
                    424, numericsSettingDirs
                },
                {
                    425, numericsSettingDirs
                },
                {
                    426, numericsSettingForm
                },
                {
                    427, numericsSettingForm
                }
            };
        }

        private static Dictionary<int, NumericsSetting> CreateStructuresStabilityPointNumericsSettings()
        {
            var numericsSettingForm = new NumericsSetting(1, 1, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6, 6, 25);
            var numericsSettingDir = new NumericsSetting(11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6, 6, 25);

            return new Dictionary<int, NumericsSetting>
            {
                {
                    422, numericsSettingForm
                },
                {
                    424, numericsSettingDir
                },
                {
                    425, numericsSettingDir
                },
                {
                    430, numericsSettingDir
                },
                {
                    431, numericsSettingForm
                },
                {
                    432, numericsSettingForm
                },
                {
                    433, numericsSettingForm
                },
                {
                    434, numericsSettingDir
                },
                {
                    435, numericsSettingDir
                }
            };
        }
    }
}