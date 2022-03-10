// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
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
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => AggregatedSerializableFailureMechanismCreator.Create(
                null, new SerializableTotalAssemblyResult(),
                new ExportableFailureMechanism(
                    ExportableFailureMechanismAssemblyResultTestFactory.CreateResult(),
                    Enumerable.Empty<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>(),
                    random.NextEnumValue<ExportableFailureMechanismType>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => AggregatedSerializableFailureMechanismCreator.Create(
                new IdentifierGenerator(), null,
                new ExportableFailureMechanism(
                    ExportableFailureMechanismAssemblyResultTestFactory.CreateResult(),
                    Enumerable.Empty<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>(),
                    random.NextEnumValue<ExportableFailureMechanismType>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("serializableTotalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedSerializableFailureMechanismCreator.Create(
                new IdentifierGenerator(), new SerializableTotalAssemblyResult(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Create_WithValidArguments_ReturnsAggregatedSerializableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new ExportableFailureMechanism(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResult(),
                new[]
                {
                    ExportableFailureMechanismSectionAssemblyResultTestFactory.CreateWithProbability(
                        ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(random.Next()), random.Next()),
                    ExportableFailureMechanismSectionAssemblyResultTestFactory.CreateWithProbability(
                        ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(random.Next()), random.Next())
                },
                random.NextEnumValue<ExportableFailureMechanismType>());

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
            Assert.AreEqual(SerializableFailureMechanismTypeCreator.Create(failureMechanism.Code), serializableFailureMechanism.FailureMechanismType);

            SerializableFailureMechanismAssemblyResultTestHelper.AssertSerializableFailureMechanismAssemblyResult(failureMechanism.FailureMechanismAssembly,
                                                                                                                  serializableFailureMechanism.FailureMechanismAssemblyResult);

            SerializableFailureMechanismSectionCollection serializableFailureMechanismSectionCollection = aggregatedFailureMechanism.FailureMechanismSectionCollection;
            Assert.AreEqual("Vi.0", serializableFailureMechanismSectionCollection.Id);

            AssertFailureMechanismSectionAssemblies(failureMechanism,
                                                    serializableFailureMechanismSectionCollection,
                                                    serializableFailureMechanism,
                                                    aggregatedFailureMechanism.FailureMechanismSections,
                                                    aggregatedFailureMechanism.FailureMechanismSectionAssemblyResults);
        }

        private static void AssertFailureMechanismSectionAssemblies(
            ExportableFailureMechanism expectedFailureMechanism,
            SerializableFailureMechanismSectionCollection expectedSerializableFailureMechanismSectionCollection,
            SerializableFailureMechanism expectedSerializableFailureMechanism,
            IEnumerable<SerializableFailureMechanismSection> serializableFailureMechanismSections,
            IEnumerable<SerializableFailureMechanismSectionAssembly> serializableFailureMechanismSectionAssemblies)
        {
            IEnumerable<ExportableFailureMechanismSection> expectedSections = expectedFailureMechanism.SectionAssemblyResults.Select(sar => sar.FailureMechanismSection);
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, serializableFailureMechanismSections.Count());

            IEnumerable<ExportableFailureMechanismSectionAssemblyWithProbabilityResult> expectedSectionAssemblyResults = expectedFailureMechanism.SectionAssemblyResults;
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

                ExportableFailureMechanismSectionAssemblyWithProbabilityResult expectedSectionAssemblyResult = expectedSectionAssemblyResults.ElementAt(i);
                SerializableFailureMechanismSectionAssembly actualSectionAssemblyResult = serializableFailureMechanismSectionAssemblies.ElementAt(i);
                Assert.AreEqual($"T.{sectionAssemblyId++}", actualSectionAssemblyResult.Id);
                Assert.AreEqual(expectedSerializableFailureMechanism.Id, actualSectionAssemblyResult.FailureMechanismId);

                Assert.AreEqual(actualSection.Id, actualSectionAssemblyResult.FailureMechanismSectionId);
                Assert.AreEqual(expectedSectionAssemblyResult.AssemblyGroup, actualSectionAssemblyResult.SectionResult.AssemblyGroup);
                Assert.AreEqual(expectedSectionAssemblyResult.Probability, actualSectionAssemblyResult.SectionResult.Probability);
            }
        }

        private static SerializableTotalAssemblyResult CreateSerializableTotalAssembly(string totalAssemblyId)
        {
            return new SerializableTotalAssemblyResult(totalAssemblyId,
                                                       new SerializableAssessmentProcess(),
                                                       new SerializableAssessmentSectionAssemblyResult());
        }
    }
}