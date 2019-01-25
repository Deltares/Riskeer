// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Settings;
using Riskeer.HydraRing.Calculation.Data.Variables;
using Riskeer.HydraRing.Calculation.Services;

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
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.Model);

            // Assert
            Assert.AreEqual(HydraRingUncertaintiesType.Model, hydraRingConfigurationService.UncertaintiesType);
        }

        [Test]
        public void AddHydraRingCalculationInput_DuplicateSectionId_ThrowsArgumentException()
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.Model);
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
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.Model);
            var calculationInput1 = new HydraRingCalculationInputImplementation(1, 2);
            var calculationInput2 = new HydraRingCalculationInputImplementation(2, 3);
            calculationInput2.SetFailureMechanismType(HydraRingFailureMechanismType.DikeHeight);

            hydraRingConfigurationService.AddHydraRingCalculationInput(calculationInput1);

            // Call
            TestDelegate test = () => hydraRingConfigurationService.AddHydraRingCalculationInput(calculationInput2);

            // Assert
            const string expectedMessage = "Running calculations for multiple failure mechanism types is not supported.";
            var exception = Assert.Throws<NotSupportedException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void WriteDatabaseCreationScript_SingleHydraRingCalculationInput_WritesExpectedCreationScript([Values(true, false)] bool withForeland,
                                                                                                             [Values(true, false)] bool withBreakWater,
                                                                                                             [Values(true, false)] bool runPreprocessor)
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.Model);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(1, 700004, withForeland, withBreakWater)
            {
                PreprocessorSetting = runPreprocessor
                                          ? new PreprocessorSetting(1001.1, 1002.2, new NumericsSetting(1008, 1009, 1010, 1011.11, 1012.12, 1013.13, 1014.14, 1015, 1016, 1017, 1018.18, 1019.19, 1020.20, 1021))
                                          : new PreprocessorSetting(),
                DesignTablesSetting = new DesignTablesSetting(6.6, 7.7),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        1, new NumericsSetting(8, 9, 10, 11.11, 12.12, 13.13, 14.14, 15, 16, 17, 18.18, 19.19, 20.20, 21)
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(3)
            });

            string expectedForelandModelsScript = withForeland || withBreakWater
                                                      ? "INSERT INTO [ForelandModels] VALUES (1, 1, 3);" + Environment.NewLine
                                                      : string.Empty;
            string expectedForelandsScript = withForeland
                                                 ? "INSERT INTO [Forelands] VALUES (1, 1, 1.1, 2.2);" + Environment.NewLine +
                                                   "INSERT INTO [Forelands] VALUES (1, 2, 2.2, 3.3);" + Environment.NewLine
                                                 : string.Empty;
            string expectedBreakWatersScript = withBreakWater
                                                   ? "INSERT INTO [Breakwaters] VALUES (1, 1, 99.9);" + Environment.NewLine
                                                   : string.Empty;

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 2, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 2.2);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 1, 3);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 1, 1, 1, 4, 5, 0, 0, 0, 0, 6.6, 7.7, 1.1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [PreprocessorSettings];" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [PreprocessorSettings] VALUES (1, 1001.1, 1002.2);" + Environment.NewLine
                                                 : string.Empty) +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 1, 8, 9, 10, 11.11, 12.12, 13.13, 14.14, 15, 3, 16, 17, 18.18, 19.19, 20.2, 21);" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 7, 1008, 1009, 1010, 1011.11, 1012.12, 1013.13, 1014.14, 1015, 3, 1016, 1017, 1018.18, 1019.19, 1020.2, 1021);" + Environment.NewLine
                                                 : string.Empty) +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 0, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 2, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 4, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 19, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 18, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                            "INSERT INTO [CalculationProfiles] VALUES (1, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                            "INSERT INTO [CalculationProfiles] VALUES (1, 2, 11.1, 22.2, 33.3);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (1, 1, 1, 1, 1);" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [SectionFaultTreeModels] VALUES (1, 1, 1, 1, 9);" + Environment.NewLine
                                                 : string.Empty) +
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
                                            expectedForelandModelsScript +
                                            Environment.NewLine +
                                            "DELETE FROM [Forelands];" + Environment.NewLine +
                                            expectedForelandsScript +
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
                                            expectedBreakWatersScript;

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
        public void WriteDatabaseCreationScript_MultipleHydraRingCalculationInputs_WritesExpectedCreationScript([Values(true, false)] bool runPreprocessor)
        {
            // Setup
            var hydraRingConfigurationService = new HydraRingConfigurationService(HydraRingUncertaintiesType.Model);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(1, 700004)
            {
                PreprocessorSetting = runPreprocessor
                                          ? new PreprocessorSetting(1001.1, 1002.2, new NumericsSetting(1008, 1009, 1010, 1011.11, 1012.12, 1013.13, 1014.14, 1015, 1016, 1017, 1018.18, 1019.19, 1020.20, 1021))
                                          : new PreprocessorSetting(),
                DesignTablesSetting = new DesignTablesSetting(6.6, 7.7),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        1, new NumericsSetting(8, 9, 10, 11.11, 12.12, 13.13, 14.14, 15, 16, 17, 18.18, 19.19, 20.20, 21)
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(2)
            });
            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(2, 700005)
            {
                PreprocessorSetting = runPreprocessor
                                          ? new PreprocessorSetting(2001.1, 2002.2, new NumericsSetting(2008, 2009, 2010, 2011.11, 2012.12, 2013.13, 2014.14, 2015, 2016, 2017, 2018.18, 2019.19, 2020.20, 2021))
                                          : new PreprocessorSetting(),
                DesignTablesSetting = new DesignTablesSetting(8.8, 9.9),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        1, new NumericsSetting(108, 109, 110, 111.11, 112.12, 113.13, 114.14, 115, 116, 117, 118.18, 119.19, 120.20, 121)
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(3)
            });
            hydraRingConfigurationService.AddHydraRingCalculationInput(new HydraRingCalculationInputImplementation(3, 700006)
            {
                PreprocessorSetting = runPreprocessor
                                          ? new PreprocessorSetting(3001.1, 3002.2, new NumericsSetting(3008, 3009, 3010, 3011.11, 3012.12, 3013.13, 3014.14, 3015, 3016, 3017, 3018.18, 3019.19, 3020.20, 3021))
                                          : new PreprocessorSetting(),
                DesignTablesSetting = new DesignTablesSetting(10.10, 11.11),
                NumericsSettings = new Dictionary<int, NumericsSetting>
                {
                    {
                        1, new NumericsSetting(208, 209, 210, 211.11, 212.12, 213.13, 214.14, 215, 216, 217, 218.18, 219.19, 220.20, 221)
                    }
                },
                TimeIntegrationSetting = new TimeIntegrationSetting(1)
            });

            string expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                            "INSERT INTO [HydraulicModels] VALUES (1, 2, 'WTI 2017');" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [Sections];" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (1, 1, 1, 1, 1, 0, 0, 0, 0, 700004, 700004, 100, 3.3, 2.2);" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (2, 1, 1, 2, 2, 0, 0, 0, 0, 700005, 700005, 100, 3.3, 2.2);" + Environment.NewLine +
                                            "INSERT INTO [Sections] VALUES (3, 1, 1, 3, 3, 0, 0, 0, 0, 700006, 700006, 100, 3.3, 2.2);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [SectionCalculationSchemes];" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (1, 1, 2);" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (2, 1, 3);" + Environment.NewLine +
                                            "INSERT INTO [SectionCalculationSchemes] VALUES (3, 1, 1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [DesignTables];" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (1, 1, 1, 1, 4, 5, 0, 0, 0, 0, 6.6, 7.7, 1.1);" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (2, 1, 1, 1, 4, 5, 0, 0, 0, 0, 8.8, 9.9, 1.1);" + Environment.NewLine +
                                            "INSERT INTO [DesignTables] VALUES (3, 1, 1, 1, 4, 5, 0, 0, 0, 0, 10.1, 11.11, 1.1);" + Environment.NewLine +
                                            Environment.NewLine +
                                            "DELETE FROM [PreprocessorSettings];" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [PreprocessorSettings] VALUES (1, 1001.1, 1002.2);" + Environment.NewLine +
                                                   "INSERT INTO [PreprocessorSettings] VALUES (2, 2001.1, 2002.2);" + Environment.NewLine +
                                                   "INSERT INTO [PreprocessorSettings] VALUES (3, 3001.1, 3002.2);" + Environment.NewLine
                                                 : string.Empty) +
                                            Environment.NewLine +
                                            "DELETE FROM [Numerics];" + Environment.NewLine +
                                            "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 1, 8, 9, 10, 11.11, 12.12, 13.13, 14.14, 15, 3, 16, 17, 18.18, 19.19, 20.2, 21);" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [Numerics] VALUES (1, 1, 1, 1, 7, 1008, 1009, 1010, 1011.11, 1012.12, 1013.13, 1014.14, 1015, 3, 1016, 1017, 1018.18, 1019.19, 1020.2, 1021);" + Environment.NewLine
                                                 : string.Empty) +
                                            "INSERT INTO [Numerics] VALUES (2, 1, 1, 1, 1, 108, 109, 110, 111.11, 112.12, 113.13, 114.14, 115, 3, 116, 117, 118.18, 119.19, 120.2, 121);" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [Numerics] VALUES (2, 1, 1, 1, 7, 2008, 2009, 2010, 2011.11, 2012.12, 2013.13, 2014.14, 2015, 3, 2016, 2017, 2018.18, 2019.19, 2020.2, 2021);" + Environment.NewLine
                                                 : string.Empty) +
                                            "INSERT INTO [Numerics] VALUES (3, 1, 1, 1, 1, 208, 209, 210, 211.11, 212.12, 213.13, 214.14, 215, 3, 216, 217, 218.18, 219.19, 220.2, 221);" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [Numerics] VALUES (3, 1, 1, 1, 7, 3008, 3009, 3010, 3011.11, 3012.12, 3013.13, 3014.14, 3015, 3, 3016, 3017, 3018.18, 3019.19, 3020.2, 3021);" + Environment.NewLine
                                                 : string.Empty) +
                                            Environment.NewLine +
                                            "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 0, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 2, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 4, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 19, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (1, 1, 1, 1, 26, 0, 18, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 0, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 0, 2, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 0, 4, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 0, 19, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (2, 1, 1, 1, 26, 0, 18, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 0, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 2, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 0, 2, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 4, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 0, 4, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 19, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 0, 19, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 1, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 1.1, 18, 2.2, 3.3, 4.4, 5.5, 0, 6.6, 300);" + Environment.NewLine +
                                            "INSERT INTO [VariableDatas] VALUES (3, 1, 1, 1, 26, 0, 18, 0, NULL, NULL, NULL, 1, 0, 300);" + Environment.NewLine +
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
                                            (runPreprocessor
                                                 ? "INSERT INTO [SectionFaultTreeModels] VALUES (1, 1, 1, 1, 9);" + Environment.NewLine
                                                 : string.Empty) +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (2, 1, 1, 1, 1);" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [SectionFaultTreeModels] VALUES (2, 1, 1, 1, 9);" + Environment.NewLine
                                                 : string.Empty) +
                                            "INSERT INTO [SectionFaultTreeModels] VALUES (3, 1, 1, 1, 1);" + Environment.NewLine +
                                            (runPreprocessor
                                                 ? "INSERT INTO [SectionFaultTreeModels] VALUES (3, 1, 1, 1, 9);" + Environment.NewLine
                                                 : string.Empty) +
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

        private class HydraRingCalculationInputImplementation : HydraRingCalculationInput
        {
            private readonly int sectionId;
            private readonly bool withForeland;
            private readonly bool withBreakWater;
            private HydraRingFailureMechanismType failureMechanismType;

            public HydraRingCalculationInputImplementation(int sectionId, int hydraulicBoundaryLocationId, bool withForeland = true, bool withBreakWater = true)
                : base(hydraulicBoundaryLocationId)
            {
                this.sectionId = sectionId;
                this.withForeland = withForeland;
                this.withBreakWater = withBreakWater;

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
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.Deterministic, HydraRingDeviationType.Standard, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.Normal, HydraRingDeviationType.Variation, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Standard, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.TruncatedNormal, HydraRingDeviationType.Standard, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.TruncatedNormal, HydraRingDeviationType.Variation, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.TruncatedNormal, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.RayleighN, HydraRingDeviationType.Standard, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.RayleighN, HydraRingDeviationType.Variation, 1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
                    yield return new TestHydraRingVariable(26, HydraRingDistributionType.RayleighN, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
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
                    if (withForeland)
                    {
                        yield return new HydraRingForelandPoint(1.1, 2.2);
                        yield return new HydraRingForelandPoint(2.2, 3.3);
                    }
                }
            }

            public override HydraRingBreakWater BreakWater
            {
                get
                {
                    return withBreakWater
                               ? new HydraRingBreakWater(1, 99.9)
                               : null;
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
            public HydraRingProfilePointDerivative(double x, double z, double roughness)
                : base(x, z)
            {
                Roughness = roughness;
            }

            public override double Roughness { get; }
        }
    }
}