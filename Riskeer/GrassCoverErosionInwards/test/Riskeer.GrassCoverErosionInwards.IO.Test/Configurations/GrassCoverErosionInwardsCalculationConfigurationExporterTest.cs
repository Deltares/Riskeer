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

using System.Collections.Generic;
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.IO.Configurations;

namespace Riskeer.GrassCoverErosionInwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            GrassCoverErosionInwardsCalculationConfigurationExporter,
            GrassCoverErosionInwardsCalculationConfigurationWriter,
            GrassCoverErosionInwardsCalculationScenario,
            GrassCoverErosionInwardsCalculationConfiguration>
    {
        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
                yield return new TestCaseData("completeConfiguration", new[]
                    {
                        CreateFullCalculation()
                    })
                    .SetName("Calculation configuration with all parameters set");

                yield return new TestCaseData("sparseConfiguration", new[]
                    {
                        CreateSparseCalculation()
                    })
                    .SetName("Calculation configuration with none of its parameters set");

                yield return new TestCaseData("folderWithSubfolderAndCalculation", new[]
                    {
                        CreateNestedCalculation()
                    })
                    .SetName("Calculation configuration with hierarchy");

                yield return new TestCaseData("calculationScenarioIrrelevant", new[]
                    {
                        CreateIrrelevantCalculationScenario()
                    })
                    .SetName("Calculation configuration with scenario is relevant false");
            }
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Export_ValidData_ReturnTrueAndWritesFile(string fileName, ICalculationBase[] calculations)
        {
            // Setup
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Riskeer.GrassCoverErosionInwards.IO,
                Path.Combine(nameof(GrassCoverErosionInwardsCalculationConfigurationExporter), $"{fileName}.xml"));

            // Call and Assert
            WriteAndValidate(calculations, expectedXmlFilePath);
        }

        private static GrassCoverErosionInwardsCalculationScenario CreateFullCalculation()
        {
            return new GrassCoverErosionInwardsCalculationScenario
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new[]
                    {
                        new RoughnessPoint(new Point2D(0, 0), 2.1),
                        new RoughnessPoint(new Point2D(1, 1), 3.9),
                        new RoughnessPoint(new Point2D(2, 2), 5.2)
                    }, new[]
                    {
                        new Point2D(1, 0),
                        new Point2D(3, 4),
                        new Point2D(6, 5)
                    }, new BreakWater(BreakWaterType.Caisson, 0), new DikeProfile.ConstructionProperties
                    {
                        Id = "Dijkprofiel ID",
                        Name = "Dijkprofiel",
                        DikeHeight = 3.45
                    })
                }
            };
        }

        private static GrassCoverErosionInwardsCalculationScenario CreateSparseCalculation()
        {
            return new GrassCoverErosionInwardsCalculationScenario
            {
                Name = "sparse config"
            };
        }

        private static CalculationGroup CreateNestedCalculation()
        {
            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Location1")
                }
            };
            var calculation2 = new GrassCoverErosionInwardsCalculationScenario
            {
                Name = "Berekening 2"
            };

            var calculationGroup2 = new CalculationGroup
            {
                Name = "Nested",
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup
            {
                Name = "Testmap",
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            return calculationGroup;
        }

        private static GrassCoverErosionInwardsCalculationScenario CreateIrrelevantCalculationScenario()
        {
            return new GrassCoverErosionInwardsCalculationScenario
            {
                Name = "irrelevant",
                Contribution = (RoundedDouble) 0.5432,
                IsRelevant = false
            };
        }

        protected override GrassCoverErosionInwardsCalculationScenario CreateCalculation()
        {
            return new GrassCoverErosionInwardsCalculationScenario();
        }

        protected override GrassCoverErosionInwardsCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new GrassCoverErosionInwardsCalculationConfigurationExporter(calculations, filePath);
        }
    }
}