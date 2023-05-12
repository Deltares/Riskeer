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
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Converters;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableAssessmentSectionFactoryTest
    {
        [Test]
        public void CreateExportableAssessmentSection_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(
                null, new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateExportableAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(new IdentifierGenerator(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableAssessmentSection_WithAssessmentSectionWithReferenceLine_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = "assessmentSectionName",
                Id = "assessmentSectionId"
            };
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);

            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());
            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());

            assessmentSection.SpecificFailureMechanisms.ForEachElementDo(sfm => sfm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections);
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections);

            AddFailureMechanismSections(assessmentSection);

            var idGenerator = new IdentifierGenerator();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(
                    idGenerator, assessmentSection);

                // Assert
                var factory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionCalculator = factory.LastCreatedAssessmentSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismCalculator = factory.LastCreatedFailureMechanismAssemblyCalculator;

                AssertExportableAssessmentSection(assessmentSection, assessmentSectionCalculator.AssessmentSectionAssemblyResult, exportableAssessmentSection);
                AssertExportableFailureMechanisms(assessmentSection.GetFailureMechanisms()
                                                                   .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                   .Cast<IFailureMechanism<FailureMechanismSectionResult>>(),
                                                  failureMechanismCalculator.AssemblyResultOutput,
                                                  exportableAssessmentSection.FailureMechanisms);

                CombinedFailureMechanismSectionAssemblyResultWrapper combinedFailureMechanismAssemblyResult = assessmentSectionCalculator.CombinedFailureMechanismSectionAssemblyOutput;
                AssertExportableFailureMechanismSectionCollections(assessmentSection, combinedFailureMechanismAssemblyResult.AssemblyResults.Count(),
                                                                   exportableAssessmentSection.FailureMechanismSectionCollections);
                AssertExportableCombinedFailureMechanismSectionAssemblyOutput(
                    combinedFailureMechanismAssemblyResult, exportableAssessmentSection.FailureMechanisms,
                    exportableAssessmentSection.CombinedSectionAssemblies);
            }
        }

        [Test]
        public void CreateExportableAssessmentSection_AllFailureMechanismNotInAssembly_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = "assessmentSectionName",
                Id = "assessmentSectionId"
            };
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);

            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());
            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());

            AddFailureMechanismSections(assessmentSection);

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms()
                                                                            .Concat(assessmentSection.SpecificFailureMechanisms))
            {
                failureMechanism.InAssembly = false;
            }

            var idGenerator = new IdentifierGenerator();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var factory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionCalculator = factory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionCalculator.CombinedFailureMechanismSectionAssemblyOutput = new CombinedFailureMechanismSectionAssemblyResultWrapper(
                    Enumerable.Empty<CombinedFailureMechanismSectionAssembly>(), AssemblyMethod.BOI3A1, AssemblyMethod.BOI3B1, AssemblyMethod.BOI3C1);

                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(idGenerator, assessmentSection);

                // Assert
                AssertExportableAssessmentSection(assessmentSection, assessmentSectionCalculator.AssessmentSectionAssemblyResult, exportableAssessmentSection);

                CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanisms);
                CollectionAssert.IsEmpty(exportableAssessmentSection.FailureMechanismSectionCollections);

                AssertExportableCombinedFailureMechanismSectionAssemblyOutput(
                    assessmentSectionCalculator.CombinedFailureMechanismSectionAssemblyOutput,
                    exportableAssessmentSection.FailureMechanisms, exportableAssessmentSection.CombinedSectionAssemblies);
            }
        }

        private static ExportableAssessmentSectionAssemblyGroup ConvertAssemblyGroup(AssessmentSectionAssemblyGroup assemblyGroup)
        {
            switch (assemblyGroup)
            {
                case AssessmentSectionAssemblyGroup.APlus:
                    return ExportableAssessmentSectionAssemblyGroup.APlus;
                case AssessmentSectionAssemblyGroup.A:
                    return ExportableAssessmentSectionAssemblyGroup.A;
                case AssessmentSectionAssemblyGroup.B:
                    return ExportableAssessmentSectionAssemblyGroup.B;
                case AssessmentSectionAssemblyGroup.C:
                    return ExportableAssessmentSectionAssemblyGroup.C;
                case AssessmentSectionAssemblyGroup.D:
                    return ExportableAssessmentSectionAssemblyGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void AddFailureMechanismSections(AssessmentSection assessmentSection)
        {
            var random = new Random(21);
            FailureMechanismTestHelper.AddSections(assessmentSection.Piping, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.HeightStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.ClosingStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityPointStructures, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityStoneCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaveImpactAsphaltCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.DuneErosion, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.Microstability, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.PipingStructure, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaterPressureAsphaltCover, random.Next(1, 10));

            foreach (SpecificFailureMechanism specificFailureMechanism in assessmentSection.SpecificFailureMechanisms)
            {
                FailureMechanismTestHelper.AddSections(specificFailureMechanism, random.Next(1, 10));
            }
        }

        private static void AssertExportableAssessmentSection(
            IAssessmentSection assessmentSection, AssessmentSectionAssemblyResultWrapper assessmentSectionAssemblyResult,
            ExportableAssessmentSection exportableAssessmentSection)
        {
            Assert.AreEqual(assessmentSection.Name, exportableAssessmentSection.Name);
            Assert.AreEqual($"Wks.{assessmentSection.Id}", exportableAssessmentSection.Id);
            CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

            ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(assessmentSectionAssemblyResult.ProbabilityMethod),
                            exportableAssessmentSectionAssemblyResult.ProbabilityAssemblyMethod);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(assessmentSectionAssemblyResult.AssemblyGroupMethod),
                            exportableAssessmentSectionAssemblyResult.AssemblyGroupAssemblyMethod);
            Assert.AreEqual(ConvertAssemblyGroup(assessmentSectionAssemblyResult.AssemblyResult.AssemblyGroup),
                            exportableAssessmentSectionAssemblyResult.AssemblyGroup);
            Assert.AreEqual(assessmentSectionAssemblyResult.AssemblyResult.Probability, exportableAssessmentSectionAssemblyResult.Probability);
        }

        private static void AssertExportableFailureMechanismSectionCollections(
            IAssessmentSection assessmentSection, int nrOfCombinedSectionAssemblyResults,
            IEnumerable<ExportableFailureMechanismSectionCollection> failureMechanismSectionCollections)
        {
            IEnumerable<IFailureMechanism> failureMechanismsInAssembly = assessmentSection.GetFailureMechanisms()
                                                                                          .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                                          .Where(fm => fm.InAssembly);

            int nrOfFailureMechanismsInAssembly = failureMechanismsInAssembly.Count();
            int nrOfExpectedCollections = nrOfFailureMechanismsInAssembly + 1;
            Assert.AreEqual(nrOfExpectedCollections, failureMechanismSectionCollections.Count());

            for (var i = 0; i < nrOfFailureMechanismsInAssembly; i++)
            {
                int nrOfExpectedSections = failureMechanismsInAssembly.ElementAt(i).Sections.Count();
                Assert.AreEqual(nrOfExpectedSections, failureMechanismSectionCollections.ElementAt(i).Sections.Count());
            }

            ExportableFailureMechanismSectionCollection combinedFailureMechanismSectionCollection = failureMechanismSectionCollections.Last();
            IEnumerable<ExportableFailureMechanismSection> exportableCombinedFailureMechanismSections = combinedFailureMechanismSectionCollection.Sections;
            CollectionAssert.AllItemsAreInstancesOfType(exportableCombinedFailureMechanismSections, typeof(ExportableCombinedFailureMechanismSection));
            Assert.AreEqual(nrOfCombinedSectionAssemblyResults, exportableCombinedFailureMechanismSections.Count());
        }

        private static void AssertExportableFailureMechanisms(IEnumerable<IFailureMechanism<FailureMechanismSectionResult>> failureMechanisms,
                                                              FailureMechanismAssemblyResultWrapper failureMechanismAssemblyResult,
                                                              IEnumerable<ExportableFailureMechanism> exportableFailureMechanisms)
        {
            Assert.AreEqual(failureMechanisms.Count(), exportableFailureMechanisms.Count());

            for (var i = 0; i < failureMechanisms.Count(); i++)
            {
                IFailureMechanism<FailureMechanismSectionResult> failureMechanism = failureMechanisms.ElementAt(i);
                ExportableFailureMechanism exportableFailureMechanism = exportableFailureMechanisms.ElementAt(i);

                if (exportableFailureMechanism is ExportableGenericFailureMechanism exportableGenericFailureMechanism)
                {
                    AssertExportableGenericFailureMechanism(failureMechanism, failureMechanismAssemblyResult, exportableGenericFailureMechanism);
                }

                if (exportableFailureMechanism is ExportableSpecificFailureMechanism exportableSpecificFailureMechanism)
                {
                    AssertExportableSpecificFailureMechanism(failureMechanism, failureMechanismAssemblyResult, exportableSpecificFailureMechanism);
                }
            }
        }

        private static void AssertExportableGenericFailureMechanism(IFailureMechanism<FailureMechanismSectionResult> failureMechanism,
                                                                    FailureMechanismAssemblyResultWrapper failureMechanismAssemblyResult,
                                                                    ExportableGenericFailureMechanism actualExportableFailureMechanism)
        {
            Assert.AreEqual(failureMechanism.Code, actualExportableFailureMechanism.Code);

            AssertExportableFailureMechanism(failureMechanism, failureMechanismAssemblyResult, actualExportableFailureMechanism);
        }

        private static void AssertExportableSpecificFailureMechanism(IFailureMechanism<FailureMechanismSectionResult> failureMechanism,
                                                                     FailureMechanismAssemblyResultWrapper failureMechanismAssemblyResult,
                                                                     ExportableSpecificFailureMechanism actualExportableFailureMechanism)
        {
            Assert.AreEqual(failureMechanism.Name, actualExportableFailureMechanism.Name);

            AssertExportableFailureMechanism(failureMechanism, failureMechanismAssemblyResult, actualExportableFailureMechanism);
        }

        private static void AssertExportableFailureMechanism(IFailureMechanism<FailureMechanismSectionResult> failureMechanism,
                                                             FailureMechanismAssemblyResultWrapper failureMechanismAssemblyResult,
                                                             ExportableFailureMechanism actualExportableFailureMechanism)
        {
            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(failureMechanismAssemblyResult.AssemblyResult, exportableFailureMechanismAssemblyResult.Probability);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(failureMechanismAssemblyResult.AssemblyMethod),
                            exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }

        private static void AssertExportableCombinedFailureMechanismSectionAssemblyOutput(
            CombinedFailureMechanismSectionAssemblyResultWrapper calculatorCombinedFailureMechanismSectionAssemblyOutput,
            IEnumerable<ExportableFailureMechanism> exportableFailureMechanisms,
            IEnumerable<ExportableCombinedSectionAssembly> exportableCombinedSectionAssemblies)
        {
            Assert.AreEqual(calculatorCombinedFailureMechanismSectionAssemblyOutput.AssemblyResults.Count(),
                            exportableCombinedSectionAssemblies.Count());

            for (var i = 0; i < exportableCombinedSectionAssemblies.Count(); i++)
            {
                CombinedFailureMechanismSectionAssembly calculatorAssemblyResult = calculatorCombinedFailureMechanismSectionAssemblyOutput.AssemblyResults.ElementAt(i);
                ExportableCombinedSectionAssembly exportableCombinedSectionAssembly = exportableCombinedSectionAssemblies.ElementAt(i);

                Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(calculatorCombinedFailureMechanismSectionAssemblyOutput.CombinedSectionResultAssemblyMethod),
                                exportableCombinedSectionAssembly.AssemblyGroupAssemblyMethod);
                Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(calculatorCombinedFailureMechanismSectionAssemblyOutput.CommonSectionAssemblyMethod),
                                exportableCombinedSectionAssembly.Section.AssemblyMethod);
                Assert.IsTrue(exportableCombinedSectionAssembly.FailureMechanismResults.All(
                                  fmr => fmr.AssemblyMethod == ExportableAssemblyMethodConverter.ConvertTo(
                                             calculatorCombinedFailureMechanismSectionAssemblyOutput.FailureMechanismResultsAssemblyMethod)));

                Assert.AreEqual(ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(calculatorAssemblyResult.Section.FailureMechanismSectionAssemblyGroup),
                                exportableCombinedSectionAssembly.AssemblyGroup);

                AssertExportableFailureMechanismCombinedSectionAssemblyResults(exportableCombinedSectionAssembly.Section,
                                                                               exportableFailureMechanisms,
                                                                               exportableCombinedSectionAssembly.FailureMechanismResults,
                                                                               calculatorAssemblyResult.FailureMechanismSectionAssemblyGroupResults);

                Assert.AreEqual(calculatorAssemblyResult.Section.SectionStart, exportableCombinedSectionAssembly.Section.StartDistance);
                Assert.AreEqual(calculatorAssemblyResult.Section.SectionEnd, exportableCombinedSectionAssembly.Section.EndDistance);
            }
        }

        private static void AssertExportableFailureMechanismCombinedSectionAssemblyResults(
            ExportableFailureMechanismSection combinedFailureMechanismSection, IEnumerable<ExportableFailureMechanism> exportableFailureMechanisms,
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> exportableFailureMechanismCombinedSectionAssemblyResults,
            IEnumerable<FailureMechanismSectionAssemblyGroup> failureMechanismSectionAssemblyGroupResults)
        {
            const int expectedNrOfFailureMechanisms = 17;
            Assert.AreEqual(expectedNrOfFailureMechanisms, exportableFailureMechanismCombinedSectionAssemblyResults.Count());

            for (var i = 0; i < expectedNrOfFailureMechanisms; i++)
            {
                AssertExportableFailureMechanismCombinedSectionAssemblyResult(
                    combinedFailureMechanismSection, exportableFailureMechanisms.ElementAt(i).SectionAssemblyResults,
                    failureMechanismSectionAssemblyGroupResults.ElementAt(i), exportableFailureMechanismCombinedSectionAssemblyResults.ElementAt(i));
            }
        }

        private static void AssertExportableFailureMechanismCombinedSectionAssemblyResult(
            ExportableFailureMechanismSection combinedFailureMechanismSectionAssembly,
            IEnumerable<ExportableFailureMechanismSectionAssemblyResult> failureMechanismSectionAssemblyResults,
            FailureMechanismSectionAssemblyGroup failureMechanismSectionAssemblyGroup,
            ExportableFailureMechanismCombinedSectionAssemblyResult actualExportableFailureMechanismCombinedSectionAssemblyResult)
        {
            ExportableFailureMechanismSectionAssemblyResult associatedAssemblyResult = GetCorrespondingSectionAssemblyResultResult(
                combinedFailureMechanismSectionAssembly, failureMechanismSectionAssemblyResults);
            Assert.AreSame(associatedAssemblyResult, actualExportableFailureMechanismCombinedSectionAssemblyResult.FailureMechanismSectionResult);
            Assert.AreEqual(ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(failureMechanismSectionAssemblyGroup),
                            actualExportableFailureMechanismCombinedSectionAssemblyResult.AssemblyGroup);
        }

        private static ExportableFailureMechanismSectionAssemblyResult GetCorrespondingSectionAssemblyResultResult(
            ExportableFailureMechanismSection combinedFailureMechanismSection,
            IEnumerable<ExportableFailureMechanismSectionAssemblyResult> sectionAssemblyResults)
        {
            return sectionAssemblyResults.FirstOrDefault(assemblyResult =>
            {
                ExportableFailureMechanismSection exportableFailureMechanismSection = assemblyResult.FailureMechanismSection;

                return combinedFailureMechanismSection.StartDistance >= exportableFailureMechanismSection.StartDistance
                       && combinedFailureMechanismSection.EndDistance <= exportableFailureMechanismSection.EndDistance;
            });
        }
    }
}