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
using Ringtoets.MacroStabilityInwards.Integration.TestUtils;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationExporterTest
        : CustomSchemaCalculationConfigurationExporterDesignGuidelinesTestFixture<
            MacroStabilityInwardsCalculationConfigurationExporter,
            MacroStabilityInwardsCalculationConfigurationWriter,
            MacroStabilityInwardsCalculation,
            MacroStabilityInwardsCalculationConfiguration>
    {
        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                yield return new TestCaseData("calculationWithoutHydraulicLocation",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutHydraulicLocationAndAssessmentLevel())
                    .SetName("calculationWithoutHydraulicLocation");
                yield return new TestCaseData("calculationWithAssessmentLevel",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithAssessmentLevel())
                    .SetName("calculationWithAssessmentLevel");
                yield return new TestCaseData("calculationWithoutSurfaceLine",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutSurfaceLine())
                    .SetName("calculationWithoutSurfaceLine");
                yield return new TestCaseData("calculationWithoutSoilModel",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutSoilModel())
                    .SetName("calculationWithoutSoilModel");
                yield return new TestCaseData("calculationWithoutSoilProfile",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithoutSoilProfile())
                    .SetName("calculationWithoutSoilProfile");
                yield return new TestCaseData("calculationWithNaNs",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithNaNs())
                    .SetName("calculationWithNaNs");
                yield return new TestCaseData("calculationWithInfinities",
                                              MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculationWithInfinities())
                    .SetName("calculationWithInfinities");
            }
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            MacroStabilityInwardsCalculation calculation = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculation();

            MacroStabilityInwardsCalculation calculation2 = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculation();
            calculation2.Name = "PK001_0002 W1-6_4_1D1";
            calculation2.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_SCH_17", 0, 0);
            calculation2.InputParameters.SurfaceLine.Name = "PK001_0002";
            calculation2.InputParameters.StochasticSoilModel = new StochasticSoilModel(1, "PK001_0002_Macrostabiliteit", string.Empty);
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

        protected override MacroStabilityInwardsCalculation CreateCalculation()
        {
            return MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsCalculation();
        }

        protected override MacroStabilityInwardsCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new MacroStabilityInwardsCalculationConfigurationExporter(calculations, filePath);
        }
    }
}