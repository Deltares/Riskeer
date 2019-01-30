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
using System.Xml;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
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
        protected override ICalculation<WaveConditionsInput> CreateCalculation()
        {
            return new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput());
        }

        protected override WaveConditionsCalculationConfigurationExporter<WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>, WaveConditionsCalculationConfiguration, ICalculation<WaveConditionsInput>> CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new TestWaveConditionsCalculationConfigurationExporter(calculations, filePath);
        }

        private class TestWaveConditionsCalculationConfigurationExporter : WaveConditionsCalculationConfigurationExporter<WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>, WaveConditionsCalculationConfiguration, ICalculation<WaveConditionsInput>>
        {
            public TestWaveConditionsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
                : base(calculations, filePath) {}

            protected override WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration> CreateWriter(string filePath)
            {
                return new TestWaveConditionsCalculationConfigurationWriter(filePath);
            }

            protected override WaveConditionsCalculationConfiguration ToConfiguration(ICalculation<WaveConditionsInput> calculation)
            {
                return new WaveConditionsCalculationConfiguration("test");
            }
        }

        private class TestWaveConditionsCalculationConfigurationWriter : WaveConditionsCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>
        {
            public TestWaveConditionsCalculationConfigurationWriter(string filePath) : base(filePath) {}

            protected override void WriteConfigurationCategoryTypeWhenAvailable(XmlWriter writer, WaveConditionsCalculationConfiguration configuration) {}
        }
    }
}