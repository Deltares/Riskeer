// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Xml;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO;

namespace Ringtoets.GrassCoverErosionOutwards.IO.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsConfigurationWriterTest
    {
        [Test]
        public void Constructor_Always_CreateWaveConditionsInputWriter()
        {
            // Call
            var writer = new GrassCoverErosionOutwardsConfigurationWriter();

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputConfigurationWriter<GrassCoverErosionOutwardsWaveConditionsCalculation>>(writer);
        }

        [Test]
        public void Write_GroupWithCalculationAndOtherGroup_WritesOutCalculationAndGroupToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("Write_GroupWithCalculationAndOtherGroup.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.GrassCoverErosionOutwards.IO,
                Path.Combine(nameof(GrassCoverErosionOutwardsConfigurationWriter), "calculationAndGroupWithNesting.xml"));

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("hr_locatie_2"),
                    UpperBoundaryRevetment = (RoundedDouble) 2.5,
                    LowerBoundaryRevetment = (RoundedDouble) 1.3,
                    UpperBoundaryWaterLevels = (RoundedDouble) 2.2,
                    LowerBoundaryWaterLevels = (RoundedDouble) (-0.2),
                    StepSize = WaveConditionsInputStepSize.Two,
                    ForeshoreProfile = new TestForeshoreProfile("profiel2"),
                    Orientation = (RoundedDouble) 12.3,
                    UseForeshore = true,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 2.11,
                        Type = BreakWaterType.Wall
                    }
                }
            };

            var calculationGroup = new CalculationGroup("NestedGroup", false);
            var rootGroup = new CalculationGroup
            {
                Children =
                {
                    calculation,
                    calculationGroup
                }
            };
            try
            {
                var writer = new GrassCoverErosionOutwardsConfigurationWriter();

                // Call
                writer.Write(rootGroup, filePath);

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}