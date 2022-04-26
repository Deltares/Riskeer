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
    public class AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(
                null, new SerializableTotalAssemblyResult(), Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(
                new IdentifierGenerator(), null, Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Create_CombinedSectionAssembliesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(
                new IdentifierGenerator(), new SerializableTotalAssemblyResult(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedSectionAssemblies", exception.ParamName);
        }

        [Test]
        public void Create_WithValidArguments_ReturnsAggregatedSerializableCombinedFailureMechanismSectionAssemblies()
        {
            // Setup
            ExportableCombinedSectionAssembly[] combinedSectionAssemblies =
            {
                CreateCombinedSectionAssembly(CreateSection(1), 1),
                CreateCombinedSectionAssembly(CreateSection(2), 2)
            };

            var idGenerator = new IdentifierGenerator();
            SerializableTotalAssemblyResult totalAssemblyResult = CreateSerializableTotalAssembly("totalAssemblyResultId");

            // Call
            AggregatedSerializableCombinedFailureMechanismSectionAssemblies aggregate =
                AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(idGenerator,
                                                                                              totalAssemblyResult,
                                                                                              combinedSectionAssemblies);

            // Assert
            SerializableFailureMechanismSectionCollection serializableFailureMechanismSectionCollection = aggregate.FailureMechanismSectionCollection;
            Assert.AreEqual("Vi.0", serializableFailureMechanismSectionCollection.Id);

            AssertCombinedFailureMechanismSectionAssemblies(combinedSectionAssemblies,
                                                            serializableFailureMechanismSectionCollection,
                                                            totalAssemblyResult,
                                                            aggregate.FailureMechanismSections,
                                                            aggregate.CombinedFailureMechanismSectionAssemblies);
        }

        private static void AssertCombinedFailureMechanismSectionAssemblies(IEnumerable<ExportableCombinedSectionAssembly> expectedCombinedSectionAssemblies,
                                                                            SerializableFailureMechanismSectionCollection expectedSerializableFailureMechanismSectionCollection,
                                                                            SerializableTotalAssemblyResult expectedSerializableTotalAssemblyResult,
                                                                            IEnumerable<SerializableFailureMechanismSection> serializableFailureMechanismSections,
                                                                            IEnumerable<SerializableCombinedFailureMechanismSectionAssembly> serializableFailureMechanismSectionAssemblies)
        {
            IEnumerable<ExportableCombinedFailureMechanismSection> expectedSections = expectedCombinedSectionAssemblies.Select(csar => csar.Section);
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, serializableFailureMechanismSections.Count());

            Assert.AreEqual(expectedCombinedSectionAssemblies.Count(), serializableFailureMechanismSectionAssemblies.Count());

            var sectionId = 0;
            var combinedSectionId = 0;
            for (var i = 0; i < expectedNrOfSections; i++)
            {
                ExportableCombinedFailureMechanismSection expectedSection = expectedSections.ElementAt(i);
                SerializableFailureMechanismSection actualSection = serializableFailureMechanismSections.ElementAt(i);

                SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(expectedSection,
                                                                                            expectedSerializableFailureMechanismSectionCollection,
                                                                                            actualSection,
                                                                                            sectionId++);

                SerializableCombinedFailureMechanismSectionAssembly actualSectionAssemblyResult = serializableFailureMechanismSectionAssemblies.ElementAt(i);
                Assert.AreEqual($"Gf.{combinedSectionId++}", actualSectionAssemblyResult.Id);
                Assert.AreEqual(actualSection.Id, actualSectionAssemblyResult.FailureMechanismSectionId);
                Assert.AreEqual(expectedSerializableTotalAssemblyResult.Id, actualSectionAssemblyResult.TotalAssemblyResultId);

                AssertCombinedFailureMechanismSectionResults(expectedCombinedSectionAssemblies.ElementAt(i).FailureMechanismResults,
                                                             actualSectionAssemblyResult.FailureMechanismResults);
            }
        }

        private static SerializableTotalAssemblyResult CreateSerializableTotalAssembly(string totalAssemblyId)
        {
            var random = new Random();
            return new SerializableTotalAssemblyResult(totalAssemblyId,
                                                       new SerializableAssessmentProcess(),
                                                       random.NextEnumValue<SerializableAssemblyMethod>(),
                                                       random.NextEnumValue<SerializableAssemblyMethod>(),
                                                       random.NextEnumValue<SerializableAssessmentSectionAssemblyGroup>(),
                                                       random.NextDouble());
        }

        private static ExportableCombinedFailureMechanismSection CreateSection(int seed)
        {
            var random = new Random(seed);
            return new ExportableCombinedFailureMechanismSection(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            }, random.NextDouble(), random.NextDouble(), random.NextEnumValue<ExportableAssemblyMethod>());
        }

        private static ExportableCombinedSectionAssembly CreateCombinedSectionAssembly(ExportableCombinedFailureMechanismSection section,
                                                                                       int seed)
        {
            return new ExportableCombinedSectionAssembly(
                section, ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(section, seed),
                new[]
                {
                    CreateCombinedSectionAssemblyResult(seed++),
                    CreateCombinedSectionAssemblyResult(seed)
                });
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateCombinedSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(
                CreateSectionAssemblyResult(random.Next()),
                random.NextEnumValue<ExportableFailureMechanismType>(),
                "code",
                "name");
        }

        private static ExportableFailureMechanismSubSectionAssemblyResult CreateSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            return new ExportableFailureMechanismSubSectionAssemblyResult(
                random.NextEnumValue(new[]
                {
                    FailureMechanismSectionAssemblyGroup.NotDominant,
                    FailureMechanismSectionAssemblyGroup.III,
                    FailureMechanismSectionAssemblyGroup.II,
                    FailureMechanismSectionAssemblyGroup.I,
                    FailureMechanismSectionAssemblyGroup.Zero,
                    FailureMechanismSectionAssemblyGroup.IMin,
                    FailureMechanismSectionAssemblyGroup.IIMin,
                    FailureMechanismSectionAssemblyGroup.IIIMin,
                    FailureMechanismSectionAssemblyGroup.NotRelevant
                }), random.NextEnumValue<ExportableAssemblyMethod>());
        }

        private static void AssertCombinedFailureMechanismSectionResults(IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> expectedCombinedSectionAssemblyResults,
                                                                         IEnumerable<SerializableCombinedFailureMechanismSectionAssemblyResult> actualCombinedSectionAssemblyResults)
        {
            int expectedNrOfCombinedSectionAssemblies = expectedCombinedSectionAssemblyResults.Count();
            Assert.AreEqual(expectedNrOfCombinedSectionAssemblies, actualCombinedSectionAssemblyResults.Count());

            for (var i = 0; i < expectedNrOfCombinedSectionAssemblies; i++)
            {
                ExportableFailureMechanismCombinedSectionAssemblyResult expectedSectionAssembly = expectedCombinedSectionAssemblyResults.ElementAt(i);
                SerializableCombinedFailureMechanismSectionAssemblyResult actualSectionAssembly = actualCombinedSectionAssemblyResults.ElementAt(i);

                AssertCombinedFailureMechanismSectionAssemblyResult(expectedSectionAssembly,
                                                                    actualSectionAssembly);
            }
        }

        private static void AssertCombinedFailureMechanismSectionAssemblyResult(ExportableFailureMechanismCombinedSectionAssemblyResult expectedSectionResult,
                                                                                SerializableCombinedFailureMechanismSectionAssemblyResult actualSectionResult)
        {
            Assert.AreEqual(expectedSectionResult.Code, actualSectionResult.GenericFailureMechanismCode);

            ExportableFailureMechanismSubSectionAssemblyResult expectedSectionAssemblyResult = expectedSectionResult.SectionAssemblyResult;
            Assert.AreEqual(SerializableFailureMechanismSectionAssemblyGroupCreator.Create(expectedSectionAssemblyResult.AssemblyGroup),
                            actualSectionResult.AssemblyGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedSectionAssemblyResult.AssemblyMethod),
                            actualSectionResult.AssemblyMethod);
        }
    }
}