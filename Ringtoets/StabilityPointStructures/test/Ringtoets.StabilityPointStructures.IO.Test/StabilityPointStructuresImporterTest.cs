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
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.IO.Test
{
    [TestFixture]
    public class StabilityPointStructuresImporterTest
    {
        private readonly ObservableList<StabilityPointStructure> testImportTarget = new ObservableList<StabilityPointStructure>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var importer = new StabilityPointStructuresImporter(testImportTarget, testReferenceLine, testFilePath);

            // Assert
            Assert.IsInstanceOf<StructuresImporter<ObservableList<StabilityPointStructure>>>(importer);
        }

        [Test]
        public void Import_ValidIncompleteFile_LogAndTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var importTarget = new ObservableList<StabilityPointStructure>();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string[] expectedMessages =
            {
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Leemans (93k3)", "KUNST2",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST2'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Lely (93k4)", "KUNST3",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST3'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal de Stontele (94k1)", "KUNST4",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST4'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerkeersluis (93k1)", "KUNST5",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST5'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerschutsluis (93k2)", "KUNST6",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST6'."
                                           })
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, importTarget.Count);
        }

        [Test]
        public void Import_VarianceValuesNeedConversion_WarnUserAboutConversion()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityPointStructures.IO,
                                                         Path.Combine("StructuresVarianceValueConversion", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var importTarget = new ObservableList<StabilityPointStructure>();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string[] expectedMessages =
            {
                "De variatie voor parameter 'KW_STERSTAB2' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 26).",
                "De variatie voor parameter 'KW_STERSTAB3' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 27).",
                "De variatie voor parameter 'KW_STERSTAB4' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 28).",
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
                "De variatie voor parameter 'KW_STERSTAB22' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 46).",
                "De variatie voor parameter 'KW_STERSTAB23' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 47).",
                "De variatie voor parameter 'KW_STERSTAB24' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 48).",
                "De variatie voor parameter 'KW_STERSTAB25' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaardafwijking (regel 49)."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);

            Assert.AreEqual(1, importTarget.Count);
            StabilityPointStructure structure = importTarget[0];
            Assert.AreEqual(0.5, structure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(2, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(1, structure.WidthFlowApertures.CoefficientOfVariation.Value);
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
            Assert.AreEqual(18, structure.FlowVelocityStructureClosable.StandardDeviation.Value);
            Assert.AreEqual(5, structure.StabilityLinearLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(5.5, structure.StabilityQuadraticLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(22, structure.AreaFlowApertures.StandardDeviation.Value);
        }

        [Test]
        public void Import_InvalidCsvFile_LogAndTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectShpIncompleteCsv", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var importTarget = new ObservableList<StabilityPointStructure>();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            bool importResult = false;
            Action call = () => importResult = structuresImporter.Import();

            // Assert
            string csvFilePath = Path.ChangeExtension(filePath, "csv");
            string[] expectedMessages =
            {
                CreateExpectedErrorMessage(csvFilePath, "Coupure Den Oever (90k1)", "KUNST1",
                                           new[]
                                           {
                                               "Parameter 'KW_STERSTAB9' komt meerdere keren voor.",
                                               "De waarde voor parameter 'KW_STERSTAB10' op regel 37, kolom 'numeriekewaarde', moet een getal zijn dat niet negatief is."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Leemans (93k3)", "KUNST2",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST2'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal Lely (93k4)", "KUNST3",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST3'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Gemaal de Stontele (94k1)", "KUNST4",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST4'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerkeersluis (93k1)", "KUNST5",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST5'."
                                           }),
                CreateExpectedErrorMessage(csvFilePath, "Stontelerschutsluis (93k2)", "KUNST6",
                                           new[]
                                           {
                                               "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST6'."
                                           })
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.AreEqual(0, importTarget.Count);
        }

        [Test]
        public void Import_ParameterIdsWithVaryingCase_TrueAndImportTargetUpdated()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectShpRandomCaseHeaderCsv", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(154493.618,568995.991),
                new Point2D(156844.169,574771.498),
                new Point2D(157910.502,579115.458),
                new Point2D(163625.153,585151.261)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var importTarget = new ObservableList<StabilityPointStructure>();
            var structuresImporter = new StabilityPointStructuresImporter(importTarget, referenceLine, filePath);

            // Call
            bool importResult = structuresImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(4, importTarget.Count);
        }

        private static string CreateExpectedErrorMessage(string filePath, string structureName, string structureId,
                                                         IEnumerable<string> messages)
        {
            return string.Format("Fout bij het lezen van bestand '{0}' (Kunstwerk '{1}' ({2})): Er zijn één of meerdere fouten gevonden waardoor dit kunstwerk niet ingelezen kan worden:" + Environment.NewLine +
                                 "{3}",
                                 filePath, structureName, structureId,
                                 string.Join(Environment.NewLine, messages.Select(msg => "* " + msg)));
        }
    }
}