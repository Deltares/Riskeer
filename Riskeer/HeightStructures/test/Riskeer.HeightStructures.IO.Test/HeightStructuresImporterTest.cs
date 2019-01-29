// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Structures;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Plugin.FileImporters;

namespace Riskeer.HeightStructures.IO.Test
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
            Assert.IsInstanceOf<StructuresImporter<HeightStructure>>(importer);
        }

        [Test]
        public void Import_ValidIncompleteFile_LogAndFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            mocks.ReplayAll();

            var importTarget = new StructureCollection<HeightStructure>();
            ReferenceLine referenceLine = CreateReferenceLine();
            string filePath = Path.Combine(commonIoTestDataPath, "CorrectFiles", "Kunstwerken.shp");

            var importer = new HeightStructuresImporter(importTarget, referenceLine, filePath,
                                                        messageProvider, strategy);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string message = CreateExpectedErrorMessage(csvFilePath, "Gemaal Leemans (93k3)", "KUNST2", new[]
            {
                "Geen geldige parameter definities gevonden."
            });
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error));
            Assert.IsFalse(importResult);
            Assert.AreEqual(0, importTarget.Count);
            Assert.IsNull(importTarget.SourcePath);
        }

        [Test]
        public void Import_ValidFileWithConversionsBetweenVarianceTypes_WarnUserAboutConversion()
        {
            // Setup
            var importTarget = new StructureCollection<HeightStructure>();
            string filePath = Path.Combine(testDataPath, "HeightStructuresVarianceConvert", "StructureNeedVarianceValueConversion.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(invocation.Arguments[1], filePath);

                        var readStructures = (IEnumerable<HeightStructure>) invocation.Arguments[0];
                        Assert.AreEqual(1, readStructures.Count());
                        HeightStructure structure = readStructures.First();
                        Assert.AreEqual(0.12, structure.LevelCrestStructure.StandardDeviation.Value);
                        Assert.AreEqual(0.24, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
                        Assert.AreEqual(1.0, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
                        Assert.AreEqual(0.97, structure.WidthFlowApertures.StandardDeviation.Value);
                        Assert.AreEqual(1.84, structure.StorageStructureArea.CoefficientOfVariation.Value);
                        Assert.AreEqual(2.18, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
                    })
                    .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new HeightStructuresImporter(importTarget, referenceLine, filePath,
                                                        messageProvider, strategy);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

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
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_InvalidCsvFile_LogAndFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            mocks.ReplayAll();

            var importTarget = new StructureCollection<HeightStructure>();
            ReferenceLine referenceLine = CreateReferenceLine();
            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpIncompleteCsv", "Kunstwerken.shp");

            var importer = new HeightStructuresImporter(importTarget, referenceLine, filePath,
                                                        messageProvider, strategy);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

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
        }

        [Test]
        public void Import_MissingParameters_LogWarningAndContinueImport()
        {
            // Setup
            var importTarget = new StructureCollection<HeightStructure>();
            string filePath = Path.Combine(testDataPath, nameof(HeightStructuresImporter),
                                           "MissingParameters", "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(invocation.Arguments[1], filePath);

                        var readStructures = (IEnumerable<HeightStructure>) invocation.Arguments[0];
                        Assert.AreEqual(1, readStructures.Count());
                        HeightStructure structure = readStructures.First();
                        var defaultStructure = new HeightStructure(new HeightStructure.ConstructionProperties
                        {
                            Name = "test",
                            Location = new Point2D(0, 0),
                            Id = "id"
                        });
                        Assert.AreEqual(defaultStructure.StructureNormalOrientation, structure.StructureNormalOrientation);
                        DistributionAssert.AreEqual(defaultStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
                        Assert.AreEqual(defaultStructure.FailureProbabilityStructureWithErosion, structure.FailureProbabilityStructureWithErosion);
                    })
                    .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new HeightStructuresImporter(importTarget, referenceLine, filePath,
                                                        messageProvider, strategy);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

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
        }

        [Test]
        public void Import_MissingParametersAndDuplicateIrrelevantParameter_LogWarningAndContinueImport()
        {
            // Setup
            var importTarget = new StructureCollection<HeightStructure>();
            string filePath = Path.Combine(testDataPath, nameof(HeightStructuresImporter),
                                           "MissingAndDuplicateIrrelevantParameters", "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(invocation.Arguments[1], filePath);

                        var readStructures = (IEnumerable<HeightStructure>) invocation.Arguments[0];
                        Assert.AreEqual(1, readStructures.Count());
                        HeightStructure structure = readStructures.First();
                        var defaultStructure = new HeightStructure(new HeightStructure.ConstructionProperties
                        {
                            Name = "test",
                            Location = new Point2D(0, 0),
                            Id = "id"
                        });
                        Assert.AreEqual(defaultStructure.StructureNormalOrientation, structure.StructureNormalOrientation);
                        DistributionAssert.AreEqual(defaultStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
                        Assert.AreEqual(defaultStructure.FailureProbabilityStructureWithErosion, structure.FailureProbabilityStructureWithErosion);
                    })
                    .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new HeightStructuresImporter(importTarget, referenceLine, filePath,
                                                        messageProvider, strategy);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

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
        }

        [Test]
        public void Import_ParameterIdsWithVaryingCase_TrueAndImportTargetUpdated()
        {
            // Setup
            var importTarget = new StructureCollection<HeightStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpRandomCaseHeaderCsv",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(invocation.Arguments[1], filePath);

                        var readStructures = (IEnumerable<HeightStructure>) invocation.Arguments[0];
                        Assert.AreEqual(4, readStructures.Count());
                    })
                    .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var referencePoints = new List<Point2D>
            {
                new Point2D(154493.618, 568995.991),
                new Point2D(156844.169, 574771.498),
                new Point2D(157910.502, 579115.458),
                new Point2D(163625.153, 585151.261)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var importer = new HeightStructuresImporter(importTarget, referenceLine, filePath,
                                                        messageProvider, strategy);

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_AllParameterIdsDefinedAndDuplicateUnknownParameterId_TrueAndImportTargetUpdated()
        {
            // Setup
            var importTarget = new StructureCollection<HeightStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "StructuresWithDuplicateIrrelevantParameterInCsv",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(invocation.Arguments[1], filePath);

                        var readStructures = (IEnumerable<HeightStructure>) invocation.Arguments[0];
                        Assert.AreEqual(1, readStructures.Count());
                    })
                    .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var referencePoints = new List<Point2D>
            {
                new Point2D(154493.618, 568995.991),
                new Point2D(156844.169, 574771.498),
                new Point2D(157910.502, 579115.458),
                new Point2D(163625.153, 585151.261)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var importer = new HeightStructuresImporter(importTarget, referenceLine, filePath,
                                                        messageProvider, strategy);

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_NoParameterIdsDefinedAndOnlyDuplicateUnknownParameterId_TrueAndImportTargetUpdated()
        {
            // Setup
            var importTarget = new StructureCollection<HeightStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "StructuresWithOnlyDuplicateIrrelevantParameterInCsv",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<HeightStructure>>();
            mocks.ReplayAll();

            var referencePoints = new List<Point2D>
            {
                new Point2D(154493.618, 568995.991),
                new Point2D(156844.169, 574771.498),
                new Point2D(157910.502, 579115.458),
                new Point2D(163625.153, 585151.261)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var structuresImporter = new HeightStructuresImporter(importTarget,
                                                                  referenceLine,
                                                                  filePath,
                                                                  messageProvider,
                                                                  updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string message = CreateExpectedErrorMessage(csvFilePath, "Eerste kunstwerk 6-3", "KWK_1", new[]
            {
                "Geen geldige parameter definities gevonden."
            });
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error));
            Assert.IsFalse(importResult);
            Assert.AreEqual(0, importTarget.Count);
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

            var importTarget = new StructureCollection<HeightStructure>();
            var referenceLine = new ReferenceLine();

            var importer = new HeightStructuresImporter(importTarget, referenceLine, validFilePath,
                                                        messageProvider, strategy);
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

            var importTarget = new StructureCollection<HeightStructure>();
            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new HeightStructuresImporter(importTarget, referenceLine, validFilePath,
                                                        messageProvider, strategy);
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
            string validFilePath = Path.Combine(testDataPath, nameof(HeightStructuresImporter),
                                                "MissingParameters", "Kunstwerken.shp");
            var importTarget = new StructureCollection<HeightStructure>();

            const string progressText = "ProgressText";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(progressText);

            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(Arg<IEnumerable<HeightStructure>>.Is.NotNull,
                                                                    Arg.Is(validFilePath)))
                    .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();
            var importer = new HeightStructuresImporter(importTarget, referenceLine, validFilePath,
                                                        messageProvider, strategy);
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
        }

        [Test]
        public void DoPostImport_UpdateStrategyReturningObservables_AllObservablesNotified()
        {
            var messageProvider = mocks.Stub<IImporterMessageProvider>();

            var observableA = mocks.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mocks.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());
            IObservable[] observables =
            {
                observableA,
                observableB
            };

            var strategy = mocks.StrictMock<IStructureUpdateStrategy<HeightStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().Return(observables);
            mocks.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, nameof(HeightStructuresImporter),
                                                "MissingParameters", "Kunstwerken.shp");

            var importTarget = new StructureCollection<HeightStructure>();
            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new HeightStructuresImporter(importTarget, referenceLine, validFilePath,
                                                        messageProvider, strategy);

            importer.Import();

            // Call
            importer.DoPostImport();

            // Assert
            // Assertions performed in TearDown
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
            return string.Format("Fout bij het lezen van bestand '{0}' (Kunstwerk '{1}' ({2})): klik op details voor meer informatie."
                                 + Environment.NewLine
                                 + "Er zijn één of meerdere fouten gevonden waardoor dit kunstwerk niet ingelezen kan worden:" + Environment.NewLine + "{3}",
                                 filePath, structureName, structureId,
                                 string.Join(Environment.NewLine, messages.Select(msg => "* " + msg)));
        }
    }
}