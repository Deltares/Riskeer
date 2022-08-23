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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class ExportableGenericFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string id = "id";
            const string code = "code";

            ExportableFailureMechanismAssemblyResult failureMechanismAssembly =
                ExportableFailureMechanismAssemblyResultTestFactory.CreateResult();
            IEnumerable<ExportableFailureMechanismSectionAssemblyResult> sectionAssemblyResults =
                Enumerable.Empty<ExportableFailureMechanismSectionAssemblyResult>();

            // Call
            var failureMechanism = new ExportableGenericFailureMechanism(
                id, failureMechanismAssembly, sectionAssemblyResults, code);

            // Assert
            Assert.IsInstanceOf<ExportableFailureMechanism>(failureMechanism);
            Assert.AreEqual(id, failureMechanism.Id);
            Assert.AreSame(failureMechanismAssembly, failureMechanism.FailureMechanismAssembly);
            Assert.AreSame(sectionAssemblyResults, failureMechanism.SectionAssemblyResults);
            Assert.AreEqual(code, failureMechanism.Code);
        }
    }
}