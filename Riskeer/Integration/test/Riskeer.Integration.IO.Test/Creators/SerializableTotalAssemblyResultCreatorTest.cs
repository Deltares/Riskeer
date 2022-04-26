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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableTotalAssemblyResultCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random();

            // Call
            void Call() => SerializableTotalAssemblyResultCreator.Create(null,
                                                                         new SerializableAssessmentProcess(),
                                                                         random.NextEnumValue<SerializableAssemblyMethod>(),
                                                                         random.NextEnumValue<SerializableAssemblyMethod>(),
                                                                         random.NextEnumValue<SerializableAssessmentSectionAssemblyGroup>(),
                                                                         random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_WithValidArguments_ReturnsSerializableTotalAssemblyResult()
        {
            // Setup
            var idGenerator = new IdentifierGenerator();

            const string assessmentProcessId = "assessmentProcessId";
            var serializableAssessmentProcess = new SerializableAssessmentProcess(assessmentProcessId, new SerializableAssessmentSection());

            var random = new Random();
            var probabilityAssemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var assemblyGroupAssemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var assemblyGroup = random.NextEnumValue<SerializableAssessmentSectionAssemblyGroup>();
            double probability = random.NextDouble();

            // Call
            SerializableTotalAssemblyResult serializableTotalAssembly =
                SerializableTotalAssemblyResultCreator.Create(idGenerator,
                                                              serializableAssessmentProcess,
                                                              probabilityAssemblyMethod,
                                                              assemblyGroupAssemblyMethod,
                                                              assemblyGroup,
                                                              probability);

            // Assert
            Assert.AreEqual("Vo.0", serializableTotalAssembly.Id);
            Assert.AreEqual(serializableAssessmentProcess.Id, serializableTotalAssembly.AssessmentProcessId);
            Assert.AreEqual(probabilityAssemblyMethod, serializableTotalAssembly.ProbabilityAssemblyMethod);
            Assert.AreEqual(assemblyGroupAssemblyMethod, serializableTotalAssembly.AssemblyGroupAssemblyMethod);
            Assert.AreEqual(assemblyGroup, serializableTotalAssembly.AssemblyGroup);
            Assert.AreEqual(probability, serializableTotalAssembly.Probability);
        }
    }
}