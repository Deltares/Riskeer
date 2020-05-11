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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(null, new TestHydraulicBoundaryLocation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateSettings_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(new AssessmentSectionStub(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithFilePath_ReturnsExpectedSettings()
        {
            // Setup
            const string hydraulicBoundaryDatabaseFilePath = "some//FilePath//HRD dutch coast south.sqlite";
            const string hlcdFilePath = "some//FilePath//HLCD dutch coast south.sqlite";
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabases.First();
            hydraulicBoundaryDatabase.FilePath = hydraulicBoundaryDatabaseFilePath;
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.SetValues(hlcdFilePath, string.Empty, 10, string.Empty,
                                                                                       usePreprocessorClosure, null, null, null,
                                                                                       null, null, null);

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
        }

        [Test]
        [TestCaseSource(nameof(GetPreprocessorConfigurations))]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithVariousPreprocessorConfigurations_ReturnsExpectedSettings(
            IAssessmentSection assessmentSection,
            string expectedPreprocessorDirectory)
        {
            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection,
                                                                                                                       assessmentSection.HydraulicBoundaryDatabases.First().Locations.First());

            // Assert
            Assert.AreEqual(expectedPreprocessorDirectory, settings.PreprocessorDirectory);
        }

        private static IEnumerable<TestCaseData> GetPreprocessorConfigurations()
        {
            const string preprocessorDirectory = "Directory";

            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSectionForCanUsePreprocessorFalse = new AssessmentSectionStub();
            HydraulicBoundaryDatabase hydraulicBoundaryDatabaseCanUsePreprocessorFalse = assessmentSectionForCanUsePreprocessorFalse.HydraulicBoundaryDatabases.First();
            hydraulicBoundaryDatabaseCanUsePreprocessorFalse.FilePath = validFilePath;
            hydraulicBoundaryDatabaseCanUsePreprocessorFalse.Locations.Add(hydraulicBoundaryLocation);
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabaseCanUsePreprocessorFalse);
            yield return new TestCaseData(assessmentSectionForCanUsePreprocessorFalse, string.Empty)
                .SetName("CanUsePreprocessorFalse");

            var assessmentSectionForUsePreprocessorFalse = new AssessmentSectionStub();
            HydraulicBoundaryDatabase hydraulicBoundaryDatabaseUsePreprocessorFalse = assessmentSectionForUsePreprocessorFalse.HydraulicBoundaryDatabases.First();
            hydraulicBoundaryDatabaseUsePreprocessorFalse.FilePath = validFilePath;
            hydraulicBoundaryDatabaseUsePreprocessorFalse.Locations.Add(hydraulicBoundaryLocation);
            hydraulicBoundaryDatabaseUsePreprocessorFalse.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            hydraulicBoundaryDatabaseUsePreprocessorFalse.HydraulicLocationConfigurationSettings.PreprocessorDirectory = preprocessorDirectory;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabaseUsePreprocessorFalse);
            yield return new TestCaseData(assessmentSectionForUsePreprocessorFalse, string.Empty)
                .SetName("UsePreprocessorFalseWithPreprocessorDirectory");

            var assessmentSectionForUsePreprocessorTrue = new AssessmentSectionStub();
            var hydraulicBoundaryDatabaseUsePreprocessorTrue = assessmentSectionForUsePreprocessorTrue.HydraulicBoundaryDatabase;
            hydraulicBoundaryDatabaseUsePreprocessorTrue.FilePath = validFilePath;
            hydraulicBoundaryDatabaseUsePreprocessorTrue.Locations.Add(hydraulicBoundaryLocation);
            hydraulicBoundaryDatabaseUsePreprocessorTrue.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            hydraulicBoundaryDatabaseUsePreprocessorTrue.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            hydraulicBoundaryDatabaseUsePreprocessorTrue.HydraulicLocationConfigurationSettings.PreprocessorDirectory = preprocessorDirectory;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabaseUsePreprocessorTrue);
            yield return new TestCaseData(assessmentSectionForUsePreprocessorTrue, preprocessorDirectory)
                .SetName("UsePreprocessorTrueWithPreprocessorDirectory");
        }
    }
}