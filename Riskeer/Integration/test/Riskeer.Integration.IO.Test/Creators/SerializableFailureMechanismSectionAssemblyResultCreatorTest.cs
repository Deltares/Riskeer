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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableFailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableFailureMechanismSectionAssemblyResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr)]
        public void Create_SectionResultHasInvalidAssemblyGroup_ThrowsAssemblyCreatorException(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            // Setup
            var random = new Random(21);
            var sectionResult = new ExportableFailureMechanismSectionAssemblyWithProbabilityResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                assemblyGroup, random.NextDouble(), random.NextEnumValue<ExportableAssemblyMethod>(),
                random.NextEnumValue<ExportableAssemblyMethod>());

            // Call
            void Call() => SerializableFailureMechanismSectionAssemblyResultCreator.Create(sectionResult);

            // Assert
            var exception = Assert.Throws<AssemblyCreatorException>(Call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_ValidData_ReturnsSerializableFailureMechanismAssemblyResult()
        {
            // Setup
            ExportableFailureMechanismSectionAssemblyWithProbabilityResult sectionResult = ExportableFailureMechanismSectionAssemblyResultTestFactory.CreateWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(), 21);

            // Call
            SerializableFailureMechanismSectionAssemblyResult serializableAssemblyResult = SerializableFailureMechanismSectionAssemblyResultCreator.Create(sectionResult);

            // Assert
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(sectionResult.ProbabilityAssemblyMethod),
                            serializableAssemblyResult.ProbabilityAssemblyMethod);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyGroupAssemblyMethod),
                            serializableAssemblyResult.AssemblyGroupAssemblyMethod);
            Assert.AreEqual(SerializableFailureMechanismSectionAssemblyGroupCreator.Create(
                                sectionResult.AssemblyGroup), serializableAssemblyResult.AssemblyGroup);
            Assert.AreEqual(sectionResult.Probability, serializableAssemblyResult.Probability);
        }
    }
}