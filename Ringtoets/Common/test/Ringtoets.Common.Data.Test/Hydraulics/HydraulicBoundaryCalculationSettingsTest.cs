﻿using System.Collections.Generic;
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