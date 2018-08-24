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
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableTotalAssemblyResultCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableTotalAssemblyResultCreator.Create(null,
                                                                                    new SerializableAssessmentProcess(),
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    new SerializableAssessmentSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_AssessmentProcessNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableTotalAssemblyResultCreator.Create(new UniqueIdentifierGenerator(),
                                                                                    null,
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    new SerializableAssessmentSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentProcess", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismAssemblyResultWithProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableTotalAssemblyResultCreator.Create(new UniqueIdentifierGenerator(),
                                                                                    new SerializableAssessmentProcess(),
                                                                                    null,
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    new SerializableAssessmentSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assemblyResultWithProbability", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismAssemblyResultWithoutProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableTotalAssemblyResultCreator.Create(new UniqueIdentifierGenerator(),
                                                                                    new SerializableAssessmentProcess(),
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    null,
                                                                                    new SerializableAssessmentSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assemblyResultWithoutProbability", exception.ParamName);
        }

        [Test]
        public void Create_AssessmentSectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableTotalAssemblyResultCreator.Create(new UniqueIdentifierGenerator(),
                                                                                    new SerializableAssessmentProcess(),
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    new SerializableFailureMechanismAssemblyResult(),
                                                                                    null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Create_WithValidArguments_ReturnsSerializableTotalAssemblyResult()
        {
            // Setup
            var idGenerator = new UniqueIdentifierGenerator();

            const string assessmentProcessId = "serializable assessment process id";
            var serializableAssessmentProcess = new SerializableAssessmentProcess(assessmentProcessId, new SerializableAssessmentSection());
            var failureMechanismAssemblyResultWithProbability = new SerializableFailureMechanismAssemblyResult();
            var failureMechanismAssemblyResultWithoutProbability = new SerializableFailureMechanismAssemblyResult();
            var assessmentSectionAssemblyResult = new SerializableAssessmentSectionAssemblyResult();

            // Call
            SerializableTotalAssemblyResult serializableTotalAssembly =
                SerializableTotalAssemblyResultCreator.Create(idGenerator,
                                                              serializableAssessmentProcess,
                                                              failureMechanismAssemblyResultWithProbability,
                                                              failureMechanismAssemblyResultWithoutProbability,
                                                              assessmentSectionAssemblyResult);

            // Assert
            Assert.AreEqual("0", serializableTotalAssembly.Id);
            Assert.AreEqual(serializableAssessmentProcess.Id, serializableTotalAssembly.AssessmentProcessId);
            Assert.AreSame(failureMechanismAssemblyResultWithProbability, serializableTotalAssembly.AssemblyResultWithProbability);
            Assert.AreSame(failureMechanismAssemblyResultWithoutProbability, serializableTotalAssembly.AssemblyResultWithoutProbability);
            Assert.AreSame(assessmentSectionAssemblyResult, serializableTotalAssembly.AssessmentSectionAssemblyResult);
        }
    }
}