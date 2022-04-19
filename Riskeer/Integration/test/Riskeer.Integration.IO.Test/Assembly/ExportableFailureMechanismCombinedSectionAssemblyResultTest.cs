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
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismCombinedSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_SectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new ExportableFailureMechanismCombinedSectionAssemblyResult(
                null, random.NextEnumValue<ExportableFailureMechanismType>(), string.Empty, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            const string code = "code";
            const string name = "name";
            var random = new Random(21);
            var combinedSectionAssembly = new ExportableFailureMechanismSubSectionAssemblyResult(
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(), random.NextEnumValue<ExportableAssemblyMethod>());
            var failureMechanismType = random.NextEnumValue<ExportableFailureMechanismType>();

            // Call
            var assemblyResult = new ExportableFailureMechanismCombinedSectionAssemblyResult(
                combinedSectionAssembly, failureMechanismType, code, name);

            // Assert
            Assert.AreSame(combinedSectionAssembly, assemblyResult.SectionAssemblyResult);
            Assert.AreEqual(failureMechanismType, assemblyResult.FailureMechanismType);
            Assert.AreEqual(code, assemblyResult.Code);
            Assert.AreEqual(name, assemblyResult.Name);
        }
    }
}