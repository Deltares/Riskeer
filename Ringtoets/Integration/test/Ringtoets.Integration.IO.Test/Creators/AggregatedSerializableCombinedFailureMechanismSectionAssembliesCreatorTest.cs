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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.Integration.IO.AggregatedSerializable;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(
                    null,
                    new SerializableTotalAssemblyResult(),
                    Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(
                    new IdentifierGenerator(),
                    null,
                    Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Create_CombinedSectionAssembliesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                AggregatedSerializableCombinedFailureMechanismSectionAssembliesCreator.Create(
                    new IdentifierGenerator(),
                    new SerializableTotalAssemblyResult(),
                    null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            Assert.AreEqual(totalAssemblyResult.Id, serializableFailureMechanismSectionCollection.TotalAssemblyResultId);

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
                Assert.AreEqual($"Gto.{combinedSectionId++}", actualSectionAssemblyResult.Id);
                Assert.AreEqual(actualSection.Id, actualSectionAssemblyResult.FailureMechanismSectionId);
                Assert.AreEqual(expectedSerializableTotalAssemblyResult.Id, actualSectionAssemblyResult.TotalAssemblyResultId);

                AssertCombinedFailureMechanismSectionResults(expectedCombinedSectionAssemblies.ElementAt(i).FailureMechanismResults,
                                                             actualSectionAssemblyResult.FailureMechanismResults);
            }
        }

        private static SerializableTotalAssemblyResult CreateSerializableTotalAssembly(string totalAssemblyId)
        {
            return new SerializableTotalAssemblyResult(totalAssemblyId,
                                                       new SerializableAssessmentProcess(),
                                                       new SerializableFailureMechanismAssemblyResult(),
                                                       new SerializableFailureMechanismAssemblyResult(),
                                                       new SerializableAssessmentSectionAssemblyResult());
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
            return new ExportableCombinedSectionAssembly(section,
                                                         ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                                                         new[]
                                                         {
                                                             CreateCombinedSectionAssemblyResult(seed++),
                                                             CreateCombinedSectionAssemblyResult(seed)
                                                         });
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateCombinedSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(CreateSectionAssemblyResult(random.Next()),
                                                                               random.NextEnumValue<ExportableFailureMechanismType>());
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
            Assert.AreEqual(SerializableFailureMechanismTypeCreator.Create(expectedSectionResult.Code),
                            actualSectionResult.FailureMechanismType);

            ExportableSectionAssemblyResult expectedSectionAssemblyResult = expectedSectionResult.SectionAssemblyResult;
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedSectionAssemblyResult.AssemblyMethod),
                            actualSectionResult.AssemblyMethod);
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(expectedSectionAssemblyResult.AssemblyCategory),
                            actualSectionResult.CategoryGroup);
        }
    }
}