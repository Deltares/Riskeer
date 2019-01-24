// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.AggregatedSerializable;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class AggregatedSerializableFailureMechanismSectionAssemblyCreatorTest
    {
        [Test]
        public void CreateWithoutProbability_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(null,
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResult(10),
                                                                                                              CreateSectionAssemblyResult(11),
                                                                                                              CreateSectionAssemblyResult(12),
                                                                                                              CreateSectionAssemblyResult(13)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithoutProbability_SerializableFailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          null,
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResult(10),
                                                                                                              CreateSectionAssemblyResult(11),
                                                                                                              CreateSectionAssemblyResult(12),
                                                                                                              CreateSectionAssemblyResult(13)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableCollection", exception.ParamName);
        }

        [Test]
        public void CreateWithoutProbability_SerializableFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          null,
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResult(10),
                                                                                                              CreateSectionAssemblyResult(11),
                                                                                                              CreateSectionAssemblyResult(12),
                                                                                                              CreateSectionAssemblyResult(13)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableFailureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateWithoutProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          (ExportableAggregatedFailureMechanismSectionAssemblyResult) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void CreateWithoutProbability_WithValidArgumentsAndAllAssemblyTypesHaveResults_ReturnsAggregatedSerializableFailureMechanismSectionAssembly()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResult(10),
                CreateSectionAssemblyResult(11),
                CreateSectionAssemblyResult(12),
                CreateSectionAssemblyResult(13));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = failureMechanismSectionAssembly.SectionResults;
            Assert.AreEqual(3, serializedSectionResults.Length);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.SimpleAssembly,
                                                                                             SerializableAssessmentType.SimpleAssessment,
                                                                                             serializedSectionResults[0]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.DetailedAssembly,
                                                                                             SerializableAssessmentType.DetailedAssessment,
                                                                                             serializedSectionResults[1]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.TailorMadeAssembly,
                                                                                             SerializableAssessmentType.TailorMadeAssessment,
                                                                                             serializedSectionResults[2]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             failureMechanismSectionAssembly.CombinedSectionResult);

            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.None,
            TestName = "SimpleAssembly")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.None,
            TestName = "DetailedAssembly")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.IIv,
            TestName = "TailorMadeAssembly")]
        public void CreateWithoutProbability_WithValidArgumentsAndOneAssemblyTypeHasResult_ReturnsAggregatedSerializableFailureMechanismSectionAssembly(
            FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup detailedAssemblyCategoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup)
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResult(simpleAssemblyCategoryGroup),
                CreateSectionAssemblyResult(detailedAssemblyCategoryGroup),
                CreateSectionAssemblyResult(tailorMadeAssemblyCategoryGroup),
                CreateSectionAssemblyResult(13));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = failureMechanismSectionAssembly.SectionResults;
            Assert.AreEqual(1, serializedSectionResults.Length);

            if (simpleAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.SimpleAssembly,
                                                                                                 SerializableAssessmentType.SimpleAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            if (detailedAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.DetailedAssembly,
                                                                                                 SerializableAssessmentType.DetailedAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            if (tailorMadeAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.TailorMadeAssembly,
                                                                                                 SerializableAssessmentType.TailorMadeAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             failureMechanismSectionAssembly.CombinedSectionResult);

            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
        }

        [Test]
        public void CreateWithProbability_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(null,
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10),
                                                                                                              CreateSectionAssemblyResultWithProbability(11),
                                                                                                              CreateSectionAssemblyResultWithProbability(12),
                                                                                                              CreateSectionAssemblyResultWithProbability(13)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithProbability_SerializableFailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          null,
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10),
                                                                                                              CreateSectionAssemblyResultWithProbability(11),
                                                                                                              CreateSectionAssemblyResultWithProbability(12),
                                                                                                              CreateSectionAssemblyResultWithProbability(13)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableCollection", exception.ParamName);
        }

        [Test]
        public void CreateWithProbability_SerializableFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          null,
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10),
                                                                                                              CreateSectionAssemblyResultWithProbability(11),
                                                                                                              CreateSectionAssemblyResultWithProbability(12),
                                                                                                              CreateSectionAssemblyResultWithProbability(13)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableFailureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateWithProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          (ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void CreateWithProbability_WithValidArgumentsAndAllAssemblyTypesHaveResults_ReturnsAggregatedSerializableFailureMechanismSectionAssembly()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResultWithProbability(10),
                CreateSectionAssemblyResultWithProbability(11),
                CreateSectionAssemblyResultWithProbability(12),
                CreateSectionAssemblyResultWithProbability(13));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = failureMechanismSectionAssembly.SectionResults;
            Assert.AreEqual(3, serializedSectionResults.Length);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.SimpleAssembly,
                                                                                             SerializableAssessmentType.SimpleAssessment,
                                                                                             serializedSectionResults[0]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.DetailedAssembly,
                                                                                             SerializableAssessmentType.DetailedAssessment,
                                                                                             serializedSectionResults[1]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.TailorMadeAssembly,
                                                                                             SerializableAssessmentType.TailorMadeAssessment,
                                                                                             serializedSectionResults[2]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             failureMechanismSectionAssembly.CombinedSectionResult);

            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.None,
            TestName = "SimpleAssembly")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.None,
            TestName = "DetailedAssembly")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.IIv,
            TestName = "TailorMadeAssembly")]
        public void CreateWithProbability_WithValidArgumentsAndOneAssemblyTypeHasResult_ReturnsAggregatedSerializableFailureMechanismSectionAssembly(
            FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup detailedAssemblyCategoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup)
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResultWithProbability(simpleAssemblyCategoryGroup),
                CreateSectionAssemblyResultWithProbability(detailedAssemblyCategoryGroup),
                CreateSectionAssemblyResultWithProbability(tailorMadeAssemblyCategoryGroup),
                CreateSectionAssemblyResultWithProbability(13));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = failureMechanismSectionAssembly.SectionResults;
            Assert.AreEqual(1, serializedSectionResults.Length);

            if (simpleAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.SimpleAssembly,
                                                                                                 SerializableAssessmentType.SimpleAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            if (detailedAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.DetailedAssembly,
                                                                                                 SerializableAssessmentType.DetailedAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            if (tailorMadeAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.TailorMadeAssembly,
                                                                                                 SerializableAssessmentType.TailorMadeAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             failureMechanismSectionAssembly.CombinedSectionResult);

            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
        }

        [Test]
        public void CreateWithoutDetailedAssembly_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(null,
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10),
                                                                                                              CreateSectionAssemblyResultWithProbability(11),
                                                                                                              CreateSectionAssemblyResultWithProbability(12)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithoutDetailedAssembly_SerializableFailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          null,
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10),
                                                                                                              CreateSectionAssemblyResultWithProbability(11),
                                                                                                              CreateSectionAssemblyResultWithProbability(12)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableCollection", exception.ParamName);
        }

        [Test]
        public void CreateWithoutDetailedAssembly_SerializableFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          null,
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10),
                                                                                                              CreateSectionAssemblyResultWithProbability(11),
                                                                                                              CreateSectionAssemblyResultWithProbability(12)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableFailureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateWithoutDetailedAssembly_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          (ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void CreateWithoutDetailedAssembly_WithValidArgumentsAndAllAssemblyTypesHaveResults_ReturnsAggregatedSerializableFailureMechanismSectionAssembly()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResult(10),
                CreateSectionAssemblyResult(11),
                CreateSectionAssemblyResult(12));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = failureMechanismSectionAssembly.SectionResults;
            Assert.AreEqual(2, serializedSectionResults.Length);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.SimpleAssembly,
                                                                                             SerializableAssessmentType.SimpleAssessment,
                                                                                             serializedSectionResults[0]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.TailorMadeAssembly,
                                                                                             SerializableAssessmentType.TailorMadeAssessment,
                                                                                             serializedSectionResults[1]);
            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             failureMechanismSectionAssembly.CombinedSectionResult);

            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.None,
            TestName = "SimpleAssembly")]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.IIv,
            TestName = "TailorMadeAssembly")]
        public void CreateWithoutDetailedAssembly_WithValidArgumentsAndOneAssemblyTypeHasResult_ReturnsAggregatedSerializableFailureMechanismSectionAssembly(
            FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup)
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResult(simpleAssemblyCategoryGroup),
                CreateSectionAssemblyResult(tailorMadeAssemblyCategoryGroup),
                CreateSectionAssemblyResult(13));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = failureMechanismSectionAssembly.SectionResults;
            Assert.AreEqual(1, serializedSectionResults.Length);

            if (simpleAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.SimpleAssembly,
                                                                                                 SerializableAssessmentType.SimpleAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            if (tailorMadeAssemblyCategoryGroup != FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.TailorMadeAssembly,
                                                                                                 SerializableAssessmentType.TailorMadeAssessment,
                                                                                                 serializedSectionResults[0]);
            }

            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             failureMechanismSectionAssembly.CombinedSectionResult);

            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
        }

        [Test]
        public void CreateWithCombinedResultOnly_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(null,
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResult(10)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithCombinedResultOnly_SerializableFailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          null,
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResult(10)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableCollection", exception.ParamName);
        }

        [Test]
        public void CreateWithCombinedResultOnly_SerializableFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          null,
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResult(10)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableFailureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateWithCombinedResultOnly_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void CreateWithCombinedResultOnly_WithValidArgumentsAndAllAssemblyTypesHaveResults_ReturnsAggregatedSerializableFailureMechanismSectionAssembly()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResult(10));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            CollectionAssert.IsEmpty(failureMechanismSectionAssembly.SectionResults);
            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
        }

        [Test]
        public void CreateWithProbabilityAndCombinedResultOnly_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(null,
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithProbabilityAndCombinedResultOnly_SerializableFailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          null,
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableCollection", exception.ParamName);
        }

        [Test]
        public void CreateWithProbabilityAndCombinedResultOnly_SerializableFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          null,
                                                                                                          new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult(
                                                                                                              ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                                                                                                              CreateSectionAssemblyResultWithProbability(10)));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableFailureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateWithProbabilityAndCombinedResultOnly_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                          new SerializableFailureMechanismSectionCollection(),
                                                                                                          new SerializableFailureMechanism(),
                                                                                                          (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void CreateWithProbabilityANdCombinedResultOnly_WithValidArgumentsAndAllAssemblyTypesHaveResults_ReturnsAggregatedSerializableFailureMechanismSectionAssembly()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            var sectionResult = new ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                CreateSectionAssemblyResultWithProbability(10));

            var random = new Random(21);
            const string serializableFailureMechanismId = "FailureMechanismId";
            var serializableFailureMechanism = new SerializableFailureMechanism(serializableFailureMechanismId,
                                                                                new SerializableTotalAssemblyResult(),
                                                                                random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                                random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                                new SerializableFailureMechanismAssemblyResult());

            const string serializableSectionCollectionId = "CollectionId";
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            // Call
            AggregatedSerializableFailureMechanismSectionAssembly aggregatedSectionAssembly =
                AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            SerializableFailureMechanismSectionAssembly failureMechanismSectionAssembly = aggregatedSectionAssembly.FailureMechanismSectionAssembly;
            Assert.AreEqual("T.0", failureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, failureMechanismSectionAssembly.FailureMechanismId);

            CollectionAssert.IsEmpty(failureMechanismSectionAssembly.SectionResults);

            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(sectionResult.CombinedAssembly,
                                                                                             SerializableAssessmentType.CombinedAssessment,
                                                                                             failureMechanismSectionAssembly.CombinedSectionResult);

            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(sectionResult.FailureMechanismSection,
                                                                                        serializableCollection,
                                                                                        aggregatedSectionAssembly.FailureMechanismSection);
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

        private static ExportableSectionAssemblyResult CreateSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup assemblyCategory)
        {
            var random = new Random(21);
            return new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                       assemblyCategory);
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

        private static ExportableSectionAssemblyResultWithProbability CreateSectionAssemblyResultWithProbability(
            FailureMechanismSectionAssemblyCategoryGroup assemblyCategory)
        {
            var random = new Random(21);
            return new ExportableSectionAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                      assemblyCategory,
                                                                      random.NextDouble());
        }
    }
}