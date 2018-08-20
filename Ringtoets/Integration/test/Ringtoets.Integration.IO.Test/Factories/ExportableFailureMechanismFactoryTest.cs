﻿using System;
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