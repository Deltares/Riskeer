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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class CombinedAssemblyFailureMechanismSectionFactoryTest
    {
        [Test]
        public void CreateInput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(null,
                                                                                                 Enumerable.Empty<IFailureMechanism>(),
                                                                                                 new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateInput_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection,
                                                                                                 null,
                                                                                                 new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void CreateInput_WithAllFailureMechanisms_ReturnsInputCollection()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms(), new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(18, inputs.Count());
                AssertSectionsWithResult(assessmentSection.Piping.SectionResults, inputs.ElementAt(0));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionInwards.SectionResults, inputs.ElementAt(1));
                AssertSectionsWithResult(assessmentSection.MacroStabilityInwards.SectionResults, inputs.ElementAt(2));
                AssertSectionsWithResult(assessmentSection.MacroStabilityOutwards.SectionResults, inputs.ElementAt(3));
                AssertSectionsWithResult(assessmentSection.Microstability.SectionResults, inputs.ElementAt(4));
                AssertSectionsWithResult(assessmentSection.StabilityStoneCover.SectionResults, inputs.ElementAt(5));
                AssertSectionsWithResult(assessmentSection.WaveImpactAsphaltCover.SectionResults, inputs.ElementAt(6));
                AssertSectionsWithResult(assessmentSection.WaterPressureAsphaltCover.SectionResults, inputs.ElementAt(7));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionOutwards.SectionResults, inputs.ElementAt(8));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffOutwards.SectionResults, inputs.ElementAt(9));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffInwards.SectionResults, inputs.ElementAt(10));
                AssertSectionsWithResult(assessmentSection.HeightStructures.SectionResults, inputs.ElementAt(11));
                AssertSectionsWithResult(assessmentSection.ClosingStructures.SectionResults, inputs.ElementAt(12));
                AssertSectionsWithResult(assessmentSection.PipingStructure.SectionResults, inputs.ElementAt(13));
                AssertSectionsWithResult(assessmentSection.StabilityPointStructures.SectionResults, inputs.ElementAt(14));
                AssertSectionsWithResult(assessmentSection.StrengthStabilityLengthwiseConstruction.SectionResults, inputs.ElementAt(15));
                AssertSectionsWithResult(assessmentSection.DuneErosion.SectionResults, inputs.ElementAt(16));
                AssertSectionsWithResult(assessmentSection.TechnicalInnovation.SectionResults, inputs.ElementAt(17));
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismTestCaseData))]
        public void CreateInput_WithOneFailureMechanism_ReturnsInputCollection(AssessmentSection assessmentSection, IFailureMechanism relevantFailureMechanism)
        {
            // Setup
            assessmentSection.GetFailureMechanisms().ForEachElementDo(failureMechanism => failureMechanism.IsRelevant = failureMechanism == relevantFailureMechanism);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, new[]
                    {
                        relevantFailureMechanism
                    }, new Random(39).NextBoolean());

                // Assert
                AssertSections(((IHasSectionResults<FailureMechanismSectionResult>) relevantFailureMechanism).SectionResults, inputs.Single());
            }
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismTestCaseData()
        {
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            yield return new TestCaseData(assessmentSection, assessmentSection.Piping);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverErosionInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.MacroStabilityInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.MacroStabilityOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.Microstability);
            yield return new TestCaseData(assessmentSection, assessmentSection.StabilityStoneCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.WaveImpactAsphaltCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.WaterPressureAsphaltCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverErosionOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverSlipOffOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverSlipOffInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.HeightStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.ClosingStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.PipingStructure);
            yield return new TestCaseData(assessmentSection, assessmentSection.StabilityPointStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.StrengthStabilityLengthwiseConstruction);
            yield return new TestCaseData(assessmentSection, assessmentSection.DuneErosion);
            yield return new TestCaseData(assessmentSection, assessmentSection.TechnicalInnovation);
        }

        private static void AssertSections<T>(IEnumerable<T> originalSectionResults, IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
            where T : FailureMechanismSectionResult
        {
            Assert.AreEqual(originalSectionResults.Count(), inputSections.Count());

            double expectedSectionStart = 0;

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                double expectedSectionEnd = expectedSectionStart + originalSectionResults.ElementAt(i).Section.Length;

                Assert.AreEqual(expectedSectionStart, inputSections.ElementAt(i).SectionStart);
                Assert.AreEqual(expectedSectionEnd, inputSections.ElementAt(i).SectionEnd);

                expectedSectionStart = expectedSectionEnd;
            }
        }

        private static void AssertSectionsWithResult<T>(IEnumerable<T> originalSectionResults, IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
            where T : FailureMechanismSectionResult
        {
            AssertSections(originalSectionResults, inputSections);

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, inputSections.ElementAt(i).CategoryGroup);
            }
        }

        #region Manual Assembly

        #region Piping

        [Test]
        public void GivenPipingFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            PipingFailureMechanism piping = assessmentSection.Piping;
            FailureMechanismTestHelper.SetSections(piping, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            PipingFailureMechanismSectionResult sectionResult = piping.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.Piping
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenPipingFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            PipingFailureMechanism piping = assessmentSection.Piping;
            FailureMechanismTestHelper.SetSections(piping, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            PipingFailureMechanismSectionResult sectionResult = piping.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.Piping
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.Zero(sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        #endregion

        #region GrassCoverErosionInwards

        [Test]
        public void GivenGrassCoverErosionInwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverErosionInwards
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenGrassCoverErosionInwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverErosionInwards
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.Zero(sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        #endregion

        #region MacroStabilityInwards

        [Test]
        public void GivenMacroStabilityInwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MacroStabilityInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.MacroStabilityInwards
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenMacroStabilityInwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MacroStabilityInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.MacroStabilityInwards
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.Zero(sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        #endregion

        #region HeightStructures

        [Test]
        public void GivenHeightStructuresFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.HeightStructures
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenHeightStructuresFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.HeightStructures
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.Zero(sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        #endregion

        #region ClosingStructures

        [Test]
        public void GivenClosingStructuresFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.ClosingStructures
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenClosingStructuresFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.ClosingStructures
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.Zero(sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        #endregion

        #region StabilityPointStructures

        [Test]
        public void GivenStabilityPointStructuresFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StabilityPointStructures
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenStabilityPointStructuresFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StabilityPointStructures
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.Zero(sectionCalculator.ManualAssemblyProbabilityInput);
            }
        }

        #endregion

        #region GrassCoverErosionOutwards

        [Test]
        public void GivenGrassCoverErosionOutwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverErosionOutwards
                }, true);

                // Then
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenGrassCoverErosionOutwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IVv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverErosionOutwards
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region StabilityStoneCover

        [Test]
        public void GivenStabilityStoneCoverFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            StabilityStoneCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StabilityStoneCover
                }, true);

                // Then
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenStabilityStoneCoverFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            StabilityStoneCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IVv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StabilityStoneCover
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region WaveImpactAsphaltCover

        [Test]
        public void GivenWaveImpactAsphaltCoverFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.WaveImpactAsphaltCover
                }, true);

                // Then
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenWaveImpactAsphaltCoverFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IVv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.WaveImpactAsphaltCover
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region DuneErosion

        [Test]
        public void GivenDuneErosionFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.DuneErosion
                }, true);

                // Then
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenDuneErosionFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IVv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.DuneErosion
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region GrassCoverSlipOffInwards

        [Test]
        public void GivenGrassCoverSlipOffInwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffInwards
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenGrassCoverSlipOffInwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffInwards
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region GrassCoverSlipOffOutwards

        [Test]
        public void GivenGrassCoverSlipOffOutwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffOutwards
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenGrassCoverSlipOffOutwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffOutwards
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region Microstability

        [Test]
        public void GivenMicrostabilityFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.Microstability
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenMicrostabilityFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.Microstability
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region MacroStabilityOutwards

        [Test]
        public void GivenMacroStabilityOutwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.MacroStabilityOutwards
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenMacroStabilityOutwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.MacroStabilityOutwards
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region PipingStructure

        [Test]
        public void GivenPipingStructureFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.PipingStructure
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenPipingStructureFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.PipingStructure
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region StrengthStabilityLengthwiseConstruction

        [Test]
        public void GivenStrengthStabilityLengthwiseConstructionFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StrengthStabilityLengthwiseConstruction
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenStrengthStabilityLengthwiseConstructionFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StrengthStabilityLengthwiseConstruction
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region TechnicalInnovation

        [Test]
        public void GivenTechnicalInnovationFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.TechnicalInnovation
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenTechnicalInnovationFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.TechnicalInnovation
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region WaterPressureAsphaltCover

        [Test]
        public void GivenWaterPressureAsphaltCoverFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.WaterPressureAsphaltCover
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenWaterPressureAsphaltCoverFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.WaterPressureAsphaltCover
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #endregion
    }
}