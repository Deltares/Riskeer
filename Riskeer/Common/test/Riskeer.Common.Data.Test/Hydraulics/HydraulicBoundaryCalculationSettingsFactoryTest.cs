// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithFilePath_ReturnsExpectedSettings()
        {
            // Setup
            const string hydraulicBoundaryDatabaseFilePath = "some//FilePath//HRD dutch coast south.sqlite";
            const string hlcdFilePath = "some//FilePath//HLCD dutch coast south.sqlite";
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = hydraulicBoundaryDatabaseFilePath
            };
            hydraulicBoundaryData.HydraulicLocationConfigurationSettings.SetValues(hlcdFilePath, string.Empty, 10, string.Empty,
                                                                      usePreprocessorClosure, null, null, null, null, null, null);

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HrdFilePath);
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
        }

        [Test]
        [TestCaseSource(nameof(GetPreprocessorConfigurations))]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithVariousPreprocessorConfigurations_ReturnsExpectedSettings(
            HydraulicBoundaryData hydraulicBoundaryData,
            string expectedPreprocessorDirectory)
        {
            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData);

            // Assert
            Assert.AreEqual(expectedPreprocessorDirectory, settings.PreprocessorDirectory);
        }

        private static IEnumerable<TestCaseData> GetPreprocessorConfigurations()
        {
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var hydraulicBoundaryDataCanUsePreprocessorFalse = new HydraulicBoundaryData
            {
                FilePath = validFilePath
            };
            HydraulicBoundaryDataTestHelper.SetHydraulicLocationConfigurationSettings(hydraulicBoundaryDataCanUsePreprocessorFalse);
            yield return new TestCaseData(hydraulicBoundaryDataCanUsePreprocessorFalse,
                                          string.Empty)
                .SetName("CanUsePreprocessorFalse");

            var hydraulicBoundaryDataUsePreprocessorFalse = new HydraulicBoundaryData
            {
                FilePath = validFilePath,
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    PreprocessorDirectory = "Directory"
                }
            };
            HydraulicBoundaryDataTestHelper.SetHydraulicLocationConfigurationSettings(hydraulicBoundaryDataUsePreprocessorFalse);
            yield return new TestCaseData(hydraulicBoundaryDataUsePreprocessorFalse, string.Empty)
                .SetName("UsePreprocessorFalseWithPreprocessorDirectory");

            const string preprocessorDirectory = "Directory";
            var hydraulicBoundaryDataUsePreprocessorTrue = new HydraulicBoundaryData
            {
                FilePath = validFilePath,
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = true,
                    PreprocessorDirectory = preprocessorDirectory
                }
            };
            HydraulicBoundaryDataTestHelper.SetHydraulicLocationConfigurationSettings(hydraulicBoundaryDataUsePreprocessorTrue);
            yield return new TestCaseData(hydraulicBoundaryDataUsePreprocessorTrue, preprocessorDirectory)
                .SetName("UsePreprocessorTrueWithPreprocessorDirectory");
        }
    }
}