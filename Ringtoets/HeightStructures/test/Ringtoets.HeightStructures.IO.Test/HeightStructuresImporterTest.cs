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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Plugin.FileImporters;

namespace Ringtoets.HeightStructures.IO.Test
{
    [TestFixture]
    public class HeightStructuresImporterTest
    {
        private readonly string commonIoTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "Structures");
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO);
        private readonly StructureCollection<HeightStructure> testImportTarget = new StructureCollection<HeightStructure>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_StructureUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HeightStructuresImporter(testImportTarget, testReferenceLine,
                                                                   testFilePath, messageProvider,
                                                                   null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureUpdateStrategy", paramName);
        }

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var replaceDataStrategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            var importer = new HeightStructuresImporter(testImportTarget, testReferenceLine, testFilePath,
                                                        messageProvider, replaceDataStrategy);

            // Assert
            Assert.IsInstanceOf<StructuresImporter<StructureCollection<HeightStructure>>>(importer);
        }

        [Test]
        public void Import_ValidIncompleteFile_LogAndFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = Path.Combine(commonIoTestDataPath, "CorrectFiles", "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var replaceDataStrategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            var structuresImporter = new HeightStructuresImporter(failureMechanism.HeightStructures,
                                                                  referenceLine, filePath, messageProvider,
                                                                  replaceDataStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string message = CreateExpectedErrorMessage(csvFilePath, "Gemaal Leemans (93k3)", "KUNST2", new[]
            {
                "Geen geldige parameter definities gevonden."
            });
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error));
            Assert.IsFalse(importResult);
            Assert.AreEqual(0, failureMechanism.HeightStructures.Count);
            Assert.IsNull(failureMechanism.HeightStructures.SourcePath);
        }

        [Test]
        public void Import_ValidFileWithConversionsBetweenVarianceTypes_WarnUserAboutConversion()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, "HeightStructuresVarianceConvert", "StructureNeedVarianceValueConversion.shp");

            ReferenceLine referenceLine = CreateReferenceLine();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var replaceDataStrategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            var structuresImporter = new HeightStructuresImporter(failureMechanism.HeightStructures,
                                                                  referenceLine, filePath, messageProvider,
                                                                  replaceDataStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                "De variatie voor parameter 'KW_HOOGTE2' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 3).",
                "De variatie voor parameter 'KW_HOOGTE3' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 4).",
                "De variatie voor parameter 'KW_HOOGTE4' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 5).",
                "De variatie voor parameter 'KW_HOOGTE5' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 6).",
                "De variatie voor parameter 'KW_HOOGTE7' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 8).",
                "De variatie voor parameter 'KW_HOOGTE8' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 9)."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, failureMechanism.HeightStructures.Count);
            HeightStructure structure = failureMechanism.HeightStructures[0];
            Assert.AreEqual(0.12, structure.LevelCrestStructure.StandardDeviation.Value);
            Assert.AreEqual(0.24, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(1.0, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(0.97, structure.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(1.84, structure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(2.18, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(filePath, failureMechanism.HeightStructures.SourcePath);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_InvalidCsvFile_LogAndFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpIncompleteCsv", "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var replaceDataStrategy = new HeightStructureReplaceDataStrategy(failureMechanism);
            var structuresImporter = new HeightStructuresImporter(failureMechanism.HeightStructures,
                                                                  referenceLine, filePath, messageProvider,
                                                                  replaceDataStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string message = CreateExpectedErrorMessage(
                csvFilePath, "Coupure Den Oever (90k1)", "KUNST1",
                new[]
                {
                    "De waarde voor parameter 'KW_HOOGTE1' op regel 2, kolom 'Numeriekewaarde', moet in het bereik [0,0, 360,0] liggen.",
                    "Parameter 'KW_HOOGTE2' komt meerdere keren voor."
                });
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
            Assert.AreEqual(0, failureMechanism.HeightStructures.Count);
            Assert.IsNull(failureMechanism.HeightStructures.SourcePath);
        }

        [Test]
        public void Import_MissingParameters_LogWarningAndContinueImportWithDefaultValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, nameof(HeightStructuresImporter), "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var replaceDataStrategy = new HeightStructureReplaceDataStrategy(failureMechanism);
            var structuresImporter = new HeightStructuresImporter(failureMechanism.HeightStructures,
                                                                  referenceLine, filePath, messageProvider,
                                                                  replaceDataStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(4, messages.Length);

                const string structure = "'Coupure Den Oever (90k1)' (KUNST1)";

                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_HOOGTE1' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[0]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_HOOGTE3' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[1]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_HOOGTE6' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[2]);
                // Don't care about the other message.
            });
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, failureMechanism.HeightStructures.Count);
            HeightStructure importedStructure = failureMechanism.HeightStructures.First();
            var defaultStructure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "test",
                Location = new Point2D(0, 0),
                Id = "id"
            });
            Assert.AreEqual(defaultStructure.StructureNormalOrientation, importedStructure.StructureNormalOrientation);
            DistributionAssert.AreEqual(defaultStructure.FlowWidthAtBottomProtection, importedStructure.FlowWidthAtBottomProtection);
            Assert.AreEqual(defaultStructure.FailureProbabilityStructureWithErosion, importedStructure.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(filePath, failureMechanism.HeightStructures.SourcePath);
        }

        [Test]
        public void Import_ParameterIdsWithVaryingCase_TrueAndImportTargetUpdated()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpRandomCaseHeaderCsv",
                                           "Kunstwerken.shp");

            var referencePoints = new List<Point2D>
            {
                new Point2D(154493.618, 568995.991),
                new Point2D(156844.169, 574771.498),
                new Point2D(157910.502, 579115.458),
                new Point2D(163625.153, 585151.261)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var failureMechanism = new HeightStructuresFailureMechanism();
            var replaceDataStrategy = new HeightStructureReplaceDataStrategy(failureMechanism);
            var structuresImporter = new HeightStructuresImporter(failureMechanism.HeightStructures,
                                                                  referenceLine, filePath, messageProvider,
                                                                  replaceDataStrategy);

            // Call
            bool importResult = structuresImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(4, failureMechanism.HeightStructures.Count);
            Assert.AreEqual(filePath, failureMechanism.HeightStructures.SourcePath);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingShapeFile_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Kunstwerken")).Return(cancelledLogMessage);

            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(commonIoTestDataPath, "CorrectFiles", "Kunstwerken.shp");

            var referenceLine = new ReferenceLine();
            var failureMechanism = new HeightStructuresFailureMechanism();

            var importer = new HeightStructuresImporter(failureMechanism.HeightStructures, referenceLine, validFilePath, messageProvider, strategy);
            importer.SetProgressChanged(delegate(string description, int step, int steps)
            {
                if (description.Contains("Inlezen van kunstwerklocaties uit een shapebestand."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingStructureData_CancelsImportAndLogs()
        {
            // Setup
            const string cancelledLogMessage = "Operation Cancelled";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText("Kunstwerken")).Return(cancelledLogMessage);

            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            mocks.ReplayAll();

            string validFilePath = Path.Combine(commonIoTestDataPath, "CorrectFiles", "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();
            var failureMechanism = new HeightStructuresFailureMechanism();

            var importer = new HeightStructuresImporter(failureMechanism.HeightStructures, referenceLine, validFilePath, messageProvider, strategy);
            importer.SetProgressChanged(delegate(string description, int step, int steps)
            {
                if (description.Contains("Inlezen van kunstwerkgegevens uit een kommagescheiden bestand."))
                {
                    importer.Cancel();
                }
            });

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(cancelledLogMessage, LogLevelConstant.Info);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ImportCompletedSuccessfullyNonetheless()
        {
            // Setup
            const string progressText = "ProgressText";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(progressText);
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();

            ReferenceLine referenceLine = CreateReferenceLine();

            string validFilePath = Path.Combine(testDataPath, "HeightStructuresImporter", "Kunstwerken.shp");

            var strategy = new TestStructureUpdateStrategy();

            var importer = new HeightStructuresImporter(failureMechanism.HeightStructures, referenceLine,
                                                        validFilePath, messageProvider, strategy);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(progressText))
                {
                    importer.Cancel();
                }
            });

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            Tuple<string, LogLevelConstant> expectedLogMessage = Tuple.Create(
                "Huidige actie was niet meer te annuleren en is daarom voortgezet.", LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedLogMessage);
            Assert.IsTrue(importResult);
            Assert.IsTrue(strategy.Updated);
        }

        private class TestStructureUpdateStrategy : IStructureUpdateStrategy<HeightStructure>
        {
            public bool Updated { get; private set; }
            public string FilePath { get; private set; }
            public HeightStructure[] ReadHeightStructures { get; private set; }
            public IEnumerable<IObservable> UpdatedInstances { get; } = Enumerable.Empty<IObservable>();

            public IEnumerable<IObservable> UpdateStructuresWithImportedData(
                StructureCollection<HeightStructure> targetDataCollection,
                IEnumerable<HeightStructure> readStructures, string sourceFilePath)
            {
                Updated = true;
                FilePath = sourceFilePath;
                ReadHeightStructures = readStructures.ToArray();
                return UpdatedInstances;
            }
        }

        private static ReferenceLine CreateReferenceLine()
        {
            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            return referenceLine;
        }

        private static string CreateExpectedErrorMessage(string filePath, string structureName,
                                                         string structureId,
                                                         IEnumerable<string> messages)
        {
            return string.Format("Fout bij het lezen van bestand '{0}' (Kunstwerk '{1}' ({2})): klik op details voor meer informatie." + Environment.NewLine
                                 + "Er zijn één of meerdere fouten gevonden waardoor dit kunstwerk niet ingelezen kan worden:" + Environment.NewLine + "{3}",
                                 filePath, structureName, structureId,
                                 string.Join(Environment.NewLine, messages.Select(msg => "* " + msg)));
        }
    }
}