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
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.Configurations.Export
{
    [TestFixture]
    public class CalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            SimpleCalculationConfigurationExporter,
            TestCalculationConfigurationWriter,
            TestCalculation,
            TestConfigurationItem>
    {
        protected override TestCalculation CreateCalculation()
        {
            return new TestCalculation("some name");
        }

        protected override SimpleCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new SimpleCalculationConfigurationExporter(calculations, filePath);
        }
    }

    public class SimpleCalculationConfigurationExporter
        : CalculationConfigurationExporter<TestCalculationConfigurationWriter, TestCalculation, TestConfigurationItem>
    {
        public SimpleCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath) : base(calculations, filePath) {}

        protected override TestCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new TestCalculationConfigurationWriter(filePath);
        }

        protected override TestConfigurationItem ToConfiguration(TestCalculation calculation)
        {
            return null;
        }
    }
}