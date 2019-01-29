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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Configurations;

namespace Riskeer.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            MacroStabilityInwardsCalculationConfigurationExporter,
            MacroStabilityInwardsCalculationConfigurationWriter,
            MacroStabilityInwardsCalculationScenario,
            MacroStabilityInwardsCalculationConfiguration>
    {
        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                const string testNameFormat = "{m}({0:40}.xml)";

                yield return new TestCaseData("calculationWithoutHydraulicLocation",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithAssessmentLevel",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithAssessmentLevel())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithoutSurfaceLine",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutSurfaceLine())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithoutSoilModel",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutSoilModel())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithoutSoilProfile",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithoutSoilProfile())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationIrrelevant",
                                              MacroStabilityInwardsTestDataGenerator.GetIrrelevantMacroStabilityInwardsCalculationScenario())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithNaNs",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithNaNs())
                    .SetName(testNameFormat);
                yield return new TestCaseData("calculationWithInfinities",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenarioWithInfinities())
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "folderWithSubfolderAndCalculation",
                        new CalculationGroup
                        {
                            Name = "PK001_0001",
                            Children =
                            {
                                MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenario(),
                                new CalculationGroup
                                {
                                    Name = "PK001_0002",
                                    Children =
                                    {
                                        MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenario()
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
                TestDataPath.Ringtoets.MacroStabilityInwards.IO,
                Path.Combine(nameof(MacroStabilityInwardsCalculationConfigurationExporter), $"{expectedFileName}.xml"));

            // Call and Assert
            WriteAndValidate(new[]
            {
                calculation
            }, expectedXmlFilePath);
        }

        protected override MacroStabilityInwardsCalculationScenario CreateCalculation()
        {
            return MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenario();
        }

        protected override MacroStabilityInwardsCalculationConfigurationExporter CallConfigurationFilePathConstructor(
            IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new MacroStabilityInwardsCalculationConfigurationExporter(calculations, filePath);
        }
    }
}