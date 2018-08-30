﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class AggregatedSerializableFailureMechanismCreatorTest
    {
        [Test]
        public void CreateFailureMechanismWithProbability_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(null,
                                                                                           new SerializableTotalAssemblyResult(),
                                                                                           new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                                                                                               ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                                               Enumerable.Empty<ExportableFailureMechanismSection>(),
                                                                                               Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismType>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismGroup>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismWithProbability_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new UniqueIdentifierGenerator(),
                                                                                           null,
                                                                                           new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                                                                                               ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                                                               Enumerable.Empty<ExportableFailureMechanismSection>(),
                                                                                               Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismType>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismGroup>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableTotalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismWithProbability_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new UniqueIdentifierGenerator(),
                                                                                           new SerializableTotalAssemblyResult(),
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismWithProbability_WithUnsupportedAggregatedSectionResult_ThrowsNotSupportedException()
        {
            // Setup
            var random = new Random(21);
            ExportableFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();

            var failureMechanism = new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                Enumerable.Empty<ExportableFailureMechanismSection>(),
                new[]
                {
                    new UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult(section)
                },
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());

            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new UniqueIdentifierGenerator(),
                                                                                           new SerializableTotalAssemblyResult(),
                                                                                           failureMechanism);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(call);
            Assert.AreEqual($"{nameof(UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult)} is not supported.",
                            exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(GetSectionAssemblyConfigurations))]
        public void CreateFailureMechanismWithProbabilityAndSectionAssemblyResultsWithProbability_WithValidArguments_ReturnsAggregatedSerializableFailureMechanism(
            IEnumerable<ExportableFailureMechanismSection> failureMechanismSections,
            IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase> failureMechanismSectionAssemblyResults,
            Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase, SerializableFailureMechanismSectionAssembly> assertSectionAssemblyResultsAction)
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                failureMechanismSections,
                failureMechanismSectionAssemblyResults,
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());

            var idGenerator = new UniqueIdentifierGenerator();

            const string totalAssemblyId = "totalAssemblyId";
            SerializableTotalAssemblyResult serializableTotalAssembly = CreateSerializableTotalAssembly(totalAssemblyId);

            // Precondition
            Assert.AreEqual(failureMechanism.Sections.Count(), failureMechanism.SectionAssemblyResults.Count());

            // Call
            AggregatedSerializableFailureMechanism aggregatedFailureMechanism =
                AggregatedSerializableFailureMechanismCreator.Create(idGenerator, serializableTotalAssembly, failureMechanism);

            // Assert
            SerializableFailureMechanism serializableFailureMechanism = aggregatedFailureMechanism.FailureMechanism;
            Assert.AreEqual("Ts.0", serializableFailureMechanism.Id);
            Assert.AreEqual(serializableTotalAssembly.Id, serializableFailureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(SerializableFailureMechanismGroupCreator.Create(failureMechanism.Group), serializableFailureMechanism.FailureMechanismGroup);
            Assert.AreEqual(SerializableFailureMechanismTypeCreator.Create(failureMechanism.Code), serializableFailureMechanism.FailureMechanismType);

            SerializableFailureMechanismAssemblyResultTestHelper.AssertSerializableFailureMechanismAssemblyResult(failureMechanism.FailureMechanismAssembly,
                                                                                                                  serializableFailureMechanism.FailureMechanismAssemblyResult);

            SerializableFailureMechanismSectionCollection serializableFailureMechanismSectionCollection = aggregatedFailureMechanism.FailureMechanismSectionCollection;
            Assert.AreEqual("Vi.1", serializableFailureMechanismSectionCollection.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, serializableFailureMechanismSectionCollection.FailureMechanismId);

            AssertFailureMechanismSectionAssemblies(2,
                                                    failureMechanism,
                                                    serializableFailureMechanismSectionCollection,
                                                    serializableFailureMechanism,
                                                    aggregatedFailureMechanism.FailureMechanismSections,
                                                    aggregatedFailureMechanism.FailureMechanismSectionAssemblyResults,
                                                    assertSectionAssemblyResultsAction);
        }

        private static void AssertFailureMechanismSectionAssemblies<T>(
            int idStartIndex,
            ExportableFailureMechanism<T> expectedFailureMechanism,
            SerializableFailureMechanismSectionCollection expectedSerializableFailureMechanismSectionCollection,
            SerializableFailureMechanism expectedSerializableFailureMechanism,
            IEnumerable<SerializableFailureMechanismSection> serializableFailureMechanismSections,
            IEnumerable<SerializableFailureMechanismSectionAssembly> serializableFailureMechanismSectionAssemblies,
            Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase, SerializableFailureMechanismSectionAssembly> assertSectionAssemblyResultAction)
            where T : ExportableFailureMechanismAssemblyResult
        {
            IEnumerable<ExportableFailureMechanismSection> expectedSections = expectedFailureMechanism.Sections;
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, serializableFailureMechanismSections.Count());

            IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase> expectedSectionAssemblyResults = expectedFailureMechanism.SectionAssemblyResults;
            Assert.AreEqual(expectedSectionAssemblyResults.Count(), serializableFailureMechanismSectionAssemblies.Count());

            int expectedIdIndex = idStartIndex;
            for (var i = 0; i < expectedNrOfSections; i++)
            {
                ExportableFailureMechanismSection expectedSection = expectedSections.ElementAt(i);
                SerializableFailureMechanismSection actualSection = serializableFailureMechanismSections.ElementAt(i);

                SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(expectedSection,
                                                                                            expectedSerializableFailureMechanismSectionCollection,
                                                                                            actualSection,
                                                                                            expectedIdIndex++);

                SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult = serializableFailureMechanismSectionAssemblies.ElementAt(i);
                Assert.AreEqual($"T.{expectedIdIndex++}", actualSectionAssemblyResult.Id);
                Assert.AreEqual(expectedSerializableFailureMechanism.Id, actualSectionAssemblyResult.FailureMechanismId);
                Assert.AreEqual(actualSection.Id, actualSectionAssemblyResult.FailureMechanismSectionId);

                assertSectionAssemblyResultAction(expectedSectionAssemblyResults.ElementAt(i), actualSectionAssemblyResult);
            }
        }

        private static void AssertSectionAssemblyResultsWithProbability(ExportableAggregatedFailureMechanismSectionAssemblyResultBase expectedSectionAssemblyResult,
                                                                        SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult)
        {
            var expectedSectionAssemblyResultWithProbability = (ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability) expectedSectionAssemblyResult;

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = actualSectionAssemblyResult.SectionResults;
            Assert.AreEqual(3, serializedSectionResults.Length);
            AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.SimpleAssembly,
                                 SerializableAssessmentType.SimpleAssessment,
                                 serializedSectionResults[0]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.DetailedAssembly,
                                 SerializableAssessmentType.DetailedAssessment,
                                 serializedSectionResults[1]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.TailorMadeAssembly,
                                 SerializableAssessmentType.TailorMadeAssessment,
                                 serializedSectionResults[2]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.CombinedAssembly,
                                 SerializableAssessmentType.CombinedAssessment,
                                 actualSectionAssemblyResult.CombinedSectionResult);
        }

        private static void AssertSectionAssemblyResultsWithoutProbability(ExportableAggregatedFailureMechanismSectionAssemblyResultBase expectedSectionAssemblyResult,
                                                                           SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult)
        {
            var expectedSectionAssemblyResultWithoutProbability = (ExportableAggregatedFailureMechanismSectionAssemblyResult) expectedSectionAssemblyResult;

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = actualSectionAssemblyResult.SectionResults;
            Assert.AreEqual(3, serializedSectionResults.Length);
            AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.SimpleAssembly,
                                 SerializableAssessmentType.SimpleAssessment,
                                 serializedSectionResults[0]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.DetailedAssembly,
                                 SerializableAssessmentType.DetailedAssessment,
                                 serializedSectionResults[1]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.TailorMadeAssembly,
                                 SerializableAssessmentType.TailorMadeAssessment,
                                 serializedSectionResults[2]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.CombinedAssembly,
                                 SerializableAssessmentType.CombinedAssessment,
                                 actualSectionAssemblyResult.CombinedSectionResult);
        }

        private static void AssertSectionAssemblyResultsWithoutDetailedAssembly(ExportableAggregatedFailureMechanismSectionAssemblyResultBase expectedSectionAssemblyResult,
                                                                                SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult)
        {
            var expectedSectionAssemblyResultWithoutProbability = (ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly) expectedSectionAssemblyResult;

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = actualSectionAssemblyResult.SectionResults;
            Assert.AreEqual(2, serializedSectionResults.Length);
            AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.SimpleAssembly,
                                 SerializableAssessmentType.SimpleAssessment,
                                 serializedSectionResults[0]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.TailorMadeAssembly,
                                 SerializableAssessmentType.TailorMadeAssessment,
                                 serializedSectionResults[1]);
            AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.CombinedAssembly,
                                 SerializableAssessmentType.CombinedAssessment,
                                 actualSectionAssemblyResult.CombinedSectionResult);
        }

        private static void AssertAssemblyResult(ExportableSectionAssemblyResult expectedResult,
                                                 SerializableAssessmentType expectedAssessmentType,
                                                 SerializableFailureMechanismSectionAssemblyResult actualResult)
        {
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(expectedResult.AssemblyCategory),
                            actualResult.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedResult.AssemblyMethod),
                            actualResult.AssemblyMethod);
            Assert.AreEqual(expectedAssessmentType, actualResult.AssessmentType);
            Assert.IsNull(actualResult.Probability);
        }

        private static void AssertAssemblyResult(ExportableSectionAssemblyResultWithProbability expectedResult,
                                                 SerializableAssessmentType expectedAssessmentType,
                                                 SerializableFailureMechanismSectionAssemblyResult actualResult)
        {
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(expectedResult.AssemblyCategory),
                            actualResult.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedResult.AssemblyMethod),
                            actualResult.AssemblyMethod);
            Assert.AreEqual(expectedAssessmentType, actualResult.AssessmentType);
            Assert.AreEqual(expectedResult.Probability, actualResult.Probability);
        }

        private static SerializableTotalAssemblyResult CreateSerializableTotalAssembly(string totalAssemblyId)
        {
            return new SerializableTotalAssemblyResult(totalAssemblyId,
                                                       new SerializableAssessmentProcess(),
                                                       new SerializableFailureMechanismAssemblyResult(),
                                                       new SerializableFailureMechanismAssemblyResult(),
                                                       new SerializableAssessmentSectionAssemblyResult());
        }

        private static IEnumerable<TestCaseData> GetSectionAssemblyConfigurations()
        {
            ExportableFailureMechanismSection[] failureMechanismSections =
            {
                CreateSection(21),
                CreateSection(22)
            };

            ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability[] failureMechanismSectionResultsWithProbability =
            {
                CreateSectionResultWithProbability(failureMechanismSections[0]),
                CreateSectionResultWithProbability(failureMechanismSections[1])
            };
            yield return new TestCaseData(failureMechanismSections,
                                          failureMechanismSectionResultsWithProbability,
                                          new Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase,
                                              SerializableFailureMechanismSectionAssembly>(AssertSectionAssemblyResultsWithProbability))
                .SetName("SectionAssemblyResults with probability");

            ExportableAggregatedFailureMechanismSectionAssemblyResult[] failureMechanismSectionResultsWithoutProbability =
            {
                CreateSectionResultWithoutProbability(failureMechanismSections[0]),
                CreateSectionResultWithoutProbability(failureMechanismSections[1])
            };
            yield return new TestCaseData(failureMechanismSections,
                                          failureMechanismSectionResultsWithoutProbability,
                                          new Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase,
                                              SerializableFailureMechanismSectionAssembly>(AssertSectionAssemblyResultsWithoutProbability))
                .SetName("SectionAssemblyResults without probability");

            ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly[] failureMechanismSectionResultsWithoutDetailedAssembly =
            {
                CreateSectionResultWithoutDetailedAssembly(failureMechanismSections[0]),
                CreateSectionResultWithoutDetailedAssembly(failureMechanismSections[1])
            };
            yield return new TestCaseData(failureMechanismSections,
                                          failureMechanismSectionResultsWithoutDetailedAssembly,
                                          new Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase,
                                              SerializableFailureMechanismSectionAssembly>(AssertSectionAssemblyResultsWithoutDetailedAssembly))
                .SetName("SectionAssemblyResults without detailed assessment");
        }

        private static ExportableFailureMechanismSection CreateSection(int seed)
        {
            var random = new Random(seed);
            return new ExportableFailureMechanismSection(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            }, random.NextDouble(), random.NextDouble());
        }

        private static ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability CreateSectionResultWithProbability(ExportableFailureMechanismSection section)
        {
            return new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(section,
                                                                                                CreateSectionAssemblyResultWithProbability(10),
                                                                                                CreateSectionAssemblyResultWithProbability(11),
                                                                                                CreateSectionAssemblyResultWithProbability(12),
                                                                                                CreateSectionAssemblyResultWithProbability(13));
        }

        private static ExportableAggregatedFailureMechanismSectionAssemblyResult CreateSectionResultWithoutProbability(ExportableFailureMechanismSection section)
        {
            return new ExportableAggregatedFailureMechanismSectionAssemblyResult(section,
                                                                                 CreateSectionAssemblyResult(10),
                                                                                 CreateSectionAssemblyResult(11),
                                                                                 CreateSectionAssemblyResult(12),
                                                                                 CreateSectionAssemblyResult(13));
        }

        private static ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly CreateSectionResultWithoutDetailedAssembly(ExportableFailureMechanismSection section)
        {
            return new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(section,
                                                                                                        CreateSectionAssemblyResult(10),
                                                                                                        CreateSectionAssemblyResult(11),
                                                                                                        CreateSectionAssemblyResult(12));
        }

        private static ExportableSectionAssemblyResult CreateSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            var assemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            if (assemblyCategory == FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                assemblyCategory = FailureMechanismSectionAssemblyCategoryGroup.IIIv;
            }

            return new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                       assemblyCategory);
        }

        private static ExportableSectionAssemblyResultWithProbability CreateSectionAssemblyResultWithProbability(int seed)
        {
            var random = new Random(seed);
            var assemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            if (assemblyCategory == FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                assemblyCategory = FailureMechanismSectionAssemblyCategoryGroup.IIIv;
            }

            return new ExportableSectionAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                      assemblyCategory,
                                                                      random.NextDouble());
        }

        private class UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult : ExportableAggregatedFailureMechanismSectionAssemblyResultBase
        {
            public UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult(ExportableFailureMechanismSection failureMechanismSection) : base(failureMechanismSection) {}
        }
    }
}