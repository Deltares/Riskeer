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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            PipingCalculationConfigurationExporter,
            PipingCalculationConfigurationWriter,
            PipingCalculationScenario,
            PipingCalculationConfiguration>
    {
        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                const string testNameFormat = "{m}({0:40}.xml)";

                yield return new TestCaseData("calculationWithoutHydraulicLocation",
                                              PipingTestDataGenerator.GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithAssessmentLevel",
                                              PipingTestDataGenerator.GetPipingCalculationScenarioWithAssessmentLevel())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithoutSurfaceLine",
                                              PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSurfaceLine())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithoutSoilModel",
                                              PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilModel())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithoutSoilProfile",
                                              PipingTestDataGenerator.GetPipingCalculationScenarioWithoutSoilProfile())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationIrrelevant",
                                              PipingTestDataGenerator.GetIrrelevantPipingCalculationScenario())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithNaNs",
                                              PipingTestDataGenerator.GetPipingCalculationScenarioWithNaNs())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithInfinities",
                                              PipingTestDataGenerator.GetPipingCalculationScenarioWithInfinities())
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "folderWithSubfolderAndCalculation",
                        new CalculationGroup
                        {
                            Name = "PK001_0001",
                            Children =
                            {
                                PipingTestDataGenerator.GetPipingCalculationScenario(),
                                new CalculationGroup
                                {
                                    Name = "PK001_0002",
                                    Children =
                                    {
                                        PipingTestDataGenerator.GetPipingCalculationScenario()
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
                TestDataPath.Ringtoets.Piping.IO,
                Path.Combine(nameof(PipingCalculationConfigurationExporter), $"{expectedFileName}.xml"));

            // Call and Assert
            WriteAndValidate(new[]
            {
                calculation
            }, expectedXmlFilePath);
        }

        protected override PipingCalculationScenario CreateCalculation()
        {
            return new PipingCalculationScenario(new GeneralPipingInput());
        }

        protected override PipingCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new PipingCalculationConfigurationExporter(calculations, filePath);
        }
    }
}