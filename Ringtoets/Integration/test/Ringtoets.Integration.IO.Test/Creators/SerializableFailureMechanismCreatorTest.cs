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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.TestUtil;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismCreatorTest
    {
        [Test]
        public void CreateWithFailureMechanismAssemblyResult_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismCreator.Create(null,
                                                                                 CreateSerializableTotalAssembly("id"),
                                                                                 CreateExportableFailureMechanismWithoutProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithFailureMechanismAssemblyResult_SerializableTotalAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                 null,
                                                                                 CreateExportableFailureMechanismWithoutProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableTotalAssembly", exception.ParamName);
        }

        [Test]
        public void CreateWithFailureMechanismAssemblyResult_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                 CreateSerializableTotalAssembly("id"),
                                                                                 (ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanismAssemblyResult_ReturnsSerializableFailureMechanism()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> failureMechanism = CreateExportableFailureMechanismWithoutProbability();

            const string totalAssemblyId = "totalAssemblyId";
            SerializableTotalAssemblyResult serializableTotalAssembly = CreateSerializableTotalAssembly(totalAssemblyId);

            // Call
            SerializableFailureMechanism serializableFailureMechanism = SerializableFailureMechanismCreator.Create(idGenerator,
                                                                                                                   serializableTotalAssembly,
                                                                                                                   failureMechanism);

            // Assert
            Assert.AreEqual("Ts.0", serializableFailureMechanism.Id);
            Assert.AreEqual(serializableTotalAssembly.Id, serializableFailureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(SerializableFailureMechanismGroupCreator.Create(failureMechanism.Group), serializableFailureMechanism.FailureMechanismGroup);
            Assert.AreEqual(SerializableFailureMechanismTypeCreator.Create(failureMechanism.Code), serializableFailureMechanism.FailureMechanismType);

            SerializableFailureMechanismAssemblyResultTestHelper.AssertSerializableFailureMechanismAssemblyResult(failureMechanism.FailureMechanismAssembly,
                                                                                                                  serializableFailureMechanism.FailureMechanismAssemblyResult);
        }

        [Test]
        public void CreateWithFailureMechanismAssemblyResultWithProbability_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismCreator.Create(null,
                                                                                 CreateSerializableTotalAssembly("id"),
                                                                                 CreateExportableFailureMechanismWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void CreateWithFailureMechanismAssemblyResultWithProbability_SerializableTotalAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                 null,
                                                                                 CreateExportableFailureMechanismWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("serializableTotalAssembly", exception.ParamName);
        }

        [Test]
        public void CreateWithFailureMechanismAssemblyResultWithProbability_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableFailureMechanismCreator.Create(new IdentifierGenerator(),
                                                                                 CreateSerializableTotalAssembly("id"),
                                                                                 (ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanismAssemblyResultWithProbability_ReturnsSerializableFailureMechanism()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> failureMechanism = CreateExportableFailureMechanismWithProbability();

            const string totalAssemblyId = "totalAssemblyId";
            SerializableTotalAssemblyResult serializableTotalAssembly = CreateSerializableTotalAssembly(totalAssemblyId);

            // Call
            SerializableFailureMechanism serializableFailureMechanism = SerializableFailureMechanismCreator.Create(idGenerator,
                                                                                                                   serializableTotalAssembly,
                                                                                                                   failureMechanism);

            // Assert
            Assert.AreEqual("Ts.0", serializableFailureMechanism.Id);
            Assert.AreEqual(serializableTotalAssembly.Id, serializableFailureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(SerializableFailureMechanismGroupCreator.Create(failureMechanism.Group), serializableFailureMechanism.FailureMechanismGroup);
            Assert.AreEqual(SerializableFailureMechanismTypeCreator.Create(failureMechanism.Code), serializableFailureMechanism.FailureMechanismType);

            SerializableFailureMechanismAssemblyResultTestHelper.AssertSerializableFailureMechanismAssemblyResult(failureMechanism.FailureMechanismAssembly,
                                                                                                                  serializableFailureMechanism.FailureMechanismAssemblyResult);
        }

        private static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> CreateExportableFailureMechanismWithoutProbability()
        {
            var random = new Random(21);
            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(),
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());
        }

        private static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateExportableFailureMechanismWithProbability()
        {
            var random = new Random(21);
            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(),
                random.NextEnumValue<ExportableFailureMechanismType>(),
                random.NextEnumValue<ExportableFailureMechanismGroup>());
        }

        private static SerializableTotalAssemblyResult CreateSerializableTotalAssembly(string totalAssemblyId)
        {
            return new SerializableTotalAssemblyResult(totalAssemblyId,
                                                       new SerializableAssessmentProcess(),
                                                       new SerializableFailureMechanismAssemblyResult(),
                                                       new SerializableFailureMechanismAssemblyResult(),
                                                       new SerializableAssessmentSectionAssemblyResult());
        }
    }
}