﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
            string[] expectedMessages =
            {
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST2'.",
                "Kunstwerk nummer 2 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST3'.",
                "Kunstwerk nummer 3 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST4'.",
                "Kunstwerk nummer 4 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST5'.",
                "Kunstwerk nummer 5 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST6'.",
                "Kunstwerk nummer 6 wordt overgeslagen."
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
                "De variatie voor parameter 'KW_STERSTAB3' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 27).",
                "De variatie voor parameter 'KW_STERSTAB4' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 28).",
                "De variatie voor parameter 'KW_STERSTAB5' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 29).",
                "De variatie voor parameter 'KW_STERSTAB6' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 30).",
                "De variatie voor parameter 'KW_STERSTAB7' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 31).",
                "De variatie voor parameter 'KW_STERSTAB8' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 32).",
                "De variatie voor parameter 'KW_STERSTAB9' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 33).",
                "De variatie voor parameter 'KW_STERSTAB10' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 34).",
                "De variatie voor parameter 'KW_STERSTAB11' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 35).",
                "De variatie voor parameter 'KW_STERSTAB12' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 36).",
                "De variatie voor parameter 'KW_STERSTAB14' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 38).",
                "De variatie voor parameter 'KW_STERSTAB17' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 41).",
                "De variatie voor parameter 'KW_STERSTAB18' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 42).",
                "De variatie voor parameter 'KW_STERSTAB19' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 43).",
                "De variatie voor parameter 'KW_STERSTAB22' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 46).",
                "De variatie voor parameter 'KW_STERSTAB23' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 47).",
                "De variatie voor parameter 'KW_STERSTAB24' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een variatiecoëfficiënt (regel 48).",
                "De variatie voor parameter 'KW_STERSTAB25' van kunstwerk 'Coupure Den Oever (90k1)' (KUNST1) wordt omgerekend in een standaard deviatie (regel 49)."
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
            Assert.AreEqual(2, structure.ConstructiveStrengthLinearModel.CoefficientOfVariation.Value);
            Assert.AreEqual(2.5, structure.ConstructiveStrengthQuadraticModel.CoefficientOfVariation.Value);
            Assert.AreEqual(10, structure.BankWidth.StandardDeviation.Value);
            Assert.AreEqual(12, structure.InsideWaterLevelFailureConstruction.StandardDeviation.Value);
            Assert.AreEqual(14, structure.LevelCrestStructure.StandardDeviation.Value);
            Assert.AreEqual(3.5, structure.FailureCollisionEnergy.CoefficientOfVariation.Value);
            Assert.AreEqual(4, structure.ShipMass.CoefficientOfVariation.Value);
            Assert.AreEqual(4.5, structure.ShipVelocity.CoefficientOfVariation.Value);
            Assert.AreEqual(18, structure.FlowVelocityStructureClosable.StandardDeviation.Value);
            Assert.AreEqual(5, structure.StabilityLinearModel.CoefficientOfVariation.Value);
            Assert.AreEqual(5.5, structure.StabilityQuadraticModel.CoefficientOfVariation.Value);
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
            string[] expectedMessages =
            {
                "Parameter 'KW_STERSTAB9' komt meermaals voor.",
                "De waarde op regel 37, kolom 'NumeriekeWaarde' is ongeldig.",
                "Kunstwerk nummer 1 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST2'.",
                "Kunstwerk nummer 2 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST3'.",
                "Kunstwerk nummer 3 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST4'.",
                "Kunstwerk nummer 4 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST5'.",
                "Kunstwerk nummer 5 wordt overgeslagen.",
                "Kan geen geldige gegevens vinden voor kunstwerklocatie met KWKIDENT 'KUNST6'.",
                "Kunstwerk nummer 6 wordt overgeslagen."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages);
            Assert.IsTrue(importResult);
            Assert.AreEqual(0, importTarget.Count);
        }
    }
}