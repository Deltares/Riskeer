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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableTotalAssemblyResultTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var totalAssemblyResult = new SerializableTotalAssemblyResult();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(totalAssemblyResult);
            Assert.IsNull(totalAssemblyResult.Id);
            Assert.IsNull(totalAssemblyResult.AssessmentProcessId);
            Assert.IsNull(totalAssemblyResult.AssemblyResultWithProbability);
            Assert.IsNull(totalAssemblyResult.AssemblyResultWithoutProbability);
            Assert.IsNull(totalAssemblyResult.AssessmentSectionAssemblyResult);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableTotalAssemblyResult), "Veiligheidsoordeel");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableTotalAssemblyResult>(
                nameof(SerializableTotalAssemblyResult.Id), "VeiligheidsoordeelID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableTotalAssemblyResult>(
                nameof(SerializableTotalAssemblyResult.AssessmentProcessId), "BeoordelingsprocesIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableTotalAssemblyResult>(
                nameof(SerializableTotalAssemblyResult.AssemblyResultWithProbability), "toetsoordeelMetKansschatting");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableTotalAssemblyResult>(
                nameof(SerializableTotalAssemblyResult.AssemblyResultWithoutProbability), "toetsoordeelZonderKansschatting");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableTotalAssemblyResult>(
                nameof(SerializableTotalAssemblyResult.AssessmentSectionAssemblyResult), "veiligheidsoordeel");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            TestDelegate call = () => new SerializableTotalAssemblyResult(invalidId,
                                                                          new SerializableAssessmentProcess(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableAssessmentSectionAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_AssessmentProcessNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableTotalAssemblyResult("id",
                                                                          null,
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableAssessmentSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentProcess", exception.ParamName);
        }

        [Test]
        public void Constructor_AssemblyResultWithoutProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableTotalAssemblyResult("id",
                                                                          new SerializableAssessmentProcess(),
                                                                          null,
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableAssessmentSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assemblyResultWithoutProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_AssemblyResultWithProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableTotalAssemblyResult("id",
                                                                          new SerializableAssessmentProcess(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          null,
                                                                          new SerializableAssessmentSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assemblyResultWithProbability", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableTotalAssemblyResult("id",
                                                                          new SerializableAssessmentProcess(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSectionAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "id";

            var assessmentProcess = new SerializableAssessmentProcess("processId",
                                                                      new SerializableAssessmentSection());
            var resultWithoutProbability = new SerializableFailureMechanismAssemblyResult();
            var resultWithProbability = new SerializableFailureMechanismAssemblyResult();
            var assessmentSectionResult = new SerializableAssessmentSectionAssemblyResult();

            // Call
            var totalAssemblyResult = new SerializableTotalAssemblyResult(id,
                                                                          assessmentProcess,
                                                                          resultWithoutProbability,
                                                                          resultWithProbability,
                                                                          assessmentSectionResult);

            // Assert
            Assert.AreEqual(id, totalAssemblyResult.Id);
            Assert.AreEqual(assessmentProcess.Id, totalAssemblyResult.AssessmentProcessId);
            Assert.AreSame(resultWithoutProbability, totalAssemblyResult.AssemblyResultWithoutProbability);
            Assert.AreSame(resultWithProbability, totalAssemblyResult.AssemblyResultWithProbability);
            Assert.AreSame(assessmentSectionResult, totalAssemblyResult.AssessmentSectionAssemblyResult);
        }
    }
}