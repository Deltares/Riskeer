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
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.StabilityPointStructures.IO.Test
{
    [TestFixture]
    public class StabilityPointStructuresImporterTest
    {
        private readonly string commonIoTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "Structures");
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.StabilityPointStructures.IO);

        private readonly StructureCollection<StabilityPointStructure> testImportTarget = new StructureCollection<StabilityPointStructure>();
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
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<StabilityPointStructure>>();
            mocks.ReplayAll();

            // Call
            var importer = new StabilityPointStructuresImporter(testImportTarget,
                                                                testReferenceLine,
                                                                testFilePath,
                                                                messageProvider,
                                                                updateStrategy);

            // Assert
            Assert.IsInstanceOf<StructuresImporter<StabilityPointStructure>>(importer);
        }

        [Test]
        public void Import_ValidIncompleteFile_LogAndFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<StabilityPointStructure>>();
            mocks.ReplayAll();

            string filePath = Path.Combine(commonIoTestDataPath, "CorrectFiles", "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<StabilityPointStructure>();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget,
                                                                          referenceLine,
                                                                          filePath,
                                                                          messageProvider,
                                                                          updateStrategy);

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
            Assert.AreEqual(0, importTarget.Count);
        }

        [Test]
        public void Import_VarianceValuesNeedConversion_WarnUserAboutConversion()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "StructuresVarianceValueConversion", "Kunstwerken.shp");

            var importTarget = new StructureCollection<StabilityPointStructure>();

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.StrictMock<IStructureUpdateStrategy<StabilityPointStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);

                var structures = (IEnumerable<StabilityPointStructure>) i.Arguments[0];

                Assert.AreEqual(1, structures.Count());
                StabilityPointStructure structure = structures.First();
                Assert.AreEqual(0.5, structure.StorageStructureArea.CoefficientOfVariation.Value);
                Assert.AreEqual(2, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
                Assert.AreEqual(4, structure.WidthFlowApertures.StandardDeviation.Value);
                Assert.AreEqual(4, structure.InsideWaterLevel.StandardDeviation.Value);
                Assert.AreEqual(6, structure.ThresholdHeightOpenWeir.StandardDeviation.Value);
                Assert.AreEqual(1.5, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
                Assert.AreEqual(8, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
                Assert.AreEqual(2, structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.Value);
                Assert.AreEqual(2.5, structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.Value);
                Assert.AreEqual(10, structure.BankWidth.StandardDeviation.Value);
                Assert.AreEqual(12, structure.InsideWaterLevelFailureConstruction.StandardDeviation.Value);
                Assert.AreEqual(14, structure.LevelCrestStructure.StandardDeviation.Value);
                Assert.AreEqual(3.5, structure.FailureCollisionEnergy.CoefficientOfVariation.Value);
                Assert.AreEqual(4, structure.ShipMass.CoefficientOfVariation.Value);
                Assert.AreEqual(4.5, structure.ShipVelocity.CoefficientOfVariation.Value);
                Assert.AreEqual(5, structure.StabilityLinearLoadModel.CoefficientOfVariation.Value);
                Assert.AreEqual(5.5, structure.StabilityQuadraticLoadModel.CoefficientOfVariation.Value);
                Assert.AreEqual(22, structure.AreaFlowApertures.StandardDeviation.Value);
            });
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget,
                                                                          referenceLine,
                                                                          filePath,
                                                                          messageProvider,
                                                                          updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                "De variatie voor parameter 'KW_STERSTAB2' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 26).",
                "De variatie voor parameter 'KW_STERSTAB3' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 27).",
                "De variatie voor parameter 'KW_STERSTAB4' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 28).",
                "De variatie voor parameter 'KW_STERSTAB5' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 29).",
                "De variatie voor parameter 'KW_STERSTAB6' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 30).",
                "De variatie voor parameter 'KW_STERSTAB7' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 31).",
                "De variatie voor parameter 'KW_STERSTAB8' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 32).",
                "De variatie voor parameter 'KW_STERSTAB9' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 33).",
                "De variatie voor parameter 'KW_STERSTAB10' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 34).",
                "De variatie voor parameter 'KW_STERSTAB11' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 35).",
                "De variatie voor parameter 'KW_STERSTAB12' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 36).",
                "De variatie voor parameter 'KW_STERSTAB14' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 38).",
                "De variatie voor parameter 'KW_STERSTAB17' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 41).",
                "De variatie voor parameter 'KW_STERSTAB18' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 42).",
                "De variatie voor parameter 'KW_STERSTAB19' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 43).",
                "De variatie voor parameter 'KW_STERSTAB23' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 47).",
                "De variatie voor parameter 'KW_STERSTAB24' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 48).",
                "De variatie voor parameter 'KW_STERSTAB25' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 49)."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_InvalidCsvFile_LogAndTrue()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<StabilityPointStructure>>();
            mocks.ReplayAll();

            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpIncompleteCsv", "Kunstwerken.shp");

            ReferenceLine referenceLine = CreateReferenceLine();
            var importTarget = new StructureCollection<StabilityPointStructure>();

            var structuresImporter = new StabilityPointStructuresImporter(importTarget,
                                                                          referenceLine,
                                                                          filePath,
                                                                          messageProvider,
                                                                          updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string message = CreateExpectedErrorMessage(
                csvFilePath, "Coupure Den Oever (90k1)", "KUNST1",
                new[]
                {
                    "Parameter 'KW_STERSTAB9' komt meerdere keren voor.",
                    "De waarde voor parameter 'KW_STERSTAB10' op regel 37, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0."
                });
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
            Assert.AreEqual(0, importTarget.Count);
        }

        [Test]
        public void Import_ParameterIdsWithVaryingCase_TrueAndImportTargetUpdated()
        {
            // Setup
            string filePath = Path.Combine(commonIoTestDataPath, "CorrectShpRandomCaseHeaderCsv", "Kunstwerken.shp");

            var importTarget = new StructureCollection<StabilityPointStructure>();

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<StabilityPointStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);
                Assert.AreEqual(4, ((IEnumerable<StabilityPointStructure>) i.Arguments[0]).Count());
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

            var structuresImporter = new StabilityPointStructuresImporter(importTarget,
                                                                          referenceLine,
                                                                          filePath,
                                                                          messageProvider,
                                                                          updateStrategy);

            // Call
            bool importResult = structuresImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_MissingParameters_LogWarningAndContinueImportWithDefaultValues()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, nameof(StabilityPointStructuresImporter),
                                           "MissingParameters", "Kunstwerken.shp");

            var importTarget = new StructureCollection<StabilityPointStructure>();

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<StabilityPointStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);

                var readStructures = (IEnumerable<StabilityPointStructure>) i.Arguments[0];

                Assert.AreEqual(1, readStructures.Count());
                StabilityPointStructure importedStructure = readStructures.First();
                var defaultStructure = new StabilityPointStructure(new StabilityPointStructure.ConstructionProperties
                {
                    Name = "test",
                    Location = new Point2D(0, 0),
                    Id = "id"
                });
                DistributionAssert.AreEqual(defaultStructure.StorageStructureArea, importedStructure.StorageStructureArea);
                DistributionAssert.AreEqual(defaultStructure.ThresholdHeightOpenWeir, importedStructure.ThresholdHeightOpenWeir);
                DistributionAssert.AreEqual(defaultStructure.InsideWaterLevelFailureConstruction, importedStructure.InsideWaterLevelFailureConstruction);
                Assert.AreEqual(defaultStructure.LevellingCount, importedStructure.LevellingCount);
                DistributionAssert.AreEqual(defaultStructure.AreaFlowApertures, importedStructure.AreaFlowApertures);
            });
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget,
                                                                          referenceLine,
                                                                          filePath,
                                                                          messageProvider,
                                                                          updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(11, messages.Length);

                const string structure = "'Coupure Den Oever (90k1)' (KUNST1)";

                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB2' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[0]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB6' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[4]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB12' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[7]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB20' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[9]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB25' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[10]);
                // Don't care about the other messages.
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_MissingParametersAndDuplicateIrrelevantParameter_LogWarningAndContinueImportWithDefaultValues()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, nameof(StabilityPointStructuresImporter),
                                           "MissingAndDuplicateIrrelevantParameters", "Kunstwerken.shp");

            var importTarget = new StructureCollection<StabilityPointStructure>();

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<StabilityPointStructure>>();
            updateStrategy.Expect(u => u.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().WhenCalled(i =>
            {
                Assert.AreEqual(filePath, i.Arguments[1]);

                var readStructures = (IEnumerable<StabilityPointStructure>) i.Arguments[0];

                Assert.AreEqual(1, readStructures.Count());
                StabilityPointStructure importedStructure = readStructures.First();
                var defaultStructure = new StabilityPointStructure(new StabilityPointStructure.ConstructionProperties
                {
                    Name = "test",
                    Location = new Point2D(0, 0),
                    Id = "id"
                });
                DistributionAssert.AreEqual(defaultStructure.StorageStructureArea, importedStructure.StorageStructureArea);
                DistributionAssert.AreEqual(defaultStructure.ThresholdHeightOpenWeir, importedStructure.ThresholdHeightOpenWeir);
                DistributionAssert.AreEqual(defaultStructure.InsideWaterLevelFailureConstruction, importedStructure.InsideWaterLevelFailureConstruction);
                Assert.AreEqual(defaultStructure.LevellingCount, importedStructure.LevellingCount);
                DistributionAssert.AreEqual(defaultStructure.AreaFlowApertures, importedStructure.AreaFlowApertures);
            });
            mocks.ReplayAll();

            ReferenceLine referenceLine = CreateReferenceLine();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget,
                                                                          referenceLine,
                                                                          filePath,
                                                                          messageProvider,
                                                                          updateStrategy);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(11, messages.Length);

                const string structure = "'Coupure Den Oever (90k1)' (KUNST1)";

                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB2' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[0]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB6' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[4]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB12' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[7]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB20' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[9]);
                Assert.AreEqual($"Geen definitie gevonden voor parameter 'KW_STERSTAB25' van kunstwerk {structure}. Er wordt een standaard waarde gebruikt.", messages[10]);
                // Don't care about the other messages.
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_AllParameterIdsDefinedAndDuplicateUnknownParameterId_TrueAndImportTargetUpdated()
        {
            // Setup
            var importTarget = new StructureCollection<StabilityPointStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "StructuresWithDuplicateIrrelevantParameterInCsv",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var strategy = mocks.StrictMock<IStructureUpdateStrategy<StabilityPointStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments()
                    .WhenCalled(invocation =>
                    {
                        Assert.AreSame(invocation.Arguments[1], filePath);

                        var readStructures = (IEnumerable<StabilityPointStructure>) invocation.Arguments[0];
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

            var importer = new StabilityPointStructuresImporter(importTarget, referenceLine, filePath,
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
            var importTarget = new StructureCollection<StabilityPointStructure>();
            string filePath = Path.Combine(commonIoTestDataPath, "StructuresWithOnlyDuplicateIrrelevantParameterInCsv",
                                           "Kunstwerken.shp");

            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var updateStrategy = mocks.Stub<IStructureUpdateStrategy<StabilityPointStructure>>();
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

            var structuresImporter = new StabilityPointStructuresImporter(importTarget,
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

            var strategy = mocks.StrictMock<IStructureUpdateStrategy<StabilityPointStructure>>();
            strategy.Expect(s => s.UpdateStructuresWithImportedData(null, null)).IgnoreArguments().Return(observables);
            mocks.ReplayAll();

            string filePath = Path.Combine(testDataPath, nameof(StabilityPointStructuresImporter),
                                           "MissingParameters", "Kunstwerken.shp")
                ;

            var importTarget = new StructureCollection<StabilityPointStructure>();
            ReferenceLine referenceLine = CreateReferenceLine();

            var importer = new StabilityPointStructuresImporter(importTarget, referenceLine, filePath,
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