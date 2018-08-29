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
    public class SerializableFailureMechanismSectionAssemblyCreatorTest
    {
        [Test]
        public void CreateWithoutProbability_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyCreator.Create(null, 
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
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyCreator.Create(new UniqueIdentifierGenerator(), 
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
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyCreator.Create(new UniqueIdentifierGenerator(),
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
            TestDelegate call = () => SerializableFailureMechanismSectionAssemblyCreator.Create(new UniqueIdentifierGenerator(),
                                                                                                new SerializableFailureMechanismSectionCollection(),
                                                                                                new SerializableFailureMechanism(), 
                                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void CreateWithoutProbability_WithValidArguments_ReturnsSerializableFailureMechanismSectionAssembly()
        {
            // Setup
            var idGenerator = new UniqueIdentifierGenerator();
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
            var serializableCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId,
                                                                                           serializableFailureMechanism);

            // Call
            SerializableFailureMechanismSectionAssembly serializableSectionAssembly =
                SerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, sectionResult);

            // Assert
            Assert.AreEqual("T.0", serializableSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanism.Id, serializableSectionAssembly.FailureMechanismId);
            Assert.AreEqual("Wks.1", serializableSectionAssembly.FailureMechanismSectionId);

            SerializableFailureMechanismSectionAssemblyResult[] serializedSectionResults = serializableSectionAssembly.SectionResults;
            Assert.AreEqual(3, serializedSectionResults.Length);
            AssertAssemblyResult(sectionResult.SimpleAssembly,
                                 SerializableAssessmentType.SimpleAssessment,
                                 serializedSectionResults[0]);
            AssertAssemblyResult(sectionResult.DetailedAssembly,
                                 SerializableAssessmentType.DetailedAssessment,
                                 serializedSectionResults[1]);
            AssertAssemblyResult(sectionResult.TailorMadeAssembly,
                                 SerializableAssessmentType.TailorMadeAssessment,
                                 serializedSectionResults[2]);
            AssertAssemblyResult(sectionResult.CombinedAssembly,
                                 SerializableAssessmentType.CombinedAssessment,
                                 serializableSectionAssembly.CombinedSectionResult);
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
    }
}