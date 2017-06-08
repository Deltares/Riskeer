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

using System.IO;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            MacroStabilityInwardsCalculationConfigurationWriter,
            MacroStabilityInwardsCalculationConfiguration>
    {
        [Test]
        public void Write_CalculationGroupsAndCalculation_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.xml");

            var surfaceline = new RingtoetsMacroStabilityInwardsSurfaceLine
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
            calculation2.HydraulicBoundaryLocationName = "PUNT_SCH_17";
            calculation2.SurfaceLineName = "PK001_0002";
            calculation2.StochasticSoilModelName = "PK001_0002_Macrostabiliteit";
            calculation2.StochasticSoilProfileName = "W1-6_4_1D1";

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
                new MacroStabilityInwardsCalculationConfigurationWriter(filePath).Write(new[]
                {
                    calculationGroup
                });

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string actualXml = File.ReadAllText(filePath);
                string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO,
                                                                        Path.Combine(nameof(MacroStabilityInwardsCalculationConfigurationWriter),
                                                                                     "folderWithSubfolderAndCalculation.xml"));
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private static MacroStabilityInwardsCalculationConfiguration CreateFullCalculationConfiguration()
        {
            return new MacroStabilityInwardsCalculationConfiguration("PK001_0001 W1-6_0_1D1")
            {
                HydraulicBoundaryLocationName = "PUNT_KAT_18",
                SurfaceLineName = "PK001_0001",
                StochasticSoilModelName = "PK001_0001_Macrostabiliteit",
                StochasticSoilProfileName = "W1-6_0_1D1"
            };
        }

        protected override MacroStabilityInwardsCalculationConfigurationWriter CreateWriterInstance(string filePath)
        {
            return new MacroStabilityInwardsCalculationConfigurationWriter(filePath);
        }
    }
}