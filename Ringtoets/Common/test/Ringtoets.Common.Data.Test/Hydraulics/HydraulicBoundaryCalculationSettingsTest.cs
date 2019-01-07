// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsTest
    {
        [Test]
        [TestCaseSource(nameof(GetTestCasesWithAllParameters))]
        public void Constructor_WithArguments_ExpectedValues(string hydraulicBoundaryDatabaseFilePath,
                                                             string hlcdFilePath,
                                                             string preprocessorDirectory)
        {
            // Call
            var settings = new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabaseFilePath,
                                                                    hlcdFilePath,
                                                                    preprocessorDirectory);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(preprocessorDirectory, settings.PreprocessorDirectory);
        }

        [Test]
        [TestCaseSource(nameof(GetTestCasesWithoutHlcdParameter))]
        public void Constructor_ExpectedValues(string hydraulicBoundaryDatabaseFilePath,
                                               string preprocessorDirectory)
        {
            // Call
            var settings = new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabaseFilePath,
                                                                    preprocessorDirectory);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.IsNull(settings.HlcdFilePath);
            Assert.AreEqual(preprocessorDirectory, settings.PreprocessorDirectory);
        }

        private static IEnumerable<TestCaseData> GetTestCasesWithAllParameters()
        {
            yield return new TestCaseData("D:\\HydraulicBoundaryDatabase.sqlite",
                                          "D:\\HLCD.sqlite",
                                          "D:\\")
                .SetName("All inputs with values");
            yield return new TestCaseData("  ", "  ", "  ")
                .SetName("All inputs whitespace");
            yield return new TestCaseData(string.Empty, string.Empty, string.Empty)
                .SetName("All inputs empty");
            yield return new TestCaseData(null, null, null)
                .SetName("All inputs null");
        }

        private static IEnumerable<TestCaseData> GetTestCasesWithoutHlcdParameter()
        {
            yield return new TestCaseData("D:\\HydraulicBoundaryDatabase.sqlite",
                                          "D:\\")
                .SetName("All inputs with values");
            yield return new TestCaseData("  ", "  ")
                .SetName("All inputs whitespace");
            yield return new TestCaseData(string.Empty, string.Empty)
                .SetName("All inputs empty");
            yield return new TestCaseData(null, null)
                .SetName("All inputs null");
        }
    }
}