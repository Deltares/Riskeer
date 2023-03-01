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
            void Call() => comparer.Compare(null, CreateAssessmentSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Compare_OtherAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var comparer = new AssessmentSectionMergeComparer();

            // Call
            void Call() => comparer.Compare(CreateAssessmentSection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
        public void Compare_AssessmentSectionWithEquivalentHydraulicLocationConfigurationSettings_ReturnsTrue()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            SetHydraulicLocationConfigurationValues(assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationSettings,
                                                    "FilePath1");

            AssessmentSection otherAssessmentSection = CreateAssessmentSection();
            SetHydraulicLocationConfigurationValues(otherAssessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationSettings,
                                                    "FilePath2");

            var comparer = new AssessmentSectionMergeComparer();

            // Call
            bool result = comparer.Compare(assessmentSection, otherAssessmentSection);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCaseSource(nameof(GetAssessmentSectionWithNotEquivalentHydraulicLocationConfigurationSettingsTestCases))]
        public void Compare_AssessmentSectionWithNotEquivalentHydraulicLocationConfigurationSettings_ReturnsFalse(AssessmentSection otherAssessmentSection)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();
            SetHydraulicLocationConfigurationValues(assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationSettings);

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
            settings.FilePath = hlcdFilePath;
            settings.ScenarioName = "ScenarioName";
            settings.Year = 2017;
            settings.Scope = "Scope";
            settings.SeaLevel = "SeaLevel";
            settings.RiverDischarge = "RiverDischarge";
            settings.LakeLevel = "LakeLevel";
            settings.WindDirection = "WindDirection";
            settings.WindSpeed = "WindSpeed";
            settings.Comment = "Comment";
            settings.UsePreprocessorClosure = false;
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
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.HydraulicBoundaryData.Version = "DifferentVersion",
                                                                   "HydraulicBoundaryData");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.MaximumAllowableFloodingProbability -= 1e-15,
                                                                   "MaximumAllowableFloodingProbability");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.SignalFloodingProbability -= 1e-15,
                                                                   "SignalFloodingProbability");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.FailureMechanismContribution.NormativeProbabilityType = sec.FailureMechanismContribution.NormativeProbabilityType == NormativeProbabilityType.MaximumAllowableFloodingProbability
                                                                                                                                          ? NormativeProbabilityType.SignalFloodingProbability
                                                                                                                                          : NormativeProbabilityType.MaximumAllowableFloodingProbability,
                                                                   "NormativeProbabilityType");
            yield return new ChangePropertyData<AssessmentSection>(sec => sec.ChangeComposition(AssessmentSectionComposition.DikeAndDune),
                                                                   "Composition");
        }

        private static IEnumerable<TestCaseData> GetAssessmentSectionWithNotEquivalentHydraulicLocationConfigurationSettingsTestCases()
        {
            foreach (ChangePropertyData<HydraulicLocationConfigurationSettings> changeSingleDataProperty in ChangeSingleDataOfHydraulicLocationConfigurationSettings())
            {
                AssessmentSection assessmentSection = CreateAssessmentSection();
                changeSingleDataProperty.ActionToChangeProperty(assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationSettings);
                yield return new TestCaseData(assessmentSection).SetName(changeSingleDataProperty.PropertyName);
            }
        }

        private static IEnumerable<ChangePropertyData<HydraulicLocationConfigurationSettings>> ChangeSingleDataOfHydraulicLocationConfigurationSettings()
        {
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.ScenarioName = "Other ScenarioName";
                                                                                        },
                                                                                        "Different ScenarioName");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.Year = 2023;
                                                                                        },
                                                                                        "Different Year");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.Scope = "Other Scope";
                                                                                        },
                                                                                        "Different Scope");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.SeaLevel = "Other SeaLevel";
                                                                                        },
                                                                                        "Different SeaLevel");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.RiverDischarge = "Other RiverDischarge";
                                                                                        },
                                                                                        "Different RiverDischarge");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.LakeLevel = "Other LakeLevel";
                                                                                        },
                                                                                        "Different LakeLevel");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.WindDirection = "Other WindDirection";
                                                                                        },
                                                                                        "Different WindDirection");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.WindSpeed = "Other Windspeed";
                                                                                        },
                                                                                        "Different WindSpeed");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.Comment = "Other Comment";
                                                                                        },
                                                                                        "Different Comment");
            yield return new ChangePropertyData<HydraulicLocationConfigurationSettings>(settings =>
                                                                                        {
                                                                                            SetHydraulicLocationConfigurationValues(settings);

                                                                                            settings.UsePreprocessorClosure = true;
                                                                                        },
                                                                                        "Different UsePreprocessorClosure");
        }
    }
}