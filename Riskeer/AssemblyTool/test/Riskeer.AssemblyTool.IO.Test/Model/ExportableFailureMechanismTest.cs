// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableFailureMechanismTest
    {
        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            void Call() => new TestExportableFailureMechanism(invalidId, null, Enumerable.Empty<ExportableFailureMechanismSectionAssemblyResult>());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_FailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestExportableFailureMechanism("id", null, Enumerable.Empty<ExportableFailureMechanismSectionAssemblyResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestExportableFailureMechanism("id", ExportableFailureMechanismAssemblyResultTestFactory.CreateResult(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string id = "id";

            ExportableFailureMechanismAssemblyResult failureMechanismAssembly =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResult();
            IEnumerable<ExportableFailureMechanismSectionAssemblyResult> sectionAssemblyResults =
                Enumerable.Empty<ExportableFailureMechanismSectionAssemblyResult>();

            // Call
            var failureMechanism = new TestExportableFailureMechanism(id, failureMechanismAssembly, sectionAssemblyResults);

            // Assert
            Assert.AreEqual(id, failureMechanism.Id);
            Assert.AreSame(failureMechanismAssembly, failureMechanism.FailureMechanismAssembly);
            Assert.AreSame(sectionAssemblyResults, failureMechanism.SectionAssemblyResults);
        }

        private class TestExportableFailureMechanism : ExportableFailureMechanism
        {
            public TestExportableFailureMechanism(string id, ExportableFailureMechanismAssemblyResult failureMechanismAssembly,
                                                  IEnumerable<ExportableFailureMechanismSectionAssemblyResult> sectionAssemblyResults)
                : base(id, failureMechanismAssembly, sectionAssemblyResults) {}
        }
    }
}