using System;
using System.IO;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Importers;

namespace Riskeer.Integration.IO.Test.Importers
{
    [TestFixture]
    public class AssemblyImporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new AssemblyImporter(CreateExportableAssessmentSection(), "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<ExportableAssessmentSection>>(importer);
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new AssemblyImporter(CreateExportableAssessmentSection(), path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importSuccessful);
        }

        private static ExportableAssessmentSection CreateExportableAssessmentSection()
        {
            var random = new Random(21);
            return new ExportableAssessmentSection("", "", new Point2D[0],
                                                   new ExportableAssessmentSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(), random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>()),
                                                   new ExportableFailureMechanismAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>(),
                                                                                                               random.NextDouble()),
                                                   new ExportableFailureMechanismAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>()),
                                                   new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>[0], new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>[0],
                                                   new ExportableCombinedSectionAssembly[0]);
        }
    }
}