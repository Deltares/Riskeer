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

using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithFilePath_ReturnsExpectedSettings()
        {
            // Setup
            const string hydraulicBoundaryDatabaseFilePath = "some//FilePath//HRD dutch coast south.sqlite";
            const string hlcdFilePath = "some//FilePath//HLCD dutch coast south.sqlite";

            var database = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath
            };
            database.HydraulicLocationConfigurationSettings.SetValues(hlcdFilePath, string.Empty, 10, string.Empty,
                                                                      null, null, null, null, null, null);

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(database);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
        }

        [Test]
        [TestCaseSource(nameof(GetPreprocessorConfigurations))]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithVariousPreprocessorConfigurations_ReturnsExpectedSettings(
            HydraulicBoundaryDatabase database,
            string expectedPreprocessorDirectory)
        {
            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(database);

            // Assert
            Assert.AreEqual(expectedPreprocessorDirectory, settings.PreprocessorDirectory);
        }

        private static IEnumerable<TestCaseData> GetPreprocessorConfigurations()
        {
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var hydraulicBoundaryDatabaseCanUsePreprocessorFalse = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabaseCanUsePreprocessorFalse);
            yield return new TestCaseData(hydraulicBoundaryDatabaseCanUsePreprocessorFalse,
                                          string.Empty)
                .SetName("CanUsePreprocessorFalse");

            var hydraulicBoundaryDatabaseUsePreprocessorFalse = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                CanUsePreprocessor = true,
                PreprocessorDirectory = "Directory"
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabaseUsePreprocessorFalse);
            yield return new TestCaseData(hydraulicBoundaryDatabaseUsePreprocessorFalse, string.Empty)
                .SetName("UsePreprocessorFalseWithPreprocessorDirectory");

            const string preprocessorDirectory = "Directory";
            var hydraulicBoundaryDatabaseUsePreprocessorTrue = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = preprocessorDirectory
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabaseUsePreprocessorTrue);
            yield return new TestCaseData(hydraulicBoundaryDatabaseUsePreprocessorTrue, preprocessorDirectory)
                .SetName("UsePreprocessorTrueWithPreprocessorDirectory");
        }
    }
}