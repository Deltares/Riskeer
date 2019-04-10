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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Data;
using Riskeer.Integration.Service.Comparers;

namespace Riskeer.Integration.Service.Test.Comparers
{
    [TestFixture]
    public class AssessmentSectionMergeComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new AssessmentSectionMergeComparer();

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeComparer>(comparer);
        }

        [Test]
        public void Compare_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var comparer = new AssessmentSectionMergeComparer();

            // Call
            TestDelegate call = () => comparer.Compare(null, CreateAssessmentSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Compare_OtherAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var comparer = new AssessmentSectionMergeComparer();

            // Call
            TestDelegate call = () => comparer.Compare(CreateAssessmentSection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("otherAssessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Compare_AssessmentSectionsHaveEquivalentReferenceLines_ReturnsTrue(bool referenceLineImported)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            AssessmentSection otherAssessmentSection = CreateAssessmentSection();

            if (!referenceLineImported)
            {
                assessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());
                otherAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());
            }

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(assessmentSection, otherAssessmentSection);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCaseSource(nameof(GetUnequivalentAssessmentSectionWithoutHydraulicLocationConfigurationSettingsTestCases))]
        public void Compare_AssessmentSectionsNotEquivalentAndWithoutHydraulicLocationConfigurationSettings_ReturnsFalse(AssessmentSection otherAssessmentSection)
        {
            // Setup
            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(CreateAssessmentSection(), otherAssessmentSection);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Compare_AssessmentSectionWithEquivalentHydraulicBoundaryLocationConfigurationSettings_ReturnsTrue(bool hydraulicLocationConfigurationSettingsSet)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            AssessmentSection otherAssessmentSection = CreateAssessmentSection();

            if (hydraulicLocationConfigurationSettingsSet)
            {
                SetHydraulicLocationConfigurationValues(assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings,
                                                        "FilePath1");

                SetHydraulicLocationConfigurationValues(otherAssessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings,
                                                        "FilePath2");
            }

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(assessmentSection, otherAssessmentSection);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCaseSource(nameof(GetUnequivalentAssessmentSectionWithHydraulicLocationConfigurationSettingsTestCases))]
        public void Compare_AssessmentSectionNotEquivalentWitHydraulicBoundaryLocationConfigurationSettings_ReturnsFalse(AssessmentSection otherAssessmentSection)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            SetHydraulicLocationConfigurationValues(assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings);

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(assessmentSection, otherAssessmentSection);

            // Assert
            Assert.IsFalse(result);
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike, 0.1, 0.025);
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(1, 2)
            });

            return assessmentSection;
        }

        private static void SetHydraulicLocationConfigurationValues(HydraulicLocationConfigurationSettings settings,
                                                                    string hlcdFilePath = "filePath")
        {
            const string scenarioName = "ScenarioName";
            const int year = 2017;
            const string scope = "Scope";
            const bool usePreprocessorClosure = false;
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                               usePreprocessorClosure, seaLevel, riverDischarge,
                               lakeLevel, windDirection, windSpeed, comment);
        }

        private static IEnumerable<TestCaseData> GetUnequivalentAssessmentSectionWithoutHydraulicLocationConfigurationSettingsTestCases()
        {
            foreach (ChangePropertyData<AssessmentSection> changeSingleDataProperty in ChangeSingleDataPropertiesOfAssessmentSection())
            {
                AssessmentSection assessmentSection = CreateAssessmentSection();
                changeSingleDataProperty.ActionToChangeProperty(assessmentSection);
                yield return new TestCaseData(assessmentSection).SetName(changeSingleDataProperty.PropertyName);
            }
        }

        private static IEnumerable<ChangePropertyData<AssessmentSection>> ChangeSingleDataPropertiesOfAssessmentSection()
        {
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ReferenceLine.SetGeometry(new[]
                                                                   {
                                                                       new Point2D(1, 1)
                                                                   }),
                                                                   "Referenceline different point count");

            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ReferenceLine.SetGeometry(new[]
                                                                   {
                                                                       new Point2D(1, 1),
                                                                       new Point2D(1, 3)
                                                                   }),
                                                                   "Referenceline different point");

            yield return new ChangePropertyData<AssessmentSection>(sec => sec.Id = "DifferentVersion",
                                                                   "Id");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.HydraulicBoundaryDatabase.Version = "DifferentVersion",
                                                                   "HydraulicBoundaryDataBase");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.LowerLimitNorm = sec.FailureMechanismContribution.LowerLimitNorm - 1e-15,
                                                                   "LowerLimitNorm");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.SignalingNorm = sec.FailureMechanismContribution.SignalingNorm - 1e-15,
                                                                   "SignalingNorm");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.NormativeNorm = sec.FailureMechanismContribution.NormativeNorm == NormType.LowerLimit
                                                                                                                               ? NormType.Signaling
                                                                                                                               : NormType.LowerLimit,
                                                                   "NormType");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ChangeComposition(AssessmentSectionComposition.DikeAndDune),
                                                                   "Composition");
        }

        private static IEnumerable<TestCaseData> GetUnequivalentAssessmentSectionWithHydraulicLocationConfigurationSettingsTestCases()
        {
            foreach (ChangePropertyData<HydraulicLocationConfigurationSettings> changeSingleDataProperty in ChangeSingleDataOfHydraulicLocationConfigurationSettings())
            {
                AssessmentSection assessmentSection = CreateAssessmentSection();
                changeSingleDataProperty.ActionToChangeProperty(assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings);
                yield return new TestCaseData(assessmentSection).SetName(changeSingleDataProperty.PropertyName);
            }
        }

        private static IEnumerable<ChangePropertyData<HydraulicLocationConfigurationSettings>> ChangeSingleDataOfHydraulicLocationConfigurationSettings()
        {
            const string hlcdFilePath = "path";
            const string scenarioName = "ScenarioName";
            const int year = 2017;
            const string scope = "Scope";
            const bool usePreprocessorClosure = false;
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, "Other ScenarionName", year, scope,
                                                                                                                       usePreprocessorClosure, seaLevel, riverDischarge,
                                                                                                                       lakeLevel, windDirection, windSpeed, comment),
                                                                                        "Different ScenarioName");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, 2023, scope,
                                                                                                                       usePreprocessorClosure, seaLevel, riverDischarge,
                                                                                                                       lakeLevel, windDirection, windSpeed, comment),
                                                                                        "Different Year");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, "Other Scope",
                                                                                                                       usePreprocessorClosure, seaLevel, riverDischarge,
                                                                                                                       lakeLevel, windDirection, windSpeed, comment),
                                                                                        "Different Scope");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                                                                                                                       true, seaLevel, riverDischarge,
                                                                                                                       lakeLevel, windDirection, windSpeed, comment),
                                                                                        "Different UsePreprocessorClosure");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                                                                                                                       usePreprocessorClosure, "Other SeaLevel", riverDischarge,
                                                                                                                       lakeLevel, windDirection, windSpeed, comment),
                                                                                        "Different SeaLevel");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                                                                                                                       usePreprocessorClosure, seaLevel, "Other RiverDischarge",
                                                                                                                       lakeLevel, windDirection, windSpeed, comment),
                                                                                        "Different RiverDischarge");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                                                                                                                       usePreprocessorClosure, seaLevel, riverDischarge,
                                                                                                                       "Other LakeLevel", windDirection, windSpeed, comment),
                                                                                        "Different LakeLevel");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                                                                                                                       usePreprocessorClosure, seaLevel, riverDischarge,
                                                                                                                       lakeLevel, "Other WindDirection", windSpeed, comment),
                                                                                        "Different WindDirection");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                                                                                                                       usePreprocessorClosure, seaLevel, riverDischarge,
                                                                                                                       lakeLevel, windDirection, "Other Windspeed", comment),
                                                                                        "Different WindSpeed");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings => settings.SetValues(hlcdFilePath, scenarioName, year, scope,
                                                                                                                       usePreprocessorClosure, seaLevel, riverDischarge,
                                                                                                                       lakeLevel, windDirection, windSpeed, "Other Comment"),
                                                                                        "Different Comment");
        }
    }
}