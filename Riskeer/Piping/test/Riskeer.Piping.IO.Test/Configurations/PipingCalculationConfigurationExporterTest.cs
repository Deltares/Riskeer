﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationExporterTest
    {
        private abstract class PipingCalculationConfigurationExporterTestFixture : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            PipingCalculationConfigurationExporter,
            PipingCalculationConfigurationWriter,
            IPipingCalculationScenario<PipingInput>,
            PipingCalculationConfiguration>
        {
            protected const string testNameFormat = "{m}({0:40}.xml)";
            
            protected override PipingCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
            {
                return new PipingCalculationConfigurationExporter(calculations, filePath);
            }
        }

        private class SemiProbabilisticPipingCalculationConfigurationExporterTest : PipingCalculationConfigurationExporterTestFixture
        {
            private static IEnumerable<TestCaseData> Calculations
            {
                get
                {
                    yield return new TestCaseData("semiProbabilisticCalculationWithoutHydraulicLocation",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationWithAssessmentLevel",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithAssessmentLevel())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationWithoutSurfaceLine",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSurfaceLine<SemiProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationWithoutSoilModel",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilModel<SemiProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationWithoutSoilProfile",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilProfile<SemiProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationIrrelevant",
                                                  PipingTestDataGenerator.GetIrrelevantPipingCalculationScenario<SemiProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationWithNaNs",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithNaNs())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationWithInfinities",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithInfinities())
                        .SetName(testNameFormat);
                    yield return new TestCaseData(
                            "folderWithSubfolderAndSemiProbabilisticCalculation",
                            new CalculationGroup
                            {
                                Name = "PK001_0001",
                                Children =
                                {
                                    PipingTestDataGenerator.GetPipingCalculationScenario<SemiProbabilisticPipingCalculationScenario>(),
                                    new CalculationGroup
                                    {
                                        Name = "PK001_0002",
                                        Children =
                                        {
                                            PipingTestDataGenerator.GetPipingCalculationScenario<SemiProbabilisticPipingCalculationScenario>()
                                        }
                                    }
                                }
                            }
                        )
                        .SetName(testNameFormat);
                }
            }

            [Test]
            [TestCaseSource(nameof(Calculations))]
            public void Write_ValidCalculation_ValidFile(string expectedFileName, ICalculationBase calculation)
            {
                // Setup
                string expectedXmlFilePath = TestHelper.GetTestDataPath(
                    TestDataPath.Riskeer.Piping.IO,
                    Path.Combine(nameof(PipingCalculationConfigurationExporter), $"{expectedFileName}.xml"));

                // Call and Assert
                WriteAndValidate(new[]
                {
                    calculation
                }, expectedXmlFilePath);
            }

            protected override IPipingCalculationScenario<PipingInput> CreateCalculation()
            {
                return new SemiProbabilisticPipingCalculationScenario();
            }
        }
    }
}