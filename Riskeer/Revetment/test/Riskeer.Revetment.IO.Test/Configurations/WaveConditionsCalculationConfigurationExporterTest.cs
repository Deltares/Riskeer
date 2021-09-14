// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Configurations.Export;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.IO.Configurations;

namespace Riskeer.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationExporterTest : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
        WaveConditionsCalculationConfigurationExporter<WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>, WaveConditionsCalculationConfiguration, ICalculation<WaveConditionsInput>>,
        WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>,
        ICalculation<WaveConditionsInput>,
        WaveConditionsCalculationConfiguration>
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestWaveConditionsCalculationConfigurationExporter(Array.Empty<ICalculationBase>(), "filePath", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Export_VariousTargetProbabilities_ReturnTrueAndWritesFile()
        {
            // Setup
            var calculation1 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput())
            {
                Name = "Calculation 1 (lower limit)",
                InputParameters =
                {
                    WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit
                }
            };

            var calculation2 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput())
            {
                Name = "Calculation 2 (signaling)",
                InputParameters =
                {
                    WaterLevelType = WaveConditionsInputWaterLevelType.Signaling
                }
            };

            var calculation3 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput())
            {
                Name = "Calculation 3 (user defined)",
                InputParameters =
                {
                    WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                    CalculationsTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.003)
                }
            };

            var calculation4 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput())
            {
                Name = "Calculation 4 (none)",
                InputParameters =
                {
                    WaterLevelType = WaveConditionsInputWaterLevelType.None
                }
            };

            var calculationGroup2 = new CalculationGroup
            {
                Name = "group 2",
                Children =
                {
                    calculation2,
                    calculation4
                }
            };

            var calculationGroup = new CalculationGroup
            {
                Name = "group 1",
                Children =
                {
                    calculation1,
                    calculation3,
                    calculationGroup2
                }
            };

            string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Revetment.IO,
                                                                    Path.Combine(
                                                                        nameof(WaveConditionsCalculationConfigurationExporter<CalculationConfigurationWriter<WaveConditionsCalculationConfiguration>, WaveConditionsCalculationConfiguration, ICalculation<WaveConditionsInput>>),
                                                                        "targetProbabilityConfiguration.xml"));
            // Call & Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        protected override ICalculation<WaveConditionsInput> CreateCalculation()
        {
            return new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput());
        }

        protected override WaveConditionsCalculationConfigurationExporter<WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>, WaveConditionsCalculationConfiguration, ICalculation<WaveConditionsInput>> CallConfigurationFilePathConstructor(
            IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new TestWaveConditionsCalculationConfigurationExporter(calculations, filePath, new AssessmentSectionStub());
        }

        private class TestWaveConditionsCalculationConfigurationExporter : WaveConditionsCalculationConfigurationExporter<WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>, WaveConditionsCalculationConfiguration, ICalculation<WaveConditionsInput>>
        {
            public TestWaveConditionsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath, IAssessmentSection assessmentSection)
                : base(calculations, new TestWaveConditionsCalculationConfigurationWriter(filePath), assessmentSection) {}

            protected override WaveConditionsCalculationConfiguration ToConfiguration(ICalculation<WaveConditionsInput> calculation)
            {
                var configuration = new WaveConditionsCalculationConfiguration(calculation.Name);
                SetConfigurationProperties(configuration, calculation);
                return configuration;
            }
        }

        private class TestWaveConditionsCalculationConfigurationWriter : WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>
        {
            public TestWaveConditionsCalculationConfigurationWriter(string filePath) : base(filePath) {}

            protected override void WriteWaveConditionsSpecificParameters(XmlWriter writer, WaveConditionsCalculationConfiguration configuration) {}

            protected override int GetConfigurationVersion()
            {
                return 2;
            }
        }
    }
}