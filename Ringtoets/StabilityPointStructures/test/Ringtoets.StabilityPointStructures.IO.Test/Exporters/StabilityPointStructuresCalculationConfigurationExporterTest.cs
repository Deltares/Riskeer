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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.IO.Exporters;
using Ringtoets.StabilityPointStructures.IO.Writers;

namespace Ringtoets.StabilityPointStructures.IO.Test.Exporters
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationExporterTest
        : CustomSchemaCalculationConfigurationExporterDesignGuidelinesTestFixture<
            StabilityPointStructuresCalculationConfigurationExporter,
            StabilityPointStructuresCalculationConfigurationWriter,
            StructuresCalculation<StabilityPointStructuresInput>,
            StabilityPointStructuresCalculationConfiguration>
    {

        private static IEnumerable<TestCaseData> Calculations
        {
            get
            {
               yield return new TestCaseData("sparseConfiguration", new[]
                    {
                        CreateSparseCalculation()
                    })
                    .SetName("Calculation configuration with none of its parameters set");
            }
        }

        private static ICalculation CreateSparseCalculation()
        {
            return new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "sparse config"
            };
        }

        protected override StructuresCalculation<StabilityPointStructuresInput> CreateCalculation()
        {
            return new TestStabilityPointStructuresCalculation();
        }

        protected override StabilityPointStructuresCalculationConfigurationExporter CallConfigurationFilePathConstructor(
            IEnumerable<ICalculationBase> calculations,
            string filePath)
        {
            return new StabilityPointStructuresCalculationConfigurationExporter(calculations, filePath);
        }

        [Test]
        [TestCaseSource(nameof(Calculations))]
        public void Export_ValidData_ReturnTrueAndWritesFile(string fileName, ICalculationBase[] calculations)
        {
            // Setup
            string testDirectory = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.StabilityPointStructures.IO,
                nameof(StabilityPointStructuresCalculationConfigurationExporter));

            string expectedXmlFilePath = Path.Combine(testDirectory, $"{fileName}.xml");

            // Call and Assert
            WriteAndValidate(calculations, expectedXmlFilePath);
        }
    }
}