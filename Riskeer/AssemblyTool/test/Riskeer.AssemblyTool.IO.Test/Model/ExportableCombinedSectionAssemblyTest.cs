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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyTest
    {
        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Setup
            var random = new Random(21);
            ExportableCombinedFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection();

            // Call
            void Call() => new ExportableCombinedSectionAssembly(
                invalidId, section, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                random.NextEnumValue<ExportableAssemblyMethod>(), Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableCombinedSectionAssembly(
                "id", null, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                random.NextEnumValue<ExportableAssemblyMethod>(), Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            ExportableCombinedFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection();

            // Call
            void Call() => new ExportableCombinedSectionAssembly(
                "id", section, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                random.NextEnumValue<ExportableAssemblyMethod>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismResults", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string id = "id";
            var assemblyGroup = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            ExportableCombinedFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection();

            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults =
                Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            var result = new ExportableCombinedSectionAssembly(id, section, assemblyGroup, assemblyMethod, failureMechanismResults);

            // Assert
            Assert.AreEqual(id, result.Id);
            Assert.AreSame(section, result.Section);
            Assert.AreEqual(assemblyGroup, result.AssemblyGroup);
            Assert.AreEqual(assemblyMethod, result.AssemblyGroupAssemblyMethod);
            Assert.AreSame(failureMechanismResults, result.FailureMechanismResults);
        }
    }
}