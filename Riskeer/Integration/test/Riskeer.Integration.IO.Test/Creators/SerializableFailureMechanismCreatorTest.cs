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
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableFailureMechanismCreator.Create(null, CreateSerializableTotalAssembly("id"), CreateExportableFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_SerializableTotalAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableFailureMechanismCreator.Create(new IdentifierGenerator(), null, CreateExportableFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("serializableTotalAssembly", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableFailureMechanismCreator.Create(new IdentifierGenerator(), CreateSerializableTotalAssembly("id"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanismAssemblyResult_ReturnsSerializableFailureMechanism()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();
            ExportableFailureMechanism failureMechanism = CreateExportableFailureMechanism();

            const string totalAssemblyId = "totalAssemblyId";
            SerializableTotalAssemblyResult serializableTotalAssembly = CreateSerializableTotalAssembly(totalAssemblyId);

            // Call
            SerializableFailureMechanism serializableFailureMechanism = SerializableFailureMechanismCreator.Create(idGenerator,
                                                                                                                   serializableTotalAssembly,
                                                                                                                   failureMechanism);

            // Assert
            Assert.AreEqual("Fm.0", serializableFailureMechanism.Id);
            Assert.AreEqual(serializableTotalAssembly.Id, serializableFailureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(failureMechanism.Code, serializableFailureMechanism.GenericFailureMechanismCode);

            SerializableFailureMechanismAssemblyResultTestHelper.AssertSerializableFailureMechanismAssemblyResult(failureMechanism.FailureMechanismAssembly,
                                                                                                                  serializableFailureMechanism.FailureMechanismAssemblyResult);
        }

        private static ExportableFailureMechanism CreateExportableFailureMechanism()
        {
            return new ExportableFailureMechanism(
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResult(),
                Enumerable.Empty<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>(),
                "code");
        }

        private static SerializableTotalAssemblyResult CreateSerializableTotalAssembly(string totalAssemblyId)
        {
            return new SerializableTotalAssemblyResult(totalAssemblyId,
                                                       new SerializableAssessmentProcess(),
                                                       new SerializableAssessmentSectionAssemblyResult());
        }
    }
}