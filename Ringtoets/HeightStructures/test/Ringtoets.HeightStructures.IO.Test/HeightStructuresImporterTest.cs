// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.IO.Test
{
    [TestFixture]
    public class HeightStructuresImporterTest
    {
        private readonly ObservableList<HeightStructure> testImportTarget = new ObservableList<HeightStructure>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var importer = new HeightStructuresImporter(testImportTarget, testReferenceLine, testFilePath);

            // Assert
            Assert.IsInstanceOf<StructuresImporter<ObservableList<HeightStructure>>>(importer);
        }

        [Test]
        public void Import_ValidIncompleteFile_LogAndTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();

            var importTarget = new ObservableList<HeightStructure>();
            var structuresImporter = new HeightStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string[] expectedSubMessages =
            {
                "Geen geldige parameter definities gevonden."
            };
            string[] expectedMessages =
            {
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Leemans (93k3)", "KUNST2", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Lely (93k4)", "KUNST3", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal de Stontele (94k1)", "KUNST4", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerkeersluis (93k1)", "KUNST5", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerschutsluis (93k2)", "KUNST6", expectedSubMessages)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, importTarget.Count);
        }

        [Test]
        public void Import_ValidFileWithConversionsBetweenVarianceTypes_WarnUserAboutConversion()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO,
                                                         Path.Combine("HeightStructuresVarianceConvert", "StructureNeedVarianceValueConversion.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();

            var importTarget = new ObservableList<HeightStructure>();
            var structuresImporter = new HeightStructuresImporter(importTarget, referenceLine, filePath);

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
            Assert.AreEqual(1, importTarget.Count);
            HeightStructure structure = importTarget[0];
            Assert.AreEqual(0.12, structure.LevelCrestStructure.StandardDeviation.Value);
            Assert.AreEqual(0.24, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(1.0, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(0.97, structure.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(1.84, structure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(2.18, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_InvalidCsvFile_LogAndTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectShpIncompleteCsv", "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();

            var importTarget = new ObservableList<HeightStructure>();
            var structuresImporter = new HeightStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string[] expectedSubMessages =
            {
                "Geen geldige parameter definities gevonden.",
            };
            string[] expectedMessages =
            {
                CreateExpectedErrorMessage(csvFilePath, "Coupure Den Oever (90k1)", "KUNST1",
                                           new[]
                                           {
                                               "De waarde voor parameter 'KW_HOOGTE1' op regel 2, kolom 'Numeriekewaarde', moet in het bereik [0,0, 360,0] liggen.",
                                               "Parameter 'KW_HOOGTE2' komt meerdere keren voor."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Leemans (93k3)", "KUNST2", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Lely (93k4)", "KUNST3", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal de Stontele (94k1)", "KUNST4", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerkeersluis (93k1)", "KUNST5", expectedSubMessages),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerschutsluis (93k2)", "KUNST6", expectedSubMessages)
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.AreEqual(0, importTarget.Count);
        }

        [Test]
        public void Import_MissingParameters_LogWarningAndContinueImportWithDefaultValues()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO,
                                                         Path.Combine(nameof(HeightStructuresImporter), "Kunstwerken.shp"));

            ReferenceLine referenceLine = CreateReferenceLine();

            var importTarget = new ObservableList<HeightStructure>();
            var structuresImporter = new HeightStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            var importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(4, messages.Length);

                Assert.AreEqual("Geen geldige definitie gevonden voor parameter 'KW_HOOGTE1'. Er wordt een standaard waarde gebruikt.", messages[0]);
                Assert.AreEqual("Geen geldige definitie gevonden voor parameter 'KW_HOOGTE3'. Er wordt een standaard waarde gebruikt.", messages[1]);
                Assert.AreEqual("Geen geldige definitie gevonden voor parameter 'KW_HOOGTE6'. Er wordt een standaard waarde gebruikt.", messages[2]);
                // Don't care about the other message.
            });
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, importTarget.Count);
            HeightStructure importedStructure = importTarget.First();
            var defaultStructure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "test",
                Location = new Point2D(0, 0),
                Id = "id"
            });
            Assert.AreEqual(defaultStructure.StructureNormalOrientation, importedStructure.StructureNormalOrientation);
            DistributionAssert.AreEqual(defaultStructure.FlowWidthAtBottomProtection, importedStructure.FlowWidthAtBottomProtection);
            Assert.AreEqual(defaultStructure.FailureProbabilityStructureWithErosion, importedStructure.FailureProbabilityStructureWithErosion);
        }

        [Test]
        public void Import_ParameterIdsWithVaryingCase_TrueAndImportTargetUpdated()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectShpRandomCaseHeaderCsv", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(154493.618, 568995.991),
                new Point2D(156844.169, 574771.498),
                new Point2D(157910.502, 579115.458),
                new Point2D(163625.153, 585151.261)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var importTarget = new ObservableList<HeightStructure>();
            var structuresImporter = new HeightStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            bool importResult = structuresImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(4, importTarget.Count);
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
            return string.Format("Fout bij het lezen van bestand '{0}' (Kunstwerk '{1}' ({2})): klik op details voor meer informatie." + Environment.NewLine
                                 + "Er zijn één of meerdere fouten gevonden waardoor dit kunstwerk niet ingelezen kan worden:" + Environment.NewLine + "{3}",
                                 filePath, structureName, structureId,
                                 string.Join(Environment.NewLine, messages.Select(msg => "* " + msg)));
        }
    }
}