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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.AggregatedSerializable;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreator.Create(null,
                                                                                                                  new SerializableTotalAssemblyResult(),
                                                                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                                                                  CreateExportableCombinedSectionAssembly());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                                  null,
                                                                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                                                                  CreateExportableCombinedSectionAssembly());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableTotalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                                  new SerializableTotalAssemblyResult(),
                                                                                                                  null,
                                                                                                                  CreateExportableCombinedSectionAssembly());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableFailureMechanismSectionCollection", exception.ParamName);
        }

        [Test]
        public void Create_CombinedSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreator.Create(new IdentifierGenerator(),
                                                                                                                  new SerializableTotalAssemblyResult(),
                                                                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Create_WithValidArguments_ReturnsAggregatedSerializableCombinedFailureMechanismSectionAssembly()
        {
            // Setup
            var combinedSectionAssembly = new ExportableCombinedSectionAssembly(ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection(),
                                                                                CreateSectionAssemblyResult(10),
                                                                                new[]
                                                                                {
                                                                                    CreateCombinedSectionAssemblyResult(10),
                                                                                    CreateCombinedSectionAssemblyResult(11)
                                                                                });

            const string serializableTotalAssemblyResultId = "serializableTotalAssemblyId";
            SerializableTotalAssemblyResult serializableTotalAssembly = CreateSerializableTotalAssembly(serializableTotalAssemblyResultId);

            const string serializableSectionCollectionId = "serializableSectionCollectionId";
            var serializableSectionCollection = new SerializableFailureMechanismSectionCollection(serializableSectionCollectionId);

            var idGenerator = new IdentifierGenerator();

            // Call
            AggregatedSerializableCombinedFailureMechanismSectionAssembly aggregate =
                AggregatedSerializableCombinedFailureMechanismSectionAssemblyCreator.Create(idGenerator,
                                                                                            serializableTotalAssembly,
                                                                                            serializableSectionCollection,
                                                                                            combinedSectionAssembly);

            // Assert
            SerializableFailureMechanismSection serializableFailureMechanismSection = aggregate.FailureMechanismSection;
            SerializableFailureMechanismSectionTestHelper.AssertFailureMechanismSection(combinedSectionAssembly.Section,
                                                                                        serializableSectionCollection,
                                                                                        serializableFailureMechanismSection);

            SerializableCombinedFailureMechanismSectionAssembly serializableCombinedFailureMechanismSectionAssembly = aggregate.CombinedFailureMechanismSectionAssembly;
            Assert.AreEqual("Gto.0", serializableCombinedFailureMechanismSectionAssembly.Id);
            Assert.AreEqual(serializableFailureMechanismSection.Id, serializableCombinedFailureMechanismSectionAssembly.FailureMechanismSectionId);
            Assert.AreEqual(serializableTotalAssembly.Id, serializableCombinedFailureMechanismSectionAssembly.TotalAssemblyResultId);

            SerializableFailureMechanismSectionAssemblyResultTestHelper.AssertAssemblyResult(combinedSectionAssembly.CombinedSectionAssemblyResult,
                                                                                             SerializableAssessmentType.CombinedSectionAssessment,
                                                                                             serializableCombinedFailureMechanismSectionAssembly.CombinedSectionResult);

            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> expectedFailureMechanismSectionResults = combinedSectionAssembly.FailureMechanismResults;
            SerializableCombinedFailureMechanismSectionAssemblyResult[] serializedFailureMechanismSectionResults = serializableCombinedFailureMechanismSectionAssembly.FailureMechanismResults;
            int expectedNrOfFailureMechanismResult = expectedFailureMechanismSectionResults.Count();
            Assert.AreEqual(expectedNrOfFailureMechanismResult, serializedFailureMechanismSectionResults.Length);
            for (var i = 0; i < expectedNrOfFailureMechanismResult; i++)
            {
                AssertCombinedSectionAssemblyResult(expectedFailureMechanismSectionResults.ElementAt(i),
                                                    serializedFailureMechanismSectionResults[i]);
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

        private static ExportableCombinedSectionAssembly CreateExportableCombinedSectionAssembly()
        {
            return new ExportableCombinedSectionAssembly(ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection(),
                                                         ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                                                         Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>());
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

        private static void AssertCombinedSectionAssemblyResult(ExportableFailureMechanismCombinedSectionAssemblyResult expectedSectionResult,
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