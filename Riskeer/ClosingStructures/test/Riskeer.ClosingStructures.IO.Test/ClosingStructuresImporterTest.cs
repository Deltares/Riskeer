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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Structures;

namespace Riskeer.ClosingStructures.IO.Test
{
    [TestFixture]
    public class ClosingStructuresImporterTest
    {
        private readonly string commonIoTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "Structures");
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.ClosingStructures.IO);

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
        public void Constructor_WithoutUpdateStrategy_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ClosingStructuresImporter(
                new StructureCollection<ClosingStructure>(),
                new ReferenceLine(),
                string.Empty,
                messageProvider,
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structureUpdateStrategy", exception.ParamName);
        }

        [Test]
        public void Constructor_WithUpdateStrategy_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<ClosingStructure>>();
            mocks.ReplayAll();

            // Call
            var importer = new ClosingStructuresImporter(
                new StructureCollection<ClosingStructure>(),
                new ReferenceLine(),
                string.Empty,
                messageProvider,
                updateStrategy);

            // Assert
            Assert.IsInstanceOf<StructuresImporter<ClosingStructure>>(importer);
        }

        [Test]
        public void Import_ValidIncompleteFile_LogAndFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<ClosingStructure>>();
            mocks.ReplayAll();

            string filePath = Path.Combine(commonIoTestDataPath, "CorrectFiles", "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<ClosingStructure>();
            var importer = new ClosingStructuresImporter(importTarget, referenceLine, filePath,
                                                         messageProvider, updateStrategy);

            // Call
            var importResult = false;
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
        public void Import_VarianceValuesNeedConversion_WarnUserAboutConversion()
        {
            // Setup
            var importTarget = new StructureCollection<ClosingStructure>();
            string filePath = Path.Combine(testDataPath, "StructuresVarianceValueConversion",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStructureUpdateStrategy<ClosingStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);

                var closingStructures = (IEnumerable<ClosingStructure>) i.Arguments[0];
                Assert.AreEqual(1, closingStructures.Count());

                ClosingStructure structure = closingStructures.First();
                Assert.AreEqual(0.2, structure.StorageStructureArea.CoefficientOfVariation.Value);
                Assert.AreEqual(20, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
                Assert.AreEqual(50, structure.WidthFlowApertures.StandardDeviation.Value);
                Assert.AreEqual(2.2, structure.LevelCrestStructureNotClosing.StandardDeviation.Value);
                Assert.AreEqual(3.3, structure.InsideWaterLevel.StandardDeviation.Value);
                Assert.AreEqual(4.4, structure.ThresholdHeightOpenWeir.StandardDeviation.Value);
                Assert.AreEqual(5.5, structure.AreaFlowApertures.StandardDeviation.Value);
                Assert.AreEqual(0.1, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
                Assert.AreEqual(6.6, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            });
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new ClosingStructuresImporter(importTarget, referenceLine, filePath,
                                                         messageProvider, updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = importer.Import();

            // Assert
            string[] expectedMessages =
            {
                "De variatie voor parameter 'KW_BETSLUIT1' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 10).",
                "De variatie voor parameter 'KW_BETSLUIT2' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 11).",
                "De variatie voor parameter 'KW_BETSLUIT4' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 13).",
                "De variatie voor parameter 'KW_BETSLUIT5' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 14).",
                "De variatie voor parameter 'KW_BETSLUIT6' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 15).",
                "De variatie voor parameter 'KW_BETSLUIT7' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 16).",
                "De variatie voor parameter 'KW_BETSLUIT8' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 17).",
                "De variatie voor parameter 'KW_BETSLUIT9' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 18).",
                "De variatie voor parameter 'KW_BETSLUIT10' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 19)."
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
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<ClosingStructure>>();
            mocks.ReplayAll();
            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpIncompleteCsv",
                                           "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<ClosingStructure>();
            var structuresImporter = new ClosingStructuresImporter(importTarget, referenceLine,
                                                                   filePath, messageProvider, updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string message = CreateExpectedErrorMessage(
                csvFilePath, "Coupure Den Oever (90k1)", "KUNST1",
                new[]
                {
                    "De waarde voor parameter 'KW_BETSLUIT3' op regel 13, kolom 'Numeriekewaarde', moet in het bereik [0,0, 360,0] liggen.",
                    "Parameter 'KW_BETSLUIT5' komt meerdere keren voor."
                });
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error), 1
            );
            Assert.IsFalse(importResult);
            Assert.AreEqual(0, importTarget.Count);
            Assert.IsNull(importTarget.SourcePath);
        }

        [Test]
        public void Import_ParameterIdsWithVaryingCase_TrueAndImportTargetUpdated()
        {
            // Setup
            var importTarget = new StructureCollection<ClosingStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpRandomCaseHeaderCsv",
                                           "Kunstwerken.shp");
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStructureUpdateStrategy<ClosingStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);
                Assert.AreEqual(4, ((IEnumerable<ClosingStructure>) i.Arguments[0]).Count());
            });
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
            var structuresImporter = new ClosingStructuresImporter(importTarget, referenceLine,
                                                                   filePath, messageProvider, updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 5);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_MissingParameters_LogWarningAndContinueImportWithDefaultValues()
        {
            // Setup
            var importTarget = new StructureCollection<ClosingStructure>();
            string filePath = Path.Combine(testDataPath, nameof(ClosingStructuresImporter),
                                           "MissingParameters", "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStructureUpdateStrategy<ClosingStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);

                var defaultStructure = new ClosingStructure(new ClosingStructure.ConstructionProperties
                {
                    Name = "test",
                    Location = new Point2D(0, 0),
                    Id = "id"
                });

                var readStructures = (IEnumerable<ClosingStructure>) i.Arguments[0];
                Assert.AreEqual(1, readStructures.Count());
                ClosingStructure importedStructure = readStructures.First();
                DistributionAssert.AreEqual(defaultStructure.StorageStructureArea, importedStructure.StorageStructureArea);
                DistributionAssert.AreEqual(defaultStructure.LevelCrestStructureNotClosing, importedStructure.LevelCrestStructureNotClosing);
                DistributionAssert.AreEqual(defaultStructure.AreaFlowApertures, importedStructure.AreaFlowApertures);
                Assert.AreEqual(defaultStructure.FailureProbabilityReparation, importedStructure.FailureProbabilityReparation);
            });
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();

            var structuresImporter = new ClosingStructuresImporter(importTarget, referenceLine,
                                                                   filePath, messageProvider, updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(10, messages.Length);

                const string structure = "'Coupure Den Oever (90k1)' (KUNST1)";

                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT1' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[0]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT5' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[3]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT8' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[6]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT14' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[8]);
                // Don't care about the other messages.
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_MissingParametersAndDuplicateIrrelevantParameter_LogWarningAndContinueImportWithDefaultValues()
        {
            // Setup
            var importTarget = new StructureCollection<ClosingStructure>();
            string filePath = Path.Combine(testDataPath, nameof(ClosingStructuresImporter),
                                           "MissingAndDuplicateIrrelevantParameters", "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStructureUpdateStrategy<ClosingStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);

                var defaultStructure = new ClosingStructure(new ClosingStructure.ConstructionProperties
                {
                    Name = "test",
                    Location = new Point2D(0, 0),
                    Id = "id"
                });

                var readStructures = (IEnumerable<ClosingStructure>) i.Arguments[0];
                Assert.AreEqual(1, readStructures.Count());
                ClosingStructure importedStructure = readStructures.First();
                DistributionAssert.AreEqual(defaultStructure.StorageStructureArea, importedStructure.StorageStructureArea);
                DistributionAssert.AreEqual(defaultStructure.LevelCrestStructureNotClosing, importedStructure.LevelCrestStructureNotClosing);
                DistributionAssert.AreEqual(defaultStructure.AreaFlowApertures, importedStructure.AreaFlowApertures);
                Assert.AreEqual(defaultStructure.FailureProbabilityReparation, importedStructure.FailureProbabilityReparation);
            });
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();

            var structuresImporter = new ClosingStructuresImporter(importTarget, referenceLine,
                                                                   filePath, messageProvider, updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(10, messages.Length);

                const string structure = "'Coupure Den Oever (90k1)' (KUNST1)";

                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT1' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[0]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT5' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[3]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT8' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[6]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_BETSLUIT14' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[8]);
                // Don't care about the other messages.
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_AllParameterIdsDefinedAndDuplicateUnknownParameterId_TrueAndImportTargetUpdated()
        {
            // Setup
            var importTarget = new StructureCollection<ClosingStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "StructuresWithDuplicateIrrelevantParameterInCsv",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<ClosingStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(invocation.Arguments[1], filePath);

                        var readStructures = (IEnumerable<ClosingStructure>) invocation.Arguments[0];
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

            var importer = new ClosingStructuresImporter(importTarget, referenceLine, filePath,
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
            var importTarget = new StructureCollection<ClosingStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "StructuresWithOnlyDuplicateIrrelevantParameterInCsv",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<ClosingStructure>>();
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

            var structuresImporter = new ClosingStructuresImporter(importTarget,
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

            var strategy = mocks.StrictMock<IStructureUpdateStrategy<ClosingStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().Return(observables);
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, nameof(ClosingStructuresImporter),
                                           "MissingParameters", "Kunstwerken.shp");

            var importTarget = new StructureCollection<ClosingStructure>();
            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new ClosingStructuresImporter(importTarget, referenceLine, filePath,
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

        private static string CreateExpectedErrorMessage(string filePath, string structureName, string structureId,
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