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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Piping.IO.Configurations;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationWriterTest
        : CustomSchemaCalculationConfigurationWriterDesignGuidelinesTestFixture<
            PipingCalculationConfigurationWriter,
            PipingCalculationConfiguration>
    {
        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {

                var calculation = CreateFullCalculationConfiguration();
                calculation.HydraulicBoundaryLocation = null;
                calculation.EntryPointL = 0.0;
                calculation.ExitPointL = 10;
                yield return new TestCaseData("calculationWithoutHydraulicLocation",
                                              calculation)
                    .SetName("calculationWithoutHydraulicLocation");

                calculation = CreateFullCalculationConfiguration();
                calculation.AssessmentLevel = 3.0;
                calculation.EntryPointL = 0.0;
                calculation.ExitPointL = 10;
                yield return new TestCaseData("calculationWithAssessmentLevel",
                                              calculation)
                    .SetName("calculationWithAssessmentLevel");

                calculation = CreateFullCalculationConfiguration();
                calculation.SurfaceLine = null;
                yield return new TestCaseData("calculationWithoutSurfaceLine",
                                              calculation)
                    .SetName("calculationWithoutSurfaceLine");

                calculation = CreateFullCalculationConfiguration();
                calculation.StochasticSoilModel = null;
                calculation.EntryPointL = 0.0;
                calculation.ExitPointL = 10;
                yield return new TestCaseData("calculationWithoutSoilModel",
                                              calculation)
                    .SetName("calculationWithoutSoilModel");

                calculation = CreateFullCalculationConfiguration();
                calculation.StochasticSoilProfile = null;
                calculation.EntryPointL = 0.0;
                calculation.ExitPointL = 10;
                yield return new TestCaseData("calculationWithoutSoilProfile",
                                              calculation)
                    .SetName("calculationWithoutSoilProfile");

                calculation = CreateFullCalculationConfiguration();
                calculation.AssessmentLevel = double.NaN;
                calculation.EntryPointL = double.NaN;
                calculation.ExitPointL = double.NaN;
                calculation.PhreaticLevelExit.Mean = double.NaN;
                calculation.PhreaticLevelExit.StandardDeviation = double.NaN;
                calculation.DampingFactorExit.Mean = double.NaN;
                calculation.DampingFactorExit.StandardDeviation = double.NaN;
                yield return new TestCaseData("calculationWithNaNs",
                                              calculation)
                    .SetName("calculationWithNaNs");

                calculation = CreateFullCalculationConfiguration();
                calculation.AssessmentLevel = double.NegativeInfinity;
                calculation.EntryPointL = double.NegativeInfinity;
                calculation.ExitPointL = double.PositiveInfinity;
                calculation.PhreaticLevelExit.Mean = double.NegativeInfinity;
                calculation.PhreaticLevelExit.StandardDeviation = double.PositiveInfinity;
                calculation.DampingFactorExit.Mean = double.PositiveInfinity;
                calculation.DampingFactorExit.StandardDeviation = double.PositiveInfinity;
                yield return new TestCaseData("calculationWithInfinities",
                                              calculation)
                    .SetName("calculationWithInfinities");
            }
        }

        [Test]
        public void Write_CalculationGroupsAndCalculation_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            var surfaceline = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5),
                Name = "PK001_0001"
            };
            surfaceline.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });

            var calculation = CreateFullCalculationConfiguration();

            var calculation2 = CreateFullCalculationConfiguration();
            calculation2.Name = "PK001_0002 W1-6_4_1D1";
            calculation2.HydraulicBoundaryLocation = "PUNT_SCH_17";
            calculation2.SurfaceLine = "PK001_0002";
            calculation2.StochasticSoilModel = "PK001_0002_Piping";
            calculation2.StochasticSoilProfile = "W1-6_4_1D1";
            calculation2.EntryPointL = 0.3;
            calculation2.ExitPointL = 0.4;

            var calculationGroup2 = new CalculationGroupConfiguration("PK001_0002", new IConfigurationItem[]
            {
                calculation2
            });

            var calculationGroup = new CalculationGroupConfiguration("PK001_0001", new IConfigurationItem[]
            {
                calculation,
                calculationGroup2
            });

            try
            {
                // Call
                new PipingCalculationConfigurationWriter(filePath).Write(new[]
                {
                    calculationGroup
                });

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                                        Path.Combine("PipingCalculationConfigurationWriter",
                                                                                     "folderWithSubfolderAndCalculation.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static PipingCalculationConfiguration CreateFullCalculationConfiguration()
        {
            return new PipingCalculationConfiguration("PK001_0001 W1-6_0_1D1")
            {
                HydraulicBoundaryLocation = "PUNT_KAT_18",
                SurfaceLine = "PK001_0001",
                StochasticSoilModel = "PK001_0001_Piping",
                StochasticSoilProfile = "W1-6_0_1D1",
                EntryPointL = 0.1,
                ExitPointL = 0.2,
                PhreaticLevelExit = new StochastConfiguration
                {
                    Mean = 0,
                    StandardDeviation = 0.1
                },
                DampingFactorExit = new StochastConfiguration
                {
                    Mean = 0.7,
                    StandardDeviation = 0.1
                }
            };
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Write_ValidCalculation_ValidFile(string expectedFileName, PipingCalculationConfiguration calculation)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            try
            {
                // Call
                new PipingCalculationConfigurationWriter(filePath).Write(new[]
                {
                    calculation
                });

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, Path.Combine("PipingCalculationConfigurationWriter", $"{expectedFileName}.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        protected override PipingCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new PipingCalculationConfigurationWriter(filePath);
        }
    }
}