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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Exceptions;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableCombinedFailureMechanismSectionAssemblyResultCreatorTest
    {
        [Test]
        public void Create_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableCombinedFailureMechanismSectionAssemblyResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant)]
        public void Create_SectionResultWithInvalidAssemblyGroup_ThrowsAssemblyCreatorException(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            // Setup
            var random = new Random(21);
            var sectionResult = new ExportableFailureMechanismCombinedSectionAssemblyResult(
                new ExportableFailureMechanismSubSectionAssemblyResult(assemblyGroup, random.NextEnumValue<ExportableAssemblyMethod>()),
                random.NextEnumValue<ExportableFailureMechanismType>(),
                "code");

            // Call
            void Call() => SerializableCombinedFailureMechanismSectionAssemblyResultCreator.Create(sectionResult);

            // Assert
            var exception = Assert.Throws<AssemblyCreatorException>(Call);
            Assert.AreEqual("The assembly result is invalid and cannot be created.", exception.Message);
        }

        [Test]
        public void Create_WithExportableFailureMechanismCombinedSectionAssemblyResult_ReturnsSerializableCombinedFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var sectionResult = new ExportableFailureMechanismCombinedSectionAssemblyResult(
                CreateSectionAssemblyResult(), random.NextEnumValue<ExportableFailureMechanismType>(), "code");

            // Call
            SerializableCombinedFailureMechanismSectionAssemblyResult serializableResult =
                SerializableCombinedFailureMechanismSectionAssemblyResultCreator.Create(sectionResult);

            // Assert
            Assert.AreEqual(SerializableFailureMechanismTypeCreator.Create(sectionResult.FailureMechanismType),
                            serializableResult.FailureMechanismType);
            Assert.AreEqual(sectionResult.Code, serializableResult.GenericFailureMechanismCode);
            ExportableFailureMechanismSubSectionAssemblyResult expectedSectionAssemblyResult = sectionResult.SectionAssemblyResult;
            Assert.AreEqual(SerializableFailureMechanismSectionAssemblyGroupCreator.Create(expectedSectionAssemblyResult.AssemblyGroup),
                            serializableResult.AssemblyGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedSectionAssemblyResult.AssemblyMethod),
                            serializableResult.AssemblyMethod);
        }

        private static ExportableFailureMechanismSubSectionAssemblyResult CreateSectionAssemblyResult()
        {
            var random = new Random(21);
            return new ExportableFailureMechanismSubSectionAssemblyResult(
                random.NextEnumValue(new[]
                {
                    FailureMechanismSectionAssemblyGroup.NotDominant,
                    FailureMechanismSectionAssemblyGroup.III,
                    FailureMechanismSectionAssemblyGroup.II,
                    FailureMechanismSectionAssemblyGroup.I,
                    FailureMechanismSectionAssemblyGroup.Zero,
                    FailureMechanismSectionAssemblyGroup.IIIMin,
                    FailureMechanismSectionAssemblyGroup.IIMin,
                    FailureMechanismSectionAssemblyGroup.IMin,
                    FailureMechanismSectionAssemblyGroup.NotRelevant
                }), random.NextEnumValue<ExportableAssemblyMethod>());
        }
    }
}