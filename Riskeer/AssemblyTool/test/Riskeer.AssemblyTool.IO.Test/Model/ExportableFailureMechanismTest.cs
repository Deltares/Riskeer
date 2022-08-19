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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
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
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanism(invalidId, null, Enumerable.Empty<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>(),
                                                          random.NextEnumValue<ExportableFailureMechanismType>(), string.Empty, string.Empty);

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }
        
        [Test]
        public void Constructor_FailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanism("id", null, Enumerable.Empty<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>(),
                                                          random.NextEnumValue<ExportableFailureMechanismType>(), string.Empty, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanism("id", ExportableFailureMechanismAssemblyResultTestFactory.CreateResult(),
                                                          null, random.NextEnumValue<ExportableFailureMechanismType>(),
                                                          string.Empty, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string id = "id";
            const string code = "code";
            const string name = "name";
            
            var random = new Random(21);

            ExportableFailureMechanismAssemblyResult failureMechanismAssembly =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResult();
            IEnumerable<ExportableFailureMechanismSectionAssemblyWithProbabilityResult> sectionAssemblyResults =
                Enumerable.Empty<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>();
            var failureMechanismType = random.NextEnumValue<ExportableFailureMechanismType>();
            

            // Call
            var failureMechanism = new ExportableFailureMechanism(id, failureMechanismAssembly, sectionAssemblyResults, failureMechanismType, code, name);

            // Assert
            Assert.AreEqual(id, failureMechanism.Id);
            Assert.AreSame(failureMechanismAssembly, failureMechanism.FailureMechanismAssembly);
            Assert.AreSame(sectionAssemblyResults, failureMechanism.SectionAssemblyResults);
            Assert.AreEqual(failureMechanismType, failureMechanism.FailureMechanismType);
            Assert.AreEqual(code, failureMechanism.Code);
            Assert.AreEqual(name, failureMechanism.Name);
        }
    }
}