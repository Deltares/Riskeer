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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Integration.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
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
            }
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenario();

            MacroStabilityInwardsCalculationScenario calculation2 = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationScenario();
            calculation2.Name = "PK001_0002 W1-6_4_1D1";
            calculation2.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_SCH_17", 0, 0);
            calculation2.InputParameters.SurfaceLine.Name = "PK001_0002";
            calculation2.InputParameters.StochasticSoilModel = new StochasticSoilModel("PK001_0002_Macrostabiliteit");
            calculation2.InputParameters.StochasticSoilProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile("W1-6_4_1D1", 0, new[]
                {
                    new MacroStabilityInwardsSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var calculationGroup2 = new CalculationGroup("PK001_0002", false)
            {
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup("PK001_0001", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO,
                                                                    Path.Combine(nameof(MacroStabilityInwardsCalculationConfigurationExporter),
                                                                                 "folderWithSubfolderAndCalculation.xml"));

            // Call and Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Write_ValidCalculation_ValidFile(string expectedFileName, MacroStabilityInwardsCalculation calculation)
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

        protected override MacroStabilityInwardsCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new MacroStabilityInwardsCalculationConfigurationExporter(calculations, filePath);
        }
    }
}