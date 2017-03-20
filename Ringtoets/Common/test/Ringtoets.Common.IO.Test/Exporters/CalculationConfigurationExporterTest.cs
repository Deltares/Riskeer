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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Exporters;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Test.Exporters
{
    [TestFixture]
    public class CalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            SimpleCalculationConfigurationExporter,
            SimpleCalculationConfigurationWriter,
            TestCalculation>
    {
        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            var calculation = new TestCalculation("Calculation A");
            var calculation2 = new TestCalculation("Calculation B");

            var calculationGroup2 = new CalculationGroup("Group B", false)
            {
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup("Group A", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            string testFileSubPath = Path.Combine(
                nameof(CalculationConfigurationExporter<SimpleCalculationConfigurationWriter, TestCalculation>),
                "folderWithSubfolderAndCalculation.xml");
            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.Common.IO,
                testFileSubPath);

            // Call and Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        protected override TestCalculation CreateCalculation()
        {
            return new TestCalculation("TestCalculation A");
        }
    }

    public class SimpleCalculationConfigurationExporter : CalculationConfigurationExporter<SimpleCalculationConfigurationWriter, TestCalculation>
    {
        public SimpleCalculationConfigurationExporter(IEnumerable<ICalculationBase> configuration, string targetFilePath) : base(configuration, targetFilePath) {}
    }

    public class SimpleCalculationConfigurationWriter : CalculationConfigurationWriter<TestCalculation>
    {
        protected override void WriteCalculation(TestCalculation calculation, XmlWriter writer)
        {
            writer.WriteElementString("calculation", calculation.Name);
        }
    }
}