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
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableSectionAssemblyResultFactoryTest
    {
        [Test]
        public void CreateExportableSectionAssemblyResult_WithValidArguments_ReturnsExportableAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var assembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableSectionAssemblyResult exportableAssemblyResult =
                ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(assembly, assemblyMethod);

            // Assert
            Assert.AreEqual(assembly, exportableAssemblyResult.AssemblyCategory);
            Assert.AreEqual(assemblyMethod, exportableAssemblyResult.AssemblyMethod);
        }

        [Test]
        public void CreateExportableSectionAssemblyResultWithProbability_FailureMechanismSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Setiup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            TestDelegate call = () => ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(
                null, assemblyMethod);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionAssembly", exception.ParamName);
        }

        [Test]
        public void CreateExportableSectionAssemblyResultWithProbability_WithFailureMechanismAssembly_ReturnsExportableAssemblyResultWithProbability()
        {
            // Setup
            var random = new Random(21);
            var assembly = new FailureMechanismSectionAssembly(random.NextDouble(),
                                                               random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableSectionAssemblyResultWithProbability exportableAssemblyResult =
                ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(assembly, assemblyMethod);

            // Assert
            Assert.AreEqual(assembly.Group, exportableAssemblyResult.AssemblyCategory);
            Assert.AreEqual(assembly.Probability, exportableAssemblyResult.Probability);
            Assert.AreEqual(assemblyMethod, exportableAssemblyResult.AssemblyMethod);
        }
    }
}