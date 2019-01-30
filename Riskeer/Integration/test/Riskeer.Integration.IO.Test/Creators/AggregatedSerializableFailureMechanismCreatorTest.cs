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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.AggregatedSerializable;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class AggregatedSerializableFailureMechanismCreatorTest
    {
        [Test]
        public void CreateFailureMechanismWithoutProbability_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(null,
                                                                                           new SerializableTotalAssemblyResult(),
                                                                                           new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                                                                                               ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                                               Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismType>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismGroup>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismWithoutProbability_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                           null,
                                                                                           new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                                                                                               ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                                                               Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismType>(),
                                                                                               random.NextEnumValue<ExportableFailureMechanismGroup>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableTotalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismWithoutProbability_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                           new SerializableTotalAssemblyResult(),
                                                                                           (ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismWithoutProbability_WithUnsupportedAggregatedSectionResult_ThrowsNotSupportedException()
        {
            // Setup
            var random = new Random(21);
            ExportableFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();

            var failureMechanism = new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                new[]
                {
                    new UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult(section)
                },
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());

            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                           new SerializableTotalAssemblyResult(),
                                                                                           failureMechanism);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(call);
            Assert.AreEqual($"{nameof(UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult)} is not supported.",
                            exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(GetSectionAssemblyConfigurations))]
        public void CreateFailureMechanismWithoutProbabilityAndSectionAssemblyResultsWithProbability_WithValidArguments_ReturnsAggregatedSerializableFailureMechanism(
            IEnumerable<ExportableFailureMechanismSection> failureMechanismSections,
            IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase> failureMechanismSectionAssemblyResults,
            Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase, SerializableFailureMechanismSectionAssembly> assertSectionAssemblyResultsAction)
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                failureMechanismSectionAssemblyResults,
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());

            var idGenerator = new IdentifierGenerator();

            const string totalAssemblyId = "totalAssemblyId";
            SerializableTotalAssemblyResult serializableTotalAssembly = CreateSerializableTotalAssembly(totalAssemblyId);

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
            Assert.AreEqual("Vi.0", serializableFailureMechanismSectionCollection.Id);

            AssertFailureMechanismSectionAssemblies(failureMechanism,
                                                    serializableFailureMechanismSectionCollection,
                                                    serializableFailureMechanism,
                                                    aggregatedFailureMechanism.FailureMechanismSections,
                                                    aggregatedFailureMechanism.FailureMechanismSectionAssemblyResults,
                                                    assertSectionAssemblyResultsAction);
        }

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
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                           null,
                                                                                           new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                                                                                               ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
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
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                           new SerializableTotalAssemblyResult(),
                                                                                           (ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>) null);

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
                new[]
                {
                    new UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult(section)
                },
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());

            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
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
                failureMechanismSectionAssemblyResults,
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());

            var idGenerator = new IdentifierGenerator();

            const string totalAssemblyId = "totalAssemblyId";
            SerializableTotalAssemblyResult serializableTotalAssembly = CreateSerializableTotalAssembly(totalAssemblyId);

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
            Assert.AreEqual("Vi.0", serializableFailureMechanismSectionCollection.Id);

            AssertFailureMechanismSectionAssemblies(failureMechanism,
                                                    serializableFailureMechanismSectionCollection,
                                                    serializableFailureMechanism,
                                                    aggregatedFailureMechanism.FailureMechanismSections,
                                                    aggregatedFailureMechanism.FailureMechanismSectionAssemblyResults,
                                                    assertSectionAssemblyResultsAction);
        }

        private static void AssertFailureMechanismSectionAssemblies<T>(
            ExportableFailureMechanism<T> expectedFailureMechanism,
            SerializableFailureMechanismSectionCollection expectedSerializableFailureMechanismSectionCollection,
            SerializableFailureMechanism expectedSerializableFailureMechanism,
            IEnumerable<SerializableFailureMechanismSection> serializableFailureMechanismSections,
            IEnumerable<SerializableFailureMechanismSectionAssembly> serializableFailureMechanismSectionAssemblies,
            Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase, SerializableFailureMechanismSectionAssembly> assertSectionAssemblyResultAction)
            where T : ExportableFailureMechanismAssemblyResult
        {
            IEnumerable<ExportableFailureMechanismSection> expectedSections = expectedFailureMechanism.SectionAssemblyResults.Select(sar => sar.FailureMechanismSection);
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, serializableFailureMechanismSections.Count());

            IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase> expectedSectionAssemblyResults = expectedFailureMechanism.SectionAssemblyResults;
            Assert.AreEqual(expectedSectionAssemblyResults.Count(), serializableFailureMechanismSectionAssemblies.Count());

            var sectionId = 0;
            var sectionAssemblyId = 0;
            for (var i = 0; i < expectedNrOfSections; i++)
            {
                ExportableFailureMechanismSection expectedSection = expectedSections.ElementAt(i);
                SerializableFailureMechanismSection actualSection = serializableFailureMechanismSections.ElementAt(i);

                SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(expectedSection,
                                                                                            expectedSerializableFailureMechanismSectionCollection,
                                                                                            actualSection,
                                                                                            sectionId++);

                SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult = serializableFailureMechanismSectionAssemblies.ElementAt(i);
                Assert.AreEqual($"T.{sectionAssemblyId++}", actualSectionAssemblyResult.Id);
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
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.SimpleAssembly,
                                                                                             SerializableAssessmentType.SimpleAssessment,
                                                                                             serializedSectionResults[0]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.DetailedAssembly,
                                                                                             SerializableAssessmentType.DetailedAssessment,
                                                                                             serializedSectionResults[1]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.TailorMadeAssembly,
                                                                                             SerializableAssessmentType.TailorMadeAssessment,
                                                                                             serializedSectionResults[2]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithProbability.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             actualSectionAssemblyResult.CombinedSectionResult);
        }

        private static void AssertSectionAssemblyResultsWithoutProbability(ExportableAggregatedFailureMechanismSectionAssemblyResultBase expectedSectionAssemblyResult,
                                                                           SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult)
        {
            var expectedSectionAssemblyResultWithoutProbability = (ExportableAggregatedFailureMechanismSectionAssemblyResult) expectedSectionAssemblyResult;

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = actualSectionAssemblyResult.SectionResults;
            Assert.AreEqual(3, serializedSectionResults.Length);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.SimpleAssembly,
                                                                                             SerializableAssessmentType.SimpleAssessment,
                                                                                             serializedSectionResults[0]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.DetailedAssembly,
                                                                                             SerializableAssessmentType.DetailedAssessment,
                                                                                             serializedSectionResults[1]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.TailorMadeAssembly,
                                                                                             SerializableAssessmentType.TailorMadeAssessment,
                                                                                             serializedSectionResults[2]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             actualSectionAssemblyResult.CombinedSectionResult);
        }

        private static void AssertSectionAssemblyResultsWithoutDetailedAssembly(ExportableAggregatedFailureMechanismSectionAssemblyResultBase expectedSectionAssemblyResult,
                                                                                SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult)
        {
            var expectedSectionAssemblyResultWithoutProbability = (ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly) expectedSectionAssemblyResult;

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = actualSectionAssemblyResult.SectionResults;
            Assert.AreEqual(2, serializedSectionResults.Length);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.SimpleAssembly,
                                                                                             SerializableAssessmentType.SimpleAssessment,
                                                                                             serializedSectionResults[0]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.TailorMadeAssembly,
                                                                                             SerializableAssessmentType.TailorMadeAssessment,
                                                                                             serializedSectionResults[1]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             actualSectionAssemblyResult.CombinedSectionResult);
        }

        private static void AssertSectionAssemblyWithCombinedResult(ExportableAggregatedFailureMechanismSectionAssemblyResultBase expectedSectionAssemblyResult,
                                                                    SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult)
        {
            var expectedSectionAssemblyResultWithoutProbability = (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult) expectedSectionAssemblyResult;
            CollectionAssert.IsEmpty(actualSectionAssemblyResult.SectionResults);

            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             actualSectionAssemblyResult.CombinedSectionResult);
        }

        private static void AssertSectionAssemblyWithCombinedProbabilityResult(ExportableAggregatedFailureMechanismSectionAssemblyResultBase expectedSectionAssemblyResult,
                                                                               SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult)
        {
            var expectedSectionAssemblyResultWithoutProbability = (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult) expectedSectionAssemblyResult;
            CollectionAssert.IsEmpty(actualSectionAssemblyResult.SectionResults);

            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(expectedSectionAssemblyResultWithoutProbability.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             actualSectionAssemblyResult.CombinedSectionResult);
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

            ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult[] failureMechanismSectionResultsWithCombinedAssembly =
            {
                CreateSectionResultWithCombinedAssembly(failureMechanismSections[0]),
                CreateSectionResultWithCombinedAssembly(failureMechanismSections[1])
            };
            yield return new TestCaseData(failureMechanismSections,
                                          failureMechanismSectionResultsWithCombinedAssembly,
                                          new Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase,
                                              SerializableFailureMechanismSectionAssembly>(AssertSectionAssemblyWithCombinedResult))
                .SetName("SectionAssemblyResults with combined assessment");

            ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult[] failureMechanismSectionResultsWithCombinedProbabilityAssembly =
            {
                CreateSectionResultWithCombinedProbabilityAssembly(failureMechanismSections[0]),
                CreateSectionResultWithCombinedProbabilityAssembly(failureMechanismSections[1])
            };
            yield return new TestCaseData(failureMechanismSections,
                                          failureMechanismSectionResultsWithCombinedProbabilityAssembly,
                                          new Action<ExportableAggregatedFailureMechanismSectionAssemblyResultBase,
                                              SerializableFailureMechanismSectionAssembly>(AssertSectionAssemblyWithCombinedProbabilityResult))
                .SetName("SectionAssemblyResults with combined probability assessment");
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

        private static ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult CreateSectionResultWithCombinedAssembly(ExportableFailureMechanismSection section)
        {
            return new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult(section, CreateSectionAssemblyResult(10));
        }

        private static ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult CreateSectionResultWithCombinedProbabilityAssembly(ExportableFailureMechanismSection section)
        {
            return new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult(section, CreateSectionAssemblyResultWithProbability(10));
        }

        private static ExportableSectionAssemblyResult CreateSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            return new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                       random.NextEnumValue(new[]
                                                       {
                                                           FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                                                           FailureMechanismSectionAssemblyCategoryGroup.Iv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.IIv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.IIIv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.IVv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.Vv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.VIv,
                                                           FailureMechanismSectionAssemblyCategoryGroup.VIIv
                                                       }));
        }

        private static ExportableSectionAssemblyResultWithProbability CreateSectionAssemblyResultWithProbability(int seed)
        {
            var random = new Random(seed);
            return new ExportableSectionAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                      random.NextEnumValue(new[]
                                                                      {
                                                                          FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                                                                          FailureMechanismSectionAssemblyCategoryGroup.Iv,
                                                                          FailureMechanismSectionAssemblyCategoryGroup.IIv,
                                                                          FailureMechanismSectionAssemblyCategoryGroup.IIIv,
                                                                          FailureMechanismSectionAssemblyCategoryGroup.IVv,
                                                                          FailureMechanismSectionAssemblyCategoryGroup.Vv,
                                                                          FailureMechanismSectionAssemblyCategoryGroup.VIv,
                                                                          FailureMechanismSectionAssemblyCategoryGroup.VIIv
                                                                      }),
                                                                      random.NextDouble());
        }

        private class UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult : ExportableAggregatedFailureMechanismSectionAssemblyResultBase
        {
            public UnsupportedExportableAggregatedFailureMechanismSectionAssemblyResult(ExportableFailureMechanismSection failureMechanismSection) : base(failureMechanismSection) {}
        }
    }
}