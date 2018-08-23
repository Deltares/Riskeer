// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismFactoryTest
    {
        [Test]
        public void CreateDefaultExportableFailureMechanismWithProbability_Always_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(failureMechanismCode, group, assemblyMethod);

            // Assert
            Assert.AreEqual(group, exportableFailureMechanism.Group);
            Assert.AreEqual(failureMechanismCode, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(assemblyMethod, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(0, failureMechanismAssemblyResult.Probability);

            CollectionAssert.IsEmpty(exportableFailureMechanism.Sections);
            CollectionAssert.IsEmpty(exportableFailureMechanism.SectionAssemblyResults);
        }

        [Test]
        public void CreateDefaultExportableFailureMechanismWithoutProbability_Always_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(failureMechanismCode, group, assemblyMethod);

            // Assert
            Assert.AreEqual(group, exportableFailureMechanism.Group);
            Assert.AreEqual(failureMechanismCode, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(assemblyMethod, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);

            CollectionAssert.IsEmpty(exportableFailureMechanism.Sections);
            CollectionAssert.IsEmpty(exportableFailureMechanism.SectionAssemblyResults);
        }
    }
}