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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Configurations;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.IO.Writers;

namespace Ringtoets.StabilityStoneCover.IO.Test.Writers
{
    [TestFixture]
    public class StabilityStoneCoverCalculationConfigurationWriterTest
        : CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<
            StabilityStoneCoverCalculationConfigurationWriter,
            StabilityStoneCoverWaveConditionsCalculation>
    {
        [Test]
        public void Write_GroupWithCalculationAndOtherGroup_WritesOutCalculationAndGroupToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("Write_GroupWithCalculationAndOtherGroup.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.StabilityStoneCover.IO,
                Path.Combine(nameof(StabilityStoneCoverCalculationConfigurationWriter), "calculationAndGroupWithNesting.xml"));

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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
            try
            {
                var writer = new StabilityStoneCoverCalculationConfigurationWriter();

                // Call
                writer.Write(new ICalculationBase[]
                {
                    calculation,
                    calculationGroup
                }, filePath);

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

        protected override void AssertDefaultConstructedInstance(StabilityStoneCoverCalculationConfigurationWriter writer)
        {
            Assert.IsInstanceOf<WaveConditionsCalculationConfigurationWriter<StabilityStoneCoverWaveConditionsCalculation>>(writer);
        }
    }
}