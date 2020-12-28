// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationExporterTest
    {
        [Test]
        public void Export_NotSupportedCalculationScenarioType_ThrowsNotSupportedException()
        {
            // Setup
            var exporter = new PipingCalculationConfigurationExporter(new[]
            {
                new TestPipingCalculationScenario()
            }, "NotSupportedCalculationType.xml");

            // Call
            void Call() => exporter.Export();

            // Assert
            Assert.Throws<NotSupportedException>(Call);
        }

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

            protected void PerformTest(string expectedFileName, ICalculationBase calculation)
            {
                // Setup
                string expectedXmlFilePath = TestHelper.GetTestDataPath(
                    TestDataPath.Riskeer.Piping.IO,
                    Path.Combine(nameof(PipingCalculationConfigurationExporter), $"{expectedFileName}.xml"));

                // Call & Assert
                WriteAndValidate(new[]
                {
                    calculation
                }, expectedXmlFilePath);
            }
        }

        private class SemiProbabilisticPipingCalculationConfigurationExporterTest : PipingCalculationConfigurationExporterTestFixture
        {
            private static IEnumerable<TestCaseData> Calculations
            {
                get
                {
                    yield return new TestCaseData("semiProbabilisticCalculationWithoutHydraulicLocation",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutHydraulicLocation<SemiProbabilisticPipingCalculationScenario>())
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
                                                  PipingTestDataGenerator.GetSemiProbabilisticPipingCalculationScenarioWithNaNs())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("semiProbabilisticCalculationWithInfinities",
                                                  PipingTestDataGenerator.GetSemiProbabilisticPipingCalculationScenarioWithInfinities())
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
                PerformTest(expectedFileName, calculation);
            }

            protected override IPipingCalculationScenario<PipingInput> CreateCalculation()
            {
                return new SemiProbabilisticPipingCalculationScenario();
            }
        }

        private class ProbabilisticPipingCalculationConfigurationExporterTest : PipingCalculationConfigurationExporterTestFixture
        {
            private static IEnumerable<TestCaseData> Calculations
            {
                get
                {
                    yield return new TestCaseData("probabilisticCalculationWithoutHydraulicLocation",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutHydraulicLocation<ProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("probabilisticCalculationWithoutSurfaceLine",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSurfaceLine<ProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("probabilisticCalculationWithoutSoilModel",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilModel<ProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("probabilisticCalculationWithoutSoilProfile",
                                                  PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilProfile<ProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("probabilisticCalculationIrrelevant",
                                                  PipingTestDataGenerator.GetIrrelevantPipingCalculationScenario<ProbabilisticPipingCalculationScenario>())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("probabilisticCalculationWithNaNs",
                                                  PipingTestDataGenerator.GetProbabilisticPipingCalculationScenarioWithNaNs())
                        .SetName(testNameFormat);
                    yield return new TestCaseData("probabilisticCalculationWithInfinities",
                                                  PipingTestDataGenerator.GetProbabilisticPipingCalculationScenarioWithInfinities())
                        .SetName(testNameFormat);
                    yield return new TestCaseData(
                            "folderWithSubfolderAndProbabilisticCalculation",
                            new CalculationGroup
                            {
                                Name = "PK001_0001",
                                Children =
                                {
                                    PipingTestDataGenerator.GetPipingCalculationScenario<ProbabilisticPipingCalculationScenario>(),
                                    new CalculationGroup
                                    {
                                        Name = "PK001_0002",
                                        Children =
                                        {
                                            PipingTestDataGenerator.GetPipingCalculationScenario<ProbabilisticPipingCalculationScenario>()
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
                PerformTest(expectedFileName, calculation);
            }

            protected override IPipingCalculationScenario<PipingInput> CreateCalculation()
            {
                return new ProbabilisticPipingCalculationScenario();
            }
        }
    }
}