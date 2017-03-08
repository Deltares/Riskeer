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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Writers;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Test
{
    [TestFixture]
    public class WaveConditionsInputConfigurationWriterTest
    {
        [Test]
        public void Constructor_Always_ReturnsConfigurationWriter()
        {
            // Call
            var writer = new SimpleWaveConditionsInputConfigurationWriter();

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationWriter<SimpleWaveConditionsCalculation>>(writer);
        }

        [Test]
        public void WriteConfiguration_SingleCalculation_WritesCompleteConfigurationToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("WriteConfiguration_SingleCalculation.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Revetment.IO,
                Path.Combine(nameof(WaveConditionsInputConfigurationWriter<ICalculation>), "completeConfiguration.xml"));

            var calculation = new SimpleWaveConditionsCalculation
            {
                Name = "Berekening 1",
                Input = new WaveConditionsInput
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1"),
                    UpperBoundaryRevetment = (RoundedDouble) 1.5,
                    LowerBoundaryRevetment = (RoundedDouble) 0.5,
                    UpperBoundaryWaterLevels = (RoundedDouble) 1.4,
                    LowerBoundaryWaterLevels = (RoundedDouble) 0.6,
                    StepSize = WaveConditionsInputStepSize.One,
                    ForeshoreProfile = new TestForeshoreProfile("profiel1"),
                    Orientation = (RoundedDouble) 67.1,
                    UseForeshore = true,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 1.234,
                        Type = BreakWaterType.Dam
                    }
                }
            };

            // Call
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings
                {
                    Indent = true
                }))
                {
                    // Call
                    new SimpleWaveConditionsInputConfigurationWriter().PublicWriteCalculation(calculation, writer);
                }

                // Assert
                string actualXml = File.ReadAllText(filePath);
                string expectedXml = File.ReadAllText(expectedXmlFilePath);

                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }

            // Assert
        }
    }

    public class SimpleWaveConditionsInputConfigurationWriter : WaveConditionsInputConfigurationWriter<SimpleWaveConditionsCalculation>
    {
        protected override void WriteCalculation(SimpleWaveConditionsCalculation calculation, XmlWriter writer)
        {
            WriteCalculation(calculation.Name, calculation.Input, writer);
        }

        public void PublicWriteCalculation(SimpleWaveConditionsCalculation calculation, XmlWriter writer)
        {
            WriteCalculation(calculation, writer);
        }
    }

    public class SimpleWaveConditionsCalculation : Observable, ICalculation
    {
        public string Name { get; set; }
        public bool HasOutput { get; }
        public Comment Comments { get; }

        public void ClearOutput()
        {
            throw new System.NotImplementedException();
        }

        public WaveConditionsInput Input { get; set; }
    }
}